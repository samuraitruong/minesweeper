using System;
using Android.Views;

namespace Minesweeper
{
    public class MineSweeperFactory
    {
        public Minesweeper Create(AppSetting setting)
        {

            switch (setting.Level)
            {
                case GameLevel.Easy:
                    return new Minesweeper(13, 10, 16);
                    break;

                case GameLevel.Normal:
                    return new Minesweeper(18, 13, 40);
                    break;

                case GameLevel.Hard:
                    return new Minesweeper(20, 15, 70);
                    break;

                case GameLevel.VeryHard:
                    return new Minesweeper(20, 15, 90);
                    break;

                case GameLevel.ExtremHard:
                    return new Minesweeper(20, 15, 120);
                    break;

                case GameLevel.Custom:
                    return new Minesweeper(setting.Rows, setting.Cols, setting.Mines);
                    break;

            }
            throw new Exception("Unknow game setting");
        }
    }
}