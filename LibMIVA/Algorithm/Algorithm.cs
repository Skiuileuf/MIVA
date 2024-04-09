using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibMIVA.Algorithm
{
    public abstract class Algorithm
    {
        public const double workHoursPerRobot = 200;

        public abstract List<Knapsack> Run();

        public static void ExportAlgorithmResultsToCSV(List<Knapsack> results, string filepath)
        {
            using (var writer = new StreamWriter(filepath))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(results);
                }
            }
        }
    }
}
