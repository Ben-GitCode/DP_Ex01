using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BasicFacebookFeatures
{
    public partial class FormTimeline : Form
    {
        public FormTimeline()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Timeline Viewer";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new System.Drawing.Size(800, 600);

            // Add a label for the title
            Label labelTitle = new Label()
            {
                Text = "Timeline Viewer",
                Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold),
                AutoSize = true,
                Location = new System.Drawing.Point(20, 20)
            };
            this.Controls.Add(labelTitle);

            // Add a placeholder for timeline data
            Label labelTimeline = new Label()
            {
                Text = "Timeline data will be displayed here.",
                Font = new System.Drawing.Font("Segoe UI", 12),
                AutoSize = true,
                Location = new System.Drawing.Point(20, 60)
            };
            this.Controls.Add(labelTimeline);

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
