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

        // For designer
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
                PopulateTimeline();
            }
        }

        private void FormTimeline_Load(object sender, EventArgs e)
        {
            AdjustColumns();
            if (m_LoginResult != null && m_LoginResult.LoggedInUser != null)
            {
                PopulateTimeline();
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            PopulateTimeline();
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

        private void PopulateTimeline()
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

            List<TimelineItem> items = GetTimelineItems(user);
            IEnumerable<TimelineItem> orderedItems = GetSortedItems(items);
            FillListView(orderedItems);

            listViewTimeline.EndUpdate();
        }

        private List<TimelineItem> GetTimelineItems(User user)
        {
            List<TimelineItem> items = new List<TimelineItem>();
            string filter = comboBoxContent.SelectedItem != null ? comboBoxContent.SelectedItem.ToString() : "All";

            if (filter == "All" || filter == "Posts")
            {
                AddPosts(user, items);
            }

            if (filter == "All" || filter == "Photos")
            {
                AddPhotos(user, items);
            }

            return items;
        }

        private void AddPosts(User user, List<TimelineItem> items)
        {
            try
            {
                foreach (Post post in user.Posts)
                {
                    DateTime? created = TryGetCreatedTime(post);
                    if (created.HasValue)
                    {
                        items.Add(new TimelineItem
                        {
                            Created = created.Value,
                            Type = "Post",
                            Summary = TryGetSummary(post),
                            SourceObject = post
                        });
                    }
                }
            }
            catch
            {
            }
        }

        private void AddPhotos(User user, List<TimelineItem> items)
        {
            try
            {
                foreach (Photo photo in user.PhotosTaggedIn)
                {
                    DateTime? created = TryGetCreatedTime(photo);
                    if (created.HasValue)
                    {
                        items.Add(new TimelineItem
                        {
                            Created = created.Value,
                            Type = "Photo",
                            Summary = TryGetSummary(photo),
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
                        DateTime? created = TryGetCreatedTime(photo);
                        if (created.HasValue)
                        {
                            items.Add(new TimelineItem
                            {
                                Created = created.Value,
                                Type = "Photo",
                                Summary = TryGetSummary(photo),
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

        private IEnumerable<TimelineItem> GetSortedItems(List<TimelineItem> items)
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

            if (granularity.Contains("Day"))
            {
                return items
                    .OrderByDescending(i => i.Created.Year)
                    .ThenByDescending(i => i.Created.Month)
                    .ThenByDescending(i => i.Created.Day)
                    .ThenByDescending(i => i.Created);
            }

            DateTime birth;
            if (granularity.Contains("Age") && TryParseBirthday(out birth))
            {
                return items
                    .OrderByDescending(i => GetAge(i.Created, birth))
                    .ThenByDescending(i => i.Created);
            }

            return items.OrderByDescending(i => i.Created);
        }

        private bool TryParseBirthday(out DateTime birth)
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

        private int GetAge(DateTime created, DateTime birth)
        {
            int age = created.Year - birth.Year;
            if (created < birth.AddYears(age))
            {
                age--;
            }

            return age;
        }

        private void FillListView(IEnumerable<TimelineItem> items)
        {
            bool hasBirthday = TryParseBirthday(out DateTime birthDate);
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
                else if (granularity.Contains("Day"))
                {
                    dateText = item.Created.ToString("d");
                }
                else if (granularity.Contains("Age") && hasBirthday)
                {
                    int age = GetAge(item.Created, birthDate);
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

            string mediaUrl = GetMediaUrl(item);
            if (!string.IsNullOrEmpty(mediaUrl))
            {
                ShowMedia(mediaUrl);
                return;
            }

            string link = GetLink(item);
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
                ClearPreview();
                return;
            }

            string mediaUrl = GetMediaUrl(selected.Tag);
            if (!string.IsNullOrEmpty(mediaUrl))
            {
                ShowMedia(mediaUrl);
            }
            else
            {
                ClearPreview();
            }
        }

        private string GetMediaUrl(object item)
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

        private string GetLink(object item)
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

        private void ClearPreview()
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

        private void ShowMedia(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                ClearPreview();
                return;
            }

            if (!IsImageUrl(url))
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

            ClearPreview();
            LoadImage(url);
        }

        private void LoadImage(string url)
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
                ClearPreview();
            }
        }

        private bool IsImageUrl(string url)
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

        private DateTime? TryGetCreatedTime(object item)
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

        private string TryGetSummary(object item)
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

        private class TimelineItem
        {
            public DateTime Created { get; set; }
            public string Type { get; set; }
            public string Summary { get; set; }
            public object SourceObject { get; set; }
        }
    }
}
