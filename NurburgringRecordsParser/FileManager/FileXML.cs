using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace NurburgringRecordsParser.FileManager
{
    internal class FileXML : IFile
    {
        XDocument Document;

        GoogleDrive.API drive;

        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }

        public FileXML()
        {
            Document = new XDocument();
        }

        public FileXML(string filePath, string fileName, string fileType, bool isFileExists)
        {
            if (isFileExists)
                Document = XDocument.Load(filePath);
            else
                Document = new XDocument();

            FilePath = filePath;
            FileName = fileName;
            FileType = fileType;
        }

        public XmlReader GetXmlReader()
        {
            return Document.CreateReader();
        }

        public XmlWriter GetXmlWriter()
        {
            throw new NotImplementedException();
        }

        public void Add(object content)
        {
            Document.Add(content);
        }

        public void Save()
        {
            Document.Save(FilePath);
        }

        public async Task SaveToDrive()
        {
            if (drive == null)
                drive = GoogleDrive.API.GetInstance();

            using var stream = new MemoryStream();
            Document.Save(stream);
            await drive.UploadFile(stream, FileName, FileType);
        }
    }
}
