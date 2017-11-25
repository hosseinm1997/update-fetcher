using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Net;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json.Linq;
using System.IO;

namespace FetchBotUpdate
{
    public partial class Form1 : Form
    {
        string setting_file_address = Application.StartupPath + "\\fetcher_setting.json";
        bool hidden = false;
        byte max_reconnect_time_seconds = 30;
        byte reconnect_times=0;
        bool fetching = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            using (WebClient client = new WebClient())
            {
                try
                {

                    string get_update_api = "https://api.telegram.org/bot" + txtToken.Text.Trim() + "/getUpdates";
                    byte[] response =
                    client.UploadValues(get_update_api, new NameValueCollection()
                   {
                       { "offset", (Convert.ToInt64(txtOffset.Text) + 1).ToString() },
                       //{ "favorite+flavor", "flies" }
                   });

                    string result = System.Text.Encoding.UTF8.GetString(response);

                    //dynamic updates = JObject.Parse("{\"ok\":true,\"result\":[{\"update_id\":124749113, \"message\":{\"message_id\":419,\"from\":{\"id\":203712431,\"is_bot\":false,\"first_name\":\"Ho\u00a7einM\",\"username\":\"hosseinm1997\",\"language_code\":\"en-US\"},\"chat\":{\"id\":203712431,\"first_name\":\"Ho\u00a7einM\",\"username\":\"hosseinm1997\",\"type\":\"private\"},\"date\":1510132178,\"text\":\"/start\",\"entities\":[{\"offset\":0,\"length\":6,\"type\":\"bot_command\"}]}},{\"update_id\":124749114, \"message\":{\"message_id\":420,\"from\":{\"id\":203712431,\"is_bot\":false,\"first_name\":\"Ho\u00a7einM\",\"username\":\"hosseinm1997\",\"language_code\":\"en-US\"},\"chat\":{\"id\":203712431,\"first_name\":\"Ho\u00a7einM\",\"username\":\"hosseinm1997\",\"type\":\"private\"},\"date\":1510138405,\"text\":\"/start\",\"entities\":[{\"offset\":0,\"length\":6,\"type\":\"bot_command\"}]}},{\"update_id\":124749115, \"message\":{\"message_id\":421,\"from\":{\"id\":203712431,\"is_bot\":false,\"first_name\":\"Ho\u00a7einM\",\"username\":\"hosseinm1997\",\"language_code\":\"en-US\"},\"chat\":{\"id\":203712431,\"first_name\":\"Ho\u00a7einM\",\"username\":\"hosseinm1997\",\"type\":\"private\"},\"date\":1510138405,\"text\":\"/start\",\"entities\":[{\"offset\":0,\"length\":6,\"type\":\"bot_command\"}]}},{\"update_id\":124749116, \"message\":{\"message_id\":422,\"from\":{\"id\":203712431,\"is_bot\":false,\"first_name\":\"Ho\u00a7einM\",\"username\":\"hosseinm1997\",\"language_code\":\"en-US\"},\"chat\":{\"id\":203712431,\"first_name\":\"Ho\u00a7einM\",\"username\":\"hosseinm1997\",\"type\":\"private\"},\"date\":1510138406,\"text\":\"/start\",\"entities\":[{\"offset\":0,\"length\":6,\"type\":\"bot_command\"}]}},{\"update_id\":124749117, \"message\":{\"message_id\":423,\"from\":{\"id\":203712431,\"is_bot\":false,\"first_name\":\"Ho\u00a7einM\",\"username\":\"hosseinm1997\",\"language_code\":\"en-US\"},\"chat\":{\"id\":203712431,\"first_name\":\"Ho\u00a7einM\",\"username\":\"hosseinm1997\",\"type\":\"private\"},\"date\":1510140995,\"text\":\"/start\",\"entities\":[{\"offset\":0,\"length\":6,\"type\":\"bot_command\"}]}},{\"update_id\":124749118, \"message\":{\"message_id\":424,\"from\":{\"id\":203712431,\"is_bot\":false,\"first_name\":\"Ho\u00a7einM\",\"username\":\"hosseinm1997\",\"language_code\":\"en-US\"},\"chat\":{\"id\":203712431,\"first_name\":\"Ho\u00a7einM\",\"username\":\"hosseinm1997\",\"type\":\"private\"},\"date\":1510140996,\"text\":\"/start\",\"entities\":[{\"offset\":0,\"length\":6,\"type\":\"bot_command\"}]}}]}");
                    dynamic updates = JObject.Parse(result);
                    if (updates["result"].Count > 0)
                    {
                        foreach (var item in updates["result"])
                        {
                            dynamic a = JsonConvert.SerializeObject(item);
                            byte[] tresponse =
                            client.UploadValues(txtUrlAddress.Text.Trim(), new NameValueCollection(){
                            {"update" , a}
                            //{ "offset", "Cosby" },
                            //{ "favorite+flavor", "flies" }
                        });
                            //string tresult = System.Text.Encoding.UTF8.GetString(response);
                        }

                        txtOffset.Text = (string)updates["result"].Last["update_id"];
                        saveSetting();
                    }
                    disableReconnectTimer();
                    if(!timer1.Enabled)
                        timer1.Enabled = true;

                    if(!fetching)
                        lblStatus.Text = "Fetching";

                }
                catch (WebException webexception)
                {
                    timer1.Enabled = false;
                    enableReconnectTimer();
                    reconnect_times++;
                    if (reconnect_times == max_reconnect_time_seconds)
                    {
                        disableReconnectTimer();

                        if (hidden)
                        {
                            this.Show();
                            this.WindowState = FormWindowState.Normal;
                            hidden = !hidden;
                        }

                        if (MessageBox.Show("There is no internet connection After " + max_reconnect_time_seconds.ToString() + " seconds trying. Check your network\nTry reconnect again?", "Connection failed", MessageBoxButtons.YesNo,MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
                        {
                            enableReconnectTimer();
                        }
                        else
                        {
                            btnStopFetch_Click(null, null);
                        }
                    }
                }
            }
        }

        private void enableReconnectTimer()
        {
            if (!timReconnect.Enabled)
            {
                reconnect_times = 0;
                timReconnect.Enabled = true;
                fetching = false;
                lblStatus.Text = "Trying reconnecting...";
            }
        }

        private void disableReconnectTimer()
        {
            if (timReconnect.Enabled)
            {
                timReconnect.Enabled = false;
            }
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            if (txtToken.Text.Trim() == "" || txtUrlAddress.Text.Trim() == "" || txtInterval.Text.Trim() == "")
            {
                MessageBox.Show("Please fill all text fields before start","Empty Fields",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            timer1.Interval = int.Parse(txtInterval.Text.Trim());
            timer1.Enabled = true;

            txtToken.Enabled = false;
            txtUrlAddress.Enabled = false;
            txtInterval.Enabled = false;
            btnFetch.Enabled = false;
            btnStopFetch.Enabled = true;

            fetching = !fetching;
            lblStatus.Text = "Fetching";
        }

        private void btnStopFetch_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timReconnect.Enabled = false;

            txtToken.Enabled = true;
            txtUrlAddress.Enabled = true;
            txtInterval.Enabled = true;
            btnFetch.Enabled = true;
            btnStopFetch.Enabled = false;

            fetching = !fetching;
            lblStatus.Text = "Stopped";
        }

        private void saveSetting()
        {
            JObject o = new JObject();
            o["last_token"] = txtToken.Text;
            o["last_url"] = txtUrlAddress.Text;
            o["last_id"] = txtOffset.Text;
            o["last_interval"] = txtInterval.Text;
            StreamWriter sw = new StreamWriter(setting_file_address);
            sw.Write(o.ToString());
            sw.Close();
        }

        private void readSetting()
        {
            if (File.Exists(setting_file_address))
            {
                StreamReader sr = new StreamReader(setting_file_address);
                dynamic o = JsonConvert.DeserializeObject(sr.ReadToEnd());
                sr.Close();

                txtToken.Text = (string) o["last_token"];
                txtUrlAddress.Text = (string) o["last_url"];
                txtOffset.Text = (string) o["last_id"];
                txtInterval.Text = (string) o["last_interval"];

                //txtOffset.Text = (Int64.Parse(txtOffset.Text) + 1).ToString();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            readSetting();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (hidden)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.Hide();
            }
            hidden = !hidden;

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                hidden = !hidden;
            }
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "show")
                notifyIcon1_DoubleClick(null, null);
            else
                this.Hide();

            hidden = !hidden;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (timer1.Enabled)
            {
                MessageBox.Show("Fetching is in progress. Stop it before exit", "Warning",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                e.Cancel = true;
            }
        }

        private void timReconnect_Tick(object sender, EventArgs e)
        {
            timer1_Tick(sender, e);
        }

        private void lblStatus_TextChanged(object sender, EventArgs e)
        {
            notifyIcon1.Text = "Status: " + lblStatus.Text;
            notifyIcon1.ShowBalloonTip(1000, "Status" ,lblStatus.Text,ToolTipIcon.Info);
        }
    }
}
