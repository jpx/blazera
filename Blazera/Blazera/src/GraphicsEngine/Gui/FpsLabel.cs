namespace Blazera
{
    public class FpsLabel : BlazeraLib.Label
    {
        BlazeraLib.Timer Timer = new BlazeraLib.Timer();

        public FpsLabel() :
            base(null, ESize.VLarge)
        {
            Timer.Start();
        }

        public override void Update(BlazeraLib.Time dt)
        {
            base.Update(dt);

            if (!Timer.IsDelayCompleted(.5D))
                return;

            Text = ((int)(1D / dt.Value)).ToString();
        }
    }
}
