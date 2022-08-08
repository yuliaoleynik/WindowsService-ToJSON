using ChoETL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyService
{
    internal class ETL
    {
        public ETL() { }

        public void Save(string pathFrom, string pathTo)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            int count = new DirectoryInfo(@"C:\C#\A").GetFiles().Length;
            try
            {
                if (pathFrom.EndsWith(".txt"))
                {
                    using (var reader = new StreamReader(pathFrom))
                    {
                        using (var writer = new ChoJSONWriter<TransactionJSON>(pathTo + $@"\sample{count}.json"))
                        {
                            writer.Write(TxtToObject(reader));
                        }
                    }
                }
                else
                {
                    using (var jw = new ChoJSONWriter<TransactionJSON>(pathTo + $@"\sample{count}.json"))
                    {
                        using (var cr = new ChoCSVReader<TransactionCSV>(pathFrom)
                            .WithFirstLineHeader()
                            .WithDelimiter(",")
                            .MayHaveQuotedFields()
                            )
                        {
                            List<TransactionCSV> objList = cr.ToList();
                            jw.Write(Transform(objList));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (var log = new StreamWriter(pathTo + @"\sample.txt"))
                {
                    var trace = new StackTrace(ex, true);

                    foreach (var frame in trace.GetFrames())
                    {
                        log.Write($"Строка: {frame.GetFileLineNumber()}");
                        log.Write($"Столбец: {frame.GetFileColumnNumber()}");
                        log.Write($"Метод: {frame.GetMethod()}");
                    }
                }
            }
        }

        public List<TransactionJSON> Transform(List<TransactionCSV> input)
        {
            var output = new List<TransactionJSON>();
            var groupByCity = new Dictionary<string, List<TransactionCSV>>();

            foreach(var el in input)
            {
                if(!groupByCity.ContainsKey(el.GetCity()))
                {
                    groupByCity[el.GetCity()] = new List<TransactionCSV>();
                }
                groupByCity[el.GetCity()].Add(el);
            }

            foreach(var el in groupByCity)
            {
                output.Add(new TransactionJSON(el.Value));
            }
            return output;
        }

        public List<TransactionJSON> TxtToObject(StreamReader input)
        {
            var output = new List<TransactionCSV>();
            while (!input.EndOfStream)
            {
                var readLine = input.ReadLine();
                if (readLine == null) continue;
                var lines = Regex.Split(readLine, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                output.Add(new TransactionCSV
                {
                    first_name = lines[0],
                    last_name = lines[1],
                    address = lines[2],
                    payment = decimal.Parse(lines[3]),
                    date = DateTime.Parse(lines[4]),
                    account_number = long.Parse(lines[5]),
                    service = lines[6]
                });
                
            }
            return Transform(output);
        }

        public void Validate()
        {

        }
    }
}
