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

            this.SuspendLayout();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(800, 600);
            this.Name = "FormMedia";
            this.Text = "Media Viewer";
            this.BackColor = Color.FromArgb(235, 236, 237);

            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 72,
                BackColor = Color.FromArgb(59, 89, 152)
            };
            Label headerTitle = new Label
            {
                Text = "Media",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(14, 12),
                BackColor = Color.Transparent
            };
            Label headerSub = new Label
            {
                Text = "Albums • Posts • Photos",
                ForeColor = Color.FromArgb(220, 220, 220),
                Font = new Font("Segoe UI", 9F),
                AutoSize = true,
                Location = new Point(16, 50),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(headerTitle);
            headerPanel.Controls.Add(headerSub);

            tabControl1 = new TabControl()
            {
                Dock = DockStyle.Fill
            };

            tabPageAlbums = new TabPage("Albums") { BackColor = Color.FromArgb(250, 250, 250) };
            Panel albumsLeft = new Panel { Dock = DockStyle.Left, Width = 300, BackColor = Color.White, Padding = new Padding(12) };
            Panel albumsRight = new Panel { Dock = DockStyle.Fill, BackColor = Color.Black, Padding = new Padding(12, 12, 12, 70) };

            linkAlbums = new LinkLabel()
            {
                Text = "Albums",
                Dock = DockStyle.Top,
                AutoSize = true,
                LinkColor = Color.FromArgb(66, 103, 178),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            linkAlbums.LinkClicked += linkAlbums_LinkClicked;

            listBoxAlbums = new ListBox()
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White
            };
            listBoxAlbums.SelectedIndexChanged += listBoxAlbums_SelectedIndexChanged;

            pictureBoxAlbum = new PictureBox()
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Black
            };

            Panel albumsBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 58,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(8, 6, 8, 6)
            };
            Label albumsCaption = new Label
            {
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoEllipsis = true,
                Text = ""
            };
            Label albumsMeta = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(110, 110, 110),
                AutoEllipsis = true,
                Text = ""
            };
            albumsBottom.Controls.Add(albumsMeta);
            albumsBottom.Controls.Add(albumsCaption);

            albumsLeft.Controls.Add(listBoxAlbums);
            albumsLeft.Controls.Add(linkAlbums);
            albumsRight.Controls.Add(pictureBoxAlbum);
            albumsRight.Controls.Add(albumsBottom);
            tabPageAlbums.Controls.Add(albumsRight);
            tabPageAlbums.Controls.Add(albumsLeft);

            tabPagePosts = new TabPage("Posts") { BackColor = Color.FromArgb(250, 250, 250) };
            Panel postsLeft = new Panel { Dock = DockStyle.Left, Width = 300, BackColor = Color.White, Padding = new Padding(12) };
            Panel postsRight = new Panel { Dock = DockStyle.Fill, BackColor = Color.Black, Padding = new Padding(12, 12, 12, 70) };

            linkPosts = new LinkLabel()
            {
                Text = "Posts",
                Dock = DockStyle.Top,
                AutoSize = true,
                LinkColor = Color.FromArgb(66, 103, 178),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            linkPosts.LinkClicked += linkPosts_LinkClicked;

            listBoxPosts = new ListBox()
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White
            };
            listBoxPosts.SelectedIndexChanged += listBoxPosts_SelectedIndexChanged;

            pictureBoxPost = new PictureBox()
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Black
            };

            Panel postsBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 58,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(8, 6, 8, 6)
            };
            Label postsCaption = new Label
            {
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoEllipsis = true,
                Text = ""
            };
            Label postsMeta = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(110, 110, 110),
                AutoEllipsis = true,
                Text = ""
            };
            postsBottom.Controls.Add(postsMeta);
            postsBottom.Controls.Add(postsCaption);

            postsLeft.Controls.Add(listBoxPosts);
            postsLeft.Controls.Add(linkPosts);
            postsRight.Controls.Add(pictureBoxPost);
            postsRight.Controls.Add(postsBottom);
            tabPagePosts.Controls.Add(postsRight);
            tabPagePosts.Controls.Add(postsLeft);

            tabPagePhotos = new TabPage("Photos") { BackColor = Color.FromArgb(250, 250, 250) };
            Panel photosLeft = new Panel { Dock = DockStyle.Left, Width = 300, BackColor = Color.White, Padding = new Padding(12) };
            Panel photosRight = new Panel { Dock = DockStyle.Fill, BackColor = Color.Black, Padding = new Padding(12, 12, 12, 70) };

            linkPhotos = new LinkLabel()
            {
                Text = "Photos",
                Dock = DockStyle.Top,
                AutoSize = true,
                LinkColor = Color.FromArgb(66, 103, 178),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            linkPhotos.LinkClicked += linkPhotos_LinkClicked;

            listBoxPhotos = new ListBox()
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White
            };
            listBoxPhotos.SelectedIndexChanged += listBoxPhotos_SelectedIndexChanged;

            pictureBoxPhoto = new PictureBox()
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Black
            };

            Panel photosBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 58,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(8, 6, 8, 6)
            };
            Label photosCaption = new Label
            {
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoEllipsis = true,
                Text = ""
            };
            Label photosMeta = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(110, 110, 110),
                AutoEllipsis = true,
                Text = ""
            };
            photosBottom.Controls.Add(photosMeta);
            photosBottom.Controls.Add(photosCaption);

            photosLeft.Controls.Add(listBoxPhotos);
            photosLeft.Controls.Add(linkPhotos);
            photosRight.Controls.Add(pictureBoxPhoto);
            photosRight.Controls.Add(photosBottom);
            tabPagePhotos.Controls.Add(photosRight);
            tabPagePhotos.Controls.Add(photosLeft);

            tabControl1.TabPages.AddRange(new TabPage[] { tabPageAlbums, tabPagePosts, tabPagePhotos });

            buttonBack = new Button()
            {
                Text = "Back",
                Size = new Size(100, 36),
                Location = new Point(this.ClientSize.Width - 100 - 16, this.ClientSize.Height - 36 - 12),
                BackColor = Color.FromArgb(66, 103, 178),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            buttonBack.Click += buttonBack_Click;

            this.Controls.Add(tabControl1);
            this.Controls.Add(headerPanel);
            this.Controls.Add(buttonBack);

            buttonBack.BringToFront();

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
