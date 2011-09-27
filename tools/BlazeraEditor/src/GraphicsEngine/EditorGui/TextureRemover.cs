using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraEditor
{
    public class TextureRemover : WindowedWidget
    {
        #region Singleton

        private static TextureRemover _instance;
        public static TextureRemover Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new TextureRemover();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        #region widgets declaration
        Label ConfirmationLabel = new Label();
        CheckBox RemoveImageCheckBox = new CheckBox("Remove image", LabeledWidget.EMode.Left, false, true);

        HAutoSizeBox ButtonBox = new HAutoSizeBox();
        Button YesButton = new Button("Yes", Button.EMode.BackgroundLabel, true);
        Button NoButton = new Button("No", Button.EMode.BackgroundLabel, true);
        #endregion

        String TextureToRemove;
        String ImageToRemove;

        private TextureRemover() :
            base("Confirmation")
        {
            #region widgets init
            AddItem(ConfirmationLabel);
            AddItem(RemoveImageCheckBox);

            AddItem(ButtonBox, 0, HAlignment.Center);
            YesButton.Clicked += new ClickEventHandler(YesButton_Clicked);
            ButtonBox.AddItem(YesButton);
            NoButton.Clicked += new ClickEventHandler(NoButton_Clicked);
            ButtonBox.AddItem(NoButton);
            #endregion
        }

        void NoButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            Close();
        }

        void YesButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            if (!RemoveImageCheckBox.IsChecked)
            {
                CallValidated();
                return;
            }

            CallConfirmationDialogBox(new String[] { ConfirmationDialogBox.Instance.GetDeletionStr("Image", ImageToRemove) }, RemoveImage);
            ConfirmationDialogBox.Instance.Closed += new CloseEventHandler(Instance_Closed);
        }

        void Instance_Closed(Widget sender, CloseEventArgs e)
        {
            ConfirmationDialogBox.Instance.Closed -= new CloseEventHandler(Instance_Closed);
            Reset();
        }

        void RemoveImage()
        {
            CallValidated();
            Log.Cl("Image removed : " + ImageToRemove);
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            if (openingInfo == null)
                return;

            if (!openingInfo.IsValid())
                return;

            TextureToRemove = openingInfo.GetArg<String>("TextureToRemove");
            ImageToRemove = openingInfo.GetArg<String>("ImageToRemove");

            ConfirmationLabel.Text = ConfirmationDialogBox.Instance.GetDeletionStr("Texture", TextureToRemove);
        }

        protected override Dictionary<String, Object> OnValidate()
        {
            return new Dictionary<String, Object>();
        }
    }
}
