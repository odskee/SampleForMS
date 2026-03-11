using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.FileProviders;

namespace DCSMCT
{


    public class CustomFilesBlazorWebView : BlazorWebView
    {
        public override IFileProvider CreateFileProvider(string contentRootDir)
        {
            var lPhysicalFiles = new PhysicalFileProvider(FileSystem.Current.AppDataDirectory);
            return new CompositeFileProvider(lPhysicalFiles, base.CreateFileProvider(contentRootDir));
        }

    }



    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }


    }
}
