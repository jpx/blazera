using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public static class Create
    {
        public static Player Player(String type)
        {
            Player player = new Player(ScriptManager.Get<Player>(Create.GetFullType(typeof(Player), type)));
            player.SetType(type, false);
            return player;
        }

        public static NPC NPC(String type)
        {
            NPC NPC = new NPC(ScriptManager.Get<NPC>(Create.GetFullType(typeof(NPC), type)));
            NPC.SetType(type, false);
            return NPC;
        }

        public static Map Map(String type)
        {
            Map map = ScriptManager.Get<Map>(Create.GetFullType(typeof(Map), type));
            map.SetType(type, false);
            return map;
        }

        public static Ground Ground(String type)
        {
            Ground ground = ScriptManager.Get<Ground>(Create.GetFullType(typeof(Ground), type));
            ground.SetType(type, false);
            return ground;
        }

        public static Texture Texture(String type)
        {
            Texture texture = new Texture(ScriptManager.Get<Texture>(Create.GetFullType(typeof(Texture), type)));
            texture.SetType(type, false);
            return texture;
        }

        public static Tile Tile(String type)
        {
            Tile tile = new Tile(ScriptManager.Get<Tile>(Create.GetFullType(typeof(Tile), type)));
            tile.SetType(type, false);
            return tile;
        }

        public static TileSet TileSet(String type)
        {
            TileSet tileSet = new TileSet(ScriptManager.Get<TileSet>(Create.GetFullType(typeof(TileSet), type)));
            tileSet.SetType(type, false);
            return tileSet;
        }

        public static Animation Animation(String type)
        {
            Animation animation = new Animation(ScriptManager.Get<Animation>(Create.GetFullType(typeof(Animation), type)));
            animation.SetType(type, false);
            return animation;
        }

        public static QuestItem QuestItem(String type)
        {
            QuestItem questItem = new QuestItem(ScriptManager.Get<QuestItem>(Create.GetFullType(typeof(QuestItem), type)));
            questItem.SetType(type, false);
            return questItem;
        }

        public static WorldItem WorldItem(String type)
        {
            WorldItem worldItem = new WorldItem(ScriptManager.Get<WorldItem>(Create.GetFullType(typeof(WorldItem), type)));
            worldItem.SetType(type, false);
            return worldItem;
        }

        public static GroundElement GroundElement(String type)
        {
            GroundElement groundElement = new GroundElement(ScriptManager.Get<GroundElement>(Create.GetFullType(typeof(GroundElement), type)));
            groundElement.SetType(type, false);
            return groundElement;
        }

        public static Element Element(String type)
        {
            Element element = new Element(ScriptManager.Get<Element>(Create.GetFullType(typeof(Element), type)));
            element.SetType(type, false);
            return element;
        }

        public static Wall Wall(String type)
        {
            Wall wall = new Wall(ScriptManager.Get<Wall>(Create.GetFullType(typeof(Wall), type)));
            wall.SetType(type, false);
            return wall;
        }

        public static DisplaceableElement DisplaceableElement(String type)
        {
            DisplaceableElement displaceableElement = new DisplaceableElement(ScriptManager.Get<DisplaceableElement>(Create.GetFullType(typeof(DisplaceableElement), type)));
            displaceableElement.SetType(type, false);
            return displaceableElement;
        }

        public static Spell Spell(String type)
        {
            Spell spell = new Spell(ScriptManager.Get<Spell>(Create.GetFullType(typeof(Spell), type)));
            spell.SetType(type, false);
            return spell;
        }

        private static String GetFullType(Type T, String type)
        {
            return /*T.Name + "/" + */T.Name + "_" + type;
        }

        public static BaseObject CreateFromLongType(string longType)
        {
            string baseType = longType.Split('_')[0];
            string type = longType.Split('_')[1];

            BaseObject obj = null;

            switch (baseType)
            {
                case "Player": obj = Create.Player(type); break;
                case "NPC": obj = Create.NPC(type); break;
                case "Map": obj = Create.Map(type); break;
                case "Ground": obj = Create.Ground(type); break;
                case "Texture": obj = Create.Texture(type); break;
                case "Tile": obj = Create.Player(type); break;
                case "TileSet": obj = Create.TileSet(type); break;
                case "Animation": obj = Create.Animation(type); break;
                case "QuestItem": obj = Create.QuestItem(type); break;
                case "WorldItem": obj = Create.WorldItem(type); break;
                case "GroundElement": obj = Create.GroundElement(type); break;
                case "Element": obj = Create.Element(type); break;
                case "Wall": obj = Create.Wall(type); break;
                case "DisplaceableElement": obj = Create.DisplaceableElement(type); break;
            }

            return obj;
        }

        public static BaseObject CreateFromFullType(string fullType)
        {
            return CreateFromLongType(fullType.Split('/')[1]);
        }
    }
}
