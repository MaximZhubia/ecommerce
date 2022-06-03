using ECommerceSystem.Common.Entities;
using ECommerceSystem.Common.Models;
using ECommerceSystem.Data;
using ECommerceSystem.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSystem.Services
{
    public class FileService : IFileService
    {
        private const string FilesDirectory = "Files";

        private readonly ApplicationDbContext _applicationDbContext;
        private readonly DbSet<Common.Entities.File> _files;

        public FileService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _files = _applicationDbContext.Files;
        }

        public IEnumerable<ResponseFile> GetFiles()
        {
            List<ResponseFile> result = new List<ResponseFile>();

            foreach (Common.Entities.File file in _files)
            {
                string ownerUsername = _applicationDbContext.Users.SingleOrDefault(x => x.UserId == file.UserId).Username;
                result.Add(new ResponseFile(file.Id, file.Name, file.Price, file.UserId, ownerUsername));
            }

            return result; 
        }

        public Common.Entities.File GetFile(int id)
        {
            return _files.ToList().FirstOrDefault(x => x.Id == id);
        }

        public Common.Entities.File Upload(string fileName, double price, int ownerId, byte[] fileContentBytes)
        {
            if (!Directory.Exists(FilesDirectory))
            {
                Directory.CreateDirectory(FilesDirectory);
            }

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string fileExtension = Path.GetExtension(fileName);
            string newFileName = fileNameWithoutExtension + "-" + ownerId + "-" + ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds() + fileExtension;
            string newFilePath = Path.Combine(FilesDirectory, newFileName);

            System.IO.File.WriteAllBytes(newFilePath, fileContentBytes);

            return _applicationDbContext.AddFile(fileNameWithoutExtension + fileExtension, newFilePath, price, ownerId);
        }
    }
}
