namespace RecommendationApp
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblItems = new System.Windows.Forms.Label();
            this.listBoxItems = new System.Windows.Forms.ListBox();
            this.btnRate = new System.Windows.Forms.Button();
            this.btnShowRecommendations = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblItems
            // 
            this.lblItems.AutoSize = true;
            this.lblItems.Location = new System.Drawing.Point(58, 40);
            this.lblItems.Name = "lblItems";
            this.lblItems.Size = new System.Drawing.Size(55, 16);
            this.lblItems.TabIndex = 0;
            this.lblItems.Text = "İçerikler:";
            // 
            // listBoxItems
            // 
            this.listBoxItems.FormattingEnabled = true;
            this.listBoxItems.ItemHeight = 16;
            this.listBoxItems.Location = new System.Drawing.Point(58, 65);
            this.listBoxItems.Name = "listBoxItems";
            this.listBoxItems.Size = new System.Drawing.Size(680, 260);
            this.listBoxItems.TabIndex = 1;
            // 
            // btnRate
            // 
            this.btnRate.Location = new System.Drawing.Point(58, 350);
            this.btnRate.Name = "btnRate";
            this.btnRate.Size = new System.Drawing.Size(150, 40);
            this.btnRate.TabIndex = 2;
            this.btnRate.Text = "Puan Ver";
            this.btnRate.UseVisualStyleBackColor = true;
            this.btnRate.Click += new System.EventHandler(this.btnRate_Click);
            // 
            // btnShowRecommendations
            // 
            this.btnShowRecommendations.Location = new System.Drawing.Point(228, 350);
            this.btnShowRecommendations.Name = "btnShowRecommendations";
            this.btnShowRecommendations.Size = new System.Drawing.Size(190, 40);
            this.btnShowRecommendations.TabIndex = 3;
            this.btnShowRecommendations.Text = "Önerileri Göster";
            this.btnShowRecommendations.UseVisualStyleBackColor = true;
            this.btnShowRecommendations.Click += new System.EventHandler(this.BtnShowRecommendations_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 430);
            this.Controls.Add(this.btnShowRecommendations);
            this.Controls.Add(this.btnRate);
            this.Controls.Add(this.listBoxItems);
            this.Controls.Add(this.lblItems);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblItems;
        private System.Windows.Forms.ListBox listBoxItems;
        private System.Windows.Forms.Button btnRate;
        private System.Windows.Forms.Button btnShowRecommendations;
    }
}
