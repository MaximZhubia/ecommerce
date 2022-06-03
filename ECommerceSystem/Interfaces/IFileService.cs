using ECommerceSystem.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSystem.Interfaces
{
    public interface IFileService
    {
        IEnumerable<ResponseFile> GetFiles();
        Common.Entities.File GetFile(int id);
        Common.Entities.File Upload(string fileName, double price, int ownerId, byte[] fileContentBytes);
    }
}
