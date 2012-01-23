using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraEditor
{
    public class TileSetCreator : WindowedWidget
    {
        #region Singleton

        private static TileSetCreator _instance;
        public static TileSetCreator Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new TileSetCreator();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        const Int32 TILESELECTOR_WIDTH = 4;
        const Int32 TILESELECTOR_HEIGHT = 4;

        #region widgets declaration
        LabeledTextBox TypeTextBox = new LabeledTextBox("Type", LabeledWidget.EMode.Right, TextBox.EInputType.All, true);
        VAutoSizeBox TileSetBox = new VAutoSizeBox(false, "TileSet");
        TileSelector TileSelector = new TileSelector(TILESELECTOR_WIDTH, TILESELECTOR_HEIGHT);
        HAutoSizeBox TileSelectorButtonBox = new HAutoSizeBox();
        Button AddTileButton = new Button("Add tile");

        CheckBox CreateFromTextureCheckBox = new CheckBox("Generate from texture");
        VAutoSizeBox FromTextureBox = new VAutoSizeBox(false, "From texture");
        VAutoSizeBox TextureBox = new VAutoSizeBox(false, "Texture");
        Button TextureButton = new Button(Button.EMPTY_LABEL, Button.EMode.LabelEffect);
        Button TileSetCreateButton = new Button("Generate");

        HAutoSizeBox ButtonBox = new HAutoSizeBox();
        Button SaveButton = new Button("Save");
        Button CancelButton = new Button("Cancel");
        #endregion

        Boolean TextureMode;
        BlazeraLib.Texture CurrentTileSetTexture;

        List<Tile> CurrentTileList = new List<Tile>();

        TileSetCreator() :
            base("TileSet creator")
        {
            #region widgets init
            AddItem(TypeTextBox);
            AddItem(TileSetBox);
            TileSetBox.AddItem(TileSelector);
            TileSetBox.AddItem(TileSelectorButtonBox);
            AddTileButton.Clicked += new ClickEventHandler(AddTileButton_Clicked);
            TileSelectorButtonBox.AddItem(AddTileButton);
            CreateFromTextureCheckBox.Checked += new CheckEventHandler(CreateFromTextureCheckBox_Checked);
            TileSetBox.AddItem(CreateFromTextureCheckBox);
            FromTextureBox.Seal();
            TileSetBox.AddItem(FromTextureBox);
            FromTextureBox.AddItem(TextureBox);
            TextureButton.Clicked += new ClickEventHandler(TextureButton_Clicked);
            TextureBox.AddItem(TextureButton);
            TileSetCreateButton.Clicked += new ClickEventHandler(TileSetCreateButton_Clicked);
            FromTextureBox.AddItem(TileSetCreateButton);
            AddItem(ButtonBox);
            SaveButton.Clicked += new ClickEventHandler(SaveButton_Clicked);
            ButtonBox.AddItem(SaveButton);
            CancelButton.Clicked += new ClickEventHandler(CancelButton_Clicked);
            ButtonBox.AddItem(CancelButton);
            #endregion

            TextureMode = false;
        }

        void CancelButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        void SaveButton_Clicked(object sender, MouseButtonEventArgs e)
        {
            CallValidated();
        }

        void AddTileButton_Clicked(object sender, MouseButtonEventArgs e)
        {
             SetFocusedWindow(TileMan.Instance, new OpeningInfo(false, new Dictionary<String, Object>()
             {
                 { "Mode", "TileSetCreator_Add_Mode" }
             }), OnTileManAddValidated);
        }

        public Boolean CurrentTileListContains(String tileType)
        {
            foreach (Tile tile in CurrentTileList)
                if (tile.Type == tileType)
                    return true;

            return false;
        }

        void OnTileManAddValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            Tile tile = e.GetArg<Tile>("Tile");

            AddTile(tile);

            TileSelector.AddTile(tile, OnTileClick);
        }

        void TileSetCreateButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            if (TextureButton.Text == Button.EMPTY_LABEL)
            {
                CallInformationDialogBox(InformationDialogBox.EType.Error, new String[] { InformationDialogBox.Instance.GetTextureErrorStr() });
                return;
            }

            CurrentTileList.Clear();

            Int32 width = (Int32)(CurrentTileSetTexture.Dimension.X / GameData.TILE_SIZE);
            Int32 height = (Int32)(CurrentTileSetTexture.Dimension.Y / GameData.TILE_SIZE);
            Int32 count = 0;

            for (Int32 y = 0; y < height; ++y)
            {
                for (Int32 x = 0; x < width; ++x)
                {
                    BlazeraLib.Texture tileTexture = new BlazeraLib.Texture(CurrentTileSetTexture);
                    tileTexture.ImageSubRect = new BlazeraLib.IntRect(
                        x * GameData.TILE_SIZE,
                        y * GameData.TILE_SIZE,
                        x * GameData.TILE_SIZE + GameData.TILE_SIZE,
                        y * GameData.TILE_SIZE + GameData.TILE_SIZE);
                    tileTexture.SetType("Tile_" + CurrentTileSetTexture.Type + "_" + count);
                    //tileTexture.ToScript();

                    Tile tile = new Tile();
                    tile.SetType(CurrentTileSetTexture.Type + "_" + count);
                    tile.Texture = tileTexture;
                    //tile.ToScript();
                    ++count;

                    AddTile(tile);
                }
            }

            TileSelector.SetTileSet(CurrentTileList, width, OnTextureModeTileClick);
        }

        void AddTile(Tile tile)
        {
            CurrentTileList.Add(tile);
        }

        TileContainer CurrentEditedTileContainer;
        void OnTileClick(object sender, MouseButtonEventArgs e)
        {
            CurrentEditedTileContainer = (TileContainer)sender;

            if (e.Button == Mouse.Button.Right)
            {
                CallConfirmationDialogBox(new String[] { ConfirmationDialogBox.Instance.GetDeletionStr("Tile", CurrentEditedTileContainer.GetCurrentTileType()) }, RemoveTile);

                return;
            }

            SetFocusedWindow(TileMan.Instance, new OpeningInfo(false, new Dictionary<String, Object>()
                {
                    { "Mode", "TileSetCreator_Edit_Mode" }
                }), OnTileManEditValidated);
        }

        void OnTextureModeTileClick(object sender, MouseButtonEventArgs e)
        {
            CurrentEditedTileContainer = (TileContainer)sender;

            if (e.Button != Mouse.Button.Right)
                return;

            CallConfirmationDialogBox(new String[] { ConfirmationDialogBox.Instance.GetDeletionStr("Tile", CurrentEditedTileContainer.GetCurrentTileType()) }, RemoveTile);
        }

        void RemoveTile()
        {
            TileSelector.RemoveTile(CurrentEditedTileContainer);
            CurrentTileList.Remove(CurrentEditedTileContainer.CurrentTile);
        }

        void OnTileManEditValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            CurrentTileList.Remove(CurrentEditedTileContainer.CurrentTile);
            Tile tile = e.GetArg<Tile>("Tile");
            CurrentEditedTileContainer.SetContent(tile);
            CurrentTileList.Add(CurrentEditedTileContainer.CurrentTile);
        }

        void TextureButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            SetFocusedWindow(TextureMan.Instance, new OpeningInfo(false, new Dictionary<String, Object>()
            {
                { "Mode", "TileSetCreatorMode" }
            }), OnTextureManValidated);
        }

        void OnTextureManValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            CurrentTileSetTexture = e.GetArg<BlazeraLib.Texture>("Texture");
            TextureButton.Text = CurrentTileSetTexture.Type;
        }

        void CreateFromTextureCheckBox_Checked(object sender, CheckEventArgs e)
        {
            FromTextureBox.Seal(!e.IsChecked);

            TextureMode = e.IsChecked;

            if (!e.IsChecked)
            {
                TextureButton.Text = Button.EMPTY_LABEL;
                AddTileButton.Seal(false);
                TileSelector.ResetWidth();
            }
            else
            {
                AddTileButton.Seal();
            }

            TypeTextBox.TextBox.Reset();
            TileSelector.Clear();
            CurrentTileList.Clear();
        }

        public override void Reset()
        {
            base.Reset();

            CreateFromTextureCheckBox.SetIsChecked(false);

            TextureButton.Text = Button.EMPTY_LABEL;
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
                case "TileSetMan_Edit_Mode":

                    TypeTextBox.TextBox.Reset(openingInfo.GetArg<String>("TileSetType"));
                    TileSelector.SetTileSet(openingInfo.GetArg<TileSet>("TileSet"));

                    break;
            }
        }

        protected override Dictionary<String, Object> OnValidate()
        {
            Boolean typeIsValid = TypeTextBox.TextBox.TextIsValid();
            Boolean tileSetIsValid = CurrentTileList.Count > 0;

            if (!typeIsValid ||
                !tileSetIsValid)
            {
                List<String> errorMessages = new List<String>();
                if (!typeIsValid) errorMessages.Add(InformationDialogBox.Instance.GetTypeErrorStr());
                if (!tileSetIsValid) errorMessages.Add("Wrong TileSet !");
                CallInformationDialogBox(InformationDialogBox.EType.Error, errorMessages.ToArray());

                return base.OnValidate();
            }

            TileSet tileSet = new TileSet();
            tileSet.SetType(TypeTextBox.TextBox.Text);

            foreach (Tile tile in CurrentTileList)
            {
                if (TextureMode)
                {
                    tile.Texture.ToScript();
                    tile.ToScript();
                }

                tileSet.AddTileType(tile.Type);
            }

            return new Dictionary<String, Object>()
            {
                { "TileSet", tileSet }
            };
        }
    }
}
