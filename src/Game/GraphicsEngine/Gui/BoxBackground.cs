using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

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

        public BoxBackground(Vector2 dimension, String name = null, Boolean noBackgroundMode = true) :
            base()
        {
            this.Dimension = dimension;

            this.Name = name;

            this.Margins = Border.GetBoxBorderWidth();

            if (this.Name != null)
            {
                this.Label = new Label(this.Name);
                Label.Style = LABEL_STYLE;
                Label.IsColorLinked = false;
                this.AddWidget(this.Label);

                this.TopBorderHeight = this.Label.Dimension.Y + this.Margins * 4F;

                this.TopBorder = new Border(new Vector2(this.Label.Dimension.X + this.Margins * 4F, this.TopBorderHeight), Border.EMode.Box, noBackgroundMode);
                this.Label.Position = this.GetGlobalFromLocal(this.TopBorder.Dimension / 2F - this.Label.Dimension / 2F);
            }
            
            this.BottomBorder = new Border(this.Dimension, Border.EMode.Box, noBackgroundMode);
        }

        public void SetBackgroundAlphaFactor(double backgroundAlphaFactor)
        {
            if (TopBorder != null)
                TopBorder.SetBackgroundAlphaFactor(backgroundAlphaFactor);

            BottomBorder.SetBackgroundAlphaFactor(backgroundAlphaFactor);
        }

        public override void Refresh()
        {
            if (this.TopBorder != null)
            {
                if (RefreshInfo.IsDimensionRefreshed)
                    this.TopBorder.Resize(new Vector2(
                            (this.Label.Dimension.X + this.Margins * 4F) / this.TopBorder.Dimension.X,
                            this.TopBorderHeight / this.TopBorder.Dimension.Y));

                this.TopBorder.Move(this.Position - this.TopBorder.Position);

                this.Label.Position = this.GetGlobalFromLocal(this.TopBorder.Dimension / 2F - this.Label.Dimension / 2F);
            }

            if (RefreshInfo.IsDimensionRefreshed)
                this.BottomBorder.Resize(new Vector2(
                    this.Dimension.X / this.BottomBorder.Dimension.X,
                    this.Dimension.Y / this.BottomBorder.Dimension.Y));

            this.BottomBorder.Move(Position - BottomBorder.Position);
        }

        public override void Draw(RenderWindow window)
        {
            if (this.TopBorder != null)
                this.TopBorder.Draw(window);

            base.Draw(window);

            this.BottomBorder.Draw(window);
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

                if (this.TopBorder != null)
                    this.TopBorder.SetColor(this.Color);

                this.BottomBorder.SetColor(this.Color);
            }
        }

        public override Vector2 Dimension
        {
            set
            {
                if (TopBorder == null)
                {
                    base.Dimension = value;
                    return;
                }

                base.Dimension = new Vector2(
                    Math.Max(TopBorder.Dimension.X, value.X),
                    value.Y);
            }
        }
    }
}

