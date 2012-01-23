using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class BoxBackground : Widget
    {
        const float DEFAULT_TITLE_HEIGHTFACTOR = .9F;

        const Text.Styles LABEL_STYLE = Text.Styles.Bold;

        private Label Label { get; set; }
        public float TopBorderHeight { get; private set; }

        private Border TopBorder { get; set; }
        private Border BottomBorder { get; set; }

        private float Margins { get; set; }

        public BoxBackground(Vector2f dimension, String name = null, Boolean noBackgroundMode = true) :
            base()
        {
            Dimension = dimension;

            Name = name;

            Margins = Border.GetBoxBorderWidth();

            if (Name != null)
            {
                Label = new Label(Name);
                Label.Style = LABEL_STYLE;
                Label.IsColorLinked = false;
                AddWidget(Label);

                TopBorderHeight = Label.Dimension.Y + Margins * 4F;

                TopBorder = new Border(new Vector2f(Label.Dimension.X + Margins * 4F, TopBorderHeight), Border.EMode.Box, noBackgroundMode);
                Label.Position = GetGlobalFromLocal(TopBorder.Dimension / 2F - Label.Dimension / 2F);
            }
            
            BottomBorder = new Border(Dimension, Border.EMode.Box, noBackgroundMode);
        }

        public void SetBackgroundAlphaFactor(double backgroundAlphaFactor)
        {
            if (TopBorder != null)
                TopBorder.SetBackgroundAlphaFactor(backgroundAlphaFactor);

            BottomBorder.SetBackgroundAlphaFactor(backgroundAlphaFactor);
        }

        public override void Refresh()
        {
            if (TopBorder != null)
            {
                if (RefreshInfo.IsDimensionRefreshed)
                    TopBorder.Resize(new Vector2f(
                            (Label.Dimension.X + Margins * 4F) / TopBorder.Dimension.X,
                            TopBorderHeight / TopBorder.Dimension.Y));

                TopBorder.Move(Position - TopBorder.Position);

                Label.Position = GetGlobalFromLocal(TopBorder.Dimension / 2F - Label.Dimension / 2F);
            }

            if (RefreshInfo.IsDimensionRefreshed)
                BottomBorder.Resize(new Vector2f(
                    Dimension.X / BottomBorder.Dimension.X,
                    Dimension.Y / BottomBorder.Dimension.Y));

            BottomBorder.Move(Position - BottomBorder.Position);
        }

        public override void Draw(RenderTarget window)
        {
            if (TopBorder != null)
                TopBorder.Draw(window);

            base.Draw(window);

            BottomBorder.Draw(window);
        }

        public override Color Color
        {
            get
            {
                return base.Color;
            }
            set
            {
                base.Color = value;

                if (TopBorder != null)
                    TopBorder.SetColor(Color);

                BottomBorder.SetColor(Color);
            }
        }

        public override Vector2f Dimension
        {
            set
            {
                if (TopBorder == null)
                {
                    base.Dimension = value;
                    return;
                }

                base.Dimension = new Vector2f(
                    Math.Max(TopBorder.Dimension.X, value.X),
                    value.Y);
            }
        }
    }
}

