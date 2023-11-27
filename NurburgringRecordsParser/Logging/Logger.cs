using NurburgringRecordsParser.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NurburgringRecordsParser.Logging
{
    internal sealed class Logger
    {
        static Logger _instance;
        string _logFilePath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "NurburgringRecords\\log.txt");

        Logger() { }

        public static Logger GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Logger();
            }

            return _instance;
        }

        public void LogFilter(Record record)
        {
            StringBuilder str = new StringBuilder();
            if (record.Category != null)
                str.Append($"Category - {record.Category}, ");
            if (record.Time != null)
                str.Append($"Time - {record.Time}, ");
            if (record.Manufacturer != null)
                str.Append($"Manufacturer - {record.Manufacturer}, ");
            if (record.Driver != null)
                str.Append($"Driver - {record.Driver}, ");
            if (record.Date != null)
                str.Append($"Date - {record.Date}, ");

            if (str.Length != 0)
                str.Remove(str.Length - 2, 2);
            else
                str.Append("No filter");

            using (StreamWriter writer = new StreamWriter(_logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now} | Filter | {str}");
            }
        }

        public void LogTransform(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(_logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now} | Transform | The table is saved in \"{filePath}\"");
            }
        }

        public void LogSave(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(_logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now} | Save | Filtered xml file saved in \"{filePath}\"");
            }
        }
    }
}
