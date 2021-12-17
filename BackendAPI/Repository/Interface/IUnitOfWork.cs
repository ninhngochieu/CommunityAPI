using System.Threading.Tasks;
using BackendAPI.Services.Interface;

namespace BackendAPI.Repository.Interface
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        ILikeRepository LikeRepository { get; }
        IMessageRepository MessageRepository { get; }

        Task<bool> Complete();
        bool HasChange();
    }
}