using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using System.Collections.Generic;
using Foundation;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ManagedAppConfigurationForms.iOS.AppConfig
{
	/// <summary>
	/// Managed app config settings callback interface.
	/// </summary>
	public interface IManagedAppConfigSettingsCallback
	{
		/// <summary>
		/// Called when key value pairs are added or modified
		/// </summary>
		/// <param name="changes">Dictionary of Changes.</param>
		void SettingsDidChange(Dictionary<string, object> changes);
	}

	/// <summary>
	/// Class to manage MDM AppConfig settings interactions.
	/// </summary>
	public class ManagedAppConfigSettings
    {
		private static System.Object processLock = new System.Object();
		private static System.Object instanceLock = new System.Object();
		static ManagedAppConfigSettings instance;

		/// <summary>
		/// The callback.
		/// </summary>
		public IManagedAppConfigSettingsCallback callback;

		ManagedAppConfigSettings()
		{
		}

		/// <summary>
		/// Return a singleton instance of ManagedAppConfigSettings
		/// </summary>
		/// <value>ManagedAppConfigSettings instance</value>
		public static ManagedAppConfigSettings ClientInstance
		{
			get
			{
				lock (instanceLock)
				{
					if (instance == null)
					{
						instance = new ManagedAppConfigSettings();
					}
				}
				return instance;
			}
		}

		/// <summary>
		/// Start the ManagedAppConfigSettings
		/// It is recommended to set the callback before calling Start ()
		/// </summary>
		public void Start()
		{
			SetDefaultsObservation(true);
			Task.Run(() => {
				CheckAppConfigChanges(callback);
			});
		}

		/// <summary>
		/// Callback when the device observes that the Defaults file changed
		/// </summary>
		/// <param name="notification">Notification.</param>
		void OnDefaultsChanged(NSNotification notification)
		{
			Task.Run(() => {
				CheckAppConfigChanges(callback);
			});
		}

		private void SetDefaultsObservation(bool enabled)
		{
			var NSUserDefaultsDidChangeNotification = (NSString)"NSUserDefaultsDidChangeNotification";
			if (enabled)
			{
				NSNotificationCenter.DefaultCenter.AddObserver(NSUserDefaultsDidChangeNotification, OnDefaultsChanged);
			}
			else
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(NSUserDefaultsDidChangeNotification);
			}
		}

		/// <summary>
		/// Retrieve the dictionary of keys stored by the MDM server
		/// </summary>
		/// <returns>dictionary of key/value pairs.</returns>
		static Dictionary<string, object> AppConfig()
		{
			NSDictionary serverConfig = NSUserDefaults.StandardUserDefaults.DictionaryForKey(ManagedAppConfigKeys.kMDM_CONFIGURATION_KEY);
			return NSDictionaryToDictionary(serverConfig);
		}

		/// <summary>
		/// Find any keys that changed as a result of the server pushing down
		/// New keys.
		/// </summary>
		/// <returns>dictionary of key/value pairs that changed or were added.</returns>
		Dictionary<string, object> CheckAppConfigChanges(IManagedAppConfigSettingsCallback theCallback)
		{
			lock (processLock)
			{
				var changes = new Dictionary<string, object>();

				// copy the keys into the result
				var newConfig = AppConfig();
				if (newConfig != null)
				{
					foreach (string key in newConfig.Keys)
					{
						changes[key] = newConfig[key];
					}
				}

				if (changes.Count > 0 && theCallback != null)
				{
					Debug.WriteLine("notify others of changes: {0}", changes);
					theCallback.SettingsDidChange(changes);
				}

				return changes;
			}
		}

		/// <summary>
		/// Helper function to convert NSDictionary to C# Dictionary
		/// </summary>
		/// <returns>C# Dictionary.</returns>
		/// <param name="originalDict">NSDictionary.</param>
		static Dictionary<string, object> NSDictionaryToDictionary(NSDictionary originalDict)
		{
			if (originalDict == null)
			{
				return null;
			}

			var returnDict = new Dictionary<string, object>();

			foreach (NSObject key in originalDict.Keys)
			{
				string keyStr = key.ToString();
				object value = originalDict.ObjectForKey(key);
				returnDict[keyStr] = value;
			}
			return returnDict;
		}
	}
}