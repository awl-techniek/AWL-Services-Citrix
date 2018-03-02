using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace AWL.Citrix.Service
{
    public class StoreFrontAuthenticate: Attribute, IAuthenticationFilter
    {
        public static string StoreFrontBaseAddres { get; set; }
        public static string StoreName { get; set; }

        public bool AllowMultiple
        {
            get { return false; }
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = context.Request;
            AuthenticationHeaderValue authorization = request.Headers.Authorization;

            var cookies = request.Headers.GetCookies().FirstOrDefault();

            if (cookies == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
                return;
            }
            StoreFrontBaseAddres = "http://" + context.Request.Headers.GetValues("X-ServerIdentifier").FirstOrDefault();
            var auth = AuthenticateUser(cookies);
            if (auth == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
                return;
            }

            var user = await GetUserInfo(auth);
            if (user == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
                return;
            }

            //var identity = await userManager.CreateIdentityAsync(user, "BasicAuth");
            IList<Claim> claimCollection = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claimCollection, "StoreFront");
            context.Principal = new ClaimsPrincipal(claimsIdentity);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        private AuthCredential AuthenticateUser(CookieHeaderValue cookies)
        {
            var auth    = cookies["CtxsAuthId"];
            var token   = cookies["CsrfToken"];
            var session = cookies["ASP.NET_SessionId"];

            if (auth == null || token == null || session == null)
            {
                return null;
            }

            var credentials = new AuthCredential
            {
                AuthToken = auth.Value,
                CSRFToken = token.Value,
                SessionID = session.Value
            };

            if (!credentials.IsValid)
            {
                return null;
            }

            return credentials;
        }
        private async Task<User> GetUserInfo(AuthCredential auth)
        {
            User user = null;
            Uri address = new Uri(StoreFrontBaseAddres);
            using (var handler = new HttpClientHandler())
            {
                handler.CookieContainer = new CookieContainer();
                handler.CookieContainer.Add(address, new Cookie("CtxsAuthId", auth.AuthToken));
                handler.CookieContainer.Add(address, new Cookie("CsrfToken", auth.CSRFToken));
                handler.CookieContainer.Add(address, new Cookie("ASP.NET_SessionId", auth.SessionID));
                using (var client = new HttpClient(handler))
                {
                    client.BaseAddress = address;
                    client.DefaultRequestHeaders.Add("X-Citrix-IsUsingHTTPS", "yes");
                    client.DefaultRequestHeaders.Add("Csrf-Token", auth.CSRFToken);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

                    var response = await client.PostAsync("/Citrix/" + StoreName + "/Authentication/GetUserName", new StringContent(""));
                    if (response.IsSuccessStatusCode)
                    {
                        user = new User()
                        {
                            Credential = auth,
                            UserName = await response.Content.ReadAsStringAsync()
                        };
                    }
                }
            }
            return user;
        }
    }
}
