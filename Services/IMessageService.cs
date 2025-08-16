using p4.Models.DTO;
using p4.Models.Entities;

namespace p4.Services
{
    public interface IMessageService
    {
        Task<Msg> Send(MesssageDTO message);
        Task<List<Msg>> All(int conv);
        Task<List<Msg>> MarkAsSeen(int conv,int userId);
        void debug(string bulllll);
    }
}