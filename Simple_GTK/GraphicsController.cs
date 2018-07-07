using System;
using Cairo;
using Gdk;
using Gtk;

namespace Simple_GTK
{
    public class GraphicsController : DrawingArea
    {
        float _angle = 0;
        ImageSurface _surface;
        uint _idTimer;
        static ImageSurface _image;
        static ImageSurface _atlas;
        static ImageSurface _button;
        static int _x, _y, _dx, _dy;
        int _areaX, _areaY, _areaWidth, _areaHeight;
        int _mouseX, _mouseY, _width, _height;
        bool _mousePressed;
        Cursor _cursorPressed, _cursorReleased;

        public GraphicsController(int width, int height)
        {
            _idTimer = 0;
            _mouseX = 0; _mouseY = 0;
            _mousePressed = false;
            _cursorPressed = new Cursor(CursorType.Dot);
            _cursorReleased = new Cursor(CursorType.BlankCursor);
            _x = 0;
            _y = 0;
            _dx = 3;
            _dy = 3;
            _image = new ImageSurface("camera-hands.png");
            _atlas = new ImageSurface("Atlas2.png");
            _width = width;
            _height = height;
            Load();
            FillCommonToBuffer();
        }

        void Load()
        {
            _button = new ImageSurface(Format.Argb32, 336, 83);
            using (Cairo.Context context = new Context(_button))
            {
                context.SetSource(_atlas, -196, -1720);
                context.Paint();
                context.SetSource(_atlas, -531, -1960);
                context.Paint();
            }
        }

        public void StartAnimationWholeScreen(uint interval)
        {
            uint id = GLib.Timeout.Add(interval, new GLib.TimeoutHandler(Run), GLib.Priority.Low);
        }

        public void StartAnimationArea(uint interval, int x, int y, int width, int height)
        {
            _areaX = x;
            _areaY = y;
            _areaWidth = width;
            _areaHeight = height;
            uint id = GLib.Timeout.Add(interval, new GLib.TimeoutHandler(RunArea), GLib.Priority.Low);
        }

        public void StopAnimation()
        {
            if (_idTimer != 0)
            {
                GLib.Timeout.Remove(_idTimer);
                _idTimer = 0;
            }
        }

        void Redraw()
        {
            FillCommonToBuffer();
            this.QueueDraw();
        }

        bool Run()
        {
            FillCommonToBuffer();
            this.QueueDraw();
            _y += _dy;
            _x += _dx;
            if (_x > 1100 || _x < 0)
                _dx = -_dx;
            if (_y > 860 || _y < 0)
                _dy = -_dy;
            return true;
        }

        bool RunArea()
        {
            //FillCommonToBuffer();
            this.QueueDrawArea(_areaX, _areaY, _areaWidth, _areaHeight);
            _y += _dy;
            _x += _dx;
            if (_x > 1100 || _x < 0)
                _dx = -_dx;
            if (_y > 860 || _y < 0)
                _dy = -_dy;
            return true;
        }

        static void DrawRoundedRectangle(Cairo.Context gr, double x, double y, double width, double height, double radius)
        {
            gr.Save();
            if ((radius > height / 2) || (radius > width / 2))
                radius = Min(height / 2, width / 2);

            gr.MoveTo(x, y + radius);
            gr.Arc(x + radius, y + radius, radius, Math.PI, -Math.PI / 2);
            gr.LineTo(x + width - radius, y);
            gr.Arc(x + width - radius, y + radius, radius, -Math.PI / 2, 0);
            gr.LineTo(x + width, y + height - radius);
            gr.Arc(x + width - radius, y + height - radius, radius, 0, Math.PI / 2);
            gr.LineTo(x + radius, y + height);
            gr.Arc(x + radius, y + height - radius, radius, Math.PI / 2, Math.PI);

            gr.ClosePath();
            gr.Restore();
            gr.GetTarget().Dispose();
        }
        static double Min(params double[] arr)
        {
            int minp = 0;

            for (int i = 1; i < arr.Length; i++)
                if (arr[i] < arr[minp])
                    minp = i;

            return arr[minp];
        }

