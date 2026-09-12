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
//this allows the class to have access to the serialization and deserialization methods
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Byte_me___Group_2
{
    public partial class Register : Form
    {
        // this list that holds all existing users
        List<User> ExistingUsers = new List<User>();
        public Register()
        {
            InitializeComponent();
        }
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
                return;
            }

            else
            {  // checking if the exisitng accounts  file  exists// 
                if (File.Exists("ExistingUsers.ser"))
                {
                    // if it exists , load all the saved users into the list first
                    try
                    {
                        ReadDataFromFile("ExistingUsers", ExistingUsers);

                        // now checking is the username and password already exists
                        for (int i = 0; i < ExistingUsers.Count; i++)
                        {
                            if (ExistingUsers[i].Username == Username)
                            {
                                found = true;
                                break;// to break  the loop since the account exists
                            }
                        }

                    }

                    // to catch any error that pops up  //
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
                User newUser = new User(Username, Password);
                ExistingUsers.Add(newUser);

                // save the whole updated list back to the file 
                WriteDataToFile("ExistingUsers", ExistingUsers);

                MessageBox.Show("Account created successfully!");

                FrmMain login = new FrmMain();
                login.Show();
                this.Hide();
            }
        }

        // Serialises the list of users to a file
        public void WriteDataToFile(string listnName, List<User> myList)
        {
            try
            { // opens/create the file for writing
                FileStream outfile = new FileStream(listnName + ".ser", FileMode.Create, FileAccess.Write);
                BinaryFormatter bFormatter = new BinaryFormatter();

                // write the whole list to file as bytes 
                bFormatter.Serialize(outfile, myList);
                outfile.Close();

            }
            // to catch all the generale expections that may arise 
            catch (Exception generalException)
            {
                MessageBox.Show("there is an error saving your data " + generalException.Message);
            }


        }


        // Deserialises the list of users from a file
        public void ReadDataFromFile(string listName, List<User> myList)
        {
            try
            {
                //open the existing file for reading 
                FileStream infile = new FileStream(listName + ".ser", FileMode.Open, FileAccess.Read);
                BinaryFormatter bFormatter = new BinaryFormatter();

                myList.Clear();

                //convert the bytes back into the List<User>
                List<User> tempList = (List<User>)bFormatter.Deserialize(infile);

                for (int i = 0; i < tempList.Count; i++)
                {
                    myList.Add(tempList[i]);
                }
                infile.Close();

            }
            catch (FileNotFoundException)
            {

            }
            catch (Exception generalException)
            {
                MessageBox.Show("there is an error reading your data " + generalException.Message);
            }
        }
    }
}
