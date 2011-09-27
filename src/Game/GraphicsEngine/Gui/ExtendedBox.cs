using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlazeraLib
{
    public abstract class ExtendedBox : Widget
    {
        protected enum EType
        {
            Vertical,
            Horizontal
        }

        protected const EType DEFAULT_TYPE = EType.Vertical;

        public const Int32 DEFAULT_SIZE = 10;

        public Int32 Size { get; protected set; }
        public Int32 CurrentPointer { get; protected set; }

        protected VAutoSizeBox VMainBox { get; set; }
        protected HAutoSizeBox HMainBox { get; set; }

        private List<Widget> ExtendedItems { get; set; }

        protected EType Type { get; set; }

        protected ExtendedBox(EType type = DEFAULT_TYPE, Boolean noBorderMode = true, Int32 size = DEFAULT_SIZE) :
            base()
        {
            this.Size = size;
            this.CurrentPointer = 0;

            this.ExtendedItems = new List<Widget>();

            this.Type = type;

            if (this.Type == EType.Vertical)
            {
                this.VMainBox = new VAutoSizeBox(noBorderMode);
                this.AddWidget(this.VMainBox);
            }
            else
            {
                this.HMainBox = new HAutoSizeBox(noBorderMode);
                this.AddWidget(this.HMainBox);
            }
        }

        public void AddItem(Widget widget)
        {
            this.ExtendedItems.Add(widget);

            if (this.GetCurrentSize() <= this.Size)
                this.RefreshBox();
        }

        public Boolean RemoveItem(Widget widget)
        {
            if (!this.ExtendedItems.Remove(widget))
                return false;

            this.CurrentPointer = 0;

            if (VMainBox != null)
                VMainBox.RemoveItem(widget);

            if (HMainBox != null)
                HMainBox.RemoveItem(widget);

            this.RefreshBox();

            return true;
        }

        public void Clear()
        {
            this.CurrentPointer = 0;

            this.ExtendedItems.Clear();
            if (this.HMainBox == null)
                this.VMainBox.Clear();
            else
                this.HMainBox.Clear();
        }

        public void SetCurrentPointer(Int32 pointer)
        {
            this.CurrentPointer = pointer;

            if (this.ExtendedItems.Count > 0 &&
                this.CurrentPointer >= this.ExtendedItems.Count - this.GetCurrentSize())
                this.CurrentPointer = this.ExtendedItems.Count - this.GetCurrentSize();

            if (this.CurrentPointer < 0)
                this.CurrentPointer = 0;

            this.RefreshBox();
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            if (ExtendedItems == null)
                return;

            foreach (Widget widget in ExtendedItems)
                widget.Open(openingInfo);
        }

        public override void Close(ClosingInfo closingInfo = null)
        {
            base.Close(closingInfo);

            if (ExtendedItems == null)
                return;

            foreach (Widget widget in ExtendedItems)
                widget.Close(closingInfo);
        }

        public override void Reset()
        {
            base.Reset();

            SetCurrentPointer(0);
        }

        public Widget GetAt(Int32 index)
        {
            if (index < 0 ||
                index > ExtendedItems.Count - 1)
                return null;

            return ExtendedItems[index];
        }

        private void RefreshBox()
        {
            if (this.HMainBox == null)
            {
                if (this.CurrentPointer + this.GetCurrentSize() < this.Size &&
                !(this.VMainBox.GetCount() < this.GetCurrentSize()))
                    return;

                this.VMainBox.Clear();

                List<Widget> newList = new List<Widget>();
                for (Int32 count = this.CurrentPointer; count < this.CurrentPointer + this.GetCurrentSize(); count++)
                    newList.Add(this.ExtendedItems[count]);

                this.VMainBox.AddItem(newList, 0, HAlignment.Left);
            }
            else
            {
                if (this.CurrentPointer + this.GetCurrentSize() < this.Size &&
                !(this.HMainBox.GetCount() < this.GetCurrentSize()))
                    return;

                this.HMainBox.Clear();

                List<Widget> newList = new List<Widget>();
                for (Int32 count = this.CurrentPointer; count < this.CurrentPointer + this.GetCurrentSize(); count++)
                    newList.Add(this.ExtendedItems[count]);

                this.HMainBox.AddItem(newList, 0, VAlignment.Top);
            }
        }

        public override Vector2 Dimension
        {
            get
            {
                if (this.VMainBox == null && this.HMainBox == null)
                    return base.Dimension;

                if (this.HMainBox == null)
                    return this.VMainBox.BackgroundDimension;

                return this.HMainBox.BackgroundDimension;
            }
        }

        private Int32 GetCurrentSize()
        {
            return Math.Min(this.ExtendedItems.Count, this.Size);
        }

        public Int32 GetTotalSize()
        {
            return this.ExtendedItems.Count;
        }

        public Boolean CurrentContains(Widget widget)
        {
            if (this.HMainBox == null)
                return this.VMainBox.Contains(widget);

            return this.HMainBox.Contains(widget);
        }
    }

    public class ExtendedVBox : ExtendedBox
    {
        public ExtendedVBox(Int32 size = DEFAULT_SIZE, Boolean noBorderMode = true) :
            base(EType.Vertical, noBorderMode, size)
        {

        }
    }

    public class ExtendedHBox : ExtendedBox
    {
        public ExtendedHBox(Int32 size = DEFAULT_SIZE, Boolean noBorderMode = true) :
            base(EType.Horizontal, noBorderMode, size)
        {

        }
    }

    public class MultiBox : Widget
    {
        protected const Int32 DEFAULT_SIZE = 4;
        protected const Alignment DEFAULT_ALIGNMENT = BlazeraLib.Alignment.Vertical;

        Alignment Alignment;
        Int32 Size;

        Int32 VSize;
        Int32 CurrentVPointer;

        Int32 HSize;
        Int32 CurrentHPointer;

        Box MainBox;

        VAutoSizeBox GetVMainBox() { return (VAutoSizeBox)MainBox; }
        HAutoSizeBox GetHMainBox() { return (HAutoSizeBox)MainBox; }

        List<Widget> ExtendedItems;

        Widget GetWidgetAt(Int32 row, Int32 column)
        {
            return ExtendedItems[row * Size + column];
        }

        public Int32 GetSize()
        {
            return Size;
        }

        public event PointerChangeEventHandler OnVPointerChange;
        public event PointerChangeEventHandler OnHPointerChange;

        public MultiBox(Alignment alignment = DEFAULT_ALIGNMENT, Int32 vSize = DEFAULT_SIZE, Int32 hSize = DEFAULT_SIZE) :
            base()
        {
            Alignment = alignment;
            Size = 0;

            VSize = vSize;
            CurrentVPointer = 0;

            HSize = hSize;
            CurrentHPointer = 0;

            ExtendedItems = new List<Widget>();

            if (Alignment == BlazeraLib.Alignment.Vertical)
                MainBox = new VAutoSizeBox(true, null, 0F);
            else
                MainBox = new HAutoSizeBox(true, null, 0F);

            AddWidget(MainBox);
        }

        public void SetSize(Int32 size)
        {
            Size = size;

            Clear();
        }

        void AddItem(Widget widget, Boolean refresh = true)
        {
            Int32 rowCount = GetRowCount();

            ExtendedItems.Add(widget);

            if (GetRowCount() != rowCount)
            {
                SetCurrentVPointer(CurrentVPointer + 1);
               
                UpdatePointers();
                RefreshBox();
            }

            SetCurrentHPointer(GetCurrentColumnOnCurrentRow());

            if (!refresh)
                return;

            UpdatePointers();
            RefreshBox();
        }

        public void AddItem(Widget widget)
        {
            AddItem(widget, true);
        }

        public void AddItem(List<Widget> widgets)
        {
            foreach (Widget widget in widgets)
                AddItem(widget, false);

            RefreshBox();
        }

        public Boolean RemoveItem(Widget widget)
        {
            if (!ExtendedItems.Remove(widget))
                return false;

            UpdatePointers();

            RefreshBox();

            return true;
        }

        void CallOnVPointerChange()
        {
            if (OnVPointerChange != null)
                OnVPointerChange(this, new PointerChangeEventArgs(CurrentVPointer));
        }

        void CallOnHPointerChange()
        {
            if (OnHPointerChange != null)
                OnHPointerChange(this, new PointerChangeEventArgs(CurrentHPointer));
        }

        void UpdatePointers()
        {
            UpdateVPointer();
            UpdateHPointer();
        }

        Boolean UpdateVPointer()
        {
            Boolean vPointerChange = false;

            if (ExtendedItems.Count > 0 &&
                CurrentVPointer > GetRowCount() - GetCurrentVSize())
            {
                CurrentVPointer = GetRowCount() - GetCurrentVSize();

                CallOnVPointerChange();
                vPointerChange = true;
            }

            if (CurrentVPointer < 0)
            {
                CurrentVPointer = 0;

                CallOnVPointerChange();
                vPointerChange = true;
            }

            return vPointerChange;
        }

        Boolean UpdateHPointer()
        {
            if (CurrentHPointer < 0)
            {
                CurrentHPointer = 0;

                CallOnHPointerChange();
                return true;
            }

            return false;
        }

        public void SetCurrentPointers(Int32 vPointer, Int32 hPointer)
        {
            SetCurrentVPointer(vPointer);
            SetCurrentHPointer(hPointer);
        }

        public void SetCurrentVPointer(Int32 pointer)
        {
            if (CurrentVPointer == pointer)
                return;

            CurrentVPointer = pointer;

            if (!UpdateVPointer())
                CallOnVPointerChange();

            RefreshBox();
        }

        public void SetCurrentHPointer(Int32 pointer)
        {
            if (CurrentHPointer == pointer)
                return;

            CurrentHPointer = pointer;

            if (!UpdateHPointer())
                CallOnHPointerChange();

            RefreshBox();
        }

        private void RefreshBox()
        {
            if (Alignment == BlazeraLib.Alignment.Vertical)
                GetVMainBox().Clear();
            else
                GetHMainBox().Clear();

            for (Int32 vCount = CurrentVPointer; vCount < CurrentVPointer + GetCurrentVSize(); vCount++)
            {
                List<Widget> newList = GetItems(vCount, CurrentHPointer, GetCurrentHSize(vCount));

                if (Alignment == BlazeraLib.Alignment.Vertical)
                {
                    HAutoSizeBox HB = new HAutoSizeBox(true, null, 0F);
                    HB.AddItem(newList, 0, VAlignment.Top);
                    GetVMainBox().AddItem(HB, 0, HAlignment.Left);
                }
                else
                {
                    VAutoSizeBox VB = new VAutoSizeBox(true, null, 0F);
                    VB.AddItem(newList, 0, HAlignment.Center);
                    GetHMainBox().AddItem(VB, 0, VAlignment.Center);
                }
            }
        }

        List<Widget> GetItems(Int32 row, Int32 startIndex, Int32 count)
        {
            List<Widget> items = new List<Widget>();

            Int32 hSize = GetHSize(row);

            for (Int32 hCount = startIndex; hCount < startIndex + count; hCount++)
            {
                if (hCount >= hSize)
                    break;

                items.Add(GetWidgetAt(row, hCount));
            }

            return items;
        }

        public override Vector2 Dimension
        {
            get
            {
                if (MainBox == null)
                    return base.Dimension;

                return MainBox.BackgroundDimension;
            }
        }

        public void Clear()
        {
            SetCurrentPointers(0, 0);

            ExtendedItems.Clear();

            if (Alignment == BlazeraLib.Alignment.Vertical)
                GetVMainBox().Clear();
            else
                GetHMainBox().Clear();
        }

        public override void Reset()
        {
            base.Reset();

            SetCurrentPointers(0, 0);
        }

        Int32 GetCurrentVSize()
        {
            return Math.Min(GetRowCount(), VSize);
        }

        Int32 GetHSize(Int32 row)
        {
            if (row > GetCurrentRow())
                return 0;

            if (row < GetCurrentRow())
                return Size;

            return GetItemCount() % Size;
        }

        Int32 GetCurrentHSize(Int32 row)
        {
            return Math.Min(GetHSize(row), HSize);
        }

        public Int32 GetItemCount()
        {
            return ExtendedItems.Count;
        }

        Int32 GetCurrentRow()
        {
            return GetItemCount() / Size;
        }

        public Int32 GetRowCount()
        {
            return GetCurrentRow() + (GetItemCount() % Size == 0 ? 0 : 1);
        }

        Int32 GetCurrentColumnOnCurrentRow()
        {
            Int32 currentColumnOnCurrentRow = GetItemCount() % Size;

            if (currentColumnOnCurrentRow == 0)
                return Size - 1;

            return currentColumnOnCurrentRow - 1;
        }
    }

    public delegate void PointerChangeEventHandler(Widget sender, PointerChangeEventArgs e);

    public class PointerChangeEventArgs : EventArgs
    {
        public Int32 Value;

        public PointerChangeEventArgs(Int32 value)
        {
            Value = value;
        }
    }

    #region OldMultiBox

    /*public class MultiBox : Widget
    {
        protected const Int32 DEFAULT_SIZE = 10;

        private Int32 VSize { get; set; }
        private Int32 CurrentVPointer { get; set; }

        private Int32 HSize { get; set; }
        private Int32 CurrentMaxHSize { get; set; }
        private Int32 CurrentMinHSize { get; set; }
        private Int32 CurrentHPointer { get; set; }

        protected VAutoSizeBox MainBox { get; set; }

        private List<List<Widget>> ExtendedItems { get; set; }

        public MultiBox(Int32 vSize = DEFAULT_SIZE, Int32 hSize = DEFAULT_SIZE) :
            base()
        {
            this.VSize = vSize;
            this.CurrentVPointer = 0;

            this.HSize = hSize;
            this.CurrentMaxHSize = 0;
            this.CurrentMinHSize = 0;
            this.CurrentHPointer = 0;

            this.ExtendedItems = new List<List<Widget>>();

            this.MainBox = new VAutoSizeBox(true, null, 0F);
            this.AddWidget(this.MainBox);
        }

        public void AddItem(Int32 row, Widget widget)
        {
            while (ExtendedItems.Count - 1 < row)
                ExtendedItems.Add(new List<Widget>());

            ExtendedItems[row].Add(widget);

            UpdatePointers();

            UpdateCurrentMinHSize();
            UpdateCurrentMaxHSize();

            if (ExtendedItems[row].Count <= HSize)
                RefreshBox();
        }

        public void AddItem(Dictionary<Int32, List<Widget>> widgets)
        {
            while (ExtendedItems.Count - 1 < widgets.Keys.Max())
                ExtendedItems.Add(new List<Widget>());

            foreach (Int32 row in widgets.Keys)
                foreach (Widget widget in widgets[row])
                    ExtendedItems[row].Add(widget);

            UpdatePointers();

            UpdateCurrentMinHSize();
            UpdateCurrentMaxHSize();

            RefreshBox();
        }

        public Boolean RemoveItem(Int32 row, Widget widget)
        {
            if (!ExtendedItems[row].Remove(widget))
                return false;

            if (ExtendedItems[row].Count == 0)
            {
                SetCurrentVPointer(CurrentVPointer - 1);
                ExtendedItems.Remove(ExtendedItems[row]);
            }

            UpdatePointers();

            UpdateCurrentMinHSize();
            UpdateCurrentMaxHSize();

            RefreshBox();

            return true;
        }

        public Boolean RemoveItem(Widget widget)
        {
            for (Int32 count = 0; count < ExtendedItems.Count; ++count)
            {
                if (!ExtendedItems[count].Contains(widget))
                    continue;

                return RemoveItem(count, widget);
            }

            return false;
        }

        void UpdatePointers()
        {
            UpdateVPointer();
            UpdateHPointer();
        }

        void UpdateVPointer()
        {
            if (this.ExtendedItems.Count > 0 &&
                this.CurrentVPointer >= this.ExtendedItems.Count - this.GetCurrentVSize())
                this.CurrentVPointer = this.ExtendedItems.Count - this.GetCurrentVSize();

            if (this.CurrentVPointer < 0)
                this.CurrentVPointer = 0;

            UpdateCurrentMinHSize();
            UpdateCurrentMaxHSize();
        }

        void UpdateHPointer()
        {
            if (this.CurrentHPointer < 0)
                this.CurrentHPointer = 0;
        }

        public void SetCurrentPointers(Int32 vPointer, Int32 hPointer)
        {
            this.SetCurrentVPointer(vPointer);
            this.SetCurrentHPointer(hPointer);
        }

        public void SetCurrentVPointer(Int32 pointer)
        {
            if (CurrentVPointer == pointer)
                return;

            this.CurrentVPointer = pointer;

            UpdateVPointer();

            this.RefreshBox();
        }

        public void SetCurrentHPointer(Int32 pointer)
        {
            if (CurrentHPointer == pointer)
                return;

            this.CurrentHPointer = pointer;

            UpdateHPointer();

            this.RefreshBox();
        }

        private void RefreshBox()
        {
            this.MainBox.Clear();

            Int32 vOffset = 0;
            for (Int32 vCount = this.CurrentVPointer; vCount < this.CurrentVPointer + this.GetCurrentVSize() + vOffset; vCount++)
            {
                List<Widget> newList = GetItems(vCount, CurrentHPointer, GetCurrentHSize(vCount));

                while (newList.Count == 0
                    && ++vCount < ExtendedItems.Count)
                {
                    newList = GetItems(vCount, CurrentHPointer, GetCurrentHSize(vCount));
                    ++vOffset;
                }

                HAutoSizeBox HB = new HAutoSizeBox(true, null, 0F);
                HB.AddItem(newList, 0, VAlignment.Top);
                MainBox.AddItem(HB, 0, HAlignment.Left);
            }

            Init();
        }

        List<Widget> GetItems(Int32 row, Int32 startIndex, Int32 count)
        {
            List<Widget> items = new List<Widget>();

            for (Int32 hCount = startIndex; hCount < startIndex + count; hCount++)
            {
                if (hCount >= ExtendedItems[row].Count)
                    break;

                items.Add(ExtendedItems[row][hCount]);
            }

            return items;
        }

        public override Vector2 Dimension
        {
            get
            {
                if (MainBox == null)
                    return base.Dimension;

                return MainBox.BackgroundDimension;
            }
        }

        public void Clear()
        {
            SetCurrentPointers(0, 0);

            ExtendedItems.Clear();

            MainBox.Clear();
        }

        public override void Reset()
        {
            base.Reset();

            SetCurrentPointers(0, 0);
        }

        Int32 GetCurrentVSize()
        {
            return Math.Min(ExtendedItems.Count, VSize);
        }

        Int32 GetCurrentHSize(Int32 row)
        {
            return Math.Min(ExtendedItems[row].Count, HSize);
        }

        void UpdateCurrentMinHSize()
        {
            if (ExtendedItems.Count == 0)
                return;

            this.CurrentMinHSize = ExtendedItems[CurrentVPointer].Count;

            for (Int32 vCount = CurrentVPointer + 1; vCount < CurrentVPointer + GetCurrentVSize(); ++vCount)
                if (ExtendedItems[vCount].Count < CurrentMinHSize)
                    CurrentMinHSize = ExtendedItems[vCount].Count;
        }

        void UpdateCurrentMaxHSize()
        {
            if (ExtendedItems.Count == 0)
                return;

            this.CurrentMaxHSize = ExtendedItems[CurrentVPointer].Count;

            for (Int32 vCount = CurrentVPointer + 1; vCount < CurrentVPointer + GetCurrentVSize(); ++vCount)
                if (ExtendedItems[vCount].Count > CurrentMaxHSize)
                    CurrentMaxHSize = ExtendedItems[vCount].Count;
        }

        public Int32 GetItemCount(Int32 row)
        {
            return ExtendedItems[row].Count;
        }

        public Int32 GetItemCount()
        {
            Int32 itemCount = 0;

            for (Int32 count = 0; count < ExtendedItems.Count; ++count)
                itemCount += GetItemCount(count);

            return itemCount;
        }
    }*/

    #endregion
}