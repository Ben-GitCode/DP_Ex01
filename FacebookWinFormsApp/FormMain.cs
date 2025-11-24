using System;
using System.Drawing;
using System.Windows.Forms;
using FacebookWrapper;
using FacebookWrapper.ObjectModel;

namespace BasicFacebookFeatures
{
    public partial class FormMain : Form
    {
        private LoginResult m_LoginResult;

        public FormMain()
        {
            InitializeComponent();
            FacebookService.s_CollectionLimit = 25;

            this.MinimumSize = new Size(700, 500);
            this.MaximumSize = new Size(700, 500);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (m_LoginResult != null)
            {
                MessageBox.Show("Already logged in.");
                return;
            }

            m_LoginResult = FacebookService.Login(
                textBoxAppID.Text,
                "email",
                "public_profile",
                "user_photos",
                "user_posts",
                "user_likes",
                "user_videos",
                "user_friends"
            );

            if (!string.IsNullOrEmpty(m_LoginResult.ErrorMessage))
            {
                MessageBox.Show("Login failed: " + m_LoginResult.ErrorMessage);
                return;
            }

            afterLogin();
        }

        private void buttonConnectAsDesig_Click(object sender, EventArgs e)
        {
            try
            {
                m_LoginResult = FacebookService.Connect(
                    "EAAUm6cZC4eUEBPZCFs9rJRpwlUmdHcPvU1tUNkIyP37zRZCjSvfdHaW5t3xsOnUL0bEKHL8Snjk6AZC3O32KWEbaItglEnXWQ2zEMXHqsdfdv0ecXNs3hO69juHrZCfRN9FGvfuJZAXhP4Pm57DRRoDeB8De6ZABnfrRflh6zgPwnavpyHS3ZCYX1E6K1QLTHff5sAZDZD"
                );
                afterLogin();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connect failed: " + ex.Message);
            }
        }

        private void afterLogin()
        {
            buttonLogin.Enabled = false;
            buttonConnectAsDesig.Enabled = false;
            buttonLogout.Enabled = true;

            buttonLogin.Text = $"Logged in as {m_LoginResult.LoggedInUser.Name}";
            pictureBoxProfile.ImageLocation = m_LoginResult.LoggedInUser.PictureNormalURL;
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            FacebookService.LogoutWithUI();
            m_LoginResult = null;

            buttonLogin.Enabled = true;
            buttonConnectAsDesig.Enabled = true;
            buttonLogin.Text = "Login";
            buttonLogout.Enabled = false;

            pictureBoxProfile.Image = null;
        }

        private void linkAlbums_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!checkLogin()) return;

            listBoxAlbums.Items.Clear();
            listBoxAlbums.DisplayMember = "Name";

            foreach (Album album in m_LoginResult.LoggedInUser.Albums)
                listBoxAlbums.Items.Add(album);
        }

        private void listBoxAlbums_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxAlbums.SelectedItem is Album album && album.PictureAlbumURL != null)
                pictureBoxAlbum.LoadAsync(album.PictureAlbumURL);
        }

        private void linkPages_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!checkLogin()) return;

            listBoxPages.Items.Clear();
            listBoxPages.DisplayMember = "Name";

            foreach (Page p in m_LoginResult.LoggedInUser.LikedPages)
                listBoxPages.Items.Add(p);
        }

        private void listBoxPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxPages.SelectedItem is Page page && !string.IsNullOrEmpty(page.PictureNormalURL))
                pictureBoxPage.LoadAsync(page.PictureNormalURL);
        }

        private void linkEvents_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!checkLogin()) return;

            listBoxEvents.Items.Clear();
            listBoxEvents.DisplayMember = "Name";

            foreach (Event ev in m_LoginResult.LoggedInUser.Events)
                listBoxEvents.Items.Add(ev);
        }

        private void listBoxEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxEvents.SelectedItem is Event ev && ev.Cover != null && !string.IsNullOrEmpty(ev.Cover.SourceURL))
                pictureBoxEvent.LoadAsync(ev.Cover.SourceURL);
        }

        private void linkGroups_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!checkLogin()) return;

            listBoxGroups.Items.Clear();
            listBoxGroups.DisplayMember = "Name";

            foreach (Group g in m_LoginResult.LoggedInUser.Groups)
                listBoxGroups.Items.Add(g);
        }

        private void listBoxGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxGroups.SelectedItem is Group g && !string.IsNullOrEmpty(g.PictureNormalURL))
                pictureBoxGroup.LoadAsync(g.PictureNormalURL);
        }

        private bool checkLogin()
        {
            if (m_LoginResult == null)
            {
                MessageBox.Show("You must login first.");
                return false;
            }
            return true;
        }
    }
}
