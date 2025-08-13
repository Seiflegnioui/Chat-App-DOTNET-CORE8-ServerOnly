using p4.Models.DTO;
using p4.Models.Entities;

namespace p4.Services
{
    public interface IConversationService
    {
        Task<int> StartAsyn(ConvDTO conv);
    }
}