using System.Threading.Tasks;

namespace Sample.Chatting.Database
{
    public interface IChattingDB
    {
        Task<bool> IsDuplicatedAsync(string id);
        Task<bool> CreateUserAsync(string id, string password);
        Task<bool> IsApprovedAsync(string id, string password);
    }
}
