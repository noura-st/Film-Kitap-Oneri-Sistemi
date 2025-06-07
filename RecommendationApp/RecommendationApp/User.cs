using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace RecommendationApp
{
    public class User
    {
        private int userID;
        private string username;

        public int UserID
        {
            get { return userID; }
            set
            {
                if (value > 0)
                    userID = value;
                else
                    throw new ArgumentException("UserID sıfırdan büyük olmalı.");
            }
        }

        public string Username
        {
            get { return username; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    username = value;
                else
                    throw new ArgumentException("Kullanıcı adı boş olamaz.");
            }
        }

        public User(int userId, string username)
        {
            UserID = userId;  
            Username = username;
        }

        public override string ToString()
        {
            return Username;
        }
    }
}