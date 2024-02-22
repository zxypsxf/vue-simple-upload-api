namespace FileUploaderDemo
{
    public class UploadHelper
    {
        #region 分片上传

        #region 检查文件是否存在
        public static FileChunkResponse CheckFilePresence(string md5, string fileName)
        {
            var obj = new FileChunkResponse()
            {
                code = 2000,
                message = "成功",
                data = new
                {
                    presence = false,
                    md5 = md5
                }
            };

            return obj;
        }
        #endregion

        #region 上传分片文件
        public static FileChunkResponse UploadChunk(string md5, Microsoft.AspNetCore.Http.IFormFile file, int index)
        {
            var obj = new FileChunkResponse()
            {
                code = 2000,
                message = "上传成功",
                data = new
                {
                    md5 = md5
                }
            };

            string filePath = $"Upload/Chunk/" + md5 + "/";
            string fullPath = CoreHttpContext.MapPath(filePath);
            string path = fullPath + index;

            if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return obj;
        }
        #endregion

        #region 合并分片文件
        public static FileChunkResponse MergeChunk(string md5, string fileName, int fileChunkNum)
        {
            if (string.IsNullOrEmpty(md5))
            {
                return new FileChunkResponse()
                {
                    code = 0,
                    message = "参数不正确"
                };
            }
            var obj = new FileChunkResponse()
            {
                code = 2000,
                message = "上传成功",
                data = new
                {
                    md5 = md5,
                    fileName = fileName,
                    fileChunkNum = fileChunkNum
                }
            };
            List<string> allowExt = new List<string>() { ".png", ".jpg", ".jpeg", ".gif", ".pdf", ".mp4" };
            string ext = Path.GetExtension(fileName);
            if (!allowExt.Contains(ext))
            {
                DeleteFolder(md5);
                return new FileChunkResponse()
                {
                    code = 0,
                    message = "不允许的文件类型"
                };
            }
            string uploadPath = "Upload/Chunk/";
            string root = CoreHttpContext.MapPath(uploadPath);

            string newFileName = Guid.NewGuid().ToString(); //随机生成新的文件名
            string newFileNameWithExt = newFileName + ext;

            string sourcePath = Path.Combine(root, md5 + "/");//源数据文件夹
            string targetPath = Path.Combine(root, newFileName + ext);//合并后的文件


            DirectoryInfo dicInfo = new DirectoryInfo(sourcePath);
            if (Directory.Exists(Path.GetDirectoryName(sourcePath)))
            {
                FileInfo[] files = dicInfo.GetFiles();
                foreach (FileInfo file in files.OrderBy(f => int.Parse(f.Name)))
                {
                    FileStream addFile = new FileStream(targetPath, FileMode.Append, FileAccess.Write);
                    BinaryWriter AddWriter = new BinaryWriter(addFile);

                    //获得上传的分片数据流
                    Stream stream = file.Open(FileMode.Open);
                    BinaryReader TempReader = new BinaryReader(stream);
                    //将上传的分片追加到临时文件末尾
                    AddWriter.Write(TempReader.ReadBytes((int)stream.Length));
                    //关闭BinaryReader文件阅读器
                    TempReader.Close();
                    stream.Close();
                    AddWriter.Close();
                    addFile.Close();

                    TempReader.Dispose();
                    stream.Dispose();
                    AddWriter.Dispose();
                    addFile.Dispose();
                }
                DeleteFolder(md5);

                FileStream file_temp = new FileStream(targetPath, FileMode.Open, FileAccess.Read);
                int fileSize = (int)file_temp.Length; //获得文件大小，以字节为单位

                var jsonObj = new
                {
                    name = fileName,
                    path = uploadPath + newFileNameWithExt,
                    size = fileSize,
                    ext = ext,
                };
                obj.data = jsonObj;
            }
            else
            {
                obj.code = 0;
                obj.message = "失败";
            }

            return obj;
        }
        #endregion

        #region 删除文件夹及其内容
        /// <summary>
        /// 删除文件夹及其内容
        /// </summary>
        /// <param name="dir"></param>
        private static void DeleteFolder(string md5)
        {
            string root = CoreHttpContext.MapPath($"Upload/Chunk/{md5}");
            //删除这个目录下的所有文件
            if (Directory.GetFiles(root).Length > 0)
            {
                foreach (string f in Directory.GetFiles(root))
                {
                    System.IO.File.Delete(f);
                }
            }
            Directory.Delete(root, true);
        }
        #endregion

        #endregion
    }
}

public class FileChunkResponse
{
    /// <summary>
    /// 2000 成功
    /// </summary>
    public int code { get; set; }
    public string message { get; set; }
    public object data { get; set; }
}
