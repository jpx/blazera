using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using System.Threading;

namespace BlazeraEditor
{
    public class GameEditor
    {
        public GameEditor()
        {
            GameEditor.IsRunning = true;
            Init();
        }

        private void Init()
        {
            GameTime.Instance.Init();
            GameEngine.Instance.Init();
            GraphicsEngine.Instance.Init();
        }

        public void Launch()
        {
            while (GameEditor.IsRunning)
            {
                GameTime.Update();

                Time Dt = GameTime.Dt;
                GameEditor.IsRunning = GraphicsEngine.Instance.Update(Dt);
            }
        }

        public static Boolean IsRunning
        {
            get;
            set;
        }
    }
}
