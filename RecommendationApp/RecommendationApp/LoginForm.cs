using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RecommendationApp
{
    public partial class LoginForm : Form
    {
        public User LoggedInUser { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifreyi gir.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();

                // PasswordHash is fetched and verified in code: a WHERE Password = @password
                // clause would compare against the stored hash and could never match.
                string query = "SELECT UserID, PasswordHash FROM Users WHERE Username = @username";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            // Same message as a wrong password, so the form does not
                            // reveal which usernames exist.
                            MessageBox.Show("Kullanıcı adı veya şifre hatalı!");
                            return;
                        }

                        int userId = reader.GetInt32(0);
                        string storedHash = reader.IsDBNull(1) ? null : reader.GetString(1);

                        if (!PasswordHasher.Verify(password, storedHash))
                        {
                            MessageBox.Show("Kullanıcı adı veya şifre hatalı!");
                            return;
                        }

                        LoggedInUser = new User(userId, username);
                    }
                }
            }

            // ShowDialog returns only when the main form closes; Program.cs reads
            // LoggedInUser afterwards.
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifreyi girin.");
                return;
            }

            if (password.Length < 8)
            {
                MessageBox.Show("Şifre en az 8 karakter olmalı.");
                return;
            }

            string hash = PasswordHasher.Hash(password);

            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();

                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@username", username);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Bu kullanıcı adı zaten kayıtlı.");
                        return;
                    }
                }

                string insertQuery =
                    "INSERT INTO Users (Username, PasswordHash) OUTPUT INSERTED.UserID VALUES (@username, @hash)";
                int userId;
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@username", username);
                    insertCmd.Parameters.AddWithValue("@hash", hash);
                    userId = (int)insertCmd.ExecuteScalar();
                }

                MessageBox.Show("Kayıt başarılı!");

                LoggedInUser = new User(userId, username);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
