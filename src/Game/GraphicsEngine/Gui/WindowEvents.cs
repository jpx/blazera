using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class WindowEvents
    {
        private WindowEvents()
        {
            Events = new Queue<Event>();
        }

        public void Init(RenderWindow window)
        {
            window.Closed += new EventHandler(window_Closed);
            window.GainedFocus += new EventHandler(window_GainedFocus);
            window.JoystickButtonPressed += new EventHandler<JoystickButtonEventArgs>(window_JoyButtonPressed);
            window.JoystickButtonReleased += new EventHandler<JoystickButtonEventArgs>(window_JoyButtonReleased);
            window.JoystickMoved += new EventHandler<JoystickMoveEventArgs>(window_JoyMoved);
            window.KeyPressed += new EventHandler<KeyEventArgs>(window_KeyPressed);
            window.KeyReleased += new EventHandler<KeyEventArgs>(window_KeyReleased);
            window.LostFocus += new EventHandler(window_LostFocus);
            window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(window_MouseButtonPressed);
            window.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(window_MouseButtonReleased);
            window.MouseEntered += new EventHandler(window_MouseEntered);
            window.MouseLeft += new EventHandler(window_MouseLeft);
            window.MouseMoved += new EventHandler<MouseMoveEventArgs>(window_MouseMoved);
            window.MouseWheelMoved += new EventHandler<MouseWheelEventArgs>(window_MouseWheelMoved);
            window.Resized += new EventHandler<SizeEventArgs>(window_Resized);
            window.TextEntered += new EventHandler<TextEventArgs>(window_TextEntered);
        }

        private void window_TextEntered(object sender, TextEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.TextEntered;
            evt.Text.Unicode = (uint)Char.ConvertToUtf32(e.Unicode, 0);

            Events.Enqueue(evt);
        }

        private void window_Resized(object sender, SizeEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.Resized;
            evt.Size.Height = e.Height;
            evt.Size.Width = e.Width;

            Events.Enqueue(evt);
        }

        private void window_MouseWheelMoved(object sender, MouseWheelEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.MouseWheelMoved;
            evt.MouseWheel.Delta = e.Delta;

            Events.Enqueue(evt);
        }

        private void window_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.MouseMoved;
            evt.MouseMove.X = e.X;
            evt.MouseMove.Y = e.Y;

            Events.Enqueue(evt);
        }

        private void window_MouseLeft(object sender, EventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.MouseLeft;

            Events.Enqueue(evt);
        }

        private void window_MouseEntered(object sender, EventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.MouseEntered;

            Events.Enqueue(evt);
        }

        private void window_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.MouseButtonReleased;
            evt.MouseButton.Button = e.Button;
            evt.MouseButton.X = e.X;
            evt.MouseButton.Y = e.Y;

            Events.Enqueue(evt);
        }

        private void window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.MouseButtonPressed;
            evt.MouseButton.Button = e.Button;
            evt.MouseButton.X = e.X;
            evt.MouseButton.Y = e.Y;

            Events.Enqueue(evt);
        }

        private void window_LostFocus(object sender, EventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.LostFocus;

            Events.Enqueue(evt);
        }

        private void window_KeyReleased(object sender, KeyEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.KeyReleased;
            evt.Key.Alt = e.Alt ? 1 : 0;
            evt.Key.Control = e.Control ? 1 : 0;
            evt.Key.Shift = e.Shift ? 1 : 0;
            evt.Key.Code = e.Code;

            Events.Enqueue(evt);
        }

        private void window_KeyPressed(object sender, KeyEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.KeyPressed;
            evt.Key.Alt = e.Alt ? 1 : 0;
            evt.Key.Control = e.Control ? 1 : 0;
            evt.Key.Shift = e.Shift ? 1 : 0;
            evt.Key.Code = e.Code;

            Events.Enqueue(evt);
        }

        private void window_JoyMoved(object sender, JoystickMoveEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.JoystickMoved;
            evt.JoystickMove.Axis = e.Axis;
            evt.JoystickMove.JoystickId = e.JoystickId;
            evt.JoystickMove.Position = e.Position;

            Events.Enqueue(evt);
        }

        private void window_JoyButtonReleased(object sender, JoystickButtonEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.JoystickButtonReleased;
            evt.JoystickButton.Button = e.Button;
            evt.JoystickButton.JoystickId = e.JoystickId;

            Events.Enqueue(evt);
        }

        private void window_JoyButtonPressed(object sender, JoystickButtonEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.JoystickButtonPressed;
            evt.JoystickButton.Button = e.Button;
            evt.JoystickButton.JoystickId = e.JoystickId;

            Events.Enqueue(evt);
        }

        private void window_GainedFocus(object sender, EventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.GainedFocus;

            Events.Enqueue(evt);
        }

        private void window_Closed(object sender, EventArgs e)
        {
            ((RenderWindow)sender).Close();
        }

        private static WindowEvents _instance;
        public static WindowEvents Instance
        {
            get
            {
                if (_instance == null)
                {
                    WindowEvents.Instance = new WindowEvents();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public static Boolean EventHappened()
        {
            return WindowEvents.Instance.Events.Count > 0;
        }

        public static Event GetEvent()
        {
            return WindowEvents.Instance.Events.Dequeue();
        }

        private Queue<Event> Events
        {
            get;
            set;
        }

        public static Keyboard.Key KeyCodeFromString(String str)
        {
            if (str == "A") return Keyboard.Key.A;
            if (str == "B") return Keyboard.Key.B;
            if (str == "C") return Keyboard.Key.C;
            if (str == "D") return Keyboard.Key.D;
            if (str == "E") return Keyboard.Key.E;
            if (str == "F") return Keyboard.Key.F;
            if (str == "G") return Keyboard.Key.G;
            if (str == "H") return Keyboard.Key.H;
            if (str == "I") return Keyboard.Key.I;
            if (str == "J") return Keyboard.Key.J;
            if (str == "K") return Keyboard.Key.K;
            if (str == "L") return Keyboard.Key.L;
            if (str == "M") return Keyboard.Key.M;
            if (str == "N") return Keyboard.Key.N;
            if (str == "O") return Keyboard.Key.O;
            if (str == "P") return Keyboard.Key.P;
            if (str == "Q") return Keyboard.Key.Q;
            if (str == "R") return Keyboard.Key.R;
            if (str == "S") return Keyboard.Key.S;
            if (str == "T") return Keyboard.Key.T;
            if (str == "U") return Keyboard.Key.U;
            if (str == "V") return Keyboard.Key.V;
            if (str == "W") return Keyboard.Key.W;
            if (str == "X") return Keyboard.Key.X;
            if (str == "Y") return Keyboard.Key.Y;
            if (str == "Z") return Keyboard.Key.Z;
            return new Keyboard.Key();
        }
    }
}
