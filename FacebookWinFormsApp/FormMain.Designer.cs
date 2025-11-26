using System.Windows.Forms;
using System.Drawing;

namespace BasicFacebookFeatures
{
    partial class FormMain
    {
        private System.ComponentModel.IContainer components = null;

        // ==== UI CONTROLS ====
        private TabControl tabControl1;
        private TabPage tabPageLogin;
        private TabPage tabPageData;

        private Button buttonLogin;
        private Button buttonLogout;
        private Button buttonConnectAsDesig;

        private TextBox textBoxAppID;
        private PictureBox pictureBoxProfile;

        private LinkLabel linkAlbums;
        private LinkLabel linkPages;
        private LinkLabel linkPosts;
        private LinkLabel linkFriends;

        private ListBox listBoxAlbums;
        private ListBox listBoxPages;
        private ListBox listBoxPosts;
        private ListBox listBoxFriends;

        private PictureBox pictureBoxAlbum;
        private PictureBox pictureBoxGroup;
        private PictureBox pictureBoxPage;
        private PictureBox pictureBoxPost;
        private PictureBox pictureBoxFriend;

        // Toggle Switch
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
            // ==== MAIN FORM ====
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Facebook Features";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = Color.FromArgb(66, 103, 178);

            // ==== TAB CONTROL ====
            tabControl1 = new TabControl()
            {
                Dock = DockStyle.Fill,
                Appearance = TabAppearance.Normal,
                ItemSize = new Size(120, 25)
            };

            tabPageLogin = new TabPage("Login") { BackColor = Color.White };
            tabPageData = new TabPage("Facebook Data") { BackColor = Color.White };

            tabControl1.TabPages.Add(tabPageLogin);
            tabControl1.TabPages.Add(tabPageData);
            this.Controls.Add(tabControl1);

            // ==== BOTTOM PANEL + TOGGLE ====
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

            // ==== LOGIN PAGE ====
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
                Enabled = false
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

            tabPageLogin.Controls.AddRange(new Control[]
            {
                buttonLogin, buttonConnectAsDesig, buttonLogout,
                textBoxAppID, pictureBoxProfile
            });

            // ==== DATA PAGE ====
            createDataSection(tabPageData);
        }

        private void createDataSection(TabPage parent)
        {
            // Albums
            linkAlbums = new LinkLabel()
            {
                Text = "Fetch Albums",
                Location = new Point(20, 20),
                AutoSize = true
            };
            linkAlbums.LinkClicked += linkAlbums_LinkClicked;

            listBoxAlbums = new ListBox()
            {
                Location = new Point(20, 45),
                Size = new Size(180, 120)
            };
            listBoxAlbums.SelectedIndexChanged += listBoxAlbums_SelectedIndexChanged;

            pictureBoxAlbum = new PictureBox()
            {
                Location = new Point(220, 45),
                Size = new Size(120, 120),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            // ==== POSTS ====

            // Link to load posts
            linkPosts = new LinkLabel()
            {
                Text = "Fetch Posts",
                Location = new Point(300, 20),
                AutoSize = true,
                LinkColor = Color.FromArgb(66, 103, 178)
            };
            linkPosts.LinkClicked += linkPosts_LinkClicked;

            // Posts list
            listBoxPosts = new ListBox()
            {
                Location = new Point(300, 45),
                Size = new Size(250, 120)
            };
            listBoxPosts.SelectedIndexChanged += listBoxPosts_SelectedIndexChanged;

            // Post image
            pictureBoxPost = new PictureBox()
            {
                Location = new Point(560, 45),
                Size = new Size(120, 120),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            // Friends
            linkFriends = new LinkLabel()
            {
                Text = "Fetch Friends",
                Location = new Point(20, 200),
                AutoSize = true
            };
            linkFriends.LinkClicked += linkFriends_LinkClicked;

            listBoxFriends = new ListBox()
            {
                Location = new Point(20, 225),
                Size = new Size(180, 120)
            };
            listBoxFriends.SelectedIndexChanged += listBoxFriends_SelectedIndexChanged;

            pictureBoxFriend = new PictureBox()
            {
                Location = new Point(220, 225),
                Size = new Size(120, 120),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            
            // Pages
            linkPages = new LinkLabel()
            {
                Text = "Fetch Liked Pages",
                Location = new Point(380, 380),
                AutoSize = true
            };
            linkPages.LinkClicked += linkPages_LinkClicked;

            listBoxPages = new ListBox()
            {
                Location = new Point(380, 405),
                Size = new Size(180, 120)
            };
            listBoxPages.SelectedIndexChanged += listBoxPages_SelectedIndexChanged;

            pictureBoxPage = new PictureBox()
            {
                Location = new Point(580, 405),
                Size = new Size(120, 120),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            parent.Controls.AddRange(new Control[]
            {
                linkAlbums, listBoxAlbums, pictureBoxAlbum,
                linkPosts, listBoxPosts, pictureBoxPost,
                linkFriends, listBoxFriends, pictureBoxFriend,
                pictureBoxGroup,
                linkPages, listBoxPages, pictureBoxPage
            });
        }
    }
}
