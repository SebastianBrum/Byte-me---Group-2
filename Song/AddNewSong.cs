using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace Byte_me___Group_2
{
    public partial class  Home : Form
    {
        // Handles "Upload Song": pick an audio file, collect details, add it to chosen playlists
        public void btnUploadSong_Click(object sender, EventArgs e)
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
                int addedCount = 0;     // playlists it was newly added to
                int replacedCount = 0;  // playlists where it replaced an existing track
                bool confirmed = ShowUploadSongPrompt(playlistFiles, suggestedTitle, fileDlg.FileName,
                    out songTitle, out songArtist, out songDuration, out targetPlaylists, out addedCount, out replacedCount);
                if (!confirmed)
                    return; // user cancelled the details prompt

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
        private bool ShowUploadSongPrompt(string[] playlistFiles, string suggestedTitle, string songFilePath,
            out string songTitle, out string songArtist, out string songDuration, out string[] targetPlaylists, out int addedCount, out int replacedCount)
        {
            songTitle = null;
            songArtist = null;
            songDuration = null;
            targetPlaylists = new string[0];
            addedCount = 0;
            replacedCount = 0;

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
                Label lblDuration = new Label() { Left = 20, Top = 125, Width = 320, Text = "Duration (auto-detected):" };
                TextBox txtDuration = new TextBox() { Left = 20, Top = 148, Width = 320, ReadOnly = true };

                // Try to read the duration from the selected audio file using TagLib# if available.
                try
                {
                    if (!string.IsNullOrWhiteSpace(songFilePath) && File.Exists(songFilePath))
                    {
                        // Attempt to locate the TagLib.File type from loaded assemblies or by common assembly names
                        Type tagFileType = Type.GetType("TagLib.File, TagLib")
                            ?? Type.GetType("TagLib.File, taglib-sharp")
                            ?? AppDomain.CurrentDomain.GetAssemblies()
                                .Select(a => a.GetType("TagLib.File")).FirstOrDefault(t => t != null);

                        if (tagFileType != null)
                        {
                            MethodInfo createMethod = tagFileType.GetMethod("Create", new Type[] { typeof(string) });
                            if (createMethod != null)
                            {
                                object tfile = createMethod.Invoke(null, new object[] { songFilePath });
                                if (tfile != null)
                                {
                                    PropertyInfo propsProp = tfile.GetType().GetProperty("Properties");
                                    object props = propsProp.GetValue(tfile);
                                    PropertyInfo durationProp = props.GetType().GetProperty("Duration");
                                    TimeSpan dur = (TimeSpan)durationProp.GetValue(props);
                                    if (dur.TotalHours >= 1)
                                        txtDuration.Text = string.Format("{0}:{1:D2}:{2:D2}", (int)dur.TotalHours, dur.Minutes, dur.Seconds);
                                    else
                                        txtDuration.Text = string.Format("{0}:{1:D2}", dur.Minutes, dur.Seconds);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // If TagLib# is not present or fails, leave duration empty (it will default to 0:00 later)
                }
                Label lblPlaylists = new Label() { Left = 20, Top = 180, Width = 320, Text = "Add to which playlist(s)?" };
                CheckedListBox clb = new CheckedListBox() { Left = 20, Top = 203, Width = 320, Height = 130 };

                int i = 0;
                for (i = 0; i < playlistFiles.Length; i++)
                {
                    clb.Items.Add(Path.GetFileNameWithoutExtension(playlistFiles[i])); // list every playlist as a tick-box
                }

                Button addButton = new Button() { Text = "Add Song", Left = 195, Width = 145, Top = 345 };
                Button cancelButton = new Button() { Text = "Cancel", Left = 20, Width = 145, Top = 345 };

                // locals to capture values from the lambda (cannot assign out params inside lambda)
                string localSongTitle = null;
                string localSongArtist = null;
                string localSongDuration = null;
                string[] localTargetPlaylists = new string[0];
                int localAdded = 0;
                int localReplaced = 0;

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
                    // ensure at least one playlist is selected; keep the popup open if not
                    if (clb.CheckedItems.Count == 0)
                    {
                        MessageBox.Show("Tick at least one playlist to add the song to.",
                            "No playlist selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Collect selected playlist paths
                    int checkedCountLocal = 0;
                    for (int ii = 0; ii < clb.Items.Count; ii++) if (clb.GetItemChecked(ii)) checkedCountLocal++;
                    string[] selectedLocal = new string[checkedCountLocal];
                    int wi = 0;
                    for (int ii = 0; ii < clb.Items.Count; ii++)
                    {
                        if (clb.GetItemChecked(ii))
                        {
                            selectedLocal[wi] = playlistFiles[ii];
                            wi++;
                        }
                    }

                    // Try to write into each selected playlist now. If any fail, show error and keep dialog open
                    int addedLocal = 0;
                    int replacedLocal = 0;
                    try
                    {
                        for (int ii = 0; ii < selectedLocal.Length; ii++)
                        {
                            bool wasReplaced = UpsertTrackInPlaylist(selectedLocal[ii], txtTitle.Text.Trim(), txtArtist.Text.Trim(),
                                string.IsNullOrWhiteSpace(txtDuration.Text) ? "0:00" : txtDuration.Text.Trim(), songFilePath);
                            if (wasReplaced) replacedLocal++; else addedLocal++;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Could not add the song to one or more selected playlists:\n" + ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // keep dialog open so user can retry/change selection
                    }

                    // success: capture into locals and close
                    localSongTitle = txtTitle.Text.Trim();
                    localSongArtist = txtArtist.Text.Trim();
                    localSongDuration = string.IsNullOrWhiteSpace(txtDuration.Text) ? "0:00" : txtDuration.Text.Trim();
                    localTargetPlaylists = selectedLocal;
                    localAdded = addedLocal;
                    localReplaced = replacedLocal;
                    prompt.DialogResult = DialogResult.OK; // close
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

                // transfer captured locals to out parameters
                songTitle = localSongTitle;
                songArtist = localSongArtist;
                songDuration = localSongDuration;
                targetPlaylists = localTargetPlaylists;
                addedCount = localAdded;
                replacedCount = localReplaced;
                return true;
            }
        }

        // Adds (or updates) a "Title|Artist|Duration" line in a playlist file
        public bool UpsertTrackInPlaylist(string filePath, string title, string artist, string duration, string songFilePath)
        {
            string safeTitle = title.Replace("|", "/").Trim();
            string safeArtist = artist.Replace("|", "/").Trim();
            string safeDuration = duration.Replace("|", "/").Trim();
            string newLine = safeTitle + "|" + safeArtist + "|" + safeDuration + "|" + songFilePath;

            string[] existingLines = ReadAllLinesSafe(filePath);
            bool foundExisting = false;

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                int i = 0;
                for (i = 0; i < existingLines.Length; i++)
                {
                    if (existingLines[i].Trim().Length == 0)
                        continue;
                    string[] parts = existingLines[i].Split('|');
                    string existingTitle = parts.Length > 0 ? parts[0].Trim() : "";
                    if (string.Equals(existingTitle, safeTitle, StringComparison.OrdinalIgnoreCase))
                    {
                        writer.WriteLine(newLine);
                        foundExisting = true;
                    }
                    else
                    {
                        writer.WriteLine(existingLines[i]);
                    }
                }
                if (!foundExisting)
                {
                    writer.WriteLine(newLine);
                }
            }

            // Only add to Songs.txt if this title isn't already listed there
            string songsFilePath = Path.Combine(userFolder, "Songs.txt");
            string[] existingSongsLines = ReadAllLinesSafe(songsFilePath);
            bool alreadyInSongsFile = false;

            for (int i = 0; i < existingSongsLines.Length; i++)
            {
                if (existingSongsLines[i].Trim().Length == 0)
                    continue;

                string[] parts = existingSongsLines[i].Split('|');
                string existingTitle = parts.Length > 0 ? parts[0].Trim() : "";

                if (string.Equals(existingTitle, safeTitle, StringComparison.OrdinalIgnoreCase))
                {
                    alreadyInSongsFile = true;
                    break;
                }
            }

            if (!alreadyInSongsFile)
            {
                using (StreamWriter SongsFile = new StreamWriter(songsFilePath, true))
                {
                    SongsFile.WriteLine($"{safeTitle}|{safeArtist}|{safeDuration}|{songFilePath}");
                }
            }

            return foundExisting;
        }
    }
}
