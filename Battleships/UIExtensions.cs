using Common.Structures.Common;
using NetworkCommsDotNet.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Battleships
{
    public static class UIExtensions
    {
        public static MainWindow GetMainWindow(this UserControl c)
        {
            return Window.GetWindow(c) as MainWindow;
        }

        public static void Invoke(this UserControl c, Delegate method, params object[] args)
        {
            c.Dispatcher.BeginInvoke(method, args);
        }

        public static void Invoke(this UserControl c, Action method)
        {
            c.Dispatcher.BeginInvoke(method);
        }

        public static void Invoke<T>(this UserControl c, Action<T> method, T arg1)
        {
            c.Dispatcher.BeginInvoke(method, arg1);
        }
        public static void Invoke<T1, T2>(this UserControl c, Action<T1, T2> method, T1 arg1, T2 arg2)
        {
            c.Dispatcher.BeginInvoke(method, arg1, arg2);
        }

        public static void SafelyRemoveById(this ObservableCollection<PlayerDisplay> c, ShortGuid id)
        {
            lock (c)
            {
                foreach (var p in c)
                {
                    if (p.PlayerID == id)
                    {
                        c.Remove(p);
                        break;
                    }
                }
            }
        }
    }
}
