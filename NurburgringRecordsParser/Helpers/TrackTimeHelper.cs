
using System.Diagnostics;

namespace NurburgringRecordsParser.Helpers
{
    internal class TrackTimeHelper
    {
        static Dictionary<float, string> speedEstimation = new Dictionary<float, string> {
            { 360.0f, "Incredibly fast (less than 6:00)" },
            { 420.0f, "Pretty fast (6:00 - 7:00)" },
            { 450.0f, "Fast (7:00 - 7:30)" },
            { 480.0f, "Slow (7:30 - 8:00)" },
        };

        public static string Estimate(string time)
        {
            if (time == null)
                return null;

            time = time[..time.IndexOf(".")];

            // Here TotalMinutes stands for TotalSeconds
            var seconds = TimeSpan.Parse(time).TotalMinutes;

            foreach (var est in speedEstimation)
            {
                if (seconds < est.Key)
                    return est.Value;
            }

            return "Pretty slow (more than 8:00)";
        }
    }
}
