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
using System.IO;
using Java.IO;

namespace Minesweeper.Services
{
    public class InternalFlatFileStogare : IGameHistory
    {
        private Java.IO.File file = null;
        string root = "";
        public InternalFlatFileStogare(Java.IO.File f)
        {
            this.root = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) +"/game_history_v1.1.mines";
            this.file = f;
            if(!file.Exists())
            {
                file.CreateNewFile();
            }
        }

        public void AddAddItem(GameHistoryItem item)
        {
            var list = this.AllHistories();
            var exist = list.FirstOrDefault(p => p.Rows == item.Rows &&
                                                p.Cols == item.Cols &&
                                                p.Mine == item.Mine);
            if (exist == null)
            {
                item.Date = DateTime.Now;
                list.Add(item);
            }
            else
            {
                exist.Score = Math.Max(exist.Score, item.Score);
                exist.Date = DateTime.Now;
            }
            list.OrderByDescending(p => p.Score);
            var xml = list.SerializeAsXml();
            SaveToFile(xml);
        }

        private void SaveToFile(string xml)
        {
            file.Mkdirs();
            file.Mkdir();
            //BufferedWriter writer = new BufferedWriter(new FileWriter(file, true));
            System.IO.File.WriteAllText(root, xml);
            //writer.Write(xml);
            //writer.Close();
        }
        private string ReadFile()
        {
            try
            {

                return System.IO.File.ReadAllText(root);
            }
            catch(Exception ex) {
                return string.Empty;
            }
            return string.Empty;
        }

        public List<GameHistoryItem> AllHistories()
        {
            var xml = ReadFile();
            if(string.IsNullOrEmpty(xml) )
            {
                return new List<GameHistoryItem>();
            }
            return xml.DeserializeAsXml<List<GameHistoryItem>>();
        }
    }
}