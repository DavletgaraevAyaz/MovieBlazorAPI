using APIBlazor.Interfaces;
using APIBlazor.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIBlazor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("GetAllMovies")]
        public async Task<IActionResult> GetMovies()
        {
            return await _movieService.GetAllMovies();
        }

        [HttpGet("GetMovie/{id}")]
        public async Task<IActionResult> GetMovie(int id)
        {
            return await _movieService.GetMovieById(id);
        }

        [HttpPost("CreateMovie")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> CreateMovie([FromBody] CreateMovieRequest request)
        {
            return await _movieService.CreateMovie(request);
        }

        [HttpPut("UpdateMovie/{id}")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] UpdateMovieRequest request)
        {
            return await _movieService.UpdateMovie(id, request);
        }

        [HttpDelete("DeleteMovie/{id}")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            return await _movieService.DeleteMovie(id);
        }
    }
}
