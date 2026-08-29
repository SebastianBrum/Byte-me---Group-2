using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Byte_me___Group_2
{
    public partial class Home : Form
    {
        // Rewrites a favourites/recent file, dropping any line matching "name"
        private void RemoveNameFromFile(string filePath, string name)
        {
            string[] existing = ReadAllLinesSafe(filePath); // current lines
            using (StreamWriter writer = new StreamWriter(filePath, false)) // overwrite the file
            {
                int i = 0;
                for (i = 0; i < existing.Length; i++)
                {
                    if (!string.Equals(existing[i].Trim(), name, StringComparison.OrdinalIgnoreCase)
                        && existing[i].Trim().Length > 0)
                    {
                        writer.WriteLine(existing[i]); // keep every line except the matching one
                    }
                }
            }
        }

        private string[] FilterToFavourites(string[] files, string[] favourites)
        {
            string[] buffer = new string[files.Length]; // oversized temp array
            int count = 0;
            int i = 0;
            for (i = 0; i < files.Length; i++)
            {
                string name = Path.GetFileNameWithoutExtension(files[i]);
                if (StringArrayContains(favourites, name))
                {
                    buffer[count] = files[i]; // keep this file
                    count++;
                }
            }
            string[] result = new string[count]; // trim to actual size
            Array.Copy(buffer, result, count);
            return result;
        }
    }
}
