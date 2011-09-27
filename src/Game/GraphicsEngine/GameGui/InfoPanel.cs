using System.Collections.Generic;
using SFML.Graphics;

namespace BlazeraLib
{
    public abstract class InfoPanel : GameWidget
    {
        #region Constants

        protected const float DEFAULT_MARGINS = 10F;

        #endregion

        #region Members

        ConfigurableBox MainBox;

        RoundedRectangleShape BackgroundShape;

        float Margins;

        #endregion

        public InfoPanel() :
            base()
        {
            MainBox = new ConfigurableBox();
            AddWidget(MainBox);

            Margins = DEFAULT_MARGINS;
        }

        public void AddBox(InfoPanelBox box)
        {
            MainBox.AddConfiguration(box.ConfigurationName, box);
        }

        public void BuildBox(string configurationName, InfoPanelBox.BuildInfo buildInfo, bool setAsCurrent = true)
        {
            ((InfoPanelBox)MainBox.GetConfiguration(configurationName)).Build(buildInfo);

            if (setAsCurrent)
                MainBox.SetCurrentConfiguration(configurationName);
        }

        public override void Draw(RenderWindow window)
        {
            if (BackgroundShape != null)
                BackgroundShape.Draw(window);

            base.Draw(window);
        }

        public override void Refresh()
        {
            base.Refresh();

            BackgroundShape = new RoundedRectangleShape(Dimension, 20F, 3F, Color.Black, Color.Black, true);
            BackgroundShape.SetPosition(Position);
            MainBox.Position = GetGlobalFromLocal(new Vector2());

            if (GetRoot() == null)
                return;

            if (Left < GetRoot().Left)
                Left = GetRoot().Left;
            if (Top < GetRoot().Top)
                Top = GetRoot().Top;
            if (BackgroundRight > GetRoot().BackgroundRight)
                BackgroundRight = GetRoot().BackgroundRight;
            if (BackgroundBottom > GetRoot().BackgroundBottom)
                BackgroundBottom = GetRoot().BackgroundBottom;
        }

        public override Vector2 Dimension
        {
            get
            {
                if (MainBox == null)
                    return base.Dimension + GetStructureDimension();

                return MainBox.BackgroundDimension + GetStructureDimension();
            }
        }

        public override Vector2 BackgroundDimension
        {
            get
            {
                if (BackgroundShape == null)
                    return base.BackgroundDimension;

                return BackgroundShape.GetBackgroundDimension();
            }
        }

        protected override Vector2 GetBasePosition()
        {
            return base.GetBasePosition() + new Vector2(Margins, Margins) + (BackgroundShape == null ? new Vector2() : BackgroundShape.GetBasePosition());
        }

        protected override Vector2 GetStructureDimension()
        {
            return base.GetStructureDimension() + new Vector2(Margins * 2F, Margins * 2F);
        }
    }
}
