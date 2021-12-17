using System.Threading.Tasks;
using AutoMapper;
using BackendAPI.Models;
using BackendAPI.Repository.Interface;
using BackendAPI.Services.Interface;

namespace BackendAPI.Repository.Implement
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly CommunityContext _context;
        private readonly IMapper _mapper;

        public UnitOfWork(CommunityContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IUserRepository UserRepository => new UserRepository(_context, _mapper);
        public ILikeRepository LikeRepository => new LikeRepository(_context);
        public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);
        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChange()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}