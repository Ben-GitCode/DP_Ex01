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
        private LoginResult m_LoginResult;
        private UiPalette m_UiPalette;

        public FormTimeline(LoginResult i_LoginResult, UiPalette i_UiPalette)
        {
            InitializeComponent();
            m_LoginResult = i_LoginResult;
            m_UiPalette = i_UiPalette;

            this.Load += FormTimeline_Load;
            if (listViewTimeline != null)
            {
                listViewTimeline.SelectedIndexChanged += ListViewTimeline_SelectedIndexChanged;
                listViewTimeline.DoubleClick += ListViewTimeline_DoubleClick;
            }
        }

        public FormTimeline(LoginResult i_LoginResult) : this(i_LoginResult, null) { }
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

        public void SetLoginResult(LoginResult i_LoginResult)
        {
            m_LoginResult = i_LoginResult;
            if (IsHandleCreated && Visible)
            {
                populateTimeline();
            }
        }

        public void SetPalette(UiPalette palette)
        {
            Color formBack = r_IsDarkMode ? ColorPalette.sr_Black : ColorPalette.sr_White;
            Color panelBack = r_IsDarkMode ? ColorPalette.sr_DarkGray : ColorPalette.sr_White;
            Color text = r_IsDarkMode ? ColorPalette.sr_WhitishBlue : ColorPalette.sr_DarkBlue;
            Color listBack = r_IsDarkMode ? ColorPalette.sr_DarkGray : ColorPalette.sr_White;
            Color listFore = text;
            Color headerBack = r_IsDarkMode ? ColorPalette.sr_DarkBlue : ColorPalette.sr_WhitishBlue;
            Color buttonBackColor = ColorPalette.sr_FacebookBlue;

            BackColor = formBack;

            applyHeaderTheme(headerBack, text);
            applyPanelsTheme(panelBack, text);
            applyListTheme(listBack, listFore);
            applyPreviewTheme(panelBack);
            applyButtonsTheme(buttonBackColor);
            applyCombosTheme(listBack, listFore);
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
            var p = m_UiPalette ?? new UiPalette();

            foreach(Control control in topPanel.Controls.OfType<Panel>())
            {
                control.BackColor = i_HeaderBack;

                foreach(Control subControl in control.Controls)
                {
                    if(subControl is Label || subControl is LinkLabel)
                    {
                        if(subControl.Text.Contains("Timeline"))
                        {
                            subControl.ForeColor = ColorPalette.sr_White;
                        }
                        else if(subControl.Text.Contains("Recent"))
                        {
                            subControl.ForeColor = ColorPalette.sr_MidGray;
                        }
                        else
                        {
                            subControl.ForeColor = i_TextColor;
                        }
                    }
                }
            }

            Control filtersPanel = topPanel.Controls.OfType<Panel>().FirstOrDefault(p => p.Location.Y == 132);
            if(filtersPanel != null)
            {
                foreach(Control control in filtersPanel.Controls)
                {
                    if(control is Label || control is LinkLabel)
                    {
                        control.ForeColor = i_TextColor;
                    }
                }
            }

            if (leftPanel != null)
            {
                leftPanel.BackColor = p.PanelBack;
                leftPanel.ForeColor = p.PrimaryText;
            }

            if (rightPanel != null)
            {
                rightPanel.BackColor = p.PanelBack;
                rightPanel.ForeColor = p.PrimaryText;
            }

            if (listViewTimeline != null)
            {
                listViewTimeline.BackColor = p.ListBack;
                listViewTimeline.ForeColor = p.ListFore;
            }

            if (placeholderLabel != null)
            {
                placeholderLabel.ForeColor = r_IsDarkMode ? ColorPalette.sr_MidGray : ColorPalette.sr_MidGray;
                placeholderLabel.BackColor = Color.Transparent;
            }

            if (pictureBoxPreview != null)
            {
                pictureBoxPreview.BackColor = r_IsDarkMode ? ColorPalette.sr_DarkGray : SystemColors.ControlDark;
            }

            if (webBrowserPreview != null)
            {
                webBrowserPreview.BackColor = p.PanelBack;
            }

        private void applyButtonsTheme(Color i_ButtonBackColor)
        {
            if(buttonBack != null)
            {
                buttonBack.ForeColor = ColorPalette.sr_White;
                buttonBack.BackColor = r_IsDarkMode ? ColorPalette.sr_DarkBlue : i_ButtonBackColor;
                buttonBack.FlatStyle = FlatStyle.Flat;
            }

            if(buttonRefresh != null)
            {
                buttonRefresh.ForeColor = ColorPalette.sr_White;
                buttonRefresh.BackColor = r_IsDarkMode ? ColorPalette.sr_DarkBlue : i_ButtonBackColor;
                buttonRefresh.FlatStyle = FlatStyle.Flat;
            }

            if (comboBoxContent != null)
            {
                comboBoxContent.BackColor = p.ListBack;
                comboBoxContent.ForeColor = p.ListFore;
                comboBoxContent.FlatStyle = FlatStyle.Standard;
            }

            if (comboBoxGranularity != null)
            {
                comboBoxGranularity.BackColor = p.ListBack;
                comboBoxGranularity.ForeColor = p.ListFore;
                comboBoxGranularity.FlatStyle = FlatStyle.Standard;
            }
        }

        private void buttonRefresh_Click(object i_Sender, EventArgs i_EventArgs)
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

        private void withListViewUpdate(Action i_Action)
        {
            if(listViewTimeline == null)
            {
                return;
            }

            listViewTimeline.BeginUpdate();
            try
            {
                i_Action?.Invoke();
            }
            finally
            {
                listViewTimeline.EndUpdate();
            }
        }

        private List<TimelineItem> getTimelineItems(User i_User)
        {
            List<TimelineItem> items = new List<TimelineItem>();
            string filter = getSelectedFilter();

            if(filter == "All" || filter == "Posts")
            {
                addPosts(i_User, items);
            }

            if(filter == "All" || filter == "Photos")
            {
                addPhotos(i_User, items);
            }

            return items;
        }

        private string getSelectedFilter()
        {
            return comboBoxContent != null && comboBoxContent.SelectedItem != null
                       ? comboBoxContent.SelectedItem.ToString()
                       : "All";
        }

        private void addPosts(User i_User, List<TimelineItem> i_TimelineItems)
        {
            try
            {
                foreach(Post post in i_User.Posts)
                {
                    DateTime? created = tryGetCreatedTime(post);
                    if(created.HasValue)
                    {
                        i_TimelineItems.Add(
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

        private void addPhotos(User i_User, List<TimelineItem> i_TimelineItems)
        {
            addTaggedPhotos(i_User, i_TimelineItems);
            addAlbumPhotos(i_User, i_TimelineItems);
        }

        private void addTaggedPhotos(User i_User, List<TimelineItem> i_TimelineItems)
        {
            try
            {
                foreach(Photo photo in i_User.PhotosTaggedIn)
                {
                    DateTime? created = tryGetCreatedTime(photo);
                    if(created.HasValue)
                    {
                        i_TimelineItems.Add(
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

        private void addAlbumPhotos(User i_User, List<TimelineItem> i_TimelineItems)
        {
            try
            {
                foreach(Album album in i_User.Albums)
                {
                    foreach(Photo photo in album.Photos)
                    {
                        DateTime? created = tryGetCreatedTime(photo);
                        if(created.HasValue)
                        {
                            i_TimelineItems.Add(
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

        private IEnumerable<TimelineItem> getSortedItems(List<TimelineItem> i_TimelineItems)
        {
            string granularity = getGranularity();

            if(granularity.Contains("Year"))
            {
                return i_TimelineItems.OrderByDescending(i => i.Created.Year).ThenByDescending(i => i.Created);
            }

            if(granularity.Contains("Month"))
            {
                return i_TimelineItems.OrderByDescending(i => i.Created.Year).ThenByDescending(i => i.Created.Month)
                    .ThenByDescending(i => i.Created);
            }

            DateTime birth;
            if(granularity.Contains("Age") && tryParseBirthday(out birth))
            {
                return i_TimelineItems.OrderByDescending(i => getAge(i.Created, birth))
                    .ThenByDescending(i => i.Created);
            }

            return i_TimelineItems.OrderByDescending(i => i.Created);
        }

        private string getGranularity()
        {
            return comboBoxGranularity != null && comboBoxGranularity.SelectedItem != null
                       ? comboBoxGranularity.SelectedItem.ToString()
                       : "Timeline by date";
        }

        private bool tryParseBirthday(out DateTime i_Birth)
        {
            i_Birth = DateTime.MinValue;
            string birthday = m_LoginResult != null && m_LoginResult.LoggedInUser != null
                                  ? m_LoginResult.LoggedInUser.Birthday
                                  : null;

            if(string.IsNullOrEmpty(birthday))
            {
                return false;
            }

            return DateTime.TryParse(birthday, out i_Birth);
        }

        private int getAge(DateTime i_Created, DateTime i_Birth)
        {
            int age = i_Created.Year - i_Birth.Year;
            if(i_Created < i_Birth.AddYears(age))
            {
                age--;
            }

            return age;
        }

        private void fillListView(IEnumerable<TimelineItem> i_TimelineItems)
        {
            bool hasBirthday = tryParseBirthday(out DateTime birthDate);
            string granularity = getGranularity();

            foreach(TimelineItem item in i_TimelineItems)
            {
                string dateText = formatDateText(item, granularity, hasBirthday, birthDate);
                string summary = formatSummary(item.Summary);

                addListViewItem(item, dateText, summary);
            }
        }

        private string formatDateText(
            TimelineItem i_TimelineItem,
            string i_Granularity,
            bool i_HasBirthday,
            DateTime i_BirthDate)
        {
            if(i_Granularity.Contains("Year"))
            {
                return i_TimelineItem.Created.Year.ToString();
            }

            if(i_Granularity.Contains("Month"))
            {
                return i_TimelineItem.Created.ToString("MMM yyyy");
            }

            if(i_Granularity.Contains("Age") && i_HasBirthday)
            {
                int age = getAge(i_TimelineItem.Created, i_BirthDate);
                return age >= 0 ? age + "y" : "Unknown";
            }

            return i_TimelineItem.Created.ToString("g");
        }

        private string formatSummary(string i_Summary)
        {
            if(string.IsNullOrEmpty(i_Summary))
            {
                return string.Empty;
            }

            return i_Summary.Length > 100 ? i_Summary.Substring(0, 100) + "..." : i_Summary;
        }

        private void addListViewItem(TimelineItem i_TimelineItem, string i_DateText, string i_Summary)
        {
            ListViewItem listItem = new ListViewItem(i_DateText);
            listItem.SubItems.Add(i_TimelineItem.Type);
            listItem.SubItems.Add(i_Summary);
            listItem.Tag = i_TimelineItem.SourceObject;
            listViewTimeline.Items.Add(listItem);
        }

        private void ListViewTimeline_DoubleClick(object i_Sender, EventArgs i_EventArgs)
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

        private void ListViewTimeline_SelectedIndexChanged(object i_Sender, EventArgs i_EventArgs)
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

        private string getMediaUrl(object i_Item)
        {
            if(i_Item == null)
            {
                return null;
            }

            string[] props = { "PictureNormalURL", "FullPicture", "PictureURL", "Picture", "Source", "Image" };
            Type t = i_Item.GetType();

            foreach(string propName in props)
            {
                PropertyInfo prop = t.GetProperty(propName);
                if(prop != null)
                {
                    string value = prop.GetValue(i_Item) as string;
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

        private void showMedia(string i_Url)
        {
            if(string.IsNullOrEmpty(i_Url))
            {
                clearPreview();
                return;
            }

            if(!isImageUrl(i_Url))
            {
                showNonImageMedia(i_Url);
                return;
            }

            clearPreview();
            loadImage(i_Url);
        }

        private void showNonImageMedia(string i_Url)
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
                webBrowserPreview.Navigate(i_Url);
                webBrowserPreview.Visible = true;
            }
        }

        private void loadImage(string i_Url)
        {
            try
            {
                setPreviewVisibilityForImage();

                using(WebClient webClient = new WebClient())
                {
                    byte[] data = webClient.DownloadData(i_Url);
                    using(MemoryStream memoryStream = new MemoryStream(data))
                    {
                        Image img = Image.FromStream(memoryStream);
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

        private bool isImageUrl(string i_Url)
        {
            if(string.IsNullOrEmpty(i_Url))
            {
                return false;
            }

            string lower = i_Url.ToLowerInvariant();
            return lower.EndsWith(".jpg") || lower.EndsWith(".jpeg") || lower.EndsWith(".png") || lower.EndsWith(".gif")
                   || lower.EndsWith(".bmp");
        }

        private DateTime? tryGetCreatedTime(object i_Item)
        {
            if(i_Item == null)
            {
                return null;
            }

            string[] dateProps = { "CreatedTime", "StartTime", "UpdatedTime" };
            Type t = i_Item.GetType();

            foreach(string propName in dateProps)
            {
                PropertyInfo prop = t.GetProperty(propName);
                if(prop != null)
                {
                    object value = prop.GetValue(i_Item);
                    if(value is DateTime)
                    {
                        return (DateTime)value;
                    }

                    DateTime dateTime;
                    if(value is string && DateTime.TryParse((string)value, out dateTime))
                    {
                        return dateTime;
                    }
                }
            }

            return null;
        }

        private string tryGetSummary(object i_Item)
        {
            if(i_Item == null)
            {
                return "Post";
            }

            string[] textProps = { "Message", "Caption", "Name", "Description" };
            Type t = i_Item.GetType();

            foreach(string propName in textProps)
            {
                PropertyInfo prop = t.GetProperty(propName);
                if(prop != null)
                {
                    string value = prop.GetValue(i_Item) as string;
                    if(!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
            }

            return "Post";
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

        private class TimelineItem
        {
            public DateTime Created { get; set; }

            public string Type { get; set; }

            public string Summary { get; set; }

            public object SourceObject { get; set; }
        }
    }
}