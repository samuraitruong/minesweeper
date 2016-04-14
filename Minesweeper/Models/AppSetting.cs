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

namespace Minesweeper { 
    public enum GameLevel
    {
        Easy,
        Normal,
        Hard, 
        VeryHard,
        ExtremHard,
        Custom
    }
    
    [Serializable]
    public class AppSetting
    {
        public int Rows { get; set; } 
        public int Cols { get; set; }
        public int Mines { get; set; }
        public GameLevel Level{ get; set; }
   }
}