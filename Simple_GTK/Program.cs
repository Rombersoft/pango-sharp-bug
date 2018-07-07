using System;
using Cairo;
using Gdk;
using Gtk;

namespace Simple_GTK
{
    class MainClass
    {

        public static void Main(string[] args)
        {
            Application.Init();
            MainWindow win = new MainWindow();
            win.Show();
            Application.Run();
        }
    }
}