using System;
using System.Windows.Forms;

namespace BasicFacebookFeatures
{
    public partial class FormSelfAnalytics : Form
    {
        public FormSelfAnalytics()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Self Analytics";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new System.Drawing.Size(800, 600);

            // Add a label for the title
            Label labelTitle = new Label()
            {
                Text = "Self Analytics Summary",
                Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold),
                AutoSize = true,
                Location = new System.Drawing.Point(20, 20)
            };
            this.Controls.Add(labelTitle);

            // Add a placeholder for analytics data
            Label labelAnalytics = new Label()
            {
                Text = "Analytics data will be displayed here.",
                Font = new System.Drawing.Font("Segoe UI", 12),
                AutoSize = true,
                Location = new System.Drawing.Point(20, 60)
            };
            this.Controls.Add(labelAnalytics);

            // Add a "Back" button to navigate back to the menu
            Button buttonBack = new Button()
            {
                Text = "Back to Menu",
                Location = new System.Drawing.Point(20, 500),
                Size = new System.Drawing.Size(120, 40),
                BackColor = System.Drawing.Color.FromArgb(66, 103, 178),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            buttonBack.Click += (sender, e) => this.Close(); // Close the form to return to the menu
            this.Controls.Add(buttonBack);
        }
    }
}