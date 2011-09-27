using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public abstract class DialogBox : WindowedWidget
    {
        public enum EType
        {
            Confirmation,
            Information
        }

        public delegate void DOnValidate();

        protected VAutoSizeBox MessageBox;

        protected DOnValidate OnDialogValidate;

        protected DialogBox(String title) :
            base(title)
        {
            MessageBox = new VAutoSizeBox();
            AddItem(MessageBox, 0, HAlignment.Center);

            Validated += new ValidateEventHandler(DialogBox_Validated);
        }

        void DialogBox_Validated(WindowedWidget sender, ValidateEventArgs e)
        {
            CallOnDialogValidate();
        }

        protected void CallOnDialogValidate()
        {
            if (OnDialogValidate != null)
                OnDialogValidate();
        }

        protected override Dictionary<String, Object> OnValidate()
        {
            return new Dictionary<String, Object>();
        }
    }

    public class ConfirmationDialogBox : DialogBox
    {
        #region Singleton

        private static ConfirmationDialogBox _instance;
        public static ConfirmationDialogBox Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new ConfirmationDialogBox();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        HAutoSizeBox ButtonBox;

        Button YesButton;
        Button NoButton;

        private ConfirmationDialogBox() :
            base("Confirmation")
        {
            ButtonBox = new HAutoSizeBox();
            AddItem(ButtonBox, 0, HAlignment.Center);

            YesButton = new Button("Yes", Button.EMode.BackgroundLabel, true);
            YesButton.Clicked += new ClickEventHandler(YesButton_Clicked);
            ButtonBox.AddItem(YesButton);

            NoButton = new Button("No", Button.EMode.BackgroundLabel, true);
            NoButton.Clicked += new ClickEventHandler(NoButton_Clicked);
            ButtonBox.AddItem(NoButton);
        }

        void NoButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            Close();
        }

        void YesButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            CallOnDialogValidate();
            Close();
        }

        public void InitDialog(String[] messages, DOnValidate onDialogValidate = null)
        {
            MessageBox.Clear();

            foreach (String message in messages)
            {
                Label messageLabel = new Label(message);
                MessageBox.AddItem(messageLabel);
            }

            OnDialogValidate = onDialogValidate;
        }

        public String GetDeletionStr(BaseObject obj)
        {
            return "Do you really want to remove " + obj.GetType().Name + " :: " + obj.Type + " :: ?";
        }

        public String GetDeletionStr(String baseType, String type)
        {
            return "Do you really want to remove " + baseType + " :: " + type + " :: ?";
        }
    }

    public class InformationDialogBox : DialogBox
    {
        #region Singleton

        private static InformationDialogBox _instance;
        public static InformationDialogBox Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new InformationDialogBox();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        public enum EType
        {
            Information,
            Error
        }

        Button OkButton;

        private InformationDialogBox() :
            base(null)
        {
            OkButton = new Button("Ok", Button.EMode.BackgroundLabel, true);
            OkButton.Clicked += new ClickEventHandler(OkButton_Clicked);
            AddItem(OkButton, 0, HAlignment.Center);
        }

        void OkButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            Close();
        }

        public void InitDialog(EType type, String[] messages)
        {
            MessageBox.Clear();

            foreach (String message in messages)
            {
                Label messageLabel = new Label(message);
                MessageBox.AddItem(messageLabel);
            }

            GetBackground().SetTitle(Enum.GetName(typeof(EType), type));
        }

        public String GetTypeErrorStr()
        {
            return "Wrong type name !";
        }

        public String GetRectErrorStr()
        {
            return "Wrong rect !";
        }

        public String GetTextureErrorStr()
        {
            return "Wrong texture !";
        }

        public String GetDeletionErrorStr(String baseType, String type)
        {
            return "Impossible to remove " + baseType + " :: " + type + " :: !";
        }

        public String GetErrorStr()
        {
            return "Wrong settings !";
        }
    }
}
