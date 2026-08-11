using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Byte_me___Group_2
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void lblRegister_Click(object sender, EventArgs e)
        {
            // Go to register if not registred
            Register register = new Register();

            register.Show();

            this.Hide();
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            //Covert characters to * to hide password
            txtPassword.PasswordChar = '*';
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // try is used to to prevent program from crashing, if prblem while reading the file 
            try
            {
                // Get username and password user typed in
                string username = txtUsername.Text;
                string password = txtPassword.Text;

                // Check if feild is empty
                if (username == "" || password == "")
                {
                    lblError.Text = "Please enter a Username or Password";
                    return;
                }

                // Variable keep track of whether username or password were found 
                bool found = false;

                //Open users.txt file so that we can read what is stored inside 
                StreamReader reader = new StreamReader("ExistingUsers.txt");

                //Keep reading file while there are still lines left to read
                while (!reader.EndOfStream)
                {
                    //Read one line from text file
                    string line = reader.ReadLine();
                    // Split line at the comma
                    string[] user = line.Split(',');

                    // Check whether username  and password entered by user match the info in the text file
                    if (user[0] == username && user[1] == password)
                    {
                        // If they match, the login details are correct
                        found = true;
                    }
                }

                //close the text file after we have finished reading it 
                reader.Close();

                //Check if correct username and password were found
                if (found)
                {
                    lblError.Text = "";

                    //Create new home form, passing the logged-in username through
                    //so Home can show a personalised "Welcome back, <username>" message

                    Home home = new Home(username);
                    home.Show();
                    this.Hide();
                }
                else
                {
                    //If username and and password were not found, tell user  their details are incorrect
                    lblError.Text = "Incorrect UserName or Password";
                }

            }

            //If something goes wrong, display error message 

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
