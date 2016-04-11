using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Android.Preferences;

namespace Minesweeper
{
    public class AppManager
    {
        private const string SETTING_KEY_NAME = "GAME_SETTING";

        public static AppSetting LoadPreferences(Context ctx)
        {
            try
            {


                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(ctx);
                string xml = prefs.GetString(SETTING_KEY_NAME, string.Empty);
                if (string.IsNullOrEmpty(xml)) return new AppSetting() { Level = GameLevel.Easy };
                return xml.DeserializeAsXml<AppSetting>();
            }catch (Exception ex)
            {
            }
            return new AppSetting() { Level = GameLevel.Easy };
        }

        public static void SaveSetting(AppSetting settings, Context ctx)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(ctx);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString(SETTING_KEY_NAME, settings.SerializeAsXml());
            editor.Apply();
        }



    }
}