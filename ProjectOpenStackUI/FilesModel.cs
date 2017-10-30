using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOpenStackUI
{
    /// <summary>
    /// Main methods to execute
    /// </summary>
    public enum MethodNode
    {
        DOWNLOAD,
        DELETE
    };

    /// <summary>
    /// Represents a file or a folder
    /// </summary>
    public class FileModel
    {
        public NodeModel Node { get; set; }

        public Boolean IsDirectory { get; set; }

        public int Size { get; set; }

        public String Name { get; set; }

        public String Hash { get; set; }

        public String Last_modified { get; set; }

        public String Content_type { get; set; }

        public int Count { get; set; }

        public String Uri { get; set; }
    }

    /// <summary>
    /// Represents a list of files
    /// </summary>
    public class FilesModel
    {
        List<FileModel> files = new List<FileModel>();

        public FilesModel()
        {
        }

        public FilesModel(List<FileModel> files)
        {
            this.files = files;
        }

        public void AddFile(FileModel file)
        {
            files.Add(file);
        }

        public List<FileModel> GetFiles()
        {
            return files;
        }

        public void AddFile(Boolean isDirectory, int size, String name, String uri, String hash, String last_modified, String content_type, int count)
        {
            files.Add(new FileModel()
            {
                IsDirectory = isDirectory,
                Size = size,
                Name = name,
                Uri = uri,
                Hash = hash,
                Last_modified = last_modified,
                Content_type = content_type,
                Count = count
            });
        }

        public FileModel FindFileModel(String name)
        {
            FileModel tmp = files.Where(x => x.Name.Equals(name)).FirstOrDefault();
            return tmp;
        }
    }
}
