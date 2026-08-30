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

        ////Deletes a playlist
        //private void deletePlaylist(string playlist, string filePath)
        //{
        //    DialogResult confirm = MessageBox.Show(
        //        "Delete \"" + playlist + "\"? This cannot be undone.",
        //        "Delete playlist", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        //    if (confirm == DialogResult.No)
        //        return; // user backed out

        //    try
        //    {
        //        if (File.Exists(filePath))
        //            File.Delete(filePath); // remove the playlist file
        //        RemoveNameFromFile(favouritesFile, playlist); // scrub from favourites
        //        RemoveNameFromFile(recentFile, playlist);     // scrub from recents

        //        string coverPath = Path.Combine(coversFolder, playlist + ".png");
        //        if (File.Exists(coverPath))
        //            File.Delete(coverPath); // remove any cover art too

        //        RefreshPlaylistView(); // rebuild sidebar/grid without the deleted playlist
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("This playlist could not be deleted:\n" + ex.Message,
        //            "Error deleting playlist", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

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
    }
}
