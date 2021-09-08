using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Vehicles.API.Helpers
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile imageFile);
        Task<string> UploadImageAsync(IFormFile imageFile, string folder);
        string UploadImage(byte[] pictureArray, string folder);
    }
}
