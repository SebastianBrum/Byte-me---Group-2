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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Byte_me___Group_2
{
    public partial class FrmMain : Form
    {
        // List that stores all user objects
        List<User> users = new List<User>();
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

        // Deserialization
        //Reads the saved users from the file
        public void ReadUsersFromFile()
        {
            try
            {
                // Open the users file
                FileStream file = new FileStream("ExistingUsers.ser", FileMode.Open, FileAccess.Read);

                // Create BinaryFormatter
                BinaryFormatter formatter = new BinaryFormatter();

                // Convert the binary data back into a List<user>
                users = (List<User>)formatter.Deserialize(file);

                // close the file
                file.Close();
            }
            catch (FileNotFoundException)
            {
                // File does not exsist yet
                users = new List<User>();
            }
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

                // Deserialize users from file
                ReadUsersFromFile();

                // Variable keep track of whether username or password were found 
                bool found = false;

                // Go through each User object in the list
                foreach (User user in users)
                {
                    //Check if username and password match
                    if (user.Username == username && user.Password == password)
                    {
                        found = true;
                        break;
                    }
                }

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
