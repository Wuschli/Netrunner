using System.Threading.Tasks;

namespace Netrunner.Shared.Chat
{
    public interface IChatHub
    {
        Task ReceiveMessage(ChatMessage message);
    }
}