using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.IO;

namespace HttpClient
{
    public partial class Form1 : Form
    {
        public TcpClient TcpHandle;
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            string request = "";
            string uri = getUri(textBox2.Text);
            string host = getHostName(textBox2.Text);
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    request = GETMethod(host, uri);
                    break;
                case 1:
                    request = POSTMethod(host, getPOSTParametrs(ref uri), uri);
                    break;
                case 2:
                    request = HEADMethod(host, uri);
                    break;
            }
            textBox1.Text += request + "\r\n";
            TcpHandle = new TcpClient(getHostName(textBox2.Text), 80);
            NetworkStream stream = TcpHandle.GetStream();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(request);
            stream.Write(data, 0, data.Length);

            var reader = new StreamReader(stream, Encoding.UTF8);
            textBox1.Text += reader.ReadToEnd();
            stream.Close();
            TcpHandle.Close();
        }


        public string getUri(string url)
        {
            Regex RegExUri = new Regex("(?<=[a-zA-Z])/.*");
            MatchCollection matches = RegExUri.Matches(url);
            string uri = "/";
            foreach (Match match in matches)
                uri = match.Value;
            return uri;
        }

        public string getHostName(string url)
        {
            Regex RegExUri = new Regex("(?<=(//)|^).+.[A-Za-z]{2,3}(?=/)");
            MatchCollection matches = RegExUri.Matches(url);
            string host = "";
            foreach (Match match in matches)
                host = match.Value;
            host = host.Replace("https://", "");
            host = host.Replace("http://", "");
            int fSlash = host.IndexOf("/");
            if (fSlash != -1)
                host = host.Substring(0, fSlash);
            return host;
        }

        public string getPOSTParametrs(ref string uri)
        {
            //(?<=\?|&)[A-Za-z0-9_-]+
            //(?<=\=)[A-Za-z0-9-_]+
            //(?<=\?).+
            Regex ParamsRegex = new Regex("(?<=[?]).+");
            MatchCollection mathces = ParamsRegex.Matches(uri);
            string POSTParams = "";
            foreach (Match match in mathces)
            {
                POSTParams = match.Value;
            }
            uri = uri.Remove(uri.Length - POSTParams.Length - 1, POSTParams.Length + 1);

            return POSTParams;
        }


        public string GETMethod(string url, string uri)
        {
            string mainOption = "{0} {1} HTTP/1.0\r\n";
            string hostOption = "Host: {0}\r\n";
            string acceptOption = "Accept: {0}\r\n";
            string connectionOption = "Connection: {0}\r\n";
            string userAgentOption = "User-Agent: {0}\r\n";
            string lastLineOption = "\r\n";

            return String.Format(mainOption, "GET", uri) + 
                   String.Format(hostOption, url) + 
                   String.Format(acceptOption, "text/html") + 
                   String.Format(connectionOption, "close") + 
                   String.Format(userAgentOption, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36") +
                   lastLineOption;
        }

        public string POSTMethod(string url, string parametrs, string uri)
        {
            string mainOption = "{0} {1} HTTP/1.0\r\n";
            string hostOption = "Host: {0}\r\n";
            string acceptOption = "Accept: {0}\r\n";
            string connectionOption = "Connection: {0}\r\n";
            string userAgentOption = "User-Agent: {0}\r\n";
            string contentTypeOption = "Content-type: {0}\r\n";
            string contentLengthOption = "Content-length: {0}\r\n";
            string lastLineOption = "\r\n";

            return String.Format(mainOption, "POST", uri) + 
                   String.Format(hostOption, url) + String.Format(acceptOption, "text/html") +
                   String.Format(contentTypeOption, "application/x-www-form-urlencoded") + 
                   String.Format(connectionOption, "keep-alive") +
                   String.Format(contentLengthOption, parametrs.Length) +
                   String.Format(userAgentOption, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36") +
                   lastLineOption + 
                   parametrs +
                   lastLineOption;

        }

        public string HEADMethod(string url, string uri)
        {
            string mainOption = "{0} {1} HTTP/1.0\r\n";
            string hostOption = "Host: {0}\r\n";
            string acceptOption = "Accept: {0}\r\n";
            string connectionOption = "Connection: {0}\r\n";
            string userAgentOption = "User-Agent: {0}\r\n";
            string lastLineOption = "\r\n";
            return String.Format(mainOption, "HEAD", uri) + 
                   String.Format(hostOption, url) + 
                   String.Format(acceptOption, "text/html") + 
                   String.Format(connectionOption, "keep-alive") + 
                   String.Format(userAgentOption, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36") +
                   lastLineOption;

        }

    }
}
