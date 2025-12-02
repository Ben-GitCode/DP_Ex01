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
        private readonly bool r_IsDarkMode;
        private readonly LoginResult r_LoginResult;

        public FormTimeline(LoginResult i_LoginResult)
        {
            InitializeComponent();
            r_LoginResult = i_LoginResult;
            Load += FormTimeline_Load;
            if(listViewTimeline != null)
            {
                listViewTimeline.SelectedIndexChanged += ListViewTimeline_SelectedIndexChanged;
                listViewTimeline.DoubleClick += ListViewTimeline_DoubleClick;
            }
        }

        public FormTimeline(LoginResult i_LoginResult, bool i_IsDarkMode)
            : this(i_LoginResult)
        {
            r_IsDarkMode = i_IsDarkMode;
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


        private void FormTimeline_Load(object i_Sender, EventArgs i_EventArgs)
        {
            applyDarkMode();
            AdjustColumns();
            if(r_LoginResult != null && r_LoginResult.LoggedInUser != null)
            {
                populateTimeline();
            }
        }

        private void applyDarkMode()
        {
            Color formBack = r_IsDarkMode ? Color.FromArgb(24, 25, 26) : SystemColors.Control;
            Color panelBack = r_IsDarkMode ? Color.FromArgb(36, 37, 38) : SystemColors.Control;
            Color text = r_IsDarkMode ? Color.White : Color.Black;
            Color listBack = r_IsDarkMode ? Color.FromArgb(24, 25, 26) : Color.White;
            Color listFore = text;
            Color headerBack = r_IsDarkMode ? Color.FromArgb(45, 60, 100) : Color.FromArgb(59, 89, 152);
            Color back = Color.FromArgb(66, 103, 178);

            BackColor = formBack;

            applyHeaderTheme(headerBack, text);
            applyPanelsTheme(panelBack, text);
            applyListTheme(listBack, listFore);
            applyPreviewTheme(panelBack);
            applyButtonsTheme(back);
            applyCombosTheme(listBack, listFore);
        }

        private void applyHeaderTheme(Color i_HeaderBack, Color i_TextColor)
        {
            if(topPanel == null)
            {
                return;
            }

            topPanel.BackColor = i_HeaderBack;

            foreach(Control control in topPanel.Controls)
            {
                if(control is Label || control is LinkLabel)
                {
                    control.ForeColor = Color.White;
                }
                else
                {
                    control.ForeColor = i_TextColor;
                }
            }
        }

        private void applyPanelsTheme(Color i_PanelBackColor, Color i_TextColor)
        {
            if(leftPanel != null)
            {
                leftPanel.BackColor = i_PanelBackColor;
                leftPanel.ForeColor = i_TextColor;
            }

            if(rightPanel != null)
            {
                rightPanel.BackColor = i_PanelBackColor;
                rightPanel.ForeColor = i_TextColor;
            }
        }

        private void applyListTheme(Color i_ListBackColor, Color i_ListForeColor)
        {
            if(listViewTimeline != null)
            {
                listViewTimeline.BackColor = i_ListBackColor;
                listViewTimeline.ForeColor = i_ListForeColor;
            }
        }

        private void applyPreviewTheme(Color i_PanelBackColor)
        {
            if(placeholderLabel != null)
            {
                placeholderLabel.ForeColor = r_IsDarkMode ? Color.Gainsboro : Color.DimGray;
                placeholderLabel.BackColor = Color.Transparent;
            }

            if(pictureBoxPreview != null)
            {
                pictureBoxPreview.BackColor = r_IsDarkMode ? Color.Black : SystemColors.ControlDark;
            }

            if(webBrowserPreview != null)
            {
                webBrowserPreview.BackColor = i_PanelBackColor;
            }
        }

        private void applyButtonsTheme(Color i_ButtonBackColor)
        {
            if(i_ButtonBackColor != null)
            {
                if(buttonBack != null)
                {
                    buttonBack.ForeColor = Color.White;
                    if(buttonBack.BackColor == SystemColors.Control || buttonBack.BackColor.A == 0)
                    {
                        buttonBack.BackColor = i_ButtonBackColor;
                    }

                    buttonBack.FlatStyle = FlatStyle.Flat;
                }

                if(buttonRefresh != null)
                {
                    buttonRefresh.ForeColor = Color.White;
                    if(buttonRefresh.BackColor == SystemColors.Control || buttonRefresh.BackColor.A == 0)
                    {
                        buttonRefresh.BackColor = i_ButtonBackColor;
                    }

                    buttonRefresh.FlatStyle = FlatStyle.Flat;
                }
            }
        }

        private void applyCombosTheme(Color i_ListBackColor, Color i_ListForeColor)
        {
            if(comboBoxContent != null)
            {
                comboBoxContent.BackColor = i_ListBackColor;
                comboBoxContent.ForeColor = i_ListForeColor;
                comboBoxContent.FlatStyle = FlatStyle.Standard;
            }

            if(comboBoxGranularity != null)
            {
                comboBoxGranularity.BackColor = i_ListBackColor;
                comboBoxGranularity.ForeColor = i_ListForeColor;
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

                    User user = r_LoginResult != null ? r_LoginResult.LoggedInUser : null;
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
            string birthday = r_LoginResult != null && r_LoginResult.LoggedInUser != null
                                  ? r_LoginResult.LoggedInUser.Birthday
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