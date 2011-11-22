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
        public Boolean IsActive { get; set; }

        public BoundingBox(WorldObject holder, int left, int top, int right, int bottom)
        {
            this.IsActive = true;

            this.Holder = holder;

            this.BaseLeft = left;
            this.BaseTop = top;
            this.BaseRight = right;
            this.BaseBottom = bottom;

            if (this.Holder != null)
                this.Update();
        }

        public BoundingBox(BoundingBox copy, WorldObject holder)
        {
            this.IsActive = true;

            this.Holder = holder;

            this.BaseLeft = copy.BaseLeft;
            this.BaseTop = copy.BaseTop;
            this.BaseRight = copy.BaseRight;
            this.BaseBottom = copy.BaseBottom;

            if (this.Holder != null)
                this.Update();
        }

        public Boolean BoundingBoxTest(BoundingBox testBB)
        {
            if (!this.IsActive)
                return false;

            if (this.Z != testBB.Z)
                return false;

            return !(this.Left >= testBB.Right ||
                     this.Right <= testBB.Left ||
                     this.Top >= testBB.Bottom ||
                     this.Bottom <= testBB.Top);
        }

        public Boolean BoundingBoxTest(BoundingBox testBB, Vector2f offset)
        {
            if (!this.IsActive)
                return false;

            if (this.Z != testBB.Z)
                return false;

            return !(this.Left + offset.X >= testBB.Right ||
                     this.Right + offset.X <= testBB.Left ||
                     this.Top + offset.Y >= testBB.Bottom ||
                     this.Bottom + offset.Y <= testBB.Top);
        }

        public void Update()
        {
            this.Left = this.BaseLeft + this.Holder.Left;
            this.Right = this.BaseRight + this.Holder.Left;
            this.Top = this.BaseTop + this.Holder.Top;
            this.Bottom = this.BaseBottom + this.Holder.Top;

            this.Z = Holder.Z;
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
                nextTRect.Left < 0F ? (Int32)(nextTRect.Left / GameDatas.TILE_SIZE) - 1 : (Int32)(nextTRect.Left / GameDatas.TILE_SIZE),
                nextTRect.Top < 0F ? (Int32)(nextTRect.Top / GameDatas.TILE_SIZE) - 1 : (Int32)(nextTRect.Top / GameDatas.TILE_SIZE),
                nextTRect.Right < 0F ? (Int32)(nextTRect.Right / GameDatas.TILE_SIZE) - 1 : (Int32)(nextTRect.Right / GameDatas.TILE_SIZE),
                nextTRect.Bottom < 0F ? (Int32)(nextTRect.Bottom / GameDatas.TILE_SIZE) - 1 : (Int32)(nextTRect.Bottom / GameDatas.TILE_SIZE));
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
                return (int)this.Left / GameDatas.TILE_SIZE;
            }
            private set
            {
                this.Left = value * GameDatas.TILE_SIZE;
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
                return (int)this.Top / GameDatas.TILE_SIZE;
            }
            private set
            {
                this.Top = value * GameDatas.TILE_SIZE;
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
                return (int)this.Right / GameDatas.TILE_SIZE;
            }
            private set
            {
                this.Right = value * GameDatas.TILE_SIZE;
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
                return (int)this.Bottom / GameDatas.TILE_SIZE;
            }
            private set
            {
                this.Bottom = value * GameDatas.TILE_SIZE;
            }
        }
    }
}
