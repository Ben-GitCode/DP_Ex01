using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using FacebookWrapper;
using FacebookWrapper.ObjectModel;

namespace BasicFacebookFeatures
{
    public partial class FormTimeline : Form
    {
        private bool m_IsDarkMode;
        private LoginResult m_LoginResult;

        public FormTimeline(LoginResult loginResult)
        {
            InitializeComponent();
            m_LoginResult = loginResult;
            Load += FormTimeline_Load;
            if(listViewTimeline != null)
            {
                listViewTimeline.SelectedIndexChanged += ListViewTimeline_SelectedIndexChanged;
                listViewTimeline.DoubleClick += ListViewTimeline_DoubleClick;
            }
        }

        public FormTimeline(LoginResult loginResult, bool isDarkMode)
            : this(loginResult)
        {
            m_IsDarkMode = isDarkMode;
        }

        public FormTimeline()
        {
            InitializeComponent();
            Load += FormTimeline_Load;
            if(listViewTimeline != null)
            {
                listViewTimeline.SelectedIndexChanged += ListViewTimeline_SelectedIndexChanged;
                listViewTimeline.DoubleClick += ListViewTimeline_DoubleClick;
            }
        }

        public void SetLoginResult(LoginResult loginResult)
        {
            m_LoginResult = loginResult;
            if(IsHandleCreated && Visible)
            {
                populateTimeline();
            }
        }

        public void SetDarkMode(bool isDarkMode)
        {
            m_IsDarkMode = isDarkMode;
            applyDarkMode();
        }

        private void FormTimeline_Load(object sender, EventArgs e)
        {
            applyDarkMode();
            AdjustColumns();
            if(m_LoginResult != null && m_LoginResult.LoggedInUser != null)
            {
                populateTimeline();
            }
        }

        private void applyDarkMode()
        {
            Color formBack = m_IsDarkMode ? Color.FromArgb(24, 25, 26) : SystemColors.Control;
            Color panelBack = m_IsDarkMode ? Color.FromArgb(36, 37, 38) : SystemColors.Control;
            Color text = m_IsDarkMode ? Color.White : Color.Black;
            Color listBack = m_IsDarkMode ? Color.FromArgb(24, 25, 26) : Color.White;
            Color listFore = text;
            Color headerBack = m_IsDarkMode ? Color.FromArgb(45, 60, 100) : Color.FromArgb(59, 89, 152);
            Color buttonBack = Color.FromArgb(66, 103, 178);

            BackColor = formBack;

            applyHeaderTheme(headerBack, text);
            applyPanelsTheme(panelBack, text);
            applyListTheme(listBack, listFore);
            applyPreviewTheme(panelBack);
            applyButtonsTheme(buttonBack);
            applyCombosTheme(listBack, listFore);
        }

        private void applyHeaderTheme(Color headerBack, Color text)
        {
            if(topPanel == null)
            {
                return;
            }

            topPanel.BackColor = headerBack;

            foreach(Control c in topPanel.Controls)
            {
                if(c is Label || c is LinkLabel)
                {
                    c.ForeColor = Color.White;
                }
                else
                {
                    c.ForeColor = text;
                }
            }
        }

        private void applyPanelsTheme(Color panelBack, Color text)
        {
            if(leftPanel != null)
            {
                leftPanel.BackColor = panelBack;
                leftPanel.ForeColor = text;
            }

            if(rightPanel != null)
            {
                rightPanel.BackColor = panelBack;
                rightPanel.ForeColor = text;
            }
        }

        private void applyListTheme(Color listBack, Color listFore)
        {
            if(listViewTimeline != null)
            {
                listViewTimeline.BackColor = listBack;
                listViewTimeline.ForeColor = listFore;
            }
        }

        private void applyPreviewTheme(Color panelBack)
        {
            if(placeholderLabel != null)
            {
                placeholderLabel.ForeColor = m_IsDarkMode ? Color.Gainsboro : Color.DimGray;
                placeholderLabel.BackColor = Color.Transparent;
            }

            if(pictureBoxPreview != null)
            {
                pictureBoxPreview.BackColor = m_IsDarkMode ? Color.Black : SystemColors.ControlDark;
            }

            if(webBrowserPreview != null)
            {
                webBrowserPreview.BackColor = panelBack;
            }
        }

