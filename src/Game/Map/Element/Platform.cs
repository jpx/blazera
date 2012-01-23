using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class Platform : Wall
    {
        public Platform()
            : base()
        {
        }

        public Platform(Platform copy)
            : base(copy)
        {
        }

        protected override void BuildBoundingBoxes()
        {
            for (int z = 0; z < H; ++z)
            {
                AddBoundingBox(new BBoundingBox(
                    this,
                    0,
                    0,
                    Width * GameData.TILE_SIZE,
                    Height * GameData.TILE_SIZE,
                    z));
            }

            // summit BBs
            // left
            AddBoundingBox(new BBoundingBox(
                this,
                -SUMMIT_BB_HALF_WIDTH,
                0,
                SUMMIT_BB_HALF_WIDTH,
                Height * GameData.TILE_SIZE,
                H));

            // top
            AddBoundingBox(new BBoundingBox(
                this,
                0,
                -SUMMIT_BB_HALF_WIDTH,
                Width * GameData.TILE_SIZE,
                SUMMIT_BB_HALF_WIDTH,
                H));

            // right
            AddBoundingBox(new BBoundingBox(
                this,
                Width * GameData.TILE_SIZE - SUMMIT_BB_HALF_WIDTH,
                0,
                Width * GameData.TILE_SIZE + SUMMIT_BB_HALF_WIDTH,
                Height * GameData.TILE_SIZE,
                H));

            // bottom
            AddBoundingBox(new BBoundingBox(
                this,
                0,
                Height * GameData.TILE_SIZE - SUMMIT_BB_HALF_WIDTH,
                Width * GameData.TILE_SIZE,
                Height * GameData.TILE_SIZE + SUMMIT_BB_HALF_WIDTH,
                H));
        }

        public override object Clone()
        {
            return new Platform(this);
        }
    }
}
