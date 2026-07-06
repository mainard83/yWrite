using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace yWrite
{
    public partial class MainForm : Form
    {
        private WebView2 webView;

        public MainForm()
        {
            InitializeComponent();
            this.Text = "yWrite";
            this.Size = new System.Drawing.Size(1100, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyPreview = true;

            webView = new WebView2
            {
                Dock = DockStyle.Fill,
                DefaultBackgroundColor = System.Drawing.Color.Black
            };
            this.Controls.Add(webView);

            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.NavigateToString(Properties.Resources.index_html); // we'll embed index.html as resource
            webView.Focus();
            this.KeyDown += OnKeyDown;
        }

        private async void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.N:
                        e.SuppressKeyPress = true;
                        await webView.ExecuteScriptAsync("document.getElementById('wrap').innerText = ''; document.getElementById('wrap').dispatchEvent(new Event('input'));");
                        break;
                    case Keys.O:
                        e.SuppressKeyPress = true;
                        await OpenFile();
                        break;
                    case Keys.S:
                        e.SuppressKeyPress = true;
                        await SaveFile();
                        break;
                }
            }
        }

        private async System.Threading.Tasks.Task OpenFile()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|Markdown files (*.md)|*.md|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string content = System.IO.File.ReadAllText(openFileDialog.FileName);
                    string escaped = content.Replace("`", "\\`").Replace("${", "\\${");
                    await webView.ExecuteScriptAsync($"document.getElementById('wrap').innerText = `{escaped}`; document.getElementById('wrap').dispatchEvent(new Event('input'));");
                }
            }
        }

        private async System.Threading.Tasks.Task SaveFile()
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|Markdown files (*.md)|*.md";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string content = await webView.ExecuteScriptAsync("document.getElementById('wrap').innerText");
                    System.IO.File.WriteAllText(saveFileDialog.FileName, content);
                }
            }
        }
    }
}
