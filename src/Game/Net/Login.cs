using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BlazeraLib
{
    public class Login
    {
        public Login(String name) :
            this(name, null)
        {

        }

        public Login(String name, String password)
        {
            Name = name;
            Password = password;
        }

        private String _name;
        private String Name
        {
            get
            {
                return _name;
            }
            set
            {
                Regex objAlphaNumericPattern = new Regex("[^a-zA-Z0-9]");
                if (objAlphaNumericPattern.IsMatch(value))
                {
                    _name = value;
                }
            }
        }

        private String Password
        {
            get;
            set;
        }
    }
}
