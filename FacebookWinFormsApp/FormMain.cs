using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FacebookWrapper;
using FacebookWrapper.ObjectModel;

namespace BasicFacebookFeatures
{
    public partial class FormMain : Form
    {
        private bool m_IsDarkMode;
        private LoginResult m_LoginResult;
        private UiPalette m_Palette;

        public FormMain()
        {
            InitializeComponent();
            FacebookService.s_CollectionLimit = 25;
            applyDarkMode();
        }

        private void buttonLogin_Click(object i_Sender, EventArgs i_EventArgs)
        {
            if(m_LoginResult != null)
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

            if(!string.IsNullOrEmpty(m_LoginResult.ErrorMessage))
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
            catch(Exception ex)
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
            if(m_LoginResult == null)
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
                    featureForm = new FormMedia(m_LoginResult, m_Palette);
                    break;
                case "Self Analytics":
                    featureForm = new FormSelfAnalytics(m_LoginResult, m_Palette);
                    break;
                case "Timeline":
                    featureForm = new FormTimeline(m_LoginResult, m_Palette);
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

            if(tabControl1.SelectedTab != null && tabControl1.SelectedTab != tabPageLogin
                                               && tabControl1.SelectedTab.Name != "Menu")
            {
                tabControl1.TabPages.Remove(tabControl1.SelectedTab);
            }

            if(tabControl1.TabPages["Menu"] != null)
            {
                tabControl1.SelectedTab = tabControl1.TabPages["Menu"];
            }
        }

        private void linkAlbums_LinkClicked(object i_Sender, LinkLabelLinkClickedEventArgs i_EventArgs)
        {
            if(!checkLogin())
            {
                return;
            }

            listBoxAlbums.Items.Clear();
            listBoxAlbums.DisplayMember = "Name";

            foreach(Album album in m_LoginResult.LoggedInUser.Albums)
            {
                listBoxAlbums.Items.Add(album);
            }
        }

        private void listBoxAlbums_SelectedIndexChanged(object i_Sender, EventArgs i_EventArgs)
        {
            if(listBoxAlbums.SelectedItem is Album album && album.PictureAlbumURL != null)
            {
                pictureBoxAlbum.LoadAsync(album.PictureAlbumURL);
            }
        }

