using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

            allItems.Clear();
            listBoxItems.Items.Clear();

            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();

                string query = "SELECT ItemID, Title, Type, Genre FROM Items ORDER BY Title";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Item(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3));

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

            // Puanlar kaydedildikten sonra bir sonraki öneri güncel olsun.
            if (userRatings.Count > 0)
            {
                MessageBox.Show(
                    $"{userRatings.Count} puan kaydedildi. Önerileri görmek için \"Önerileri Göster\"e bas.",
                    "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnShowRecommendations_Click(object sender, EventArgs e)
        {
            // Favourite genre: the genre with the highest total score for this user.
            string genreQuery = @"
                SELECT TOP 1 I.Genre, SUM(R.Score) AS TotalScore
                FROM Ratings R
                JOIN Items I ON R.ItemID = I.ItemID
                WHERE R.UserID = @userId
                GROUP BY I.Genre
                ORDER BY TotalScore DESC";

            // Items in that genre the user has not rated yet.
            string recommendQuery = @"
                SELECT I.Title, I.Type, I.Genre
                FROM Items I
                WHERE I.Genre = @genre
                  AND I.ItemID NOT IN (SELECT ItemID FROM Ratings WHERE UserID = @userId)
                ORDER BY I.Title";

            string favoriteGenre = null;
            var recommendations = new List<string>();

            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();

                using (SqlCommand genreCmd = new SqlCommand(genreQuery, conn))
                {
                    genreCmd.Parameters.AddWithValue("@userId", currentUser.UserID);

                    using (SqlDataReader reader = genreCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            favoriteGenre = reader["Genre"].ToString();
                        }
                    }
                }

                if (favoriteGenre == null)
                {
                    MessageBox.Show(
                        "Henüz öneri yapılacak kadar puan bulunamadı. Önce birkaç içeriği puanla.",
                        "Öneri", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (SqlCommand recommendCmd = new SqlCommand(recommendQuery, conn))
                {
                    recommendCmd.Parameters.AddWithValue("@genre", favoriteGenre);
                    recommendCmd.Parameters.AddWithValue("@userId", currentUser.UserID);

                    using (SqlDataReader reader = recommendCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            recommendations.Add($"{reader["Title"]} ({reader["Type"]})");
                        }
                    }
                }
            }

            if (recommendations.Count == 0)
            {
                MessageBox.Show(
                    $"Favori türün: {favoriteGenre}. Bu türde puanlamadığın yeni içerik kalmadı.",
                    "Öneri", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string message = $"Favori türün: {favoriteGenre}\n\nSana önerilenler:\n";
            foreach (string line in recommendations)
            {
                message += $"• {line}\n";
            }

            MessageBox.Show(message, "Öneriler", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
