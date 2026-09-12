using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

//I changed this for test 2
namespace Byte_me___Group_2
{
    // Home page: dashboard visuals (welcome message, search, theme toggle) + playlist functionality.
    // Data is stored as plain text files under a "Data" folder next to the .exe.\
    public partial class Home : Form
    {
        private string username;
        

        // Text shown in the search box when empty and unfocused
        private const string SearchPlaceholder = "Search playlists or songs...";
        // Tracks light/dark toggle state (visual only for now)
        private bool isDarkMode = false;

        //  File-backed data layer
        private readonly string dataFolder;        // Data folder path
        private readonly string userFolder;
        private readonly string playlistsFolder;    // Data\Playlists folder path
        private readonly string coversFolder;        // Data\Covers folder path
        private readonly string favouritesFile;       // favourites.txt path
        private readonly string recentFile;            // recent.txt path
        private const int MaxRecentEntries = 6;         // max lines kept in recent.txt

        // True = only show favourited playlists in sidebar/grid
        private bool showFavouritesOnly = false;
        // Dynamically built sidebar rows (replace the fixed demo rows)
        private readonly Panel[] dynamicNavRows = new Panel[200];
        private int dynamicNavRowCount = 0;
        // Dynamically built grid cards (replace the fixed demo cards)
        private readonly Panel[] dynamicCards = new Panel[200];
        private int dynamicCardCount = 0;

        // Label shown when there are zero playlists to display
        private Label emptyStateLabel;
        // Cap on how many rows fit in the sidebar quick-list (space only, not a data cap)
        private const int MaxSidebarRows = 12;

        // Palette used to colour playlist "covers" deterministically by name
        private static readonly Color[] CoverPalette = new Color[]
        {
            Color.FromArgb(109, 74, 224),
            Color.FromArgb(139, 111, 240),
            Color.FromArgb(190, 168, 245),
            Color.FromArgb(88, 58, 196),
            Color.FromArgb(216, 189, 247),
            Color.FromArgb(189, 101, 214)
        };

        PlaylistControl playlistPanel;

        //username = parameter received from Form1.cs
        public Home(string username)
        {
            InitializeComponent(); // build Designer-generated controls
            
            //Sets the username
            this.username = username;

            SetWelcomeUsername(username);

            // Build the paths to the data folder and its files
            dataFolder = Path.Combine(Application.StartupPath, "Data");

            userFolder = Path.Combine(dataFolder, username);
            Directory.CreateDirectory(userFolder);

            playlistsFolder = Path.Combine(userFolder, "Playlists");
            Directory.CreateDirectory(playlistsFolder);

            coversFolder = Path.Combine(userFolder, "Covers");
            Directory.CreateDirectory(coversFolder);

            favouritesFile = Path.Combine(userFolder, "favourites.txt");

            // Recent playlists opened
            recentFile = Path.Combine(userFolder, "recent.txt");

            lblAvatar.Text = this.username[0].ToString();

            EnsureStorageExists();       // create folders/files if missing
            HideLegacyFixedSlots();      // hide the Designer's 6 demo rows/cards
            SetActiveFilterHighlight();  // highlight "All playlists" as active
            RefreshPlaylistView();       // populate sidebar/grid from disk
        }
        //Add an option to add a playlist to favourites
        



        // Hides the Designer's original 6 hardcoded sidebar rows and 6 grid cards permanently
        private void HideLegacyFixedSlots()
        {
            pnlNavLateNightDrive.Visible = false;
            pnlNavFocusFlow.Visible = false;
            pnlNavSundaySoul.Visible = false;
            pnlNavGymPulse.Visible = false;
            pnlNavAcousticCorner.Visible = false;
            pnlNavThrowback2000s.Visible = false;
            pnlPlaylistCard1.Visible = false;
            pnlPlaylistCard2.Visible = false;
            pnlPlaylistCard3.Visible = false;
            pnlPlaylistCard4.Visible = false;
            pnlPlaylistCard5.Visible = false;
            pnlPlaylistCard6.Visible = false;
        }

