using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraEditor
{
    public class TileSetMan : WindowedWidget
    {
        #region Singleton

        private static TileSetMan _instance;
        public static TileSetMan Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new TileSetMan();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        #region widgets declaration
        HAutoSizeBox TileSetBox = new HAutoSizeBox(false, "TileSets");
        TextList TileSetTextList = new TextList(5, false);
        VAutoSizeBox ButtonBox = new VAutoSizeBox();
        Button CreateButton = new Button("Create");
        Button RefreshButton = new Button("Refresh");
        #endregion

        TileSetMan() :
            base("TileSet manager")
        {
            #region widgets init
            AddItem(TileSetBox);
            TileSetBox.AddItem(TileSetTextList);
            TileSetBox.AddItem(ButtonBox);

            CreateButton.Clicked += new ClickEventHandler(CreateButton_Clicked);
            ButtonBox.AddItem(CreateButton);
            RefreshButton.Clicked += new ClickEventHandler(TileSetRefreshButton_Clicked);
            ButtonBox.AddItem(RefreshButton);
            #endregion

            LoadTileSets();
        }

        void CreateButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            SetFocusedWindow(TileSetCreator.Instance, new OpeningInfo(true), OnTileSetCreatorAddValidated);
        }

        void OnTileSetCreatorAddValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            TileSet tileSet = e.GetArg<TileSet>("TileSet");
            tileSet.ToScript();

            AddTileSet(tileSet);
        }

        void AddTileSet(TileSet tileSet)
        {
            Button tileSetButton = new Button(tileSet.Type, Button.EMode.LabelEffect);
            TileSetTextList.AddText(tileSetButton);
            tileSetButton.Clicked += new ClickEventHandler(tileSetButton_Clicked);
        }

        void LoadTileSets()
        {
            foreach (String type in FileReader.Instance.GetTileSetTypes())
                AddTileSet(Create.TileSet(type));
        }

        void RefreshTileSets()
        {
            TileSetTextList.Clear();
            LoadTileSets();
        }

        String CurrentEditedTileSetType;
        void tileSetButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            CurrentEditedTileSetType = ((Button)sender).Text;
            if (e.Button == MouseButton.Right)
            {
                CallConfirmationDialogBox(new String[] { ConfirmationDialogBox.Instance.GetDeletionStr("TileSet", CurrentEditedTileSetType) }, RemoveTileSet);
                return;
            }

            SetFocusedWindow(TileSetCreator.Instance, new OpeningInfo(true, new Dictionary<String, Object>()
                {
                    { "Mode", "TileSetMan_Edit_Mode" },
                    { "TileSetType", TileSetTextList.GetCurrent() },
                    { "TileSet", Create.TileSet(TileSetTextList.GetCurrent()) }
                }), OnTileCreatorEditValidated);
        }

        void RemoveTileSet()
        {
            FileManager.Instance.RemoveTileSet(CurrentEditedTileSetType);
            TileSetTextList.RemoveText(CurrentEditedTileSetType);
        }

        void OnTileCreatorEditValidated(WindowedWidget sender, ValidateEventArgs e)
        {

        }

        void TileSetRefreshButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            RefreshTileSets();
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
                case "MapHandler_Select_Mode":

                    TileSetTextList.RemoveEvent(tileSetButton_Clicked);
                    TileSetTextList.AddEvent(OnTileSetButtonClick);

                    Closed += new CloseEventHandler(TileSetMan_Closed);
                    break;
            }
        }

        void TileSetMan_Closed(Widget sender, CloseEventArgs e)
        {
            Closed -= new CloseEventHandler(TileSetMan_Closed);
            TileSetTextList.RemoveEvent(OnTileSetButtonClick);
            TileSetTextList.AddEvent(tileSetButton_Clicked);
        }

        String CurrentChosenTileSet;
        void OnTileSetButtonClick(object sender, MouseButtonEventArgs e)
        {
            CurrentChosenTileSet = ((Button)sender).Text;
            CallValidated();
        }

        protected override Dictionary<String, Object> OnValidate()
        {
            if (OpeningMode != "MapHandler_Select_Mode")
                return base.OnValidate();

            return new Dictionary<String, Object>()
            {
                { "TileSet", Create.TileSet(CurrentChosenTileSet) }
            };
        }
    }
}
