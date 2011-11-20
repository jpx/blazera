using System;
using BlazeraLib;
using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;
using SFML.Graphics;
using SFML.Window;
namespace Box2DXDemo
{
    class Program
    {
        static bool IsRunning = true;
        static RenderWindow Window
        {
            get;
            set;
        }

        static void Main(string[] args)
        {
            ScriptEngine.Instance.Init("GameDatas");
            ScriptEngine.Instance.Load_Assembly("Blazera");

            ImageManager.Instance.Init();
            SoundManager.Instance.Init();

            GameTime.Instance.Init();

            Window = new RenderWindow(new VideoMode(GameDatas.WINDOW_WIDTH, GameDatas.WINDOW_HEIGHT), "Blazera", GameDatas.WINDOW_STYLE);

            Window.SetFramerateLimit(60);

            WindowEvents.Instance.Init(Window);

            Player pl = Create.Player("Vlad");
            Player pl2 = Create.Player("Vlad");
            Map map = Create.Map("combat_test");

            pl.SetMap(map, 200F, 200F);
            pl2.SetMap(map, 100F, 100F);

            #region Box2DX
            // Define the size of the world. Simulation will still work
            // if bodies reach the end of the world, but it will be slower.
            AABB worldAABB = new AABB();
            worldAABB.LowerBound.Set(-1000.0f);
            worldAABB.UpperBound.Set(1000.0f);

            // Define the gravity vector.
            Vec2 gravity = new Vec2(0.0f, 0.0f);

            // Do we want to let bodies sleep?
            bool doSleep = true;

            // Construct a world object, which will hold and simulate the rigid bodies.
            Box2DX.Dynamics.World world = new Box2DX.Dynamics.World(worldAABB, gravity, doSleep);

            // Define the ground body.
            BodyDef groundBodyDef = new BodyDef();
            groundBodyDef.Position.Set(0.0f, -10.0f);

            // Call the body factory which  creates the ground box shape.
            // The body is also added to the world.
            Body groundBody = world.CreateBody(groundBodyDef);

            // Define the ground box shape.
            PolygonDef groundShapeDef = new PolygonDef();

            // The extents are the half-widths of the box.
            groundShapeDef.SetAsBox(50.0f, 10.0f);

            // Add the ground shape to the ground body.
            groundBody.CreateFixture(groundShapeDef);

            // PLAYER 1
            // Define the dynamic body. We set its position and call the body factory.
            BodyDef bodyDef = new BodyDef();
            bodyDef.Position.Set(0.0f, 4.0f);
            Body body = world.CreateBody(bodyDef);

            // Define another box shape for our dynamic body.
            PolygonDef shapeDef = new PolygonDef();
            shapeDef.SetAsBox(1.0f, 1.0f);

            // Set the box density to be non-zero, so it will be dynamic.
            shapeDef.Density = 1.0f;

            // Override the default friction.
            shapeDef.Friction = 0.3f;

            // Add the shape to the body.
            body.CreateFixture(shapeDef);

            // Now tell the dynamic body to compute it's mass properties base
            // on its shape.
            body.SetMassFromShapes();

            // PLAYER 2
            BodyDef bodyDef2 = new BodyDef();
            bodyDef2.Position.Set(2.0f, 40.0f);
            Body body2 = world.CreateBody(bodyDef2);

            // Define another box shape for our dynamic body.
            PolygonDef shapeDef2 = new PolygonDef();
            shapeDef2.SetAsBox(1.0f, 1.0f);

            // Set the box density to be non-zero, so it will be dynamic.
            shapeDef2.Density = 1.0f;

            // Override the default friction.
            shapeDef2.Friction = 0.3f;

            // Add the shape to the body.
            body2.CreateFixture(shapeDef2);

            // Now tell the dynamic body to compute it's mass properties base
            // on its shape.
            body2.SetMassFromShapes();

            body2.SetLinearDamping(2F);

            // Prepare for simulation. Typically we use a time step of 1/60 of a
            // second (60Hz) and 10 iterations. This provides a high quality simulation
            // in most game scenarios.
            float timeStep = 1.0f / 60.0f;
            int velocityIterations = 8;
            int positionIterations = 1;

            #endregion

            while (IsRunning)
            {
                Time Dt = GameTime.GetDt();
                Inputs.Instance.UpdateState();

                Window.DispatchEvents();
                Window.Clear();

                Time trueDt = new Time(Window.GetFrameTime());

                Log.Cl("Dt : " + 1D / trueDt.Value);


                // box2d test /////////////////////////////////////////////////////////
                world.Step(timeStep, velocityIterations, positionIterations);

                // Now print the position and angle of the body.
                Vec2 position = body.GetPosition();
                float angle = body.GetAngle();
                // Now print the position and angle of the body.
                Vec2 position2 = body2.GetPosition();
                float angle2 = body2.GetAngle();

                pl.Position = new Vector2(position.X, position.Y);
                pl2.Position = new Vector2(position2.X, position2.Y);
                ///////////////////////////////////////////////////////////////////////

                // map update comes here
                map.Update(GameTime.Dt);
                map.Draw(Window);

                const float pow = 5000F;
                if (Inputs.IsGameInput(InputType.Down))
                {
                    body.ApplyForce(new Vec2(0, pow), position);
                }
                if (Inputs.IsGameInput(InputType.Up))
                {
                    body.ApplyForce(new Vec2(0, -pow), position);
                }
                if (Inputs.IsGameInput(InputType.Left))
                {
                    body.ApplyForce(new Vec2(-pow, 0), position);
                }
                if (Inputs.IsGameInput(InputType.Right))
                {
                    body.ApplyForce(new Vec2(pow, 0), position);
                }
                if (Inputs.IsGameInput(InputType.Down2))
                {
                    body2.ApplyForce(new Vec2(0, pow), position);
                }
                if (Inputs.IsGameInput(InputType.Up2))
                {
                    body2.ApplyForce(new Vec2(0, -pow), position);
                }
                if (Inputs.IsGameInput(InputType.Left2))
                {
                    body2.ApplyForce(new Vec2(-pow, 0), position);
                }
                if (Inputs.IsGameInput(InputType.Right2))
                {
                    body2.ApplyForce(new Vec2(pow, 0), position);
                }

                while (WindowEvents.EventHappened())
                {
                    BlzEvent evt = new BlzEvent(WindowEvents.GetEvent());

                    if (evt.Type == EventType.KeyPressed || evt.Type == EventType.KeyReleased)
                        Inputs.Instance.UpdateState(evt);

                    if (Inputs.IsGameInput(InputType.Back, evt))
                        Window.Close();

                    if (evt.Type == EventType.KeyPressed)
                    {
                        if (Inputs.IsGameInput(InputType.Down, evt))
                            pl.EnableDirection(Direction.S);
                        if (Inputs.IsGameInput(InputType.Up, evt))
                            pl.EnableDirection(Direction.N);
                        if (Inputs.IsGameInput(InputType.Left, evt))
                            pl.EnableDirection(Direction.O);
                        if (Inputs.IsGameInput(InputType.Right, evt))
                            pl.EnableDirection(Direction.E);
                        if (Inputs.IsGameInput(InputType.Down2, evt))
                            pl2.EnableDirection(Direction.S);
                        if (Inputs.IsGameInput(InputType.Up2, evt))
                            pl2.EnableDirection(Direction.N);
                        if (Inputs.IsGameInput(InputType.Left2, evt))
                            pl2.EnableDirection(Direction.O);
                        if (Inputs.IsGameInput(InputType.Right2, evt))
                            pl2.EnableDirection(Direction.E);
                    }

                    if (evt.Type == EventType.KeyReleased)
                    {
                        if (Inputs.IsGameInput(InputType.Down, evt))
                            pl.DisableDirection(Direction.S);
                        if (Inputs.IsGameInput(InputType.Up, evt))
                            pl.DisableDirection(Direction.N);
                        if (Inputs.IsGameInput(InputType.Left, evt))
                            pl.DisableDirection(Direction.O);
                        if (Inputs.IsGameInput(InputType.Right, evt))
                            pl.DisableDirection(Direction.E);
                        if (Inputs.IsGameInput(InputType.Down2, evt))
                            pl2.DisableDirection(Direction.S);
                        if (Inputs.IsGameInput(InputType.Up2, evt))
                            pl2.DisableDirection(Direction.N);
                        if (Inputs.IsGameInput(InputType.Left2, evt))
                            pl2.DisableDirection(Direction.O);
                        if (Inputs.IsGameInput(InputType.Right2, evt))
                            pl2.DisableDirection(Direction.E);
                    }
                }

                Window.Display();

                IsRunning = Window.IsOpened();
            }
        }
    }
}
