using NurburgringRecordsParser.Helpers;
using NurburgringRecordsParser.Interfaces;
using NurburgringRecordsParser.Models;
using System.Xml;

namespace NurburgringRecordsParser.Parsers
{
    internal class DOMSearch : ISearch
    {
        XmlDocument _document = new();

        public DOMSearch(XmlReader reader)
        {
            _document.Load(reader);
        }

        public List<Record> Search(Record record)
        {
            List<Record> records = new();

            XmlNodeList groups = _document.SelectNodes("//Category");

            foreach (XmlNode group in groups)
            {
                XmlNodeList recs = group.ChildNodes;
                foreach (XmlNode rec in recs)
                {
                    string category = rec.ParentNode.Attributes.GetNamedItem("Category").Value;
                    if (record.Category != null && record.Category != category)
                        continue;

                    string time = rec.Attributes.GetNamedItem("Time").Value;
                    var speedEstimation = TrackTimeHelper.Estimate(time);
                    if (record.Time != null && record.Time != speedEstimation)
                        continue;

                    string manufacturer = rec.Attributes.GetNamedItem("Manufacturer").Value;
                    if (record.Manufacturer != null && record.Manufacturer != manufacturer)
                        continue;

                    string driver = rec.Attributes.GetNamedItem("Driver").Value;
                    if (record.Driver != null && record.Driver != driver)
                        continue;

                    string date = rec.Attributes.GetNamedItem("Date").Value;
                    string year = date.Substring(date.Length - 4);
                    if (record.Date != null && record.Date != year)
                        continue;

                    records.Add(new Record
                    {
                        Category = category,
                        Time = time,
                        Manufacturer = manufacturer,
                        Model = rec.Attributes.GetNamedItem("Model").Value,
                        Driver = driver,
                        Date = date,
                        Video = rec.Attributes.GetNamedItem("Video").Value,
                    });
                }
            }

            return records;
        }
    }
}
