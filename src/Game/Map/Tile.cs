using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class Tile : DrawableBaseObject, IUpdateable
    {
        public Texture Texture { get; set; }
        
        private Vector2f _position;
        public Vector2f Position
        {
            get { return _position; }
            set
            {
                _position = value;

                Texture.Position = Position;
            }
        }
		
        public Tile() :
            base()
        {

        }

        public Tile(Tile copy) :
            base(copy)
        {
            Texture = new Texture(copy.Texture);
            Position = copy.Position;
        }

        public override object Clone()
        {
            return new Tile(this);
        }

        public override void ToScript()
        {
            Sw = new ScriptWriter(this);

            Sw.InitObject();

            base.ToScript();

            Sw.WriteProperty("Texture", "Create:Texture(\"" + Texture.Type + "\")");

            Sw.EndObject();
        }

        public virtual void Update(Time dt)
        {
        }

        public override void Draw(RenderTarget window)
        {
            Texture.Draw(window);
        }
    }
}