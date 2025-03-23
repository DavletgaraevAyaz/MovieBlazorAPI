using APIBlazor.Requests;
using Microsoft.AspNetCore.Mvc;

namespace APIBlazor.Interfaces
{
    public interface IMovieService
    {
        Task<IActionResult> GetAllMovies();
        Task<IActionResult> GetMovieById(int id);
        Task<IActionResult> CreateMovie(CreateMovieRequest request);
        Task<IActionResult> UpdateMovie(int id, UpdateMovieRequest request);
        Task<IActionResult> DeleteMovie(int id);
    }
}
