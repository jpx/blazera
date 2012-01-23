using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraEditor
{
    public enum WorldObjectBaseType
    {
        Element,
        DisplaceableElement,
        GroundElement,
        WorldItem,
        Wall
    }

    public class ObjectMan : WindowedWidget
    {
        #region Singleton

        private static ObjectMan _instance;
        public static ObjectMan Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new ObjectMan();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        #region widgets
        VAutoSizeBox ObjectMainBox = new VAutoSizeBox(false, "Objects");
        HAutoSizeBox ObjectBox = new HAutoSizeBox();

        VAutoSizeBox ObjectListBox = new VAutoSizeBox();
        DownList ObjectBaseTypeDownList = new DownList(4);
        TextList ObjectTextList = new TextList(8);

        DisplayScreen DisplayScreen = new DisplayScreen();

        HAutoSizeBox ButtonBox = new HAutoSizeBox();
        Button CreateButton = new Button("Create", Button.EMode.BackgroundLabel, true);
        Button EditButton = new Button("Edit", Button.EMode.BackgroundLabel, true);
        Button RemoveButton = new Button("Remove", Button.EMode.BackgroundLabel, true);
        Button RefreshButton = new Button("Refresh");
        Button SelectButton = new Button("Select");

        #endregion

        Dictionary<WorldObjectBaseType, Dictionary<String, WorldObject>> Objects = new Dictionary<WorldObjectBaseType, Dictionary<String, WorldObject>>();

        private ObjectMan() :
            base("Object manager")
        {
            #region widgets init
            AddItem(ObjectMainBox);

            ObjectMainBox.AddItem(ObjectBox);

            ObjectBox.AddItem(ObjectListBox);
            ObjectListBox.AddItem(ObjectBaseTypeDownList, 0, HAlignment.Center);

            ObjectListBox.AddItem(ObjectTextList, 0, HAlignment.Center);

            ObjectBox.AddItem(DisplayScreen);

            ObjectMainBox.AddItem(ButtonBox);

            CreateButton.Clicked += new ClickEventHandler(CreateButton_Clicked);
            ButtonBox.AddItem(CreateButton);
            ButtonBox.AddItem(EditButton);
            RemoveButton.Clicked += new ClickEventHandler(RemoveButton_Clicked);
            ButtonBox.AddItem(RemoveButton);
            RefreshButton.Clicked += new ClickEventHandler(RefreshButton_Clicked);
            ButtonBox.AddItem(RefreshButton);
            SelectButton.Seal();
            ButtonBox.AddItem(SelectButton);
            #endregion

            InitBaseTypes();

            LoadObjects();
        }

        void CreateButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            SetFocusedWindow(ObjectCreator.Instance, new OpeningInfo(true), OnObjectCreatorValidated);
        }

        void OnObjectCreatorValidated(WindowedWidget sender, ValidateEventArgs e)
        {
            WorldObject wObj = e.GetArg<WorldObject>("Object");
            wObj.ToScript();

            AddType(e.GetArg<WorldObjectBaseType>("BaseType"), wObj.Type);
        }

        void RemoveButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            CallConfirmationDialogBox(new String[] { ConfirmationDialogBox.Instance.GetDeletionStr(GetCurrentObject()) }, RemoveObject);
        }

        void RemoveObject()
        {
            FileManager.Instance.RemoveObject(GetCurrentBaseType().ToString(), GetCurrentType());

            RefreshObjects();
        }

        void RefreshButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            RefreshObjects();
        }

        void InitBaseTypes()
        {
            foreach (WorldObjectBaseType baseType in Enum.GetValues(typeof(WorldObjectBaseType)))
            {
                Objects.Add(baseType, new Dictionary<String, WorldObject>());

                Button baseTypeButton = new Button(baseType.ToString(), Button.EMode.LabelEffect);
                ObjectBaseTypeDownList.AddText(baseTypeButton);
                baseTypeButton.Clicked += new ClickEventHandler(baseTypeButton_Clicked);
            }
        }

        void baseTypeButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            RefreshObjects();
        }

        void AddType(WorldObjectBaseType baseType, String type)
        {
            if (!Objects[baseType].ContainsKey(type))
                Objects[baseType].Add(type, GetObjectFromType(baseType, type));

            Button typeButton = new Button(type, Button.EMode.Label);
            typeButton.Clicked += new ClickEventHandler(typeButton_Clicked);
            ObjectTextList.AddText(typeButton);
        }

        void typeButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            DisplayScreen.SetCurrentPicture(new Texture(GetObject(((Button)sender).Text).GetSkinTexture()));
        }

        WorldObject GetObjectFromType(WorldObjectBaseType baseType, String type)
        {
            switch (baseType)
            {
                case WorldObjectBaseType.Element: return Create.Element(type);
                case WorldObjectBaseType.DisplaceableElement: return Create.DisplaceableElement(type);
                case WorldObjectBaseType.GroundElement: return Create.GroundElement(type);
                case WorldObjectBaseType.WorldItem: return Create.WorldItem(type);
                case WorldObjectBaseType.Wall: return Create.Wall(type);
                default: return null;
            }
        }

        void LoadObjects()
        {
            String strBaseType = ObjectBaseTypeDownList.GetCurrent();
            foreach (String type in FileReader.Instance.GetObjectTypes(strBaseType))
            {
                AddType(GetCurrentBaseType(), type);
            }

            if (ObjectTextList.GetTextCount() > 0)
                DisplayScreen.SetCurrentPicture(new Texture(GetObject(((Button)ObjectTextList.GetAt(0)).Text).GetSkinTexture()));
        }

        void RefreshObjects()
        {
            ObjectTextList.Clear();

            LoadObjects();

            if (ObjectTextList.GetTextCount() == 0)
                DisplayScreen.SetCurrentPicture(null);
        }

        WorldObject GetObject(String type)
        {
            return Objects[GetCurrentBaseType()][type];
        }

        WorldObjectBaseType GetCurrentBaseType()
        {
            return (WorldObjectBaseType)Enum.Parse(typeof(WorldObjectBaseType), ObjectBaseTypeDownList.GetCurrent());
        }

        String GetCurrentType()
        {
            return ObjectTextList.GetCurrent();
        }

        WorldObject GetCurrentObject()
        {
            return GetObject(GetCurrentType());
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
                case "ObjectPencilSelectMode":

                    SelectButton.Seal(false);
                    SelectButton.Clicked += new ClickEventHandler(SelectButton_Clicked);

                    DisplayScreen.ScreenClicked += new ClickEventHandler(DisplayScreen_ScreenClicked);

                    Closed += new CloseEventHandler(ObjectMan_Closed);

                    break;
            }
        }

        void ObjectMan_Closed(Widget sender, CloseEventArgs e)
        {
            Closed -= new CloseEventHandler(ObjectMan_Closed);
            SelectButton.Seal();
            ObjectMan.Instance.Open();
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

        protected override Dictionary<String, Object> OnValidate()
        {
            WorldObject currentObj = GetCurrentObject();

            if (currentObj == null)
                return base.OnValidate();
            
            return new Dictionary<String, Object>
            {
                { "Object", currentObj }
            };
        }
    }
}
