using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class HAutoSizeBox : HBox
    {
        private float Offset { get; set; }

        public HAutoSizeBox(Boolean noBackgroundMode = true, String name = null, float offset = DEFAULT_BORDER_WIDTH, float xExtremityOffset = DEFAULT_X_EXTREMITY_OFFSET, Boolean backgroundNoBackgroundMode = true) :
            base(noBackgroundMode, name, xExtremityOffset, backgroundNoBackgroundMode)
        {
            Offset = offset;
            XExtremityOffset = xExtremityOffset;
        }

        private Vector2f GetMaxDimension()
        {
            if (Items.Count == 0)
                return new Vector2f();

            Vector2f maxDimension = new Vector2f();

            IEnumerator<Widget> iEnum = Items.GetEnumerator();

            while (iEnum.MoveNext())
            {
                maxDimension = new Vector2f(
                    maxDimension.X + iEnum.Current.BackgroundDimension.X + Offset,
                    iEnum.Current.BackgroundDimension.Y + GetLevelOffset(iEnum.Current) > maxDimension.Y ? iEnum.Current.BackgroundDimension.Y + GetLevelOffset(iEnum.Current) : maxDimension.Y);
            }

            if (Items.Count > 0)
                maxDimension = new Vector2f(maxDimension.X - Offset + XExtremityOffset * 2F, maxDimension.Y);

            return maxDimension;
        }

        public override void Refresh()
        {
            Dimension = GetMaxDimension();

            base.Refresh();
        }
    }

    public class VAutoSizeBox : VBox
    {
        private float Offset { get; set; }

        public VAutoSizeBox(Boolean noBackgroundMode = true, String name = null, float offset = DEFAULT_BORDER_WIDTH, float yExtremityOffset = DEFAULT_Y_EXTREMITY_OFFSET, Boolean backgroundNoBackgroundMode = true) :
            base(noBackgroundMode, name, yExtremityOffset, backgroundNoBackgroundMode)
        {
            Offset = offset;
            YExtremityOffset = yExtremityOffset;
        }

        private Vector2f GetMaxDimension()
        {
            if (Items.Count == 0)
                return new Vector2f();

            Vector2f maxDimension = new Vector2f();

            IEnumerator<Widget> iEnum = Items.GetEnumerator();

            while (iEnum.MoveNext())
            {
                maxDimension = new Vector2f(
                    iEnum.Current.BackgroundDimension.X + GetLevelOffset(iEnum.Current) > maxDimension.X ? iEnum.Current.BackgroundDimension.X + GetLevelOffset(iEnum.Current) : maxDimension.X,
                    maxDimension.Y + iEnum.Current.BackgroundDimension.Y + Offset);
            }

            if (Items.Count > 0)
                maxDimension = new Vector2f(maxDimension.X, maxDimension.Y - Offset + YExtremityOffset * 2F);

            return maxDimension;
        }

        public override void Refresh()
        {
            Dimension = GetMaxDimension();

            base.Refresh();
        }
    }
}
