using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraEditor
{
    public class WarpPointCreator : WindowedWidget
    {
        #region Singleton

        private static WarpPointCreator _instance;
        public static WarpPointCreator Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new WarpPointCreator();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        #region widgets
        VAutoSizeBox SettingBox = new VAutoSizeBox(false, "Settings");

        LabeledTextBox NameTextBox = new LabeledTextBox("Name", LabeledWidget.EMode.Right, TextBox.EInputType.AlphaNumeric, true);
        HAutoSizeBox PointBox = new HAutoSizeBox(false, "Coordinates");
        Button PointButton = new Button(PointCreator.Instance.PointToString(new Vector2f()), Button.EMode.LabelEffect, true);
        LabeledDownList DirectionDownList = new LabeledDownList("Direction", 8);
        CheckBox DefaultCheckBox = new CheckBox("Default", LabeledWidget.EMode.Right, false, true);

        HAutoSizeBox ButtonBox = new HAutoSizeBox();
        Button SaveButton = new Button("Save");
        Button CancelButton = new Button("Cancel");
        #endregion

        Vector2f CurrentPoint;

        private WarpPointCreator() :
            base("Warp point creator")
        {
            #region widgets init
            AddItem(SettingBox);
            SettingBox.AddItem(NameTextBox);

            SettingBox.AddItem(PointBox);
            PointButton.Clicked += new ClickEventHandler(PointButton_Clicked);
            PointBox.AddItem(PointButton);

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                Button directionButton = new Button(direction.ToString(), Button.EMode.LabelEffect);
                DirectionDownList.DownList.AddText(directionButton);
            }

            SettingBox.AddItem(DirectionDownList);

            SettingBox.AddItem(DefaultCheckBox);

            AddItem(ButtonBox);
            SaveButton.Clicked += new ClickEventHandler(SaveButton_Clicked);
            ButtonBox.AddItem(SaveButton);
            CancelButton.Clicked += new ClickEventHandler(CancelButton_Clicked);
            ButtonBox.AddItem(CancelButton);
            #endregion
        }

        void CancelButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            Close();
        }

        void SaveButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            CallValidated();
        }

        void PointButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            SetFocusedWindow(PointCreator.Instance, new OpeningInfo(true, new Dictionary<String, Object>
            {
                {"Point", new Vector2f(CurrentPoint.X, CurrentPoint.Y) }
            }), OnPointCreatorValidated);
        }

        void OnPointCreatorValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            CurrentPoint = new Vector2f(e.GetArg<Int32>("X"), e.GetArg<Int32>("Y"));
            PointButton.Text = PointCreator.Instance.PointToString(CurrentPoint);
        }

        protected override Dictionary<String, Object> OnValidate()
        {
            if (!NameTextBox.TextBox.TextIsValid())
            {
                CallInformationDialogBox(InformationDialogBox.EType.Error, new String[] { InformationDialogBox.Instance.GetTypeErrorStr() });
                return base.OnValidate();
            }

            WarpPoint warpPoint = new WarpPoint(NameTextBox.TextBox.Text, CurrentPoint, (Direction)Enum.Parse(typeof(Direction), DirectionDownList.GetCurrent()));

            return new Dictionary<String, Object>
            {
                { "WarpPoint", warpPoint },
                { "Default", DefaultCheckBox.IsChecked }
            };
        }

        public override void Reset()
        {
            base.Reset();

            CurrentPoint = new Vector2f();
            PointButton.Text = PointCreator.Instance.PointToString(CurrentPoint);
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            if (openingInfo == null || !openingInfo.IsValid())
                return;

            NameTextBox.TextBox.Reset(openingInfo.GetArg<String>("Name"));
            CurrentPoint = openingInfo.GetArg<Vector2f>("Point");
            PointButton.Text = PointCreator.Instance.PointToString(CurrentPoint);
            DirectionDownList.DownList.SetCurrent(openingInfo.GetArg<Direction>("Direction").ToString());
            DefaultCheckBox.SetIsChecked(openingInfo.GetArg<Boolean>("Default"));
        }

        public String WarpPointToString(WarpPoint warpPoint)
        {
            return warpPoint.Name + " ( " + warpPoint.Point.X.ToString() + ", " + warpPoint.Point.Y.ToString() + " )";
        }
    }
}
