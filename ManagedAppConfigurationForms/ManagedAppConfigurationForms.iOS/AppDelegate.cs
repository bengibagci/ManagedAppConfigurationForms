using System.Collections.Generic;
using Foundation;
using ManagedAppConfigurationForms.iOS.AppConfig;
using UIKit;
using Xamarin.Forms;

namespace ManagedAppConfigurationForms.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IManagedAppConfigSettingsCallback
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            ManagedAppConfigSettings.ClientInstance.callback = this;
            ManagedAppConfigSettings.ClientInstance.Start();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public void SettingsDidChange(Dictionary<string, object> changes)
        {
            var interestingKeys = new[] { ManagedAppConfigKeys.kMDM_PageBgColor };

            foreach (string key in interestingKeys)
            {
                if (changes.ContainsKey(key))
                {
                    object value = changes[key];

                    if (value != null && (value is string || value.GetType() == typeof(NSString)))
                    {
                        var strVal = value.ToString();

                        var log = "Settings value " + key + " to " + strVal;
                        Services.LogService.GetLog(log);
                        System.Diagnostics.Debug.WriteLine(log);

                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            /*
                            if (key == ManagedAppConfigKeys.kMDM_PageBgColor)
                            {
                                App.Current.Resources[key] = strVal;
                            }
                            else
                            {
                                var color = Color.FromHex(strVal);
                                App.Current.Resources[key] = color;
                            }
                            */
                            var color = Color.FromHex(strVal);
                            App.Current.Resources[key] = color;
                        });
                    }
                    else
                    {
                        var type = value.GetType();

                        //var log = "Value for "+ key + " is not an expected type(" + type + ")";

                        var log = "Key: " + key + " is null";

                        Services.LogService.GetLog(log);
                        System.Diagnostics.Debug.WriteLine("Value for {0} is not an expected type ({1})", key, type);
                    }
                }
            }
        }
    }
}
