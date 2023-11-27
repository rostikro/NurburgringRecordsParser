using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NurburgringRecordsParser.FileManager
{
    internal class FileHtml : IFile
    {
        StringBuilder content;

        GoogleDrive.API drive;

        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }

        public FileHtml()
        {
            content = new StringBuilder();
        }

        public FileHtml(string filePath, string fileName, string fileType, bool isFileExists)
        {
            if (isFileExists)
                content = new StringBuilder(File.ReadAllText(filePath));
            else
                content = new StringBuilder();

            FilePath = filePath;
            FileName = fileName;
            FileType = fileType;
        }

        public XmlReader GetXmlReader()
        {
            throw new NotImplementedException();
        }

        public void Add(object o)
        {
            throw new NotImplementedException();
        }

        public XmlWriter GetXmlWriter()
        {
            return XmlWriter.Create(content);
        }

        public void Save()
        {
            File.WriteAllText(FilePath, content.ToString());
        }

        public async Task SaveToDrive()
        {
            if (drive == null)
                drive = GoogleDrive.API.GetInstance();

            using var stream = new MemoryStream();
            using var streamWriter = new StreamWriter(stream);
            streamWriter.Write(content);
            await drive.UploadFile(stream, FileName, FileType);           
        }
    }
}
