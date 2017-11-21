using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Web;

namespace NBFeedbackExtractor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            lblExtractURL.Text = ExtractURL;
            lblProcessed.Text = string.Empty;
        }

        private string Slug
        {
            get
            {
                return txtSlug.Text.Trim();
            }
        }

        private string FeedbackPageID
        {
            get
            {
                return txtFeedbackPageID.Text.Trim();
            }
        }

        private string ExtractURL
        {
            get
            {
                return string.Format(@"https://{0}.nationbuilder.com/admin/activities/page.json?id={1}&page=", Slug, FeedbackPageID) + "{0}";
            }
        }
        private string HTMLToText(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return string.Empty;
            }
            else
            {
                return HttpUtility.HtmlDecode(Regex.Replace(html, "<.+?>", string.Empty));
            }
        }

        private string SafeQuotesWithComma(string s)
        {
            return SafeQuotes(s, true);
        }
        private string SafeQuotesNoComma(string s)
        {
            return SafeQuotes(s, false);
        }
        private string SafeQuotes(string s, bool addComma)
        {
            s = s.Trim();
            s = s.Replace("\"", "\"\"");
            s = s.Replace("\t", "");
            s = s.Replace("\n", "");
            s = s.Replace("â€˜", "'");
            s = s.Replace("â€™", "'");
            s = s.Replace("â€˜", "'");
            s = s.Trim();
            s = "\"" + s + "\"";
            if (addComma)
            {
                s = s + ",";
            }
            return s;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string outputFile = @"C:\Users\chris_7e1obwo\Desktop\MyNewTextFile.csv";
            saveFileDialog1.Filter = "csv files (*.csv)|*.csv|txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.DefaultExt = ".csv";
            saveFileDialog1.OverwritePrompt = true;
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                //Don't do anything
            }
            else
            {
                outputFile = saveFileDialog1.FileName;
                txtEmail.Enabled = false;
                txtPassword.Enabled = false;
                txtSlug.Enabled = false;
                txtFeedbackPageID.Enabled = false;
                button1.Enabled = false;
                this.Invalidate();
                this.Refresh();
                try
                {
                    lblExtractURL.Text = ExtractURL;
                    lblProcessed.Text = string.Empty;
                    CookieAwareWebClient cawc = new CookieAwareWebClient();
                    string baseURL = string.Format(@"https://{0}.nationbuilder.com/", Slug);

                    // the website sets some cookie that is needed for login, and as well the 'authenticity_token' is always different
                    // NOTE: Here we need the "/new"
                    string response = cawc.DownloadString(baseURL + @"forms/user_sessions/new");

                    // parse the 'authenticity_token' and cookie is auto handled by the cookieContainer
                    string token = Regex.Match(response, "authenticity_token.+?value=\"(.+?)\"").Groups[1].Value;

                    var values = new NameValueCollection
           {
             {"authenticity_token",token},
             {"email_address",""},
             {"user_session[email]", txtEmail.Text.Trim()},
             {"user_session[password]", txtPassword.Text.Trim()},
             {"user_session[remember_me]", @"1"},
             {"commit", @"Sign in with email"},
           };

                    // NOTE: Here we MUST NOT HAVE the "/new"
                    var bResponse = cawc.UploadValues(new Uri(baseURL + @"forms/user_sessions"), "POST", values);

                    int page = 0;
                    Rootobject masterRootobject = new Rootobject();
                    masterRootobject.activities = new List<Activity>();
                    Rootobject rootobject;

                    do
                    {
                        page++;
                        string url = string.Format(ExtractURL, page.ToString());
                        lblExtractURL.Text = "Extracting: " + url;
                        lblExtractURL.Invalidate();
                        this.Invalidate();
                        this.Refresh();
                        string sjson = cawc.DownloadString(url);
                        rootobject = JsonConvert.DeserializeObject<Rootobject>(sjson);

                        foreach (Activity a in rootobject.activities)
                        {
                            a.oneliner = HTMLToText(a.oneliner);
                            a.extended = HTMLToText(a.extended);
                        }

                        masterRootobject.activities.AddRange(rootobject.activities);
                        lblProcessed.Text = string.Format("{0} Feedback Activities downloaded...", masterRootobject.activities.Count.ToString());
                    } while (rootobject.activities.Count() > 0);

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("OneLiner,Extended,IsPrivate,TimeStamp,SignupID,SignupName");
                    foreach (Activity a in masterRootobject.activities)
                    {
                        StringBuilder line = new StringBuilder();
                        line.Append(SafeQuotesWithComma(a.oneliner));
                        line.Append(SafeQuotesWithComma(a.extended));
                        line.Append(SafeQuotesWithComma(a.isPrivate.ToString()));
                        line.Append(SafeQuotesWithComma(a.timestamp.ToShortDateString() + " " + a.timestamp.ToShortTimeString()));
                        line.Append(SafeQuotesWithComma(a.relatedSignups.signup.id.ToString()));
                        line.Append(SafeQuotesNoComma(a.relatedSignups.signup.name));
                        sb.AppendLine(line.ToString());
                    }

                    System.IO.File.WriteAllText(outputFile, sb.ToString());
                    lblProcessed.Text = string.Format("{0} Feedback Activities downloaded from {1} page(s) and saved to \"{2}\".", masterRootobject.activities.Count.ToString(), (page - 1).ToString(), outputFile);
                }
                finally
                {
                    txtEmail.Enabled = true;
                    txtPassword.Enabled = true;
                    txtSlug.Enabled = true;
                    txtFeedbackPageID.Enabled = true;
                    button1.Enabled = true;
                }
            }
        }

        private void txtSlug_TextChanged(object sender, EventArgs e)
        {
            lblExtractURL.Text = ExtractURL;
        }
    }
}
