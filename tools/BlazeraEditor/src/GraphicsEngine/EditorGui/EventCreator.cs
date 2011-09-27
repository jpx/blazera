using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraEditor
{
    public class EventCreator : WindowedWidget
    {
        #region Singleton

        private static EventCreator _instance;
        public static EventCreator Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new EventCreator();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        #region widgets declaration
        VAutoSizeBox EventCreatorMainVBox = new VAutoSizeBox();
        HAutoSizeBox EventCreatorMainHBox = new HAutoSizeBox();
        VAutoSizeBox SettingsBox = new VAutoSizeBox(false, "Settings");
        LabeledDownList TypeDownList = new LabeledDownList("Type", 3);
        CheckBox ActionKeyModeCheckBox = new CheckBox("Action key mode");
        LabeledDownList ActionKeyDownList = new LabeledDownList("Action key", 4);
        VAutoSizeBox ActionBox = new VAutoSizeBox(false, "Actions");
        TextList ActionTextList = new TextList(4, false);
        Button ActionAddButton = new Button("Add action");
        HAutoSizeBox ButtonBox = new HAutoSizeBox();
        Button SaveButton = new Button("Save");
        Button CancelButton = new Button("Cancel");
        #endregion

        Dictionary<Button, BlazeraLib.Action> Actions = new Dictionary<Button, BlazeraLib.Action>();

        EventCreator() :
            base("Event creator")
        {
            #region widgets init
            AddItem(EventCreatorMainVBox);
            EventCreatorMainVBox.AddItem(EventCreatorMainHBox);
            EventCreatorMainHBox.AddItem(SettingsBox);
            foreach (ObjectEventType objectEventType in Enum.GetValues(typeof(ObjectEventType)))
                TypeDownList.DownList.AddText(new Button(objectEventType.ToString(), Button.EMode.LabelEffect));
            SettingsBox.AddItem(TypeDownList);
            ActionKeyModeCheckBox.Checked += new CheckEventHandler(ActionKeyModeCheckBox_Checked);
            SettingsBox.AddItem(ActionKeyModeCheckBox);
            foreach (InputType inputType in Enum.GetValues(typeof(InputType)))
                ActionKeyDownList.DownList.AddText(new Button(inputType.ToString(), Button.EMode.LabelEffect));
            ActionKeyDownList.Seal();
            SettingsBox.AddItem(ActionKeyDownList, 1);
            EventCreatorMainHBox.AddItem(ActionBox);
            ActionBox.AddItem(ActionTextList);
            ActionAddButton.Clicked += new ClickEventHandler(ActionAddButton_Clicked);
            ActionBox.AddItem(ActionAddButton);
            EventCreatorMainVBox.AddItem(ButtonBox);
            SaveButton.Clicked += new ClickEventHandler(SaveButton_Clicked);
            ButtonBox.AddItem(SaveButton);
            CancelButton.Clicked += new ClickEventHandler(CancelButton_Clicked);
            ButtonBox.AddItem(CancelButton);
            #endregion
        }

        #region widgets events
        void CancelButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            Close();
        }

        void SaveButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            CallValidated();
        }

        void ActionAddButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            SetFocusedWindow(ActionCreator.Instance, new OpeningInfo(true), OnActionCreatorAddValidated);
        }

        void OnActionCreatorAddValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            AddAction(e.GetArg<BlazeraLib.Action>("Action"));
        }

        void ActionKeyModeCheckBox_Checked(object sender, CheckEventArgs e)
        {
            ActionKeyDownList.Seal(!e.IsChecked);
        }
        #endregion

        void AddAction(BlazeraLib.Action action)
        {
            Button actionButton = new Button(action.Type.ToString(), Button.EMode.LabelEffect);
            actionButton.Clicked += new ClickEventHandler(actionButton_Clicked);
            ActionTextList.AddText(actionButton);

            Actions.Add(actionButton, action);
        }

        void RemoveAction(Button actionButton)
        {
            ActionTextList.RemoveText(actionButton);
            Actions.Remove(actionButton);
        }

        Button CurrentEditedActionButton;
        void actionButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            CurrentEditedActionButton = (Button)sender;

            if (e.Button == SFML.Window.MouseButton.Right)
            {
                CallConfirmationDialogBox(new String[] { ConfirmationDialogBox.Instance.GetDeletionStr("Action", CurrentEditedActionButton.Text) }, OnActionRemovingConfirmation);

                return;
            }

            SetFocusedWindow(ActionCreator.Instance, new OpeningInfo(true, new Dictionary<String, Object>()
            {
                { "Mode", "EventCreator_Edit_Mode" },
                { "Action", Actions[CurrentEditedActionButton] }
            }), OnActionCreatorEditValidated);
        }

        void OnActionCreatorEditValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            BlazeraLib.Action action = e.GetArg<BlazeraLib.Action>("Action");

            CurrentEditedActionButton.Text = ActionCreator.Instance.ActionToString(action);
            Actions[CurrentEditedActionButton] = action;
        }

        void OnActionRemovingConfirmation()
        {
            RemoveAction(CurrentEditedActionButton);
        }

        public override void Reset()
        {
            base.Reset();

            ActionTextList.Clear();
            Actions.Clear();
        }

        ObjectEvent CurrentEditedEvent;
        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            if (openingInfo == null)
                return;

            if (!openingInfo.IsModeValid())
                return;

            switch (OpeningMode = openingInfo.GetMode())
            {
                case "BoundingBoxCreator_Edit_Mode":

                    CurrentEditedEvent = openingInfo.GetArg<ObjectEvent>("Event");

                    TypeDownList.DownList.SetCurrent(CurrentEditedEvent.Type.ToString());
                    if (CurrentEditedEvent.ActionKeyMode)
                    {
                        ActionKeyModeCheckBox.SetIsChecked(true);
                        ActionKeyDownList.DownList.SetCurrent(CurrentEditedEvent.ActionKey.ToString());
                    }

                    foreach (BlazeraLib.Action action in CurrentEditedEvent.Actions)
                        AddAction(action);

                    break;
            }
        }

        protected override Dictionary<String, Object> OnValidate()
        {
            ObjectEvent objectEvent = new ObjectEvent(
                (ObjectEventType)Enum.Parse(typeof(ObjectEventType),
                TypeDownList.GetCurrent()),
                ActionKeyModeCheckBox.IsChecked,
                (InputType)Enum.Parse(typeof(InputType), ActionKeyDownList.GetCurrent()));

            foreach (BlazeraLib.Action action in Actions.Values)
                objectEvent.AddAction(action);

            return new Dictionary<String, Object>()
            {
                { "Event", objectEvent }
            };
        }

        public String EventToString(ObjectEvent objectEvent)
        {
            return "Event ( " + objectEvent.Type.ToString() + " )";
        }
    }

    public class ActionCreator : WindowedWidget
    {
        #region Singleton

        private static ActionCreator _instance;
        public static ActionCreator Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new ActionCreator();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        #region widgets declaration
        VAutoSizeBox ActionCreatorMainVBox = new VAutoSizeBox();
        HAutoSizeBox ActionCreatorMainHBox = new HAutoSizeBox();
        VAutoSizeBox SettingsBox = new VAutoSizeBox(false, "Settings");
        LabeledDownList TypeDownList = new LabeledDownList("Type", 4);
        ConfigurableBox TypeConfigurableBox = new ConfigurableBox();
        WarpActionBox WarpActionBox = new WarpActionBox();
        TakeItemActionBox TakeItemActionBox = new TakeItemActionBox();
        VAutoSizeBox ConditionBox = new VAutoSizeBox(false, "Conditions");
        TextList ConditionTextList = new TextList(4, false);
        Button ConditionAddButton = new Button("Add condition");
        HAutoSizeBox ButtonBox = new HAutoSizeBox();
        Button SaveButton = new Button("Save");
        Button CancelButton = new Button("Cancel");
        #endregion

        ActionCreator() :
            base("Action creator")
        {
            #region widgets init
            AddItem(ActionCreatorMainVBox);
            ActionCreatorMainVBox.AddItem(ActionCreatorMainHBox);
            ActionCreatorMainHBox.AddItem(SettingsBox);
            foreach (ActionType actionType in Enum.GetValues(typeof(ActionType)))
            {
                Button actionTypeButton = new Button(actionType.ToString(), Button.EMode.LabelEffect);
                actionTypeButton.Clicked += new ClickEventHandler(actionTypeButton_Clicked);
                TypeDownList.DownList.AddText(actionTypeButton);
            }
            SettingsBox.AddItem(TypeDownList);
            SettingsBox.AddItem(TypeConfigurableBox);
            TypeConfigurableBox.AddConfiguration("Warp", WarpActionBox);
            TypeConfigurableBox.AddConfiguration("TakeItem", TakeItemActionBox);

            ActionCreatorMainHBox.AddItem(ConditionBox);
            ConditionBox.AddItem(ConditionTextList);
            ConditionAddButton.Clicked += new ClickEventHandler(ConditionAddButton_Clicked);
            ConditionBox.AddItem(ConditionAddButton);

            ActionCreatorMainVBox.AddItem(ButtonBox);
            SaveButton.Clicked += new ClickEventHandler(SaveButton_Clicked);
            ButtonBox.AddItem(SaveButton);
            CancelButton.Clicked += new ClickEventHandler(CancelButton_Clicked);
            ButtonBox.AddItem(CancelButton);
            #endregion
        }

        #region widgets events
        void CancelButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            Close();
        }

        void SaveButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            CallValidated();
        }

        void actionTypeButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            TypeConfigurableBox.SetCurrentConfiguration(((Button)sender).Text);
        }

        void ConditionAddButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            SetFocusedWindow(ConditionCreator.Instance, new OpeningInfo(true), null);
        }
        #endregion

        public String ActionToString(BlazeraLib.Action action)
        {
            return action.Type.ToString();
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
                case "EventCreator_Edit_Mode":

                    BlazeraLib.Action action = openingInfo.GetArg<BlazeraLib.Action>("Action");
                    TypeDownList.DownList.SetCurrent(action.Type.ToString());

                    switch (action.Type)
                    {
                        case ActionType.Warp:
                            WarpActionBox.SetSettings((WarpAction)action);
                            break;
                    }

                    break;
            }
        }

        protected override Dictionary<String, Object> OnValidate()
        {
            Dictionary<String, Object> onValidate = null;

            ActionType CurrentActionType = (ActionType)Enum.Parse(typeof(ActionType), TypeDownList.GetCurrent());

            switch (CurrentActionType)
            {
                case ActionType.Warp:

                    WarpAction warpAction = (WarpAction)WarpActionBox.GetAction();

                    if (warpAction == null)
                    {
                        CallInformationDialogBox(InformationDialogBox.EType.Error, new String[] { InformationDialogBox.Instance.GetErrorStr() });

                        return base.OnValidate();
                    }

                    onValidate = new Dictionary<String, Object>()
                    {
                        { "Action", warpAction }
                    };

                    break;
            }

            return onValidate;
        }
    }

    public class ConditionCreator : WindowedWidget
    {
        #region Singleton

        private static ConditionCreator _instance;
        public static ConditionCreator Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new ConditionCreator();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        ConditionCreator() :
            base("Condition creator")
        {

        }
    }
}
