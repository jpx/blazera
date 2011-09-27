using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraEditor
{
    public class WarpActionBox : ActionBox
    {
        #region widgets declaration
        VAutoSizeBox MapNameBox = new VAutoSizeBox(false, "Map name");
        VAutoSizeBox WarpPointNameBox = new VAutoSizeBox(false, "WarpPoint name");

        DownList MapDownList = new DownList(4);
        DownList WarpPointDownList = new DownList(4);
        Dictionary<Button, Map> Maps = new Dictionary<Button, Map>();
        #endregion

        public WarpActionBox() :
            base()
        {
            #region widgets init
            AddItem(MapNameBox);
            MapNameBox.AddItem(MapDownList);
            AddItem(WarpPointNameBox);
            WarpPointNameBox.AddItem(WarpPointDownList);
            #endregion
        }

        public override BlazeraLib.Action GetAction()
        {
            return new WarpAction(MapDownList.GetCurrent(), WarpPointDownList.GetCurrent());
        }

        public void SetSettings(WarpAction action)
        {
            MapDownList.SetCurrent(action.MapName);
            WarpPointDownList.SetCurrent(action.WarpPointName);
        }

        void RefreshMapDownList()
        {
            Maps.Clear();
            MapDownList.Clear();

            foreach (Map map in MapMan.Instance.GetMapList())
            {
                Button mapButton = new Button(map.Type, Button.EMode.LabelEffect);
                mapButton.Clicked += new ClickEventHandler(mapButton_Clicked);
                MapDownList.AddText(mapButton);

                Maps.Add(mapButton, map);
            }
        }

        void mapButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            RefreshWarpPointDownList((Button)sender);
        }

        void RefreshWarpPointDownList(Button mapButton)
        {
            WarpPointDownList.Clear();

            Map currentSelectedMap = Maps[mapButton];

            foreach (String warpPointName in currentSelectedMap.WarpPoints.Keys)
            {
                Button warpPointNameButton = new Button(warpPointName, Button.EMode.LabelEffect);
                WarpPointDownList.AddText(warpPointNameButton);
            }
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            RefreshMapDownList();
        }
    }
}
