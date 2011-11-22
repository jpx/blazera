using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraEditor
{
    public class MapMan : WindowedWidget
    {
        #region Singleton

        private static MapMan _instance;
        public static MapMan Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new MapMan();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        public const Int32 MAP_MIN_SIZE = 1;
        public const Int32 MAP_MAX_SIZE = 500;

        public const String DEFAULT_GROUND_TILE = "TileSet11_22";

        #region Widgets
        // maps
        HAutoSizeBox MapListBox = new HAutoSizeBox(false, "Maps");
        TextList MapList = new TextList(5);

        VAutoSizeBox MapListButtonBox = new VAutoSizeBox();
        Button CreateButton = new Button("Create", Button.EMode.BackgroundLabel, true);
        Button RemoveButton = new Button("Remove");
        Button RefreshButton = new Button("Refresh");
        Button SelectButton = new Button("Select");

        // current map
        VAutoSizeBox CurrentMapBox = new VAutoSizeBox(false, "Current map");

        VAutoSizeBox WarpPointBox = new VAutoSizeBox(false, "Warp points");
        TextList WarpPointList = new TextList(5, false);
        Button AddWarpPointButton = new Button("Add");
        CheckBox WarpPointCheckBox = new CheckBox("Show warp points", LabeledWidget.EMode.Left, true);

        HAutoSizeBox CurrentMapButtonBox = new HAutoSizeBox();
        Button SaveButton = new Button("Save");
        #endregion

        Dictionary<String, MapWidget> Maps = new Dictionary<String, MapWidget>();

        Map CurrentMap;

        // gestion des warpPoints dans la warpPointList
        Dictionary<Button, WarpPoint> WarpPoints = new Dictionary<Button, WarpPoint>();
        WarpPoint CurrentWarpPoint;
        Button CurrentWarpPointButton;

        public void SetCurrentMap(String mapType)
        {
            Map newMap = AddMap(mapType);

            if (CurrentMap != null &&
                CurrentMap == newMap)
                return;

            CurrentMap = newMap;

            RefreshWarpPoints();

            MapHandler.Instance.SetMap(CurrentMap);

            if (!WarpPointCheckBox.IsChecked)
                GetCurrentMapWidget().Close();
        }

        public static Map GetCurrent()
        {
            return Instance.CurrentMap;
        }

        private MapMan() :
            base("Map manager")
        {
            #region widgets init
            // maps
            this.AddItem(MapListBox);
            MapListBox.AddItem(MapList);

            MapListBox.AddItem(MapListButtonBox);
            CreateButton.Clicked += new ClickEventHandler(CreateButton_Clicked);
            MapListButtonBox.AddItem(CreateButton, 0, HAlignment.Right);
            RemoveButton.Clicked += new ClickEventHandler(RemoveButton_Clicked);
            MapListButtonBox.AddItem(RemoveButton, 0, HAlignment.Right);
            RefreshButton.Clicked += new ClickEventHandler(RefreshButton_Clicked);
            MapListButtonBox.AddItem(RefreshButton, 0, HAlignment.Right);
            SelectButton.Seal();
            SelectButton.Clicked += new ClickEventHandler(SelectButton_Clicked);
            MapListButtonBox.AddItem(SelectButton, 0, HAlignment.Right);

            // current map
            this.AddItem(CurrentMapBox);
            // current map warp points
            CurrentMapBox.AddItem(WarpPointBox, 0, HAlignment.Left);
            WarpPointBox.AddItem(WarpPointList);
            AddWarpPointButton.Clicked += new ClickEventHandler(AddWarpPointButton_Clicked);
            WarpPointBox.AddItem(AddWarpPointButton);
            WarpPointCheckBox.Checked += new CheckEventHandler(WarpPointCheckBox_Checked);
            WarpPointBox.AddItem(WarpPointCheckBox);

            // current map buttons
            CurrentMapBox.AddItem(CurrentMapButtonBox, 0, HAlignment.Left);
            SaveButton.Clicked += new ClickEventHandler(SaveButton_Clicked);
            CurrentMapButtonBox.AddItem(SaveButton);
            #endregion
        }

        public List<Map> GetMapList()
        {
            List<Map> maps = new List<Map>();

            foreach (MapWidget mapWidget in Maps.Values)
                maps.Add(mapWidget.Map);

            return maps;
        }

        void SelectButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            CallValidated();
        }

        void WarpPointCheckBox_Checked(object sender, CheckEventArgs e)
        {
            GetCurrentMapWidget().SwitchState();
        }

        MapWidget GetCurrentMapWidget()
        {
            return Maps[CurrentMap.Type];
        }

        public void InitMap(String mapType)
        {
            SetCurrentMap(mapType);

            LoadMaps();
        }

        void AddWarpPointButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            SetFocusedWindow(WarpPointCreator.Instance, new OpeningInfo(true), AddWarpPointCreator_Validated);
        }

        void AddWarpPointCreator_Validated(WindowedWidget sender, ValidateEventArgs e)
        {
            AddWarpPoint(e.GetArg<WarpPoint>("WarpPoint"), true, e.GetArg<Boolean>("Default"));
        }

        #region events
        void RemoveButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            CallConfirmationDialogBox(new String[] { ConfirmationDialogBox.Instance.GetDeletionStr(CurrentMap) }, RemoveCurrentMap);
        }

        void RemoveCurrentMap()
        {
            FileManager.Instance.RemoveMap(CurrentMap.Type);
            RemoveMap(CurrentMap.Type);

            this.RefreshMaps();
        }

        void MapCreator_Validated(WindowedWidget sender, ValidateEventArgs e)
        {
            Map map = e.GetArg<Map>("Map");

            map.ToScript();

            this.RefreshMaps();
        }

        void CreateButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            SetFocusedWindow(MapCreator.Instance, new OpeningInfo(true), MapCreator_Validated);
        }

        void RefreshButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            this.RefreshMaps();
        }

        void SaveButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            CurrentMap.ToScript();
        }

        void button_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            String mapType = ((Button)sender).Text;
            SetCurrentMap(mapType);
        }
        #endregion

        Map AddMap(String mapType)
        {
            if (Maps.ContainsKey(mapType))
                return Maps[mapType].Map;

            Maps.Add(mapType, new MapWidget(Create.Map(mapType)));

            return Maps[mapType].Map;
        }

        Boolean RemoveMap(String mapType)
        {
            return Maps.Remove(mapType);
        }

        public MapWidget GetMapWidget(String mapType)
        {
            if (!Maps.ContainsKey(mapType))
                return null;

            return Maps[mapType];
        }

        private void LoadMaps()
        {
            foreach (String mapType in FileReader.Instance.GetMapTypes())
            {
                Button button = new Button(mapType, Button.EMode.Label);
                button.Name = mapType;
                button.Clicked += new ClickEventHandler(button_Clicked);
                MapList.AddText(button, CurrentMap.Type == mapType);

                AddMap(mapType);
            }
        }

        private void RefreshMaps()
        {
            MapList.Clear();

            this.LoadMaps();

            if (MapList.GetTextCount() > 0)
                SetCurrentMap(((Button)MapList.GetAt(0)).Text);
        }

        #region WarpPoint

        void RefreshWarpPoints()
        {
            WarpPointList.Clear();
            WarpPoints.Clear();

            if (CurrentMap.WarpPoints.Count == 0)
            {
                AddWarpPoint(WarpPoint.GetDefault());

                return;
            }

            foreach (WarpPoint warpPoint in CurrentMap.WarpPoints.Values)
            {
                AddWarpPoint(warpPoint, false);
            }
        }

        void AddWarpPoint(WarpPoint warpPoint, Boolean addToMap = true, Boolean defaultWarpPoint = false)
        {
            // ajout a la TextList
            Button button = new Button(WarpPointCreator.Instance.WarpPointToString(warpPoint), Button.EMode.LabelEffect);
            button.Clicked += new ClickEventHandler(warpPointbutton_Clicked);
            WarpPointList.AddText(button);

            // ajout au dico button/warpPoint
            WarpPoints.Add(button, warpPoint);

            // ajout a la map
            Maps[CurrentMap.Type].AddWarpPoint(warpPoint, addToMap, defaultWarpPoint);
        }

        void RemoveWarpPoint()
        {
            // suppression de la map
            GetCurrentMapWidget().RemoveWarpPoint(WarpPointToRemove.Name);

            // suppression de la TextList
            WarpPointList.RemoveText(CurrentWarpPointButton);

            // suppression du dico button/warpPoint
            WarpPoints.Remove(CurrentWarpPointButton);
        }

        // warpPoint courant a supprimer
        WarpPoint WarpPointToRemove;

        void warpPointbutton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            CurrentWarpPointButton = (Button)sender;
            CurrentWarpPoint = WarpPoints[CurrentWarpPointButton];

            // clique droit --> suppression
            if (e.Button == Mouse.Button.Right)
            {
                WarpPointToRemove = CurrentWarpPoint;

                if (WarpPoints.Count > 1)
                    CallConfirmationDialogBox(new String[] { ConfirmationDialogBox.Instance.GetDeletionStr("WarpPoint", CurrentWarpPointButton.Text) }, RemoveWarpPoint);
                else
                    CallInformationDialogBox(InformationDialogBox.EType.Error, new String[] { InformationDialogBox.Instance.GetDeletionErrorStr("WarpPoint", CurrentWarpPointButton.Text) });

                return;
            }

            // clique gauche --> edit

            SetFocusedWindow(WarpPointCreator.Instance, new OpeningInfo(
                true,
                new Dictionary<String, Object>
                {
                    { "Name", CurrentWarpPoint.Name },
                    { "Point", CurrentWarpPoint.Point },
                    { "Direction", CurrentWarpPoint.Direction },
                    { "Default", CurrentMap.DefaultWarpPoint == CurrentWarpPoint }
                }),
                CurrentWarpPointPointCreator_Validated);
        }

        void CurrentWarpPointPointCreator_Validated(WindowedWidget sender, ValidateEventArgs e)
        {
            WarpPoint newWarpPoint = e.GetArg<WarpPoint>("WarpPoint");

            WarpPoints[CurrentWarpPointButton] = newWarpPoint;

            Maps[CurrentMap.Type].SetWarpPoint(CurrentWarpPoint.Name, newWarpPoint, e.GetArg<Boolean>("Default"));
            CurrentWarpPoint = newWarpPoint;
            CurrentWarpPointButton.Text = WarpPointCreator.Instance.WarpPointToString(CurrentWarpPoint);
        }

        public Boolean WarpPointAreShown()
        {
            return WarpPointCheckBox.IsChecked;
        }

        #endregion
    }
}