        public void FillCommonToBuffer()
        {
            _surface = new ImageSurface(Format.ARGB32, _width, _height);
            using (Context cr = new Context(_surface))
            {
                cr.Save();
                //cr.Scale(0.8,0.8);
                cr.SetSourceColor(new Cairo.Color(0.8, 0.8, 0.5, 1));
                cr.Paint();
                cr.Scale(0.5, 0.5);
                cr.SetSourceSurface(_atlas, 0, 0);
                cr.Paint();
                cr.Scale(2, 2);
                cr.SetSourceSurface(_image, _x, _y);
                cr.Paint();
                cr.SetSource(_button, _x - 75, _y + 40);
                cr.Paint();

                using (Pango.Layout layout = Pango.CairoHelper.CreateLayout(cr))
                {
                    layout.Width = Pango.Units.FromPixels(500);
                    layout.Height = Pango.Units.FromPixels(500);
                    layout.Wrap = Pango.WrapMode.Word;
                    layout.Alignment = Pango.Alignment.Left;
                    layout.Ellipsize = Pango.EllipsizeMode.Start;
                    layout.FontDescription = Pango.FontDescription.FromString("Ahafoni CLM Bold 20");
                    layout.SetMarkup("<span color=\"#ff00ff\">It’s like setting up text in word or writer <span style=\"italic\">You can have indents, </span> sizes, fonts, etc etc. In this example Pango.Layout, the width is set by converting the window width into pango units because layouts are not measured in pixels. The wrap goes hand in hand with width so that any long text will wrap at the set width value. The FontDescription is quite handy. Here you can define your font. Thanks go to TD on the #mono channel for his tips here. If you want to know the names of available fonts you can enter here, go to gedit and look at the available font names. In the example above, I have the font name of “Ahafoni CLM”, the weight of “Bold” and size of 100.</span>");
                    Pango.CairoHelper.ShowLayout(cr, layout);
                }
                cr.SelectFontFace("Georgia", FontSlant.Normal, FontWeight.Bold);
                cr.SetSourceRGB(0, 0, 0);
                cr.SetFontSize(24);
                cr.MoveTo(0, 100);
                cr.ShowText(string.Format("X:{0} Y:{1} {2}", _mouseX, _mouseY, _mousePressed ? "pressed" : "released"));

                cr.Translate(300, 300);
                cr.Rotate(_angle);
                cr.Translate(-250, -250);

                DrawRoundedRectangle(cr, 40, 40, 140, 140, 80);
                DrawRoundedRectangle(cr, 320, 320, 140, 140, 80);
                DrawRoundedRectangle(cr, 40, 320, 140, 140, 80);
                DrawRoundedRectangle(cr, 320, 40, 140, 140, 80);
                DrawRoundedRectangle(cr, 150, 180, 200, 140, 30);

                cr.SetSourceColor(new Cairo.Color(1, 0.6, 0, 1));
                cr.FillPreserve();
                cr.SetSourceColor(new Cairo.Color(1, 0.8, 0, 1));
                cr.LineWidth = 8;
                cr.Stroke();

                cr.Translate(250, 250);
                cr.Rotate(-_angle);
                cr.Translate(-300, -300);

                cr.Restore();
                cr.GetTarget().Dispose();
                _angle += 0.005f;
            }
        }

        protected override bool OnDrawn(Context cr)
        {
            //cr.Save();
            cr.Rectangle(0, 0, _surface.Width, _surface.Height);
            cr.SetSourceSurface(_surface, 0, 0);
            cr.FillRule = FillRule.Winding;
            cr.FillPreserve();
            //cr.Restore();
            cr.Dispose();
            _surface.Dispose();
            return true;
        }

        internal void OnButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            if (args.Event.Button == 1)
            {
                _mousePressed = true;
                _mouseX = (int)args.Event.X;
                _mouseY = (int)args.Event.Y;
                Redraw();
                this.Parent.Screen.ActiveWindow.Cursor = _cursorPressed;
            }
        }

        internal void OnButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
        {
            if (args.Event.Button == 1)
            {
                _mousePressed = false;
                _mouseX = (int)args.Event.X;
                _mouseY = (int)args.Event.Y;
                Redraw();
                this.Parent.Screen.ActiveWindow.Cursor = _cursorReleased;
            }
        }

        internal void OnKeyPressEvent(object o, KeyPressEventArgs args)
        {
            Console.WriteLine();
        }

        internal void OnKeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (args.Event.Key == Gdk.Key.Escape) Application.Quit();
        }
    }
}
