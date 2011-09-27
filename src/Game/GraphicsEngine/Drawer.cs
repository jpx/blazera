using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlazeraLib
{
    public class Drawer
    {
        private Drawer()
        {

        }

        public void Init()
        {

        }

        private static Drawer _instance;
        public static Drawer Instance
        {
            get
            {
                if (_instance == null)
                    Drawer.Instance = new Drawer();

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public Image GetBorder(Sprite sprite, Color color, UInt32 thickness, Boolean decrease = false)
        {
            Image result = new Image((UInt32)sprite.Width + thickness * 2, (UInt32)sprite.Height + thickness * 2, new Color(255, 255, 255, 0));

            Color currentColor = color;

            for (UInt32 y = 0; y < sprite.Height; ++y)
            {
                UInt32 xUp = 0;
                while (xUp < sprite.Width)
                {
                    if (sprite.GetPixel(xUp, y).A == 0)
                    {
                        xUp++;
                        continue;
                    }

                    for (UInt32 thicknessCount = 1; thicknessCount < thickness + 1; ++thicknessCount)
                    {
                        if (decrease)
                            currentColor.A = thicknessCount == 1 ? (Byte)0 : (Byte)(color.A - color.A * (thicknessCount - 1) / thickness);

                        result.SetPixel(xUp - thicknessCount + thickness, y + thickness, currentColor);
                    }

                    currentColor = color;

                    break;
                }

                UInt32 xDown = (UInt32)sprite.Width - 1;
                while (xDown > 0)
                {
                    if (sprite.GetPixel(xDown, y).A == 0)
                    {
                        xDown--;
                        continue;
                    }

                    for (UInt32 thicknessCount = 1; thicknessCount < thickness + 1; ++thicknessCount)
                    {
                        if (decrease)
                            currentColor.A = thicknessCount == 1 ? (Byte)0 : (Byte)(color.A - color.A * (thicknessCount - 1) / thickness);

                        result.SetPixel(xDown + thicknessCount + thickness, y + thickness, currentColor);
                    }

                    currentColor = color;

                    break;
                }
            }

            for (UInt32 x = 0; x < sprite.Width; ++x)
            {
                UInt32 yUp = 0;
                while (yUp < sprite.Height)
                {
                    if (sprite.GetPixel(x, yUp).A == 0)
                    {
                        yUp++;
                        continue;
                    }

                    for (UInt32 thicknessCount = 1; thicknessCount < thickness + 1; ++thicknessCount)
                    {
                        if (decrease)
                            currentColor.A = thicknessCount == 1 ? (Byte)0 : (Byte)(color.A - color.A * (thicknessCount - 1) / thickness);

                        result.SetPixel(x + thickness, yUp - thicknessCount + thickness, currentColor);
                    }

                    currentColor = color;

                    break;
                }

                UInt32 yDown = (UInt32)sprite.Height - 1;
                while (yDown > 0)
                {
                    if (sprite.GetPixel(x, yDown).A == 0)
                    {
                        yDown--;
                        continue;
                    }

                    for (UInt32 thicknessCount = 1; thicknessCount < thickness + 1; ++thicknessCount)
                    {
                        if (decrease)
                            currentColor.A = thicknessCount == 1 ? (Byte)0 : (Byte)(color.A - color.A * (thicknessCount - 1) / thickness);

                        result.SetPixel(x + thickness, yDown + thicknessCount + thickness, currentColor);
                    }

                    currentColor = color;

                    break;
                }
            }

            return result;
        }
    }
}
