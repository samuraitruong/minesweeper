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
using Android.Database;
using Minesweeper.Models;

namespace Minesweeper
{
    public class ScoreAdapter : ArrayAdapter<GameHistoryItem>
    {
        Activity context;
        Action<GameHistoryItem> OnItemClick;
        public ScoreAdapter(Activity context,  List<GameHistoryItem> dataList)
            : base(context,0, dataList.ToArray())
        {
            this.context = context;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var data = GetItem(position);

            if (convertView == null)
            {
                convertView = LayoutInflater.From(this.context).Inflate(Resource.Layout.HistoryListItem, parent, false);
            }
            // Lookup view for data population
            TextView tvName = (TextView)convertView.FindViewById<TextView>(Resource.Id.tvName);
            TextView tvMine = (TextView)convertView.FindViewById<TextView>(Resource.Id.tvMine);
            TextView tvScore = (TextView)convertView.FindViewById<TextView>(Resource.Id.tvScore);
            TextView tvTime = (TextView)convertView.FindViewById<TextView>(Resource.Id.tvTime);
            // TextView tvMine = (TextView)convertView.FindViewById<TextView>(Resource.Id.tvMine);

            var imageButton = convertView.FindViewById<ImageView>(Resource.Id.imageView1);
            imageButton.Focusable = false;
            imageButton.FocusableInTouchMode = false;
            imageButton.Clickable = true;

            imageButton.Click += (sender, args) => Console.WriteLine("ImageButton {0} clicked", position);

            //// Populate the data into the template view using the data object
            tvName.Text = $"{data.Rows}x{data.Cols}";
            tvMine.Text = $"{data.Mine}";
            tvTime.Text = $"{data.PlayTimes}";
            tvScore.Text = $"{data.Score}";
            //tvHome.setText(user.hometown);
            // Return the completed view to render on screen
            return convertView;
        }

    }
}