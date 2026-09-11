using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Byte_me___Group_2.Login
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
