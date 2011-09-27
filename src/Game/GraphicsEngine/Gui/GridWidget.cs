namespace BlazeraLib
{
    public class GridWidget : Widget
    {
        GridShape Grid;

        public GridWidget(uint scale, uint width, uint height) :
            base()
        {
            Grid = new GridShape(scale, width, height);
        }

        public override void Draw(SFML.Graphics.RenderWindow window)
        {
            base.Draw(window);

            Grid.Draw(window);
        }

        public override void Refresh()
        {
            base.Refresh();

            if (!RefreshInfo.IsPositionRefreshed)
                return;

            Grid.Move(RefreshInfo.PositionOffsetRefresh);
        }

        public override SFML.Graphics.Vector2 Dimension
        {
            get
            {
                if (Grid == null)
                    return base.Dimension;

                return Grid.Dimension;
            }
        }
    }
}
