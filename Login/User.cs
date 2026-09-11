using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// This allows the class to have access to the serialization and deserialization methods
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Byte_me___Group_2
{
    [Serializable]
    public  class User
    {
        // private  data members for the username and password
        // the m is used to indicate that the variable is a member of the class
        private string mUsername;
        private string mPassword;


        // these the properties that let other classes to read 
        // set mUsername and mPassword since they are private 
        public string Username
        {
            get { return mUsername; }
            set { mUsername = value; }
        }

        public string Password
        {
            get { return mPassword; }
            set { mPassword = value; }
        }

        //defAult constructor for the user class that initializes the username and password to empty strings
        public User()
        {
            mUsername = "";
            mPassword = "";

        }

        // parameterized constructor for the user class
        // that initializes the username and password to the values passed in as parameters
        public User(string username, string password)
        {
            mUsername = username;
            mPassword = password;
        }

         

    }
}
