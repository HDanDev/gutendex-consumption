using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Services
{
    // Service responsible of file writting
    public class FileWriter
    {
        public async Task WriteAsync(string path, IEnumerable<string> lines)
        {
            await File.WriteAllLinesAsync(path, lines);
        }
    }
}
