using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Byte_me___Group_2
{
    public partial class Home : Form
    {
        private string GetActiveSearchQuery()
        {
            if (string.IsNullOrEmpty(txtSearch.Text) || txtSearch.Text == SearchPlaceholder)
                return "";
            return txtSearch.Text.Trim();
        }

        // Keeps files whose name OR whose track lines contain the search query
        private string[] FilterBySearch(string[] files, string query)
        {
            if (string.IsNullOrEmpty(query))
                return files; // nothing typed, keep everything
            string[] buffer = new string[files.Length];
            int count = 0;
            int i = 0;
            for (i = 0; i < files.Length; i++)
            {
                string name = Path.GetFileNameWithoutExtension(files[i]);
                bool matches = name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0; // name match
                if (!matches)
                {
                    string[] lines = ReadAllLinesSafe(files[i]); // check each track line too
                    int t = 0;
                    for (t = 0; t < lines.Length && !matches; t++)
                    {
                        if (lines[t].IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                            matches = true; // matched a song title/artist/duration line
                    }
                }
                if (matches)
                {
                    buffer[count] = files[i];
                    count++;
                }
            }
            string[] result = new string[count];
            Array.Copy(buffer, result, count);
            return result;
        }

        // Sorts playlists: favourites first, then recently opened, then everything else
        private string[] OrderByFavouriteThenRecentThenName(string[] files, string[] favourites, string[] recents)
        {
            int n = files.Length;
            string[] result = new string[n];
            Array.Copy(files, result, n); // working copy to sort in place

            int[] scores = new int[n]; // 0 = favourite, 1 = recent, 2 = other
            int i = 0;
            for (i = 0; i < n; i++)
            {
                string name = Path.GetFileNameWithoutExtension(result[i]);
                if (StringArrayContains(favourites, name))
                    scores[i] = 0;
                else if (StringArrayContains(recents, name))
                    scores[i] = 1;
                else
                    scores[i] = 2;
            }

            // bubble sort by score, keeping equal-score items in their original order
            int a = 0;
            for (a = 0; a < n - 1; a++)
            {
                int b = 0;
                for (b = 0; b < n - 1 - a; b++)
                {
                    if (scores[b] > scores[b + 1])
                    {
                        int tempScore = scores[b];
                        scores[b] = scores[b + 1];
                        scores[b + 1] = tempScore;
                        string tempFile = result[b];
                        result[b] = result[b + 1];
                        result[b + 1] = tempFile;
                    }
                }
            }
            return result;
        }

        // Re-filters the playlist view live as the user types in the search box
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtSearch.Text == SearchPlaceholder)
                return; // ignore the placeholder text itself
            try
            {
                RefreshPlaylistView(); // apply the new search text
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong while searching your playlists:\n" + ex.Message,
                    "Search error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---- Search box placeholder behaviour ----
        // Clears the placeholder text when the search box gains focus
        private void txtSearch_GotFocus(object sender, EventArgs e)
        {
            if (txtSearch.Text == SearchPlaceholder)
            {
                txtSearch.Text = "";                              // remove placeholder
                txtSearch.ForeColor = Color.FromArgb(31, 41, 55); // normal text colour
            }
        }

        // Restores the placeholder text when the search box loses focus and is empty
        private void txtSearch_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = SearchPlaceholder;                    // show placeholder again
                txtSearch.ForeColor = Color.FromArgb(156, 163, 175);   // greyed-out colour
            }
        }
    }
}
