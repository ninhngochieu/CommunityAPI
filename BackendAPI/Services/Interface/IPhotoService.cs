using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace BackendAPI.Services.Interface
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile formFile);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}