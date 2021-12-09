using System.Threading.Tasks;
using BackendAPI.Modules;
using BackendAPI.Services.Interface;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BackendAPI.Services.Implement
{
    public class PhotoService: IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(IOptions<CloudinarySettings> options)
        {
            _cloudinary = new Cloudinary(new Account()
            {
                Cloud = options.Value.CloudName,
                ApiKey = options.Value.ApiKey,
                ApiSecret =options.Value.ApiSecret
            });
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile formFile)
        {
            var uploadResult = new ImageUploadResult();
            if (formFile.Length > 0)
            {
                using var stream = formFile.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(formFile.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var @params = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(@params);
        }
    }
}