        private void applyButtonsTheme(Color buttonBack)
        {
            if(buttonBack != null)
            {
                if(this.buttonBack != null)
                {
                    this.buttonBack.ForeColor = Color.White;
                    if(this.buttonBack.BackColor == SystemColors.Control || this.buttonBack.BackColor.A == 0)
                    {
                        this.buttonBack.BackColor = buttonBack;
                    }

                    this.buttonBack.FlatStyle = FlatStyle.Flat;
                }

                if(buttonRefresh != null)
                {
                    buttonRefresh.ForeColor = Color.White;
                    if(buttonRefresh.BackColor == SystemColors.Control || buttonRefresh.BackColor.A == 0)
                    {
                        buttonRefresh.BackColor = buttonBack;
                    }

                    buttonRefresh.FlatStyle = FlatStyle.Flat;
                }
            }
        }

        private void applyCombosTheme(Color listBack, Color listFore)
        {
            if(comboBoxContent != null)
            {
                comboBoxContent.BackColor = listBack;
                comboBoxContent.ForeColor = listFore;
                comboBoxContent.FlatStyle = FlatStyle.Standard;
            }

            if(comboBoxGranularity != null)
            {
                comboBoxGranularity.BackColor = listBack;
                comboBoxGranularity.ForeColor = listFore;
                comboBoxGranularity.FlatStyle = FlatStyle.Standard;
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            populateTimeline();
        }

        private void populateTimeline()
        {
            if(listViewTimeline == null)
            {
                return;
            }

            withListViewUpdate(() =>
                {
                    listViewTimeline.Items.Clear();

                    User user = m_LoginResult != null ? m_LoginResult.LoggedInUser : null;
                    if(user == null)
                    {
                        return;
                    }

                    List<TimelineItem> items = getTimelineItems(user);
                    IEnumerable<TimelineItem> orderedItems = getSortedItems(items);
                    fillListView(orderedItems);
                });
        }

        private void withListViewUpdate(Action action)
        {
            if(listViewTimeline == null)
            {
                return;
            }

            listViewTimeline.BeginUpdate();
            try
            {
                action?.Invoke();
            }
            finally
            {
                listViewTimeline.EndUpdate();
            }
        }

        private List<TimelineItem> getTimelineItems(User user)
        {
            List<TimelineItem> items = new List<TimelineItem>();
            string filter = getSelectedFilter();

            if(filter == "All" || filter == "Posts")
            {
                addPosts(user, items);
            }

            if(filter == "All" || filter == "Photos")
            {
                addPhotos(user, items);
            }

            return items;
        }

        private string getSelectedFilter()
        {
            return comboBoxContent != null && comboBoxContent.SelectedItem != null
                       ? comboBoxContent.SelectedItem.ToString()
                       : "All";
        }

        private void addPosts(User user, List<TimelineItem> items)
        {
            try
            {
                foreach(Post post in user.Posts)
                {
                    DateTime? created = tryGetCreatedTime(post);
                    if(created.HasValue)
                    {
                        items.Add(
                            new TimelineItem
                                {
                                    Created = created.Value,
                                    Type = "Post",
                                    Summary = tryGetSummary(post),
                                    SourceObject = post
                                });
                    }
                }
            }
            catch
            {
            }
        }

        private void addPhotos(User user, List<TimelineItem> items)
        {
            addTaggedPhotos(user, items);
            addAlbumPhotos(user, items);
        }

        private void addTaggedPhotos(User user, List<TimelineItem> items)
        {
            try
            {
                foreach(Photo photo in user.PhotosTaggedIn)
                {
                    DateTime? created = tryGetCreatedTime(photo);
                    if(created.HasValue)
                    {
                        items.Add(
                            new TimelineItem
                                {
                                    Created = created.Value,
                                    Type = "Photo",
                                    Summary = tryGetSummary(photo),
                                    SourceObject = photo
                                });
                    }
                }
            }
            catch
            {
            }
        }

        private void addAlbumPhotos(User user, List<TimelineItem> items)
        {
            try
            {
                foreach(Album album in user.Albums)
                {
                    foreach(Photo photo in album.Photos)
                    {
                        DateTime? created = tryGetCreatedTime(photo);
                        if(created.HasValue)
                        {
                            items.Add(
                                new TimelineItem
                                    {
                                        Created = created.Value,
                                        Type = "Photo",
                                        Summary = tryGetSummary(photo),
                                        SourceObject = photo
                                    });
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private IEnumerable<TimelineItem> getSortedItems(List<TimelineItem> items)
        {
            string granularity = getGranularity();

            if(granularity.Contains("Year"))
            {
                return items.OrderByDescending(i => i.Created.Year).ThenByDescending(i => i.Created);
            }

            if(granularity.Contains("Month"))
            {
                return items.OrderByDescending(i => i.Created.Year).ThenByDescending(i => i.Created.Month)
                    .ThenByDescending(i => i.Created);
            }

            DateTime birth;
            if(granularity.Contains("Age") && tryParseBirthday(out birth))
            {
                return items.OrderByDescending(i => getAge(i.Created, birth)).ThenByDescending(i => i.Created);
            }

            return items.OrderByDescending(i => i.Created);
        }

        private string getGranularity()
        {
            return comboBoxGranularity != null && comboBoxGranularity.SelectedItem != null
                       ? comboBoxGranularity.SelectedItem.ToString()
                       : "Timeline by date";
        }

        private bool tryParseBirthday(out DateTime birth)
        {
            birth = DateTime.MinValue;
            string birthday = m_LoginResult != null && m_LoginResult.LoggedInUser != null
                                  ? m_LoginResult.LoggedInUser.Birthday
                                  : null;

            if(string.IsNullOrEmpty(birthday))
            {
                return false;
            }

            return DateTime.TryParse(birthday, out birth);
        }

        private int getAge(DateTime created, DateTime birth)
        {
            int age = created.Year - birth.Year;
            if(created < birth.AddYears(age))
            {
                age--;
            }

            return age;
        }

        private void fillListView(IEnumerable<TimelineItem> items)
        {
            bool hasBirthday = tryParseBirthday(out DateTime birthDate);
            string granularity = getGranularity();

            foreach(TimelineItem item in items)
            {
                string dateText = formatDateText(item, granularity, hasBirthday, birthDate);
                string summary = formatSummary(item.Summary);

                addListViewItem(item, dateText, summary);
            }
        }

        private string formatDateText(TimelineItem item, string granularity, bool hasBirthday, DateTime birthDate)
        {
            if(granularity.Contains("Year"))
            {
                return item.Created.Year.ToString();
            }

            if(granularity.Contains("Month"))
            {
                return item.Created.ToString("MMM yyyy");
            }

            if(granularity.Contains("Age") && hasBirthday)
            {
                int age = getAge(item.Created, birthDate);
                return age >= 0 ? age + "y" : "Unknown";
            }

            return item.Created.ToString("g");
        }

        private string formatSummary(string summary)
        {
            if(string.IsNullOrEmpty(summary))
            {
                return string.Empty;
            }

            return summary.Length > 100 ? summary.Substring(0, 100) + "..." : summary;
        }

        private void addListViewItem(TimelineItem item, string dateText, string summary)
        {
            ListViewItem listItem = new ListViewItem(dateText);
            listItem.SubItems.Add(item.Type);
            listItem.SubItems.Add(summary);
            listItem.Tag = item.SourceObject;
            listViewTimeline.Items.Add(listItem);
        }

        private void ListViewTimeline_DoubleClick(object sender, EventArgs e)
        {
            if(listViewTimeline.SelectedItems.Count == 0)
            {
                return;
            }

            ListViewItem selected = listViewTimeline.SelectedItems[0];
            object item = selected.Tag;

            string mediaUrl = getMediaUrl(item);
            if(!string.IsNullOrEmpty(mediaUrl))
            {
                showMedia(mediaUrl);
                return;
            }

            string link = getLink(item);
            if(!string.IsNullOrEmpty(link))
            {
                try
                {
                    Process.Start(link);
                }
                catch
                {
                }

                return;
            }

            MessageBox.Show(selected.SubItems[2].Text);
        }

        private void ListViewTimeline_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listViewTimeline.SelectedItems.Count == 0)
            {
                return;
            }

            ListViewItem selected = listViewTimeline.SelectedItems[0];
            if(selected.SubItems.Count < 2 || selected.SubItems[1].Text != "Photo")
            {
                clearPreview();
                return;
            }

            string mediaUrl = getMediaUrl(selected.Tag);
            if(!string.IsNullOrEmpty(mediaUrl))
            {
                showMedia(mediaUrl);
            }
            else
            {
                clearPreview();
            }
        }

        private string getMediaUrl(object item)
        {
            if(item == null)
            {
                return null;
            }

            string[] props = { "PictureNormalURL", "FullPicture", "PictureURL", "Picture", "Source", "Image" };
            Type t = item.GetType();

            foreach(string propName in props)
            {
                PropertyInfo prop = t.GetProperty(propName);
                if(prop != null)
                {
                    string value = prop.GetValue(item) as string;
                    if(!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
            }

            return null;
        }

        private string getLink(object item)
        {
            if(item == null)
            {
                return null;
            }

            Type t = item.GetType();
            PropertyInfo prop = t.GetProperty("Link");
            if(prop == null)
            {
                return null;
            }

            return prop.GetValue(item) as string;
        }

        private void clearPreview()
        {
            if(pictureBoxPreview != null && pictureBoxPreview.Image != null)
            {
                pictureBoxPreview.Image.Dispose();
                pictureBoxPreview.Image = null;
            }

            if(pictureBoxPreview != null)
            {
                pictureBoxPreview.Visible = false;
            }

            if(placeholderLabel != null)
            {
                placeholderLabel.Visible = true;
            }

            if(webBrowserPreview != null)
            {
                webBrowserPreview.Visible = false;
            }
        }

        private void showMedia(string url)
        {
            if(string.IsNullOrEmpty(url))
            {
                clearPreview();
                return;
            }

            if(!isImageUrl(url))
            {
                showNonImageMedia(url);
                return;
            }

            clearPreview();
            loadImage(url);
        }

        private void showNonImageMedia(string url)
        {
            if(pictureBoxPreview != null)
            {
                pictureBoxPreview.Visible = false;
            }

            if(placeholderLabel != null)
            {
                placeholderLabel.Visible = false;
            }

            if(webBrowserPreview != null)
            {
                webBrowserPreview.Navigate(url);
                webBrowserPreview.Visible = true;
            }
        }

        private void loadImage(string url)
        {
            try
            {
                setPreviewVisibilityForImage();

                using(WebClient wc = new WebClient())
                {
                    byte[] data = wc.DownloadData(url);
                    using(MemoryStream ms = new MemoryStream(data))
                    {
                        Image img = Image.FromStream(ms);
                        if(pictureBoxPreview != null)
                        {
                            pictureBoxPreview.Image = new Bitmap(img);
                        }
                    }
                }
            }
            catch
            {
                clearPreview();
            }
        }

        private void setPreviewVisibilityForImage()
        {
            if(placeholderLabel != null)
            {
                placeholderLabel.Visible = false;
            }

            if(webBrowserPreview != null)
            {
                webBrowserPreview.Visible = false;
            }

            if(pictureBoxPreview != null)
            {
                pictureBoxPreview.Visible = true;
            }
        }

        private bool isImageUrl(string url)
        {
            if(string.IsNullOrEmpty(url))
            {
                return false;
            }

            string lower = url.ToLowerInvariant();
            return lower.EndsWith(".jpg") || lower.EndsWith(".jpeg") || lower.EndsWith(".png") || lower.EndsWith(".gif")
                   || lower.EndsWith(".bmp");
        }

        private DateTime? tryGetCreatedTime(object item)
        {
            if(item == null)
            {
                return null;
            }

            string[] dateProps = { "CreatedTime", "StartTime", "UpdatedTime" };
            Type t = item.GetType();

            foreach(string propName in dateProps)
            {
                PropertyInfo prop = t.GetProperty(propName);
                if(prop != null)
                {
                    object value = prop.GetValue(item);
                    if(value is DateTime)
                    {
                        return (DateTime)value;
                    }

                    DateTime dt;
                    if(value is string && DateTime.TryParse((string)value, out dt))
                    {
                        return dt;
                    }
                }
            }

            return null;
        }

        private string tryGetSummary(object item)
        {
            if(item == null)
            {
                return "Post";
            }

            string[] textProps = { "Message", "Caption", "Name", "Description" };
            Type t = item.GetType();

            foreach(string propName in textProps)
            {
                PropertyInfo prop = t.GetProperty(propName);
                if(prop != null)
                {
                    string value = prop.GetValue(item) as string;
                    if(!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
            }

            return "Post";
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

        private class TimelineItem
        {
            public DateTime Created { get; set; }

            public string Type { get; set; }

            public string Summary { get; set; }

            public object SourceObject { get; set; }
        }
    }
}