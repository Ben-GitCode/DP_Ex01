using System.Windows.Forms;
using System.Drawing;

namespace BasicFacebookFeatures
{
    partial class FormMain
    {
        private System.ComponentModel.IContainer components = null;

        private TabControl tabControl1;
        private TabPage tabPageLogin;

        private Button buttonLogin;
        private Button buttonLogout;
        private Button buttonConnectAsDesig;
        private Button buttonGoToMenu;

        private TextBox textBoxAppID;
        private PictureBox pictureBoxProfile;

        private LinkLabel linkAlbums;
        private LinkLabel linkPages;
        private LinkLabel linkPosts;
        private LinkLabel linkPhotos;

        private ListBox listBoxAlbums;
        private ListBox listBoxPages;
        private ListBox listBoxPosts;
        private ListBox listBoxPhotos;

        private PictureBox pictureBoxAlbum;
        private PictureBox pictureBoxPage;
        private PictureBox pictureBoxPost;
        private PictureBox pictureBoxPhoto;

        private Panel panelBottom;
        private Panel toggleBackground;
        private Panel toggleCircle;
        private Label labelDarkMode;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {

            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Facebook Features";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = Color.FromArgb(66, 103, 178);
            this.ClientSize = new Size(1250, 600);
            this.MinimumSize = new Size(1250, 600);
            this.MaximumSize = new Size(1250, 600);

            tabControl1 = new TabControl()
            {
                Dock = DockStyle.Fill,
                Appearance = TabAppearance.Normal,
                ItemSize = new Size(120, 25)
            };

            tabPageLogin = new TabPage("Login") { BackColor = Color.White };
            TabPage tabPageMenu = new TabPage("Menu") { BackColor = Color.White, Name = "Menu" };

            tabControl1.TabPages.Add(tabPageLogin);
            tabControl1.TabPages.Add(tabPageMenu);
            this.Controls.Add(tabControl1);

            panelBottom = new Panel()
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(66, 103, 178)
            };

            labelDarkMode = new Label()
            {
                Text = "Dark Mode",
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };

            toggleBackground = new Panel()
            {
                Size = new Size(50, 25),
                Location = new Point(120, 17),
                BackColor = Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle
            };

