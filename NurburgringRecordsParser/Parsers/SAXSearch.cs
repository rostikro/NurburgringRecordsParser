using NurburgringRecordsParser.Helpers;
using NurburgringRecordsParser.Interfaces;
using NurburgringRecordsParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NurburgringRecordsParser.Parsers
{
    internal class SAXSearch : ISearch
    {
        XmlReader _reader;

        public SAXSearch(XmlReader reader)
        {
            _reader = XmlReader.Create(reader, null);
        }

        public List<Record> Search(Record record)
        {
            List<Record> records = new();
            string category = null;
            Record rec = null;

            while (_reader.Read())
            {
                if (_reader.Name == "Category")
                {
                    while (_reader.MoveToNextAttribute())
                    {
                        category = _reader.Value;
                    }
                    continue;
                }

                if (_reader.Name == "Record")
                {
                    rec = new Record { Category = category };

                    while (_reader.MoveToNextAttribute())
                    {
                        if (_reader.Name == "Time")
                        {
                            rec.Time = _reader.Value;
                            continue;
                        }
                        if (_reader.Name == "Manufacturer")
                        {
                            rec.Manufacturer = _reader.Value;
                            continue;
                        }
                        if (_reader.Name == "Model")
                        {
                            rec.Model = _reader.Value;
                            continue;
                        }
                        if (_reader.Name == "Driver")
                        {
                            rec.Driver = _reader.Value;
                            continue;
                        }
                        if (_reader.Name == "Date")
                        {
                            rec.Date = _reader.Value;
                            continue;
                        }
                        if (_reader.Name == "Video")
                        {
                            rec.Video = _reader.Value;
                            continue;
                        }
                    }

                    records.Add(rec);
                }
            }

            return (from val in records
                    where
                    (record.Category == null || record.Category == val.Category) &&
                    (record.Time == null || record.Time == TrackTimeHelper.Estimate(val.Time)) &&
                    (record.Manufacturer == null || record.Manufacturer == val.Manufacturer) &&
                    (record.Driver == null || record.Driver == val.Driver) &&
                    (record.Date == null || record.Date == val.Date[^4..])
                    select val).ToList();
        }
    }
}
