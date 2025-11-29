using FacebookWrapper;
using FacebookWrapper.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                listViewTimeline.SelectedIndexChanged += ListViewTimeline_SelectedIndexChanged;
        }

        // parameterless ctor for designer
        public FormTimeline()
        {
            InitializeComponent();
            this.Load += FormTimeline_Load;
            if (listViewTimeline != null)
                listViewTimeline.SelectedIndexChanged += ListViewTimeline_SelectedIndexChanged;
        }

        // Allow callers to set login result when using parameterless ctor
        public void SetLoginResult(LoginResult loginResult)
        {
            m_LoginResult = loginResult;
            if (this.IsHandleCreated && this.Visible)
            {
                try
                {
                    PopulateTimeline();
                }
                catch { }
            }
        }

        private void FormTimeline_Load(object sender, EventArgs e)
        {
            AdjustColumns();
            if (m_LoginResult == null || m_LoginResult.LoggedInUser == null)
                return; // keep simple, do not throw, user can call SetLoginResult later
            PopulateTimeline();
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
            try
            {
                if (leftPanel == null || listViewTimeline == null) return;
                if (leftPanel.IsDisposed || listViewTimeline.IsDisposed) return;

                const int dateColWidth = 160;
                const int typeColWidth = 90;
                int leftWidth = leftPanel.ClientSize.Width;
                int available = Math.Max(120, leftWidth - dateColWidth - typeColWidth - 12);

                if (listViewTimeline.Columns.Count == 3)
                {
                    listViewTimeline.Columns[0].Width = dateColWidth;
                    listViewTimeline.Columns[1].Width = typeColWidth;
                    listViewTimeline.Columns[2].Width = available;
                }
            }
            catch { }
        }

        private void PopulateTimeline()
        {
            if (listViewTimeline == null) return;

            listViewTimeline.BeginUpdate();
            try
            {
                listViewTimeline.Items.Clear();

                var user = m_LoginResult?.LoggedInUser;
                if (user == null) return;

                var items = new List<TimelineItem>();

                string contentFilter = comboBoxContent.SelectedItem?.ToString() ?? "All";
                string granularity = comboBoxGranularity.SelectedItem?.ToString() ?? "Timeline by date";

                // Posts
                if (contentFilter == "All" || contentFilter == "Posts")
                {
                    try
                    {
                        foreach (var p in user.Posts ?? Enumerable.Empty<dynamic>())
                        {
                            var created = TryGetCreatedTime(p);
                            if (created.HasValue)
                                items.Add(new TimelineItem { Created = created.Value, Type = "Post", Summary = TryGetSummary(p, "Post"), SourceObject = p });
                        }
                    }
                    catch { }
                }

                // Photos tagged + photos uploads
                if (contentFilter == "All" || contentFilter == "Photos")
                {
                    try
                    {
                        foreach (var ph in user.PhotosTaggedIn ?? Enumerable.Empty<dynamic>())
                        {
                            var created = TryGetCreatedTime(ph);
                            if (created.HasValue)
                                items.Add(new TimelineItem { Created = created.Value, Type = "Photo", Summary = TryGetSummary(ph, "Photo"), SourceObject = ph });
                        }
                    }
                    catch { }

                    try
                    {
                        foreach (var album in user.Albums ?? Enumerable.Empty<Album>())
                        {
                            foreach (var photo in album.Photos ?? Enumerable.Empty<Photo>())
                            {
                                var created2 = TryGetCreatedTime(photo);
                                if (created2.HasValue)
                                    items.Add(new TimelineItem { Created = created2.Value, Type = "Photo", Summary = TryGetSummary(photo, "Photo"), SourceObject = photo });
                            }
                        }
                    }
                    catch { }
                }

                // Sort according to granularity
                IEnumerable<TimelineItem> ordered = items;
                if (granularity.Contains("Year"))
                    ordered = items.OrderByDescending(i => i.Created.Year).ThenByDescending(i => i.Created);
                else if (granularity.Contains("Month"))
                    ordered = items.OrderByDescending(i => new { i.Created.Year, i.Created.Month }).ThenByDescending(i => i.Created);
                else if (granularity.Contains("Day"))
                    ordered = items.OrderByDescending(i => new { i.Created.Year, i.Created.Month, i.Created.Day }).ThenByDescending(i => i.Created);
                else if (granularity.Contains("Age") && m_LoginResult?.LoggedInUser?.Birthday != null)
                {
                    DateTime birth;
                    if (DateTime.TryParse(m_LoginResult.LoggedInUser.Birthday, out birth))
                    {
                        ordered = items.OrderByDescending(i =>
                        {
                            var age = i.Created.Year - birth.Year;
                            if (i.Created < birth.AddYears(age)) age--;
                            return age;
                        }).ThenByDescending(i => i.Created);
                    }
                }
                else
                    ordered = items.OrderByDescending(i => i.Created);

                foreach (var ti in ordered)
                {
                    var lvi = new ListViewItem(ti.Created.ToString("g"));
                    lvi.SubItems.Add(ti.Type);
                    lvi.SubItems.Add(ti.Summary);
                    lvi.Tag = ti.SourceObject;
                    listViewTimeline.Items.Add(lvi);
                }
            }
            finally
            {
                listViewTimeline.EndUpdate();
            }
        }

        private void ListViewTimeline_DoubleClick(object sender, EventArgs e)
        {
            if (listViewTimeline.SelectedItems.Count == 0) return;

            var selected = listViewTimeline.SelectedItems[0];
            dynamic src = selected.Tag;

            string media = null;
            try { media = src.PictureNormalURL as string; } catch { }
            if (string.IsNullOrEmpty(media)) try { media = src.FullPicture as string; } catch { }
            if (string.IsNullOrEmpty(media)) try { media = src.Source as string; } catch { }
            if (string.IsNullOrEmpty(media)) try { media = src.Picture as string; } catch { }
            if (string.IsNullOrEmpty(media)) try { media = src.Image as string; } catch { }

            if (!string.IsNullOrEmpty(media))
            {
                ShowMedia(media);
                return;
            }

            string link = null;
            try { link = src.Link as string; } catch { }
            if (!string.IsNullOrEmpty(link))
            {
                try { System.Diagnostics.Process.Start(link); } catch { }
                return;
            }

            MessageBox.Show(selected.SubItems[2].Text, "Timeline item");
        }

        // Preview when selection changes - immediately show photo if item is photo
        private void ListViewTimeline_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewTimeline.SelectedItems.Count == 0) return;

            var selected = listViewTimeline.SelectedItems[0];
            if (selected.SubItems.Count < 2) return;

            if (selected.SubItems[1].Text == "Photo")
            {
                dynamic src = selected.Tag;
                string media = null;
                try { media = src.PictureNormalURL as string; } catch { }
                if (string.IsNullOrEmpty(media)) try { media = src.FullPicture as string; } catch { }
                if (string.IsNullOrEmpty(media)) try { media = src.Source as string; } catch { }
                if (string.IsNullOrEmpty(media)) try { media = src.Picture as string; } catch { }
                if (string.IsNullOrEmpty(media)) try { media = src.Image as string; } catch { }

                if (!string.IsNullOrEmpty(media))
                {
                    ShowMedia(media);
                }
            }
            else
            {
                // clear preview for non-photo
                try
                {
                    if (pictureBoxPreview != null)
                    {
                        var old = pictureBoxPreview.Image;
                        pictureBoxPreview.Image = null;
                        try { old?.Dispose(); } catch { }
                    }
                    pictureBoxPreview.Visible = false;
                    if (placeholderLabel != null) placeholderLabel.Visible = true;
                    if (webBrowserPreview != null) webBrowserPreview.Visible = false;
                }
                catch { }
            }
        }

        private void ShowMedia(string url)
        {
            if (string.IsNullOrEmpty(url)) return;

            bool isImage = false;
            try
            {
                var lower = url.ToLowerInvariant();
                if (lower.EndsWith(".jpg") || lower.EndsWith(".jpeg") || lower.EndsWith(".png") ||
                    lower.EndsWith(".gif") || lower.EndsWith(".bmp") || lower.Contains("format=jpg") ||
                    lower.Contains("type=large"))
                    isImage = true;
            }
            catch { isImage = false; }

            try
            {
                if (isImage)
                {
                    placeholderLabel.Visible = false;
                    if (webBrowserPreview != null) webBrowserPreview.Visible = false;
                    pictureBoxPreview.Visible = true;

                    var old = pictureBoxPreview.Image;
                    if (old != null)
                    {
                        pictureBoxPreview.Image = null;
                        try { old.Dispose(); } catch { }
                    }

                    using (var wc = new WebClient())
                    {
                        byte[] data = wc.DownloadData(url);
                        using (var ms = new MemoryStream(data))
                        {
                            var img = Image.FromStream(ms);
                            pictureBoxPreview.Image = new Bitmap(img);
                        }
                    }
                    return;
                }

                // non-image
                pictureBoxPreview.Visible = false;
                placeholderLabel.Visible = false;
                if (webBrowserPreview != null)
                {
                    webBrowserPreview.Visible = true;
                    try { webBrowserPreview.Navigate(url); } catch { }
                }
            }
            catch
            {
                try
                {
                    System.Diagnostics.Process.Start(url);
                }
                catch { }
            }
            finally
            {
                try
                {
                    pictureBoxPreview.Visible = false;
                    if (placeholderLabel != null) placeholderLabel.Visible = true;
                }
                catch { }
            }
        }

        private DateTime? TryGetCreatedTime(dynamic item)
        {
            if (item == null) return null;
            try
            {
                DateTime? dt = item.CreatedTime;
                if (dt.HasValue) return dt.Value;
            }
            catch { }
            try
            {
                DateTime? dt2 = item.StartTime;
                if (dt2.HasValue) return dt2.Value;
            }
            catch { }
            try
            {
                DateTime? dt3 = item.UpdatedTime;
                if (dt3.HasValue) return dt3.Value;
            }
            catch { }
            return null;
        }

        private string TryGetSummary(dynamic item, string type)
        {
            if (item == null) return "no summary";
            try
            {
                if (!string.IsNullOrEmpty(item.Message)) return item.Message;
            }
            catch { }
            try
            {
                if (!string.IsNullOrEmpty(item.Caption)) return item.Caption;
            }
            catch { }
            try
            {
                if (!string.IsNullOrEmpty(item.Name)) return item.Name;
            }
            catch { }
            return type;
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
