namespace BlazeraLib
{
    public class MapBaseWidget : GameWidget
    {
        public override void Draw(SFML.Graphics.RenderTarget window)
        {
            SFML.Graphics.View currentView = window.GetView();
            window.SetView(GetRoot().MapView);

            base.Draw(window);

            window.SetView(currentView);
        }
    }
}
