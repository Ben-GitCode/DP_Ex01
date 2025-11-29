using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BasicFacebookFeatures
{
    partial class FormSelfAnalytics
    {
        private IContainer components = null;

        private Label labelTitle;
        private TextBox textBoxAnalytics;
        private Button buttonBack;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            this.SuspendLayout();

            // labelTitle
            labelTitle = new Label()
            {
                Text = "Self Analytics Summary",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            this.Controls.Add(labelTitle);

            // textBoxAnalytics
            textBoxAnalytics = new TextBox()
            {
                Location = new Point(20, 60),
                Size = new Size(740, 420),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 10F),
                BackColor = Color.White
            };
            this.Controls.Add(textBoxAnalytics);

            // buttonBack
            buttonBack = new Button()
            {
                Text = "Back to Menu",
                Location = new Point(20, 500),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(66, 103, 178),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Name = "buttonBack"
            };
            buttonBack.Click += (sender, e) => this.Close();
            this.Controls.Add(buttonBack);

            // FormSelfAnalytics
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Self Analytics";
            this.Name = "FormSelfAnalytics";

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
