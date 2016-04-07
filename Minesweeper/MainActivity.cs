using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace Minesweeper
{
    [Activity(Label = "Minesweeper Xamarin", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        Minesweeper sweeper = new Minesweeper(10, 10, 24);
        Button btnNewGame = null;
        GridView gridView = null;

        public void ShowAlert(string str)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle(str);
            alert.SetPositiveButton("OK", (senderAlert, args) =>
            {
                // write your own set of instructions
            });

            //run the alert in UI thread to display in the screen
            RunOnUiThread(() =>
            {
                alert.Show();
            });
        }
        private int ConvertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
            return dp;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var metrics = Resources.DisplayMetrics;
            var widthInDp = ConvertPixelsToDp(metrics.WidthPixels);
            var heightInDp = ConvertPixelsToDp(metrics.HeightPixels);


            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            sweeper.NewGame();
            gridView = FindViewById<GridView>(Resource.Id.gridView1);
            //btnNewGame = FindViewById<Button>(Resource.Id.button1);

            int colWidth = metrics.WidthPixels / 10;

            gridView.NumColumns = 10;
            gridView.Adapter = new MinesweeperAdapter(this, sweeper, colWidth);

            //btnNewGame.Click += BtnNewGame_Click;
            gridView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
            {
                if (sweeper.IsEndGame) return;

                var imageView = args.View as ImageView;

                this.sweeper.Open(args.Position, (pos, data) =>
                {
                    this.UpdateMinePlace(pos, data, gridView);
                });

                if(sweeper.IsWin())
                {
                    ShowMessage("You Won", "the message here", gridView);
                }
                Toast.MakeText(this, args.Position.ToString(), ToastLength.Short).Show();
            };
            gridView.LongClickable = true;
            
            gridView.ItemLongClick += Gridview_ItemLongClick;
        }

        private void BtnNewGame_Click(object sender, EventArgs e)
        {
            startNewGame();
        }

        private void startNewGame()
        {
            sweeper.NewGame();
            sweeper.Visit((pos, data) =>
            {
                var imgView = gridView.GetChildAt(pos) as ImageView;
                imgView.SetImageResource(Resource.Drawable.tile);
            });
        }

        private void UpdateMinePlace(int pos, MinePlace data, GridView gridView)
        {
            var img = "m" + (data.ArroundMines > 0 ? data.ArroundMines.ToString() : "");

            if (data.Flagged)
            {
                img = "flag";

            }

            if (data.HasMine)
            {
                img = "boom";
                //show all mine field..
            }
            var drawableImage = Resources.GetDrawable(Resources.GetIdentifier(img, "drawable", PackageName));
            var bitmap = (drawableImage as BitmapDrawable).Bitmap;
            if (bitmap != null)
            {
                var imgView = gridView.GetChildAt(pos) as ImageView;
                imgView.SetImageBitmap(bitmap);
            }

            if (data.HasMine && !data.Flagged)
            {
                this.ShowMessage("You dead!!!", "lose message here",gridView);
            }
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
            var imageView = args.View as ImageView;

            imageView.SetImageResource(Resource.Drawable.Icon);
            var data = sweeper.Flag(args.Position);

            UpdateMinePlace(args.Position, data, sender as GridView);
            Toast.MakeText(this, "Flagged!!!", ToastLength.Short).Show();
        }


    }
}

