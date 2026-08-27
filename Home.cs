using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

//I changed this for test 2
namespace Byte_me___Group_2
{
    // Home page: dashboard visuals (welcome message, search, theme toggle) + playlist functionality.
    // Data is stored as plain text files under a "Data" folder next to the .exe.
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

            recentFile = Path.Combine(userFolder, "recent.txt");

            pnlPlaylist.Visible = false;
            pnlPlaylist.Enabled = false;
            pnlPlaylist.Location = pnlMainContent.Location;
            pnlPlaylist.Size = pnlPlaylist.Size;

            lblAvatar.Text = this.username[0].ToString();

            EnsureStorageExists();       // create folders/files if missing
            HideLegacyFixedSlots();      // hide the Designer's 6 demo rows/cards
            SetActiveFilterHighlight();  // highlight "All playlists" as active
            RefreshPlaylistView();       // populate sidebar/grid from disk
        }

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
                setupPlaylistPage(name);
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

        /*
         *\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
         *
         * Playlist Section
         * 
         *\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
         */

        //Lists for the songNames, artists, and durations
        List<string> songs = new List<string>();
        List<string> artists = new List<string>();
        List<string> songDurations = new List<string>();

        //Basic display setup when the user opens a playlist
        private void setupPlaylistPage(string title)
        {
            lblPlaylistName.Text = title;
            lblPlaylistPathName.Text = title;
            lblDateCreated.Text = null;
            lblWelecome.Text = $"Welcome back, {this.username}";
            playlistVisible(false);
            homeVisible(false);
            addPLaylistButton(false);
            changeCoverButton(false);
            readSongs(title);
            loadImage(title);

            lblPlalistCount.Text = $"This playlist has {songs.Count} songs";

            string creationDate = File.GetCreationTime(Path.Combine(playlistsFolder, $"{title}.txt")).ToString("dd MMMM yyyy");
            addToCreationLabel(creationDate);
        }

        //Goes back to the home screen when the user clicks on the filepath
        private void lblBreadcrumb_Click(object sender, EventArgs e)
        {
            playlistVisible(true);
            homeVisible(true);
            addPLaylistButton(true);
            changeCoverButton(true);
        }

        //Goes back to the home screen when the user clicks back
        private void btnBackHome_Click(object sender, EventArgs e)
        {
            playlistVisible(true);
            homeVisible(true);
            addPLaylistButton(true);
            changeCoverButton(true);
        }

        private void loadImage(string playlist)
        {
            // Write all the image files to an array
            string[] coverImagesFiles = Directory.GetFiles(coversFolder);

            //Boolean to check if the user selected a file for the playlist
            bool hasImage = false;

            // Iterate through the images in the folder
            foreach (string coverImage in coverImagesFiles)
            {
                // Gets the image name without the extension
                string currentImage = Path.GetFileNameWithoutExtension(coverImage);


                //  Checks if the current image in the loop is the correct one
                if (currentImage == playlist)
                {
                    pbxPlaylistCoverPhoto.Image = Image.FromFile(coverImage);
                    pbxPlaylistCoverPhoto.SizeMode = PictureBoxSizeMode.StretchImage;

                    // The user has a image for the playlist
                    hasImage = true;
                }
            }
            
            //Sets the default image if the user didn't set an image themselves
            if (!hasImage)
            {
                string defaultImagePath = Path.Combine(dataFolder, "DefaultCover", "default.png");
                pbxPlaylistCoverPhoto.Image = Image.FromFile(defaultImagePath);
                pbxPlaylistCoverPhoto.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        //Makeing the active playlist visible on a panel, or making it invisible if the home screen is showing
        private void playlistVisible(bool home)
        {
            pnlPlaylist.Visible = !home;
            pnlPlaylist.Enabled = !home;
        }

        //Making the home screen visible on a panel, or making it invisible if a playlist is active
        private void homeVisible(bool home)
        {
            pnlMainContent.Visible = home;
            pnlMainContent.Enabled = home;
        }

        private void addPLaylistButton(bool home)
        {
            btnNewPlaylist.Visible = !home;
            btnNewPlaylist.Enabled = !home;
        }

        private void changeCoverButton(bool home)
        {
            btnChangeCoverPhoto.Visible = !home;
            btnChangeCoverPhoto.Enabled = !home;
        }

        private void addToCreationLabel(string val)
        {
            if (lblDateCreated.Text != "")
            {
                lblDateCreated.Text += $"•{val}";
            } 
            else
            {
                lblDateCreated.Text = val;
            }
        }

        //Reads the songs from the textfile into the songs list
        private void readSongs(string playlistName)
        {
            string filepath = Path.Combine(
            dataFolder,
            this.username,
            "Playlists",
            playlistName + ".txt"
            );

            //Clears the lists from old values
            songs.Clear();
            artists.Clear();
            songDurations.Clear();

            //Clears the display panel from old songs
            flpSongs.Controls.Clear();
            Panel titlePanel = createTitlePanel();
            flpSongs.Controls.Add(titlePanel);

            //Reads from the textfile
            try
            {
                using (StreamReader reader = new StreamReader(filepath))
                {
                    int totalSongs = 0;
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        getSongTitle(line);
                        totalSongs++;
                    }

                    addToCreationLabel($"{totalSongs} tracks");
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Playlist file corrupted.");
            }
            catch (Exception)
            {
                MessageBox.Show("Please try again");
            }

            //Displays the songs in the panel
            displaySongs(playlistName);
        }

        //gets the song title from the textfile and saves it into the songs list
        private void getSongTitle(string line)
        {
            int characterPosition = line.IndexOf("|");
            songs.Add(line.Substring(0, characterPosition));

            getSongArtist(line.Substring(characterPosition + 1));
        }

        //Gets the songs artist from the textfile and saves it into the artists list
        private void getSongArtist(string line)
        {
            int characterPosition = line.IndexOf("|");
            artists.Add(line.Substring(0, characterPosition));
            getSongDuration(line.Substring(characterPosition + 1));
        }

        //Gets the song duration from the textfile and saves it into the songDurations List
        private void getSongDuration(string line)
        {
            songDurations.Add(line);
        }

        //Displays the songs into the display panel
        private void displaySongs(string playlistName)
        {
            for (int i = 0; i < songs.Count; i++)
            {
                Panel songPanel = createSongPanel(songs[i], artists[i], songDurations[i], playlistName,i);
                flpSongs.Controls.Add(songPanel);
            }
        }

        //Creates the headers for the display panel
        private Panel createTitlePanel()
        {
            Panel headingPanel = new Panel();
            headingPanel.Width = 784;
            headingPanel.Height = 30;
            headingPanel.Cursor = Cursors.Hand;

            Label Title = new Label();
            Title.Text = "Names";
            Title.Font = new Font("Microsoft Sans Sarif", 10, FontStyle.Bold);
            Title.Width = 53;
            Title.Height = 16;
            Title.AutoSize = false;
            Title.TextAlign = ContentAlignment.MiddleLeft;
            Title.Left = 17;
            Title.Top = 13;

            Label Artist = new Label();
            Artist.Text = "Artists";
            Artist.Font = new Font("Microsoft Sans Sarif", 10, FontStyle.Bold);
            Artist.Width = 53;
            Artist.Height = 16;
            Artist.AutoSize = false;
            Artist.TextAlign = ContentAlignment.MiddleLeft;
            Artist.Left = 241;
            Artist.Top = 13;

            Label Duration = new Label();
            Duration.Text = "Duration";
            Duration.Font = new Font("Microsoft Sans Sarif", 10, FontStyle.Bold);
            Duration.Width = 70;
            Duration.Height = 16;
            Duration.AutoSize = false;
            Duration.TextAlign = ContentAlignment.MiddleLeft;
            Duration.Left = 423;
            Duration.Top = 13;

            headingPanel.Controls.Add(Title);
            headingPanel.Controls.Add(Artist);
            headingPanel.Controls.Add(Duration);

            return headingPanel;
        }

        //Creates the song display
        private Panel createSongPanel(string songName, string artist, string duration, string playlistName, int songIndex)
        {
            //Song panel
            Panel songPanel = new Panel();
            songPanel.Width = 784;
            songPanel.Height = 30;
            songPanel.Cursor = Cursors.Hand;

            //Title
            Label songTitle = new Label();
            songTitle.Text = songName;
            songTitle.Font = new Font("Microsoft Sans Sarif", 8, FontStyle.Regular);
            songTitle.Width = 53;
            songTitle.Height = 16;
            songTitle.AutoSize = false;
            songTitle.TextAlign = ContentAlignment.MiddleLeft;
            songTitle.Left = 17;
            songTitle.Top = 13;

            //Artist
            Label songArtist = new Label();
            songArtist.Text = artist;
            songArtist.Font = new Font("Microsoft Sans Sarif", 8, FontStyle.Regular);
            songArtist.Width = 53;
            songArtist.Height = 16;
            songArtist.AutoSize = false;
            songArtist.TextAlign = ContentAlignment.MiddleLeft;
            songArtist.Left = 241;
            songArtist.Top = 13;

            //Duration
            Label songDuration = new Label();
            songDuration.Text = duration;
            songDuration.Font = new Font("Microsoft Sans Sarif", 8, FontStyle.Regular);
            songDuration.Width = 53;
            songDuration.Height = 16;
            songDuration.AutoSize = false;
            songDuration.TextAlign = ContentAlignment.MiddleLeft;
            songDuration.Left = 423;
            songDuration.Top = 13;

            //Delete button
            Button buttonDelete = new Button();
            buttonDelete.Width = 40;
            buttonDelete.Height = 24;
            buttonDelete.Cursor = Cursors.Hand;
            buttonDelete.Left = flpSongs.Width - buttonDelete.Width - 40;
            buttonDelete.Top = 3;
            buttonDelete.Text = "🗑️";
            buttonDelete.Font = new Font("Microsoft Sans Sarif", 8, FontStyle.Regular);
            buttonDelete.ForeColor = Color.Black;
            buttonDelete.BackColor = Color.Red;

            // Delete functionality. Sends the index in the array as paramater
            buttonDelete.Click += (sender, e) =>
            {
                DeleteSong(songIndex, playlistName);
            };

            // Adds the controls to the song panel
            songPanel.Controls.Add(songTitle);
            songPanel.Controls.Add(songArtist);
            songPanel.Controls.Add(songDuration);
            songPanel.Controls.Add(buttonDelete);


            return songPanel;
        }

        private void DeleteSong(int songIndex, string playlistName)
        {
            // Get datafolder
            string dataFolder = Path.Combine(
                Application.StartupPath,
                "Data");

            // Get the playlist file
            string[] files = Directory.GetFiles(
                dataFolder,
                playlistName + ".txt",
                SearchOption.AllDirectories);

            // Test if the playlist exists
            if (files.Length == 0)
            {
                MessageBox.Show("Playlist not found.");
                return;
            }

            // Saves the current playlist path
            string playlistPath = files[0];

            // Reads the songs into an array
            string[] songs = File.ReadAllLines(playlistPath);

            // Checks if the playlist does have songs
            if (songs.Length == 0)
            {
                MessageBox.Show("There are no songs to delete.");
                return;
            }

            // Checks if the selected song actually exists in the playlist
            if (songIndex < 0 || songIndex >= songs.Length)
            {
                return;
            }

            // Gets the song that the user selected
            string songToDelete = songs[songIndex];

            // Ask the user for confirmation
            DialogResult confirm = MessageBox.Show(
                $"Are you sure you want to delete \"{songToDelete}\"?",
                "Delete Song",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            // Stop if the user selects No
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            // Converts array into a list so that it is easier to delete
            List<string> remainingSongs = songs.ToList();

            // Deletes the song from the list
            remainingSongs.RemoveAt(songIndex);

            // Writes remaining songs back into the playlist
            File.WriteAllLines(playlistPath, remainingSongs);

            // Rerenders all the remaining songs to display
            readSongs(playlistName);

            // Shows a message for successful deletion
            MessageBox.Show("Song deleted successfully.");
        }

        private void btnChangeCoverPhoto_Click(object sender, EventArgs e)
        {
            //Prevents the user from selected a file that isn't an image
            ofdCoverPicture.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

            //Open the filedialog
            if (ofdCoverPicture.ShowDialog() == DialogResult.OK)
            {
                // Gets the image's filepath
                string filepath = ofdCoverPicture.FileName;

                //Gets the image extension
                string extension = Path.GetExtension(filepath);

                // Changes the coverPhoto visually
                pbxPlaylistCoverPhoto.Image = Image.FromFile(filepath);

                //Makes the image fit
                pbxPlaylistCoverPhoto.SizeMode = PictureBoxSizeMode.StretchImage;

                //Copies the coverphoto over to the covers file.
                File.Copy(filepath, Path.Combine(coversFolder, lblPlaylistName.Text + extension), true);
            }
        }

        //Deletes the current playlist
        private void btnDelete_Click(object sender, EventArgs e)
        {
            deletePlaylist(lblPlaylistName.Text, Path.Combine(playlistsFolder, lblPlaylistName.Text + ".txt"));
            btnBackHome_Click(sender, e);
        }

        //Closes all the open form when you press the red x
        private void Home_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
