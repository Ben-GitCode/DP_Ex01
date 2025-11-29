using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BasicFacebookFeatures
{
    public partial class FormMedia : Form
    {
        private IContainer components = null;

        private TabControl tabControl1;
        private TabPage tabPageAlbums;
        private TabPage tabPagePosts;
        private TabPage tabPagePhotos;

        private LinkLabel linkAlbums;
        private ListBox listBoxAlbums;
        private PictureBox pictureBoxAlbum;

        private LinkLabel linkPosts;
        private ListBox listBoxPosts;
        private PictureBox pictureBoxPost;

        private LinkLabel linkPhotos;
        private ListBox listBoxPhotos;
        private PictureBox pictureBoxPhoto;

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
            this.Name = "FormMedia";
            this.Text = "Media Viewer";

            // TabControl
            tabControl1 = new TabControl()
            {
                Dock = DockStyle.Fill
            };

            // Albums Tab
            tabPageAlbums = new TabPage("Albums") { BackColor = Color.White };
            linkAlbums = new LinkLabel()
            {
                Text = "Albums",
                Location = new Point(20, 10),
                AutoSize = true,
                LinkColor = Color.FromArgb(66, 103, 178)
            };
            linkAlbums.LinkClicked += linkAlbums_LinkClicked;

            listBoxAlbums = new ListBox()
            {
                Location = new Point(20, 40),
                Size = new Size(320, 440),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F)
            };
            listBoxAlbums.SelectedIndexChanged += listBoxAlbums_SelectedIndexChanged;

            pictureBoxAlbum = new PictureBox()
            {
                Location = new Point(360, 40),
                Size = new Size(400, 400),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            tabPageAlbums.Controls.AddRange(new Control[] { linkAlbums, listBoxAlbums, pictureBoxAlbum });

            // Posts Tab
            tabPagePosts = new TabPage("Posts") { BackColor = Color.White };
            linkPosts = new LinkLabel()
            {
                Text = "Posts",
                Location = new Point(20, 10),
                AutoSize = true,
                LinkColor = Color.FromArgb(66, 103, 178)
            };
            linkPosts.LinkClicked += linkPosts_LinkClicked;

            listBoxPosts = new ListBox()
            {
                Location = new Point(20, 40),
                Size = new Size(320, 440),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F)
            };
            listBoxPosts.SelectedIndexChanged += listBoxPosts_SelectedIndexChanged;

            pictureBoxPost = new PictureBox()
            {
                Location = new Point(360, 40),
                Size = new Size(400, 400),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            tabPagePosts.Controls.AddRange(new Control[] { linkPosts, listBoxPosts, pictureBoxPost });

            // Photos Tab
            tabPagePhotos = new TabPage("Photos") { BackColor = Color.White };
            linkPhotos = new LinkLabel()
            {
                Text = "Photos",
                Location = new Point(20, 10),
                AutoSize = true,
                LinkColor = Color.FromArgb(66, 103, 178)
            };
            linkPhotos.LinkClicked += linkPhotos_LinkClicked;

            listBoxPhotos = new ListBox()
            {
                Location = new Point(20, 40),
                Size = new Size(320, 440),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F)
            };
            listBoxPhotos.SelectedIndexChanged += listBoxPhotos_SelectedIndexChanged;

            pictureBoxPhoto = new PictureBox()
            {
                Location = new Point(360, 40),
                Size = new Size(400, 400),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            tabPagePhotos.Controls.AddRange(new Control[] { linkPhotos, listBoxPhotos, pictureBoxPhoto });

            // Back button
            buttonBack = new Button()
            {
                Text = "Back",
                Size = new Size(100, 36),
                Location = new Point(560, 520),
                BackColor = Color.FromArgb(66, 103, 178),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            buttonBack.Click += buttonBack_Click;


            tabControl1.TabPages.AddRange(new TabPage[] { tabPageAlbums, tabPagePosts, tabPagePhotos });

            // Add controls: tabControl first, then buttons so buttons are on top
            this.Controls.Add(tabControl1);
            this.Controls.Add(buttonBack);

            buttonBack.BringToFront();

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
