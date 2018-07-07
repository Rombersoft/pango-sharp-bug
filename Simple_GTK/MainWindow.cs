using System;
using Gdk;
using Gtk;

namespace Simple_GTK
{
    public class MainWindow : Gtk.Window
    {
        public MainWindow() : base(Gtk.WindowType.Toplevel)
        {
            Build();
        }

        private void Build()
        {
            GraphicsController graphicsController = new GraphicsController(1280, 1024);
            graphicsController.AppPaintable = true;
            graphicsController.Visible = true;
            graphicsController.CanFocus = false;

            Title = "GTK# - PaymentSoft";
            WidthRequest = 1280;
            HeightRequest = 1024;
            CanFocus = false;
            Resizable = false;
            WindowPosition = WindowPosition.Center;
            //KeepAbove = true;
            //Decorated = false;
            Cursor cursor = new Cursor(CursorType.Hand1);
            Screen.ActiveWindow.Cursor = cursor;

            Add(graphicsController);
            SetDefaultSize(1280, 1024);
            Pixbuf one = new Gdk.Pixbuf("camera-hands.png");
            Pixbuf two = new Gdk.Pixbuf("1.jpeg");
            byte[] buffer1 = two.SaveToBuffer("jpeg", new string[1] { "quality" }, new string[1] { "100" });
            one = one.ScaleSimple(182, 162, Gdk.InterpType.Nearest);
            graphicsController.StartAnimationWholeScreen(30);
            //graphicsController.StartAnimationArea(20, 200, 200, 500, 500);
            DeleteEvent += new DeleteEventHandler(Window_Delete);
            ButtonPressEvent += graphicsController.OnButtonPressEvent;
            ButtonReleaseEvent += graphicsController.OnButtonReleaseEvent;
            KeyPressEvent += graphicsController.OnKeyPressEvent;
            KeyReleaseEvent += graphicsController.OnKeyReleaseEvent;
            //ShowAll();
        }

        void Window_Delete(object o, DeleteEventArgs args)
        {
            Application.Quit();
            args.RetVal = true;
        }

        void Image1_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            Console.WriteLine("Pressed");
        }
    }
}
