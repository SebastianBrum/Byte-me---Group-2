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
        private void RefreshStatistics()
        {
            string[] playlistFiles = Directory.GetFiles(playlistsFolder, "*.txt"); // every playlist file
            int totalPlaylists = playlistFiles.Length; // total playlist count
            int totalTracks = 0;                        // running total of tracks

            // parallel arrays used as a manual "artist name -> count" tally
            string[] artistNames = new string[200];
            int[] artistCounts = new int[200];
            int knownArtists = 0; // number of distinct artists found so far

            int p = 0;
            for (p = 0; p < playlistFiles.Length; p++)
            {
                string[] lines = ReadAllLinesSafe(playlistFiles[p]); // every track line in this playlist
                int t = 0;
                for (t = 0; t < lines.Length; t++)
                {
                    if (lines[t].Trim().Length == 0)
                        continue; // skip blank lines
                    totalTracks++; // count this track

                    string[] parts = lines[t].Split('|'); // split "Title|Artist|Duration"
                    if (parts.Length < 2)
                        continue; // malformed line, skip artist tally
                    string artist = parts[1].Trim(); // artist name
                    if (artist.Length == 0)
                        continue; // no artist to tally

                    // look for this artist among the ones already tallied
                    int foundIndex = -1;
                    int a = 0;
                    while (a < knownArtists)
                    {
                        if (string.Equals(artistNames[a], artist, StringComparison.OrdinalIgnoreCase))
                        {
                            foundIndex = a; // already known
                            break;
                        }
                        a++;
                    }
                    if (foundIndex >= 0)
                        artistCounts[foundIndex]++; // bump existing artist's count
                    else if (knownArtists < artistNames.Length)
                    {
                        artistNames[knownArtists] = artist; // record new artist
                        artistCounts[knownArtists] = 1;     // first track for them
                        knownArtists++;
                    }
                }
            }

            // Stat 1: total playlists and a bar per playlist
            string[] favouritesForStats = ReadAllLinesSafe(favouritesFile); // favourite names
            string[] recentsForStats = ReadAllLinesSafe(recentFile);        // recent names
            string[] orderedForBars = OrderByFavouriteThenRecentThenName(playlistFiles, favouritesForStats, recentsForStats); // sorted playlists

            // count how many playlists are favourited (for the sub-caption)
            int favouriteCount = 0;
            int fc = 0;
            for (fc = 0; fc < playlistFiles.Length; fc++)
            {
                if (StringArrayContains(favouritesForStats, Path.GetFileNameWithoutExtension(playlistFiles[fc])))
                    favouriteCount++;
            }

            lblStatPlaylistsValue.Text = totalPlaylists.ToString(); // big number tile
            lblStatPlaylistsSub.Text = totalPlaylists == 0
                ? "None yet"
                : favouriteCount + (favouriteCount == 1 ? " favourite playlist" : " favourite playlists"); // sub-caption
            UpdatePlaylistBars(orderedForBars); // draw the per-playlist bar chart

            // Stat 2: total tracks and average per playlist
            lblStatTracksValue.Text = totalTracks.ToString(); // big number tile
            double avgPerPlaylist = totalPlaylists > 0 ? (double)totalTracks / totalPlaylists : 0; // average
            lblStatTracksSub.Text = totalPlaylists == 0
                ? "Across all playlists"
                : "Avg " + avgPerPlaylist.ToString("0.0") + " tracks per playlist"; // sub-caption
            lblStatTracksTrendIcon.Visible = false; // no real trend data to show

            // Stat 3: top artist and ranked top-3 bars 
            UpdateTopArtists(artistNames, artistCounts, knownArtists);
        }

        // Resizes/positions the 6 mini bar-chart panels to reflect track counts per playlist
        private void UpdatePlaylistBars(string[] orderedFiles)
        {
            Panel[] bars = { pnlBarPlaylists1, pnlBarPlaylists2, pnlBarPlaylists3, pnlBarPlaylists4, pnlBarPlaylists5, pnlBarPlaylists6 };
            Color normalColor = Color.FromArgb(221, 214, 254);    // default bar colour
            Color highlightColor = Color.FromArgb(124, 92, 255);  // colour for the tallest bar
            const int baseline = 162; // bottom Y that every bar aligns to
            const int maxHeight = 40; // tallest a bar can be drawn
            const int minHeight = 6;  // smallest visible sliver for 0 tracks

            int shownCount = Math.Min(orderedFiles.Length, bars.Length); // number of bars actually used

            // find the highest track count among the shown playlists (used to scale bar heights)
            int maxTracks = 1;
            int i = 0;
            for (i = 0; i < shownCount; i++)
            {
                int c = CountTracksInFile(orderedFiles[i]);
                if (c > maxTracks)
                    maxTracks = c;
            }

            int tallestIndex = -1;  // index of the tallest bar
            int tallestCount = -1;  // track count of the tallest bar
            for (i = 0; i < bars.Length; i++)
            {
                if (i < shownCount)
                {
                    int trackCount = CountTracksInFile(orderedFiles[i]);                               // this playlist's track count
                    int height = Math.Max(minHeight, (int)Math.Round((trackCount / (double)maxTracks) * maxHeight)); // scaled bar height
                    bars[i].Visible = true;                                                             // show this bar
                    bars[i].Size = new Size(25, height);                                                 // set bar height
                    bars[i].Location = new Point(bars[i].Location.X, baseline - height);                 // align to baseline
                    bars[i].BackColor = normalColor;                                                      // default colour
                    if (trackCount > tallestCount)
                    {
                        tallestCount = trackCount; // remember the new tallest
                        tallestIndex = i;
                    }
                }
                else
                {
                    bars[i].Visible = false; // no playlist for this slot, hide it
                }
            }

            if (tallestIndex >= 0)
            {
                bars[tallestIndex].BackColor = highlightColor; // highlight the tallest bar
            }
        }

        // Fills in the top-3 artist labels and ranked bar widths
        private void UpdateTopArtists(string[] artistNames, int[] artistCounts, int knownArtists)
        {
            Label[] nameLabels = { lblArtistName1, lblArtistName2, lblArtistName3 };
            Panel[] barBgs = { pnlArtistBarBg1, pnlArtistBarBg2, pnlArtistBarBg3 };
            Panel[] barFills = { pnlArtistBarFill1, pnlArtistBarFill2, pnlArtistBarFill3 };

            if (knownArtists == 0) // no tracks/artists at all yet
            {
                lblStatArtistValue.Text = "-";
                lblStatArtistSub.Text = "No tracks yet";
                int none = 0;
                for (none = 0; none < nameLabels.Length; none++)
                {
                    nameLabels[none].Visible = false; // hide all 3 rows
                    barBgs[none].Visible = false;
                    barFills[none].Visible = false;
                }
                return;
            }

            // manually pick the top 3 (or fewer) artists by track count
            int topN = Math.Min(3, knownArtists);
            string[] topNames = new string[topN];
            int[] topCounts = new int[topN];
            bool[] used = new bool[knownArtists]; // marks artists already picked

            int rank = 0;
            for (rank = 0; rank < topN; rank++)
            {
                int bestIndex = -1;
                int a = 0;
                for (a = 0; a < knownArtists; a++)
                {
                    if (used[a])
                        continue; // already picked for an earlier rank
                    if (bestIndex == -1 || artistCounts[a] > artistCounts[bestIndex])
                        bestIndex = a; // new best candidate for this rank
                }
                used[bestIndex] = true;              // mark as picked
                topNames[rank] = artistNames[bestIndex];
                topCounts[rank] = artistCounts[bestIndex];
            }

            lblStatArtistValue.Text = topNames[0]; // #1 artist name
            lblStatArtistSub.Text = topCounts[0] + (topCounts[0] == 1 ? " track" : " tracks"); // #1 track count

            int maxCount = topCounts[0] == 0 ? 1 : topCounts[0]; // avoid divide-by-zero when scaling bars
            const int maxBarWidth = 188; // width representing 100%
            for (rank = 0; rank < 3; rank++)
            {
                if (rank < topN)
                {
                    nameLabels[rank].Visible = true;
                    barBgs[rank].Visible = true;
                    barFills[rank].Visible = true;
                    nameLabels[rank].Text = topNames[rank]; // artist name for this rank
                    int width = Math.Max(6, (int)Math.Round((topCounts[rank] / (double)maxCount) * maxBarWidth)); // scaled bar width
                    barFills[rank].Size = new Size(width, barFills[rank].Size.Height);
                }
                else
                {
                    // fewer than 3 artists exist, hide the unused row
                    nameLabels[rank].Visible = false;
                    barBgs[rank].Visible = false;
                    barFills[rank].Visible = false;
                }
            }
        }
    }
}
