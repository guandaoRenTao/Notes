using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Notes.Helpers
{
    public interface IPhotoLibrary
    {
        Task<bool> SavePhotoAsync(byte[] data, string folder, string filename);
    }
}