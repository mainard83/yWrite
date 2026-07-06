using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace yWrite_UWP
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Load the HTML file as a string and display it
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///index.html"));
            var html = await FileIO.ReadTextAsync(file);
            webView.NavigateToString(html);
            webView.Focus(FocusState.Programmatic);
            this.KeyDown += OnKeyDown;
        }

        private async void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            var ctrlState = Windows.UI.Core.CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Control);
            if ((ctrlState & Windows.UI.Core.CoreVirtualKeyStates.Down) == Windows.UI.Core.CoreVirtualKeyStates.Down)
            {
                switch (e.Key)
                {
                    case VirtualKey.N:
                        e.Handled = true;
                        await webView.InvokeScriptAsync("eval", new[] { "document.getElementById('wrap').innerText = '';" });
                        await webView.InvokeScriptAsync("eval", new[] { "document.getElementById('wrap').dispatchEvent(new Event('input'));" });
                        break;
                    case VirtualKey.O:
                        e.Handled = true;
                        await OpenFile();
                        break;
                    case VirtualKey.S:
                        e.Handled = true;
                        await SaveFile();
                        break;
                }
            }
        }

        private async Task OpenFile()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add(".md");
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var text = await FileIO.ReadTextAsync(file);
                var escaped = text.Replace("`", "\\`").Replace("${", "\\${");
                await webView.InvokeScriptAsync("eval", new[] { $"document.getElementById('wrap').innerText = `{escaped}`;" });
                await webView.InvokeScriptAsync("eval", new[] { "document.getElementById('wrap').dispatchEvent(new Event('input'));" });
            }
        }

        private async Task SaveFile()
        {
            var picker = new FileSavePicker();
            picker.FileTypeChoices.Add("Text", new[] { ".txt" });
            picker.FileTypeChoices.Add("Markdown", new[] { ".md" });
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.SuggestedFileName = "document";
            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                var content = await webView.InvokeScriptAsync("eval", new[] { "document.getElementById('wrap').innerText" });
                await FileIO.WriteTextAsync(file, content);
            }
        }
    }
}
