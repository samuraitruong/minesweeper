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
using Minesweeper.Models;

namespace Minesweeper
{
    public class GameScoreManager
    {
        IGameHistory provider = null;
        public GameScoreManager() {
            this.provider = IoC.Resove<IGameHistory>();
        }
        public GameScoreManager(IGameHistory p)
        {
            this.provider = p;
        }
        public void AddGame(Minesweeper game, int times)
        {
            //var point = Calculate()
            var data = new GameHistoryItem()
            {
                Cols = game.Columns,
                Rows = game.Rows,
                Mine = game.Mines,
                PlayTimes = times,
            };

            data.Score = Calculate(data.Cols, data.Rows, data.Mine, data.PlayTimes);

            this.provider.AddAddItem(data);
        }
        private int Calculate(int col, int row, int mines, double times)
        {
            mines = Math.Max(mines, 10);
            var t = (float)times / mines;
            var point = col * row * t;
            return (int)point;
        }
    }
}