using System.Threading.Tasks;
using JudoLibrary.Api.Settings;
using Microsoft.AspNetCore.Http;

namespace JudoLibrary.Api.BackgroundServices.VideoEditing
{
    public interface IFileManager
    {
        string TemporarySavePath(string messageInput);
        bool TemporaryFileExists(string outputConvertedName);
        void DeleteTemporaryFileInPath(string outputConvertedName);
        string GetFFMPEGPath();
        string GetFileUrl(string fileName, FileType fileType);
        string GetSavePath(string fileName);
        Task<string> SaveTemporaryFile(IFormFile video);
        bool IsTemporary(string fileName);
    }
}