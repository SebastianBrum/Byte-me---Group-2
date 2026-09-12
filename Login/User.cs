using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// This allows the class to have access to the serialization and deserialization methods
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Byte_me___Group_2
{
    [Serializable]
    public class User
    {
        private string mUsername;
        private string mPassword;
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
        public User(string username, string password)
        {
            mUsername = username;
            mPassword = password;
        }
    }
}
