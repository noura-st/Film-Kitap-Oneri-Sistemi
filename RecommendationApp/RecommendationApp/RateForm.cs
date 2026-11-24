using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RecommendationApp
{
    public partial class RateForm : Form
    {
        private readonly List<Item> items;
        private readonly Dictionary<Item, int> userRatings;
        private readonly User currentUser;
        private Label label1;
        private Label label2;
        private ComboBox cmbItems;
        private NumericUpDown numRating;
        private Button btnSubmit;
        private Button btnSaveRatings;

        public RateForm(List<Item> items, Dictionary<Item, int> userRatings, User user)
        {
            InitializeComponent();
            this.items = items;
            this.userRatings = userRatings;
            this.currentUser = user;
        }

        private void RateForm_Load(object sender, EventArgs e)
        {
            cmbItems.DataSource = null;
            cmbItems.DataSource = items;
            cmbItems.DisplayMember = "Title";
            cmbItems.ValueMember = "ItemID";

            numRating.Minimum = 1;
            numRating.Maximum = 5;
            numRating.Value = 3;
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (cmbItems.SelectedItem is Item selected)
            {
                userRatings[selected] = (int)numRating.Value;
                MessageBox.Show(
                    $"{selected.Title} için {numRating.Value} puan eklendi.",
                    "Kayıt", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "Lütfen bir öğe seçin.",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnSaveRatings_Click(object sender, EventArgs e)
        {
            if (userRatings.Count == 0)
            {
                MessageBox.Show(
                    "Kaydedilecek puan yok.",
                    "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // One statement per rating, inside a transaction: either every score
            // lands or none does, so a failure halfway cannot leave a partial set.
            // The MERGE makes re-saving an edited score an update rather than a
            // duplicate row, which the UQ_Ratings_User_Item constraint forbids.
            const string upsert = @"
                MERGE dbo.Ratings AS target
                USING (SELECT @userId AS UserID, @itemId AS ItemID) AS source
                    ON target.UserID = source.UserID AND target.ItemID = source.ItemID
                WHEN MATCHED THEN
                    UPDATE SET Score = @score
                WHEN NOT MATCHED THEN
                    INSERT (UserID, ItemID, Score) VALUES (@userId, @itemId, @score);";

            using (var conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var kvp in userRatings)
                        {
                            using (var cmd = new SqlCommand(upsert, conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@userId", currentUser.UserID);
                                cmd.Parameters.AddWithValue("@itemId", kvp.Key.ItemID);
                                cmd.Parameters.AddWithValue("@score", kvp.Value);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                    }
                    catch (SqlException ex)
                    {
                        tx.Rollback();
                        MessageBox.Show(
                            "Puanlar kaydedilemedi:\n\n" + ex.Message,
                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            MessageBox.Show(
                "Tüm puanlar kaydedildi.",
                "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbItems = new System.Windows.Forms.ComboBox();
            this.numRating = new System.Windows.Forms.NumericUpDown();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnSaveRatings = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numRating)).BeginInit();
            this.SuspendLayout();

            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Öge Seç:";

            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Puan(1-5):";

            this.cmbItems.FormattingEnabled = true;
            this.cmbItems.Location = new System.Drawing.Point(121, 39);
            this.cmbItems.Name = "cmbItems";
            this.cmbItems.Size = new System.Drawing.Size(300, 24);
            this.cmbItems.TabIndex = 2;

            this.numRating.Location = new System.Drawing.Point(151, 117);
            this.numRating.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            this.numRating.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numRating.Name = "numRating";
            this.numRating.Size = new System.Drawing.Size(120, 22);
            this.numRating.TabIndex = 3;
            this.numRating.Value = new decimal(new int[] { 1, 0, 0, 0 });

            this.btnSubmit.Location = new System.Drawing.Point(90, 186);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(110, 30);
            this.btnSubmit.TabIndex = 4;
            this.btnSubmit.Text = "Puan Ver";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.BtnSubmit_Click);

            this.btnSaveRatings.Location = new System.Drawing.Point(215, 186);
            this.btnSaveRatings.Name = "btnSaveRatings";
            this.btnSaveRatings.Size = new System.Drawing.Size(110, 30);
            this.btnSaveRatings.TabIndex = 5;
            this.btnSaveRatings.Text = "Kaydet";
            this.btnSaveRatings.UseVisualStyleBackColor = true;
            this.btnSaveRatings.Click += new System.EventHandler(this.BtnSaveRatings_Click);

            this.ClientSize = new System.Drawing.Size(472, 260);
            this.Controls.Add(this.btnSaveRatings);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.numRating);
            this.Controls.Add(this.cmbItems);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "RateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Puan Ver";
            this.Load += new System.EventHandler(this.RateForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numRating)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
