using System.ComponentModel.DataAnnotations;

namespace BlazorAPI.ApiRequest
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Почта обязательна")]
        [EmailAddress(ErrorMessage = "Неверный формат почты")]
        public string Email { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        public string Password { get; set; }
        public string Description { get; set; }
    }
}
