using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Byte_me___Group_2.Playlist
{
    public class SongHistory
    {
        // Playback history
        // Track the last 50 songs played
        private const int MaxSongs = 50;

        //  Store the playback history
        private List<string> history = new List<string>();

        //  Tracks the current position in the history
        //   initialized -1 to indicate that there is no song played yet
        private int currentIndex = -1;

        //  Store the path of the playback history file
        private string playbackHistoryFile;


        /// <summary>
        /// Initialize the playback history for a specific user 
        /// </summary>
        /// <param name="playlistFolder"> Playlistfolder filepath </param>
        /// <param name="playlistName"> The name of the playlist </param>
        public SongHistory(string playlistFolder, string playlistName)
        {
            //  the Path.Combine method is used to create the path for the playback history file
            playbackHistoryFile = Path.Combine(playlistFolder, $"{playlistName}SongQueue.txt");
            MessageBox.Show(playbackHistoryFile);

            //  the LoadHistory method is called to load the playback history from the file
            LoadHistory();
        }

        // Add a newly played song
        public void AddSong(string song)
        {
            // Check if the song is null or empty
            if (string.IsNullOrWhiteSpace(song))
                return;

            // If the user went backwards and then plays a new song,
            // remove the old forward history.
            if (currentIndex < history.Count - 1)
            {
                // Remove all songs after the current index
                history.RemoveRange(
                    currentIndex + 1,
                    history.Count - currentIndex - 1
                );
            }

            // Add the new song
            history.Add(song);

            // Make sure only the latest 50 songs are stored
            if (history.Count > MaxSongs)
            {
                // Remove the oldest song  ,the first in the list
                history.RemoveAt(0);
            }
            // Update the current index to point to the last song
            // the history.Count - 1 is used to get the index of the last song in the history list
            currentIndex = history.Count - 1;
        }

        // Go backwards
        public string PreviousSong()
        {
            // Check if there is a previous song to go back to
            if (currentIndex <= 0)
                // If there is no previous song, return null
                return null;

            // Decrement
            currentIndex--;

            // Return the previous song
            return history[currentIndex];
        }

        // Go forwards
        public string NextSong()
        {// Check if there is a next song to go forward to
            // so the currentIndex >= history.Count - 1 is used to
            // check if the current index is at the last song in the history list
            if (currentIndex >= history.Count - 1)
                return null;

            // Increment
            currentIndex++;

            return history[currentIndex];
        }

        // Returns the last song played
        public string LastPlayedSong()
        {
            // Check if there are any songs in the history
            if (history.Count == 0)
                return null;

            return history[currentIndex];
        }

        // Save queue to file
        // the SaveHistory method is used to save the playback history to a file
        private void SaveHistory()
        {
            try
            {
                // the using statement is used to create a StreamWriter
                // object that writes to the playbackHistoryFile

                // the class StreamWriter is used to write text to a file
                using (StreamWriter writer = new StreamWriter(playbackHistoryFile))
                {
                    // Write the current index to the first line of the file
                    writer.WriteLine(currentIndex);
                    // Write each song in the history to the file
                    foreach (string song in history)
                    {
                        // the WriteLine method is used to write each
                        // song to a new line in the file
                        writer.WriteLine(song);
                    }
                }
            }

            catch (Exception)
            {
                // Prevent the application from crashing
            }
        }

        // Load queue from file
        private void LoadHistory()
        {
            try
            {
                // Check if the file exists before trying to read it
                //!file.Exists(playbackHistoryFile) is used to check if the file exists
                if (!File.Exists(playbackHistoryFile))
                    return;

                // Read all lines from the file into an array of strings
                string[] lines = File.ReadAllLines(playbackHistoryFile);

                // Check if the file is empty
                if (lines.Length == 0)
                    return;

                // convert the first line of
                //the file "which should be the current index... from a string to an integer
                currentIndex = Convert.ToInt32(lines[0]);

                // the Skip method is used to skip the first line of the file
                history = lines
                    .Skip(1)
                    // the Take method is used to take only the last 50 songs from the history  
                    .Take(MaxSongs)
                    // the ToList method is used to convert the <string>
                    // returned by the Skip and Take methods into a List<string>
                    .ToList();


                // Ensure currentIndex is within the valid range
                //if the currentIndex is less than -1, set it to -1
                if (currentIndex < -1)
                    currentIndex = -1;


                // if the currentIndex is greater than or equal
                // to the number of songs in the history, set it to the last song in the history
                if (currentIndex >= history.Count)
                    currentIndex = history.Count - 1;
            }
            catch (Exception)
            {
                // history.Clear() is used to clear the playback history list
                history.Clear();
                currentIndex = -1;
            }
        }
    }
}
