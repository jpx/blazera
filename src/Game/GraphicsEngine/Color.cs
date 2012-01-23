namespace BlazeraLib.Graphics
{
    public class Color
    {
        #region Enums

        public enum ColorName
        {
            //!\\ TODO : list all
            // Shades of Black and Gray
            White,
            Black,
            Grey,
            Silver,
            grey,
            LightGray,
            LigthSlateGrey,
            SlateGray,
            SlateGray1,
            SlateGray2,
            SlateGray3,
            SlateGray4,

            tan2,
            tan3,
            tan4,

            DarkGreen,
            ForestGreen,

            red4
        }

        #endregion

        #region Constants

        const byte DEFAULT_R = 0;
        const byte DEFAULT_G = 0;
        const byte DEFAULT_B = 0;
        const byte DEFAULT_A = 255;

        #endregion

        #region Members

        #region Static members

        static System.Collections.Generic.Dictionary<ColorName, Color> Colors = new System.Collections.Generic.Dictionary<ColorName, Color>()
        {
            { ColorName.White,          new Color(255,  255,    255)    },
            { ColorName.Black,          new Color(0,    0,      0)      },
            { ColorName.Grey,           new Color(84,   84,     84)     },
            { ColorName.Silver,         new Color(192,  192,    192)    },
            { ColorName.grey,           new Color(190,  190,    190)    },
            { ColorName.LightGray,      new Color(211,  211,    211)    },

            { ColorName.tan2,           new Color(238,  154,    73)     },
            { ColorName.tan3,           new Color(205,  133,    63)     },
            { ColorName.tan4,           new Color(139,  90,     43)     },

            { ColorName.DarkGreen,      new Color(47,   79,     47)     },
            { ColorName.ForestGreen,    new Color(34,   139,    34)     },

            { ColorName.red4,           new Color(139,  0,      0)      }
        };

        #endregion

        byte R;
        byte G;
        byte B;
        byte A;

        #endregion

        public Color(byte r = DEFAULT_R, byte g = DEFAULT_G, byte b = DEFAULT_B, byte a = DEFAULT_A)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color(Color copy) :
            this(copy.R, copy.G, copy.B, copy.A)
        {

        }

        public SFML.Graphics.Color ToSFColor()
        {
            return new SFML.Graphics.Color(R, G, B, A);
        }

        public static Color FromSFColor(SFML.Graphics.Color color)
        {
            return new Color(color.R, color.G, color.B, color.A);
        }

        public static Color GetColorFromName(ColorName colorName)
        {
            return Colors[colorName];
        }

        public static Color operator +(Color color1, Color color2)
        {
            return new Color(
                (byte)(color1.R + color2.R),
                (byte)(color1.G + color2.G),
                (byte)(color1.B + color2.B),
                (byte)(color1.A + color2.A));
        }

        public static Color operator -(Color color1, Color color2)
        {
            return new Color(
                (byte)(color1.R - color2.R),
                (byte)(color1.G - color2.G),
                (byte)(color1.B - color2.B),
                (byte)(color1.A - color2.A));
        }

        public static bool operator ==(Color color1, Color color2)
        {
            return
                color1.R == color2.R &&
                color1.G == color2.G &&
                color1.B == color2.B &&
                color1.A == color2.A;
        }

        public static bool operator !=(Color color1, Color color2)
        {
            return !(color1 == color2);
        }
    }
}
