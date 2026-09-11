namespace Byte_me___Group_2.Playlist
{
    partial class PlayerControls
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
            this.components = new System.ComponentModel.Container();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pnlTrackChild = new System.Windows.Forms.Panel();
            this.pnlTrackParent = new System.Windows.Forms.Panel();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblD = new System.Windows.Forms.Label();
            this.lblElpased = new System.Windows.Forms.Label();
            this.btnShuffle = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.lblArtistName = new System.Windows.Forms.Label();
            this.lblCurrentSong = new System.Windows.Forms.Label();
            this.picAlbumArt = new System.Windows.Forms.PictureBox();
            this.tmrTrackbarTime = new System.Windows.Forms.Timer(this.components);
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAlbumArt)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pnlTrackChild);
            this.panel2.Controls.Add(this.pnlTrackParent);
            this.panel2.Controls.Add(this.lblTime);
            this.panel2.Controls.Add(this.lblD);
            this.panel2.Controls.Add(this.lblElpased);
            this.panel2.Controls.Add(this.btnShuffle);
            this.panel2.Controls.Add(this.btnNext);
            this.panel2.Controls.Add(this.btnPlay);
            this.panel2.Controls.Add(this.btnPrevious);
            this.panel2.Controls.Add(this.lblArtistName);
            this.panel2.Controls.Add(this.lblCurrentSong);
            this.panel2.Controls.Add(this.picAlbumArt);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(788, 81);
            this.panel2.TabIndex = 10;
            // 
            // pnlTrackChild
            // 
            this.pnlTrackChild.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.pnlTrackChild.Location = new System.Drawing.Point(480, 40);
            this.pnlTrackChild.Name = "pnlTrackChild";
            this.pnlTrackChild.Size = new System.Drawing.Size(95, 10);
            this.pnlTrackChild.TabIndex = 0;
            // 
            // pnlTrackParent
            // 
            this.pnlTrackParent.Location = new System.Drawing.Point(480, 40);
            this.pnlTrackParent.Name = "pnlTrackParent";
            this.pnlTrackParent.Size = new System.Drawing.Size(247, 10);
            this.pnlTrackParent.TabIndex = 12;
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(744, 35);
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
            this.lblElpased.Location = new System.Drawing.Point(430, 35);
            this.lblElpased.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblElpased.Name = "lblElpased";
            this.lblElpased.Size = new System.Drawing.Size(31, 16);
            this.lblElpased.TabIndex = 9;
            this.lblElpased.Text = "1:24";
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
            this.btnShuffle.Click += new System.EventHandler(this.btnShuffle_Click);
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
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrevious.Location = new System.Drawing.Point(238, 32);
            this.btnPrevious.Margin = new System.Windows.Forms.Padding(2);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(36, 22);
            this.btnPrevious.TabIndex = 3;
            this.btnPrevious.Text = "I◀";
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
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
            // tmrTrackbarTime
            // 
            this.tmrTrackbarTime.Tick += new System.EventHandler(this.tmrTrackbarTime_Tick);
            // 
            // PlayerControls
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Name = "PlayerControls";
            this.Size = new System.Drawing.Size(791, 84);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAlbumArt)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel pnlTrackChild;
        private System.Windows.Forms.Panel pnlTrackParent;
        public System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblD;
        private System.Windows.Forms.Label lblElpased;
        private System.Windows.Forms.Button btnShuffle;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Label lblArtistName;
        private System.Windows.Forms.Label lblCurrentSong;
        private System.Windows.Forms.PictureBox picAlbumArt;
        public System.Windows.Forms.Timer tmrTrackbarTime;
    }
}
