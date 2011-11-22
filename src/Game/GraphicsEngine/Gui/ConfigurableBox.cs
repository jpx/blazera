using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class ConfigurableBox : Widget
    {
        private Dictionary<String, Box> Configurations { get; set; }
        private String CurrentConfiguration { get; set; }
        String DefaultConfiguration;

        public ConfigurableBox() :
            base()
        {
            this.Configurations = new Dictionary<String, Box>();
        }

        public void AddConfiguration(String configurationName, Box box)
        {
            if (configurationName == null || box == null)
                return;

            if (!this.Configurations.ContainsKey(configurationName))
                this.Configurations.Add(configurationName, box);

            if (Configurations.Count == 1)
            {
                DefaultConfiguration = configurationName;
                SetCurrentConfiguration(configurationName);
            }
        }

        public void SetCurrentConfiguration(String configurationName)
        {
            if (configurationName == null || !this.Configurations.ContainsKey(configurationName))
                return;

            if (CurrentConfiguration != null &&
                !RemoveWidget(GetCurrentConfiguration()))
                return;

            this.CurrentConfiguration = configurationName;

            AddWidget(GetCurrentConfiguration());

            this.EnableCurrentConfiguration();
        }

        private Box GetCurrentConfiguration()
        {
            return this.Configurations[this.CurrentConfiguration];
        }

        public Box GetConfiguration(string configurationName)
        {
            return Configurations[configurationName];
        }

        private void EnableCurrentConfiguration()
        {
            this.GetCurrentConfiguration().Open();
        }

        public override void Refresh()
        {
            base.Refresh();

            GetCurrentConfiguration().Position = Position;
        }

        public override Vector2f Dimension
        {
            get
            {
                if (this.CurrentConfiguration != null)
                    return this.GetCurrentConfiguration().BackgroundDimension;

                return base.Dimension;
            }
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            if (Configurations == null)
                return;

            this.EnableCurrentConfiguration();
        }

        public override Color Color
        {
            set
            {
                base.Color = value;

                foreach (Box box in Configurations.Values)
                    box.Color = Color;
            }
        }

        public override Color BackgroundColor
        {
            set
            {
                base.BackgroundColor = value;

                foreach (Box box in Configurations.Values)
                    box.BackgroundColor = BackgroundColor;
            }
        }

        public override void Reset()
        {
            base.Reset();

            if (DefaultConfiguration != null)
                SetCurrentConfiguration(DefaultConfiguration);
        }
    }
}
