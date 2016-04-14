using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.App;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using System.Diagnostics;
using Android.Preferences;
using static Minesweeper.AppManager;
using Minesweeper.Models;

namespace Minesweeper
{
    [Activity(Label = "Score", MainLauncher = false, Icon = "@drawable/icon")]
    public class ScoreActivity : AppCompatActivity
    {
        Toolbar toolbar = null;
        ListView lstHistory = null;
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.Score, menu);
            base.OnCreateOptionsMenu(menu);
            return true;
        }
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Score);

            this.toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            SetSupportActionBar(toolbar);
            SupportActionBar.SetLogo(Resource.Drawable.icon32x32);
            SupportActionBar.Title = GetText(Resource.String.ApplicationName);
            this.lstHistory = (ListView)FindViewById(Resource.Id.lstHistory);

                // which columns map to which layout controls
            string[] fromColumns = new string[] { "name" };
            int[] toControlIDs = new int[] { Android.Resource.Id.Text1 };
            // use a SimpleCursorAdapter
            var t = IoC.Resove<IGameHistory>();

            lstHistory.Adapter = new ScoreAdapter(this, t.AllHistories());
            lstHistory.ItemClick += LstHistory_ItemClick;
        }

        private void LstHistory_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var data = (lstHistory.Adapter as ScoreAdapter).GetItem(e.Position) ;
            AppManager.SaveSetting(new AppSetting()
            {
                Level = GameLevel.Custom,
                Cols = data.Cols,
                Rows = data.Rows,
                Mines = data.Mine
            }, this);
            GoBack<MainActivity>();
        }
        private void GoBack<T>()
        {
            this.OnNavigateUp();
            var intent = new Intent(this, typeof(T));
            StartActivity(intent);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch(item.ItemId)
            {
                case Resource.Id.mnuClose:
                    GoBack<MainActivity>();
                    break;
            }
            return base.OnOptionsItemSelected(item);

        }
        private void GoBack()
        {
            this.OnNavigateUp();
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }
    }
}