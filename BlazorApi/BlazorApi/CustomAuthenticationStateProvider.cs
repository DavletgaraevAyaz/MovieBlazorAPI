namespace BlazorApi
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Components.Authorization;
    using Blazored.LocalStorage;
    using System.IdentityModel.Tokens.Jwt;

    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string token = null;

            try
            {
                token = await _localStorage.GetItemAsync<string>("authToken");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении токена: {ex.Message}");
            }

            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        public async Task NotifyUserAuthentication(string token)
        {
            await Task.Run(() =>
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
                var user = new ClaimsPrincipal(identity);

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            });
        }

        public async Task MarkUserAsLoggedOut()
        {
            await Task.Run(() =>
            {
                var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
            });
        }
    }
}
