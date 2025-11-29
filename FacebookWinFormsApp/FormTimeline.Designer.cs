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

            // topPanel
            this.topPanel.Dock = DockStyle.Top;
            this.topPanel.Height = 40;

            // comboBoxContent
            this.comboBoxContent.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxContent.Width = 160;
            this.comboBoxContent.Location = new Point(8, 8);
            this.comboBoxContent.Items.AddRange(new object[] { "All", "Posts", "Photos" });
            this.comboBoxContent.SelectedIndex = 0;

            // comboBoxGranularity
            this.comboBoxGranularity.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxGranularity.Width = 140;
            this.comboBoxGranularity.Location = new Point(176, 8);
            this.comboBoxGranularity.Items.AddRange(new object[] {
                "Timeline by date", "By Year", "By Month", "By Age at time of item" });
            this.comboBoxGranularity.SelectedIndex = 0;

            // buttonRefresh
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.Location = new Point(330, 6);
            this.buttonRefresh.Width = 80;
            this.buttonRefresh.Click += new EventHandler(this.buttonRefresh_Click);

            // add controls to topPanel
            this.topPanel.Controls.AddRange(new Control[] {
                this.comboBoxContent, this.comboBoxGranularity, this.buttonRefresh });

            // leftPanel acts like former Panel1
            this.leftPanel.Dock = DockStyle.Left;
            this.leftPanel.Width = 360;
            this.leftPanel.MinimumSize = new Size(220, 0);
            this.leftPanel.Resize += new EventHandler(this.leftPanel_Resize);

            // listViewTimeline - left panel
            this.listViewTimeline.Dock = DockStyle.Fill;
            this.listViewTimeline.View = View.Details;
            this.listViewTimeline.FullRowSelect = true;
            this.listViewTimeline.MultiSelect = false;
            this.listViewTimeline.Columns.Add("Date", 160);
            this.listViewTimeline.Columns.Add("Type", 90);
            this.listViewTimeline.Columns.Add("Summary", 520);
            this.listViewTimeline.DoubleClick += new EventHandler(this.ListViewTimeline_DoubleClick);
            this.leftPanel.Controls.Add(this.listViewTimeline);

            // rightPanel acts like former Panel2
            this.rightPanel.Dock = DockStyle.Fill;
            this.rightPanel.MinimumSize = new Size(360, 0);

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

            // placeholder label
            this.placeholderLabel.Text = "Select a timeline item to preview image here.";
            this.placeholderLabel.Dock = DockStyle.Top;
            this.placeholderLabel.Height = 24;
            this.placeholderLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.placeholderLabel.Visible = true;

            // add preview and placeholder to rightPanel (placeholder on top)
            this.rightPanel.Controls.Add(this.pictureBoxPreview);
            this.rightPanel.Controls.Add(this.webBrowserPreview);
            this.rightPanel.Controls.Add(this.placeholderLabel);

            // FormTimeline
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Timeline";
            this.Name = "FormTimeline";

            // add controls to form - topPanel first so panels fill remaining space
            this.Controls.Add(this.rightPanel);
            this.Controls.Add(this.leftPanel);
            this.Controls.Add(this.topPanel);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
