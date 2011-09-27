using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;

namespace BlazeraEditor
{
    public class PointCreator : WindowedWidget
    {
        #region Singleton

        private static PointCreator _instance;
        public static PointCreator Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new PointCreator();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        #region widgets
        HAutoSizeBox PointBox = new HAutoSizeBox(false, "Coordinates");
        UpDownBox XUpDownBox = new UpDownBox(0, 9999, 1, 0, "X", LabeledWidget.EMode.Bottom);
        UpDownBox YUpDownBox = new UpDownBox(0, 9999, 1, 0, "Y", LabeledWidget.EMode.Bottom);

        HAutoSizeBox ButtonBox = new HAutoSizeBox();
        Button CreateButton = new Button("Create");
        Button SaveButton = new Button("Save");
        Button CancelButton = new Button("Cancel");
        #endregion

        private PointCreator() :
            base("Point creator")
        {
            #region widgets init
            AddItem(PointBox);

            PointBox.AddItem(XUpDownBox);
            PointBox.AddItem(YUpDownBox);

            AddItem(ButtonBox);

            CreateButton.Clicked += new ClickEventHandler(CreateButton_Clicked);
            ButtonBox.AddItem(CreateButton);
            SaveButton.Clicked += new ClickEventHandler(SaveButton_Clicked);
            ButtonBox.AddItem(SaveButton);
            CancelButton.Clicked += new ClickEventHandler(CancelButton_Clicked);
            ButtonBox.AddItem(CancelButton);
            #endregion
        }

        #region events
        void CancelButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            Close();
        }

        void SaveButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            CallValidated();
        }

        void CreateButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            if (MapHandler.Instance.IsVisible)
                MapHandler.Instance.Closed += new CloseEventHandler(MapHandler_Closed);

            SetFocusedWindow(MapHandler.Instance, new OpeningInfo(false, new Dictionary<String, Object>()
                {
                    { "Mode", "PointCreatorMode" }
                }));
            MapHandler.Instance.Clicked += new MapClickEventHandler(MapHandler_Clicked);
        }

        void MapHandler_Closed(Widget sender, CloseEventArgs e)
        {
            MapHandler.Instance.Closed -= new CloseEventHandler(MapHandler_Closed);
            MapHandler.Instance.Clicked -= new MapClickEventHandler(MapHandler_Clicked);
            MapHandler.Instance.Open();
        }

        void MapHandler_Clicked(MapHandler sender, MapClickEventArgs e)
        {
            MapHandler.Instance.Clicked -= new MapClickEventHandler(MapHandler_Clicked);
            MapHandler.Instance.Close();

            XUpDownBox.SetCurrentValue((Int32)e.Point.X);
            YUpDownBox.SetCurrentValue((Int32)e.Point.Y);
        }
        #endregion

        public void SetMapDimension(Vector2 mapDimension)
        {
            XUpDownBox.ChangeValues(0, (Int32)mapDimension.X);
            YUpDownBox.ChangeValues(0, (Int32)mapDimension.Y);
        }

        void SetValues(Int32 x, Int32 y)
        {
            SetValues(new Vector2(x, y));
        }

        void SetValues(Vector2 point)
        {
            XUpDownBox.SetCurrentValue((Int32)point.X);
            YUpDownBox.SetCurrentValue((Int32)point.Y);
        }

        protected override Dictionary<String, Object> OnValidate()
        {
            Int32 x = XUpDownBox.GetCurrentValue();
            Int32 y = YUpDownBox.GetCurrentValue();

            return new Dictionary<String, Object>
            {
                { "X", x },
                { "Y", y }
            };
        }

        public String PointToString(Vector2 point)
        {
            return "Point ( " + point.X.ToString() + ", " + point.Y.ToString() + " )";
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            if (openingInfo == null || !openingInfo.IsValid())
                return;

            SetValues(openingInfo.GetArg<Vector2>("Point"));
        }
    }
}