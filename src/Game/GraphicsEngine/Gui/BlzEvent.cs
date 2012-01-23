using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;

namespace BlazeraLib
{
    public class BlzEvent
    {
        public enum EType
        {
            Key,
            MouseButton,
            MouseMove
        }

        private Event Event { get; set; }
        public Boolean IsHandled { get; set; }
        public Boolean IsAbsorbed { get; set; }

        public BlzEvent(Event evt)
        {
            Event = evt;

            IsHandled = false;
            IsAbsorbed = false;
        }

        public EventType Type
        {
            get { return Event.Type; }
        }

        public EType GetType()
        {
            if (Type == EventType.MouseMoved ||
                Type == EventType.MouseEntered ||
                Type == EventType.MouseLeft ||
                Type == EventType.GainedFocus ||
                Type == EventType.LostFocus)
                return EType.MouseMove;

            if (Type == EventType.JoystickButtonPressed ||
                Type == EventType.JoystickButtonReleased ||
                Type == EventType.KeyPressed ||
                Type == EventType.KeyReleased ||
                Type == EventType.TextEntered)
                return EType.Key;

            return EType.MouseButton;
        }

        public JoystickButtonEvent JoyButton
        {
            get { return Event.JoystickButton; }
        }

        public JoystickMoveEvent JoyMove
        {
            get { return Event.JoystickMove; }
        }

        public KeyEvent Key
        {
            get { return Event.Key; }
        }

        public MouseButtonEvent MouseButton
        {
            get { return Event.MouseButton; }
        }

        public MouseMoveEvent MouseMove
        {
            get { return Event.MouseMove; }
        }

        public MouseWheelEvent MouseWheel
        {
            get { return Event.MouseWheel; }
        }

        public SizeEvent Size
        {
            get { return Event.Size; }
        }

        public TextEvent Text
        {
            get { return Event.Text; }
        }
    }
}
