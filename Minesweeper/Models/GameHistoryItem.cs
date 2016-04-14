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

namespace Minesweeper.Models
{
    public class GameHistoryItem
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public int Mine { get; set; }
        public int PlayTimes { get; set; }
        public int Score { get; set; }
        public GameLevel Level { get; set; }
        public DateTime Date { get; internal set; }
    }
}