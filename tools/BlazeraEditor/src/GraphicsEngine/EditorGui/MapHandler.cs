using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraEditor
{
    public class MapHandler : WindowedWidget
    {
        #region Singleton

        private static MapHandler _instance;
        public static MapHandler Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new MapHandler();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        #region Constants

        public const float DEFAULT_SIZE = 18F;
        static readonly Vector2f SIZE = new Vector2f(DEFAULT_SIZE + 5F, DEFAULT_SIZE) * GameData.TILE_SIZE;

        static readonly Color CURRENT_SELECTED_OBJECT_COLOR = new Color(238, 64, 0, 192);

        const Int32 TILE_SELECTOR_WIDTH = 4;
        const Int32 TILE_SELECTOR_HEIGHT = 8;

        #endregion

        #region Members

        Boolean ViewIsLocked = false;

        public event MapClickEventHandler Clicked;

        Dictionary<String, Tool> Tools = new Dictionary<String, Tool>();
        Tool CurrentTool;

        Dictionary<Button, EBoundingBox> CurrentSelectedObjectExternalBoundingBoxes = new Dictionary<Button, EBoundingBox>();
        Dictionary<String, WorldObject> SelectedObjects = new Dictionary<String, WorldObject>();
        WorldObject CurrentSelectedObject;

        #endregion

        #region widgets declaration
        HAutoSizeBox SubMainBox = new HAutoSizeBox();

        /************\
         * left box *
        \************/
        VAutoSizeBox LeftMainBox = new VAutoSizeBox();
        // map
        HAutoSizeBox MapMainBox = new HAutoSizeBox(false);
        MapBox MapBox = new MapBox(SIZE);
        // player
        HAutoSizeBox PlayerMainBox = new HAutoSizeBox();

        VAutoSizeBox ModeBox = new VAutoSizeBox(false, "Modes");
        CheckBox GodModeCheckBox = new CheckBox("God mode", LabeledWidget.EMode.Left, false, true);
        CheckBox TransparentModeCheckBox = new CheckBox("Transparent mode", LabeledWidget.EMode.Left, false, true);

        VAutoSizeBox PlayerBox = new VAutoSizeBox(false, "Player", Border.GetBoxBorderWidth());

        UpDownBox VelocityUpDownBox = new UpDownBox((Int32)PlayerHdl.Vlad.Velocity, 500, 25, 0, "Velocity");
        HAutoSizeBox PosBox = new HAutoSizeBox();
        VAutoSizeBox PosUpDownBox = new VAutoSizeBox();
        UpDownBox XPosUpDownBox = new UpDownBox(0, 99, GameData.TILE_SIZE, 0, "X");
        UpDownBox YPosUpDownBox = new UpDownBox(0, 99, GameData.TILE_SIZE, 0, "Y");
        Button PosApplyButton = new Button("Apply");

        VAutoSizeBox ViewBox = new VAutoSizeBox(false, "View");
        CheckBox LockViewCheckBox = new CheckBox("Lock view on player", LabeledWidget.EMode.Left, true, true);

        /*************\
         * right box *
        \*************/
        // tools
        VAutoSizeBox ToolMainBox = new VAutoSizeBox(false, "Tools");
        RadioButton ToolRadioButton = new RadioButton();

        ConfigurableBox ToolConfigurableBox = new ConfigurableBox();

        //SelectorTool
        VAutoSizeBox SelectorToolBox = new VAutoSizeBox(false, "Object selector");
        HAutoSizeBox SelectorToolHBox = new HAutoSizeBox(); 
        LabeledDownList SelectorDownList = new LabeledDownList("Selected objects", 4, LabeledWidget.EMode.Bottom);
        Label SelectorToolSelectorObjectCountLabel = new Label();
        TextList SelectorTextList = new TextList(4);
        HAutoSizeBox SelectorToolRemoveButtonBox = new HAutoSizeBox();
        Button SelectorToolRemoveButton = new Button("Remove");
        Button SelectorToolRemoveAllButton = new Button("Remove all");
        VAutoSizeBox SelectorToolCurrentObjectBox = new VAutoSizeBox(false, "Current object");
        DisplayScreen SelectorToolCurrentObjectDisplayScreen = new DisplayScreen();
        ConfigurableBox SelectorToolCurrentObjectConfigurableBox = new ConfigurableBox();
        /* SelectorTool
            None*/
        VAutoSizeBox SelectorToolNoneBox = new VAutoSizeBox();
        /* SelectorTool
            Element*/
        VAutoSizeBox SelectorToolElementBox = new VAutoSizeBox();
        HAutoSizeBox SelectorToolElementPositionBox = new HAutoSizeBox(false, "Position");
        UpDownBox SelectorToolElementXUpDownBox = new UpDownBox(0, 99, 1, 0, "X", LabeledWidget.EMode.Bottom);
        UpDownBox SelectorToolElementYUpDownBox = new UpDownBox(0, 99, 1, 0, "Y", LabeledWidget.EMode.Bottom);
        VAutoSizeBox SelectorToolElementEventBox = new VAutoSizeBox(false, "Events");
        TextList SelectorToolElementEventTextList = new TextList(4, false);
        Button SelectorToolElementAddEventButton = new Button("Add event");

        // ObjectPencil
        VAutoSizeBox ObjectPencilBox = new VAutoSizeBox(false, "Object pencil");
        Button ChangeObjectButton = new Button("Select object", Button.EMode.BackgroundLabel, true);
        CheckBox LockPencilCheckBox = new CheckBox("Lock pencil", LabeledWidget.EMode.Left, false, true);
        VAutoSizeBox LockObjectPencilBox = new VAutoSizeBox(false);
        UpDownBox LockValueUpDownBox = new UpDownBox(1, 64, 8, 32, "Lock value", LabeledWidget.EMode.Right);
        Button ObjectPencilFillButton = new Button("Fill");
        Button ObjectPencilFillRandomlyButton = new Button("Fill randomly", Button.EMode.BackgroundLabel, true);

        // TilePencil
        VAutoSizeBox TilePencilBox = new VAutoSizeBox(false, "Tile pencil");
        TileSelector TileSelector = new TileSelector(TILE_SELECTOR_WIDTH, TILE_SELECTOR_HEIGHT);
        Button SelectTileSetButton = new Button("Select TileSet");
        #endregion

        #region events
        void PosApplyButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            UpdatePlayerPos();
        }

        void UpdatePlayerPos()
        {
            PlayerHdl.Vlad.MoveTo(new Vector2f(XPosUpDownBox.GetCurrentValue(), YPosUpDownBox.GetCurrentValue()));
        }

        void lockViewCheckBox_Checked(object sender, CheckEventArgs e)
        {
            ViewIsLocked = e.IsChecked;
        }

        void VelocityUpDownBox_ValueChanged(UpDownBox sender, ValueChangeEventArgs e)
        {
            PlayerHdl.Vlad.Velocity = e.Value;
        }

        void transparent_Checked(object sender, CheckEventArgs e)
        {
            if (e.IsChecked)
            {
                /*PlayerHdl.Vlad.Color = new Color(
                    PlayerHdl.Vlad.CurrentAnimation.CurrentSprite.Color.R,
                    PlayerHdl.Vlad.CurrentAnimation.CurrentSprite.Color.G,
                    PlayerHdl.Vlad.CurrentAnimation.CurrentSprite.Color.B,
                    128);
                PlayerHdl.Vlad.IsVisible = false;
                MapBox.AddObjectToDraw(PlayerHdl.Vlad);*/
            }
            else
            {
                /*PlayerHdl.Vlad.Color = new Color(
                    PlayerHdl.Vlad.CurrentAnimation.CurrentSprite.Color.R,
                    PlayerHdl.Vlad.CurrentAnimation.CurrentSprite.Color.G,
                    PlayerHdl.Vlad.CurrentAnimation.CurrentSprite.Color.B,
                    255);

                PlayerHdl.Vlad.IsVisible = true;
                MapBox.RemoveObjectToDraw(PlayerHdl.Vlad);*/
            }
        }

        void godMode_Checked(object sender, CheckEventArgs e)
        {
            if (e.IsChecked)
            {
                foreach (BBoundingBox BB in PlayerHdl.Vlad.BBoundingBoxes)
                    BB.Activate(false);
            }
            else
            {
                foreach (BBoundingBox BB in PlayerHdl.Vlad.BBoundingBoxes)
                    BB.Activate(true);
            }
        }

        void MapHandler_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (!ContainsMouse())
                return;

            if (Clicked != null)
                Clicked(this, new MapClickEventArgs(new Vector2f(e.X, e.Y)));
        }
        #endregion

        MapHandler() :
            base("Map handler")
        {
            #region widgets init
            AddItem(SubMainBox);
            SubMainBox.AddItem(LeftMainBox);

            // map
            LeftMainBox.AddItem(MapMainBox);
            MapBox.MainPlayer = PlayerHdl.Vlad;
            MapBox.MapClicked += new ClickEventHandler(MapHandler_Clicked);
            MapMainBox.AddItem(MapBox);

            LeftMainBox.AddItem(PlayerMainBox, 0, HAlignment.Center);
            // modes
            PlayerMainBox.AddItem(ModeBox);
            GodModeCheckBox.Checked += new CheckEventHandler(godMode_Checked);
            ModeBox.AddItem(GodModeCheckBox, 0, HAlignment.Left);
            TransparentModeCheckBox.Checked += new CheckEventHandler(transparent_Checked);
            ModeBox.AddItem(TransparentModeCheckBox, 0, HAlignment.Left);
            // player
            PlayerMainBox.AddItem(PlayerBox);
            VelocityUpDownBox.ValueChanged += new ValueChangeEventHandler(VelocityUpDownBox_ValueChanged);
            PlayerBox.AddItem(VelocityUpDownBox, 0, HAlignment.Left);
            PlayerBox.AddItem(PosBox);
            PosBox.AddItem(PosUpDownBox);
            PosApplyButton.Clicked += new ClickEventHandler(PosApplyButton_Clicked);
            PosBox.AddItem(PosApplyButton);
            PosUpDownBox.AddItem(XPosUpDownBox, 0, HAlignment.Left);
            PosUpDownBox.AddItem(YPosUpDownBox, 0, HAlignment.Left);
            // view
            PlayerMainBox.AddItem(ViewBox);
            LockViewCheckBox.Checked += new CheckEventHandler(lockViewCheckBox_Checked);
            ViewBox.AddItem(LockViewCheckBox);


            // tools
            SubMainBox.AddItem(ToolMainBox, 0, VAlignment.Top);

            AddTool(SelectorTool.Instance);
            AddTool(ObjectPencil.Instance);
            AddTool(TilePencil.Instance);
            ToolMainBox.AddItem(ToolRadioButton);

            ToolMainBox.AddItem(ToolConfigurableBox);
            // SelectorTool
            SelectorTool.Instance.OnObjectAddition += new SelectedObjectChangeEventHandler(Instance_OnObjectAddition);
            SelectorTool.Instance.OnObjectSuppression += new SelectedObjectChangeEventHandler(Instance_OnObjectSuppression);
            ToolConfigurableBox.AddConfiguration("SelectorTool", SelectorToolBox);
            SelectorToolBox.AddItem(SelectorToolHBox);
            SelectorToolHBox.AddItem(SelectorDownList);
            SelectorToolHBox.AddItem(SelectorToolSelectorObjectCountLabel);
            SelectorToolBox.AddItem(SelectorTextList);
            SelectorToolBox.AddItem(SelectorToolRemoveButtonBox);
            SelectorToolRemoveButton.Clicked += new ClickEventHandler(SelectorToolRemoveButton_Clicked);
            SelectorToolRemoveButtonBox.AddItem(SelectorToolRemoveButton);
            SelectorToolRemoveAllButton.Clicked += new ClickEventHandler(SelectorToolRemoveAllButton_Clicked);
            SelectorToolRemoveButtonBox.AddItem(SelectorToolRemoveAllButton);

            SelectorToolBox.AddItem(SelectorToolCurrentObjectBox);
            SelectorToolCurrentObjectBox.AddItem(SelectorToolCurrentObjectDisplayScreen);
            SelectorToolCurrentObjectBox.AddItem(SelectorToolCurrentObjectConfigurableBox);

            SelectorToolCurrentObjectConfigurableBox.AddConfiguration("None", SelectorToolNoneBox);

            SelectorToolCurrentObjectConfigurableBox.AddConfiguration("Element", SelectorToolElementBox);
            SelectorToolElementBox.AddItem(SelectorToolElementPositionBox);
            SelectorToolElementPositionBox.AddItem(SelectorToolElementXUpDownBox);
            SelectorToolElementPositionBox.AddItem(SelectorToolElementYUpDownBox);
            SelectorToolElementBox.AddItem(SelectorToolElementEventBox);
            SelectorToolElementEventBox.AddItem(SelectorToolElementEventTextList);
            SelectorToolElementAddEventButton.Clicked += new ClickEventHandler(SelectorToolElementAddEventButton_Clicked);
            SelectorToolElementEventBox.AddItem(SelectorToolElementAddEventButton);
            // ObjectPencil
            ToolConfigurableBox.AddConfiguration("ObjectPencil", ObjectPencilBox);
            ChangeObjectButton.Clicked += new ClickEventHandler(ChangeObjectButton_Clicked);
            ObjectPencilBox.AddItem(ChangeObjectButton);
            LockPencilCheckBox.Checked += new CheckEventHandler(LockPencilCheckBox_Checked);
            ObjectPencilBox.AddItem(LockPencilCheckBox);
            LockObjectPencilBox.Seal();
            ObjectPencilBox.AddItem(LockObjectPencilBox);
            LockValueUpDownBox.ValueChanged += new ValueChangeEventHandler(LockValueUpDownBox_ValueChanged);
            LockObjectPencilBox.AddItem(LockValueUpDownBox, 0, HAlignment.Right);
            ObjectPencilFillButton.Clicked += new ClickEventHandler(ObjectPencilFillButton_Clicked);
            LockObjectPencilBox.AddItem(ObjectPencilFillButton);
            ObjectPencilFillRandomlyButton.Clicked += new ClickEventHandler(ObjectPencilFillRandomlyButton_Clicked);
            LockObjectPencilBox.AddItem(ObjectPencilFillRandomlyButton);
            // TilePencil
            ToolConfigurableBox.AddConfiguration("TilePencil", TilePencilBox);
            TilePencilBox.AddItem(TileSelector);
            SelectTileSetButton.Clicked += new ClickEventHandler(SelectTileSetButton_Clicked);
            TilePencilBox.AddItem(SelectTileSetButton);
            #endregion

            OnFocusGain += new WindowedWidgetEventHandler(MapHandler_OnFocusGain);
            OnFocusLoss += new WindowedWidgetEventHandler(MapHandler_OnFocusLoss);
        }

        void MapHandler_OnFocusLoss(WindowedWidget sender, WindowedWidgetEventArgs e)
        {
            MapBox.RunMapUpdate(false);
        }

        void MapHandler_OnFocusGain(WindowedWidget sender, WindowedWidgetEventArgs e)
        {
            MapBox.RunMapUpdate(true);
        }

        public override void Init()
        {
            base.Init();
        }

        void SelectorToolElementAddEventButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            SetFocusedWindow(BoundingBoxCreator.Instance, new OpeningInfo(true, new Dictionary<String, Object>()
                {
                    { "Mode", "MapHandler_Add_Mode" },
                    { "Texture", new BlazeraLib.Texture(CurrentSelectedObjectTexture) }
                }), OnBoundingBoxCreatorAddValidation);
        }

        void OnBoundingBoxCreatorAddValidation(WindowedWidget sender, ValidateEventArgs e)
        {
            EBoundingBox BB = e.GetArg<EBoundingBox>("BoundingBox");
            BB.Holder = CurrentSelectedObject;
            CurrentSelectedObject.AddEventBoundingBox(BB, EventBoundingBoxType.External);
            AddBoundingBoxToCurrentSelectedObject(BB);
        }

        void AddBoundingBoxToCurrentSelectedObject(EBoundingBox BB)
        {
            Button BBButton = new Button(BoundingBoxCreator.Instance.BBToString(BB), Button.EMode.LabelEffect);
            BBButton.Clicked += new ClickEventHandler(BBButton_Clicked);
            SelectorToolElementEventTextList.AddText(BBButton);

            CurrentSelectedObjectExternalBoundingBoxes.Add(BBButton, BB);
        }

        void RemoveBoundingBoxFromCurrentSelectedObject(Button BBButton)
        {
            CurrentSelectedObject.RemoveEventBoundingBox(CurrentSelectedObjectExternalBoundingBoxes[BBButton], EventBoundingBoxType.External);
            CurrentSelectedObjectExternalBoundingBoxes.Remove(BBButton);
            SelectorToolElementEventTextList.RemoveText(BBButton);
        }

        Button CurrentEditedBBButton;
        void BBButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            CurrentEditedBBButton = (Button)sender;

            if (e.Button == Mouse.Button.Right)
            {
                CallConfirmationDialogBox(new String[]
                {
                    ConfirmationDialogBox.Instance.GetDeletionStr("Event", ((Button)sender).Text)
                }, OnRemoveBoundingBoxFromCurrentSelectedObjectConfirmation);

                return;
            }

            EBoundingBox BB = CurrentSelectedObjectExternalBoundingBoxes[CurrentEditedBBButton];

            SetFocusedWindow(BoundingBoxCreator.Instance, new OpeningInfo(true, new Dictionary<String, Object>()
            {
                { "Mode", "MapHandler_Edit_Mode" },
                { "BoundingBox", BB },
                { "Texture", new BlazeraLib.Texture(CurrentSelectedObjectTexture) }
            }), OnBoundingBoxCreatorEditValidated);
        }

        void OnBoundingBoxCreatorEditValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            CurrentSelectedObject.RemoveEventBoundingBox(CurrentSelectedObjectExternalBoundingBoxes[CurrentEditedBBButton], EventBoundingBoxType.External);

            EBoundingBox BB = e.GetArg<EBoundingBox>("BoundingBox");
            BB.Holder = CurrentSelectedObject;

            CurrentSelectedObject.AddEventBoundingBox(BB, EventBoundingBoxType.External);

            CurrentEditedBBButton.Text = BoundingBoxCreator.Instance.BBToString(BB);
            CurrentSelectedObjectExternalBoundingBoxes[CurrentEditedBBButton] = BB;
        }

        void OnRemoveBoundingBoxFromCurrentSelectedObjectConfirmation()
        {
            RemoveBoundingBoxFromCurrentSelectedObject(CurrentEditedBBButton);
        }

        void SelectorToolRemoveAllButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            SelectorTool.Instance.RemoveSelectedObjectsFromMap();
        }

        void SelectorToolRemoveButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (CurrentSelectedObject == null)
                return;

            SelectorTool.Instance.RemoveSelectedObjectFromMap(CurrentSelectedObject);
        }

        void AddSelectedObject(WorldObject wObj)
        {
            if (SelectedObjects.ContainsKey(wObj.Id))
                return;

            Button objectButton = new Button(wObj.Id, Button.EMode.Label);

            SelectedObjects.Add(objectButton.Text, wObj);
            
            objectButton.Clicked += new ClickEventHandler(objectButton_Clicked);
            SelectorTextList.AddText(objectButton);

            SelectorToolSelectorObjectCountLabel.Text = "( " + SelectedObjects.Count + " )";
        }

        void RemoveSelectedObject(WorldObject wObj)
        {
            SelectedObjects.Remove(wObj.Id);

            SelectorTextList.RemoveText(wObj.Id);

            SelectorToolSelectorObjectCountLabel.Text = "( " + SelectedObjects.Count + " )";

            if (SelectedObjects.Count > 0)
                return;

            SelectorToolCurrentObjectDisplayScreen.SetCurrentPicture(null);

            SelectorToolCurrentObjectConfigurableBox.SetCurrentConfiguration("None");
        }

        void Instance_OnObjectSuppression(SelectorTool sender, SelectedObjectChangeEventArgs e)
        {
            RemoveSelectedObject(e.Object);
        }

        void Instance_OnObjectAddition(SelectorTool sender, SelectedObjectChangeEventArgs e)
        {
            AddSelectedObject(e.Object);
        }

        BlazeraLib.Texture CurrentSelectedObjectTexture;
        void objectButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            CurrentSelectedObjectExternalBoundingBoxes.Clear();

            if (CurrentSelectedObject != null)
                CurrentSelectedObject.Color = SelectedObjects.ContainsValue(CurrentSelectedObject) ? SelectorTool.OBJECT_SELECTION_COLOR : Color.White;

            CurrentSelectedObject = SelectedObjects[((Button)sender).Text];

            CurrentSelectedObjectTexture = new BlazeraLib.Texture(SelectedObjects[((Button)sender).Text].GetSkinTexture());
            CurrentSelectedObjectTexture.Color = Color.White;
            SelectorToolCurrentObjectDisplayScreen.SetCurrentPicture(CurrentSelectedObjectTexture);

            CurrentSelectedObject.Color = CURRENT_SELECTED_OBJECT_COLOR;

            SelectorToolCurrentObjectConfigurableBox.SetCurrentConfiguration(CurrentSelectedObject.GetType().Name);
            SelectorToolElementXUpDownBox.SetCurrentValue((Int32)CurrentSelectedObject.Position.X);
            SelectorToolElementYUpDownBox.SetCurrentValue((Int32)CurrentSelectedObject.Position.Y);

            SelectorToolElementEventTextList.Clear();
            foreach (EBoundingBox BB in CurrentSelectedObject.GetEventBoundingBoxes(EventBoundingBoxType.External))
                AddBoundingBoxToCurrentSelectedObject(BB);
        }

        void SelectTileSetButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            SetFocusedWindow(TileSetMan.Instance, new OpeningInfo(false, new Dictionary<String, Object>()
            {
                { "Mode", "MapHandler_Select_Mode" }
            }), OnTileSetManSelectValidated);
        }

        void OnTileSetManSelectValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            TileSelector.SetTileSet(e.GetArg<TileSet>("TileSet"), tileContainer_Clicked);
        }

        void tileContainer_Clicked(object sender, MouseButtonEventArgs e)
        {
            TilePencil.Instance.SetCurrentTile(((TileContainer)sender).CurrentTile);
        }

        void ObjectPencilFillRandomlyButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            ObjectPencil.Instance.FillMapRandomly(100);
        }

        void ObjectPencilFillButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            ObjectPencil.Instance.FillMap();
        }

        void LockValueUpDownBox_ValueChanged(UpDownBox sender, ValueChangeEventArgs e)
        {
            ObjectPencil.Instance.SetLockValue((UInt32)e.Value);
        }

        void LockPencilCheckBox_Checked(object sender, CheckEventArgs e)
        {
            LockObjectPencilBox.Seal(!e.IsChecked);

            if (!e.IsChecked)
                ObjectPencil.Instance.SetLockValue(1);
            else
                ObjectPencil.Instance.SetLockValue((UInt32)LockValueUpDownBox.GetCurrentValue());
        }

        protected override bool FocusedHandleEvent(BlzEvent evt)
        {
            if (base.FocusedHandleEvent(evt))
                return true;

            if (CurrentTool != null && GetBackground().State == WindowBackground.EState.Restored)
            {
                if (evt.GetType() == BlzEvent.EType.MouseMove)
                {
                    if (CurrentTool.HandleEvent(evt))
                        evt.IsHandled = true;
                }
                else
                {
                    if (CurrentTool.HandleEvent(evt))
                        return true;
                }
            }

            return false;
        }

        void ChangeObjectButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            SetFocusedWindow(ObjectMan.Instance, new OpeningInfo(false, new Dictionary<String, Object>()
            {
                { "Mode", "ObjectPencilSelectMode" }
            }), OnObjectManSelectPencilValidated);
        }

        void OnObjectManSelectPencilValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            ObjectPencil.Instance.SetCurrentObject(e.GetArg<WorldObject>("Object"));
        }

        public override void Update(Time dt)
        {
            base.Update(dt);

            if (PlayerHdl.Vlad.IsMoving())
            {
                XPosUpDownBox.SetCurrentValue((Int32)PlayerHdl.Vlad.Position.X);
                YPosUpDownBox.SetCurrentValue((Int32)PlayerHdl.Vlad.Position.Y);
            }

            if (LockViewCheckBox.IsChecked)
                MapBox.UpdateLockedViewMove(dt, PlayerHdl.Vlad);
            else
                MapBox.UpdateUnlockedViewMove();
        }

        public override void Draw(RenderTarget window)
        {
            base.Draw(window);

            if (CurrentTool == null)
                return;

            CurrentTool.Draw(window);
        }

        public void SetGameRoot(RenderWindow window)
        {
            MapBox.SetGameRoot(window);
        }

        public void SetMap(Map map)
        {
            MapBox.SetMap(map);

            MapWidget currentMapWidget = MapMan.Instance.GetMapWidget(map.Type);
            if (currentMapWidget != null)
            {
                MapBox.ClearWidgetToDraw();
                MapBox.AddWidgetToDraw(currentMapWidget);
            }

            XPosUpDownBox.ChangeValues(0, (Int32)map.Dimension.X);
            YPosUpDownBox.ChangeValues(0, (Int32)map.Dimension.Y);

            SelectorToolElementXUpDownBox.ChangeValues(0, (Int32)map.Dimension.X);
            SelectorToolElementYUpDownBox.ChangeValues(0, (Int32)map.Dimension.Y);

            currentMapWidget.Map.InterruptEvents();
            currentMapWidget.Map.DisableAllMoves();

            currentMapWidget.Map.Init();

            PlayerHdl.Warp(map.Type);
        }

        void AddTool(Tool tool)
        {
            tool.SetMapBox(MapBox);

            Tools.Add(tool.Name, tool);

            ToolRadioButton.AddButton(tool.IconTexture, OnToolRadioButtonChecked, LabeledWidget.EMode.Right, tool.Name);
        }

        void OnToolRadioButtonChecked(object sender, CheckEventArgs e)
        {
            if (!e.IsChecked)
                return;

            if (CurrentTool != null)
            {
                CurrentTool.Reset();
                RemoveWidget(CurrentTool);
            }

            String toolName = ((RadioButton.Item)sender).Name;

            CurrentTool = Tools[toolName];

            ToolConfigurableBox.SetCurrentConfiguration(toolName);

            AddWidget(CurrentTool, false, false);
        }

        Tool TmpCurrentTool;
        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            if (openingInfo == null)
                return;

            if (!openingInfo.IsValid(1))
                return;

            String mode = openingInfo.GetArg<String>("Mode");

            switch (mode)
            {
                case "PointCreatorMode":

                    ToolMainBox.Seal();
                    Closed += new CloseEventHandler(MapHandler_Closed);
                    TmpCurrentTool = CurrentTool;
                    CurrentTool = null;

                    break;
            }
        }

        void MapHandler_Closed(Widget sender, CloseEventArgs e)
        {
            Closed -= new CloseEventHandler(MapHandler_Closed);
            CurrentTool = TmpCurrentTool;
            ToolMainBox.Seal(false);
        }
    }

    public delegate void MapClickEventHandler(MapHandler sender, MapClickEventArgs e);

    public class MapClickEventArgs : EventArgs
    {
        public Vector2f Point { get; private set; }

        public MapClickEventArgs(Vector2f point)
        {
            Point = point;
        }
    }
}
