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
    public partial class  Home : Form
    {
        private void pnlNavAllPlaylists_Click(object sender, EventArgs e)
        {
            showFavouritesOnly = false; // turn off favourites-only filter
            SetActiveFilterHighlight(); // update highlight styling
            RefreshPlaylistView();      // rebuild the list
        }

        // Switches to "Favourites" view and refreshes
        private void pnlNavFavourites_Click(object sender, EventArgs e)
        {
            showFavouritesOnly = true;  // turn on favourites-only filter

            SetActiveFilterHighlight(); // update highlight styling
            RefreshPlaylistView();      // rebuild the list
        }

        // Colours the "All playlists" / "Favourites" buttons to show which is active
        private void SetActiveFilterHighlight()
        {
            Color activeBackground = Color.FromArgb(237, 233, 254); // light purple
            Color activeText = Color.FromArgb(124, 92, 255);        // purple text
            Color inactiveText = Color.FromArgb(75, 85, 99);        // grey text
            Color inactiveCount = Color.FromArgb(156, 163, 175);    // lighter grey text

            // "All playlists" button styling
            pnlNavAllPlaylists.BackColor = showFavouritesOnly ? Color.White : activeBackground;
            lblNavAllPlaylistsText.ForeColor = showFavouritesOnly ? inactiveText : activeText;
            lblNavAllPlaylistsCount.ForeColor = showFavouritesOnly ? inactiveCount : activeText;

            // "Favourites" button styling
            pnlNavFavourites.BackColor = showFavouritesOnly ? activeBackground : Color.White;
            lblNavFavouritesText.ForeColor = showFavouritesOnly ? activeText : inactiveText;
            lblNavFavouritesCount.ForeColor = showFavouritesOnly ? activeText : inactiveCount;
        }

        // Rebuilds the sidebar quick-list and main grid from what's on disk
        private void RefreshPlaylistView()
        {
            // remove old dynamic sidebar rows before rebuilding
            int r = 0;
            for (r = 0; r < dynamicNavRowCount; r++)
            {
                pnlSidebar.Controls.Remove(dynamicNavRows[r]); // detach from UI
                dynamicNavRows[r].Dispose();                    // free resources
                dynamicNavRows[r] = null;                       // clear reference
            }
            dynamicNavRowCount = 0; // reset counter

            // remove old dynamic grid cards before rebuilding
            for (r = 0; r < dynamicCardCount; r++)
            {
                flpPlaylists.Controls.Remove(dynamicCards[r]); // detach from UI
                dynamicCards[r].Dispose();                      // free resources
                dynamicCards[r] = null;                         // clear reference
            }
            dynamicCardCount = 0; // reset counter

            string[] allPlaylistFiles = Directory.GetFiles(playlistsFolder, "*.txt"); // every playlist file
            string[] favourites = ReadAllLinesSafe(favouritesFile); // favourited playlist names
            string[] recents = ReadAllLinesSafe(recentFile);        // recently opened playlist names

            lblNavAllPlaylistsCount.Text = allPlaylistFiles.Length.ToString(); // total playlist count

            // count how many playlists are favourited
            int favouriteTotal = 0;
            int f = 0;
            for (f = 0; f < allPlaylistFiles.Length; f++)
            {
                string favCheckName = Path.GetFileNameWithoutExtension(allPlaylistFiles[f]); // name without .txt
                if (StringArrayContains(favourites, favCheckName))
                    favouriteTotal++; // matched a favourite
            }
            lblNavFavouritesCount.Text = favouriteTotal.ToString(); // show favourite count

            // apply favourites-only filter if active
            string[] visibleFiles = showFavouritesOnly
                ? FilterToFavourites(allPlaylistFiles, favourites)
                : allPlaylistFiles;

            string searchQuery = GetActiveSearchQuery();               // current typed search text
            string[] searchedFiles = FilterBySearch(visibleFiles, searchQuery); // narrow down by search
            string[] orderedFiles = OrderByFavouriteThenRecentThenName(searchedFiles, favourites, recents); // sort

            // build a row/card for every playlist in the ordered list
            int i = 0;
            for (i = 0; i < orderedFiles.Length; i++)
            {
                string path = orderedFiles[i];                              // file path
                string name = Path.GetFileNameWithoutExtension(path);       // playlist name
                int trackCount = CountTracksInFile(path);                   // number of tracks

                if (i < MaxSidebarRows) // only the first N appear in the sidebar
                {
                    int y = 280 + 42 * i;                                   // vertical position for this row
                    Panel row = BuildNavRow(name, trackCount, path, y);     // build the row
                    pnlSidebar.Controls.Add(row);                           // add to sidebar
                    dynamicNavRows[dynamicNavRowCount] = row;               // track for next teardown
                    dynamicNavRowCount++;
                }

                Panel card = BuildPlaylistCard(name, trackCount, path); // build the grid card
                flpPlaylists.Controls.Add(card);                        // add to grid
                dynamicCards[dynamicCardCount] = card;                  // track for next teardown
                dynamicCardCount++;
            }

            ShowOrHideEmptyState(orderedFiles.Length == 0); // show message if list is empty
            RefreshStatistics();                             // recompute the 3 stat tiles
        }

        // Shows/hides the "no playlists" message in the main grid
        private void ShowOrHideEmptyState(bool shouldShow)
        {
            if (shouldShow)
            {
                if (emptyStateLabel == null) // create the label once, lazily
                {
                    emptyStateLabel = new Label();
                    emptyStateLabel.AutoSize = false;
                    emptyStateLabel.Font = new Font("Segoe UI", 9.5F);
                    emptyStateLabel.ForeColor = Color.FromArgb(107, 114, 128);
                    emptyStateLabel.Location = new Point(32, 408);
                    emptyStateLabel.Size = new Size(500, 24);
                    pnlMainContent.Controls.Add(emptyStateLabel);
                }
                string activeSearch = GetActiveSearchQuery(); // current search text, if any
                if (activeSearch.Length > 0)
                {
                    emptyStateLabel.Text = "No playlists or songs match \"" + activeSearch + "\"."; // no search matches
                }
                else
                {
                    // message depends on which filter is active
                    emptyStateLabel.Text = showFavouritesOnly
                        ? "No favourite playlists yet - star one, or switch back to \"All playlists\"."
                        : "No playlists yet - click \"+ New Playlist\" to create your first one.";
                }
                emptyStateLabel.Visible = true; // show the message
            }
            else if (emptyStateLabel != null)
            {
                emptyStateLabel.Visible = false; // hide the message
            }
        }

        // Builds one sidebar row control for a single playlist
        private Panel BuildNavRow(string title, int trackCount, string filePath, int yPosition)
        {
            Panel row = new Panel();
            row.BackColor = Color.White;
            row.Cursor = Cursors.Hand;             // shows a hand cursor on hover
            row.Location = new Point(15, yPosition); // vertical position passed in
            row.Size = new Size(270, 40);
            row.Tag = filePath;                     // remember which file this row represents
            row.Click += pnlPlaylistNavRow_Click;   // open playlist on click

            Label text = new Label();
            text.Font = new Font("Segoe UI", 9F);
            text.ForeColor = Color.FromArgb(55, 65, 81);
            text.Location = new Point(15, 10);
            text.Size = new Size(200, 22);
            text.Text = "♫   " + title;             // playlist name with a music note
            text.TextAlign = ContentAlignment.MiddleLeft;
            text.Click += pnlPlaylistNavRow_Click;  // clicking the text also opens it

            Label count = new Label();
            count.Font = new Font("Segoe UI", 9F);
            count.ForeColor = Color.FromArgb(156, 163, 175);
            count.Location = new Point(230, 9);
            count.Size = new Size(30, 22);
            count.Text = trackCount.ToString();      // number of tracks
            count.TextAlign = ContentAlignment.MiddleRight;
            count.Click += pnlPlaylistNavRow_Click;  // clicking the count also opens it

            row.Controls.Add(text);
            row.Controls.Add(count);
            return row;
        }

        // Opens a playlist when a sidebar row (or its labels) is clicked
        private void pnlPlaylistNavRow_Click(object sender, EventArgs e)
        {
            this.Controls.Remove(playlistPanel);
            playlistPanel.Dispose();

            Control clicked = sender as Control;
            if (clicked == null)
                return;
            // sender may be the row Panel itself or one of its child labels
            Control row = (clicked is Panel) ? clicked : clicked.Parent;
            if (row == null)
                return;
            string filePath = row.Tag as string; // file path stored on the row
            OpenPlaylist(filePath);
        }
    }
}
