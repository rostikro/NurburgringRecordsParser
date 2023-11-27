using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace NurburgringRecordsParser.FileManager
{
    internal interface IFile
    {
        string FileName { get; set; }
        string FileType { get; set; }
        string FilePath { get; set; }
        XmlReader GetXmlReader();
        XmlWriter GetXmlWriter();
        void Add(object content);
        void Save();
        Task SaveToDrive();
    }
}
