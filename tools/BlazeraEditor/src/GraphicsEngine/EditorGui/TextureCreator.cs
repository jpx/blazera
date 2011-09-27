using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraEditor
{
    public class TextureCreator : WindowedWidget
    {
        #region Singleton

        private static TextureCreator _instance;
        public static TextureCreator Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new TextureCreator();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        const String SUBRECT_BUTTON_DEFAULT_EMPTY_TEXT = "Rect ( )";

        #region widgets declaration
        VAutoSizeBox SettingBox = new VAutoSizeBox(false, "Settings");
        LabeledTextBox TypeTextBox = new LabeledTextBox("Type", LabeledWidget.EMode.Right, TextBox.EInputType.All, true);

        HAutoSizeBox ImageBox = new HAutoSizeBox(false, "Image", 6F);
        DownList ImageDownList = new DownList(5);
        Button RefreshButton = new Button("Refresh", Button.EMode.BackgroundLabel, true);

        VAutoSizeBox SubRectBox = new VAutoSizeBox(false, "Sub rect");
        CheckBox SubRectCheckBox = new CheckBox("Active", LabeledWidget.EMode.Left, false, true);
        Button SubRectButton = new Button(SUBRECT_BUTTON_DEFAULT_EMPTY_TEXT, Button.EMode.LabelEffect, true);

        HAutoSizeBox ButtonBox = new HAutoSizeBox();
        Button SaveButton = new Button("Save", Button.EMode.BackgroundLabel, true);
        Button CancelButton = new Button("Cancel", Button.EMode.BackgroundLabel, true);

        CheckBox FolderModeCheckBox = new CheckBox("Folder processing");
        VAutoSizeBox FolderBox = new VAutoSizeBox(false, "Folder");
        LabeledTextBox FolderNameTextBox = new LabeledTextBox("Folder name");
        Button ProcessButton = new Button("Process");
        #endregion

        IntRect CurrentImageSubRect;

        private TextureCreator() :
            base("Texture creator")
        {
            #region widgets init
            // setting box
            AddItem(SettingBox);
            SettingBox.AddItem(TypeTextBox);
            
            // image box
            SettingBox.AddItem(ImageBox);
            ImageBox.AddItem(ImageDownList);
            ImageBox.AddItem(RefreshButton);

            // sub rect box
            SettingBox.AddItem(SubRectBox);
            SubRectCheckBox.Checked += new CheckEventHandler(SubRectCheckBox_Checked);
            SubRectBox.AddItem(SubRectCheckBox);
            SubRectButton.Seal();
            SubRectButton.Clicked += new ClickEventHandler(SubRectButton_Clicked);
            SubRectBox.AddItem(SubRectButton, 1);

            // button box
            AddItem(ButtonBox);
            SaveButton.Clicked += new ClickEventHandler(SaveButton_Clicked);
            ButtonBox.AddItem(SaveButton);
            CancelButton.Clicked += new ClickEventHandler(CancelButton_Clicked);
            ButtonBox.AddItem(CancelButton);

            // folder mode
            FolderModeCheckBox.Checked += new CheckEventHandler(FolderModeCheckBox_Checked);
            AddItem(FolderModeCheckBox);
            FolderBox.Seal();
            AddItem(FolderBox);
            FolderBox.AddItem(FolderNameTextBox);
            ProcessButton.Clicked += new ClickEventHandler(ProcessButton_Clicked);
            FolderBox.AddItem(ProcessButton);
            #endregion

            LoadImages();
        }

        void ProcessButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            ProcessFolder();
        }

        void ProcessFolder()
        {
            if (!FolderNameTextBox.TextBox.TextIsValid())
            {
                CallInformationDialogBox(InformationDialogBox.EType.Error, new string[] { "Invalid folder name." });
                return;
            }

            Animation anim = new Animation();
            anim.SetType(FolderNameTextBox.TextBox.Text);

            uint count = 0;
            foreach (string imageName in FileReader.Instance.GetImages(FolderNameTextBox.TextBox.Text))
            {
                string textureType = "Animation_" + FolderNameTextBox.TextBox.Text + count++.ToString();

                Texture texture = new Texture();
                texture.SetType(textureType);
                texture.ImagePath = FolderNameTextBox.TextBox.Text + "/" + imageName;
                texture.ToScript();

                anim.AddFrame(texture);
            }

            anim.ToScript();
        }

        void FolderModeCheckBox_Checked(object sender, CheckEventArgs e)
        {
            FolderBox.Seal(!e.IsChecked);

            SettingBox.Seal(e.IsChecked);
            ButtonBox.Seal(e.IsChecked);
        }

        void SubRectButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            Texture currentTexture = new Texture();
            currentTexture.ImagePath = ImageDownList.GetCurrent();

            SetFocusedWindow(TextureRectDrawer.Instance, new OpeningInfo(true, new Dictionary<String, Object>()
            {
                { "Mode", "TextureCreator_Mode" },
                { "Texture", currentTexture },
                { "RectIsValid", SubRectButton.Text != SUBRECT_BUTTON_DEFAULT_EMPTY_TEXT },
                { "Rect", CurrentImageSubRect }
            }), OnTextureRectDrawerValidation);
        }

        void OnTextureRectDrawerValidation(WindowedWidget sender, ValidateEventArgs e)
        {
            IntRect rect = e.GetArg<IntRect>("Rect");
            CurrentImageSubRect = rect;
            SubRectButton.Text = TextureRectDrawer.Instance.RectToString(rect);
        }

        void SubRectCheckBox_Checked(object sender, CheckEventArgs e)
        {
            SubRectButton.Seal(!e.IsChecked);

            SubRectButton.Text = SUBRECT_BUTTON_DEFAULT_EMPTY_TEXT;

            if (e.IsChecked)
                TypeTextBox.TextBox.Reset();
            else
                TypeTextBox.TextBox.Reset(FilterExtension(ImageDownList.GetCurrent()));
        }

        void CancelButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            Close();
        }

        void SaveButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            CallValidated();
        }

        void LoadImages()
        {
            foreach (String imagePath in FileReader.Instance.GetImages())
            {
                Button imageButton = new Button(imagePath, Button.EMode.LabelEffect);
                imageButton.Clicked += new ClickEventHandler(imageButton_Clicked);
                ImageDownList.AddText(imageButton);
            }
        }

        void imageButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            if (!SubRectCheckBox.IsChecked)
                TypeTextBox.TextBox.Reset(FilterExtension(((Button)sender).Text));
        }

        void RefreshImages()
        {
            ImageDownList.Reset();

            LoadImages();
        }

        public override void Reset()
        {
            base.Reset();

            SubRectButton.Text = SUBRECT_BUTTON_DEFAULT_EMPTY_TEXT;
        }

        String FilterExtension(String imageName)
        {
            if (imageName.Length == 0)
                return "";

            String[] extensions = new String[]
            {
                ".png",
                ".PNG"
            };

            foreach (String extention in extensions)
            {
                if (imageName.Length < extention.Length)
                    continue;

                String ext = imageName.Substring(imageName.Length - extention.Length, extention.Length);
                if (ext == extention)
                    return imageName.Substring(0, imageName.Length - extention.Length);
            }

            return imageName;
        }

        protected override Dictionary<String, Object> OnValidate()
        {
            if (!TypeTextBox.TextBox.TextIsValid())
            {
                CallInformationDialogBox(InformationDialogBox.EType.Error, new String[] { InformationDialogBox.Instance.GetTypeErrorStr() });
                return base.OnValidate();
            }

            Texture texture = new Texture();
            texture.SetType(TypeTextBox.TextBox.Text);
            texture.ImagePath = ImageDownList.GetCurrent();

            if (SubRectCheckBox.IsChecked)
            {
                texture.ImageSubRect = CurrentImageSubRect;
            }

            return new Dictionary<String, Object>
            {
                { "Texture", texture }
            };
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            if (!SubRectCheckBox.IsChecked)
                SubRectButton.Seal();
        }
    }
}
