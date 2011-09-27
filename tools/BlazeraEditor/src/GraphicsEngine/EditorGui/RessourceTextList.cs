using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraEditor
{
    public class RessourceTextList : Widget
    {
        public delegate void DOnLoad();
        DOnLoad OnLoad;

        TextList TextList;

        List<BaseObject> Objects;

        public RessourceTextList(Int32 size = BlazeraLib.ExtendedBox.DEFAULT_SIZE, Boolean cursorMode = true, DOnLoad onLoad = null) :
            base()
        {
            TextList = new TextList(size, cursorMode);

            SetOnLoad(onLoad);
        }

        public void SetOnLoad(DOnLoad onLoad)
        {
            OnLoad = onLoad;
        }

        public void LoadRessources()
        {
            if (OnLoad != null)
                OnLoad();
        }
    }
}
