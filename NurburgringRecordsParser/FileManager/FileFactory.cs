
namespace NurburgringRecordsParser.FileManager
{

    public enum FileTypes
    {
        HTML = 0,
        XML = 1,
        XSL = 2,
    }

    internal class FileFactory
    {
        public static IFile CreateFile(FileTypes type)
        {
            switch (type)
            {
                case FileTypes.HTML:
                    return new FileHtml();
                case FileTypes.XML:
                case FileTypes.XSL:
                    return new FileXML();
                default:
                    return null;
            }
        }

        public static IFile CreateFile(FileTypes type, string filePath, string fileName, bool isFileExists = true)
        {
            switch (type)
            {
                case FileTypes.HTML:
                    return new FileHtml(filePath, fileName, "text/html", isFileExists);
                case FileTypes.XML:
                case FileTypes.XSL:
                    return new FileXML(filePath, fileName, "text/xml", isFileExists);
                default:
                    return null;
            }
        }
    }
}
