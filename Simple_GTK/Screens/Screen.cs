using System;
namespace Simple_GTK
{
    public abstract class Screen
    {
        protected string _name;

        internal string Name { get { return _name; } }

        internal abstract bool Initialize();

        internal virtual void Draw()
        {
            //TODO:Draw panel
        }

        internal virtual void DoWorkInput()
        {
            //TODO:Check click on panel
        }

        internal abstract void Finalize();
    }
}
