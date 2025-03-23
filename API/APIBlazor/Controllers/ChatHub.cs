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

        
        public async Task SendMessageToUser(string message, int idUser, int idReceiver)
        {
            var receiver = await _context.Users.FindAsync(idReceiver);
            if (receiver != null)
            {
                string connection = receiver.ConnectionId;
                try
                {
                    //await Clients.User(connection).SendAsync("ReceiveMessage", message, idUser, idReceiver);
                    await Clients.All.SendAsync("ReceiveMessage", message, idUser, idReceiver);
                    _context.Messages.Add(new Messages
                    {
                        SenderId = idUser,
                        ReceiverId = idReceiver,
                        Message = message
                    });
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Good sending");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error sending message: {e.Message}");
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
