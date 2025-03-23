using System.Net.Http.Headers;
using System.Net.Http.Json;
using BlazorAPI.ApiRequest.Model;
using Blazored.LocalStorage;

namespace BlazorAPI.ApiRequest
{
    public class MovieService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly NotificationService _notificationService;
        private readonly ILocalStorageService _localStorage;
        private const string BaseUrl = "api/Movies";

        public MovieService(IHttpClientFactory httpClientFactory, NotificationService notificationService, ILocalStorageService localStorage)
        {
            _httpClientFactory = httpClientFactory;
            _notificationService = notificationService;
            _localStorage = localStorage;

        }

        public async Task<List<Movie>> GetMoviesAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AuthorizedClient");
                var url = $"{BaseUrl}/GetAllMovies";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _notificationService.ShowError($"Ошибка при получении фильмов. Код: {response.StatusCode}");
                    return new List<Movie>();
                }

                var result = await response.Content.ReadFromJsonAsync<List<Movie>>();
                return result ?? new List<Movie>();
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Ошибка при получении списка фильмов: {ex.Message}");
                return new List<Movie>();
            }
        }

        public async Task<Movie?> GetMovieAsync(int? id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AuthorizedClient");
                return await client.GetFromJsonAsync<Movie>($"{BaseUrl}/GetMovie/{id}");
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Ошибка при получении фильма: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateMovieAsync(Movie movie)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AuthorizedClient");
                var token = await _localStorage.GetItemAsync<string>("authToken");

                if (string.IsNullOrEmpty(token))
                {
                    _notificationService.ShowError("Не найден токен авторизации");
                    return false;
                }

                // Создаем запрос вручную
                var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/CreateMovie");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Content = JsonContent.Create(movie);

                var response = await client.SendAsync(request);

                // Проверяем статус ответа
                if (response.IsSuccessStatusCode)
                {
                    _notificationService.ShowSuccess("Фильм успешно добавлен");
                    return true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _notificationService.ShowError("Ошибка авторизации. Пожалуйста, войдите снова");
                    return false;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _notificationService.ShowError($"Ошибка при создании фильма. Статус: {response.StatusCode}. {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Ошибка при создании фильма: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateMovieAsync(int id, Movie movie)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AuthorizedClient");
                var token = await _localStorage.GetItemAsync<string>("authToken");

                if (string.IsNullOrEmpty(token))
                {
                    _notificationService.ShowError("Не найден токен авторизации");
                    return false;
                }

                // Создаем запрос вручную
                var request = new HttpRequestMessage(HttpMethod.Put, $"{BaseUrl}/UpdateMovie/{id}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Content = JsonContent.Create(movie);

                var response = await client.SendAsync(request);

                // Проверяем статус ответа
                if (response.IsSuccessStatusCode)
                {
                    _notificationService.ShowSuccess("Фильм успешно обновлен");
                    return true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _notificationService.ShowError("Ошибка авторизации. Пожалуйста, войдите снова");
                    return false;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _notificationService.ShowError("Фильм не найден");
                    return false;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _notificationService.ShowError($"Ошибка при обновлении фильма. Статус: {response.StatusCode}. {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Ошибка при обновлении фильма: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteMovieAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AuthorizedClient");
                var token = await _localStorage.GetItemAsync<string>("authToken");

                if (string.IsNullOrEmpty(token))
                {
                    _notificationService.ShowError("Не найден токен авторизации");
                    return false;
                }

                var request = new HttpRequestMessage(HttpMethod.Delete, $"{BaseUrl}/DeleteMovie/{id}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _notificationService.ShowSuccess("Фильм успешно удален");
                    return true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _notificationService.ShowError("Ошибка авторизации. Пожалуйста, войдите снова");
                    return false;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _notificationService.ShowError("Фильм не найден");
                    return false;
                }
                else
                {
                    _notificationService.ShowError($"Ошибка при удалении фильма. Код: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Ошибка при удалении фильма: {ex.Message}");
                return false;
            }
        }
        public async Task<Movie?> SearchMovieByIdAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AuthorizedClient");
                var token = await _localStorage.GetItemAsync<string>("authToken");

                if (string.IsNullOrEmpty(token))
                {
                    _notificationService.ShowError("Не найден токен авторизации");
                    return null;
                }

                var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/GetMovie/{id}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var movie = await response.Content.ReadFromJsonAsync<Movie>();
                    if (movie != null)
                    {
                        return movie;
                    }
                    _notificationService.ShowError("Фильм не найден");
                    return null;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _notificationService.ShowError("Фильм не найден");
                    return null;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _notificationService.ShowError($"Ошибка при поиске фильма. Код: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Ошибка при поиске фильма: {ex.Message}");
                return null;
            }
        }
    }
}

