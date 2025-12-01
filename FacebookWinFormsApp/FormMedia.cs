using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using FacebookWrapper;
using FacebookWrapper.ObjectModel;

namespace BasicFacebookFeatures
{
    public partial class FormMedia : Form
    {
        private readonly LoginResult m_LoginResult;
        private bool m_IsDarkMode;

        public FormMedia()
        {
            InitializeComponent();
        }

        public FormMedia(LoginResult i_LoginResult, bool i_IsDarkMode)
            : this()
        {
            m_LoginResult = i_LoginResult;
            m_IsDarkMode = i_IsDarkMode;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            applyDarkMode();

            if(m_LoginResult == null)
            {
                MessageBox.Show(
                    "FormMedia did not receive a LoginResult. Make sure you open it with new FormMedia(m_LoginResult, isDarkMode).");
                return;
            }

            loadAlbums();
            loadPosts();
            loadPhotos();
        }

        public void SetDarkMode(bool i_IsDarkMode)
        {
            m_IsDarkMode = i_IsDarkMode;
            applyDarkMode();
        }

        private void applyDarkMode()
        {
            Color formBack = m_IsDarkMode ? Color.FromArgb(24, 25, 26) : Color.FromArgb(235, 236, 237);
            Color pageBack = m_IsDarkMode ? Color.FromArgb(36, 37, 38) : Color.White;
            Color text = m_IsDarkMode ? Color.White : Color.Black;
            Color listBack = m_IsDarkMode ? Color.FromArgb(24, 25, 26) : Color.White;
            Color listFore = text;

            BackColor = formBack;

            Action<Control> walk = null;
            walk = c =>
                {
                    if(c is TabControl tc)
                    {
                        tc.BackColor = formBack;
                        foreach(TabPage p in tc.TabPages)
                        {
                            p.BackColor = pageBack;
                            walk(p);
                        }

                        return;
                    }

                    if(c is ListBox lb)
                    {
                        lb.BackColor = listBack;
                        lb.ForeColor = listFore;
                    }
                    else if(c is LinkLabel ll)
                    {
                        ll.LinkColor = text;
                        ll.ActiveLinkColor = m_IsDarkMode ? Color.DeepSkyBlue : Color.Blue;
                        ll.VisitedLinkColor = ll.LinkColor;
                        ll.ForeColor = text;
                    }
                    else if(c is Label lbl)
                    {
                        lbl.ForeColor = text;
                    }
                    else if(c is Button btn)
                    {
                        btn.ForeColor = Color.White;
                        if(btn.BackColor == SystemColors.Control || btn.BackColor.A == 0)
                        {
                            btn.BackColor = Color.FromArgb(66, 103, 178);
                        }

                        btn.FlatStyle = FlatStyle.Flat;
                    }

                    foreach(Control child in c.Controls)
                    {
                        walk(child);
                    }
                };

            walk(this);
        }

        private void loadAlbums()
        {
            listBoxAlbums.Items.Clear();
            listBoxAlbums.DisplayMember = "Name";

            if(m_LoginResult?.LoggedInUser?.Albums == null)
            {
                return;
            }

            foreach(Album album in m_LoginResult.LoggedInUser.Albums)
            {
                listBoxAlbums.Items.Add(album);
            }
        }

        private void loadPosts()
        {
            listBoxPosts.Items.Clear();
            listBoxPosts.DisplayMember = "Message";

            if(m_LoginResult?.LoggedInUser?.Posts == null)
            {
                return;
            }

            foreach(Post post in m_LoginResult.LoggedInUser.Posts)
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

            if(m_LoginResult?.LoggedInUser?.PhotosTaggedIn == null)
            {
                return;
            }

            foreach(Photo photo in m_LoginResult.LoggedInUser.PhotosTaggedIn)
            {
                listBoxPhotos.Items.Add(photo);
            }
        }

        private void linkAlbums_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            loadAlbums();
        }

        private void listBoxAlbums_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBoxAlbums.SelectedItem is Album album && album.PictureAlbumURL != null)
            {
                pictureBoxAlbum.LoadAsync(album.PictureAlbumURL);
            }
        }

        private void linkPosts_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            loadPosts();
        }

        private void listBoxPosts_SelectedIndexChanged(object sender, EventArgs e)
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

        private void linkPhotos_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            loadPhotos();
        }

        private void listBoxPhotos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBoxPhotos.SelectedItem is Photo photo && !string.IsNullOrEmpty(photo.PictureNormalURL))
            {
                pictureBoxPhoto.LoadAsync(photo.PictureNormalURL);
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            try
            {
                FormMain main = Application.OpenForms.OfType<FormMain>().FirstOrDefault();
                if(main != null)
                {
                    MethodInfo mi = main.GetType().GetMethod(
                        "navigateToMenu",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                    if(mi != null)
                    {
                        mi.Invoke(main, null);
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

        public void LoadImageIntoPhotoBox(string i_Url)
        {
            if(string.IsNullOrEmpty(i_Url))
            {
                return;
            }

            try
            {
                pictureBoxPhoto.Image = null;
                pictureBoxPhoto.LoadAsync(i_Url);
            }
            catch(Exception)
            {
                try
                {
                    pictureBoxPhoto.Image = null;
                }
                catch
                {
                }
            }
        }
    }
}