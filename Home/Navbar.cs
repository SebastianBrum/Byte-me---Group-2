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
        // Random subtitle options shown under the main heading
        private static readonly string[] WelcomeSubMessages = new string[]
        {
            "Here's what's happening in your library today.",
            "Let's pick up where you left off.",
            "Your playlists are right where you left them.",
            "Time to queue up something good."
        };
        private static readonly Random rng = new Random(); // shared random generator

        // Sets the personalised welcome heading + a random subtitle
        public void SetWelcomeUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return; // nothing to show without a username
            }
            // time-of-day greeting + username for the big heading
            lblMainWelcome.Text = GetTimeOfDayGreeting() + ", " + username;
            // random subtitle line
            lblMainWelcomeSub.Text = WelcomeSubMessages[rng.Next(WelcomeSubMessages.Length)];
        }

        // Returns "Good morning/afternoon/evening" based on the current hour
        private string GetTimeOfDayGreeting()
        {
            int hour = DateTime.Now.Hour; // current hour (0-23)
            if (hour < 12)
            {
                return "Good morning";
            }
            if (hour < 18)
            {
                return "Good afternoon";
            }
            return "Good evening";
        }

        // ---- Theme toggle (visual demo only) ----
        // Flips the sun/moon icon; no real theme change wired up yet
        private void lblThemeToggle_Click(object sender, EventArgs e)
        {
            isDarkMode = !isDarkMode;                     // flip the flag
            lblThemeToggle.Text = isDarkMode ? "🌙" : "☀"; // swap icon to match
        }
    }
}
