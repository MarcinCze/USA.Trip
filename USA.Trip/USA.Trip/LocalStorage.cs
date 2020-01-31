using Android.App;
using Android.Content;
using Android.Preferences;
using Android.Widget;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using USA.Trip.Models;

namespace USA.Trip
{
    public class LocalStorage
    {
        public Settings LocalSettings { get; protected set; }

        public LocalStorage(Context context)
        {
            Read(context);

            if (LocalSettings == null)
                Init(context);
        }
        
        protected void Init(Context context)
        {
            LocalSettings = new Settings
            {
                UpdateTimestamp = DateTime.Now,
                Budget = new Income { Cash = 0, Card = 0 },
                Expenses = new List<OutcomeEntry>()
            };
            Task.Run(() => Save(context));
        }

        protected void Read(Context context)
        {
            try
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
                string settingsFromStorageJson = prefs.GetString(Constants.LocalStorageKeyName, string.Empty);

                if (string.IsNullOrEmpty(settingsFromStorageJson))
                {
                    LocalSettings = null;
                    return;
                }

                LocalSettings = JsonConvert.DeserializeObject<Settings>(settingsFromStorageJson);
            }
            catch (Exception)
            {
                Toast.MakeText(Application.Context, $"ERROR IN {nameof(Read)}", ToastLength.Long).Show();
            }
        }

        public void Save(Context context)
        {
            try
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
                ISharedPreferencesEditor editor = prefs.Edit();
                string json = JsonConvert.SerializeObject(LocalSettings);
                editor.PutString(Constants.LocalStorageKeyName, json);
                editor.Apply();
            }
            catch (Exception)
            {
                Toast.MakeText(Application.Context, $"ERROR IN {nameof(Save)}", ToastLength.Long).Show();
            }
        }
    }
}