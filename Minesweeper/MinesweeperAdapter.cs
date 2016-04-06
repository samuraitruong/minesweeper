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
using Java.Lang;

namespace Minesweeper
{
    public class MinesweeperAdapter : BaseAdapter
    {
        private readonly Context context;
        private readonly Minesweeper sweeper;

        
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ImageView imageView;

            if (convertView == null)
            {
                // if it's not recycled, initialize some attributes
                imageView = new ImageView(context);
                imageView.LayoutParameters = new AbsListView.LayoutParams(24, 24);
                imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
                //imageView.SetPadding(1, 1, 1, 1);
                imageView.Tag = this.GetItem(position);
            }
            else
            {
                imageView = (ImageView)convertView;
            }
            imageView.SetImageResource(Resource.Drawable.tile);
            return imageView;
        }

        public MinesweeperAdapter(Context c, Minesweeper sweeper)
        {
            context = c;
            this.sweeper = sweeper;
        }

        public override int Count
        {
            get { return sweeper.TotalCell(); }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

    }
}