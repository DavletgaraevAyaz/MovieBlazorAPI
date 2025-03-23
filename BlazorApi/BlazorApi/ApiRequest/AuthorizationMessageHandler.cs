using Blazored.LocalStorage;
using System.Net.Http.Headers;

public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;

    public AuthorizationMessageHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            if (OperatingSystem.IsBrowser())
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в AuthorizationMessageHandler: {ex.Message}");
            throw;
        }
    }
}