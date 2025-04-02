using APIBlazor.DataBaseContext;
using APIBlazor.Model;
using APIBlazor.Requests;
using Azure.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace APIBlazor.Controllers
{
    public class ChatHub : Hub
    {
        private readonly ContextDB _context;

        public ChatHub(ContextDB context)
        {
            _context = context;
        }

        public async Task CreateInitialChatsForUser(int userId)
        {
            var movies = await _context.Movies.ToListAsync();
            foreach (var movie in movies)
            {
                // Проверяем, есть ли уже чат для данного фильма и пользователя
                var existingChat = await _context.ChatFilm
                    .Where(cf => cf.SenderId == userId && cf.MovieId == movie.Id)
                    .FirstOrDefaultAsync();

                if (existingChat == null)
                {
                    // Создаем новый чат
                    _context.ChatFilm.Add(new ChatFilm
                    {
                        SenderId = userId,
                        MovieId = movie.Id
                    });
                    await _context.SaveChangesAsync();
                }
            }
        }
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext.Request.Query["userId"];
            if (int.TryParse(userId, out int parsedUserId))
            {
                var user = await _context.Users.FindAsync(parsedUserId);
                if (user != null)
                {
                    user.ConnectionId = Context.ConnectionId;
                    await _context.SaveChangesAsync();
                }
            }
            await base.OnConnectedAsync();
        }

        public async Task SendMessageToUser(string message, int idUser, int idReceiver)
        {
            var receiver = await _context.Users.FindAsync(idReceiver);
            if (receiver != null)
            {
                try
                {
                    // Отправляем сообщение конкретному пользователю через его ConnectionId
                    await Clients.Client(receiver.ConnectionId).SendAsync("ReceiveMessage", message, idUser, idReceiver);

                    // Также отправляем сообщение отправителю для обновления его интерфейса
                    var sender = await _context.Users.FindAsync(idUser);
                    if (sender != null)
                    {
                        await Clients.Client(sender.ConnectionId).SendAsync("ReceiveMessage", message, idUser, idReceiver);
                    }

                    // Сохраняем сообщение в базе данных
                    _context.Messages.Add(new Messages
                    {
                        SenderId = idUser,
                        ReceiverId = idReceiver,
                        Message = message,
                        Timestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"))
                    });
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Сообщение успешно отправлено и сохранено в базе данных.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка при отправке сообщения: {e.Message}");
                }
            }
        }

        public async Task SendMessageFilm(string message, int senderId, int? idFilm)
        {
            var user = await _context.Users.FindAsync(senderId);
            string Title = null;
            if (idFilm != null) Title = (await _context.Movies.FindAsync(idFilm)).Name;

            _context.ChatFilm.Add(new ChatFilm
            {
                SenderId = senderId,
                MovieId = idFilm,
                Message = message
            });
            await _context.SaveChangesAsync();

            await Clients.All.SendAsync("ReceiveMessageFilm", message, senderId, user.Name, Title);
        }

        public async Task RegisterUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                try
                {
                    user.ConnectionId = Context.ConnectionId;
                    await _context.SaveChangesAsync();
                } catch (Exception e) { }
            }
        }
    }
}
