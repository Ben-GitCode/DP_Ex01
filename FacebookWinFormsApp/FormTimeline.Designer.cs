// Designer file - FormTimeline.Designer.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BasicFacebookFeatures
{
    partial class FormTimeline
    {
        private System.ComponentModel.IContainer components = null;

        // Designer-managed UI controls
        private Panel topPanel;
        private ComboBox comboBoxContent;
        private ComboBox comboBoxGranularity;
        private Button buttonRefresh;
        private Button buttonBack; // <-- Added buttonBack here

        // Replaced SplitContainer with simple panels
        private Panel leftPanel;
        private Panel rightPanel;
        private ListView listViewTimeline;
        private PictureBox pictureBoxPreview;
        private WebBrowser webBrowserPreview;
        private Label placeholderLabel;

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
            this.components = new System.ComponentModel.Container();

            // Top panel
            this.topPanel = new Panel();
            this.comboBoxContent = new ComboBox();
            this.comboBoxGranularity = new ComboBox();
            this.buttonRefresh = new Button();

            // Panels and inner controls
            this.leftPanel = new Panel();
            this.rightPanel = new Panel();
            this.listViewTimeline = new ListView();
            this.pictureBoxPreview = new PictureBox();
            this.webBrowserPreview = new WebBrowser();
            this.placeholderLabel = new Label();

            this.SuspendLayout();

            //
            // topPanel - repurposed to show cover + profile area like a timeline header
            //
            this.topPanel.Dock = DockStyle.Top;
            this.topPanel.Height = 160;
            this.topPanel.BackColor = Color.FromArgb(59, 89, 152); // facebook-like blue

            // local cover area inside topPanel (no field stored)
            Panel coverPanel = new Panel()
            {
                BackColor = Color.FromArgb(59, 89, 152),
                Location = new Point(0, 0),
                Size = new Size(800, 110),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // local profile picture (overlaps cover)
            PictureBox profilePic = new PictureBox()
            {
                Location = new Point(16, 70),
                Size = new Size(84, 84),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            // user name/title label
            Label headerNameLabel = new Label()
            {
                Location = new Point(110, 60),
                AutoSize = true,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                Text = "Your Timeline",
                BackColor = Color.Transparent
            };

            // sub header / info
            Label headerSubLabel = new Label()
            {
                Location = new Point(110, 116),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(220, 220, 220),
                Text = "Recent posts, photos and activity",
                BackColor = Color.Transparent
            };

            // filters strip (placed below cover inside topPanel)
            Panel filtersPanel = new Panel()
            {
                Location = new Point(8, 132),
                Size = new Size(780, 24),
                BackColor = Color.Transparent
            };

            // comboBoxContent
            this.comboBoxContent.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxContent.Width = 160;
            this.comboBoxContent.Location = new Point(0, 0);
            this.comboBoxContent.Font = new Font("Segoe UI", 9F);
            this.comboBoxContent.Items.AddRange(new object[] { "All", "Posts", "Photos" });
            this.comboBoxContent.SelectedIndex = 0;

            // comboBoxGranularity
            this.comboBoxGranularity.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxGranularity.Width = 220;
            this.comboBoxGranularity.Location = new Point(170, 0);
            this.comboBoxGranularity.Font = new Font("Segoe UI", 9F);
            this.comboBoxGranularity.Items.AddRange(new object[] {
                "Timeline by date", "By Year", "By Month", "By Age at time of item" });
            this.comboBoxGranularity.SelectedIndex = 0;

            // buttonRefresh
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.Location = new Point(400, 0);
            this.buttonRefresh.Width = 80;
            this.buttonRefresh.Font = new Font("Segoe UI", 9F);
            this.buttonRefresh.FlatStyle = FlatStyle.System;
            this.buttonRefresh.Click += new EventHandler(this.buttonRefresh_Click);

            // assemble filters
            filtersPanel.Controls.Add(this.comboBoxContent);
            filtersPanel.Controls.Add(this.comboBoxGranularity);
            filtersPanel.Controls.Add(this.buttonRefresh);

            // add header elements to topPanel
            coverPanel.Controls.Add(profilePic);
            coverPanel.Controls.Add(headerNameLabel);
            coverPanel.Controls.Add(headerSubLabel);
            this.topPanel.Controls.Add(coverPanel);
            this.topPanel.Controls.Add(filtersPanel);

            //
            // leftPanel acts like former Panel1 - feed area
            //
            this.leftPanel.Dock = DockStyle.Left;
            this.leftPanel.Width = 420;
            this.leftPanel.MinimumSize = new Size(320, 0);
            this.leftPanel.Resize += new EventHandler(this.leftPanel_Resize);
            this.leftPanel.Padding = new Padding(12);
            this.leftPanel.BackColor = Color.FromArgb(245, 247, 250); // subtle background for feed column

            // listViewTimeline - styled to look like feed cards (simple approach)
            this.listViewTimeline.Dock = DockStyle.Fill;
            this.listViewTimeline.View = View.Details;
            this.listViewTimeline.FullRowSelect = true;
            this.listViewTimeline.MultiSelect = false;
            this.listViewTimeline.HeaderStyle = ColumnHeaderStyle.None;
            this.listViewTimeline.BorderStyle = BorderStyle.None;
            this.listViewTimeline.BackColor = Color.White;
            this.listViewTimeline.Font = new Font("Segoe UI", 10F);
            this.listViewTimeline.Columns.Clear();
            this.listViewTimeline.Columns.Add("Date", 130);
            this.listViewTimeline.Columns.Add("Type", 80);
            this.listViewTimeline.Columns.Add("Summary", 180);
            this.listViewTimeline.DoubleClick += new EventHandler(this.ListViewTimeline_DoubleClick);
            this.leftPanel.Controls.Add(this.listViewTimeline);

            //
            // rightPanel acts like former Panel2 - preview and sidebar
            //
            this.rightPanel.Dock = DockStyle.Fill;
            this.rightPanel.MinimumSize = new Size(360, 0);
            this.rightPanel.Padding = new Padding(8);
            this.rightPanel.BackColor = Color.FromArgb(250, 250, 250);

            // pictureBoxPreview - right panel
            this.pictureBoxPreview.Dock = DockStyle.Fill;
            this.pictureBoxPreview.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.BackColor = SystemColors.ControlDark;
            this.pictureBoxPreview.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Visible = false;

            // webBrowserPreview
            this.webBrowserPreview.Dock = DockStyle.Fill;
            this.webBrowserPreview.Visible = false;
            this.webBrowserPreview.ScriptErrorsSuppressed = true;

            // placeholder label (now used as preview header and info)
            this.placeholderLabel.Text = "Select a timeline item to preview image here.";
            this.placeholderLabel.Dock = DockStyle.Top;
            this.placeholderLabel.Height = 28;
            this.placeholderLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.placeholderLabel.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            this.placeholderLabel.Visible = true;
            this.placeholderLabel.BackColor = Color.Transparent;
            this.placeholderLabel.ForeColor = Color.FromArgb(90, 90, 90);

            // add preview and placeholder to rightPanel (placeholder on top)
            this.rightPanel.Controls.Add(this.pictureBoxPreview);
            this.rightPanel.Controls.Add(this.webBrowserPreview);
            this.rightPanel.Controls.Add(this.placeholderLabel);

            //
            // buttonBack - ensure instance is created before use
            //
            this.buttonBack = new Button();

            // buttonBack
            this.buttonBack.Text = "Back";
            this.buttonBack.Size = new Size(100, 36);
            this.buttonBack.Location = new Point(660, 520);
            this.buttonBack.BackColor = Color.FromArgb(66, 103, 178);
            this.buttonBack.ForeColor = Color.White;
            this.buttonBack.FlatStyle = FlatStyle.Flat;
            this.buttonBack.Click += new EventHandler(this.buttonBack_Click);
            this.Controls.Add(this.buttonBack); // <-- Added buttonBack to Controls

            // FormTimeline
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(900, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Timeline";
            this.Name = "FormTimeline";
            this.BackColor = Color.FromArgb(235, 236, 237); // app background similar to fb

            // add controls to form - topPanel first so panels fill remaining space
            this.Controls.Add(this.rightPanel);
            this.Controls.Add(this.leftPanel);
            this.Controls.Add(this.topPanel);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
