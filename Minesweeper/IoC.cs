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
using TinyIoC;
using Minesweeper.Services;
using XLabs.Ioc;

namespace Minesweeper
{
    public class IoC
    {
        
        public static void SetIoc(Activity activity)
        {
            if (!Resolver.IsSet)
            {
                var container = TinyIoCContainer.Current;
                var flatDB = new InternalFlatFileStogare(activity.GetDir("game_history_v1.db", FileCreationMode.Append));
                container.Register<IGameHistory>(flatDB);
                Resolver.SetResolver(new XLabs.Ioc.TinyIOC.TinyResolver((container)));
            }
        }
        
        
        internal static T Resove<T>() where T : class
        {
            return TinyIoC.TinyIoCContainer.Current.Resolve<T>();
        }
    }
}