using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public abstract class BoundingBox
    {
        #region Constants

        protected const int DEFAULT_BASE_Z = 0;

        #endregion Constants

        public Boolean IsActive { get; private set; }

        public BoundingBox(WorldObject holder, int left, int top, int right, int bottom, int z)
        {
            IsActive = true;

            BaseLeft = left;
            BaseTop = top;
            BaseRight = right;
            BaseBottom = bottom;

            BaseZ = z;

            Holder = holder;
        }

        public BoundingBox(BoundingBox copy, WorldObject holder)
        {
            IsActive = true;

            BaseLeft = copy.BaseLeft;
            BaseTop = copy.BaseTop;
            BaseRight = copy.BaseRight;
            BaseBottom = copy.BaseBottom;

            BaseZ = copy.BaseZ;

            Holder = holder;
        }

        public void Activate(bool isActive = true)
        {
            IsActive = isActive;
        }

        public Boolean BoundingBoxTest(BoundingBox testBB)
        {
            if (!IsActive || !testBB.IsActive)
                return false;

            if (Z != testBB.Z)
                return false;

            if (Holder == testBB.Holder)
                return false;

            if (this == testBB)
                return false;

            return !(Left >= testBB.Right ||
                     Right <= testBB.Left ||
                     Top >= testBB.Bottom ||
                     Bottom <= testBB.Top);
        }

        public Boolean BoundingBoxTest(BoundingBox testBB, Vector2f offset)
        {
            if (!IsActive || !testBB.IsActive)
                return false;

            if (Z != testBB.Z)
                return false;

            if (Holder == testBB.Holder)
                return false;

            if (this == testBB)
                return false;

            return !(Left + offset.X >= testBB.Right ||
                     Right + offset.X <= testBB.Left ||
                     Top + offset.Y >= testBB.Bottom ||
                     Bottom + offset.Y <= testBB.Top);
        }

        public void Update()
        {
            Left = BaseLeft + Holder.Left;
            Right = BaseRight + Holder.Left;
            Top = BaseTop + Holder.Top;
            Bottom = BaseBottom + Holder.Top;

            Z = BaseZ + Holder.Z;
        }

        public IntRect GetBaseRect()
        {
            return new IntRect(
                BaseLeft,
                BaseTop,
                BaseRight,
                BaseBottom);
        }

        public IntRect GetNextTRect(Vector2f offset)
        {
            FloatRect nextTRect = new FloatRect(
                Left + offset.X,
                Top + offset.Y,
                Right + offset.X,
                Bottom + offset.Y);

            return new IntRect(
                nextTRect.Left < 0F ? (Int32)(nextTRect.Left / GameData.TILE_SIZE) - 1 : (Int32)(nextTRect.Left / GameData.TILE_SIZE),
                nextTRect.Top < 0F ? (Int32)(nextTRect.Top / GameData.TILE_SIZE) - 1 : (Int32)(nextTRect.Top / GameData.TILE_SIZE),
                nextTRect.Right < 0F ? (Int32)(nextTRect.Right / GameData.TILE_SIZE) - 1 : (Int32)(nextTRect.Right / GameData.TILE_SIZE),
                nextTRect.Bottom < 0F ? (Int32)(nextTRect.Bottom / GameData.TILE_SIZE) - 1 : (Int32)(nextTRect.Bottom / GameData.TILE_SIZE));
        }

        WorldObject _holder;
        public WorldObject Holder
        {
            get { return _holder; }
            set
            {
                _holder = value;

                if (Holder != null)
                    Update();
            }
        }

        public int BaseLeft
        {
            get;
            private set;
        }

        public int BaseRight
        {
            get;
            private set;
        }

        public int BaseTop
        {
            get;
            private set;
        }

        public int BaseBottom
        {
            get;
            private set;
        }

        public int BaseZ
        {
            get;
            private set;
        }

        public Int32 Z { get; private set; }

        public float Left
        {
            get;
            set;
        }

        public int TLeft
        {
            get
            {
                return (int)Left / GameData.TILE_SIZE;
            }
            private set
            {
                Left = value * GameData.TILE_SIZE;
            }
        }

        public float Top
        {
            get;
            set;
        }

        public int TTop
        {
            get
            {
                return (int)Top / GameData.TILE_SIZE;
            }
            private set
            {
                Top = value * GameData.TILE_SIZE;
            }
        }

        public float Right
        {
            get;
            set;
        }

        public int TRight
        {
            get
            {
                return (int)Right / GameData.TILE_SIZE;
            }
            private set
            {
                Right = value * GameData.TILE_SIZE;
            }
        }

        public float Bottom
        {
            get;
            set;
        }

        public int TBottom
        {
            get
            {
                return (int)Bottom / GameData.TILE_SIZE;
            }
            private set
            {
                Bottom = value * GameData.TILE_SIZE;
            }
        }
    }
}
