using Microsoft.AspNetCore.Mvc;

namespace FileUploaderDemo.Controllers
{
    [Route("[controller]/[action]")]
    public class FileChunkController : Controller
    {
        public object Presence(string md5, string fileName)
        {
            var obj = UploadHelper.CheckFilePresence(md5, fileName);
            return obj;
        }

        [HttpPost]
        public object Upload(string md5, Microsoft.AspNetCore.Http.IFormFile file, int fileName)
        {
            var obj = UploadHelper.UploadChunk(md5, file, fileName);
            return obj;
        }

        [HttpPost]
        public object Merge([FromBody] FileChunkInput payload)
        {
            var obj = UploadHelper.MergeChunk(payload.md5, payload.fileName, payload.fileChunkNum);
            return obj;
        }
    }
}
