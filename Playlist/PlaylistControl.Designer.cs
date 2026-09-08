using System.Windows.Forms;

namespace Byte_me___Group_2
{
    partial class PlaylistControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlaylistControl));
            this.pnlPlaylist = new System.Windows.Forms.Panel();
            this.dgvDisplaySongs = new System.Windows.Forms.DataGridView();
            this.songName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.songArtist = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.songDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SongDelete = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnBackHome = new System.Windows.Forms.Button();
            this.lblBreadcrumb = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblD = new System.Windows.Forms.Label();
            this.lblElpased = new System.Windows.Forms.Label();
            this.trkProgress = new System.Windows.Forms.TrackBar();
            this.btnShuffle = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.lblArtistName = new System.Windows.Forms.Label();
            this.lblCurrentSong = new System.Windows.Forms.Label();
            this.picAlbumArt = new System.Windows.Forms.PictureBox();
            this.pnlPlaylistHeader = new System.Windows.Forms.Panel();
            this.pbxPlaylistCoverPhoto = new System.Windows.Forms.PictureBox();
            this.MediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAddsongs = new System.Windows.Forms.Button();
            this.lblDateCreated = new System.Windows.Forms.Label();
            this.lblPlaylistName = new System.Windows.Forms.Label();
            this.lblPlaylistType = new System.Windows.Forms.Label();
            this.lblPlaylistPathName = new System.Windows.Forms.Label();
            this.pnlTopBar = new System.Windows.Forms.Panel();
            this.lblPlalistCount = new System.Windows.Forms.Label();
            this.lblWelecome = new System.Windows.Forms.Label();
            this.pnlPlaylist.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisplaySongs)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkProgress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAlbumArt)).BeginInit();
            this.pnlPlaylistHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxPlaylistCoverPhoto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MediaPlayer)).BeginInit();
            this.pnlTopBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlPlaylist
            // 
            this.pnlPlaylist.Controls.Add(this.dgvDisplaySongs);
            this.pnlPlaylist.Controls.Add(this.btnBackHome);
            this.pnlPlaylist.Controls.Add(this.lblBreadcrumb);
            this.pnlPlaylist.Controls.Add(this.panel2);
            this.pnlPlaylist.Controls.Add(this.pnlPlaylistHeader);
            this.pnlPlaylist.Controls.Add(this.lblPlaylistPathName);
            this.pnlPlaylist.Controls.Add(this.pnlTopBar);
            this.pnlPlaylist.Location = new System.Drawing.Point(2, 2);
            this.pnlPlaylist.Margin = new System.Windows.Forms.Padding(2);
            this.pnlPlaylist.Name = "pnlPlaylist";
            this.pnlPlaylist.Size = new System.Drawing.Size(815, 648);
            this.pnlPlaylist.TabIndex = 6;
            // 
            // dgvDisplaySongs
            // 
            this.dgvDisplaySongs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDisplaySongs.Location = new System.Drawing.Point(12, 329);
            this.dgvDisplaySongs.Name = "dgvDisplaySongs";
            this.dgvDisplaySongs.RowHeadersWidth = 51;
            this.dgvDisplaySongs.RowTemplate.Height = 24;
            this.dgvDisplaySongs.Size = new System.Drawing.Size(788, 290);
            this.dgvDisplaySongs.TabIndex = 13;
            // 
            // songName
            // 
            this.songName.DataPropertyName = "Name";
            this.songName.HeaderText = "Name";
            this.songName.MinimumWidth = 6;
            this.songName.Name = "songName";
            this.songName.ReadOnly = true;
            this.songName.Width = 300;
            // 
            // songArtist
            // 
            this.songArtist.DataPropertyName = "Artist";
            this.songArtist.HeaderText = "Artist";
            this.songArtist.MinimumWidth = 6;
            this.songArtist.Name = "songArtist";
            this.songArtist.ReadOnly = true;
            this.songArtist.Width = 200;
            // 
            // songDuration
            // 
            this.songDuration.DataPropertyName = "Duration";
            this.songDuration.HeaderText = "Duration";
            this.songDuration.MinimumWidth = 6;
            this.songDuration.Name = "songDuration";
            this.songDuration.ReadOnly = true;
            this.songDuration.Width = 125;
            // 
            // SongDelete
            // 
            this.SongDelete.DataPropertyName = "Delete";
            this.SongDelete.HeaderText = "";
            this.SongDelete.MinimumWidth = 6;
            this.SongDelete.Name = "SongDelete";
            this.SongDelete.ReadOnly = true;
            this.SongDelete.Width = 125;
            // 
            // btnBackHome
            // 
            this.btnBackHome.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBackHome.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBackHome.Location = new System.Drawing.Point(709, 0);
            this.btnBackHome.Margin = new System.Windows.Forms.Padding(2);
            this.btnBackHome.Name = "btnBackHome";
            this.btnBackHome.Size = new System.Drawing.Size(90, 78);
            this.btnBackHome.TabIndex = 12;
            this.btnBackHome.Text = "Back";
            this.btnBackHome.UseVisualStyleBackColor = true;
            this.btnBackHome.Click += new System.EventHandler(this.btnBackHome_Click);
            // 
            // lblBreadcrumb
            // 
            this.lblBreadcrumb.AutoSize = true;
            this.lblBreadcrumb.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblBreadcrumb.Location = new System.Drawing.Point(12, 102);
            this.lblBreadcrumb.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBreadcrumb.Name = "lblBreadcrumb";
            this.lblBreadcrumb.Size = new System.Drawing.Size(51, 16);
            this.lblBreadcrumb.TabIndex = 11;
            this.lblBreadcrumb.Text = "Home /";
            this.lblBreadcrumb.Click += new System.EventHandler(this.lblBreadcrumb_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblTime);
            this.panel2.Controls.Add(this.lblD);
            this.panel2.Controls.Add(this.lblElpased);
            this.panel2.Controls.Add(this.trkProgress);
            this.panel2.Controls.Add(this.btnShuffle);
            this.panel2.Controls.Add(this.btnNext);
            this.panel2.Controls.Add(this.btnPlay);
            this.panel2.Controls.Add(this.btnBack);
            this.panel2.Controls.Add(this.btnPrevious);
            this.panel2.Controls.Add(this.lblArtistName);
            this.panel2.Controls.Add(this.lblCurrentSong);
            this.panel2.Controls.Add(this.picAlbumArt);
            this.panel2.Location = new System.Drawing.Point(12, 229);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(788, 81);
            this.panel2.TabIndex = 9;
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(732, 31);
            this.lblTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(31, 16);
            this.lblTime.TabIndex = 11;
            this.lblTime.Text = "3:42";
            // 
            // lblD
            // 
            this.lblD.AutoSize = true;
            this.lblD.Location = new System.Drawing.Point(722, 38);
            this.lblD.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblD.Name = "lblD";
            this.lblD.Size = new System.Drawing.Size(0, 16);
            this.lblD.TabIndex = 10;
            // 
            // lblElpased
            // 
            this.lblElpased.AutoSize = true;
            this.lblElpased.Location = new System.Drawing.Point(444, 38);
            this.lblElpased.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblElpased.Name = "lblElpased";
            this.lblElpased.Size = new System.Drawing.Size(31, 16);
            this.lblElpased.TabIndex = 9;
            this.lblElpased.Text = "1:24";
            // 
            // trkProgress
            // 
            this.trkProgress.Location = new System.Drawing.Point(476, 8);
            this.trkProgress.Margin = new System.Windows.Forms.Padding(2);
            this.trkProgress.Name = "trkProgress";
            this.trkProgress.Size = new System.Drawing.Size(252, 56);
            this.trkProgress.TabIndex = 8;
            // 
            // btnShuffle
            // 
            this.btnShuffle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShuffle.Location = new System.Drawing.Point(390, 26);
            this.btnShuffle.Margin = new System.Windows.Forms.Padding(2);
            this.btnShuffle.Name = "btnShuffle";
            this.btnShuffle.Size = new System.Drawing.Size(26, 28);
            this.btnShuffle.TabIndex = 7;
            this.btnShuffle.Text = "🔀";
            this.btnShuffle.UseVisualStyleBackColor = true;
            // 
            // btnNext
            // 
            this.btnNext.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNext.Location = new System.Drawing.Point(349, 31);
            this.btnNext.Margin = new System.Windows.Forms.Padding(2);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(36, 22);
            this.btnNext.TabIndex = 6;
            this.btnNext.Text = "▶I";
            this.btnNext.UseVisualStyleBackColor = true;
            // 
            // btnPlay
            // 
            this.btnPlay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPlay.Location = new System.Drawing.Point(278, 32);
            this.btnPlay.Margin = new System.Windows.Forms.Padding(2);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(68, 21);
            this.btnPlay.TabIndex = 5;
            this.btnPlay.Text = "I I";
            this.btnPlay.UseVisualStyleBackColor = true;
            // 
            // btnBack
            // 
            this.btnBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBack.Location = new System.Drawing.Point(248, 30);
            this.btnBack.Margin = new System.Windows.Forms.Padding(2);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(25, 24);
            this.btnBack.TabIndex = 4;
            this.btnBack.Text = "◀";
            this.btnBack.UseVisualStyleBackColor = true;
            // 
            // btnPrevious
            // 
            this.btnPrevious.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrevious.Location = new System.Drawing.Point(212, 28);
            this.btnPrevious.Margin = new System.Windows.Forms.Padding(2);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(28, 30);
            this.btnPrevious.TabIndex = 3;
            this.btnPrevious.Text = "I◀";
            this.btnPrevious.UseVisualStyleBackColor = true;
            // 
            // lblArtistName
            // 
            this.lblArtistName.AutoSize = true;
            this.lblArtistName.Location = new System.Drawing.Point(106, 34);
            this.lblArtistName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblArtistName.Name = "lblArtistName";
            this.lblArtistName.Size = new System.Drawing.Size(69, 16);
            this.lblArtistName.TabIndex = 2;
            this.lblArtistName.Text = "Vela Kane";
            // 
            // lblCurrentSong
            // 
            this.lblCurrentSong.AutoSize = true;
            this.lblCurrentSong.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentSong.Location = new System.Drawing.Point(101, 12);
            this.lblCurrentSong.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCurrentSong.Name = "lblCurrentSong";
            this.lblCurrentSong.Size = new System.Drawing.Size(107, 19);
            this.lblCurrentSong.TabIndex = 1;
            this.lblCurrentSong.Text = "Neon Corridor";
            // 
            // picAlbumArt
            // 
            this.picAlbumArt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.picAlbumArt.Location = new System.Drawing.Point(18, 10);
            this.picAlbumArt.Margin = new System.Windows.Forms.Padding(2);
            this.picAlbumArt.Name = "picAlbumArt";
            this.picAlbumArt.Size = new System.Drawing.Size(68, 52);
            this.picAlbumArt.TabIndex = 0;
            this.picAlbumArt.TabStop = false;
            // 
            // pnlPlaylistHeader
            // 
            this.pnlPlaylistHeader.Controls.Add(this.pbxPlaylistCoverPhoto);
            this.pnlPlaylistHeader.Controls.Add(this.MediaPlayer);
            this.pnlPlaylistHeader.Controls.Add(this.btnDelete);
            this.pnlPlaylistHeader.Controls.Add(this.btnAddsongs);
            this.pnlPlaylistHeader.Controls.Add(this.lblDateCreated);
            this.pnlPlaylistHeader.Controls.Add(this.lblPlaylistName);
            this.pnlPlaylistHeader.Controls.Add(this.lblPlaylistType);
            this.pnlPlaylistHeader.Location = new System.Drawing.Point(12, 126);
            this.pnlPlaylistHeader.Margin = new System.Windows.Forms.Padding(2);
            this.pnlPlaylistHeader.Name = "pnlPlaylistHeader";
            this.pnlPlaylistHeader.Size = new System.Drawing.Size(788, 98);
            this.pnlPlaylistHeader.TabIndex = 8;
            // 
            // pbxPlaylistCoverPhoto
            // 
            this.pbxPlaylistCoverPhoto.Location = new System.Drawing.Point(15, 11);
            this.pbxPlaylistCoverPhoto.Margin = new System.Windows.Forms.Padding(4);
            this.pbxPlaylistCoverPhoto.Name = "pbxPlaylistCoverPhoto";
            this.pbxPlaylistCoverPhoto.Size = new System.Drawing.Size(68, 59);
            this.pbxPlaylistCoverPhoto.TabIndex = 12;
            this.pbxPlaylistCoverPhoto.TabStop = false;
            // 
            // MediaPlayer
            // 
            this.MediaPlayer.Enabled = true;
            this.MediaPlayer.Location = new System.Drawing.Point(863, 109);
            this.MediaPlayer.Margin = new System.Windows.Forms.Padding(2);
            this.MediaPlayer.Name = "MediaPlayer";
            this.MediaPlayer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("MediaPlayer.OcxState")));
            this.MediaPlayer.Size = new System.Drawing.Size(10, 10);
            this.MediaPlayer.TabIndex = 11;
            this.MediaPlayer.Visible = false;
            // 
            // btnDelete
            // 
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.Location = new System.Drawing.Point(712, 24);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(2);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(40, 28);
            this.btnDelete.TabIndex = 9;
            this.btnDelete.Text = "🗑️";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAddsongs
            // 
            this.btnAddsongs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddsongs.Location = new System.Drawing.Point(601, 26);
            this.btnAddsongs.Margin = new System.Windows.Forms.Padding(2);
            this.btnAddsongs.Name = "btnAddsongs";
            this.btnAddsongs.Size = new System.Drawing.Size(108, 26);
            this.btnAddsongs.TabIndex = 8;
            this.btnAddsongs.Text = "Add Songs";
            this.btnAddsongs.UseVisualStyleBackColor = true;
            // 
            // lblDateCreated
            // 
            this.lblDateCreated.AutoSize = true;
            this.lblDateCreated.Location = new System.Drawing.Point(91, 54);
            this.lblDateCreated.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDateCreated.Name = "lblDateCreated";
            this.lblDateCreated.Size = new System.Drawing.Size(330, 16);
            this.lblDateCreated.TabIndex = 5;
            this.lblDateCreated.Text = "8 tracks   •   32 min   •   Created 12 Mar 2026   •   Private";
            // 
            // lblPlaylistName
            // 
            this.lblPlaylistName.AutoSize = true;
            this.lblPlaylistName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlaylistName.Location = new System.Drawing.Point(89, 26);
            this.lblPlaylistName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPlaylistName.Name = "lblPlaylistName";
            this.lblPlaylistName.Size = new System.Drawing.Size(169, 28);
            this.lblPlaylistName.TabIndex = 4;
            this.lblPlaylistName.Text = "Late Night Drive";
            // 
            // lblPlaylistType
            // 
            this.lblPlaylistType.AutoSize = true;
            this.lblPlaylistType.Location = new System.Drawing.Point(91, 11);
            this.lblPlaylistType.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPlaylistType.Name = "lblPlaylistType";
            this.lblPlaylistType.Size = new System.Drawing.Size(69, 16);
            this.lblPlaylistType.TabIndex = 3;
            this.lblPlaylistType.Text = "PLAYLIST";
            // 
            // lblPlaylistPathName
            // 
            this.lblPlaylistPathName.AutoSize = true;
            this.lblPlaylistPathName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlaylistPathName.Location = new System.Drawing.Point(68, 102);
            this.lblPlaylistPathName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPlaylistPathName.Name = "lblPlaylistPathName";
            this.lblPlaylistPathName.Size = new System.Drawing.Size(126, 17);
            this.lblPlaylistPathName.TabIndex = 7;
            this.lblPlaylistPathName.Text = "Late Night Drive";
            // 
            // pnlTopBar
            // 
            this.pnlTopBar.Controls.Add(this.lblPlalistCount);
            this.pnlTopBar.Controls.Add(this.lblWelecome);
            this.pnlTopBar.Location = new System.Drawing.Point(15, 2);
            this.pnlTopBar.Margin = new System.Windows.Forms.Padding(2);
            this.pnlTopBar.Name = "pnlTopBar";
            this.pnlTopBar.Size = new System.Drawing.Size(436, 79);
            this.pnlTopBar.TabIndex = 6;
            // 
            // lblPlalistCount
            // 
            this.lblPlalistCount.AutoSize = true;
            this.lblPlalistCount.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlalistCount.Location = new System.Drawing.Point(22, 41);
            this.lblPlalistCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPlalistCount.Name = "lblPlalistCount";
            this.lblPlalistCount.Size = new System.Drawing.Size(212, 19);
            this.lblPlalistCount.TabIndex = 1;
            this.lblPlalistCount.Text = "You have 6 playlist in your library";
            // 
            // lblWelecome
            // 
            this.lblWelecome.AutoSize = true;
            this.lblWelecome.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWelecome.Location = new System.Drawing.Point(21, 14);
            this.lblWelecome.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWelecome.Name = "lblWelecome";
            this.lblWelecome.Size = new System.Drawing.Size(215, 28);
            this.lblWelecome.TabIndex = 0;
            this.lblWelecome.Text = "Welcome back,Lerato";
            // 
            // PlaylistControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlPlaylist);
            this.Name = "PlaylistControl";
            this.Size = new System.Drawing.Size(819, 654);
            this.pnlPlaylist.ResumeLayout(false);
            this.pnlPlaylist.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisplaySongs)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkProgress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAlbumArt)).EndInit();
            this.pnlPlaylistHeader.ResumeLayout(false);
            this.pnlPlaylistHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxPlaylistCoverPhoto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MediaPlayer)).EndInit();
            this.pnlTopBar.ResumeLayout(false);
            this.pnlTopBar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlPlaylist;
        private System.Windows.Forms.Button btnBackHome;
        private System.Windows.Forms.Label lblBreadcrumb;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblD;
        private System.Windows.Forms.Label lblElpased;
        private System.Windows.Forms.TrackBar trkProgress;
        private System.Windows.Forms.Button btnShuffle;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Label lblArtistName;
        private System.Windows.Forms.Label lblCurrentSong;
        private System.Windows.Forms.PictureBox picAlbumArt;
        private System.Windows.Forms.Panel pnlPlaylistHeader;
        private System.Windows.Forms.PictureBox pbxPlaylistCoverPhoto;
        private AxWMPLib.AxWindowsMediaPlayer MediaPlayer;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAddsongs;
        private System.Windows.Forms.Label lblDateCreated;
        private System.Windows.Forms.Label lblPlaylistName;
        private System.Windows.Forms.Label lblPlaylistType;
        private System.Windows.Forms.Label lblPlaylistPathName;
        private System.Windows.Forms.Panel pnlTopBar;
        private System.Windows.Forms.Label lblPlalistCount;
        private System.Windows.Forms.Label lblWelecome;
        private System.Windows.Forms.DataGridView dgvDisplaySongs;
        private System.Windows.Forms.DataGridViewTextBoxColumn songName;
        private System.Windows.Forms.DataGridViewTextBoxColumn songArtist;
        private System.Windows.Forms.DataGridViewTextBoxColumn songDuration;
        private System.Windows.Forms.DataGridViewTextBoxColumn SongDelete;
    }
}
