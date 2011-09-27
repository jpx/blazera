using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;

namespace BlazeraEditor
{
    public class ObjectPencil : Pencil
    {
        #region Singleton

        private static ObjectPencil _instance;
        public static ObjectPencil Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new ObjectPencil();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        static readonly Texture ICON_TEXTURE = Create.Texture("Element_TestTree");

        WorldObject CurrentObject;

        public void SetCurrentObject(WorldObject wObj)
        {
            CurrentObject = wObj;
            SetCursorTexture(new Texture(wObj.Skin));
        }

        protected override void Empty()
        {
            base.Empty();

            CurrentObject = null;
        }

        private ObjectPencil() :
            base(ICON_TEXTURE)
        {
            Name = "ObjectPencil";
        }

        protected override Boolean CanPaint(Vector2 point)
        {
            if (CurrentObject == null)
                return false;

            return CanFit(point - CurrentObject.Halfsize);
        }

        protected override Boolean Paint(Vector2 point)
        {
            switch (Mode)
            {
                case EMode.Normal:
                    String baseType = CurrentObject.GetType().Name;

                    WorldObject currentObject = null;
                    switch (baseType)
                    {
                        case "Element": currentObject = new Element((Element)CurrentObject); break;
                        case "DisplaceableElement": currentObject = new DisplaceableElement((DisplaceableElement)CurrentObject); break;
                        case "GroundElement": currentObject = new GroundElement((GroundElement)CurrentObject); break;
                        case "WorldItem": currentObject = new WorldItem((WorldItem)CurrentObject); break;
                    }

                    Int32 x = (Int32)(point.X - currentObject.Halfsize.X);
                    Int32 y = (Int32)(point.Y - currentObject.Halfsize.Y);

                    currentObject.SetMap(MapMan.GetCurrent(), x, y);

                    return true;

                default: return false;
            }
        }

        Boolean CanFit(Vector2 position)
        {
            return MapBox.Map.Ground.CanFit(CurrentObject, position);
        }

        public void FillMap()
        {
            if (CurrentObject == null)
                return;

            Int32 x = 0;
            Int32 y = 0;
            Int32 offset = (Int32)(LockValue < 2 ? 1 : LockValue);

            while (y < MapBox.Map.Dimension.Y + CurrentObject.Dimension.Y)
            {
                while (x < MapBox.Map.Dimension.X + offset + CurrentObject.Dimension.X)
                {
                    if (CanPaint(new Vector2(x, y)))
                        Paint(new Vector2(x, y));

                    x += offset;
                }

                x = 0;
                y += offset;
            }
        }

        public void FillMapRandomly(UInt32 objectCount)
        {
            if (CurrentObject == null)
                return;

            UInt32 count = 0;
            UInt32 loopCount = 0;

            while (count < objectCount && loopCount < 100 * objectCount)
            {
                ++loopCount;

                Int32 x = RandomHelper.Get(0, (Int32)(MapBox.Map.Dimension.X + CurrentObject.Dimension.X));
                Int32 y = RandomHelper.Get(0, (Int32)(MapBox.Map.Dimension.Y + CurrentObject.Dimension.Y));

                if (!CanPaint(new Vector2(x, y)))
                    continue;

                Paint(new Vector2(x, y));
                ++count;
            }
        }
    }
}
