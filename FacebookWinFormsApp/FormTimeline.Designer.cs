using System;
using System.Drawing;
using System.Windows.Forms;

namespace BasicFacebookFeatures
{
    partial class FormTimeline
    {
        private System.ComponentModel.IContainer components = null;

        private Panel topPanel;
        private ComboBox comboBoxContent;
        private ComboBox comboBoxGranularity;
        private Button buttonRefresh;
        private Button buttonBack;

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

            this.topPanel = new Panel();
            this.comboBoxContent = new ComboBox();
            this.comboBoxGranularity = new ComboBox();
            this.buttonRefresh = new Button();

            this.leftPanel = new Panel();
            this.rightPanel = new Panel();
            this.listViewTimeline = new ListView();
            this.pictureBoxPreview = new PictureBox();
            this.webBrowserPreview = new WebBrowser();
            this.placeholderLabel = new Label();

            this.SuspendLayout();

            this.topPanel.Dock = DockStyle.Top;
            this.topPanel.Height = 160;
            this.topPanel.BackColor = ColorPalette.sr_FacebookBlue;

            Panel coverPanel = new Panel()
            {
                BackColor = ColorPalette.sr_FacebookBlue,
                Location = new Point(0, 0),
                Size = new Size(800, 110),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            PictureBox profilePic = new PictureBox()
            {
                Location = new Point(16, 70),
                Size = new Size(84, 84),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = ColorPalette.sr_WhitishBlue // was sr_LightModePanelBackground
            };

            Label headerNameLabel = new Label()
            {
                Location = new Point(110, 60),
                AutoSize = true,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = ColorPalette.sr_White, // primary title over Facebook blue
                Text = "Your Timeline",
                BackColor = Color.Transparent
            };

            Label headerSubLabel = new Label()
            {
                Location = new Point(110, 116),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = ColorPalette.sr_LightGray, // was sr_CardInnerBorderLight (neutral supporting text)
                Text = "Recent posts, photos and activity",
                BackColor = Color.Transparent
            };

            Panel filtersPanel = new Panel()
            {
                Location = new Point(8, 132),
                Size = new Size(780, 24),
                BackColor = Color.Transparent
            };

            this.comboBoxContent.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxContent.Width = 160;
            this.comboBoxContent.Location = new Point(0, 0);
            this.comboBoxContent.Font = new Font("Segoe UI", 9F);
            this.comboBoxContent.Items.AddRange(new object[] { "All", "Posts", "Photos" });
            this.comboBoxContent.SelectedIndex = 0;

            this.comboBoxGranularity.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxGranularity.Width = 220;
            this.comboBoxGranularity.Location = new Point(170, 0);
            this.comboBoxGranularity.Font = new Font("Segoe UI", 9F);
            this.comboBoxGranularity.Items.AddRange(new object[] {
                "Timeline by date", "By Year", "By Month", "By Age at time of item" });
            this.comboBoxGranularity.SelectedIndex = 0;

            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.Location = new Point(400, 0);
            this.buttonRefresh.Width = 80;
            this.buttonRefresh.Font = new Font("Segoe UI", 9F);
            this.buttonRefresh.FlatStyle = FlatStyle.System;
            this.buttonRefresh.Click += new EventHandler(this.buttonRefresh_Click);

            filtersPanel.Controls.Add(this.comboBoxContent);
            filtersPanel.Controls.Add(this.comboBoxGranularity);
            filtersPanel.Controls.Add(this.buttonRefresh);

            coverPanel.Controls.Add(profilePic);
            coverPanel.Controls.Add(headerNameLabel);
            coverPanel.Controls.Add(headerSubLabel);
            this.topPanel.Controls.Add(coverPanel);
            this.topPanel.Controls.Add(filtersPanel);

            this.leftPanel.Dock = DockStyle.Left;
            this.leftPanel.Width = 420;
            this.leftPanel.MinimumSize = new Size(320, 0);
            this.leftPanel.Resize += new EventHandler(this.leftPanel_Resize);
            this.leftPanel.Padding = new Padding(12);
            this.leftPanel.BackColor = ColorPalette.sr_White;

            this.listViewTimeline.Dock = DockStyle.Fill;
            this.listViewTimeline.View = View.Details;
            this.listViewTimeline.FullRowSelect = true;
            this.listViewTimeline.MultiSelect = false;
            this.listViewTimeline.HeaderStyle = ColumnHeaderStyle.None;
            this.listViewTimeline.BorderStyle = BorderStyle.None;
            this.listViewTimeline.BackColor = ColorPalette.sr_WhitishBlue; // was sr_LightModePanelBackground
            this.listViewTimeline.Font = new Font("Segoe UI", 10F);
            this.listViewTimeline.Columns.Clear();
            this.listViewTimeline.Columns.Add("Date", 130);
            this.listViewTimeline.Columns.Add("Type", 80);
            this.listViewTimeline.Columns.Add("Summary", 180);
            this.listViewTimeline.DoubleClick += new EventHandler(this.ListViewTimeline_DoubleClick);
            this.leftPanel.Controls.Add(this.listViewTimeline);

            this.rightPanel.Dock = DockStyle.Fill;
            this.rightPanel.MinimumSize = new Size(360, 0);
            this.rightPanel.Padding = new Padding(8);
            this.rightPanel.BackColor = ColorPalette.sr_White;

            this.pictureBoxPreview.Dock = DockStyle.Fill;
            this.pictureBoxPreview.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.BackColor = SystemColors.ControlDark;
            this.pictureBoxPreview.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Visible = false;

            this.webBrowserPreview.Dock = DockStyle.Fill;
            this.webBrowserPreview.Visible = false;
            this.webBrowserPreview.ScriptErrorsSuppressed = true;

            this.placeholderLabel.Text = "Select a timeline item to preview image here.";
            this.placeholderLabel.Dock = DockStyle.Top;
            this.placeholderLabel.Height = 28;
            this.placeholderLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.placeholderLabel.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            this.placeholderLabel.Visible = true;
            this.placeholderLabel.BackColor = Color.Transparent;
            this.placeholderLabel.ForeColor = ColorPalette.sr_MidGray;

            this.rightPanel.Controls.Add(this.pictureBoxPreview);
            this.rightPanel.Controls.Add(this.webBrowserPreview);
            this.rightPanel.Controls.Add(this.placeholderLabel);

            this.buttonBack = new Button();

            this.buttonBack.Text = "Back";
            this.buttonBack.Size = new Size(100, 36);
            this.buttonBack.Location = new Point(660, 520);
            this.buttonBack.BackColor = ColorPalette.sr_FacebookBlue;
            this.buttonBack.ForeColor = Color.White;
            this.buttonBack.FlatStyle = FlatStyle.Flat;
            this.buttonBack.Click += new EventHandler(this.buttonBack_Click);
            this.Controls.Add(this.buttonBack);

            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(900, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Timeline";
            this.Name = "FormTimeline";
            this.BackColor = ColorPalette.sr_White;

            this.Controls.Add(this.rightPanel);
            this.Controls.Add(this.leftPanel);
            this.Controls.Add(this.topPanel);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void leftPanel_Resize(object sender, EventArgs e)
        {
            AdjustColumns();
        }

        private void AdjustColumns()
        {
            if (leftPanel == null || listViewTimeline == null)
            {
                return;
            }

            int dateWidth = 160;
            int typeWidth = 90;
            int summaryWidth = Math.Max(120, leftPanel.ClientSize.Width - dateWidth - typeWidth - 12);

            if (listViewTimeline.Columns.Count == 3)
            {
                listViewTimeline.Columns[0].Width = dateWidth;
                listViewTimeline.Columns[1].Width = typeWidth;
                listViewTimeline.Columns[2].Width = summaryWidth;
            }
        }
    }
}