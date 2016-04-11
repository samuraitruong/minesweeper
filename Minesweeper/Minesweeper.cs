using System;

namespace Minesweeper
{

    public struct MinePlace
    {
        public int ArroundMines { get; set; }
        public bool Confused { get; internal set; }
        public bool DeadPoint { get; internal set; }
        public bool Flagged { get; internal set; }
        public bool HasMine { get; set; }


        public bool Open { get; set; }

    }
    public class Minesweeper
    {
        private int flags = 0;
        public Minesweeper(int r, int c, int m)
        {
            this.row = r;
            this.col = c;
            this.mines = m;
            this.flags = mines;
        }
        public int RemainFlags {
            get {
                int res = mines;
                Visit((pos, data) =>
                {
                    if (data.Flagged || data.Confused)
                    {
                        res = res - 1;
                    }
                });
                return res;
            }
        }
        private MinePlace[,] minePlaces;
        private int row;
        private int col;
        private int mines;
        
        public bool IsEndGame { get; set; }
        public int Columns { get { return this.col; } }

        public void GenereteMines()
        {

            minePlaces = new MinePlace[this.row, this.col];

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < this.col; j++)
                {
                    minePlaces[i, j] = new MinePlace();
                }
            }
            Random random = new Random();
            // generate randome mines
            for (int i = 0; i < mines; i++)
            {
                bool valid = false;
                do
                {
                    var idxR = random.Next(this.row);
                    var idxC = random.Next(this.col);
                    if (!minePlaces[idxR, idxC].HasMine)
                    {
                        valid = true;
                        minePlaces[idxR, idxC].HasMine = true;
                    }

                }
                while (!valid);
            }
        }

        public void NewGame()
        {
            this.IsEndGame = false;
            this.GenereteMines();
            this.CalcMinesArround();
            
        }
        public bool IsWin()
        {
            var count = 0;
            Visit((x, data) =>
            {
                count += !data.Open ? 1 : 0;
            });

            return count == mines;
        }
        public MinePlace Flag(int position)
        {
            var r = position / this.col;
            var c = position - (r * this.col);
            var data =  minePlaces[r, c];
            if(data.Confused)
            {
                data.Flagged = true;
                data.Confused = false;
            }
            else
            data.Flagged = !data.Flagged; //toggle flag
            minePlaces[r, c] = data;
            return data;
        }
        public int TotalCell()
        {
            return row * col;
        }
        public MinePlace MineAt(int position)
        {
            var r = position / this.col;
            var c = position - (r * this.col);
            var data = minePlaces[r, c];
            return data;
        }
        public void CalcMinesArround()
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {

                    minePlaces[i, j].ArroundMines += (i >0  && j >0 && minePlaces[i - 1, j-1].HasMine) ? 1 : 0;
                    minePlaces[i, j].ArroundMines += (i >0 &&   minePlaces[i - 1, j].HasMine) ? 1 : 0;
                    minePlaces[i, j].ArroundMines += (i > 0 && j < col -1 && minePlaces[i - 1, j + 1].HasMine) ? 1 : 0;

                    minePlaces[i, j].ArroundMines += (j >0 &&  minePlaces[i , j-1].HasMine )? 1 : 0;
                    minePlaces[i, j].ArroundMines += (j< col-1 &&  minePlaces[i, j+1].HasMine) ? 1 : 0;

                    minePlaces[i, j].ArroundMines += (i < row-1  && j>0 &&  minePlaces[i+1, j-1].HasMine )? 1 : 0;
                    minePlaces[i, j].ArroundMines += (i < row -1 &&  minePlaces[i+1, j].HasMine) ? 1 : 0;
                    minePlaces[i, j].ArroundMines += (i < row -1 && j< col-1 && minePlaces[i+1, j+1].HasMine) ? 1 : 0;
                }
            }
        }

        internal MinePlace GetItem(int position)
        {
            var r = position / this.row;
            var c = position - (r * this.row);
            return minePlaces[r, c];
        }


        public void Visit(Action<int, MinePlace> callback)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    callback(i * col+ j, this.minePlaces[i, j]);
                }
            }
        }
        public void CheckArround(int i, int j, Action callback)
        {

        }
        public void Open(int pos, Action<int, MinePlace> callback)
        {
            var startX = pos / this.col;
            var startY = pos - startX * this.col;

            this.Open(startX, startY, callback, true);
        }
        public void Open(int x, int y, Action<int, MinePlace> callback, bool initClick)
        {
            if (x < 0 || x >= this.row || y < 0 || y >= this.col) return;

            var data = minePlaces[x, y];
            if(data.Flagged)
            {
                data.Flagged = false;
                data.Confused = true;
                minePlaces[x, y] = data;
                callback(x * this.col + y, data);
                return;
            }

            if (data.Confused)
            {
                data.Flagged = false;
                data.Confused = false;
            }
            if (data.Open || data.Flagged || data.Confused) return;

            data.Open = true;
            if (data.HasMine && initClick)
            {
                this.IsEndGame = true;
                data.DeadPoint = true;
                callback(x * this.col + y, data);
                minePlaces[x, y] = data;
                return;
            }

            callback(x * this.col +y, data);
            minePlaces[x, y] = data;

            if (data.ArroundMines == 0 && !data.HasMine)
            {
                //expore mind arround
                Open(x - 1, y-1, callback, false);
                Open(x - 1, y, callback, false);
                Open(x - 1, y+1, callback, false);

                Open(x , y - 1, callback, false);
                Open(x , y + 1, callback, false);

                Open(x+1, y -1, callback, false);
                Open(x+1, y + 1, callback, false);
                Open(x+1, y , callback, false);
            } 
        }

        internal bool IsDeadPoint(int position) => MineAt(position).DeadPoint;
        
    }
}