        public void adjustFormHomeWidth(int width)
        {
            this.Width = pnlSidebar.Width + width;
        }

        // Reads all lines from a file, returning an empty array if it's missing or unreadable
        private string[] ReadAllLinesSafe(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return new string[0]; // no file yet, treat as empty
                return File.ReadAllLines(filePath);
            }
            catch (Exception)
            {
                return new string[0]; // read failed, fail safe rather than crash
            }
        }

        // Case-insensitive check for whether value exists (trimmed) in array
        private bool StringArrayContains(string[] array, string value)
        {
            int i = 0;
            while (i < array.Length)
            {
                if (string.Equals(array[i].Trim(), value.Trim(), StringComparison.OrdinalIgnoreCase))
                    return true;
                i++;
            }
            return false;
        }

        // Counts non-blank lines (tracks) in a playlist file
        private int CountTracksInFile(string filePath)
        {
            int count = 0;
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Trim().Length > 0)
                            count++; // only count non-empty lines
                    }
                }
            }
            catch (Exception)
            {
                count = 0; // unreadable file, treat as 0 tracks
            }
            return count;
        }

        // Lets the user browse for a playlist .txt file to open directly
        private void lnkBrowseForFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open a playlist file";
                dlg.Filter = "Playlist text files (*.txt)|*.txt"; // only allow .txt
                dlg.InitialDirectory = playlistsFolder;             // start in the Playlists folder
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    OpenPlaylist(dlg.FileName); // open whatever file was chosen
                }
            }
        }

        // Validates a playlist file, logs it as recently opened, and "opens" it
        private void OpenPlaylist(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    MessageBox.Show("That playlist file could not be found. It may have been moved or deleted.",
                        "Playlist not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    RefreshPlaylistView(); // list may be stale, rebuild it
                    return;
                }
                string name = Path.GetFileNameWithoutExtension(filePath);
                LogRecentOpen(name);     // record this playlist as recently opened
                RefreshPlaylistView();   // reflect the new "recent" ordering immediately

                // Opens the playlist page
                playlistPanel = new PlaylistControl(this, this.username, this.coversFolder, this.playlistsFolder, this.dataFolder, ofdCoverPicture, pnlMainContent, pnlSidebar, btnNewPlaylist, btnChangeCoverPhoto);
                playlistPanel.setupPlaylistPage(name);

                this.Controls.Add(playlistPanel);
                playlistPanel.Top = pnlMainContent.Top;
                playlistPanel.Left = pnlMainContent.Left;
                playlistPanel.BringToFront();

                adjustFormHomeWidth(playlistPanel.Width);
            }
            catch (Exception ex)
            {
                MessageBox.Show("This playlist could not be opened:\n" + ex.Message,
                    "Error opening playlist", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        // Moves playlistName to the top of recent.txt, capped at MaxRecentEntries
        private void LogRecentOpen(string playlistName)
        {
            try
            {
                string[] existing = ReadAllLinesSafe(recentFile);      // current recent list
                string[] rebuilt = new string[existing.Length + 1];    // room for the new entry
                rebuilt[0] = playlistName;                              // new entry goes first
                int writeIndex = 1;
                int i = 0;
                for (i = 0; i < existing.Length; i++)
                {
                    // copy over every other entry, skipping a duplicate of this playlist
                    if (!string.Equals(existing[i].Trim(), playlistName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (writeIndex < rebuilt.Length)
                        {
                            rebuilt[writeIndex] = existing[i];
                            writeIndex++;
                        }
                    }
                }
                using (StreamWriter writer = new StreamWriter(recentFile, false)) // overwrite recent.txt
                {
                    int count = 0;
                    i = 0;
                    while (i < writeIndex && count < MaxRecentEntries) // cap total lines written
                    {
                        if (!string.IsNullOrWhiteSpace(rebuilt[i]))
                        {
                            writer.WriteLine(rebuilt[i]);
                            count++;
                        }
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not update recent.txt: " + ex.Message); // non-fatal, just log
            }
        }

        // Highlights and scrolls to the row/card matching the given file path
        private void SelectPlaylistByPath(string filePath)
        {
            Panel row = FindPanelByTag(pnlSidebar, filePath); // find the matching sidebar row
            if (row != null)
            {
                row.BackColor = Color.FromArgb(237, 233, 254); // highlight it
            }
            Panel card = FindPanelByTag(flpPlaylists, filePath); // find the matching grid card
            if (card != null)
            {
                card.BackColor = Color.FromArgb(250, 248, 255);   // highlight it
                pnlMainContent.ScrollControlIntoView(card);       // scroll it into view
            }
        }

        // Searches a container's direct children for a Panel whose Tag matches tagValue
        private Panel FindPanelByTag(Control container, string tagValue)
        {
            int i = 0;
            for (i = 0; i < container.Controls.Count; i++)
            {
                Panel p = container.Controls[i] as Panel;
                if (p != null && (p.Tag as string) == tagValue)
                {
                    return p; // found the match
                }
            }
            return null; // not found
        }

        // Creates the Data folder structure and empty favourites/recent files if missing
        private void EnsureStorageExists()
        {
            try
            {
                if (!Directory.Exists(dataFolder)) Directory.CreateDirectory(dataFolder);
                if (!Directory.Exists(playlistsFolder)) Directory.CreateDirectory(playlistsFolder);
                if (!Directory.Exists(coversFolder)) Directory.CreateDirectory(coversFolder);
                if (!File.Exists(favouritesFile)) File.Create(favouritesFile).Dispose(); // create + close immediately
                if (!File.Exists(recentFile)) File.Create(recentFile).Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not set up the application's data folder:\n" + ex.Message,
                    "Startup error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Deletes a playlist
        public void deletePlaylist(string playlist, string filePath)
        {
            DialogResult confirm = MessageBox.Show(
                "Delete \"" + playlist + "\"? This cannot be undone.",
                "Delete playlist", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.No)
                return; // user backed out

            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath); // remove the playlist file
                RemoveNameFromFile(favouritesFile, playlist); // scrub from favourites
                RemoveNameFromFile(recentFile, playlist);     // scrub from recents

                string coverPath = Path.Combine(coversFolder, playlist + ".png");
                if (File.Exists(coverPath))
                    File.Delete(coverPath); // remove any cover art too

                RefreshPlaylistView(); // rebuild sidebar/grid without the deleted playlist
            }
            catch (Exception ex)
            {
                MessageBox.Show("This playlist could not be deleted:\n" + ex.Message,
                    "Error deleting playlist", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnChangeCoverPhoto_Click(object sender, EventArgs e)
        {
            playlistPanel.changeCoverPhoto();
        }

        //Closes all the open forms when you press the red x
        private void Home_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        // Fires when the heart button on a sidebar row is clicked ---> for favourites

        // Toggles a playlist's favourite status in favourites.txt
        private void ToggleFavourite(string playlistName)
        {
            string[] existing = ReadAllLinesSafe(favouritesFile);

            if (StringArrayContains(existing, playlistName))
            {
                RemoveNameFromFile(favouritesFile, playlistName);
            }
            else
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(favouritesFile, true))
                    {
                        writer.WriteLine(playlistName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not save the favourite:\n" + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            RefreshPlaylistView();
        }

        private void FavHeart_Click(object sender, EventArgs e)
        {
            Control c = sender as Control;

            if (c != null && c.Tag is string)
            {
                string filePath = (string)c.Tag;
                string name = Path.GetFileNameWithoutExtension(filePath);
                ToggleFavourite(name);
            }
        }
        

        


    }
}
