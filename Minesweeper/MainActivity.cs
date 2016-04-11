using System;
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
using Android.Support.V4.View;

namespace Minesweeper
{
    [Activity(Label = "Minesweeper", MainLauncher = true, Icon = "@drawable/icon32x32")]
    public class MainActivity : AppCompatActivity
    {
        int bestRecord = int.MaxValue;

        Minesweeper sweeper = new Minesweeper(13, 10, 6);
        GridView gridView = null;
        IMenu mainMenu = null;
        IMenuItem mnuStatus = null;
        Button flagCount = null;
        Stopwatch watcher = new Stopwatch();
        AppSetting settings = null;
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.Main, menu);
            base.OnCreateOptionsMenu(menu);
            
            this.mainMenu = menu;
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.gameStatus:
                    this.startNewGame();
                    break;
                case Resource.Id.miSetting:
                    this.OpenSettingScreen();
                    break;
                case Resource.Id.miProfile:
                    break;
                default:
                    throw new Exception("Menu item not support !");
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void OpenSettingScreen()
        {
            var intent = new Intent(this, typeof(SettingActivity));
            //intent.PutStringArrayListExtra("phone_numbers", phoneNumbers);
            StartActivity(intent);
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            this.mnuStatus = menu.FindItem(Resource.Id.gameStatus);

            var item = MenuItemCompat.SetActionView(menu.FindItem(Resource.Id.miProfile), Resource.Layout.feed_update_count);
            View view = MenuItemCompat.GetActionView(item);
            this.flagCount = view.FindViewById<Button>(Resource.Id.notif_count);
            this.UpdateFlagCount();
            return base.OnPrepareOptionsMenu(menu);
        }
        //public void DisplayFlagCount(int count)
        //{
        //    Bitmap bitmap = Bitmap.CreateBitmap(64, 64, Bitmap.Config.Argb8888);

        //    Canvas canvas = new Canvas(bitmap);
        //    Paint green = new Paint
        //    {
        //        //AntiAlias = true,
        //        TextSize = 100,
        //        Color = Color.White
        //    };
        //    //green.TextSize = 18.0f;
        //    green.SetStyle(Paint.Style.FillAndStroke);

        //    //paint.setTextSize(testTextSize);
        //    //Rect bounds = new Rect();
        //    //green.GetTextBounds(count.ToString(), 0, count.ToString().Length, bounds);

        //    //float desiredTextSize = 40f * 40  / bounds.Width();

        //    //// Set the paint for that size.
        //    //green.TextSize = desiredTextSize;


