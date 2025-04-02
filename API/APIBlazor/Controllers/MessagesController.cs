using APIBlazor.DataBaseContext;
using APIBlazor.Model;
using APIBlazor.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static APIBlazor.Service.MovieService;
using static APIBlazor.Controllers.MessagesController;

namespace APIBlazor.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ContextDB _context;

        public MessagesController(ContextDB context)
        {
            _context = context;
        }

        [HttpGet("getMessages/{userId}")]
        public async Task<List<UserMessageDto>> GetMessages(int userId)
        {
            var userPairs = await _context.Messages
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .Select(m => new
            {
                User1Id = m.SenderId < m.ReceiverId ? m.SenderId : m.ReceiverId,
                User2Id = m.SenderId < m.ReceiverId ? m.ReceiverId : m.SenderId
            })
            .Distinct()
            .ToListAsync();

            var userIds = userPairs.Select(up => new { up.User1Id, up.User2Id }).ToList();

            var users = await _context.Users
                .Where(u => userIds.Select(x => x.User1Id).Contains(u.Id) || userIds.Select(x => x.User2Id).Contains(u.Id))
                .ToListAsync();

            var result = userPairs.Select(up => new UserMessageDto
            {
                SenderId = up.User1Id,
                ReceiverId = up.User2Id,
                SenderName = users.FirstOrDefault(u => u.Id == up.User1Id)?.Name,
                ReceiverName = users.FirstOrDefault(u => u.Id == up.User2Id)?.Name
            }).ToList();

            return result;
        }

        [HttpGet("getMessagesWithUser/{userId}/{senderId}")]
        public async Task<List<MessagesDto>> getMessagesWithUser(int userId, int senderId)
        {
            var messages = await _context.Messages
            .Where(m => (m.SenderId == userId && m.ReceiverId == senderId) ||
                         (m.SenderId == senderId && m.ReceiverId == userId))
            .OrderBy(m => m.Timestamp)
            .Select(m => new MessagesDto
            {
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                Message = m.Message,
                Timestamp = m.Timestamp,
                SenderName = _context.Users.FirstOrDefault(u => u.Id == m.SenderId).Name,
                ReceiverName = _context.Users.FirstOrDefault(u => u.Id == m.ReceiverId).Name
            })
            .ToListAsync();

            return messages;
        }

        [HttpGet("getFilmChats/{movieId}")]
        public async Task<List<MessagesFilmDto>> GetFilmChats(int movieId)
        {
            var messages = await _context.ChatFilm
                .Where(cf => cf.MovieId == movieId)
                .OrderByDescending(cf => cf.Timestamp)
                .Select(m => new MessagesFilmDto
                {
                    SenderId = m.SenderId,
                    Message = m.Message,
                    Title = m.Movie.Name,
                    SenderName = m.Users.Name,
                    Timestamp = m.Timestamp
                })
                .ToListAsync();
            return messages;
        }

        public class UserMessageDto
        {
            public int SenderId { get; set; }
            public string SenderName { get; set; }
            public int ReceiverId { get; set; }
            public int MovieId { get; set; }
            public string ReceiverName { get; set; }
        }

        public class MessagesDto
        {
            public int SenderId { get; set; }
            public string SenderName { get; set; }
            public int ReceiverId { get; set; }
            public string ReceiverName { get; set; }

            public string Message { get; set; }

            public DateTime Timestamp { get; set; }
        }

        public class MessagesFilmDto
        {
            public int SenderId { get; set; }
            public string SenderName { get; set; }
            public string? Title { get; set; }
            public string Message { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}