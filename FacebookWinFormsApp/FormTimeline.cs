using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using FacebookWrapper;
using FacebookWrapper.ObjectModel;

namespace BasicFacebookFeatures
{
    public partial class FormTimeline : Form
    {
        private LoginResult m_LoginResult;
        private bool m_IsDarkMode;

        public FormTimeline(LoginResult loginResult)
        {
            InitializeComponent();
            m_LoginResult = loginResult;
            this.Load += FormTimeline_Load;
            if (listViewTimeline != null)
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
            this.Load += FormTimeline_Load;
            if (listViewTimeline != null)
            {
                listViewTimeline.SelectedIndexChanged += ListViewTimeline_SelectedIndexChanged;
                listViewTimeline.DoubleClick += ListViewTimeline_DoubleClick;
            }
        }

        public void SetLoginResult(LoginResult loginResult)
        {
            m_LoginResult = loginResult;
            if (IsHandleCreated && Visible)
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
            if (m_LoginResult != null && m_LoginResult.LoggedInUser != null)
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

            this.BackColor = formBack;

            if (topPanel != null)
            {
                topPanel.BackColor = headerBack;
                foreach (Control c in topPanel.Controls)
                {
                    if (c is Label || c is LinkLabel)
                    {
                        c.ForeColor = Color.White;
                    }
                    else
                    {
                        c.ForeColor = text;
                    }
                }
            }

            if (leftPanel != null)
            {
                leftPanel.BackColor = panelBack;
                leftPanel.ForeColor = text;
            }

            if (rightPanel != null)
            {
                rightPanel.BackColor = panelBack;
                rightPanel.ForeColor = text;
            }

            if (listViewTimeline != null)
            {
                listViewTimeline.BackColor = listBack;
                listViewTimeline.ForeColor = listFore;
            }

            if (placeholderLabel != null)
            {
                placeholderLabel.ForeColor = m_IsDarkMode ? Color.Gainsboro : Color.DimGray;
                placeholderLabel.BackColor = Color.Transparent;
            }

            if (pictureBoxPreview != null)
            {
                pictureBoxPreview.BackColor = m_IsDarkMode ? Color.Black : SystemColors.ControlDark;
            }

            if (webBrowserPreview != null)
            {
                webBrowserPreview.BackColor = panelBack;
            }

            if (buttonBack != null)
            {
                buttonBack.ForeColor = Color.White;
                if (buttonBack.BackColor == SystemColors.Control || buttonBack.BackColor.A == 0)
                {
                    buttonBack.BackColor = Color.FromArgb(66, 103, 178);
                }
                buttonBack.FlatStyle = FlatStyle.Flat;
            }

            if (buttonRefresh != null)
            {
                buttonRefresh.ForeColor = Color.White;
                if (buttonRefresh.BackColor == SystemColors.Control || buttonRefresh.BackColor.A == 0)
                {
                    buttonRefresh.BackColor = Color.FromArgb(66, 103, 178);
                }
                buttonRefresh.FlatStyle = FlatStyle.Flat;
            }

            if (comboBoxContent != null)
            {
                comboBoxContent.BackColor = listBack;
                comboBoxContent.ForeColor = listFore;
                comboBoxContent.FlatStyle = FlatStyle.Standard;
            }

            if (comboBoxGranularity != null)
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

        private void leftPanel_Resize(object sender, EventArgs e)
        {
            AdjustColumns();
        }

        private void AdjustColumns()
        {
            if (leftPanel == null || listViewTimeline == null)
            {
                return;
            }

            int dateWidth = 160;
            int typeWidth = 90;
            int summaryWidth = Math.Max(120, leftPanel.ClientSize.Width - dateWidth - typeWidth - 12);

            if (listViewTimeline.Columns.Count == 3)
            {
                listViewTimeline.Columns[0].Width = dateWidth;
                listViewTimeline.Columns[1].Width = typeWidth;
                listViewTimeline.Columns[2].Width = summaryWidth;
            }
        }

        private void populateTimeline()
        {
            if (listViewTimeline == null)
            {
                return;
            }

            listViewTimeline.BeginUpdate();
            listViewTimeline.Items.Clear();

            User user = m_LoginResult != null ? m_LoginResult.LoggedInUser : null;
            if (user == null)
            {
                listViewTimeline.EndUpdate();
                return;
            }

            List<TimelineItem> items = getTimelineItems(user);
            IEnumerable<TimelineItem> orderedItems = getSortedItems(items);
            fillListView(orderedItems);

            listViewTimeline.EndUpdate();
        }

        private List<TimelineItem> getTimelineItems(User user)
        {
            List<TimelineItem> items = new List<TimelineItem>();
            string filter = comboBoxContent.SelectedItem != null ? comboBoxContent.SelectedItem.ToString() : "All";

            if (filter == "All" || filter == "Posts")
            {
                addPosts(user, items);
            }

            if (filter == "All" || filter == "Photos")
            {
                addPhotos(user, items);
            }

            return items;
        }

        private void addPosts(User user, List<TimelineItem> items)
        {
            try
            {
                foreach (Post post in user.Posts)
                {
                    DateTime? created = tryGetCreatedTime(post);
                    if (created.HasValue)
                    {
                        items.Add(new TimelineItem
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
            try
            {
                foreach (Photo photo in user.PhotosTaggedIn)
                {
                    DateTime? created = tryGetCreatedTime(photo);
                    if (created.HasValue)
                    {
                        items.Add(new TimelineItem
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

            try
            {
                foreach (Album album in user.Albums)
                {
                    foreach (Photo photo in album.Photos)
                    {
                        DateTime? created = tryGetCreatedTime(photo);
                        if (created.HasValue)
                        {
                            items.Add(new TimelineItem
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
            string granularity = comboBoxGranularity.SelectedItem != null
                ? comboBoxGranularity.SelectedItem.ToString()
                : "Timeline by date";

            if (granularity.Contains("Year"))
            {
                return items
                    .OrderByDescending(i => i.Created.Year)
                    .ThenByDescending(i => i.Created);
            }

            if (granularity.Contains("Month"))
            {
                return items
                    .OrderByDescending(i => i.Created.Year)
                    .ThenByDescending(i => i.Created.Month)
                    .ThenByDescending(i => i.Created);
            }

            DateTime birth;
            if (granularity.Contains("Age") && tryParseBirthday(out birth))
            {
                return items
                    .OrderByDescending(i => getAge(i.Created, birth))
                    .ThenByDescending(i => i.Created);
            }

            return items.OrderByDescending(i => i.Created);
        }

        private bool tryParseBirthday(out DateTime birth)
        {
            birth = DateTime.MinValue;
            string birthday = m_LoginResult != null && m_LoginResult.LoggedInUser != null
                ? m_LoginResult.LoggedInUser.Birthday
                : null;

            if (string.IsNullOrEmpty(birthday))
            {
                return false;
            }

            return DateTime.TryParse(birthday, out birth);
        }

        private int getAge(DateTime created, DateTime birth)
        {
            int age = created.Year - birth.Year;
            if (created < birth.AddYears(age))
            {
                age--;
            }

            return age;
        }

        private void fillListView(IEnumerable<TimelineItem> items)
        {
            bool hasBirthday = tryParseBirthday(out DateTime birthDate);
            string granularity = comboBoxGranularity.SelectedItem != null
                ? comboBoxGranularity.SelectedItem.ToString()
                : "Timeline by date";

            foreach (TimelineItem item in items)
            {
                string dateText;

                if (granularity.Contains("Year"))
                {
                    dateText = item.Created.Year.ToString();
                }
                else if (granularity.Contains("Month"))
                {
                    dateText = item.Created.ToString("MMM yyyy");
                }
                else if (granularity.Contains("Age") && hasBirthday)
                {
                    int age = getAge(item.Created, birthDate);
                    dateText = age >= 0 ? age + "y" : "Unknown";
                }
                else
                {
                    dateText = item.Created.ToString("g");
                }

                string summary = item.Summary != null && item.Summary.Length > 100
                    ? item.Summary.Substring(0, 100) + "..."
                    : item.Summary ?? string.Empty;

                ListViewItem listItem = new ListViewItem(dateText);
                listItem.SubItems.Add(item.Type);
                listItem.SubItems.Add(summary);
                listItem.Tag = item.SourceObject;
                listViewTimeline.Items.Add(listItem);
            }
        }

        private void ListViewTimeline_DoubleClick(object sender, EventArgs e)
        {
            if (listViewTimeline.SelectedItems.Count == 0)
            {
                return;
            }

            ListViewItem selected = listViewTimeline.SelectedItems[0];
            object item = selected.Tag;

            string mediaUrl = getMediaUrl(item);
            if (!string.IsNullOrEmpty(mediaUrl))
            {
                showMedia(mediaUrl);
                return;
            }

            string link = getLink(item);
            if (!string.IsNullOrEmpty(link))
            {
                try
                {
                    System.Diagnostics.Process.Start(link);
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
            if (listViewTimeline.SelectedItems.Count == 0)
            {
                return;
            }

            ListViewItem selected = listViewTimeline.SelectedItems[0];
            if (selected.SubItems.Count < 2 || selected.SubItems[1].Text != "Photo")
            {
                clearPreview();
                return;
            }

            string mediaUrl = getMediaUrl(selected.Tag);
            if (!string.IsNullOrEmpty(mediaUrl))
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
            if (item == null)
            {
                return null;
            }

            string[] props = { "PictureNormalURL", "FullPicture", "PictureURL", "Picture", "Source", "Image" };
            Type t = item.GetType();

            foreach (string propName in props)
            {
                var prop = t.GetProperty(propName);
                if (prop != null)
                {
                    string value = prop.GetValue(item) as string;
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
            }

            return null;
        }

        private string getLink(object item)
        {
            if (item == null)
            {
                return null;
            }

            Type t = item.GetType();
            var prop = t.GetProperty("Link");
            if (prop == null)
            {
                return null;
            }

            return prop.GetValue(item) as string;
        }

        private void clearPreview()
        {
            if (pictureBoxPreview != null && pictureBoxPreview.Image != null)
            {
                pictureBoxPreview.Image.Dispose();
                pictureBoxPreview.Image = null;
            }

            if (pictureBoxPreview != null)
            {
                pictureBoxPreview.Visible = false;
            }
            if (placeholderLabel != null)
            {
                placeholderLabel.Visible = true;
            }
            if (webBrowserPreview != null)
            {
                webBrowserPreview.Visible = false;
            }
        }

        private void showMedia(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                clearPreview();
                return;
            }

            if (!isImageUrl(url))
            {
                if (pictureBoxPreview != null)
                {
                    pictureBoxPreview.Visible = false;
                }
                if (placeholderLabel != null)
                {
                    placeholderLabel.Visible = false;
                }
                if (webBrowserPreview != null)
                {
                    webBrowserPreview.Navigate(url);
                    webBrowserPreview.Visible = true;
                }
                return;
            }

            clearPreview();
            loadImage(url);
        }

        private void loadImage(string url)
        {
            try
            {
                if (placeholderLabel != null)
                {
                    placeholderLabel.Visible = false;
                }
                if (webBrowserPreview != null)
                {
                    webBrowserPreview.Visible = false;
                }
                if (pictureBoxPreview != null)
                {
                    pictureBoxPreview.Visible = true;
                }

                using (WebClient wc = new WebClient())
                {
                    byte[] data = wc.DownloadData(url);
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        Image img = Image.FromStream(ms);
                        if (pictureBoxPreview != null)
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

        private bool isImageUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            string lower = url.ToLowerInvariant();
            return lower.EndsWith(".jpg") ||
                   lower.EndsWith(".jpeg") ||
                   lower.EndsWith(".png") ||
                   lower.EndsWith(".gif") ||
                   lower.EndsWith(".bmp");
        }

        private DateTime? tryGetCreatedTime(object item)
        {
            if (item == null)
            {
                return null;
            }

            string[] dateProps = { "CreatedTime", "StartTime", "UpdatedTime" };
            Type t = item.GetType();

            foreach (string propName in dateProps)
            {
                var prop = t.GetProperty(propName);
                if (prop != null)
                {
                    object value = prop.GetValue(item);
                    if (value is DateTime)
                    {
                        return (DateTime)value;
                    }

                    DateTime dt;
                    if (value is string && DateTime.TryParse((string)value, out dt))
                    {
                        return dt;
                    }
                }
            }

            return null;
        }

        private string tryGetSummary(object item)
        {
            if (item == null)
            {
                return "Post";
            }

            string[] textProps = { "Message", "Caption", "Name", "Description" };
            Type t = item.GetType();

            foreach (string propName in textProps)
            {
                var prop = t.GetProperty(propName);
                if (prop != null)
                {
                    string value = prop.GetValue(item) as string;
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
            }

            return "Post";
        }

        // Back button handler: show main menu or bring main form forward then close timeline.
        private void buttonBack_Click(object sender, EventArgs e)
        {
            try
            {
                var main = Application.OpenForms.OfType<FormMain>().FirstOrDefault();
                if (main != null)
                {
                    var mi = main.GetType().GetMethod(
                        "navigateToMenu",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

                    if (mi != null)
                    {
                        mi.Invoke(main, null);
                        this.Close();
                        return;
                    }

                    main.Invoke(new Action(() => main.BringToFront()));
                }
            }
            catch
            {
            }

            this.Close();
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
