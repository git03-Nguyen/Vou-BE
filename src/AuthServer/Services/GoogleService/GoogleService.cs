// using AuthServer.Models.Records;
// using Google.Apis.Auth.OAuth2;
//
// namespace AuthServer.Services.GoogleService
// {
//     public class GoogleService : IGoogleService
//     {
//         private readonly IConfiguration _configuration;
//         private readonly ILogger<GoogleService> _logger;
//         public GoogleService(
//             IConfiguration configuration, 
//             ILogger<GoogleService> logger)
//         {
//             _configuration = configuration;
//             _logger = logger;
//         }
//
//         public async Task<SocialAuthOutput> ExchangeAuthorizationCode(string authCode)
//         {
//             try
//             {
//                 await Google.Apis.Auth.JsonWebSignature.VerifySignedTokenAsync(authCode, new Google.Apis.Auth.SignedTokenVerificationOptions()
//                 {
//                     CertificatesUrl = GoogleAuthConsts.JsonWebKeySetUrl
//                 });
//
//                 var validPayload = await Google.Apis.Auth.GoogleJsonWebSignature.ValidateAsync(authCode, new Google.Apis.Auth.GoogleJsonWebSignature.ValidationSettings()
//                 {
//                     Audience = new string[] { _configuration["Google:ClientID"], _configuration["Google:ClientID-APP"] }
//                 });
//
//                 var userEmail = validPayload.Email;
//                 var emailVerified = validPayload.EmailVerified;
//                 var firstName = validPayload.GivenName;
//                 var lastName = validPayload.FamilyName;
//
//                 if (string.IsNullOrEmpty(userEmail) || !emailVerified)
//                 {
//                     _logger.LogError($"{nameof(GoogleService)} Don't have an email or EmailVerified: {emailVerified}");
//                     return null;
//                 }
//
//                 firstName = !string.IsNullOrEmpty(firstName) ? firstName : userEmail;
//                 lastName = !string.IsNullOrEmpty(lastName) ? lastName : Constants.DefaultLastName;
//
//                 return new SocialAuthOutput
//                 {
//                     Email = userEmail.ToLower(),
//                     FirstName = firstName,
//                     LastName = lastName
//                 };
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, $"{nameof(GoogleService)} Has error: Message = {ex.Message}");
//                 return null;
//             }
//         }
//
//     }
// }
