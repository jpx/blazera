using System.Collections.Generic;
using SFML.Graphics;

namespace BlazeraLib
{
    public abstract class InfoPanelBox : HAutoSizeBox
    {
        #region Classes

        #region BuildInfo

        public class BuildInfo
        {
            #region Members

            Dictionary<string, object> Args;

            #endregion

            public BuildInfo(Dictionary<string, object> args)
            {
                Args = args;
            }

            public T GetArg<T>(string key)
            {
                return (T)Args[key];
            }
        }

        #endregion

        #endregion

        #region Constants

        protected const float DEFAULT_MARGINS = 10F;

        const float DEFAULT_Y_Offset = 5F;
        const float DEFAULT_X_OFFSET = 10F;

        #endregion

        #region Members

        public string ConfigurationName { get; private set; }

        List<VAutoSizeBox> SubBoxes;

        #endregion

        public InfoPanelBox(string configurationName) :
            base(true, null, DEFAULT_X_OFFSET)
        {
            ConfigurationName = configurationName;

            SubBoxes = new List<VAutoSizeBox>();
        }

        public void AddItem(int column, Widget item)
        {
            while (SubBoxes.Count <= column)
            {
                SubBoxes.Add(new VAutoSizeBox(true, null, DEFAULT_Y_Offset));
                AddItem(SubBoxes[column]);
            }

            SubBoxes[column].AddItem(item);
        }

        public bool RemoveItem(int column, Widget item)
        {
            return SubBoxes[column].RemoveItem(item);
        }

        public abstract void UpdateData();

        public virtual void Build(BuildInfo buildInfo)
        {
            Clear();
        }

        public override void Clear()
        {
            base.Clear();

            SubBoxes.Clear();
        }
    }
}
