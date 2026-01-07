using System;
using System.Linq;
using System.Windows.Forms;
using FacebookWrapper;
using FacebookWrapper.ObjectModel;
using System.Drawing;
using System.Reflection;

namespace BasicFacebookFeatures
{
    public partial class FormMedia : Form
    {
        private readonly LoginResult r_LoginResult;
        private UiPalette m_Palette;

        public FormMedia()
        {
            InitializeComponent();
        }

        public FormMedia(LoginResult i_LoginResult, UiPalette i_Palette) : this()
        {
            r_LoginResult = i_LoginResult;
            m_Palette = i_Palette;
        }

        public FormMedia(LoginResult i_LoginResult, bool i_IsDarkMode) : this(i_LoginResult, null) { }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            applyPalette();

            if (r_LoginResult == null)
            {
                MessageBox.Show("FormMedia did not receive a LoginResult. Make sure you open it with new FormMedia(m_LoginResult, palette).");
                return;
            }

            loadAlbums();
            loadPosts();
            loadPhotos();
        }

        public void SetPalette(UiPalette i_Palette)
        {
            m_Palette = i_Palette ?? m_Palette;
            applyPalette();
        }

        private void applyPalette()
        {
            Color formBack = r_IsDarkMode ? ColorPalette.sr_Black : ColorPalette.sr_White;
            Color pageBack = r_IsDarkMode ? ColorPalette.sr_DarkGray : ColorPalette.sr_White;
            Color text = r_IsDarkMode ? ColorPalette.sr_WhitishBlue : ColorPalette.sr_DarkBlue;
            Color listBack = r_IsDarkMode ? ColorPalette.sr_DarkGray : ColorPalette.sr_White;
            Color listFore = text;
            Color buttonBackColor = ColorPalette.sr_FacebookBlue;

            BackColor = p.FormBack;

            listBoxAlbums.BackColor = p.ListBack;
            listBoxAlbums.ForeColor = p.ListFore;

            listBoxPosts.BackColor = p.ListBack;
            listBoxPosts.ForeColor = p.ListFore;

                    if(i_Control is ListBox listBox)
                    {
                        listBox.BackColor = listBack;
                        listBox.ForeColor = listFore;
                    }
                    else if(i_Control is LinkLabel linkLabel)
                    {
                        linkLabel.LinkColor = text;
                        linkLabel.ActiveLinkColor =
                            r_IsDarkMode ? ColorPalette.sr_WhitishBlue : ColorPalette.sr_DarkBlue;
                        linkLabel.VisitedLinkColor = linkLabel.LinkColor;
                        linkLabel.ForeColor = text;
                    }
                    else if(i_Control is Label label)
                    {
                        label.ForeColor = text;
                    }
                    else if(i_Control is Button button)
                    {
                        button.ForeColor = ColorPalette.sr_White;
                        if(button.BackColor == SystemColors.Control || button.BackColor.A == 0)
                        {
                            button.BackColor = buttonBackColor;
                        }

            pictureBoxAlbum.BackColor = p.PreviewImageBack;
            pictureBoxPost.BackColor = p.PreviewImageBack;
            pictureBoxPhoto.BackColor = p.PreviewImageBack;

            linkAlbums.LinkColor = p.PrimaryText;
            linkPosts.LinkColor = p.PrimaryText;
            linkPhotos.LinkColor = p.PrimaryText;

            Panel headerPanel = Controls.OfType<Panel>().FirstOrDefault(p => p.Height == 72);
            if(headerPanel != null)
            {
                headerPanel.BackColor = ColorPalette.sr_FacebookBlue;

                Label headerSub = headerPanel.Controls.OfType<Label>().FirstOrDefault(l => l.Text.Contains("•"));
                if(headerSub != null)
                {
                    headerSub.ForeColor = ColorPalette.sr_LightGray;
                }
            }

            walk(this);
        }

        private void loadAlbums()
        {
            listBoxAlbums.Items.Clear();
            listBoxAlbums.DisplayMember = "Name";

            if(r_LoginResult?.LoggedInUser?.Albums == null)
            {
                return;
            }

            foreach(Album album in r_LoginResult.LoggedInUser.Albums)
            {
                listBoxAlbums.Items.Add(album);
            }
        }

        private void loadPosts()
        {
            listBoxPosts.Items.Clear();
            listBoxPosts.DisplayMember = "Message";

            if(r_LoginResult?.LoggedInUser?.Posts == null)
            {
                return;
            }

            foreach(Post post in r_LoginResult.LoggedInUser.Posts)
            {
                if(!string.IsNullOrEmpty(post.Message))
                {
                    listBoxPosts.Items.Add(post);
                }
                else if(!string.IsNullOrEmpty(post.Caption))
                {
                    listBoxPosts.Items.Add(post);
                }
                else
                {
                    listBoxPosts.Items.Add("[No text] (Media Post)");
                }
            }
        }

        private void loadPhotos()
        {
            listBoxPhotos.Items.Clear();
            listBoxPhotos.DisplayMember = "Name";

            if(r_LoginResult?.LoggedInUser?.PhotosTaggedIn == null)
            {
                return;
            }

            foreach(Photo photo in r_LoginResult.LoggedInUser.PhotosTaggedIn)
            {
                listBoxPhotos.Items.Add(photo);
            }
        }

        private void linkAlbums_LinkClicked(object i_Sender, LinkLabelLinkClickedEventArgs i_EventArgs)
        {
            loadAlbums();
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
            loadPosts();
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
                }
            }
        }

        private void linkPhotos_LinkClicked(object i_Sender, LinkLabelLinkClickedEventArgs i_EventArgs)
        {
            loadPhotos();
        }

        private void listBoxPhotos_SelectedIndexChanged(object i_Sender, EventArgs i_EventArgs)
        {
            if(listBoxPhotos.SelectedItem is Photo photo && !string.IsNullOrEmpty(photo.PictureNormalURL))
            {
                pictureBoxPhoto.LoadAsync(photo.PictureNormalURL);
            }
        }

        private void buttonBack_Click(object i_Sender, EventArgs i_EventArgs)
        {
            try
            {
                FormMain main = Application.OpenForms.OfType<FormMain>().FirstOrDefault();
                if(main != null)
                {
                    MethodInfo methodInfo = main.GetType().GetMethod(
                        "navigateToMenu",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                    if(methodInfo != null)
                    {
                        methodInfo.Invoke(main, null);
                        Close();
                        return;
                    }

                    main.Invoke(new Action(() => main.BringToFront()));
                }
            }
            catch
            {
            }

            Close();
        }
    }
}