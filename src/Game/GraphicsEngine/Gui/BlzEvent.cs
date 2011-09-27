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
            this.Event = evt;

            this.IsHandled = false;
            this.IsAbsorbed = false;
        }

        public EventType Type
        {
            get { return this.Event.Type; }
        }

        public EType GetType()
        {
            if (this.Type == EventType.MouseMoved ||
                this.Type == EventType.MouseEntered ||
                this.Type == EventType.MouseLeft ||
                this.Type == EventType.GainedFocus ||
                this.Type == EventType.LostFocus)
                return EType.MouseMove;

            if (this.Type == EventType.JoyButtonPressed ||
                this.Type == EventType.JoyButtonReleased ||
                this.Type == EventType.KeyPressed ||
                this.Type == EventType.KeyReleased ||
                this.Type == EventType.TextEntered)
                return EType.Key;

            return EType.MouseButton;
        }

        public JoyButtonEvent JoyButton
        {
            get { return this.Event.JoyButton; }
        }

        public JoyMoveEvent JoyMove
        {
            get { return this.Event.JoyMove; }
        }

        public KeyEvent Key
        {
            get { return this.Event.Key; }
        }

        public MouseButtonEvent MouseButton
        {
            get { return this.Event.MouseButton; }
        }

        public MouseMoveEvent MouseMove
        {
            get { return this.Event.MouseMove; }
        }

        public MouseWheelEvent MouseWheel
        {
            get { return this.Event.MouseWheel; }
        }

        public SizeEvent Size
        {
            get { return this.Event.Size; }
        }

        public TextEvent Text
        {
            get { return this.Event.Text; }
        }
    }
}