            toggleCircle = new Panel()
            {
                Size = new Size(23, 23),
                Location = new Point(1, 1),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            toggleCircle.Click += toggleDarkMode_Click;
            toggleBackground.Click += toggleDarkMode_Click;

            toggleBackground.Controls.Add(toggleCircle);
            panelBottom.Controls.Add(labelDarkMode);
            panelBottom.Controls.Add(toggleBackground);

            this.Controls.Add(panelBottom);

            buttonLogin = new Button()
            {
                Text = "Login",
                Location = new Point(20, 20),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(66, 103, 178),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            buttonLogin.Click += buttonLogin_Click;

            buttonConnectAsDesig = new Button()
            {
                Text = "Connect As Desig",
                Location = new Point(160, 20),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(66, 103, 178),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            buttonConnectAsDesig.Click += buttonConnectAsDesig_Click;

            buttonLogout = new Button()
            {
                Text = "Logout",
                Location = new Point(330, 20),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(66, 103, 178),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Visible = false
            };
            buttonLogout.Click += buttonLogout_Click;

            textBoxAppID = new TextBox()
            {
                Text = "1282715863617766",
                Location = new Point(20, 70),
                Size = new Size(430, 28)
            };

            pictureBoxProfile = new PictureBox()
            {
                Location = new Point(500, 20),
                Size = new Size(130, 130),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };

            buttonGoToMenu = new Button()
            {
                Text = "Go to Menu",
                Location = new Point(20, 120),
                Size = new Size(200, 50),
                BackColor = Color.FromArgb(66, 103, 178),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Visible = false
            };
            buttonGoToMenu.Click += (sender, e) => navigateToMenu();
            tabPageLogin.Controls.Add(buttonGoToMenu);

            tabPageLogin.Controls.AddRange(new Control[]
            {
                buttonLogin, buttonConnectAsDesig, buttonLogout,
                textBoxAppID, pictureBoxProfile
            });

            Button buttonMedia = new Button()
            {
                Text = "Media",
                Location = new Point(20, 20),
                Size = new Size(200, 50),
                BackColor = Color.FromArgb(66, 103, 178),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            buttonMedia.Click += (sender, e) => navigateToFeature("Media");

            Button buttonSelfAnalytics = new Button()
            {
                Text = "Self Analytics",
                Location = new Point(20, 90),
                Size = new Size(200, 50),
                BackColor = Color.FromArgb(66, 103, 178),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            buttonSelfAnalytics.Click += (sender, e) => navigateToFeature("Self Analytics");

            Button buttonTimeline = new Button()
            {
                Text = "Timeline",
                Location = new Point(20, 160),
                Size = new Size(200, 50),
                BackColor = Color.FromArgb(66, 103, 178),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            buttonTimeline.Click += (sender, e) => navigateToFeature("Timeline");

            tabPageMenu.Controls.AddRange(new Control[]
            {
                buttonMedia, buttonSelfAnalytics, buttonTimeline
            });
        }

        private void createAlbumsSection(TabPage parent)
        {
            parent.Controls.Clear();

            Button buttonBack = new Button()
            {
                Text = "Back",
                Location = new Point(20, 20),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(66, 103, 178),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            buttonBack.Click += (sender, e) => navigateToMenu();

            linkAlbums = new LinkLabel()
            {
                Text = "Albums",
                Location = new Point(40, 70),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                LinkColor = Color.FromArgb(66, 103, 178),
                AutoSize = false
            };
            linkAlbums.LinkClicked += linkAlbums_LinkClicked;

            listBoxAlbums = new ListBox()
            {
                Location = new Point(40, 110),
                Size = new Size(250, 200),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            listBoxAlbums.SelectedIndexChanged += listBoxAlbums_SelectedIndexChanged;

            pictureBoxAlbum = new PictureBox()
            {
                Location = new Point(90, 330),
                Size = new Size(170, 170),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            parent.Controls.AddRange(new Control[] { buttonBack, linkAlbums, listBoxAlbums, pictureBoxAlbum });
        }

        private void createPostsSection(TabPage parent)
        {
            parent.Controls.Clear();

            Button buttonBack = new Button()
            {
                Text = "Back",
                Location = new Point(20, 20),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(66, 103, 178),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            buttonBack.Click += (sender, e) => navigateToMenu();

            linkPosts = new LinkLabel()
            {
                Text = "Posts",
                Location = new Point(40, 70),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                LinkColor = Color.FromArgb(66, 103, 178),
                AutoSize = false
            };
            linkPosts.LinkClicked += linkPosts_LinkClicked;

            listBoxPosts = new ListBox()
            {
                Location = new Point(40, 110),
                Size = new Size(250, 200),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            listBoxPosts.SelectedIndexChanged += listBoxPosts_SelectedIndexChanged;

            pictureBoxPost = new PictureBox()
            {
                Location = new Point(90, 330),
                Size = new Size(170, 170),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            parent.Controls.AddRange(new Control[] { buttonBack, linkPosts, listBoxPosts, pictureBoxPost });
        }

        private void createPhotosSection(TabPage parent)
        {
            parent.Controls.Clear();

            Button buttonBack = new Button()
            {
                Text = "Back",
                Location = new Point(20, 20),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(66, 103, 178),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            buttonBack.Click += (sender, e) => navigateToMenu();

            linkPhotos = new LinkLabel()
            {
                Text = "Photos",
                Location = new Point(40, 70),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                LinkColor = Color.FromArgb(66, 103, 178),
                AutoSize = false
            };
            linkPhotos.LinkClicked += linkPhotos_LinkClicked;

            listBoxPhotos = new ListBox()
            {
                Location = new Point(40, 110),
                Size = new Size(250, 200),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            listBoxPhotos.SelectedIndexChanged += listBoxPhotos_SelectedIndexChanged;

            pictureBoxPhoto = new PictureBox()
            {
                Location = new Point(90, 330),
                Size = new Size(170, 170),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            parent.Controls.AddRange(new Control[] { buttonBack, linkPhotos, listBoxPhotos, pictureBoxPhoto });
        }

        private void createPagesSection(TabPage parent)
        {
            parent.Controls.Clear();

            Button buttonBack = new Button()
            {
                Text = "Back",
                Location = new Point(20, 20),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(66, 103, 178),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            buttonBack.Click += (sender, e) => navigateToMenu();

            linkPages = new LinkLabel()
            {
                Text = "Pages",
                Location = new Point(40, 70),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                LinkColor = Color.FromArgb(66, 103, 178),
                AutoSize = false
            };
            linkPages.LinkClicked += linkPages_LinkClicked;

            listBoxPages = new ListBox()
            {
                Location = new Point(40, 110),
                Size = new Size(250, 200),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            listBoxPages.SelectedIndexChanged += listBoxPages_SelectedIndexChanged;

            pictureBoxPage = new PictureBox()
            {
                Location = new Point(90, 330),
                Size = new Size(170, 170),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            parent.Controls.AddRange(new Control[] { buttonBack, linkPages, listBoxPages, pictureBoxPage });
        }
    }
}