        private void linkPosts_LinkClicked(object i_Sender, LinkLabelLinkClickedEventArgs i_EventArgs)
        {
            if(!checkLogin())
            {
                return;
            }

            listBoxPosts.Items.Clear();
            listBoxPosts.DisplayMember = "Message";

            foreach(Post post in m_LoginResult.LoggedInUser.Posts)
            {
                if(post.Message != null)
                {
                    listBoxPosts.Items.Add(post);
                }
                else if(post.Caption != null)
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
            if(listBoxPosts.SelectedItem is Post post)
            {
                if(!string.IsNullOrEmpty(post.PictureURL))
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
            if(!checkLogin())
            {
                return;
            }

            listBoxPages.Items.Clear();
            listBoxPages.DisplayMember = "Name";

            foreach(Page page in m_LoginResult.LoggedInUser.LikedPages)
            {
                listBoxPages.Items.Add(page);
            }
        }

        private void listBoxPages_SelectedIndexChanged(object i_Sender, EventArgs i_EventArgs)
        {
            if(listBoxPages.SelectedItem is Page page && !string.IsNullOrEmpty(page.PictureNormalURL))
            {
                pictureBoxPage.LoadAsync(page.PictureNormalURL);
            }
        }


        private void linkPhotos_LinkClicked(object i_Sender, LinkLabelLinkClickedEventArgs i_EventArgs)
        {
            if(!checkLogin())
            {
                return;
            }

            listBoxPhotos.Items.Clear();
            listBoxPhotos.DisplayMember = "Name";

            foreach(Photo photo in m_LoginResult.LoggedInUser.PhotosTaggedIn)
            {
                listBoxPhotos.Items.Add(photo);
            }
        }

        private void listBoxPhotos_SelectedIndexChanged(object i_Sender, EventArgs i_EventArgs)
        {
            if(listBoxPhotos.SelectedItem is Photo photo && !string.IsNullOrEmpty(photo.PictureNormalURL))
            {
                pictureBoxPhoto.LoadAsync(photo.PictureNormalURL);
            }
        }

        private void toggleDarkMode_Click(object i_Sender, EventArgs i_EventArgs)
        {
            m_IsDarkMode = !m_IsDarkMode;
            applyDarkMode();
            toggleCircle.Left = m_IsDarkMode ? 26 : 1;
            toggleBackground.BackColor = m_IsDarkMode ? ColorPalette.sr_LightGray : ColorPalette.sr_LightGray;
        }

        private UiPalette buildPalette(bool i_IsDark)
        {
            Color formColor = m_IsDarkMode ? ColorPalette.sr_Black : ColorPalette.sr_White;
            Color textColor = m_IsDarkMode ? ColorPalette.sr_WhitishBlue : ColorPalette.sr_DarkBlue;
            Color listBoxBackColor = m_IsDarkMode ? ColorPalette.sr_DarkGray : ColorPalette.sr_WhitishBlue;
            Color listBoxForeColor = m_IsDarkMode ? ColorPalette.sr_WhitishBlue : ColorPalette.sr_DarkBlue;

            tabPageLogin.BackColor = formColor;

            BackColor = m_IsDarkMode ? ColorPalette.sr_Black : ColorPalette.sr_FacebookBlue;
            panelBottom.BackColor = m_IsDarkMode ? ColorPalette.sr_DarkGray : ColorPalette.sr_FacebookBlue;
            labelDarkMode.ForeColor = textColor;

            toggleBackground.BackColor = ColorPalette.sr_LightGray;
            toggleCircle.BackColor = ColorPalette.sr_White;
            toggleCircle.Left = m_IsDarkMode ? 26 : 1;

            foreach(TabPage tabPage in tabControl1.TabPages)
            {
                palette= new UiPalette
                {
                    FormBack = Color.FromArgb(24, 25, 26),
                    PanelBack = Color.FromArgb(36, 37, 38),
                    HeaderBack = Color.FromArgb(45, 60, 100),
                    PrimaryText = Color.White,
                    SecondaryText = Color.Gainsboro,
                    MutedText = Color.Silver,

                    ListBack = Color.FromArgb(24, 25, 26),
                    ListFore = Color.White,
                    ButtonBack = Color.FromArgb(66, 103, 178),
                    PlaceholderText = Color.Gainsboro,
                    PreviewImageBack = Color.Black,
                    StatsText = Color.Gainsboro,
                    ProfileBack = Color.FromArgb(36, 37, 38),

                    CardOuterStart = Color.FromArgb(30, 40, 60),
                    CardOuterEnd = Color.FromArgb(16, 22, 33),
                    CardInnerTop = Color.FromArgb(44, 47, 51),
                    CardInnerBottom = Color.FromArgb(36, 37, 38),
                    CardInnerBorder = Color.FromArgb(80, 80, 80),
                    CardShineStart = Color.FromArgb(30, 255, 255, 255),
                    CardShineEnd = Color.FromArgb(5, 255, 255, 255)
                };
            }
            else
            {   

                palette = new UiPalette
                {
                    FormBack = SystemColors.Control,
                    PanelBack = SystemColors.Control,
                    HeaderBack = Color.FromArgb(59, 89, 152),
                    PrimaryText = Color.FromArgb(12, 36, 86),
                    SecondaryText = Color.FromArgb(34, 34, 34),
                    MutedText = Color.FromArgb(90, 90, 110),

                    ListBack = Color.White,
                    ListFore = Color.Black,
                    ButtonBack = Color.FromArgb(66, 103, 178),
                    PlaceholderText = Color.DimGray,
                    PreviewImageBack = SystemColors.ControlDark,
                    StatsText = Color.FromArgb(40, 40, 40),
                    ProfileBack = Color.White,

                    CardOuterStart = Color.FromArgb(40, 83, 155),
                    CardOuterEnd = Color.FromArgb(14, 36, 86),
                    CardInnerTop = Color.FromArgb(255, 255, 255, 255),
                    CardInnerBottom = Color.FromArgb(240, 240, 246),
                    CardInnerBorder = Color.FromArgb(200, 200, 200),
                    CardShineStart = Color.FromArgb(60, 255, 255, 255),
                    CardShineEnd = Color.FromArgb(10, 255, 255, 255)
                };
            }

            return palette;
        }

        private void applyDarkMode()
        {
            if(i_Control is Button button)
            {
                button.BackColor = m_IsDarkMode ? ColorPalette.sr_DarkGray : ColorPalette.sr_FacebookBlue;
                button.ForeColor = ColorPalette.sr_White;
            }
            else if(i_Control is LinkLabel linkLabel)
            {
                toggleBackground.BackColor = m_IsDarkMode ? Color.DarkGray : Color.LightGray;
            }

            if(pictureBoxProfile != null && pictureBoxProfile.Image == null)
            {
                pictureBoxProfile.BackColor = m_Palette.ProfileBack;
            }
        }

        private void applyPaletteToControls(Control.ControlCollection i_Controls)
        {
            foreach(Control control in i_Controls)
            {
                if(control is Panel)
                {
                    control.BackColor = m_Palette.PanelBack;
                    control.ForeColor = m_Palette.PrimaryText;
                }
                else if(control is TabControl || control is TabPage)
                {
                    control.BackColor = m_Palette.FormBack;
                    control.ForeColor = m_Palette.PrimaryText;
                }
                else if(control is GroupBox)
                {
                    control.BackColor = m_Palette.PanelBack;
                    control.ForeColor = m_Palette.PrimaryText;
                }
                else if(control is Label)
                {
                    control.ForeColor = m_Palette.PrimaryText;
                }
                else if(control is LinkLabel link)
                {
                    link.BackColor = Color.Transparent;
                    link.ForeColor = m_Palette.PrimaryText;
                    link.LinkColor = m_Palette.ButtonBack;
                    link.ActiveLinkColor = m_Palette.ButtonBack;
                    link.VisitedLinkColor = m_Palette.ButtonBack;
                }
                else if(control is Button btn)
                {
                    btn.BackColor = m_Palette.ButtonBack;
                    btn.ForeColor = Color.White;
                }
                else if(control is ListBox listBox)
                {
                    listBox.BackColor = m_Palette.ListBack;
                    listBox.ForeColor = m_Palette.ListFore;
                }
                else if(control is ListView listView)
                {
                    listView.BackColor = m_Palette.ListBack;
                    listView.ForeColor = m_Palette.ListFore;
                }
                else if(control is TextBox textBox)
                {
                    textBox.BackColor = m_Palette.PanelBack;
                    textBox.ForeColor = m_Palette.PrimaryText;
                }
                else if(control is PictureBox pictureBox)
                {
                    pictureBox.BackColor = m_Palette.PreviewImageBack;
                }
                else
                {
                    if(control.BackColor == SystemColors.Control)
                    {
                        control.BackColor = m_Palette.PanelBack;
                    }

                    control.ForeColor = m_Palette.PrimaryText;
                }

                if(control.HasChildren)
                {
                    applyPaletteToControls(control.Controls);
                }
            }
        }
    }
}