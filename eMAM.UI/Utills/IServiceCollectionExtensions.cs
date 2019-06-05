//using eMAM.Service.Contracts;
//using Google.Apis.Auth.OAuth2;
//using Google.Apis.Auth.OAuth2.Flows;
//using Google.Apis.Auth.OAuth2.Responses;
//using Google.Apis.Gmail.v1;
//using Google.Apis.Services;
//using Microsoft.Extensions.DependencyInjection;

//namespace eMAM.UI.Utills
//{
//    public static class IServiceCollectionExtensions
//    {
//        public static IServiceCollection AddGmailService(this IServiceCollection services)
//        {
//            services.AddScoped<GmailService>(provider =>
//            {
//                using (var scope = provider.CreateScope())
//                {
//                    var userData = scope.ServiceProvider.GetRequiredService<IGmailUserDataService>().Get();

//                    var authorizationCodeFlowInitializer = new AuthorizationCodeFlow.Initializer(
//                        "https://accounts.google.com/o/oauth2/v2/auth", // валиден
//                        "https://oauth2.googleapis.com/token" // валиден
//                    );
//                    authorizationCodeFlowInitializer.ClientSecrets = new ClientSecrets
//                    {
//                        ClientId = "667407283017-hjbtv4a5sjr3garaprkidqo36qs4u7o3.apps.googleusercontent.com", // валиден
//                        ClientSecret = "cH5tErPh_uqDZDmp1F1aDNIs" // валиден
//                    };

//                    var authcodeflow = new AuthorizationCodeFlow(authorizationCodeFlowInitializer);
//                    var tokenResponse = new TokenResponse()
//                    {
//                        AccessToken = userData.AccessToken, // не го бърка
//                        RefreshToken = userData.RefreshToken // трябва да е валиден
//                    };

//                    return new GmailService(new BaseClientService.Initializer()
//                    {
//                        HttpClientInitializer = new UserCredential(authcodeflow, "me", tokenResponse)
//                    });
//                }
//            });

//            return services;
//        }
//    }
//}
