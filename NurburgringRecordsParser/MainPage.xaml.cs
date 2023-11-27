using NurburgringRecordsParser.FileManager;
using NurburgringRecordsParser.Helpers;
using NurburgringRecordsParser.Interfaces;
using NurburgringRecordsParser.Logging;
using NurburgringRecordsParser.Models;
using NurburgringRecordsParser.Parsers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace NurburgringRecordsParser
{
    public partial class MainPage : ContentPage
    {
        Logger _logger;

        ObservableCollection<string> _category = new();
        ObservableCollection<string> _time = new();
        ObservableCollection<string> _manufacturer = new();
        ObservableCollection<string> _driver = new();
        ObservableCollection<string> _year = new();

        IFile _mainFile;
        IFile _xslMainFile;
        IFile _filteredFile;
        IFile _htmlFile;

        bool isFiltered = false;

        public MainPage()
        {
            _logger = Logger.GetInstance();

            InitializeComponent();

            CategoryPicker.ItemsSource = _category;
            TimePicker.ItemsSource = _time;
            ManufacturerPicker.ItemsSource = _manufacturer;
            DriverPicker.ItemsSource = _driver;
            YearPicker.ItemsSource = _year;
        }

        async void OnSelectFileClicked(object sender, EventArgs e)
        {
            try
            {
                await SelectFileAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        void OnSaveFilesToGoogleDriveClicked(object sender, EventArgs e)
        {
            try
            {
                SaveFilesToGoogleDrive();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message); 
            }
        }

        void OnClearFiltersButtonClicked(object sender, EventArgs e)
        {
            ClearFilters();
        }

        void OnSearchButtonClicked(object sender, EventArgs e)
        {
            try
            {
                SearchRecords();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        void OnTransformButtonClicked(object sender, EventArgs e)
        {
            try
            {
                TransformToHtml();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        void SaveFilesToGoogleDrive()
        {
            var t1 = _mainFile?.SaveToDrive();
            var t2 = _xslMainFile?.SaveToDrive();
            var t3 = _filteredFile?.SaveToDrive();
            var t4 = _htmlFile?.SaveToDrive();

            Task.WaitAll(t1, t2, t3, t4);
        }

        void TransformToHtml()
        {
            _htmlFile = FileFactory.CreateFile(FileTypes.HTML, _mainFile.FilePath.Replace(".xml", ".html"), _mainFile.FileName.Replace(".xml", ".html"), false);

            XslCompiledTransform xsl = new();
            xsl.Load(_xslMainFile.GetXmlReader());

            if (isFiltered)
            {
                xsl.Transform(_filteredFile.GetXmlReader(), _htmlFile.GetXmlWriter());
            }
            else
            {
                xsl.Transform(_mainFile.GetXmlReader(), _htmlFile.GetXmlWriter());
            }

            _htmlFile.Save();
        }

        void SearchRecords()
        {
            var record = FilteredRecord();
            ISearch search;

            if (LINQRadioButton.IsChecked)
            {
                search = new LINQSearch(_mainFile.GetXmlReader());
            }

            else if (DOMRadioButton.IsChecked)
            {
                search = new DOMSearch(_mainFile.GetXmlReader());
            }

            else if (SAXRadioButton.IsChecked)
            {
                search = new SAXSearch(_mainFile.GetXmlReader());
            }
            else return;

            var records = search.Search(record);
            isFiltered = true;

            _logger.LogFilter(record);

            OutputRecords(records);
            FilteredFile(records);
        }

        void OutputRecords(List<Record> records)
        {
            StringBuilder str = new();
            int i = 0;

            foreach (var rec in records)
            {
                str.Append(++i + ". ");
                str.Append("Category: " + rec.Category + "\n");
                str.Append("Time: " + rec.Time + "\n");
                str.Append("Manufacturer: " + rec.Manufacturer + "\n");
                str.Append("Model: " + rec.Model + "\n");
                str.Append("Driver: " + rec.Driver + "\n");
                str.Append("Date: " + rec.Date + "\n");
                str.Append("Video: " + rec.Video + "\n\n");
            }

            TextBox.Text = str.ToString();
        }

        void FilteredFile(List<Record> records)
        {
            _filteredFile = FileFactory.CreateFile(FileTypes.XML, _mainFile.FilePath.Replace(".xml", "-filtered.xml"), _mainFile.FileName.Replace(".xml", "-filtered.xml"), false);

            XElement root = new XElement("NurburgringRecords");
            XElement category = null;

            foreach (var record in records)
            {
                if (category == null)
                {
                    category = new XElement("Category", new XAttribute("Category", record.Category));
                }

                if (record.Category != category.Attribute("Category").Value)
                {
                    root.Add(category);
                    category = new XElement("Category", new XAttribute("Category", record.Category));
                }

                category.Add(new XElement("Record",
                    new XAttribute("Time", record.Time),
                    new XAttribute("Manufacturer", record.Manufacturer),
                    new XAttribute("Model", record.Model),
                    new XAttribute("Driver", record.Driver),
                    new XAttribute("Date", record.Date),
                    new XAttribute("Video", record.Video)));
            }

            if (category != null)
                root.Add(category);

            _filteredFile.Add(root);
            _filteredFile.Save();
            _logger.LogSave(_filteredFile.FilePath);
        }

        Record FilteredRecord()
        {
            return new Record
            {
                Category = (CategoryCheck.IsChecked) ? CategoryPicker.SelectedItem as string : null,
                Time = (TimeCheck.IsChecked) ? TimePicker.SelectedItem as string : null,
                Manufacturer = (ManufacturerCheck.IsChecked) ? ManufacturerPicker.SelectedItem as string : null,
                Driver = (DriverCheck.IsChecked) ? DriverPicker.SelectedItem as string : null,
                Date = (YearCheck.IsChecked) ? YearPicker.SelectedItem as string : null,
            };
        }

        void ClearFilters()
        {
            CategoryCheck.IsChecked = false;
            TimeCheck.IsChecked = false;
            ManufacturerCheck.IsChecked = false;
            DriverCheck.IsChecked = false;
            YearCheck.IsChecked = false;

            CategoryPicker.SelectedIndex = -1;
            TimePicker.SelectedIndex = -1;
            ManufacturerPicker.SelectedIndex = -1;
            DriverPicker.SelectedIndex = -1;
            YearPicker.SelectedIndex = -1;

            SAXRadioButton.IsChecked = false;
            DOMRadioButton.IsChecked = false;
            LINQRadioButton.IsChecked = false;

            TextBox.Text = null;
        }

        async Task SelectFileAsync()
        {
            var fileTypes = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[] { ".xml" } },
                    });

            var options = new PickOptions()
            {
                FileTypes = fileTypes,
            };

            var result = await FilePicker.Default.PickAsync(options);
            if (result == null)
            {
                return;
            }

            _mainFile = FileFactory.CreateFile(FileTypes.XML, result.FullPath, result.FileName, true);
            _xslMainFile = FileFactory.CreateFile(FileTypes.XSL, result.FullPath.Replace(".xml", ".xsl"), result.FileName.Replace(".xml", ".xsl"), true);

            AnalyzeXml();
        }

        void AnalyzeXml()
        {
            var records = new LINQSearch(_mainFile.GetXmlReader()).Search(new Record());

            _category.Clear();
            _time.Clear();
            _manufacturer.Clear();
            _driver.Clear();
            _year.Clear();

            foreach (var rec in records)
            {
                if (!_category.Contains(rec.Category))
                    _category.Add(rec.Category);

                var speedEstimation = TrackTimeHelper.Estimate(rec.Time);
                if (!_time.Contains(speedEstimation))
                    _time.Add(speedEstimation);

                if (!_manufacturer.Contains(rec.Manufacturer))
                    _manufacturer.Add(rec.Manufacturer);

                if (!_driver.Contains(rec.Driver))
                    _driver.Add(rec.Driver);

                if (!_year.Contains(rec.Date[^4..]))
                    _year.Add(rec.Date[^4..]);
            }

            var sortableList = new List<string>(_year);
            sortableList.Sort();

            for (int i = 0; i < sortableList.Count; i++)
            {
                _year.Move(_year.IndexOf(sortableList[i]), i);
            }
        }
    }
}