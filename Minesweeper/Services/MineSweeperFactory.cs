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
                    return new Minesweeper(18, 13, 35);
                    break;

                case GameLevel.Hard:
                    return new Minesweeper(18, 13, 45);
                    break;

                case GameLevel.VeryHard:
                    return new Minesweeper(20, 15, 75);
                    break;

                case GameLevel.ExtremHard:
                    return new Minesweeper(20, 15, 90);
                    break;

                case GameLevel.Custom:
                    return new Minesweeper(setting.Rows, setting.Cols, setting.Mines);
                    break;

            }
            throw new Exception("Unknow game setting");
        }

        public GameLevel DetermineGameLevel(int col, int row, int mine)
        {
            var percent = (mine * 1.0) / col * row;
            if(percent<0.1)
            {
                return GameLevel.Easy;
            }

            if (percent < 0.15)
            {
                return GameLevel.Normal;
            }
            if (percent < 0.2)
            {
                return GameLevel.Hard;
            }
            if (percent < 0.25)
            {
                return GameLevel.VeryHard;
            }
            return GameLevel.ExtremHard;
        }
    }
}