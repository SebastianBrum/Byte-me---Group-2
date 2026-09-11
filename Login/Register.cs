using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Byte_me___Group_2
{
    public partial class Register : Form
    {
       public Register()
        {
            InitializeComponent();
        }

        List<User> users = new List<User>();

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // input //
            string Username = txtUsername.Text;
            string Password = txtPassword.Text;
            bool found = false;

            // processing //
            // to check if both the undername and password has been entered 
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show(" please enter a username And password");
            }

            else
            {  // checking if the exisitng accounts text file  exists// 
                if (File.Exists("ExistingUsers.txt"))
                {
                    // if it does exist the ReadUser will read it and tell us whether or not the username and passowrd entered by the user already exists // 
                    try
                    {
                        StreamReader ReadUser = new StreamReader("ExistingUsers.txt");
                        string Readline = ReadUser.ReadLine();

                        while (Readline != null)
                        {// to split the username and password into an array //
                            string[] user = Readline.Split(',');
                            // checking if the username and passoward has already been entered by the user //
                            if (user[0] == Username && user[1] == Password)
                            {
                                found = true;
                                break;// to break the loop since the account exists //
                            }

                            Readline = ReadUser.ReadLine();


                        }
                        ReadUser.Close();


                    } // to catch any error that pops up  //
                    catch (Exception generalException)
                    {
                        MessageBox.Show("there is an error please try again later " + generalException.Message);

                    }
                }

            }
            /* if is it true then tell the user that the username they entered already exists , if false then add the user */
            if (found)
            {
                lblOutput.Text = " this username already exists! please login or enter another one ";
                txtUsername.Clear();
                txtPassword.Clear();
            }
            else
            {

                // to add the username and password to the existing accounts text file
                users.Add(new User(Username, Password));
                WriteUsersToFile();
                lblOutput.Text = "Account created successfully!";


               
            FrmMain login = new FrmMain ();
             login.Show();
            this.Hide();   

            }
        }


        // Serialization
        // Saves the current users list to the file
        public void WriteUsersToFile()
        {
            try
            {
                // Open (or create) the users file for writing
                FileStream file = new FileStream("ExsistingUsers.txt", FileMode.Create, FileAccess.Write);

                // Create BinaryFormatter
                BinaryFormatter formatter = new BinaryFormatter();

                // Convert the List<User> into binary data and write it to the file
                formatter.Serialize(file, users);

                // close the file
                file.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
