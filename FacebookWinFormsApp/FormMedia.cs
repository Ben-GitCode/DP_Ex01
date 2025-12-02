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
        private readonly bool r_IsDarkMode;
        private readonly LoginResult r_LoginResult;

        public FormMedia()
        {
            InitializeComponent();
        }

        public FormMedia(LoginResult i_LoginResult, bool i_IsDarkMode)
            : this()
        {
            r_LoginResult = i_LoginResult;
            r_IsDarkMode = i_IsDarkMode;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            applyDarkMode();

            if(r_LoginResult == null)
            {
                MessageBox.Show(
                    "FormMedia did not receive a LoginResult. Make sure you open it with new FormMedia(r_LoginResult, isDarkMode).");
                return;
            }

            loadAlbums();
            loadPosts();
            loadPhotos();
        }


        private void applyDarkMode()
        {
            Color formBack = r_IsDarkMode ? Color.FromArgb(24, 25, 26) : Color.FromArgb(235, 236, 237);
            Color pageBack = r_IsDarkMode ? Color.FromArgb(36, 37, 38) : Color.White;
            Color text = r_IsDarkMode ? Color.White : Color.Black;
            Color listBack = r_IsDarkMode ? Color.FromArgb(24, 25, 26) : Color.White;
            Color listFore = text;

            BackColor = formBack;

            Action<Control> walk = null;
            walk = i_Control =>
                {
                    if(i_Control is TabControl tc)
                    {
                        tc.BackColor = formBack;
                        foreach(TabPage p in tc.TabPages)
                        {
                            p.BackColor = pageBack;
                            walk(p);
                        }

                        return;
                    }

                    if(i_Control is ListBox listBox)
                    {
                        listBox.BackColor = listBack;
                        listBox.ForeColor = listFore;
                    }
                    else if(i_Control is LinkLabel linkLabel)
                    {
                        linkLabel.LinkColor = text;
                        linkLabel.ActiveLinkColor = r_IsDarkMode ? Color.DeepSkyBlue : Color.Blue;
                        linkLabel.VisitedLinkColor = linkLabel.LinkColor;
                        linkLabel.ForeColor = text;
                    }
                    else if(i_Control is Label label)
                    {
                        label.ForeColor = text;
                    }
                    else if(i_Control is Button button)
                    {
                        button.ForeColor = Color.White;
                        if(button.BackColor == SystemColors.Control || button.BackColor.A == 0)
                        {
                            button.BackColor = Color.FromArgb(66, 103, 178);
                        }

                        button.FlatStyle = FlatStyle.Flat;
                    }

                    foreach(Control child in i_Control.Controls)
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