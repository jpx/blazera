using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraEditor
{
    public class TileCreator : WindowedWidget
    {
        #region Singleton

        private static TileCreator _instance;
        public static TileCreator Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new TileCreator();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        #region widgets declaration
        LabeledTextBox TypeTextBox = new LabeledTextBox("Type", LabeledWidget.EMode.Right, TextBox.EInputType.All, true);
        CheckBox KeepOldCheckBox = new CheckBox("Keep old");
        VAutoSizeBox TextureBox = new VAutoSizeBox(false, "Texture");
        Button TextureButton = new Button(Button.EMPTY_LABEL, Button.EMode.LabelEffect);
        HAutoSizeBox ButtonBox = new HAutoSizeBox();
        Button SaveButton = new Button("Save");
        Button CancelButton = new Button("Cancel");
        #endregion

        TileCreator() :
            base("Tile creator")
        {
            #region widgets init
            AddItem(TypeTextBox);
            KeepOldCheckBox.Seal();
            AddItem(KeepOldCheckBox);
            AddItem(TextureBox);
            TextureButton.Clicked += new ClickEventHandler(TextureButton_Clicked);
            TextureBox.AddItem(TextureButton);
            AddItem(ButtonBox);
            SaveButton.Clicked += new ClickEventHandler(SaveButton_Clicked);
            ButtonBox.AddItem(SaveButton);
            CancelButton.Clicked += new ClickEventHandler(CancelButton_Clicked);
            ButtonBox.AddItem(CancelButton);
            #endregion
        }

        void TextureButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            SetFocusedWindow(TextureMan.Instance, new OpeningInfo(false, new Dictionary<String, Object>()
                {
                    { "Mode", "TileCreatorMode" }
                }), OnTextureManValidated);
        }

        void OnTextureManValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            TextureButton.Text = e.GetArg<Texture>("Texture").Type;
        }

        void CancelButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            Close();
        }

        void SaveButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            CallValidated();
        }

        public override void Reset()
        {
            base.Reset();

            TextureButton.Text = Button.EMPTY_LABEL;
        }

        String OldType;
        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            if (openingInfo == null)
                return;

            if (!openingInfo.IsValid(1))
                return;

            String mode = openingInfo.GetMode();

            switch (mode)
            {
                case "TileManEditMode":

                    String tileType = openingInfo.GetArg<String>("Type");
                    String textureType = openingInfo.GetArg<String>("TextureType");

                    OldType = tileType;

                    TypeTextBox.TextBox.Reset(tileType);
                    TextureButton.Text = textureType;

                    TypeTextBox.TextBox.TextAdded += new TextAddedEventHandler(TextBox_TextAdded);

                    Closed += new CloseEventHandler(TileCreator_Closed);

                    break;
            }
        }

        void TileCreator_Closed(Widget sender, CloseEventArgs e)
        {
            Closed -= new CloseEventHandler(TileCreator_Closed);

            TypeTextBox.TextBox.TextAdded -= new TextAddedEventHandler(TextBox_TextAdded);

            KeepOldCheckBox.Seal();
        }

        void TextBox_TextAdded(object sender, TextAddedEventArgs e)
        {
            if (OldType == TypeTextBox.TextBox.Text)
            {
                KeepOldCheckBox.SetIsChecked(false);
                KeepOldCheckBox.Seal();
            }
            else
            {
                KeepOldCheckBox.Seal(false);
            }
        }

        protected override Dictionary<String, Object> OnValidate()
        {
            Boolean typeIsValid = TypeTextBox.TextBox.TextIsValid();
            Boolean textureIsValid = TextureButton.Text != Button.EMPTY_LABEL;

            if (!typeIsValid ||
                !textureIsValid)
            {
                List<String> errorMessage = new List<String>();
                if (!typeIsValid) errorMessage.Add(InformationDialogBox.Instance.GetTypeErrorStr());
                if (!textureIsValid) errorMessage.Add(InformationDialogBox.Instance.GetTextureErrorStr());

                CallInformationDialogBox(InformationDialogBox.EType.Error, errorMessage.ToArray());

                return base.OnValidate();
            }

            Tile tile = new Tile();
            tile.SetType(TypeTextBox.TextBox.Text);
            tile.Texture = Create.Texture(TextureButton.Text);

            return new Dictionary<String, Object>()
            {
                { "Tile", tile },
                { "TypeIsChanged", OldType != TypeTextBox.TextBox.Text },
                { "OldTileType", OldType },
                { "KeepOld", KeepOldCheckBox.IsChecked }
            };
        }
    }
}
