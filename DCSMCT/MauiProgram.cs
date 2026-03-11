using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif
using MudBlazor;
using Serilog;
using Serilog.Events;
using MudBlazor.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using MudExtensions.Services;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Services;
using Blazored.LocalStorage;
using SampleForMS.Components.Interface;
using SampleForMS.Components.Controller;


namespace DCSMCT
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            string errorMessage = "";
            var builder = MauiApp.CreateBuilder();
            string? FileToOpen = null;

            try
            {
                builder
                .UseMauiCommunityToolkit()
                .UseMauiApp<App>()
                .ConfigureLifecycleEvents(ev =>
                {
                    //ev.AddWindows(windows => windows.OnLaunched((window, args) => GetCalledArgs(args)));
                    ev.AddWindows(windows => windows.OnLaunched((window, args) =>
                        {
                            var goodArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
                            if (goodArgs.Data != null)
                            {
                                var data = goodArgs.Data as IFileActivatedEventArgs;
                                if (data != null && data.Files != null && data.Files.Any())
                                {
                                    
                                }
                            }
                        })
                    );

                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
                builder.Services.AddMauiBlazorWebView();
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
            }

            // Logging
            var flushInterval = new TimeSpan(0, 0, 1);
            var file = Path.Combine(FileSystem.AppDataDirectory, "App.log");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Debug()
                .WriteTo.File(file, shared: true, flushToDiskInterval: flushInterval, encoding: System.Text.Encoding.UTF8, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 22)
                .CreateLogger();
            builder.Logging.AddDebug();
            builder.Logging.AddSerilog(dispose: true);
            builder.Services.AddLogging(logging => logging.AddSerilog(dispose: true));
            builder.Services.AddBlazoredLocalStorage();

            if (!string.IsNullOrEmpty(errorMessage))
            {
                Log.Error(errorMessage);
            }
            else
            {
                Log.Information("App Logging has started");
            }


            try
            {
                // Set system Number localisations and possibly context menus
                Log.Information("Setting system localisation");
                CultureInfo culture = new CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.Name, true);
                culture.NumberFormat.NumberDecimalSeparator = ".";
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                Microsoft.AspNetCore.Components.WebView.Maui.BlazorWebViewHandler.BlazorWebViewMapper.AppendToMapping("AddContextMenuBack", async (handler, view) => {

#if WINDOWS

                    await handler.PlatformView.EnsureCoreWebView2Async(); handler.PlatformView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
#endif

#if DEBUG && WINDOWS
                    handler.PlatformView.CoreWebView2.Settings.AreDevToolsEnabled = true;
#else
                    handler.PlatformView.CoreWebView2.Settings.AreDevToolsEnabled = false;
#endif

                });
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            // Load appsettings.json
            var a = Assembly.GetExecutingAssembly();
            using var appsettingsStream = a.GetManifestResourceStream("DCSMCT.appsettings.json");
            if (appsettingsStream != null)
            {
                var config = new ConfigurationBuilder()
                    .AddJsonStream(appsettingsStream)
                    .Build();

                builder.Configuration.AddConfiguration(config);
            }

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif

           
            try
            {
                // Init DB
                builder.Services.AddScoped<ITestController, TestController>();

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Error("Unable to publish DB changes - you likely need to remove the existing DB!");
            }


#if WINDOWS
            builder.Services.AddDataProtection().UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
            {
                EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            }).SetApplicationName("SampleApp");

            try
            {
                builder.ConfigureLifecycleEvents(events =>
                {
                    events.AddWindows(wndLifeCycleBuilder =>
                    {
                        wndLifeCycleBuilder.OnWindowCreated(window =>
                        {
                            IntPtr nativeWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                            WindowId win32WindowsId = Win32Interop.GetWindowIdFromWindow(nativeWindowHandle);
                            AppWindow winuiAppWindow = AppWindow.GetFromWindowId(win32WindowsId);
                            if (winuiAppWindow.Presenter is OverlappedPresenter p)
                                p.Maximize();
                            else
                            {
                                const int width = 1200;
                                const int height = 800;
                                winuiAppWindow.MoveAndResize(new RectInt32(1920 / 2 - width / 2, 1080 / 2 - height / 2, width, height));
                            }
                        });
                    });
                });

                builder.ConfigureLifecycleEvents(lifecycle =>
                 {
                     lifecycle.AddWindows((builder) =>
                     {
                         builder.OnWindowCreated(del =>
                         {
                             del.Title = "MCT";
                         });
                     });
                 });
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
            }


#endif


            try
            {

                builder.Services.AddMudServices(config =>
                {
                    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                    config.SnackbarConfiguration.PreventDuplicates = false;
                    config.SnackbarConfiguration.NewestOnTop = true;
                    config.SnackbarConfiguration.ShowCloseIcon = true;
                    config.SnackbarConfiguration.VisibleStateDuration = 10000;
                    config.SnackbarConfiguration.HideTransitionDuration = 500;
                    config.SnackbarConfiguration.ShowTransitionDuration = 500;
                    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
                });


                builder.Services.AddMudExtensions();
                builder.Services.AddScoped<IMudExDialogService, MudExDialogService>();
                builder.Services.AddScoped<IDialogEventService, DialogEventService>();

                builder.Services.AddScoped<MudExAppearanceService>();

                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddRoutingCore();

                var sp = builder.Services.BuildServiceProvider();
                AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
                {
                    if (e != null && e.Exception != null)
                    {
                        string _o = e.Exception.ToString();
                        if (!_o.Contains("System.Net.Sockets.SocketException") && !_o.Contains("MudBlazor.Utilities.Exceptions.ConversionException"))
                        {
                            Log.Error(_o);
                        }
                    }
                };

                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    if (e != null)
                    {
                        string? _o = e.ExceptionObject.ToString();
                        if (_o != null)
                        {
                            Log.Error(_o);
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            var app = builder.Build();

            return app;
        }
    }
}
