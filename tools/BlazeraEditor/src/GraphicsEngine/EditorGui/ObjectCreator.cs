using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraEditor
{
    public class ObjectCreator : WindowedWidget
    {
        #region Singleton

        private static ObjectCreator _instance;
        public static ObjectCreator Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new ObjectCreator();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        #region widgets
        // Type
        VAutoSizeBox TypeBox = new VAutoSizeBox(false, "Object type");
        DownList BaseTypeDownList = new DownList(4);
        LabeledTextBox TypeTextBox = new LabeledTextBox("Type", LabeledWidget.EMode.Right);

        ConfigurableBox MainConfigurableBox = new ConfigurableBox();

        // Element
        VAutoSizeBox ElementBox = new VAutoSizeBox(false, "Element");
        VAutoSizeBox ElementTextureBox = new VAutoSizeBox(false, "Texture");
        Button ElementTextureButton = new Button(Button.EMPTY_LABEL, Button.EMode.LabelEffect);
        HAutoSizeBox ElementBoundingBoxBox = new HAutoSizeBox(false, "Bounding box");
        TextList ElementBoundingBoxTextList = new TextList(4, false);
        Button ElementAddBoundingBoxButton = new Button("Add");

        // DisplaceableElement
        VAutoSizeBox DisplaceableElementBox = new VAutoSizeBox(false, "DisplaceableElement");

        // GroundElement
        VAutoSizeBox GroundElementBox = new VAutoSizeBox(false, "GroundElement");
        VAutoSizeBox GroundElementTextureBox = new VAutoSizeBox(false, "Texture");
        Button GroundElementTextureButton = new Button(Button.EMPTY_LABEL, Button.EMode.LabelEffect);

        // Wall
        VAutoSizeBox WallBox = new VAutoSizeBox(false, "Wall");
        VAutoSizeBox WallTextureBox = new VAutoSizeBox(false, "Texture");
        Button WallTextureButton = new Button(Button.EMPTY_LABEL, Button.EMode.LabelEffect);

        // Buttons
        HAutoSizeBox ButtonBox = new HAutoSizeBox();
        Button SaveButton = new Button("Save");
        Button CancelButton = new Button("Cancel");
        #endregion

        Dictionary<Button, BBoundingBox> ElementBBoundingBoxes = new Dictionary<Button, BBoundingBox>();
        Texture CurrentTexture;

        ObjectCreator() :
            base("Object creator")
        {
            #region widgets init
            AddItem(TypeBox);
            TypeBox.AddItem(BaseTypeDownList);
            TypeBox.AddItem(TypeTextBox);

            AddItem(MainConfigurableBox);

            MainConfigurableBox.AddConfiguration("Element", ElementBox);
            ElementBox.AddItem(ElementTextureBox);
            ElementTextureButton.Clicked += new ClickEventHandler(TextureButton_Clicked);
            ElementTextureBox.AddItem(ElementTextureButton);
            ElementBox.AddItem(ElementBoundingBoxBox);
            ElementBoundingBoxBox.AddItem(ElementBoundingBoxTextList);
            ElementAddBoundingBoxButton.Clicked += new ClickEventHandler(ElementAddBoundingBoxButton_Clicked);
            ElementBoundingBoxBox.AddItem(ElementAddBoundingBoxButton);

            MainConfigurableBox.AddConfiguration("DisplaceableElement", DisplaceableElementBox);

            MainConfigurableBox.AddConfiguration("GroundElement", GroundElementBox);
            GroundElementBox.AddItem(GroundElementTextureBox);
            GroundElementTextureButton.Clicked += new ClickEventHandler(GroundElementTextureButton_Clicked);
            GroundElementTextureBox.AddItem(GroundElementTextureButton);

            MainConfigurableBox.AddConfiguration("Wall", WallBox);
            WallBox.AddItem(WallTextureBox);
            WallTextureButton.Clicked += new ClickEventHandler(WallTextureButton_Clicked);
            WallTextureBox.AddItem(WallTextureButton);

            AddItem(ButtonBox);
            SaveButton.Clicked += new ClickEventHandler(SaveButton_Clicked);
            ButtonBox.AddItem(SaveButton);
            CancelButton.Clicked += new ClickEventHandler(CancelButton_Clicked);
            ButtonBox.AddItem(CancelButton);
            #endregion

            InitBaseTypes();
        }

        void WallTextureButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            SetFocusedWindow(TextureMan.Instance, new OpeningInfo(false, new Dictionary<String, Object>()
                {
                    { "Mode", "ObjectCreatorMode" },
                    { "BaseType", GetCurrentBaseTypeStr() }
                }), OnWallTextureManValidated);
        }

        void OnWallTextureManValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            CurrentTexture = e.GetArg<Texture>("Texture");
            WallTextureButton.Text = CurrentTexture.Type;
        }

        void GroundElementTextureButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            SetFocusedWindow(TextureMan.Instance, new OpeningInfo(false, new Dictionary<String, Object>()
                {
                    { "Mode", "ObjectCreatorMode" },
                    { "BaseType", GetCurrentBaseTypeStr() }
                }), OnGroundElementTextureManValidated);
        }

        void OnGroundElementTextureManValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            CurrentTexture = e.GetArg<Texture>("Texture");
            GroundElementTextureButton.Text = CurrentTexture.Type;
        }

        void ElementAddBoundingBoxButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            SetFocusedWindow(BoundingBoxCreator.Instance, new OpeningInfo(true, new Dictionary<String, Object>()
            { 
                { "Mode", "ObjectCreator_Add_Mode" },
                { "TextureIsValid", ElementTextureIsValid() },
                { "Texture", CurrentTexture }
            }), OnBoundingBoxCreatorAddValidated);
        }

        void OnBoundingBoxCreatorAddValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            BBoundingBox BB = e.GetArg<BBoundingBox>("BoundingBox");

            Button BBButton = new Button(BoundingBoxCreator.Instance.BBToString(BB), Button.EMode.LabelEffect);
            ElementBoundingBoxTextList.AddText(BBButton);
            BBButton.Clicked += new ClickEventHandler(BBButton_Clicked);

            ElementBBoundingBoxes.Add(BBButton, BB);
        }

        Button CurrentEditedBBBButton;
        void BBButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            CurrentEditedBBBButton = (Button)sender;
            if (e.Button == SFML.Window.MouseButton.Right)
            {
                CallConfirmationDialogBox(new String[] { ConfirmationDialogBox.Instance.GetDeletionStr("BBoundingBox", ((Button)sender).Text) }, RemoveBBB);
                return;
            }

            SetFocusedWindow(BoundingBoxCreator.Instance, new OpeningInfo(true, new Dictionary<String, Object>()
            { 
                { "Mode", "ObjectCreator_Edit_Mode" },
                { "TextureIsValid", ElementTextureIsValid() },
                { "Texture", CurrentTexture },
                { "Rect", ElementBBoundingBoxes[CurrentEditedBBBButton].GetBaseRect() }
            }), OnBoundingBoxCreatorEditValidated);
        }

        void OnBoundingBoxCreatorEditValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            ElementBBoundingBoxes[CurrentEditedBBBButton] = e.GetArg<BBoundingBox>("BoundingBox");
            CurrentEditedBBBButton.Text = BoundingBoxCreator.Instance.BBToString(ElementBBoundingBoxes[CurrentEditedBBBButton]);
        }

        void RemoveBBB()
        {
            ElementBoundingBoxTextList.RemoveText(CurrentEditedBBBButton);
            ElementBBoundingBoxes.Remove(CurrentEditedBBBButton);
        }

        void TextureButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            SetFocusedWindow(TextureMan.Instance, new OpeningInfo(false, new Dictionary<String, Object>()
                {
                    { "Mode", "ObjectCreatorMode" },
                    { "BaseType", GetCurrentBaseTypeStr() }
                }), OnTextureManValidated);
        }

        void OnTextureManValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            CurrentTexture = e.GetArg<Texture>("Texture");
            ElementTextureButton.Text = CurrentTexture.Type;
        }

        void SaveButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            CallValidated();
        }

        void CancelButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            Close();
        }

        void InitBaseTypes()
        {
            foreach (WorldObjectBaseType baseType in Enum.GetValues(typeof(WorldObjectBaseType)))
            {
                Button baseTypeButton = new Button(baseType.ToString(), Button.EMode.LabelEffect);
                BaseTypeDownList.AddText(baseTypeButton);
                baseTypeButton.Clicked += new ClickEventHandler(baseTypeButton_Clicked);
            }
        }

        void baseTypeButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            MainConfigurableBox.SetCurrentConfiguration(((Button)sender).Text);

            TypeTextBox.Reset();
            ElementTextureButton.Text = Button.EMPTY_LABEL;
        }

        String GetCurrentBaseTypeStr()
        {
            return BaseTypeDownList.GetCurrent();
        }

        WorldObjectBaseType GetCurrentBaseType()
        {
            return (WorldObjectBaseType)Enum.Parse(typeof(WorldObjectBaseType), GetCurrentBaseTypeStr());
        }

        public override void Reset()
        {
            base.Reset();

            ElementTextureButton.Text = Button.EMPTY_LABEL;

            ElementBoundingBoxTextList.Clear();
            ElementBBoundingBoxes.Clear();
        }

        Boolean ElementTextureIsValid()
        {
            return ElementTextureButton.Text != Button.EMPTY_LABEL;
        }

        protected override Dictionary<String, Object> OnValidate()
        {
            Boolean typeIsValid = TypeTextBox.TextBox.TextIsValid();
            Boolean textureIsValid;

            WorldObject wObj = null;

            switch (GetCurrentBaseType())
            {
                case WorldObjectBaseType.Element:

                    textureIsValid = ElementTextureButton.Text != Button.EMPTY_LABEL;

                    if (!typeIsValid ||
                        !textureIsValid)
                    {
                        List<String> errorMessages = new List<String>();
                        if (!typeIsValid) errorMessages.Add(InformationDialogBox.Instance.GetTypeErrorStr());
                        if (!textureIsValid) errorMessages.Add(InformationDialogBox.Instance.GetTextureErrorStr());
                        CallInformationDialogBox(InformationDialogBox.EType.Error, errorMessages.ToArray());

                        return base.OnValidate();
                    }

                    wObj = new Element();
                    wObj.Skin = Create.Texture(ElementTextureButton.Text);

                    foreach (BBoundingBox BB in ElementBBoundingBoxes.Values)
                        wObj.AddBoundingBox(BB);

                    break;

                case WorldObjectBaseType.DisplaceableElement:

                    wObj = new DisplaceableElement();
                    wObj.Skin = Create.Texture(ElementTextureButton.Text);

                    foreach (BBoundingBox BB in ElementBBoundingBoxes.Values)
                        wObj.AddBoundingBox(BB);

                    break;

                case WorldObjectBaseType.GroundElement:

                    textureIsValid = GroundElementTextureButton.Text != Button.EMPTY_LABEL;

                    if (!typeIsValid ||
                        !textureIsValid)
                    {
                        List<String> errorMessages = new List<String>();
                        if (!typeIsValid) errorMessages.Add(InformationDialogBox.Instance.GetTypeErrorStr());
                        if (!textureIsValid) errorMessages.Add(InformationDialogBox.Instance.GetTextureErrorStr());
                        CallInformationDialogBox(InformationDialogBox.EType.Error, errorMessages.ToArray());

                        return base.OnValidate();
                    }

                    wObj = new GroundElement();
                    wObj.Skin = Create.Texture(GroundElementTextureButton.Text);

                    break;

                case WorldObjectBaseType.Wall:

                    textureIsValid = WallTextureButton.Text != Button.EMPTY_LABEL;

                    if (!typeIsValid ||
                        !textureIsValid)
                    {
                        List<String> errorMessages = new List<String>();
                        if (!typeIsValid) errorMessages.Add(InformationDialogBox.Instance.GetTypeErrorStr());
                        if (!textureIsValid) errorMessages.Add(InformationDialogBox.Instance.GetTextureErrorStr());
                        CallInformationDialogBox(InformationDialogBox.EType.Error, errorMessages.ToArray());

                        return base.OnValidate();
                    }

                    wObj = new Wall();
                    ((Wall)wObj).SetBase(Create.Texture(WallTextureButton.Text));

                    break;
            }

            wObj.SetType(TypeTextBox.TextBox.Text);

            return new Dictionary<String, Object>()
                {
                    { "Object", wObj },
                    { "BaseType", GetCurrentBaseType() }
                };
        }
    }
}
