using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class Label : Widget
    {
        public enum ESize
        {
            VSmall,
            Small,
            Medium,
            Large,
            VLarge,
            GameMenuVSmall,
            GameMenuSmall,
            GameMenuMedium,
            GameMenuLarge,
            GameMenuVLarge,
            SpeechBubbleSmall,
            SpeechBubbleMedium,
            SpeechBubbleLarge
        }

        public static UInt32 GetSizeFromESize(ESize size)
        {
            switch (size)
            {
                case ESize.VSmall: return VSMALL_TEXT_SIZE;
                case ESize.Small: return SMALL_TEXT_SIZE;
                case ESize.Medium: return MEDIUM_TEXT_SIZE;
                case ESize.Large: return LARGE_TEXT_SIZE;
                case ESize.VLarge: return VLARGE_TEXT_SIZE;
                case ESize.GameMenuVSmall: return GAMEMENU_VSMALL_TEXT_SIZE;
                case ESize.GameMenuSmall: return GAMEMENU_SMALL_TEXT_SIZE;
                case ESize.GameMenuMedium: return GAMEMENU_MEDIUM_TEXT_SIZE;
                case ESize.GameMenuLarge: return GAMEMENU_LARGE_TEXT_SIZE;
                case ESize.GameMenuVLarge: return GAMEMENU_VLARGE_TEXT_SIZE;
                case ESize.SpeechBubbleSmall: return SPEECHBUBBLE_SMALL_TEXT_SIZE;
                case ESize.SpeechBubbleMedium: return SPEECHBUBBLE_MEDIUM_TEXT_SIZE;
                case ESize.SpeechBubbleLarge: return SPEECHBUBBLE_LARGE_TEXT_SIZE;
                default: return MEDIUM_TEXT_SIZE;
            }
        }

        public static readonly Color DEFAULT_COLOR = Color.White;

        const UInt32 VSMALL_TEXT_SIZE = 8;
        const UInt32 SMALL_TEXT_SIZE = 9;
        const UInt32 MEDIUM_TEXT_SIZE = 11;
        const UInt32 LARGE_TEXT_SIZE = 12;
        const UInt32 VLARGE_TEXT_SIZE = 16;

        const uint GAMEMENU_VSMALL_TEXT_SIZE = 20;
        const UInt32 GAMEMENU_SMALL_TEXT_SIZE = 28;
        const UInt32 GAMEMENU_MEDIUM_TEXT_SIZE = 36;
        const UInt32 GAMEMENU_LARGE_TEXT_SIZE = 48;
        const UInt32 GAMEMENU_VLARGE_TEXT_SIZE = 72;

        const UInt32 SPEECHBUBBLE_SMALL_TEXT_SIZE = 10;
        const UInt32 SPEECHBUBBLE_MEDIUM_TEXT_SIZE = 12;
        const UInt32 SPEECHBUBBLE_LARGE_TEXT_SIZE = 16;

        public const ESize DEFAULT_TEXT_SIZE = ESize.Medium;

        public Label(String text = null, ESize size = DEFAULT_TEXT_SIZE) :
            base()
        {
            Text2D = new Text(text == null ? "" : text, GameData.DEFAULT_FONT, GetSizeFromESize(size));
            Text2D.Color = DEFAULT_COLOR;
        }

        public Label(String text, Color color, ESize size = DEFAULT_TEXT_SIZE) :
            this(text, size)
        {
            Color = color;
        }

        public override void Draw(RenderTarget window)
        {
            base.Draw(window);

            if (Text2D != null)
                window.Draw(Text2D);
        }

        private Text _text2D;
        protected Text Text2D
        {
            get { return _text2D; }
            set
            {
                _text2D = value;

                if (Text2D != null)
                    Dimension = new Vector2f(
                        Text2D.GetRect().Width,
                        Text2D.GetRect().Height);
            }
        }

        public void Add(Char c)
        {
            Text += c;
        }

        public void Add(String str)
        {
            Text += str;
        }

        public String RemoveLastChar(Int32 charCount = 1)
        {
            if (Text.Length < charCount)
                return null;

            Text = Text.Substring(0, Text.Length - charCount);

            return Text.Substring(Text.Length - charCount, charCount);
        }

        public String RemoveLastWord(Char separator)
        {
            String[] words = Text.Split(separator);

            if (words.Length == 0)
                return null;

            if (words.Length == 1)
            {
                Text = Text = "";
                return words[0];
            }

            Text = Text.Substring(0, Text.Length - words[words.Length - 1].Length - 1);

            return words[words.Length - 1];
        }

        public override void Refresh()
        {
            if (Text2D == null)
                return;

            Text2D.Position = Vector2I.FromVector2(GetGlobalFromLocal(
                         new Vector2f(
                             0F,
                             0F))).ToVector2();
        }

        public override Vector2f Dimension
        {
            get
            {
                if (Text2D == null)
                    return base.Dimension;

                return new Vector2f(
                    Text2D.GetRect().Width,
                    Text2D.GetRect().Height);
            }
        }

        public UInt32 Size
        {
            get { return Text2D.CharacterSize; }
        }

        public void SetSize(ESize size)
        {
            Text2D.CharacterSize = GetSizeFromESize(size);
        }

        public String Text
        {
            get
            {
                if (Text2D == null)
                    return null;

                return Text2D.DisplayedString;
            }
            set
            {
                if (Text2D == null)
                    return;

                Text2D.DisplayedString = value;

                CallChanged(new TextChangeEventArgs(Text));
            }
        }

        public override String ToString()
        {
            return base.ToString() + " ( \"" + Text + "\" )";
        }

        public Text.Styles Style
        {
            get { return Text2D.Style; }
            set
            {
                Text2D.Style = value;
                CallChanged(new TextChangeEventArgs(Text));
            }
        }

        public override Color Color
        {
            get { return base.Color; }
            set
            {
                base.Color = value;

                if (Text2D != null)
                    Text2D.Color = Color;
            }
        }

        public Font Font
        {
            get { return Text2D.Font; }
            set { Text2D.Font = value; }
        }

        public Vector2f Scale
        {
            get { return Text2D.Scale; }
            set { Text2D.Scale = value; }
        }
    }

    // TODO : Mutable letter color label
    /*
    public class ComposedLabel : Label
    {
        static readonly Color DEFAULT_FIRST_LETTER_COLOR_CHANGE = Color.Red;

        Text[] Texts;
        Int32[] ChangedChars;
        Color[] ColorChangedChars;

        public ComposedLabel(String text = null, ESize size = DEFAULT_TEXT_SIZE, Int32[] changedChars = null, Color[] colorChangedChars = null) :
            base(text, size)
        {
            ChangedChars = changedChars;
            ColorChangedChars = colorChangedChars;

            if (ChangedChars == null ||
                ColorChangedChars == null ||
                ChangedChars.Length != ColorChangedChars.Length)
                Texts = new Text[] { Text2D };
            else
            {
                InitChars(ChangedChars, ColorChangedChars);
            }
        }

        public ComposedLabel(String text = null, ESize size = DEFAULT_TEXT_SIZE) :
            this(text, size, new Int32[] { 0 }, new Color[] { DEFAULT_FIRST_LETTER_COLOR_CHANGE })
        {

        }

        void InitChars(Int32[] changedChars, Color[] colorChangedChars)
        {
            if (changedChars == null ||
                colorChangedChars == null ||
                changedChars.Length != colorChangedChars.Length ||
                Text.Length < changedChars.Length)
            {
                Texts = new Text[] { Text2D };
                return;
            }

            Int32 prev = changedChars[0];
            foreach (Int32 i in changedChars)
            {
                if (i > Text.Length || i < prev)
                {
                    Texts = new Text[] { Text2D };
                    return;
                }

                prev = i;
            }

            Texts = new Text[changedChars.Length];

            for (Int32 count = 0; count < changedChars.Length; ++count)
            {
                String str = "";
                if (changedChars.Length > count + 1)
                    str = Text.Substring(count, changedChars[count + 1]);
                else
                    str = Text.Substring(count);

                Texts[count] = new Text(str, Text2D.Font, Text2D.Size);
                Texts[count].Color = ColorChangedChars[count];
            }
        }

        Boolean IsNormal()
        {
            return Texts.Length == 1;
        }

        public override void Draw(RenderTarget window)
        {
            if (IsNormal())
            {
                base.Draw(window);
                return;
            }

            foreach (Text text2D in Texts)
                window.Draw(text2D);
        }

        public override void Refresh()
        {
            base.Refresh();

            for (Int32 count = 0; count < Texts.Length; ++count)
            {
                Texts[count].Position = GetGlobalFromLocal(new Vector2f(, ));
            }
        }

        public override Vector2f Dimension
        {
            get
            {
                if (Texts == null)
                    return base.Dimension;

                Vector2f dimension = new Vector2f();

                foreach (Text text2D in Texts)
                    dimension = new Vector2f(dimension.X + text2D.GetRect().Width, Math.Max(dimension.Y, text2D.GetRect().Height));

                return dimension;
            }
        }

        public new String Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;

                InitChars(ChangedChars, ColorChangedChars);
            }
        }
    }*/
}
