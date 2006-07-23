using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;

namespace DBlog.TransitData
{
    public enum TransitLoginRole
    {
        Unknown,
        Guest,
        Administrator
    };

    public class TransitLogin : TransitObject
    {
        private string mName;

        public string Name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
            }
        }

        private string mEmail;

        public string Email
        {
            get
            {
                return mEmail;
            }
            set
            {
                mEmail = value;
            }
        }

        private TransitLoginRole mRole = TransitLoginRole.Unknown;

        public TransitLoginRole Role
        {
            get
            {
                return mRole;
            }
            set
            {
                mRole = value;
            }
        }

        private string mUsername;

        public string Username
        {
            get
            {
                return mUsername;
            }
            set
            {
                mUsername = value;
            }
        }

        private string mPassword;

        public string Password
        {
            get
            {
                return mPassword;
            }
            set
            {
                mPassword = value;
            }
        }

        private string mWebsite;

        public string Website
        {
            get
            {
                return mWebsite;
            }
            set
            {
                mWebsite = value;
            }
        }

        public TransitLogin()
        {

        }

        public TransitLogin(DBlog.Data.Login o)
            : base(o.Id)
        {
            Email = o.Email;
            Name = o.Name;
            Password = o.Password;
            Username = o.Username;
            Website = o.Website;
            Role = (TransitLoginRole) Enum.Parse(typeof(TransitLoginRole), o.Role);
        }

        public Login GetLogin(ISession session)
        {
            Login login = (Id != 0) ? (Login) session.Load(typeof(Login), Id) : new Login();
            login.Name = Name;
            login.Email = Email;
            if (login.Password != Password)
            {
                // update password, not current password hash
                login.Password = ManagedLogin.GetPasswordHash(Password);
            }
            login.Role = Role.ToString();
            login.Username = Username;
            login.Website = Website;
            return login;
        }

        public bool IsAdministrator
        {
            get
            {
                return mRole == TransitLoginRole.Administrator;
            }
        }
    }
}
