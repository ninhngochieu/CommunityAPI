using System.Threading.Tasks;
using BackendAPI.Services.Interface;

namespace BackendAPI.Repository.Interface
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IMessageRepository MessageRepository { get; }
        ILikeRepository LikeRepository { get; }
        Task<bool> Complete();
        bool HasChange();
    }
}