        //    float middle = canvas.Width * 0.25f;
        //    canvas.DrawText(count.ToString(), -40 ,60, green);
        //    var mnu = this.mainMenu?.FindItem(Resource.Id.flagedCount);
        //    mnu?.SetIcon(new BitmapDrawable(bitmap));
        //    //mnu?.SetTitle(count.ToString());
        //}
        private int ConvertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
            return dp;
        }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            this.settings = LoadPreferences(this);

            var metrics = Resources.DisplayMetrics;
            var widthInDp = ConvertPixelsToDp(metrics.WidthPixels);
            var heightInDp = ConvertPixelsToDp(metrics.HeightPixels);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            gridView = FindViewById<GridView>(Resource.Id.gridView1);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.MenuItemClick += Toolbar_MenuItemClick;

            SetSupportActionBar(toolbar);
            SupportActionBar.SetLogo(Resource.Drawable.icon32x32);
            SupportActionBar.Title = GetText(Resource.String.ApplicationName);


            MineSweeperFactory factory = new MineSweeperFactory();
            sweeper = factory.Create(this.settings);
            sweeper.NewGame();

            int colWidth = metrics.WidthPixels / sweeper.Columns;
            gridView.NumColumns = sweeper.Columns;
            gridView.Adapter = new MinesweeperAdapter(this, sweeper, colWidth);
            gridView.ItemClick += this.Gridview_ItemClick;
            gridView.LongClickable = true;
            gridView.ItemLongClick += Gridview_ItemLongClick;
        }

        private void Gridview_ItemClick(object sender, AdapterView.ItemClickEventArgs args)
        {

            if (sweeper.IsEndGame) return;

            if (!watcher.IsRunning)
            {
                watcher.Start();
                //change emotion 
                SetSmileIcon(Resource.Drawable.smiley_worrying);

            }

            var imageView = args.View as ImageView;

            this.sweeper.Open(args.Position, (pos, data) =>
            {
                this.UpdateMinePlace(pos, data, gridView);
            });
            if (sweeper.IsDeadPoint(args.Position))
            {
                this.ShowMessage("You dead !!!", GetText(Resource.String.DeadMessage), gridView);
                this.SetSmileIcon(Resource.Drawable.smiley_angry);
                RevealMineField();
            }
            else {
                if (sweeper.IsWin())
                {
                    watcher.Stop();
                    int sec = (int)watcher.ElapsedMilliseconds / 1000;
                    bestRecord = Math.Min(sec, bestRecord);
                    SetSmileIcon(Resource.Drawable.smiley_happy);
                    RevealMineField();
                    ShowMessage("You Won", $"Your record : {sec} seconds\n Your best record: {bestRecord}", gridView);
                }
            }
            this.UpdateFlagCount();
        }

        private void Toolbar_MenuItemClick(object sender, Toolbar.MenuItemClickEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SetSmileIcon(int resourceId)
        {
            this.mnuStatus?.SetIcon(resourceId);
        }

        private void BtnNewGame_Click(object sender, EventArgs e)
        {
            startNewGame();
        }

        private void startNewGame()
        {
            SetSmileIcon(Resource.Drawable.smiley_waiting);
            this.flagCount?.SetText(sweeper.RemainFlags.ToString(), TextView.BufferType.Normal);
            if (watcher.IsRunning) watcher.Reset();
            else
            {
                watcher.Start();
            }
            sweeper.Visit((pos, data) =>
            {
                SetTileImage(pos, gridView, "tile");
            });
        }

        private void UpdateMinePlace(int pos, MinePlace data, GridView gridView, bool endGame = false)
        {
            var img = "m" + (data.ArroundMines > 0 ? data.ArroundMines.ToString() : "");

            if (data.HasMine)
            {
                img = "mine_normal";
            }

            if (data.Flagged)
            {
                img = endGame ? "mine_flagged" : "flag";
            }
            if (data.Confused)
            {
                img = "confused";
            }


            if (data.DeadPoint)
            {
                img = "mine_explosion";
            }
            if (!data.Open && (!data.Flagged && !data.Confused))
            {
                img = "tile";
            }
            SetTileImage(pos, gridView, img);
        }

        private void SetTileImage(int pos, GridView gridView, string img)
        {
            var drawableImage = Resources.GetDrawable(Resources.GetIdentifier(img, "drawable", PackageName));
            var bitmap = (drawableImage as BitmapDrawable).Bitmap;
            if (bitmap != null)
            {
                var imgView = gridView.GetChildAt(pos) as ImageView;
                imgView.SetImageBitmap(bitmap);
            }
        }
        public void RevealMineField()
        {
            this.sweeper.Visit((position, data) =>
            {
                data.Open = true;
                UpdateMinePlace(position, data, this.gridView, true);
            });

        }
        void ShowMessage(string title, string message, GridView grid)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle(title);
            alert.SetMessage(message);
            alert.SetPositiveButton("Again", (senderAlert, args) =>
            {
                startNewGame();
                Toast.MakeText(this, "Play Again", ToastLength.Short).Show();
            });

            alert.SetNegativeButton("Close", (senderAlert, args) =>
            {
                Toast.MakeText(this, "Cancelled!", ToastLength.Short).Show();
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }
        
        private void Gridview_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs args)
        {
            if (sweeper.IsEndGame && sweeper.RemainFlags == 0) return;
            var imageView = args.View as ImageView;
            //imageView.SetImageResource(Resource.Drawable);
            var data = sweeper.Flag(args.Position);
            UpdateMinePlace(args.Position, data, sender as GridView);
            Toast.MakeText(this, "Flagged!!!", ToastLength.Short).Show();
            UpdateFlagCount();
        }

        private void UpdateFlagCount()
        {
            this.flagCount.SetText(this.sweeper.RemainFlags.ToString(), TextView.BufferType.Normal);
        }
    }
}

