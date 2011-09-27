using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraEditor
{
    public class TileMan : WindowedWidget
    {
        #region Singleton

        private static TileMan _instance;
        public static TileMan Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new TileMan();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        #region widgets declaration
        VAutoSizeBox TileMainBox = new VAutoSizeBox(false, "Tiles");

        HAutoSizeBox TileBox = new HAutoSizeBox();
        TextList TileTextList = new TextList(5);
        TileContainer TileTileContainer = new TileContainer();

        HAutoSizeBox TileButtonBox = new HAutoSizeBox();
        Button TileCreateButton = new Button("Create");
        Button TileEditButton = new Button("Edit");
        Button TileRemoveButton = new Button("Remove");
        Button TileRefreshButton = new Button("Refresh");
        Button TileSelectButton = new Button("Select");
        #endregion

        private TileMan() :
            base("Tile manager")
        {
            #region widgets init
            AddItem(TileMainBox);

            TileMainBox.AddItem(TileBox, 0, HAlignment.Center);
            TileBox.AddItem(TileTextList);
            TileTileContainer.Clicked += new ClickEventHandler(TileEditButton_Clicked);
            TileBox.AddItem(TileTileContainer);

            TileMainBox.AddItem(TileButtonBox);
            TileCreateButton.Clicked += new ClickEventHandler(TileCreateButton_Clicked);
            TileButtonBox.AddItem(TileCreateButton);
            TileEditButton.Clicked += new ClickEventHandler(TileEditButton_Clicked);
            TileButtonBox.AddItem(TileEditButton);
            TileRemoveButton.Clicked += new ClickEventHandler(TileRemoveButton_Clicked);
            TileButtonBox.AddItem(TileRemoveButton);
            TileRefreshButton.Clicked += new ClickEventHandler(TileRefreshButton_Clicked);
            TileButtonBox.AddItem(TileRefreshButton);
            TileSelectButton.Seal();
            TileSelectButton.Clicked += new ClickEventHandler(TileSelectButton_Clicked);
            TileButtonBox.AddItem(TileSelectButton);
            #endregion

            LoadTiles();
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            if (evt.IsHandled)
                return base.OnEvent(evt);

            switch (evt.Type)
            {
                case EventType.KeyPressed:

                    switch (evt.Key.Code)
                    {
                        case KeyCode.Delete:

                            TileRemoveButton_Clicked(null, null);

                            return true;
                    }

                    break;
            }

            return base.OnEvent(evt);
        }

        void TileRefreshButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            RefreshTiles();
        }

        void TileRemoveButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            CallConfirmationDialogBox(new String[]
            {
                ConfirmationDialogBox.Instance.GetDeletionStr("Tile", TileTextList.GetCurrent())
            }, OnRemoveValidate);
        }

        void OnRemoveValidate()
        {
            RemoveCurrentTile();
        }

        void TileEditButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            SetFocusedWindow(TileCreator.Instance, new OpeningInfo(true, new Dictionary<String, Object>()
                {
                    { "Mode", "TileManEditMode" },
                    { "Type", TileTileContainer.GetCurrentTileType() },
                    { "TextureType", TileTileContainer.GetCurrentTexture().Type }
                }), OnTileCreatorEditValidated);
        }

        void TileCreateButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            SetFocusedWindow(TileCreator.Instance, new OpeningInfo(true), OnTileCreatorAddValidated);
        }

        void OnTileCreatorAddValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            Tile tile = e.GetArg<Tile>("Tile");
            tile.ToScript();
            ScriptManager.Refresh<Tile>(tile.Type);

            if (TileTextList.ContainsText(tile.Type))
            {
                ScriptManager.Refresh<Tile>(tile.Type);

                if (TileTextList.GetCurrent() == tile.Type)
                    TileTileContainer.SetContent(tile.Type);
            }
            else
                AddTile(tile.Type);
        }

        void OnTileCreatorEditValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            Tile tile = e.GetArg<Tile>("Tile");
            tile.ToScript();

            if (e.GetArg<Boolean>("TypeIsChanged"))
            {
                if (!e.GetArg<Boolean>("KeepOld"))
                {
                    String oldTileType = e.GetArg<String>("OldTileType");
                    FileManager.Instance.RemoveTile(oldTileType);
                    if (TileTextList.ContainsText(tile.Type))
                    {
                        ScriptManager.Refresh<Tile>(tile.Type);
                        TileTextList.RemoveText(oldTileType);
                    }
                    else
                        TileTextList.SetCurrentText(tile.Type);
                }
                else
                {
                    if (TileTextList.ContainsText(tile.Type))
                        ScriptManager.Refresh<Tile>(tile.Type);
                    else
                        AddTile(tile.Type);
                }
            }
            else
                ScriptManager.Refresh<Tile>(tile.Type);

            TileTileContainer.SetContent(TileTextList.GetCurrent());
        }

        

        void RemoveCurrentTile()
        {
            FileManager.Instance.RemoveTile(TileTextList.GetCurrent());
            TileTextList.RemoveCurrent();
        }

        void LoadTiles()
        {
            List<String> tileTypes = new List<String>();
            foreach (String tileType in FileReader.Instance.GetTileTypes())
            {
                tileTypes.Add(tileType);
                ScriptManager.Refresh<Tile>(tileType);
            }

            AddTile(tileTypes);
        }

        void AddTile(List<String> tileTypes)
        {
            List<Button> tileButtons = new List<Button>();

            foreach (String tileType in tileTypes)
            {
                Button tileButton = new Button(tileType, Button.EMode.Label);
                tileButton.Clicked += new ClickEventHandler(tileButton_Clicked);
                tileButtons.Add(tileButton);
            }

            TileTextList.AddText(tileButtons, 0);
        }

        void AddTile(String tileType)
        {
            Button tileButton = new Button(tileType, Button.EMode.Label);
            if (TileTextList.GetTextCount() == 0)
                TileTileContainer.SetContent(tileType);
            TileTextList.AddText(tileButton);
            tileButton.Clicked += new ClickEventHandler(tileButton_Clicked);
            ScriptManager.Refresh<Tile>(tileType);
        }

        void tileButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            TileTileContainer.SetContent(((Button)sender).Text);
        }

        void RefreshTiles()
        {
            TileTextList.Clear();
            LoadTiles();
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            if (openingInfo == null)
                return;

            if (!openingInfo.IsModeValid())
                return;

            switch (OpeningMode = openingInfo.GetMode())
            {
                case "TileSetCreator_Edit_Mode":
                case "TileSetCreator_Add_Mode":

                    TileSelectButton.Seal(false);

                    TileTileContainer.Clicked += new ClickEventHandler(TileTileContainer_Clicked);
                    TileTileContainer.Clicked -= new ClickEventHandler(TileEditButton_Clicked);
                    Closed += new CloseEventHandler(TileMan_Closed);

                    break;
            }
        }

        void TileTileContainer_Clicked(object sender, MouseButtonEventArgs e)
        {
            CallValidated();
        }

        void TileMan_Closed(Widget sender, CloseEventArgs e)
        {
            Closed -= new CloseEventHandler(TileMan_Closed);

            TileTileContainer.Clicked -= new ClickEventHandler(TileTileContainer_Clicked);
            TileTileContainer.Clicked += new ClickEventHandler(TileEditButton_Clicked);
            TileSelectButton.Seal();
        }

        void TileSelectButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            CallValidated();
        }

        protected override Dictionary<String, Object> OnValidate()
        {
            if (OpeningMode == null)
                return base.OnValidate();

            Tile tile = Create.Tile(TileTileContainer.GetCurrentTileType());

            if ((OpeningMode == "TileSetCreator_Add_Mode" || OpeningMode == "TileSetCreator_Edit_Mode") &&
                TileSetCreator.Instance.CurrentTileListContains(tile.Type))
            {
                CallInformationDialogBox(InformationDialogBox.EType.Error, new String[] { "Tile :: " + tile.Type + " :: already exists in the TileSet !" });

                return base.OnValidate();
            }

            return new Dictionary<String, Object>()
            {
                { "Tile", tile }
            };
        }
    }
}
