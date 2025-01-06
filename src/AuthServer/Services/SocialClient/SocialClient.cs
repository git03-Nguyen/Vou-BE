// using AuthServer.Models.Records;
// using SharedLib.Enums;
// using AuthServer.Services.GoogleService;
// using AuthServer.Services.AppleService;
// using AuthServer.Services.FacebookService;
//
// namespace AuthServer.Services.SocialClient;
// public class SocialClient : ISocialClient
// {
//     private readonly IGoogleService _googleService;
//     private readonly IAppleService _appleService;
//     private readonly IFacebookService _facebookService;
//     
//     public SocialClient(
//         IGoogleService googleService, 
//         IAppleService appleService, 
//         IFacebookService facebookService)
//     {
//         _googleService = googleService;
//         _appleService = appleService;
//         _facebookService = facebookService;
//     }
//
//     public async Task<SocialAuthOutput> GetInfoFromAuthorizationCode(string authorizeCode, Provider provider, string secretKey = null, bool isLoginByOIDCToken = false)
//     {
//         return provider switch
//         {
//             Provider.APPLE => await _appleService.ExchangeAuthorizationCode(authorizeCode),
//             Provider.GOOGLE => await _googleService.ExchangeAuthorizationCode(authorizeCode),
//             Provider.FACEBOOK => await _facebookService.ExchangeAuthorizationCode(authorizeCode, secretKey, isLoginByOIDCToken),
//             _ => null
//         };
//     }
// }