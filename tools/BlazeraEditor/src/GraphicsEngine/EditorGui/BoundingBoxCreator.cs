using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraEditor
{
    public class BoundingBoxCreator : WindowedWidget
    {
        enum BoundingBoxType
        {
            BBoundingBox,
            EBoundingBox
        }

        #region Singleton

        private static BoundingBoxCreator _instance;
        public static BoundingBoxCreator Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new BoundingBoxCreator();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        const Int32 RECT_MINVALUE = -999;
        const Int32 RECT_MAXVALUE = 999;

        #region widgets declaration
        DownList TypeDownList = new DownList(2);

        VAutoSizeBox RectBox = new VAutoSizeBox(false, "Rect");
        UpDownBox LeftUpDownBox = new UpDownBox(RECT_MINVALUE, RECT_MAXVALUE, 1, 0, "Left", LabeledWidget.EMode.Left);
        UpDownBox TopUpDownBox = new UpDownBox(RECT_MINVALUE, RECT_MAXVALUE, 1, 0, "Top", LabeledWidget.EMode.Left);
        UpDownBox RightUpDownBox = new UpDownBox(RECT_MINVALUE, RECT_MAXVALUE, 1, 0, "Right", LabeledWidget.EMode.Left);
        UpDownBox BottomUpDownBox = new UpDownBox(RECT_MINVALUE, RECT_MAXVALUE, 1, 0, "Bottom", LabeledWidget.EMode.Left);
        Button DrawButton = new Button("Draw");

        ConfigurableBox ConfigurableBox = new ConfigurableBox();
        VAutoSizeBox BBoundingBoxBox = new VAutoSizeBox();
        VAutoSizeBox EBoundingBoxEventBox = new VAutoSizeBox(false, "Events");
        TextList EventTextList = new TextList(4, false);
        Button EventAddButton = new Button("Add event");

        HAutoSizeBox ButtonBox = new HAutoSizeBox();
        Button SaveButton = new Button("Save");
        Button CancelButton = new Button("Cancel");
        #endregion

        Texture CurrentTexture;
        
        Dictionary<Button, ObjectEvent> Events = new Dictionary<Button, ObjectEvent>();

        EBoundingBox CurrentEventBB;

        private BoundingBoxCreator() :
            base("BB creator")
        {
            Name = "BBB";
            DrawButton.Name = "DDD";

            #region widgets init
            AddItem(TypeDownList);

            AddItem(RectBox);
            RectBox.AddItem(LeftUpDownBox);
            RectBox.AddItem(TopUpDownBox);
            RectBox.AddItem(RightUpDownBox);
            RectBox.AddItem(BottomUpDownBox);
            DrawButton.Clicked += new ClickEventHandler(DrawButton_Clicked);
            RectBox.AddItem(DrawButton, 0, HAlignment.Right);

            AddItem(ConfigurableBox);
            ConfigurableBox.AddConfiguration("BBoundingBox", BBoundingBoxBox);
            ConfigurableBox.AddConfiguration("EBoundingBox", EBoundingBoxEventBox);
            EBoundingBoxEventBox.AddItem(EventTextList);
            EventAddButton.Clicked += new ClickEventHandler(EventAddButton_Clicked);
            EBoundingBoxEventBox.AddItem(EventAddButton);

            AddItem(ButtonBox);
            SaveButton.Clicked += new ClickEventHandler(SaveButton_Clicked);
            ButtonBox.AddItem(SaveButton);
            CancelButton.Clicked += new ClickEventHandler(CancelButton_Clicked);
            ButtonBox.AddItem(CancelButton);
            #endregion

            InitTypes();
        }

        void EventAddButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            SetFocusedWindow(EventCreator.Instance, new OpeningInfo(true), OnEventCreatorAddValidated);
        }

        void OnEventCreatorAddValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            AddEvent(e.GetArg<ObjectEvent>("Event"));
        }

        void SetRect(Int32 left, Int32 top, Int32 right, Int32 bottom)
        {
            LeftUpDownBox.SetCurrentValue(left);
            TopUpDownBox.SetCurrentValue(top);
            RightUpDownBox.SetCurrentValue(right);
            BottomUpDownBox.SetCurrentValue(bottom);
        }

        void SetRect(IntRect rect)
        {
            SetRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        void SetRect(BoundingBox BB)
        {
            SetRect(BB.BaseLeft, BB.BaseTop, BB.BaseRight, BB.BaseBottom);
        }

        void DrawButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            SetFocusedWindow(TextureRectDrawer.Instance, new OpeningInfo(true, new Dictionary<String, Object>()
                {
                    { "Mode", "BoundingBoxCreator_Mode" },
                    { "Texture", CurrentTexture }
                }), OnTextureRectDrawerValidate);
        }

        void OnTextureRectDrawerValidate(WindowedWidget sender, ValidateEventArgs e)
        {
            SetRect(e.GetArg<IntRect>("Rect"));
        }

        void CancelButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            Close();
        }

        void SaveButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            CallValidated();
        }

        void InitTypes()
        {
            foreach (BoundingBoxType type in Enum.GetValues(typeof(BoundingBoxType)))
            {
                Button typeButton = new Button(type.ToString(), Button.EMode.LabelEffect);
                TypeDownList.AddText(typeButton);
                typeButton.Clicked += new ClickEventHandler(typeButton_Clicked);
            }
        }

        void typeButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            ConfigurableBox.SetCurrentConfiguration(((Button)sender).Text);
        }

        String GetCurrentBaseType()
        {
            return TypeDownList.GetCurrent();
        }

        public override void Reset()
        {
            base.Reset();

            EventTextList.Clear();
            Events.Clear();
        }

        protected override Dictionary<String, Object> OnValidate()
        {
            Int32 left = LeftUpDownBox.GetCurrentValue();
            Int32 top = TopUpDownBox.GetCurrentValue();
            Int32 right = RightUpDownBox.GetCurrentValue();
            Int32 bottom = BottomUpDownBox.GetCurrentValue();

            if (left > right ||
                top > bottom)
            {
                CallInformationDialogBox(InformationDialogBox.EType.Error, new String[] { InformationDialogBox.Instance.GetRectErrorStr() });
                return base.OnValidate();
            }

            BoundingBox BB;

            if (GetCurrentBaseType() == "BBoundingBox")
                BB = new BBoundingBox(null, left, top, right, bottom);
            else
            {
                BB = new EBoundingBox(null, EBoundingBoxType.Event, left, top, right, bottom);

                foreach (ObjectEvent objectEvent in Events.Values)
                    ((EBoundingBox)BB).AddEvent(objectEvent);
            }

            return new Dictionary<String, Object>()
            {
                { "BoundingBox", BB }
            };
        }

        public String BBToString(BBoundingBox BB)
        {
            return "B ( " + BB.BaseLeft.ToString() + ", " + BB.BaseTop.ToString() + ", " + BB.BaseRight.ToString() + ", " + BB.BaseBottom.ToString() + " )";
        }

        public String BBToString(EBoundingBox BB)
        {
            return "E ( " + BB.BaseLeft.ToString() + ", " + BB.BaseTop.ToString() + ", " + BB.BaseRight.ToString() + ", " + BB.BaseBottom.ToString() + " )";
        }

        void AddEvent(ObjectEvent objectEvent)
        {
            Button objectEventButton = new Button(EventCreator.Instance.EventToString(objectEvent), Button.EMode.LabelEffect);
            objectEventButton.Clicked += new ClickEventHandler(objectEventButton_Clicked);
            EventTextList.AddText(objectEventButton);

            Events.Add(objectEventButton, objectEvent);
        }

        void RemoveEvent(Button objectEventButton)
        {
            EventTextList.RemoveText(objectEventButton);
            Events.Remove(objectEventButton);
        }

        Button CurrentEditedEventButton;
        void objectEventButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            CurrentEditedEventButton = (Button)sender;

            if (e.Button == SFML.Window.Mouse.Button.Right)
            {
                CallConfirmationDialogBox(new String[] { ConfirmationDialogBox.Instance.GetDeletionStr("Event", CurrentEditedEventButton.Text) }, OnEventRemoveConfirmation);

                return;
            }

            SetFocusedWindow(EventCreator.Instance, new OpeningInfo(true, new Dictionary<String, Object>()
            {
                { "Mode", "BoundingBoxCreator_Edit_Mode" },
                { "Event", Events[CurrentEditedEventButton] }
            }), OnEventCreatorEditValidated);
        }

        void OnEventCreatorEditValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            ObjectEvent objectEvent = e.GetArg<ObjectEvent>("Event");
            CurrentEditedEventButton.Text = EventCreator.Instance.EventToString(objectEvent);
            Events[CurrentEditedEventButton] = objectEvent;
        }

        void OnEventRemoveConfirmation()
        {
            RemoveEvent(CurrentEditedEventButton);
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
                case "ObjectCreator_Add_Mode":

                    TypeDownList.SetCurrent("BBoundingBox");
                    TypeDownList.Seal();

                    if (!openingInfo.GetArg<Boolean>("TextureIsValid"))
                        DrawButton.Seal();
                    else
                        DrawButton.Seal(false);

                    CurrentTexture = openingInfo.GetArg<Texture>("Texture");

                    break;
                    
                case "ObjectCreator_Edit_Mode":

                    TypeDownList.SetCurrent("BBoundingBox");
                    TypeDownList.Seal();

                    if (!openingInfo.GetArg<Boolean>("TextureIsValid"))
                        DrawButton.Seal();
                    else
                        DrawButton.Seal(false);

                    CurrentTexture = openingInfo.GetArg<Texture>("Texture");

                    SetRect(openingInfo.GetArg<IntRect>("Rect"));

                    break;

                case "MapHandler_Add_Mode":

                    TypeDownList.SetCurrent("EBoundingBox");
                    TypeDownList.Seal();

                    if ((CurrentTexture = openingInfo.GetArg<Texture>("Texture")) == null)
                        DrawButton.Seal();
                    else
                        DrawButton.Seal(false);

                    break;

                case "MapHandler_Edit_Mode":

                    TypeDownList.SetCurrent("EBoundingBox");
                    TypeDownList.Seal();

                    if ((CurrentTexture = openingInfo.GetArg<Texture>("Texture")) == null)
                        DrawButton.Seal();
                    else
                        DrawButton.Seal(false);

                    CurrentEventBB = openingInfo.GetArg<EBoundingBox>("BoundingBox");

                    SetRect(CurrentEventBB);
                    foreach (ObjectEvent objectEvent in CurrentEventBB.Events)
                        AddEvent(objectEvent);

                    break;
            }
        }
    }
}