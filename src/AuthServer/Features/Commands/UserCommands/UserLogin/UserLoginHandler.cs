using System.Text.Json;
using AuthServer.Common;
using AuthServer.Data.Models;
using AuthServer.DTOs;
using AuthServer.Repositories;
using IdentityServer4;
using IdentityServer4.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Options;
using Shared.Response;
using Constants = Shared.Common.Constants;

namespace AuthServer.Features.Commands.UserCommands.UserLogin;

public class UserLoginHandler : IRequestHandler<UserLoginCommand, BaseResponse<UserLoginResponseDto>>
{
    private readonly ILogger<UserLoginHandler> _logger;
    private readonly UserManager<User> _userManager;
    private readonly AuthenticationOptions _authenticationOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUnitOfWork _unitOfWork;
    public UserLoginHandler(ILogger<UserLoginHandler> logger, UserManager<User> userManager, IOptions<AuthenticationOptions> authenticationOptions, IHttpClientFactory httpClientFactory, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _userManager = userManager;
        _httpClientFactory = httpClientFactory;
        _unitOfWork = unitOfWork;
        _authenticationOptions = authenticationOptions.Value;
    }

    public async Task<BaseResponse<UserLoginResponseDto>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<UserLoginResponseDto>();
        var methodName = $"{nameof(UserLoginHandler)}.{nameof(Handle)} Request = {JsonSerializer.Serialize(request)} =>";
        _logger.LogInformation(methodName);

        try
        {
            // 1. Check user exists
            var isEmailOrUserName = request.EmailOrUserName.Contains('@');
            var user = isEmailOrUserName
                ? await _userManager.FindByEmailAsync(request.EmailOrUserName)
                : await _userManager.FindByNameAsync(request.EmailOrUserName);
            if (user is null || user.IsDeleted && user.IsBlocked)
            {
                response.ToUnauthorizedResponse("Incorrect email, username or user is blocked");
                return response;
            }
                
            // 2. Check password is valid
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                response.ToUnauthorizedResponse();
                return response;
            }

            var isAdmin = string.Equals(user.Role, Constants.ADMIN, StringComparison.OrdinalIgnoreCase);
            if (!isAdmin)
            {
                // 3. Check user has not confirmed email yet
                var isConfirmedEmail = await _userManager.IsEmailConfirmedAsync(user);
                if (!isConfirmedEmail)
                {
                    response.ToUnauthorizedResponse("User has not confirmed email yet");
                    return response;
                }
            
                // 4. Check user is locked
                var isLockedOut = user is { LockoutEnabled: true, LockoutEnd: not null } &&
                                  user.LockoutEnd.Value > DateTimeOffset.Now; 
                if (isLockedOut)
                {
                    response.ToUnauthorizedResponse($"User is locked until {user.LockoutEnd.ToString()}"); 
                    return response;
                }
            }
            
            // 5. Check valid requested path
            if (request.Role != user.Role)
            {
                response.ToBadRequestResponse("Invalid requested path");
                return response;
            }
            
            // 6. Generate token
            // NOTE: this implements the Resource Owner Password Grant flow of OAuth 2.0
            var client = AuthConfig.Clients.First(c => c.AllowedGrantTypes.Contains(GrantType.ResourceOwnerPassword));
            var grantType = GrantType.ResourceOwnerPassword;
            var clientId = client.ClientId;
            var clientSecret = "client_secret";
            var scope = string.Join(" ", client.AllowedScopes.Concat([IdentityServerConstants.StandardScopes.OfflineAccess]));
            var identityServerUrl = _authenticationOptions.Authority;
            var identityServerTokenEndpoint = _authenticationOptions.Authority + "/connect/token";
            
            // Redirect to /connect/token with client_id, client_secret, grant_type, username, password as x-www-form-urlencoded
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(identityServerUrl);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, identityServerTokenEndpoint);
            var header = new Dictionary<string, string>
            {
                { "grant_type", grantType },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "username", user.UserName ?? string.Empty },
                { "password", request.Password },
                { "scope", scope }
            };
            httpRequest.Content = new FormUrlEncodedContent(header);
            var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken);
            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogCritical($"{methodName} Failed to get token from IdentityServer4: {httpResponse.StatusCode}");
                response.ToInternalErrorResponse();
                return response;
            }

            // Response is a json object with access_token, token_type, expires_in, refresh_token, scope
            var httpResponseContent = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
            var httpResponseDictionary = await JsonSerializer.DeserializeAsync<Dictionary<string, JsonElement>>(httpResponseContent, cancellationToken: cancellationToken);
            if (httpResponseDictionary is null)
            {
                _logger.LogCritical($"{methodName} Failed to deserialize token response");
                response.ToInternalErrorResponse();
                return response;
            }
            
            // 7. Get counterpart or player
            CounterPart? counterPart = null;
            Player? player = null;
            if (user.Role == Constants.COUNTERPART)
            {
                counterPart = await _unitOfWork.CounterParts.Where(x => x.Id == user.Id).FirstOrDefaultAsync(cancellationToken);
            }
            else if (user.Role == Constants.PLAYER)
            {
                player = await _unitOfWork.Players.Where(x => x.Id == user.Id).FirstOrDefaultAsync(cancellationToken);
            }
            
            var hasAccessToken = httpResponseDictionary.TryGetValue("access_token", out var accessToken);
            var hasRefreshToken = httpResponseDictionary.TryGetValue("refresh_token", out var refreshToken);
            var hasExpiresIn = httpResponseDictionary.TryGetValue("expires_in", out var expiresIn);
            
            var responseData = new UserLoginResponseDto
            {
                AccessToken = hasAccessToken ? accessToken.ToString() : string.Empty,
                RefreshToken = hasRefreshToken ? refreshToken.ToString() : string.Empty,
                ExpiresIn = hasExpiresIn ? expiresIn.GetInt32() : 0,
                FullProfile = new UserFullProfileDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    AvatarUrl = user.AvatarUrl ?? Common.Constants.DefaultAvatarUrl,
                    Role = user.Role,
                    CreatedDate = user.CreatedDate,
                    ModifiedDate = user.ModifiedDate,
                    ProfileLinked = user.ProfileLinked,
                    IsBlocked = user.IsBlocked,
                    BlockedDate = user.BlockedDate,
                    // For counterpart
                    Field = counterPart?.Field,
                    Addresses = counterPart?.Addresses,
                    // For player
                    BirthDate = player?.BirthDate,
                    Gender = player?.Gender,
                    FacebookUrl = player?.FacebookUrl
                }
            };
            response.ToSuccessResponse(responseData);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{methodName} Has error: {e.Message}");
            response.ToInternalErrorResponse();
        }

        return response;
    }
}