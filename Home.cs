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

        //// Builds one sidebar row control for a single playlist
        //private Panel BuildNavRow(string title, int trackCount, string filePath, int yPosition)
        //{
        //    Panel row = new Panel();
        //    row.BackColor = Color.White;
        //    row.Cursor = Cursors.Hand;             // shows a hand cursor on hover
        //    row.Location = new Point(15, yPosition); // vertical position passed in
        //    row.Size = new Size(270, 40);
        //    row.Tag = filePath;                     // remember which file this row represents
        //    row.Click += pnlPlaylistNavRow_Click;   // open playlist on click

        //    Label text = new Label();
        //    text.Font = new Font("Segoe UI", 9F);
        //    text.ForeColor = Color.FromArgb(55, 65, 81);
        //    text.Location = new Point(15, 10);
        //    text.Size = new Size(200, 22);
        //    text.Text = "♫   " + title;             // playlist name with a music note
        //    text.TextAlign = ContentAlignment.MiddleLeft;
        //    text.Click += pnlPlaylistNavRow_Click;  // clicking the text also opens it

        //    Label count = new Label();
        //    count.Font = new Font("Segoe UI", 9F);
        //    count.ForeColor = Color.FromArgb(156, 163, 175);
        //    count.Location = new Point(230, 9);
        //    count.Size = new Size(30, 22);
        //    count.Text = trackCount.ToString();      // number of tracks
        //    count.TextAlign = ContentAlignment.MiddleRight;
        //    count.Click += pnlPlaylistNavRow_Click;  // clicking the count also opens it

        //    row.Controls.Add(text);
        //    row.Controls.Add(count);
        //    return row;
        //}

        // Builds one grid card control for a single playlist
        private Panel BuildPlaylistCard(string title, int trackCount, string filePath)
        {
            Panel card = new Panel();
            card.BackColor = Color.White;
            card.Cursor = Cursors.Hand;
            card.Size = new Size(255, 262);
            card.Margin = new Padding(0, 0, 25, 25); // spacing between cards in the flow layout
            card.Tag = filePath;                      // remember which file this card represents
            card.Click += pnlPlaylistCard_Click;      // open playlist on click

            Label cover = new Label();
            cover.BackColor = GetCoverColorFor(title); // colour based on playlist name
            cover.Font = new Font("Segoe UI", 26F);
            cover.ForeColor = Color.White;
            cover.Location = new Point(0, 0);
            cover.Size = new Size(255, 175);
            cover.Text = "♫";                          // music note "cover art"
            cover.TextAlign = ContentAlignment.MiddleCenter;
            cover.Click += pnlPlaylistCard_Click;

            Label titleLabel = new Label();
            titleLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(17, 24, 39);
            titleLabel.Location = new Point(12, 188);
            titleLabel.Size = new Size(150, 22);
            titleLabel.Text = title;                   // playlist name
            titleLabel.Click += pnlPlaylistCard_Click;

            Label countLabel = new Label();
            countLabel.Font = new Font("Segoe UI", 8F);
            countLabel.ForeColor = Color.FromArgb(107, 114, 128);
            countLabel.Location = new Point(12, 212);
            countLabel.Size = new Size(150, 20);
            countLabel.Text = trackCount + (trackCount == 1 ? " track" : " tracks"); // track count text
            countLabel.Click += pnlPlaylistCard_Click;

            // small red "x" button in the corner used to delete this playlist
            Label deleteButton = new Label();
            deleteButton.BackColor = Color.FromArgb(220, 38, 38);
            deleteButton.ForeColor = Color.White;
            deleteButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            deleteButton.Size = new Size(22, 22);
            deleteButton.Location = new Point(255 - 22 - 8, 8); // top-right corner of the cover
            deleteButton.Text = "✕";
            deleteButton.TextAlign = ContentAlignment.MiddleCenter;
            deleteButton.Cursor = Cursors.Hand;
            deleteButton.Tag = filePath;                          // remember which file to delete
            deleteButton.Click += btnDeletePlaylist_Click;        // separate handler (deletes instead of opening)

            card.Controls.Add(cover);
            card.Controls.Add(titleLabel);
            card.Controls.Add(countLabel);
            card.Controls.Add(deleteButton);
            deleteButton.BringToFront(); // make sure the "x" sits above the cover
            return card;
        }

        private void FavoriteButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        // Deletes a playlist (file + cover + favourite/recent references) after confirmation
        private void btnDeletePlaylist_Click(object sender, EventArgs e)
        {
            Control clicked = sender as Control;
            if (clicked == null)
                return; // safety check
            string filePath = clicked.Tag as string; // file to delete, from the button's Tag
            if (string.IsNullOrEmpty(filePath))
                return;
            string name = Path.GetFileNameWithoutExtension(filePath); // playlist name for messages

            deletePlaylist(name, filePath);
        }

        //Deletes a playlist
        private void deletePlaylist(string playlist, string filePath)
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

        // Picks a consistent cover colour for a playlist name (same name = same colour)
        private Color GetCoverColorFor(string playlistName)
        {
            int hash = 0;
            int i = 0;
            for (i = 0; i < playlistName.Length; i++)
            {
                hash += (int)playlistName[i]; // sum of character codes
            }
            int index = hash % CoverPalette.Length; // map sum onto the palette
            if (index < 0)
                index += CoverPalette.Length; // guard against a negative result
            return CoverPalette[index];
        }

        // Returns only the files whose name is in the favourites list
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

        // Returns "" if the search box is empty or showing its placeholder, otherwise the typed text
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

        // Opens a playlist when a sidebar row (or its labels) is clicked
        private void pnlPlaylistNavRow_Click(object sender, EventArgs e)
        {
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

        // Opens a playlist when a grid card (or its labels) is clicked
        private void pnlPlaylistCard_Click(object sender, EventArgs e)
        {
            Control clicked = sender as Control;
            if (clicked == null)
                return;
            Control card = (clicked is Panel) ? clicked : clicked.Parent;
            if (card == null)
                return;
            string filePath = card.Tag as string; // file path stored on the card
            OpenPlaylist(filePath);
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

        // Handles the "+ New Playlist" button: name it, save it, select it
        private void btnNewPlaylist_Click(object sender, EventArgs e)
        {
            string typed = PromptForPlaylistName(); // ask the user for a name
            if (typed == null)
                return; // user cancelled
            string playlistName = typed.Trim();
            if (playlistName.Length == 0)
            {
                MessageBox.Show("Please enter a playlist name.", "Name required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string targetPath = Path.Combine(playlistsFolder, playlistName + ".txt"); // default save path
            if (File.Exists(targetPath))
            {
                MessageBox.Show("A playlist with that name already exists. Please choose another name.",
                    "Duplicate playlist", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // let the user confirm/change where the file is actually saved
            using (SaveFileDialog saveDlg = new SaveFileDialog())
            {
                saveDlg.Title = "Save new playlist as";
                saveDlg.Filter = "Playlist text files (*.txt)|*.txt";
                saveDlg.InitialDirectory = playlistsFolder;
                saveDlg.FileName = playlistName + ".txt";
                if (saveDlg.ShowDialog() != DialogResult.OK)
                    return; // user cancelled the save dialog
                targetPath = saveDlg.FileName; // use the chosen path
            }

            try
            {
                WriteNewPlaylistFile(targetPath); // create the empty playlist file
                RefreshPlaylistView();             // show it in the sidebar/grid
                SelectPlaylistByPath(targetPath);  // highlight and scroll to it
            }
            catch (Exception ex)
            {
                MessageBox.Show("The playlist could not be created:\n" + ex.Message,
                    "Error creating playlist", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Small pop-up form that asks the user to type a playlist name
        private string PromptForPlaylistName()
        {
            using (Form prompt = new Form())
            {
                prompt.Width = 380;
                prompt.Height = 160;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.Text = "New Playlist";
                prompt.StartPosition = FormStartPosition.CenterParent;
                prompt.MaximizeBox = false;
                prompt.MinimizeBox = false;

                Label label = new Label() { Left = 20, Top = 20, Width = 320, Text = "Playlist name:" };
                TextBox textBox = new TextBox() { Left = 20, Top = 45, Width = 320 };
                Button confirmButton = new Button() { Text = "Create", Left = 195, Width = 145, Top = 80 };
                Button cancelButton = new Button() { Text = "Cancel", Left = 20, Width = 145, Top = 80 };

                // set DialogResult in code (not on the buttons) so we control exactly when the form closes
                confirmButton.Click += (s, e) => { prompt.DialogResult = DialogResult.OK; };
                cancelButton.Click += (s, e) => { prompt.DialogResult = DialogResult.Cancel; };

                prompt.Controls.Add(label);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmButton);
                prompt.Controls.Add(cancelButton);
                prompt.AcceptButton = confirmButton; // Enter key triggers Create
                prompt.CancelButton = cancelButton;  // Escape key triggers Cancel

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : null; // null = cancelled
            }
        }

        // Creates a new, empty playlist text file
        private void WriteNewPlaylistFile(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                // no lines written - an empty playlist has zero tracks
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

        // Handles "Upload Song": pick an audio file, collect details, add it to chosen playlists
        private void btnUploadSong_Click(object sender, EventArgs e)
        {
            string[] playlistFiles = Directory.GetFiles(playlistsFolder, "*.txt"); // playlists to choose from
            if (playlistFiles.Length == 0)
            {
                MessageBox.Show("Create a playlist first, then you'll be able to upload songs into it.",
                    "No playlists yet", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; // nothing to add a song to
            }

            // Step 1: choose the audio file itself
            using (OpenFileDialog fileDlg = new OpenFileDialog())
            {
                fileDlg.Title = "Choose a song to upload";
                fileDlg.Filter = "Audio files (*.mp3;*.wav;*.wma)|*.mp3;*.wav;*.wma|All files (*.*)|*.*";
                if (fileDlg.ShowDialog() != DialogResult.OK)
                    return; // user cancelled
                string suggestedTitle = Path.GetFileNameWithoutExtension(fileDlg.FileName); // default title guess

                // Step 2: collect song details and target playlist(s)
                string songTitle, songArtist, songDuration;
                string[] targetPlaylists;
                bool confirmed = ShowUploadSongPrompt(playlistFiles, suggestedTitle,
                    out songTitle, out songArtist, out songDuration, out targetPlaylists);
                if (!confirmed)
                    return; // user cancelled the details prompt
                if (targetPlaylists.Length == 0)
                {
                    MessageBox.Show("Tick at least one playlist to add the song to.",
                        "No playlist selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Step 3: write the song into each chosen playlist
                int addedCount = 0;     // playlists it was newly added to
                int replacedCount = 0;  // playlists where it replaced an existing track
                int i = 0;
                for (i = 0; i < targetPlaylists.Length; i++)
                {
                    try
                    {
                        bool wasReplaced = UpsertTrackInPlaylist(targetPlaylists[i], songTitle, songArtist, songDuration);
                        if (wasReplaced) replacedCount++; else addedCount++;
                    }
                    catch (Exception ex)
                    {
                        // one bad playlist file shouldn't stop the rest from being updated
                        MessageBox.Show("Could not add the song to " + Path.GetFileNameWithoutExtension(targetPlaylists[i]) +
                            ":\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // build a summary message describing what happened
                string summary = "\"" + songTitle + "\" ";
                if (addedCount > 0 && replacedCount == 0)
                    summary += "was added to " + addedCount + (addedCount == 1 ? " playlist." : " playlists.");
                else if (replacedCount > 0 && addedCount == 0)
                    summary += "already existed and was updated in " + replacedCount + (replacedCount == 1 ? " playlist." : " playlists.");
                else
                    summary += "was added to " + addedCount + " and updated in " + replacedCount + " existing playlist(s).";
                MessageBox.Show(summary, "Upload complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshPlaylistView(); // reflect the updated track counts
            }
        }

        // Pop-up form collecting song title/artist/duration and which playlists to add it to
        private bool ShowUploadSongPrompt(string[] playlistFiles, string suggestedTitle,
            out string songTitle, out string songArtist, out string songDuration, out string[] targetPlaylists)
        {
            songTitle = null;
            songArtist = null;
            songDuration = null;
            targetPlaylists = new string[0];

            using (Form prompt = new Form())
            {
                prompt.Width = 380;
                prompt.Height = 420;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.Text = "Upload Song";
                prompt.StartPosition = FormStartPosition.CenterParent;
                prompt.MaximizeBox = false;
                prompt.MinimizeBox = false;

                Label lblTitle = new Label() { Left = 20, Top = 15, Width = 320, Text = "Song title:" };
                TextBox txtTitle = new TextBox() { Left = 20, Top = 38, Width = 320, Text = suggestedTitle }; // pre-filled from filename
                Label lblArtist = new Label() { Left = 20, Top = 70, Width = 320, Text = "Artist:" };
                TextBox txtArtist = new TextBox() { Left = 20, Top = 93, Width = 320 };
                Label lblDuration = new Label() { Left = 20, Top = 125, Width = 320, Text = "Duration (e.g. 3:45) - optional:" };
                TextBox txtDuration = new TextBox() { Left = 20, Top = 148, Width = 320 };
                Label lblPlaylists = new Label() { Left = 20, Top = 180, Width = 320, Text = "Add to which playlist(s)?" };
                CheckedListBox clb = new CheckedListBox() { Left = 20, Top = 203, Width = 320, Height = 130 };

                int i = 0;
                for (i = 0; i < playlistFiles.Length; i++)
                {
                    clb.Items.Add(Path.GetFileNameWithoutExtension(playlistFiles[i])); // list every playlist as a tick-box
                }

                Button addButton = new Button() { Text = "Add Song", Left = 195, Width = 145, Top = 345 };
                Button cancelButton = new Button() { Text = "Cancel", Left = 20, Width = 145, Top = 345 };

                addButton.Click += (s, e) =>
                {
                    // validate before allowing the form to close
                    if (string.IsNullOrWhiteSpace(txtTitle.Text))
                    {
                        MessageBox.Show("Please enter a song title.", "Title required",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(txtArtist.Text))
                    {
                        MessageBox.Show("Please enter an artist name.", "Artist required",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    prompt.DialogResult = DialogResult.OK; // only close once valid
                };
                cancelButton.Click += (s, e) => { prompt.DialogResult = DialogResult.Cancel; };

                prompt.Controls.Add(lblTitle);
                prompt.Controls.Add(txtTitle);
                prompt.Controls.Add(lblArtist);
                prompt.Controls.Add(txtArtist);
                prompt.Controls.Add(lblDuration);
                prompt.Controls.Add(txtDuration);
                prompt.Controls.Add(lblPlaylists);
                prompt.Controls.Add(clb);
                prompt.Controls.Add(addButton);
                prompt.Controls.Add(cancelButton);
                prompt.AcceptButton = addButton;
                prompt.CancelButton = cancelButton;

                if (prompt.ShowDialog() != DialogResult.OK)
                    return false; // user cancelled

                songTitle = txtTitle.Text.Trim();
                songArtist = txtArtist.Text.Trim();
                songDuration = string.IsNullOrWhiteSpace(txtDuration.Text) ? "0:00" : txtDuration.Text.Trim(); // default duration

                // count how many playlists were ticked
                int checkedCount = 0;
                for (i = 0; i < clb.Items.Count; i++)
                {
                    if (clb.GetItemChecked(i))
                        checkedCount++;
                }
                string[] selected = new string[checkedCount];
                int writeIndex = 0;
                for (i = 0; i < clb.Items.Count; i++)
                {
                    if (clb.GetItemChecked(i))
                    {
                        selected[writeIndex] = playlistFiles[i]; // collect the ticked playlist paths
                        writeIndex++;
                    }
                }
                targetPlaylists = selected;
                return true;
            }
        }

        // Adds (or updates) a "Title|Artist|Duration" line in a playlist file
        private bool UpsertTrackInPlaylist(string filePath, string title, string artist, string duration)
        {
            string safeTitle = title.Replace("|", "/").Trim();       // strip separator character from values
            string safeArtist = artist.Replace("|", "/").Trim();
            string safeDuration = duration.Replace("|", "/").Trim();
            string newLine = safeTitle + "|" + safeArtist + "|" + safeDuration; // line to write

            string[] existingLines = ReadAllLinesSafe(filePath); // current tracks
            bool foundExisting = false;                            // true if we replaced a track instead of adding one

            using (StreamWriter writer = new StreamWriter(filePath, false)) // rewrite the whole file
            {
                int i = 0;
                for (i = 0; i < existingLines.Length; i++)
                {
                    if (existingLines[i].Trim().Length == 0)
                        continue; // skip blank lines
                    string[] parts = existingLines[i].Split('|');
                    string existingTitle = parts.Length > 0 ? parts[0].Trim() : "";
                    if (string.Equals(existingTitle, safeTitle, StringComparison.OrdinalIgnoreCase))
                    {
                        writer.WriteLine(newLine);   // overwrite the matching track
                        foundExisting = true;
                    }
                    else
                    {
                        writer.WriteLine(existingLines[i]); // keep every other track unchanged
                    }
                }
                if (!foundExisting)
                {
                    writer.WriteLine(newLine); // no match found, append as a new track
                }
            }
            return foundExisting;
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
