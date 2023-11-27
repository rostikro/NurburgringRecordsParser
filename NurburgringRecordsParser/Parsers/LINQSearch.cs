using NurburgringRecordsParser.Helpers;
using NurburgringRecordsParser.Interfaces;
using NurburgringRecordsParser.Models;
using System.Xml;
using System.Xml.Linq;

namespace NurburgringRecordsParser.Parsers
{
    internal class LINQSearch : ISearch
    {
        XDocument _document;

        public LINQSearch(XmlReader reader)
        {
            _document = XDocument.Load(reader);
        }

        public List<Record> Search(Record record)
        {
            return (from val in _document.Descendants("Record")
                          where
                          (record.Category == null || record.Category == val.Parent.Attribute("Category").Value) &&
                          (record.Time == null || record.Time == TrackTimeHelper.Estimate(val.Attribute("Time").Value)) &&
                          (record.Manufacturer == null || record.Manufacturer == val.Attribute("Manufacturer").Value) &&
                          (record.Driver == null || record.Driver == val.Attribute("Driver").Value) &&
                          (record.Date == null || record.Date == val.Attribute("Date").Value[^4..])
                          select new Record
                          {
                              Category = val.Parent.Attribute("Category").Value,
                              Time = val.Attribute("Time").Value,
                              Manufacturer = val.Attribute("Manufacturer").Value,
                              Model = val.Attribute("Model").Value,
                              Driver = val.Attribute("Driver").Value,
                              Date = val.Attribute("Date").Value,
                              Video = val.Attribute("Video").Value,
                          }).ToList();

            //List<Record> records = new();
            //foreach (var recObj in result)
            //{
            //    var rec = new Record
            //    {
            //        Category = recObj.Parent.Attribute("Category").Value,
            //        Time = recObj.Attribute("Time").Value,
            //        Manufacturer = recObj.Attribute("Manufacturer").Value,
            //        Model = recObj.Attribute("Model").Value,
            //        Driver = recObj.Attribute("Driver").Value,
            //        Date = recObj.Attribute("Date").Value,
            //        Video = recObj.Attribute("Video").Value,
            //    };

            //    records.Add(rec);
            //}
            //_document.Save(_filePath.Replace(".", "-filter."));

            //return records;
        }
    }
}
