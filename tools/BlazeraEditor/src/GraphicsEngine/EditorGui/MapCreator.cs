using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraEditor
{
    public class MapCreator : WindowedWidget
    {
        #region Singleton

        private static MapCreator _instance;
        public static MapCreator Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new MapCreator();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        #region Widgets
        VAutoSizeBox SettingBox = new VAutoSizeBox(false, "Settings");

        LabeledTextBox TypeTextBox = new LabeledTextBox("Type", LabeledWidget.EMode.Right);

        HAutoSizeBox SizeBox = new HAutoSizeBox();
        UpDownBox WidthUDBox = new UpDownBox(MapMan.MAP_MIN_SIZE, MapMan.MAP_MAX_SIZE, 5, 0, "Width", LabeledWidget.EMode.Bottom);
        UpDownBox HeightUDBox = new UpDownBox(MapMan.MAP_MIN_SIZE, MapMan.MAP_MAX_SIZE, 5, 0, "Height", LabeledWidget.EMode.Bottom);

        HAutoSizeBox ButtonBox = new HAutoSizeBox();
        Button SaveButton = new Button("Save");
        Button CancelButton = new Button("Cancel");
        #endregion

        Map Map;

        private MapCreator() :
            base("Map creator")
        {
            #region widgets init
            this.AddItem(SettingBox);

            SettingBox.AddItem(TypeTextBox);

            SettingBox.AddItem(SizeBox);
            SizeBox.AddItem(WidthUDBox);
            SizeBox.AddItem(HeightUDBox);

            this.AddItem(ButtonBox);

            SaveButton.Clicked += new ClickEventHandler(SaveButton_Clicked);
            ButtonBox.AddItem(SaveButton);
            CancelButton.Clicked += new ClickEventHandler(CancelButton_Clicked);
            ButtonBox.AddItem(CancelButton);
            #endregion
        }

        #region events
        void PointCreator_Validated(WindowedWidget sender, ValidateEventArgs e)
        {
            Log.Cl("Point : (" + e.GetArg<Int32>("X") + ", " + e.GetArg<Int32>("Y") + ")");
        }

        void CancelButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            this.Close();
        }

        void SaveButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            this.CallValidated();
        }
        #endregion

        public override void Reset()
        {
            base.Reset();

            Map = null;
        }

        protected override Dictionary<String, Object> OnValidate()
        {
            if (!TypeTextBox.TextBox.TextIsValid())
            {
                CallInformationDialogBox(InformationDialogBox.EType.Error, new String[] { InformationDialogBox.Instance.GetTypeErrorStr() });
                return base.OnValidate();
            }

            Ground ground = new Ground();
            ground.SetType(TypeTextBox.TextBox.Text, true);
            ground.Init(WidthUDBox.GetCurrentValue(), HeightUDBox.GetCurrentValue());
            ground.FillWithTile(0, Create.Tile(MapMan.DEFAULT_GROUND_TILE));
            ground.ToScript();
            
            Map = new Map();
            Map.SetType(TypeTextBox.TextBox.Text, true);

            return new Dictionary<String, Object>
            {
                { "Map", Map }
            };
        }
    }
}
