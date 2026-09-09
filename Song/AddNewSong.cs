using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Byte_me___Group_2
{
    public partial class  Home : Form
    {
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
                        bool wasReplaced = UpsertTrackInPlaylist(targetPlaylists[i], songTitle, songArtist, songDuration, fileDlg.FileName);
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
        private bool UpsertTrackInPlaylist(string filePath, string title, string artist, string duration, string songFilePath)
        {
            string safeTitle = title.Replace("|", "/").Trim();       // strip separator character from values
            string safeArtist = artist.Replace("|", "/").Trim();
            string safeDuration = duration.Replace("|", "/").Trim();
            string newLine = safeTitle + "|" + safeArtist + "|" + safeDuration + "|" +  songFilePath; // line to write

            string[] existingLines = ReadAllLinesSafe(filePath); // current tracks
            bool foundExisting = false;                          // true if we replaced a track instead of adding one

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
    }
}
