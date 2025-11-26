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
        private bool isDarkMode = false;

        public FormMain()
        {
            InitializeComponent();
            FacebookService.s_CollectionLimit = 25;

            
            listBoxFriends.SelectedIndexChanged += listBoxFriends_SelectedIndexChanged;

            pictureBoxFriend = new PictureBox()
            {
                Location = new Point(220, 225),
                Size = new Size(120, 120),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            // Add controls to the tabPageData
            tabPageData.Controls.AddRange(new Control[]
            {
                linkFriends, listBoxFriends, pictureBoxFriend
            });
        }

        // ---------------- LOGIN ----------------
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

        private bool checkLogin()
        {
            if (m_LoginResult == null)
            {
                MessageBox.Show("You must login first.");
                return false;
            }

            return true;
        }

        // ---------------- ALBUMS ----------------
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

        private void linkPosts_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!checkLogin()) return;

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

        private void listBoxPosts_SelectedIndexChanged(object sender, EventArgs e)
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

        // ---------------- PAGES ----------------
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
            if (listBoxPages.SelectedItem is Page p &&
                !string.IsNullOrEmpty(p.PictureNormalURL))
            {
                pictureBoxPage.LoadAsync(p.PictureNormalURL);
            }
        }

        // ---------------- FRIENDS ----------------
        private void linkFriends_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!checkLogin()) return;

            listBoxFriends.Items.Clear();
            listBoxFriends.DisplayMember = "Name";

            foreach (User friend in m_LoginResult.LoggedInUser.Friends)
                listBoxFriends.Items.Add(friend);
        }

        private void listBoxFriends_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxFriends.SelectedItem is User friend &&
                !string.IsNullOrEmpty(friend.PictureNormalURL))
            {
                pictureBoxFriend.LoadAsync(friend.PictureNormalURL);
            }
        }

        // ---------------- DARK MODE ----------------
        private void toggleDarkMode_Click(object sender, EventArgs e)
        {
            isDarkMode = !isDarkMode;
            applyDarkMode();

            // Move toggle circle
            toggleCircle.Left = isDarkMode ? 26 : 1;

            // Change background color
            toggleBackground.BackColor = isDarkMode ? Color.DarkGray : Color.LightGray;
        }

        private void applyDarkMode()
        {
            Color formColor = isDarkMode ? Color.Black : Color.White;
            Color textColor = isDarkMode ? Color.White : Color.Black;
            Color listBoxBackColor = isDarkMode ? Color.Black : Color.White;
            Color listBoxForeColor = isDarkMode ? Color.White : Color.Black;

            tabPageLogin.BackColor = formColor;
            tabPageData.BackColor = formColor;

            foreach (Control control in tabPageLogin.Controls)
                applyColor(control, textColor, listBoxBackColor, listBoxForeColor);

            foreach (Control control in tabPageData.Controls)
                applyColor(control, textColor, listBoxBackColor, listBoxForeColor);
        }

        private void applyColor(Control control, Color textColor, Color listBoxBackColor, Color listBoxForeColor)
        {
            if (control is Button b)
            {
                b.BackColor = isDarkMode ? Color.FromArgb(50, 50, 50) : Color.FromArgb(66, 103, 178);
                b.ForeColor = Color.White;
            }
            else if (control is LinkLabel ll)
            {
                ll.LinkColor = textColor;
                ll.ForeColor = textColor;
            }
            else if (control is ListBox lb)
            {
                lb.BackColor = listBoxBackColor;
                lb.ForeColor = listBoxForeColor;
            }
            else
            {
                control.ForeColor = textColor;
            }
        }
    }
}
