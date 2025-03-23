using APIBlazor.DataBaseContext;
using APIBlazor.Interfaces;
using APIBlazor.Model;
using APIBlazor.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIBlazor.Service
{
    public class MovieService : IMovieService
    {
        private readonly ContextDB _context;

        public MovieService(ContextDB context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetAllMovies()
        {
            var movies = await _context.Movies
                .Select(m => new MovieDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Genre = m.Genre,
                    Description = m.Description,
                    ReleaseDate = m.ReleaseDate,
                    Rating = m.Rating
                })
                .ToListAsync();

            return new OkObjectResult(movies);
        }

        public async Task<IActionResult> GetMovieById(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return new NotFoundResult();
            }

            var movieDto = new MovieDto
            {
                Id = movie.Id,
                Name = movie.Name,
                Genre = movie.Genre,
                Description = movie.Description,
                ReleaseDate = movie.ReleaseDate,
                Rating = movie.Rating
            };

            return new OkObjectResult(movieDto);
        }

        public async Task<IActionResult> CreateMovie(CreateMovieRequest request)
        {
            var movie = new Movie
            {
                Name = request.Name,
                Genre = request.Genre,
                Description = request.Description,
                ReleaseDate = request.ReleaseDate,
                Rating = request.Rating,
                CreatedAt = DateTime.UtcNow
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            var movieDto = new MovieDto
            {
                Id = movie.Id,
                Name = movie.Name,
                Genre = movie.Genre,
                Description = movie.Description,
                ReleaseDate = movie.ReleaseDate,
                Rating = movie.Rating
            };

            return new CreatedAtActionResult("GetMovie", "Movies", new { id = movie.Id }, movieDto);
        }

        public async Task<IActionResult> UpdateMovie(int id, UpdateMovieRequest request)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return new NotFoundResult();
            }

            movie.Name = request.Name;
            movie.Genre = request.Genre;
            movie.Description = request.Description;
            movie.ReleaseDate = request.ReleaseDate;
            movie.Rating = request.Rating;
            movie.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return new NotFoundResult();
                }
                throw;
            }
        }

        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return new NotFoundResult();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
        public class MovieDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Genre { get; set; }
            public string Description { get; set; }
            public DateTime ReleaseDate { get; set; }
            public decimal Rating { get; set; }
        }
    }
}
