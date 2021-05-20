using System.Collections.Generic;
using System.Threading.Tasks;
using Netrunner.Shared.Chat;
using WampSharp.V2.Rpc;

namespace Netrunner.Shared.Services
{
    public interface IChatService
    {
        //rooms
        [WampProcedure("netrunner.chat.get_rooms")]
        Task<List<ChatRoom>> GetRooms();

        [WampProcedure("netrunner.chat.get_room_details")]
        Task<ChatRoom?> GetRoomDetails(string roomId);

        [WampProcedure("netrunner.chat.create_room")]
        Task CreateRoom(CreateChatRoom createChatRoom);

        [WampProcedure("netrunner.chat.join_room")]
        Task JoinRoom(string roomId);

        [WampProcedure("netrunner.chat.leave_room")]
        Task LeaveRoom(string roomId);

        //invites
        [WampProcedure("netrunner.chat.invite_user")]
        Task InviteUserToRoom(string roomId, string username);

        [WampProcedure("netrunner.chat.get_invites")]
        Task<IEnumerable<string>?> GetInvites();

        //messages
        [WampProcedure("netrunner.chat.get_messages")]
        Task<IEnumerable<ChatMessage>?> GetMessages(string roomId);

        [WampProcedure("netrunner.chat.send_message")]
        Task SendMessage(ChatMessage message);
    }
}