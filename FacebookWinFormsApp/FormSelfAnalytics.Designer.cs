using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BasicFacebookFeatures
{
    public partial class FormSelfAnalytics : Form
    {
        private IContainer components = null;

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

            // Form
            this.SuspendLayout();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(800, 600);
            this.Name = "FormSelfAnalytics";
            this.Text = "Self Analytics";

            // Analytics TextBox
            textBoxAnalytics = new TextBox()
            {
                Location = new Point(20, 20),
                Size = new Size(760, 500),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Back button
            buttonBack = new Button()
            {
                Text = "Back",
                Size = new Size(100, 36),
                Location = new Point(560, 540),
                BackColor = Color.FromArgb(66, 103, 178),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            buttonBack.Click += buttonBack_Click;

            this.Controls.Add(textBoxAnalytics);
            this.Controls.Add(buttonBack);

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
