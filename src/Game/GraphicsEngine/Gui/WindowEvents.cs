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
            this.Events = new Queue<Event>();
        }

        public void Init(RenderWindow window)
        {
            window.Closed += new EventHandler(window_Closed);
            window.GainedFocus += new EventHandler(window_GainedFocus);
            window.JoyButtonPressed += new EventHandler<JoyButtonEventArgs>(window_JoyButtonPressed);
            window.JoyButtonReleased += new EventHandler<JoyButtonEventArgs>(window_JoyButtonReleased);
            window.JoyMoved += new EventHandler<JoyMoveEventArgs>(window_JoyMoved);
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

            this.Events.Enqueue(evt);
        }

        private void window_Resized(object sender, SizeEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.Resized;
            evt.Size.Height = e.Height;
            evt.Size.Width = e.Width;

            this.Events.Enqueue(evt);
        }

        private void window_MouseWheelMoved(object sender, MouseWheelEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.MouseWheelMoved;
            evt.MouseWheel.Delta = e.Delta;

            this.Events.Enqueue(evt);
        }

        private void window_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.MouseMoved;
            evt.MouseMove.X = e.X;
            evt.MouseMove.Y = e.Y;

            this.Events.Enqueue(evt);
        }

        private void window_MouseLeft(object sender, EventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.MouseLeft;

            this.Events.Enqueue(evt);
        }

        private void window_MouseEntered(object sender, EventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.MouseEntered;

            this.Events.Enqueue(evt);
        }

        private void window_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.MouseButtonReleased;
            evt.MouseButton.Button = e.Button;
            evt.MouseButton.X = e.X;
            evt.MouseButton.Y = e.Y;

            this.Events.Enqueue(evt);
        }

        private void window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.MouseButtonPressed;
            evt.MouseButton.Button = e.Button;
            evt.MouseButton.X = e.X;
            evt.MouseButton.Y = e.Y;

            this.Events.Enqueue(evt);
        }

        private void window_LostFocus(object sender, EventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.LostFocus;

            this.Events.Enqueue(evt);
        }

        private void window_KeyReleased(object sender, KeyEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.KeyReleased;
            evt.Key.Alt = e.Alt ? 1 : 0;
            evt.Key.Control = e.Control ? 1 : 0;
            evt.Key.Shift = e.Shift ? 1 : 0;
            evt.Key.Code = e.Code;

            this.Events.Enqueue(evt);
        }

        private void window_KeyPressed(object sender, KeyEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.KeyPressed;
            evt.Key.Alt = e.Alt ? 1 : 0;
            evt.Key.Control = e.Control ? 1 : 0;
            evt.Key.Shift = e.Shift ? 1 : 0;
            evt.Key.Code = e.Code;

            this.Events.Enqueue(evt);
        }

        private void window_JoyMoved(object sender, JoyMoveEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.JoyMoved;
            evt.JoyMove.Axis = e.Axis;
            evt.JoyMove.JoystickId = e.JoystickId;
            evt.JoyMove.Position = e.Position;

            this.Events.Enqueue(evt);
        }

        private void window_JoyButtonReleased(object sender, JoyButtonEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.JoyButtonReleased;
            evt.JoyButton.Button = e.Button;
            evt.JoyButton.JoystickId = e.JoystickId;

            this.Events.Enqueue(evt);
        }

        private void window_JoyButtonPressed(object sender, JoyButtonEventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.JoyButtonPressed;
            evt.JoyButton.Button = e.Button;
            evt.JoyButton.JoystickId = e.JoystickId;

            this.Events.Enqueue(evt);
        }

        private void window_GainedFocus(object sender, EventArgs e)
        {
            Event evt = new Event();
            evt.Type = EventType.GainedFocus;

            this.Events.Enqueue(evt);
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

        public static KeyCode KeyCodeFromString(String str)
        {
            if (str == "A") return KeyCode.A;
            if (str == "B") return KeyCode.B;
            if (str == "C") return KeyCode.C;
            if (str == "D") return KeyCode.D;
            if (str == "E") return KeyCode.E;
            if (str == "F") return KeyCode.F;
            if (str == "G") return KeyCode.G;
            if (str == "H") return KeyCode.H;
            if (str == "I") return KeyCode.I;
            if (str == "J") return KeyCode.J;
            if (str == "K") return KeyCode.K;
            if (str == "L") return KeyCode.L;
            if (str == "M") return KeyCode.M;
            if (str == "N") return KeyCode.N;
            if (str == "O") return KeyCode.O;
            if (str == "P") return KeyCode.P;
            if (str == "Q") return KeyCode.Q;
            if (str == "R") return KeyCode.R;
            if (str == "S") return KeyCode.S;
            if (str == "T") return KeyCode.T;
            if (str == "U") return KeyCode.U;
            if (str == "V") return KeyCode.V;
            if (str == "W") return KeyCode.W;
            if (str == "X") return KeyCode.X;
            if (str == "Y") return KeyCode.Y;
            if (str == "Z") return KeyCode.Z;
            return new KeyCode();
        }
    }
}
