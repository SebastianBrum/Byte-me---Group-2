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
        // Handles the "+ New Playlist" button: name it, save it, select it
        private void btnNewPlaylist_Click(object sender, EventArgs e)
        {
            string typed = PromptForPlaylistName(); // ask the user for a name (prompt validates input)
            if (typed == null)
                return; // user cancelled
            string playlistName = typed.Trim();
            string targetPath = Path.Combine(playlistsFolder, playlistName + ".txt"); // default save path

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
                confirmButton.Click += (s, e) =>
                {
                    string name = textBox.Text.Trim();
                    if (name.Length == 0)
                    {
                        MessageBox.Show("Please enter a playlist name.", "Name required",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // keep the dialog open
                    }
                    string defaultPath = Path.Combine(playlistsFolder, name + ".txt");
                    if (File.Exists(defaultPath))
                    {
                        MessageBox.Show("A playlist with that name already exists. Please choose another name.",
                            "Duplicate playlist", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // keep the dialog open
                    }
                    prompt.DialogResult = DialogResult.OK;
                };
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
    }
}
