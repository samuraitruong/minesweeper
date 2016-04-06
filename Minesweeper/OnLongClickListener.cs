using System;
using Android.Views;

namespace Minesweeper
{
    public class OnLongClickListener : View.IOnLongClickListener
    {
        public IntPtr Handle
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool OnLongClick(View v)
        {
            return true;
        }

        public void Dispose()
        {
        }
    }
}