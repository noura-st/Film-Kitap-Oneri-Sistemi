using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecommendationApp
{
    public partial class RateForm : Form
    {
        private readonly List<Item> items;
        private readonly Dictionary<Item, int> userRatings;
        private Label label1;
        private Label label2;
        private ComboBox cmbItems;
        private NumericUpDown numRating;
        private Button btnSubmit;
        private Button btnSaveRatings;
        private readonly User currentUser;


        public RateForm(List<Item> items, Dictionary<Item, int> userRatings, User user)
        {
            InitializeComponent();
            this.items = items;
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

            btnSubmit.Click += BtnSubmit_Click;
            btnSaveRatings.Click += BtnSaveRatings_Click;
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (cmbItems.SelectedItem is Item selected)
            {
                userRatings[selected] = (int)numRating.Value;
                MessageBox.Show($"{selected.Title} için {numRating.Value} puan eklendi.",
                                "Kayıt", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Lütfen bir öğe seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnSaveRatings_Click(object sender, EventArgs e)
        {
            string cs = "Server=localhost\\SQLEXPRESS;Database=RecommendationDB;Trusted_Connection=True;";
            using (var conn = new SqlConnection(cs))
            {
                conn.Open();
                foreach (var kvp in userRatings)
                {
                    var item = kvp.Key;
                    var score = kvp.Value;

                    string q = @"INSERT INTO Ratings (UserID, ItemID, Score)
                                 VALUES (@u,@i,@s)";
                    using (var cmd = new SqlCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", currentUser.UserID);
                        cmd.Parameters.AddWithValue("@i", item.ItemID);
                        cmd.Parameters.AddWithValue("@s", score);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            MessageBox.Show("Tüm puanlar kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            this.cmbItems.Size = new System.Drawing.Size(121, 24);
            this.cmbItems.TabIndex = 2;
           
            this.numRating.Location = new System.Drawing.Point(151, 117);
            this.numRating.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numRating.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numRating.Name = "numRating";
            this.numRating.Size = new System.Drawing.Size(120, 22);
            this.numRating.TabIndex = 3;
            this.numRating.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
           
            this.btnSubmit.Location = new System.Drawing.Point(90, 186);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 4;
            this.btnSubmit.Text = "Puan Ver";
            this.btnSubmit.UseVisualStyleBackColor = true;
           
            this.btnSaveRatings.Location = new System.Drawing.Point(90, 241);
            this.btnSaveRatings.Name = "btnSaveRatings";
            this.btnSaveRatings.Size = new System.Drawing.Size(75, 23);
            this.btnSaveRatings.TabIndex = 5;
            this.btnSaveRatings.Text = "Kaydet";
            this.btnSaveRatings.UseVisualStyleBackColor = true;
           
            this.ClientSize = new System.Drawing.Size(472, 372);
            this.Controls.Add(this.btnSaveRatings);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.numRating);
            this.Controls.Add(this.cmbItems);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "RateForm";
            ((System.ComponentModel.ISupportInitialize)(this.numRating)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
