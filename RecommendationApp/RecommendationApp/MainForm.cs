using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecommendationApp
{
    public partial class MainForm : Form
    {
        private User currentUser;
        private List<Item> allItems = new List<Item>();
        private Dictionary<Item, int> userRatings = new Dictionary<Item, int>();
        public MainForm(User user)
        {
            InitializeComponent();
            currentUser = user;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text = $"Hoş Geldin, {currentUser.Username}!";

            string connectionString = "Server=localhost\\SQLEXPRESS;Database=RecommendationDB;Trusted_Connection=True;";
            allItems.Clear();
            listBoxItems.Items.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT ItemID, Title, Type, Genre
            FROM Items";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int itemId = reader.GetInt32(0);
                        string title = reader.GetString(1);
                        string type = reader.GetString(2);
                        string genre = reader.GetString(3);

                        var item = new Item(itemId, title, type, genre);
                        allItems.Add(item);
                        listBoxItems.Items.Add(item);
                    }
                }
            }
        }

        private void btnRate_Click(object sender, EventArgs e)
        {
            RateForm rateForm = new RateForm(allItems, userRatings, currentUser);
            rateForm.ShowDialog();
        }

        private void BtnShowRecommendations_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=RecommendationDB;Trusted_Connection=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string genreQuery = @"
            SELECT TOP 1 I.Genre, SUM(R.Score) AS TotalScore
            FROM Ratings R
            JOIN Items I ON R.ItemID = I.ItemID
            WHERE R.UserID = (SELECT UserID FROM Users WHERE Username = @username)
            GROUP BY I.Genre
            ORDER BY TotalScore DESC";

                SqlCommand genreCmd = new SqlCommand(genreQuery, conn);
                genreCmd.Parameters.AddWithValue("@username", currentUser.Username);

                string favoriteGenre = null;
                using (SqlDataReader reader = genreCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        favoriteGenre = reader["Genre"].ToString();
                    }
                }

                if (favoriteGenre == null)
                {
                    MessageBox.Show("Henüz öneri yapılacak kadar puan bulunamadı.");
                    return;
                }
                string recommendQuery = @"
            SELECT I.Title, I.Type, I.Genre
            FROM Items I
            WHERE I.Genre = @genre
            AND I.ItemID NOT IN (
                SELECT ItemID FROM Ratings
                WHERE UserID = (SELECT UserID FROM Users WHERE Username = @username)
            )";

                SqlCommand recommendCmd = new SqlCommand(recommendQuery, conn);
                recommendCmd.Parameters.AddWithValue("@genre", favoriteGenre);
                recommendCmd.Parameters.AddWithValue("@username", currentUser.Username);

                using (SqlDataReader recReader = recommendCmd.ExecuteReader())
                {
                    if (!recReader.HasRows)
                    {
                        MessageBox.Show($"Favori türün: {favoriteGenre}. Ama önerilecek yeni içerik yok.");
                        return;
                    }

                    string message = $"Favori türün: {favoriteGenre}\n\nSana önerilenler:\n";
                    while (recReader.Read())
                    {
                        message += $"- {recReader["Title"]} ({recReader["Type"]})\n";
                    }

                    MessageBox.Show(message);
                }

            }
        }
    }
}

