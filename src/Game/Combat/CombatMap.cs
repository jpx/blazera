using System.Collections.Generic;
using SFML.Graphics;

namespace BlazeraLib
{
    /// <summary>
    /// Map that handles combat progress
    /// </summary>
    public class CombatMap : Map
    {
        #region Constants

        public const uint CELL_SIZE = 16;

        const double ALPHA_MODE_VALUE = 50D;

        #endregion

        #region Members

        public Combat Combat { get; private set; }

        GridShape Grid;

        BlazeraLib.Game.Pathfinding.Pathfinding Pathfinding;

        bool AlphaMode;

        #endregion

        /// <summary>
        /// Constructs a default CombatMap from a given Map
        /// </summary>
        /// <param name="map">Base map of combat</param>
        public CombatMap(Map map) :
            base()
        {
            Ground = new Ground(map.Ground);

            SetPhysicsIsRunning(false);

            foreach (WorldObject wObj in map.Objects)
            {
                if (wObj is Player)
                    continue;

                ((WorldObject)System.Activator.CreateInstance(wObj.GetType(), wObj)).SetMap(this, wObj.Position.X, wObj.Position.Y);
            }

            Grid = new GridShape(CELL_SIZE, (uint)Width * (uint)(GameDatas.TILE_SIZE / CELL_SIZE), (uint)Height * (uint)(GameDatas.TILE_SIZE / CELL_SIZE));
            AddObjectToDraw(DrawOrder.Grid, Grid);

            Combat = new Combat(this);
            Combat.OnCombatantJoining += new CombatCombatantEventHandler(Combat_OnCombatantJoiningCombat);
            Combat.OnCombatantLeaving += new CombatCombatantEventHandler(Combat_OnCombatantLeavingCombat);

            #region tests
            Team t1 = new Team();

            for (int k = 0; k < 1; ++k)
            {
                PlayerCombatant p1 = new PlayerCombatant(Create.Player("Vlad"));
                p1.Name = "Player 1";
                p1.Move(new Vector2I(10 + k, 15 + k));
                p1.Status[BaseCaracteristic.Hp, BaseStatistic.Attribute.Max] = 300;
                p1.Status[BaseCaracteristic.Sp, BaseStatistic.Attribute.Max] = 10;
                p1.Status[BaseCaracteristic.Mp, BaseStatistic.Attribute.Max] = 3;
                p1.Status[Caracteristic.Intelligence] = 2;

                for (int i = 0; i < 19; ++i)
                {
                    Spell sp = Create.Spell("LightningTest");
                    sp.Name = "LightningTest " + i;
                    p1.SpellPanoply.AddSpell(sp);
                }

                t1.AddCombatant(p1);
            }

            Combat.InfoPanel.AddBox(new CombatantInfoPanelBox());

            PlayerCombatant p2 = new PlayerCombatant(Create.Player("Vlad"));
            p2.Name = "Player 2";
            p2.Status[BaseCaracteristic.Hp, BaseStatistic.Attribute.Max] = 30000;
            p2.Status[BaseCaracteristic.Sp, BaseStatistic.Attribute.Max] = 100;
            p2.Status[BaseCaracteristic.Mp, BaseStatistic.Attribute.Max] = 60;
            for (int i = 0; i < 2; ++i)
            {
                Spell sp = Create.Spell("LightningTest");
                sp.Name = "LightningTest " + i;
                p2.SpellPanoply.AddSpell(sp);
            }
            p2.Move(new Vector2I(8, 10));
            t1.AddCombatant(p2);

            MapEffectManager.Instance.AddEffect(new MapTextEffect("2044", Color.Blue), new Vector2(200, 200));

            Combat.AddTeam(t1);
            #endregion

            Pathfinding = new Game.Pathfinding.Pathfinding();
            Pathfinding.InitNodeSet(this);

            AlphaMode = false;
        }

        public List<Vector2I> GetPath(Vector2I startCellPosition, Vector2I goalCellPosition)
        {
            return Pathfinding.FindPath(startCellPosition, goalCellPosition);
        }

        void Combat_OnCombatantLeavingCombat(Combat sender, CombatCombatantEventArgs e)
        {
            RemoveObjectToDraw(DrawOrder.Normal, e.Combatant);
        }

        void Combat_OnCombatantJoiningCombat(Combat sender, CombatCombatantEventArgs e)
        {
            e.Combatant.SetMap(this);
            AddObjectToDraw(DrawOrder.Normal, e.Combatant);
        }

        public override void Update(Time dt)
        {
            base.Update(dt);

            Combat.Update(dt);
        }

        public override Vector2 Dimension
        {
            get
            {
                return new Vector2(Width, Height) * CELL_SIZE;
            }
        }

        public override int Width
        {
            get { return base.Width * GameDatas.TILE_SIZE / (int)CELL_SIZE; }
        }

        public override int Height
        {
            get { return base.Height * GameDatas.TILE_SIZE / (int)CELL_SIZE; }
        }

        public override Vector2 GetPositionFromCell(Vector2I cell)
        {
            return (cell.ToVector2() + new Vector2(.5F, .5F)) * CELL_SIZE;
        }

        public void SwitchGridVisibility()
        {
            Grid.IsVisible = !Grid.IsVisible;
        }

        public void SwitchObjectAlphaMode()
        {
            AlphaMode = !AlphaMode;

            double alphaValue = AlphaMode ? ALPHA_MODE_VALUE : 100D;
            foreach (WorldObject wObj in Objects)
                wObj.Skin.SetAlpha(alphaValue);
        } 
    }
}
