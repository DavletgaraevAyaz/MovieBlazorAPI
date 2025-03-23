using System.ComponentModel.DataAnnotations;

namespace BlazorAPI.ApiRequest
{
    public class LoginUserRequest
    {
        [Required(ErrorMessage = "Почта обязательна")]
        [EmailAddress(ErrorMessage = "Некорректный формат почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        public string Password { get; set; }
    }
}
