using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraEditor
{
    public class TextureMan : WindowedWidget
    {
        enum Filter
        {
            Animation,
            WorldItem,
            GroundElement,
            Element,
            DisplaceableElement,
            Gui,
            Tile,
            TileSet,
            Wall,
        }

        #region Singleton

        private static TextureMan _instance;
        public static TextureMan Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new TextureMan();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        #region widgets declaration
        VAutoSizeBox TextureMainBox = new VAutoSizeBox(false, "Textures");
        HAutoSizeBox TextureBox = new HAutoSizeBox();

        TextList TextureTextList = new TextList(8);

        DisplayScreen DisplayScreen = new DisplayScreen();

        HAutoSizeBox ButtonBox = new HAutoSizeBox();
        Button CreateButton = new Button("Create", Button.EMode.BackgroundLabel, true);
        Button EditButton = new Button("Edit", Button.EMode.BackgroundLabel, true);
        Button RemoveButton = new Button("Remove", Button.EMode.BackgroundLabel, true);
        Button RefreshButton = new Button("Refresh");
        Button SelectButton = new Button("Select");

        VAutoSizeBox FilterBox = new VAutoSizeBox(false, "Filter");
        CheckBox FilterCheckBox = new CheckBox("Active");
        DownList FilterDownList = new DownList(4);
        #endregion

        Dictionary<String, Texture> Textures = new Dictionary<String, Texture>();

        private TextureMan() :
            base("Texture manager")
        {
            #region widgets init
            AddItem(TextureMainBox);

            TextureMainBox.AddItem(TextureBox);

            TextureBox.AddItem(TextureTextList);

            TextureBox.AddItem(DisplayScreen);

            TextureMainBox.AddItem(ButtonBox);

            CreateButton.Clicked += new ClickEventHandler(CreateButton_Clicked);
            ButtonBox.AddItem(CreateButton);
            ButtonBox.AddItem(EditButton);
            RemoveButton.Clicked += new ClickEventHandler(RemoveButton_Clicked);
            ButtonBox.AddItem(RemoveButton);
            RefreshButton.Clicked += new ClickEventHandler(RefreshButton_Clicked);
            ButtonBox.AddItem(RefreshButton);
            SelectButton.Seal();
            ButtonBox.AddItem(SelectButton);

            AddItem(FilterBox);
            FilterCheckBox.Checked += new CheckEventHandler(FilterCheckBox_Checked);
            FilterBox.AddItem(FilterCheckBox);
            FilterDownList.Seal();
            FilterBox.AddItem(FilterDownList, 1);
            #endregion

            InitFilters();

            LoadTextures();
        }

        void FilterCheckBox_Checked(object sender, CheckEventArgs e)
        {
            FilterDownList.Seal(!e.IsChecked);

            RefreshTextures();
        }

        void CreateButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            SetFocusedWindow(TextureCreator.Instance, new OpeningInfo(true), OnTextureCreatorValidated);
        }

        void OnTextureCreatorValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            Texture texture = e.GetArg<Texture>("Texture");

            texture.ToScript();

            RefreshTextures();
        }

        #region widgets events
        void RemoveButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            SetFocusedWindow(TextureRemover.Instance, new OpeningInfo(true, new Dictionary<String, Object>
            {
                { "TextureToRemove", GetCurrentType() },
                { "ImageToRemove", GetCurrentTexture().ImagePath }
            }), RemoveTexture);
        }

        void RemoveTexture(WindowedWidget sender, ValidateEventArgs e)
        {
            //FileManager.Instance.RemoveTexture(GetCurrentType());

            //RefreshTextures();

            Log.Cl("Texture removed : " + GetCurrentType());
        }

        void RefreshButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            RefreshTextures();
        }

        void AddType(String type)
        {
            if (!Textures.ContainsKey(type))
                Textures.Add(type, Create.Texture(type));

            Button typeButton = new Button(type, Button.EMode.Label);
            typeButton.Clicked += new ClickEventHandler(typeButton_Clicked);
            TextureTextList.AddText(typeButton);
        }

        void AddType(List<String> types)
        {
            List<Button> texts = new List<Button>();
            foreach (String type in types)
            {
                if (!Textures.ContainsKey(type))
                    Textures.Add(type, Create.Texture(type));

                Button typeButton = new Button(type, Button.EMode.Label);
                typeButton.Clicked += new ClickEventHandler(typeButton_Clicked);
                texts.Add(typeButton);
            }

            TextureTextList.AddText(texts);
        }

        void typeButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            DisplayScreen.SetCurrentPicture(new Texture(GetTexture(((Button)sender).Text)));
        }
        #endregion

        void LoadTextures()
        {
            List<String> types = new List<String>();
            foreach (String type in FileReader.Instance.GetTextureTypes(GetCurrentFilter()))
                types.Add(type);

            AddType(types);

            if (TextureTextList.GetTextCount() > 0)
                DisplayScreen.SetCurrentPicture(GetCurrentTexture());
            else
                DisplayScreen.SetCurrentPicture(null);
        }

        void RefreshTextures()
        {
            TextureTextList.Clear();

            LoadTextures();
        }

        Texture GetTexture(String type)
        {
            return Textures[type];
        }

        Texture GetCurrentTexture()
        {
            return GetTexture(GetCurrentType());
        }

        String GetCurrentType()
        {
            return TextureTextList.GetCurrent();
        }

        void InitFilters()
        {
            foreach (Filter filter in Enum.GetValues(typeof(Filter)))
            {
                Button filterButton = new Button(filter.ToString(), Button.EMode.LabelEffect);
                FilterDownList.AddText(filterButton);
                filterButton.Clicked += new ClickEventHandler(filterButton_Clicked);
            }
        }

        void filterButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            RefreshTextures();
        }

        String[] GetCurrentFilter()
        {
            return FilterCheckBox.IsChecked ? new String[] { FilterDownList.GetCurrent() } : null;
        }

        protected override Dictionary<String, Object> OnValidate()
        {
            Texture texture = GetCurrentTexture();

            if (texture == null)
                return base.OnValidate();

            return new Dictionary<String, Object>()
            {
                { "Texture", new Texture(texture) }
            };
        }

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
                case "ObjectCreatorMode":

                    FilterCheckBox.SetIsChecked(true);
                    FilterCheckBox.Seal();
                    FilterDownList.SetCurrent(openingInfo.GetArg<String>("BaseType"));
                    FilterDownList.Seal();

                    SelectButton.Seal(false);
                    SelectButton.Clicked += new ClickEventHandler(SelectButton_Clicked);

                    DisplayScreen.ScreenClicked += new ClickEventHandler(DisplayScreen_ScreenClicked);

                    Closed += new CloseEventHandler(TextureMan_Closed);

                    break;

                case "TileSetCreatorMode":

                    FilterCheckBox.SetIsChecked(true);
                    FilterCheckBox.Seal();
                    FilterDownList.SetCurrent("TileSet");
                    FilterDownList.Seal();

                    SelectButton.Seal(false);
                    SelectButton.Clicked += new ClickEventHandler(SelectButton_Clicked);

                    DisplayScreen.ScreenClicked += new ClickEventHandler(DisplayScreen_ScreenClicked);

                    Closed += new CloseEventHandler(TextureMan_Closed);

                    break;

                case "TileCreatorMode":

                    FilterCheckBox.SetIsChecked(true);
                    FilterCheckBox.Seal();
                    FilterDownList.SetCurrent("Tile");
                    FilterDownList.Seal();

                    SelectButton.Seal(false);
                    SelectButton.Clicked += new ClickEventHandler(SelectButton_Clicked);

                    DisplayScreen.ScreenClicked += new ClickEventHandler(DisplayScreen_ScreenClicked);

                    Closed += new CloseEventHandler(TextureMan_Closed);

                    break;
            }
        }

        void TextureMan_Closed(Widget sender, CloseEventArgs e)
        {
            Closed -= new CloseEventHandler(TextureMan_Closed);
            FilterCheckBox.Seal(false);
            FilterDownList.Seal(false);
            SelectButton.Seal();
        }

        void DisplayScreen_ScreenClicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            DisplayScreen.ScreenClicked -= new ClickEventHandler(DisplayScreen_ScreenClicked);

            CallValidated();
        }

        void SelectButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            SelectButton.Clicked -= new ClickEventHandler(SelectButton_Clicked);

            CallValidated();
        }
    }
}
