using System;
using System.Collections.Generic;

using SFML.Window;
using SFML.Graphics;

namespace BlazeraLib
{
    public class DirectionalLightEffect : LightEffect
    {
        #region Members

        public float Direction { get; set; }
        public float Angle { get; set; }

        #endregion Members

        public DirectionalLightEffect()
            : base()
        {

        }

        public DirectionalLightEffect(DirectionalLightEffect copy)
            : base(copy)
        {
            Direction = copy.Direction;
            Angle = copy.Angle;
        }

        public override object Clone()
        {
            return new DirectionalLightEffect(this);
        }

        public override List<Shape> Generate(List<OpacityWall> walls)
        {
            if (!IsActive)
                return null;

            Triangles.Clear();

            float direction = (float)(Direction * Math.PI / 180D);
            float angle = (float)(Angle * Math.PI / 180D);

            AddTriangle(
                new Vector2f((float)(Radius * Math.Cos(Direction + Angle / 2F)), (float)(Radius * Math.Sin(Direction + Angle / 2F))),
                new Vector2f((float)(Radius * Math.Cos(Direction - Angle / 2F)), (float)(Radius * Math.Sin(Direction - Angle / 2F))),
                walls);

            return Triangles;
        }
    }
}
