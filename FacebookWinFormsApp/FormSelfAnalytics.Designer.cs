using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BasicFacebookFeatures
{
    public partial class FormSelfAnalytics : Form
    {
        private IContainer components = null;

        private Panel panelCard;
        private PictureBox pictureBoxProfile;
        private Label labelName;
        private Label labelSubtitle;
        private Label labelBirthday;
        private Label labelGender;
        private Label labelStats;
        private ListBox listBoxFriends;
        private Button buttonBack;

        private readonly Size formClientSize = new Size(820, 620);

        private readonly Point panelCardLocation = new Point(60, 40);
        private readonly Size panelCardSize = new Size(700, 460);

        private readonly Point pictureBoxProfileLocation = new Point(28, 30);
        private readonly Size pictureBoxProfileSize = new Size(160, 160);

        private readonly Point labelNameLocation = new Point(210, 20);
        private readonly Size labelNameSize = new Size(440, 60);
        private readonly float labelNameFontSize = 20F;

        private readonly int labelVerticalGap = 30;

        private readonly Size labelSubtitleSize = new Size(440, 30);
        private readonly float labelSubtitleFontSize = 10F;
        private readonly Point labelSubtitleLocation;

        private readonly Size labelBirthdaySize = new Size(440, 30);
        private readonly float labelBirthdayFontSize = 10F;
        private readonly Point labelBirthdayLocation;

        private readonly Size labelGenderSize = new Size(440, 30);
        private readonly float labelGenderFontSize = 10F;
        private readonly Point labelGenderLocation;

        private readonly Point labelStatsLocation = new Point(210, 300);
        private readonly Size labelStatsSize = new Size(440, 120);
        private readonly float labelStatsFontSize = 10F;

        private readonly Point listBoxFriendsLocation = new Point(28, 210);
        private readonly Size listBoxFriendsSize = new Size(160, 200);
        private readonly float listBoxFriendsFontSize = 9F;

        private readonly Size buttonBackSize = new Size(100, 36);
        private readonly Point buttonBackLocation = new Point(640, 520);

        public FormSelfAnalytics()
        {
            labelSubtitleLocation = new Point(labelNameLocation.X, labelNameLocation.Y + labelNameSize.Height + labelVerticalGap);
            labelBirthdayLocation = new Point(labelNameLocation.X, labelSubtitleLocation.Y + labelSubtitleSize.Height + labelVerticalGap);
            labelGenderLocation = new Point(labelNameLocation.X, labelBirthdayLocation.Y + labelBirthdaySize.Height + labelVerticalGap);

            InitializeComponent();
        }

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

            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = formClientSize;
            this.Name = "FormSelfAnalytics";
            this.Text = "Self Analytics - ID Card";
            this.BackColor = SystemColors.Control;
            this.DoubleBuffered = true;

            panelCard = new Panel()
            {
                Location = panelCardLocation,
                Size = panelCardSize,
                BackColor = Color.Transparent
            };
            panelCard.Paint += panelCard_Paint;

            pictureBoxProfile = new PictureBox()
            {
                Location = pictureBoxProfileLocation,
                Size = pictureBoxProfileSize,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White
            };

            labelName = new Label()
            {
                Location = labelNameLocation,
                Size = labelNameSize,
                Font = new Font("Segoe UI Semibold", labelNameFontSize, FontStyle.Bold),
                ForeColor = Color.FromArgb(12, 36, 86),
                Text = "(name)",
                AutoEllipsis = true,
                BackColor = Color.Transparent
            };

            labelSubtitle = new Label()
            {
                Location = labelSubtitleLocation,
                Size = labelSubtitleSize,
                Font = new Font("Segoe UI", labelSubtitleFontSize, FontStyle.Italic),
                ForeColor = Color.FromArgb(90, 90, 110),
                Text = "Personal ID Card",
                BackColor = Color.Transparent
            };

            labelBirthday = new Label()
            {
                Location = labelBirthdayLocation,
                Size = labelBirthdaySize,
                Font = new Font("Segoe UI", labelBirthdayFontSize),
                ForeColor = Color.FromArgb(34, 34, 34),
                Text = "(birthday)",
                BackColor = Color.Transparent
            };

            labelGender = new Label()
            {
                Location = labelGenderLocation,
                Size = labelGenderSize,
                Font = new Font("Segoe UI", labelGenderFontSize),
                ForeColor = Color.FromArgb(34, 34, 34),
                Text = "(gender)",
                BackColor = Color.Transparent
            };

            labelStats = new Label()
            {
                Location = labelStatsLocation,
                Size = labelStatsSize,
                Font = new Font("Consolas", labelStatsFontSize),
                ForeColor = Color.FromArgb(40, 40, 40),
                Text = "",
                AutoSize = false,
                BackColor = Color.Transparent
            };

            listBoxFriends = new ListBox()
            {
                Location = listBoxFriendsLocation,
                Size = listBoxFriendsSize,
                Font = new Font("Segoe UI", listBoxFriendsFontSize),
                BackColor = Color.WhiteSmoke
            };

            buttonBack = new Button()
            {
                Text = "Back",
                Size = buttonBackSize,
                Location = buttonBackLocation,
                BackColor = Color.FromArgb(66, 103, 178),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            buttonBack.Click += buttonBack_Click;

            panelCard.Controls.Add(pictureBoxProfile);
            panelCard.Controls.Add(labelName);
            panelCard.Controls.Add(labelSubtitle);
            panelCard.Controls.Add(labelBirthday);
            panelCard.Controls.Add(labelGender);
            panelCard.Controls.Add(labelStats);
            panelCard.Controls.Add(listBoxFriends);

            this.Controls.Add(panelCard);
            this.Controls.Add(buttonBack);

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
