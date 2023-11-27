using NurburgringRecordsParser.Models;

namespace NurburgringRecordsParser.Interfaces
{
    internal interface ISearch
    {
        List<Record> Search(Record record);
    }
}
