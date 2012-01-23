namespace Blazera
{
    public class FpsLabel : BlazeraLib.GameLabel
    {
        BlazeraLib.Timer Timer = new BlazeraLib.Timer();

        public FpsLabel() :
            base(null, BlazeraLib.Label.ESize.Large)
        {
            Timer.Start();
        }

        public override void Init()
        {
            base.Init();

            PlayerHdl.Vlad.OnStateChange += new BlazeraLib.WorldObject.StateEventHandler(Vlad_OnStateChange);
        }

        string state;
        void Vlad_OnStateChange(BlazeraLib.WorldObject sender, BlazeraLib.WorldObject.StateEventArgs e)
        {
            state = e.State;
        }

        public override void Update(BlazeraLib.Time dt)
        {
            base.Update(dt);

            if (!Timer.IsDelayCompleted(.5D))
                return;

            InnerLabel.Text = string.Empty;
            AddToText("FPS", ((int)(1D / dt.Value)).ToString());
            AddToText("Time", ((int)BlazeraLib.GameTime.GetSessionTime().Value).ToString());
            AddToText("Player pos", "( " + (int)PlayerHdl.Vlad.Position.X + ", " + (int)PlayerHdl.Vlad.Position.Y + ", " + PlayerHdl.Vlad.Z + " )");
            AddToText("Player state", state);
        }

        void AddToText(string desc, string value)
        {
            InnerLabel.Text += desc + ": " + value + "\n";
        }
    }
}
