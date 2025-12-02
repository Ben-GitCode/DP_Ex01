using System;
using System.Drawing;
using System.Windows.Forms;
using FacebookWrapper;
using FacebookWrapper.ObjectModel;

namespace BasicFacebookFeatures
{
    public partial class FormMain : Form
    {
        private bool m_IsDarkMode;
        private LoginResult m_LoginResult;

        public FormMain()
        {
            InitializeComponent();
            FacebookService.s_CollectionLimit = 25;
            // Apply initial dark mode state (false by default)
            applyDarkMode();
        }

        private void buttonLogin_Click(object i_Sender, EventArgs i_EventArgs)
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
                "user_birthday",
                "user_age_range",
                "user_gender",
                "user_link");

            if (!string.IsNullOrEmpty(m_LoginResult.ErrorMessage))
            {
                MessageBox.Show("Login failed: " + m_LoginResult.ErrorMessage);
                return;
            }

            afterLogin();
        }

        private void buttonConnectAsDesig_Click(object i_Sender, EventArgs i_EventArgs)
        {
            try
            {
                m_LoginResult = FacebookService.Connect(
                    "EAAUm6cZC4eUEBPZCFs9rJRpwlUmdHcPvU1tUNkIyP37zRZCjSvfdHaW5t3xsOnUL0bEKHL8Snjk6AZC3O32KWEbaItglEnXWQ2zEMXHqsdfdv0ecXNs3hO69juHrZCfRN9FGvfuJZAXhP4Pm57DRRoDeB8De6ZABnfrRflh6zgPwnavpyHS3ZCYX1E6K1QLTHff5sAZDZD");
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
            buttonLogout.Visible = true;
            buttonGoToMenu.Enabled = true;
            buttonGoToMenu.Visible = true;

            buttonLogin.Text = $"Logged in as {m_LoginResult.LoggedInUser.Name}";
            pictureBoxProfile.ImageLocation = m_LoginResult.LoggedInUser.PictureNormalURL;
        }

        private void buttonLogout_Click(object i_Sender, EventArgs i_EventArgs)
        {
            FacebookService.LogoutWithUI();
            m_LoginResult = null;

            buttonLogin.Enabled = true;
            buttonConnectAsDesig.Enabled = true;
            buttonLogin.Text = "Login";
            buttonLogout.Enabled = false;
            buttonLogout.Visible = false;
            buttonGoToMenu.Enabled = false;
            buttonGoToMenu.Visible = false;

            pictureBoxProfile.Image = null;
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

        private void navigateToFeature(string i_FeatureName)
        {
            if (m_LoginResult == null)
            {
                MessageBox.Show("Please login first.");
                return;
            }

            Form featureForm = null;

            switch (i_FeatureName)
            {
                case "Media":
                    featureForm = new FormMedia(m_LoginResult, m_IsDarkMode);
                    break;
                case "Self Analytics":
                    featureForm = new FormSelfAnalytics(m_LoginResult, m_IsDarkMode);
                    break;
                case "Timeline":
                    featureForm = new FormTimeline(m_LoginResult, m_IsDarkMode);
                    break;
                default:
                    MessageBox.Show("Feature not found.");
                    return;
            }

            if (featureForm != null)
            {
                Hide();

                featureForm.FormClosed += (s, e) =>
                {
                    try
                    {
                        if (!IsDisposed && IsHandleCreated)
                        {
                            BeginInvoke(new Action(() => navigateToMenu()));
                        }
                    }
                    catch
                    {
                    }
                };

                featureForm.Show();
            }
        }

        private void navigateToMenu()
        {
            Show();

            if (tabControl1.SelectedTab != null && tabControl1.SelectedTab != tabPageLogin
                                                && tabControl1.SelectedTab.Name != "Menu")
            {
                tabControl1.TabPages.Remove(tabControl1.SelectedTab);
            }

            if (tabControl1.TabPages["Menu"] != null)
            {
                tabControl1.SelectedTab = tabControl1.TabPages["Menu"];
            }
        }

        private void linkAlbums_LinkClicked(object i_Sender, LinkLabelLinkClickedEventArgs i_EventArgs)
        {
            if (!checkLogin())
            {
                return;
            }

            listBoxAlbums.Items.Clear();
            listBoxAlbums.DisplayMember = "Name";

            foreach (Album album in m_LoginResult.LoggedInUser.Albums)
            {
                listBoxAlbums.Items.Add(album);
            }
        }

        private void listBoxAlbums_SelectedIndexChanged(object i_Sender, EventArgs i_EventArgs)
        {
            if (listBoxAlbums.SelectedItem is Album album && album.PictureAlbumURL != null)
            {
                pictureBoxAlbum.LoadAsync(album.PictureAlbumURL);
            }
        }

        private void linkPosts_LinkClicked(object i_Sender, LinkLabelLinkClickedEventArgs i_EventArgs)
        {
            if (!checkLogin())
            {
                return;
            }

            listBoxPosts.Items.Clear();
            listBoxPosts.DisplayMember = "Message";

            foreach (Post post in m_LoginResult.LoggedInUser.Posts)
            {
                if (post.Message != null)
                {
                    listBoxPosts.Items.Add(post);
                }
                else if (post.Caption != null)
                {
                    listBoxPosts.Items.Add(post);
                }
                else
                {
                    listBoxPosts.Items.Add("[No text] (Media Post)");
                }
            }
        }

        private void listBoxPosts_SelectedIndexChanged(object i_Sender, EventArgs i_EventArgs)
        {
            if (listBoxPosts.SelectedItem is Post post)
            {
                if (!string.IsNullOrEmpty(post.PictureURL))
                {
                    pictureBoxPost.LoadAsync(post.PictureURL);
                }
                else
                {
                    pictureBoxPost.Image = null;
                    MessageBox.Show("This post does not have an associated image.");
                }
            }
        }

        private void linkPages_LinkClicked(object i_Sender, LinkLabelLinkClickedEventArgs i_EventArgs)
        {
            if (!checkLogin())
            {
                return;
            }

            listBoxPages.Items.Clear();
            listBoxPages.DisplayMember = "Name";

            foreach (Page page in m_LoginResult.LoggedInUser.LikedPages)
            {
                listBoxPages.Items.Add(page);
            }
        }

        private void listBoxPages_SelectedIndexChanged(object i_Sender, EventArgs i_EventArgs)
        {
            if (listBoxPages.SelectedItem is Page page && !string.IsNullOrEmpty(page.PictureNormalURL))
            {
                pictureBoxPage.LoadAsync(page.PictureNormalURL);
            }
        }


        private void linkPhotos_LinkClicked(object i_Sender, LinkLabelLinkClickedEventArgs i_EventArgs)
        {
            if (!checkLogin())
            {
                return;
            }

            listBoxPhotos.Items.Clear();
            listBoxPhotos.DisplayMember = "Name";

            foreach (Photo photo in m_LoginResult.LoggedInUser.PhotosTaggedIn)
            {
                listBoxPhotos.Items.Add(photo);
            }
        }

        private void listBoxPhotos_SelectedIndexChanged(object i_Sender, EventArgs i_EventArgs)
        {
            if (listBoxPhotos.SelectedItem is Photo photo && !string.IsNullOrEmpty(photo.PictureNormalURL))
            {
                pictureBoxPhoto.LoadAsync(photo.PictureNormalURL);
            }
        }

        private void toggleDarkMode_Click(object i_Sender, EventArgs i_EventArgs)
        {
            m_IsDarkMode = !m_IsDarkMode;
            applyDarkMode();

            toggleCircle.Left = m_IsDarkMode ? 26 : 1;

            toggleBackground.BackColor = m_IsDarkMode ? ColorPalette.sr_DarkModeToggleOnBackground : ColorPalette.sr_LightModeToggleOffBackground;
        }

        private void applyDarkMode()
        {
            Color formColor = m_IsDarkMode ? ColorPalette.sr_DarkModeFormBackground : Color.White;
            Color textColor = m_IsDarkMode ? ColorPalette.sr_DarkModeTextColor : ColorPalette.sr_LightModeTextColor;
            Color listBoxBackColor = m_IsDarkMode ? ColorPalette.sr_DarkModeListBoxBackground : ColorPalette.sr_LightModeListBoxBackground;
            Color listBoxForeColor = m_IsDarkMode ? ColorPalette.sr_DarkModeTextColor : ColorPalette.sr_LightModeTextColor;

            tabPageLogin.BackColor = formColor;

            // Update main form and bottom panel colors
            this.BackColor = m_IsDarkMode ? ColorPalette.sr_DarkModeFormBackground : ColorPalette.sr_FacebookBlue;
            panelBottom.BackColor = m_IsDarkMode ? ColorPalette.sr_DarkModePanelBackground : ColorPalette.sr_FacebookBlue;
            labelDarkMode.ForeColor = textColor;

            // Set toggle colors
            toggleBackground.BackColor = m_IsDarkMode ? ColorPalette.sr_DarkModeToggleOnBackground : ColorPalette.sr_LightModeToggleOffBackground;
            toggleCircle.BackColor = ColorPalette.sr_DarkModeToggleCircle;
            toggleCircle.Left = m_IsDarkMode ? 26 : 1;

            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                tabPage.BackColor = formColor;

                foreach (Control control in tabPage.Controls)
                {
                    applyColor(control, textColor, listBoxBackColor, listBoxForeColor);
                }
            }
        }

        private void applyColor(
            Control i_Control,
            Color i_TextColor,
            Color i_ListBoxBackColor,
            Color i_ListBoxForeColor)
        {
            if (i_Control is Button button)
            {
                button.BackColor = m_IsDarkMode ? ColorPalette.sr_DarkModeButtonBackground : ColorPalette.sr_FacebookBlue;
                button.ForeColor = Color.White;
            }
            else if (i_Control is LinkLabel linkLabel)
            {
                linkLabel.LinkColor = i_TextColor;
                linkLabel.ForeColor = i_TextColor;
            }
            else if (i_Control is ListBox listBox)
            {
                listBox.BackColor = i_ListBoxBackColor;
                listBox.ForeColor = i_ListBoxForeColor;
            }
            else
            {
                i_Control.ForeColor = i_TextColor;
            }
        }
    }
}