using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraEditor
{
    public delegate void SelectedObjectChangeEventHandler(SelectorTool sender, SelectedObjectChangeEventArgs e);
    public class SelectedObjectChangeEventArgs : EventArgs
    {
        public WorldObject Object { get; private set; }

        public SelectedObjectChangeEventArgs(WorldObject wObj)
        {
            Object = wObj;
        }
    }

    public class SelectorTool : Tool
    {
        #region Singleton

        private static SelectorTool _instance;
        public static SelectorTool Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new SelectorTool();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        static readonly BlazeraLib.Texture ICON_TEXTURE = Create.Texture("Gui_SelectorToolIcon");
        static readonly Color FILL_COLOR = new Color(123, 104, 238, 64);
        static readonly Color OUTLINE_COLOR = new Color(128, 255, 255, 128);

        const float SELECTION_PERCENT_MARGIN = 20F;
        public static readonly Color OBJECT_SELECTION_COLOR = new Color(0, 128, 255, 192);

        public event SelectedObjectChangeEventHandler OnObjectAddition;
        public event SelectedObjectChangeEventHandler OnObjectSuppression;

        RectangleSelector Selector;
        BlazeraLib.FloatRect CurrentSelectionRect = new BlazeraLib.FloatRect();

        List<WorldObject> SelectedObjects = new List<WorldObject>();

        private SelectorTool() :
            base(ICON_TEXTURE)
        {
            Name = "SelectorTool";

            Selector = new RectangleSelector(null, true, true, false);
            Selector.SetColor(FILL_COLOR, OUTLINE_COLOR);
            Selector.OnChange += new SelectionChangeEventHandler(Selector_OnChange);
            Selector.MoveIsEnabled = false;
            AddWidget(Selector);
        }

        void Selector_OnChange(Selector sender, SelectionChangeEventArgs e)
        {
            BlazeraLib.FloatRect currentSelectorRect = Selector.GetRect();

            Vector2f currentSelectionTopLeft = MapBox.GetMapLocalFromGlobal(MapBox.GetGlobalFromLocal(new Vector2f(currentSelectorRect.Left, currentSelectorRect.Top)));
            Vector2f currentSelectionBottomRight = MapBox.GetMapLocalFromGlobal(MapBox.GetGlobalFromLocal(new Vector2f(currentSelectorRect.Right, currentSelectorRect.Bottom)));
            CurrentSelectionRect = new BlazeraLib.FloatRect(
                currentSelectionTopLeft.X,
                currentSelectionTopLeft.Y,
                currentSelectionBottomRight.X,
                currentSelectionBottomRight.Y);

            RefreshSelection();
        }

        public override void SetMapBox(MapBox mapBox)
        {
            base.SetMapBox(mapBox);

            Selector.SetHolder(MapBox);
            Selector.SetHolderOffset(MapBox.GetMapOffset());
        }

        public override void Reset()
        {
            base.Reset();

            ClearSelection();
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            if (evt.IsHandled)
                return base.OnEvent(evt);

            switch (evt.Type)
            {
                case EventType.MouseButtonPressed:

                    if (evt.MouseButton.Button != Mouse.Button.Right)
                        break;

                    if (!Selector.HolderContains())
                        break;

                    ClearSelection();

                    return true;

                case EventType.KeyPressed:

                    switch (evt.Key.Code)
                    {
                        case Keyboard.Key.A:

                            if (!Keyboard.IsKeyPressed(Keyboard.Key.LControl))
                                break;

                            if (SelectedObjects.Count > 0)
                                ClearSelection();
                            else
                                AddAllObjectToSelection();

                            return true;

                        case Keyboard.Key.Delete:

                            if (SelectedObjects.Count == 0)
                                break;

                            RemoveSelectedObjectsFromMap();

                            return true;
                    }

                    break;
            }

            return base.OnEvent(evt);
        }

        void RefreshSelection()
        {
            RefreshObjectInSelection();
        }

        Boolean SelectionContainsObject(WorldObject wObj)
        {
            if (wObj == null)
                return false;

            return !(
                wObj.Left >= CurrentSelectionRect.Right - SELECTION_PERCENT_MARGIN * wObj.Dimension.X / 100F ||
                wObj.Top >= CurrentSelectionRect.Bottom - SELECTION_PERCENT_MARGIN * wObj.Dimension.Y / 100F ||
                wObj.Right < CurrentSelectionRect.Left + SELECTION_PERCENT_MARGIN * wObj.Dimension.X / 100F ||
                wObj.Bottom < CurrentSelectionRect.Top + SELECTION_PERCENT_MARGIN * wObj.Dimension.Y / 100F);
        }

        void AddObjectToSelection(WorldObject wObj)
        {
            if (wObj == PlayerHdl.Vlad)
                return;

            SelectedObjects.Add(wObj);
            wObj.Color = OBJECT_SELECTION_COLOR;

            if (OnObjectAddition != null)
                OnObjectAddition(this, new SelectedObjectChangeEventArgs(wObj));
        }

        public Boolean RemoveObjectFromSelection(WorldObject wObj)
        {
            if (!SelectedObjects.Remove(wObj))
                return false;

            wObj.Color = Color.White;

            if (OnObjectSuppression != null)
                OnObjectSuppression(this, new SelectedObjectChangeEventArgs(wObj));

            return true;
        }

        void RefreshObjectInSelection(WorldObject wObj)
        {
            if (wObj == null)
                return;

            Boolean objectIsSelected = SelectedObjects.Contains(wObj);
            Boolean selectionContainsObject = SelectionContainsObject(wObj);

            if (objectIsSelected)
            {
                if (!selectionContainsObject)
                    RemoveObjectFromSelection(wObj);

                return;
            }

            if (selectionContainsObject)
                AddObjectToSelection(wObj);
        }

        void ClearSelection()
        {
            Selector.Reset();

            Queue<WorldObject> objectsToRemove = new Queue<WorldObject>();

            foreach (WorldObject wObj in SelectedObjects)
                objectsToRemove.Enqueue(wObj);

            while (objectsToRemove.Count > 0)
                RemoveObjectFromSelection(objectsToRemove.Dequeue());
        }

        void RefreshObjectInSelection()
        {
            foreach (WorldObject wObj in MapBox.Map.GetObjects())
                RefreshObjectInSelection(wObj);
        }

        void AddAllObjectToSelection()
        {
            foreach (WorldObject wObj in MapBox.Map.GetObjects())
                AddObjectToSelection(wObj);
        }

        public Boolean RemoveSelectedObjectFromMap(WorldObject wObj)
        {
            if (!RemoveObjectFromSelection(wObj))
                return false;

            MapBox.Map.RemoveObject(wObj);

            return true;
        }

        public void RemoveSelectedObjectsFromMap()
        {
            Queue<WorldObject> objectsToRemove = new Queue<WorldObject>();

            foreach (WorldObject wObj in SelectedObjects)
                objectsToRemove.Enqueue(wObj);

            while (objectsToRemove.Count > 0)
            {
                WorldObject wObj = objectsToRemove.Dequeue();

                RemoveSelectedObjectFromMap(wObj);
            }
        }
    }
}