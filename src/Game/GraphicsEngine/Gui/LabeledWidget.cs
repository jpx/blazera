using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;

namespace BlazeraLib
{
    public abstract class LabeledWidget : Widget
    {
        public enum EMode
        {
            /// <summary>
            /// Label is on the right, widget on the left
            /// </summary>
            Left,
            /// <summary>
            /// Label is on the left, widget on the right
            /// </summary>
            Right,
            /// <summary>
            /// Label in on the bottom, widget on the top
            /// </summary>
            Top,
            /// <summary>
            /// Label in on the top, widget on the bottom
            /// </summary>
            Bottom
        }

        private enum EType
        {
            Label,
            Picture
        }

        public const EMode DEFAULT_MODE = EMode.Left;

        const float DEFAULT_BOXPICTURE_OFFSET = 0F; // offset of HAutoSizeBox if picture mode
        const float DEFAULT_PICTURE_SIZE = 30F;

        protected const Boolean DEFAULT_SHORTCUT_MODE = false;

        private EMode Mode;
        private EType Type;

        private HAutoSizeBox HBox;
        private VAutoSizeBox VBox;

        private Button Label;
        private Button Picture;

        KeyCode ShortCutKey;
        Boolean ShortCutMode;

        public LabeledWidget(String label = null, EMode mode = DEFAULT_MODE, Boolean shortCutMode = DEFAULT_SHORTCUT_MODE) :
            base()
        {
            if (label != null)
            {
                Mode = mode;
                Type = EType.Label;

                Label = new Button(label, Button.EMode.Label);

                switch (this.Mode)
                {
                    case EMode.Left:
                    case EMode.Right: HBox = new HAutoSizeBox(true, null); this.AddWidget(HBox); break;
                    case EMode.Top:
                    case EMode.Bottom: VBox = new VAutoSizeBox(true, null); this.AddWidget(VBox); break;
                }

                ShortCutMode = shortCutMode;

                if (Label.Text.Length > 0)
                    ShortCutKey = WindowEvents.KeyCodeFromString(Label.Text[0].ToString());
            }
        }

        public LabeledWidget(Texture picture = null, EMode mode = DEFAULT_MODE) :
            base()
        {
            if (picture != null)
            {
                Mode = mode;
                Type = EType.Picture;

                Picture = new Button(picture, null);
                Picture.Dimension = new SFML.Graphics.Vector2(DEFAULT_PICTURE_SIZE, DEFAULT_PICTURE_SIZE);

                switch (this.Mode)
                {
                    case EMode.Left:
                    case EMode.Right: HBox = new HAutoSizeBox(true, null, DEFAULT_BOXPICTURE_OFFSET); this.AddWidget(HBox); break;
                    case EMode.Top:
                    case EMode.Bottom: VBox = new VAutoSizeBox(true, null, DEFAULT_BOXPICTURE_OFFSET); this.AddWidget(VBox); break;
                }
            }
        }

        protected void AddLabeledWidget(Widget widget)
        {
            Widget LabelWidget = this.GetLabelWidget();

            if ((HBox != null && HBox.Contains(LabelWidget)) || (VBox != null && VBox.Contains(LabelWidget)))
                return;

            switch (Mode)
            {
                case EMode.Left:
                    HBox.AddItem(widget);
                    HBox.AddItem(LabelWidget);
                    break;

                case EMode.Right:
                    HBox.AddItem(LabelWidget);
                    HBox.AddItem(widget);
                    break;

                case EMode.Top:
                    VBox.AddItem(widget, Box.DEFAULT_ITEM_LEVEL, HAlignment.Left);
                    VBox.AddItem(LabelWidget, Box.DEFAULT_ITEM_LEVEL, HAlignment.Left);
                    break;

                case EMode.Bottom:
                    VBox.AddItem(LabelWidget, Box.DEFAULT_ITEM_LEVEL, HAlignment.Left);
                    VBox.AddItem(widget, Box.DEFAULT_ITEM_LEVEL, HAlignment.Left);
                    break;
            }
        }

        public override SFML.Graphics.Vector2 Dimension
        {
            get
            {
                if (HBox != null)
                    return HBox.BackgroundDimension;

                if (VBox != null)
                    return VBox.BackgroundDimension;

                return base.Dimension;
            }
        }

        protected Button GetLabelWidget()
        {
            if (Label != null)
                return Label;

            if (Picture != null)
                return Picture;

            return null;
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            switch (evt.Type)
            {
                case SFML.Window.EventType.KeyReleased:

                    if (!ShortCutMode)
                        break;

                    if (evt.Key.Code == ShortCutKey)
                    {
                        CallShortCut();

                        return true;
                    }

                    break;
            }

            return base.OnEvent(evt);
        }

        public override void Refresh()
        {
            
        }

        protected abstract void CallShortCut();
    }
}
