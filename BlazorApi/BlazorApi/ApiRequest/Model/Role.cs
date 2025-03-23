using System.ComponentModel.DataAnnotations;

namespace BlazorAPI.ApiRequest.Model
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

