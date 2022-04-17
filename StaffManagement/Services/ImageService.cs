using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using StaffManagement.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManagement.Services
{
    public class ImageService : IImageService
    {
        private readonly string _Name;
        private readonly string _Key;
        private readonly string _secret;

        public ImageService(string Name, string Key, string secret )
        {
            _Name = Name;
            _Key = Key;
            _secret = secret;
        }
        public async  Task<string> AddImage(string path)
        {

            var myAccount = new Account { ApiKey = _Key, ApiSecret = _secret, Cloud = _Name };
            Cloudinary _cloudinary = new Cloudinary(myAccount);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(path)
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.Url.AbsoluteUri;

        }
    }
}
