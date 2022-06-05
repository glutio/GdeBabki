using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
namespace GdeBabki.Client.Services
{
    public class CsvParser
    {
        StringBuilder sb = new();

        public async Task<List<string[]>> LoadAsync(Stream stream, int linesToLoad = 0)
        {
            var lines = new List<string[]>();

            using (var reader = new StreamReader(stream))
            {
                var numberOfColumns = 0;
                while (linesToLoad <= 1 || lines.Count < linesToLoad)
                {
                    var line = await reader.ReadLineAsync();
                    if (line == null)
                        break;

                    var columns = Parse(line).ToArray();
                    lines.Add(columns);

                    numberOfColumns = Math.Max(numberOfColumns, columns.Length);
                }
            }

            return lines;
        }

        List<string> Parse(string line)
        {
            var columns = new List<string>();

            bool isQuoted = false;
            int nQuotes = 0;

            sb.Clear();
            foreach(var c in line)
            {
                if (sb.Length == 0 && !isQuoted && c == '"')
                {
                    isQuoted = true;
                    continue;
                }
                
                if (isQuoted)
                {
                    if (c == '"')
                    {
                        nQuotes++;
                        continue;
                    }
                    else
                    {
                        if (nQuotes > 0)
                        {
                            sb.Append('"', nQuotes / 2);
                            if (nQuotes % 2 != 0)
                            {
                                isQuoted = false;
                            }
                            nQuotes = 0;
                        }
                    }
                }

                if (!isQuoted && c == ',')
                {
                    columns.Add(sb.ToString());
                    sb.Clear();
                    continue;
                }

                sb.Append(c);
            }

            if (nQuotes > 0)
            {
                sb.Append('"', nQuotes / 2);
            }

            columns.Add(sb.ToString());

            return columns;
        }
    }
}
