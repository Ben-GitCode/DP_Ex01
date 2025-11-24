using System.Windows.Forms;

namespace BasicFacebookFeatures
{
    partial class FormMain
    {
        private System.ComponentModel.IContainer components = null;

        private TabControl tabControl1;
        private TabPage tabPageLogin;
        private TabPage tabPageEmpty; // Extra empty tab

        private Button buttonLogin;
        private Button buttonLogout;
        private Button buttonConnectAsDesig;
        private TextBox textBoxAppID;
        private PictureBox pictureBoxProfile;

        private LinkLabel linkAlbums;
        private LinkLabel linkEvents;
        private LinkLabel linkGroups;
        private LinkLabel linkPages;

        private ListBox listBoxAlbums;
        private ListBox listBoxEvents;
        private ListBox listBoxGroups;
        private ListBox listBoxPages;

        private PictureBox pictureBoxAlbum;
        private PictureBox pictureBoxEvent;
        private PictureBox pictureBoxGroup;
        private PictureBox pictureBoxPage;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.ClientSize = new System.Drawing.Size(700, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Facebook Features";

            // Tab Control
            tabControl1 = new TabControl
            {
                Dock = DockStyle.Fill
            };

            tabPageLogin = new TabPage("Facebook Data + Login");
            tabPageEmpty = new TabPage("Empty Tab"); // Added empty tab

            tabControl1.TabPages.Add(tabPageLogin);
            tabControl1.TabPages.Add(tabPageEmpty); // Add to TabControl
            this.Controls.Add(tabControl1);

            // --- LOGIN CONTROLS ---
            buttonLogin = new Button()
            {
                Text = "Login",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(120, 30)
            };
            buttonLogin.Click += buttonLogin_Click;

            buttonConnectAsDesig = new Button()
            {
                Text = "Connect As Desig",
                Location = new System.Drawing.Point(150, 20),
                Size = new System.Drawing.Size(120, 30)
            };
            buttonConnectAsDesig.Click += buttonConnectAsDesig_Click;

            buttonLogout = new Button()
            {
                Text = "Logout",
                Location = new System.Drawing.Point(280, 20),
                Size = new System.Drawing.Size(120, 30),
                Enabled = false
            };
            buttonLogout.Click += buttonLogout_Click;

            textBoxAppID = new TextBox()
            {
                Text = "1282715863617766",
                Location = new System.Drawing.Point(20, 60),
                Size = new System.Drawing.Size(380, 25)
            };

            pictureBoxProfile = new PictureBox()
            {
                Location = new System.Drawing.Point(420, 20),
                Size = new System.Drawing.Size(100, 100),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };

            tabPageLogin.Controls.Add(buttonLogin);
            tabPageLogin.Controls.Add(buttonConnectAsDesig);
            tabPageLogin.Controls.Add(buttonLogout);
            tabPageLogin.Controls.Add(textBoxAppID);
            tabPageLogin.Controls.Add(pictureBoxProfile);

            // --- DATA CONTROLS ---
            // Albums
            linkAlbums = new LinkLabel()
            {
                Text = "Albums",
                Location = new System.Drawing.Point(20, 140),
                AutoSize = true
            };
            linkAlbums.LinkClicked += linkAlbums_LinkClicked;

            listBoxAlbums = new ListBox()
            {
                Location = new System.Drawing.Point(20, 160),
                Size = new System.Drawing.Size(150, 100)
            };
            listBoxAlbums.SelectedIndexChanged += listBoxAlbums_SelectedIndexChanged;

            pictureBoxAlbum = new PictureBox()
            {
                Location = new System.Drawing.Point(180, 160),
                Size = new System.Drawing.Size(100, 100),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Events
            linkEvents = new LinkLabel()
            {
                Text = "Events",
                Location = new System.Drawing.Point(300, 140),
                AutoSize = true
            };
            linkEvents.LinkClicked += linkEvents_LinkClicked;

            listBoxEvents = new ListBox()
            {
                Location = new System.Drawing.Point(300, 160),
                Size = new System.Drawing.Size(150, 100)
            };
            listBoxEvents.SelectedIndexChanged += listBoxEvents_SelectedIndexChanged;

            pictureBoxEvent = new PictureBox()
            {
                Location = new System.Drawing.Point(460, 160),
                Size = new System.Drawing.Size(100, 100),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Groups
            linkGroups = new LinkLabel()
            {
                Text = "Groups",
                Location = new System.Drawing.Point(20, 280),
                AutoSize = true
            };
            linkGroups.LinkClicked += linkGroups_LinkClicked;

            listBoxGroups = new ListBox()
            {
                Location = new System.Drawing.Point(20, 300),
                Size = new System.Drawing.Size(150, 100)
            };
            listBoxGroups.SelectedIndexChanged += listBoxGroups_SelectedIndexChanged;

            pictureBoxGroup = new PictureBox()
            {
                Location = new System.Drawing.Point(180, 300),
                Size = new System.Drawing.Size(100, 100),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Pages
            linkPages = new LinkLabel()
            {
                Text = "Liked Pages",
                Location = new System.Drawing.Point(300, 280),
                AutoSize = true
            };
            linkPages.LinkClicked += linkPages_LinkClicked;

            listBoxPages = new ListBox()
            {
                Location = new System.Drawing.Point(300, 300),
                Size = new System.Drawing.Size(150, 100)
            };
            listBoxPages.SelectedIndexChanged += listBoxPages_SelectedIndexChanged;

            pictureBoxPage = new PictureBox()
            {
                Location = new System.Drawing.Point(460, 300),
                Size = new System.Drawing.Size(100, 100),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };

            tabPageLogin.Controls.AddRange(new Control[]
            {
                linkAlbums, listBoxAlbums, pictureBoxAlbum,
                linkEvents, listBoxEvents, pictureBoxEvent,
                linkGroups, listBoxGroups, pictureBoxGroup,
                linkPages, listBoxPages, pictureBoxPage
            });
        }
    }
}
