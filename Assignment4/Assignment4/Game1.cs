#region Using Statments
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Primitives;
#endregion
namespace Assignment4
{
    #region Structs
    struct CubeData
    {
        public Vector3 position;
        public float scale;
        public Color color;
        public Vector3 velocity;
    }

    struct LightData
    {
        public Vector3 position;
        public Color color;
        public Vector3 velocity;
    }
    #endregion
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        #region Fields
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Effect skyBoxEffect;
        Effect cubeEffect;
        BasicEffect lightEffect;
        TextureCube skyboxTexture;

        Cube cube = new Cube();
        CubeData[] cubeData = new CubeData[81];
        SpherePrimitive sphere;
        LightData[] lightData = new LightData[3];
        
        Matrix world;
        Matrix view;
        Matrix projection;
        Vector3 cameraPosition = new Vector3(0, 0, 50);
        Vector3 cameraTarget = new Vector3(0, 0, 0);

        #endregion

        #region Initialize
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            world = Matrix.Identity;
            view = Matrix.CreateLookAt(cameraPosition, new Vector3(0, 0, 0), Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.DisplayMode.AspectRatio, 0.1f, 1000f);
            cubeData[0].position = Vector3.Zero;//creates sky cube info
            cubeData[0].scale = 100f;
            cubeData[0].color = Color.White;
            cubeData[0].velocity = Vector3.Zero;

            CreateCubes();
            CreateSpheres();

            sphere = new SpherePrimitive(GraphicsDevice, 10f, 10);
            lightEffect = new BasicEffect(GraphicsDevice);

            base.Initialize();
        }

        public void CreateCubes()
        {
            Random random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            for(int i = 1; i < cubeData.Length; i++)
            {
                cubeData[i].position = new Vector3((float)random.NextDouble() * 180 - 90, (float)random.NextDouble() * 180 - 90, (float)random.NextDouble() * 180 - 90);
                cubeData[i].scale = (float)random.NextDouble() * 8 + 2;
                cubeData[i].color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), 1f);
                cubeData[i].velocity = (new Vector3((float)random.NextDouble() * 2 - 1, (float)random.NextDouble() * 2 - 1, (float)random.NextDouble() * 2 - 1));
                cubeData[i].velocity.Normalize();
                cubeData[i].velocity *= (float)random.NextDouble() * 5 + 5;
            }
        }

        public void CreateSpheres()
        {
            Random random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            for(int i = 0; i < lightData.Length; i++)
            {
                lightData[i].position = new Vector3((float)random.NextDouble() * 180 - 90, (float)random.NextDouble() * 180 - 90, (float)random.NextDouble() * 180 - 90);
                lightData[i].color = new Color((float)random.NextDouble() * 0.5f + 0.5f, (float)random.NextDouble() * 0.5f + 0.5f, (float)random.NextDouble() * 0.5f + 0.5f, 1f);
                lightData[i].velocity = (new Vector3((float)random.NextDouble() * 2 - 1, (float)random.NextDouble() * 2 - 1, (float)random.NextDouble() * 2 - 1));
                lightData[i].velocity.Normalize();
                lightData[i].velocity *= (float)random.NextDouble() * 5 + 5;
            }
        }

        

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            skyboxTexture = Content.Load<TextureCube>("Skyboxes/SkyBoxTexture");
            skyBoxEffect = Content.Load<Effect>("Skyboxes/skybox");

            cubeEffect = Content.Load<Effect>("Cubes/ambient");
            
            // TODO: use this.Content to load your game content here
        }
        #endregion

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState state = Keyboard.GetState();

            #region Rotation and Translations of View
            Matrix cameraRotation, cameraTranslation;

            if (state.IsKeyDown(Keys.Up))
                view *= Matrix.CreateRotationX(-0.05f);
            else if (state.IsKeyDown(Keys.Down))
                view *= Matrix.CreateRotationX(0.05f);
            else if (state.IsKeyDown(Keys.Left))
                view *= Matrix.CreateRotationY(-0.05f);
            else if (state.IsKeyDown(Keys.Right))
                view *= Matrix.CreateRotationY(0.05f);
            else if (state.IsKeyDown(Keys.Q))
                view *= Matrix.CreateRotationZ(0.05f);
            else if (state.IsKeyDown(Keys.E))
                view *= Matrix.CreateRotationZ(-0.05f);

            if (state.IsKeyDown(Keys.A))
            {
                view *= Matrix.CreateTranslation(1f, 0, 0);
            }
            else if (state.IsKeyDown(Keys.S))
            {
                view *= Matrix.CreateTranslation(0, 0, -1f);
            }
            else if (state.IsKeyDown(Keys.D))
            {
                view *= Matrix.CreateTranslation(-1f, 0, 0);
            }
            else if (state.IsKeyDown(Keys.W))
            {
                view *= Matrix.CreateTranslation(0, 0, 1f);
            }


            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            RasterizerState originalRasterizerState = graphics.GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullClockwiseFace;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;

            BlendState orginalBlendState = graphics.GraphicsDevice.BlendState;
            BlendState blendState = new BlendState();
            blendState = BlendState.NonPremultiplied;
            graphics.GraphicsDevice.BlendState = blendState;

            DepthStencilState orginalDepthState = graphics.GraphicsDevice.DepthStencilState;
            DepthStencilState depthState = new DepthStencilState();
            depthState = DepthStencilState.Default;
            graphics.GraphicsDevice.DepthStencilState = depthState;

            Vector4 color;

            //creates skycube
            foreach (EffectTechnique technique in skyBoxEffect.Techniques)
            {
                foreach (EffectPass pass in technique.Passes)
                {
                    pass.Apply();

                    skyBoxEffect.Parameters["World"].SetValue(world * Matrix.CreateScale(cubeData[0].scale) * Matrix.CreateTranslation(cubeData[0].position));
                    skyBoxEffect.Parameters["View"].SetValue(view);
                    skyBoxEffect.Parameters["Projection"].SetValue(projection);
                    skyBoxEffect.Parameters["SkyBoxTexture"].SetValue(skyboxTexture);
                    skyBoxEffect.Parameters["CameraPosition"].SetValue(cameraPosition);

                    cube.Render(GraphicsDevice);//renders the skycube
                }
            }

            Vector3 pos, v;

            for(int i = 0; i < lightData.Length; i++)
            {
                lightData[i].position += lightData[i].velocity * gameTime.ElapsedGameTime.Milliseconds / 1000f;

                Bounce(lightData[i].position, 10f, out v, out pos);

                lightData[i].position += pos;
                lightData[i].velocity *= v;

                foreach(EffectTechnique technique in lightEffect.Techniques)
                {
                    foreach(EffectPass pass in technique.Passes)
                    {
                        pass.Apply();

                        lightEffect.World = world * Matrix.CreateTranslation(lightData[i].position);
                        lightEffect.View = view;
                        lightEffect.Projection = projection;

                        lightEffect.EnableDefaultLighting();
                        lightEffect.EmissiveColor = lightData[i].color.ToVector3();
                        lightEffect.SpecularColor = Vector3.Zero;
                        lightEffect.DirectionalLight0.DiffuseColor = Vector3.Zero;
                        lightEffect.DirectionalLight0.SpecularColor = Vector3.Zero;

                        sphere.Draw(lightEffect);
                    }
                }

            }

            for(int i = 1; i < cubeData.Length; i++)
            {
                
                cubeData[i].position += cubeData[i].velocity * gameTime.ElapsedGameTime.Milliseconds / 1000f;

                Bounce(cubeData[i].position, cubeData[i].scale, out v, out pos);

                cubeData[i].position += pos;
                cubeData[i].velocity *= v;

                foreach(EffectTechnique technique in cubeEffect.Techniques)
                {
                    foreach(EffectPass pass in technique.Passes)
                    {
                        pass.Apply();

                        cubeEffect.Parameters["World"].SetValue(world * Matrix.CreateScale(cubeData[i].scale) * Matrix.CreateTranslation(cubeData[i].position));
                        cubeEffect.Parameters["View"].SetValue(view);
                        cubeEffect.Parameters["Projection"].SetValue(projection);

                        color = cubeData[i].color.ToVector4();
                        cubeEffect.Parameters["AmbientColor"].SetValue(color);
                        cubeEffect.Parameters["AmbientIntensity"].SetValue(1f);

                        cube.Render(GraphicsDevice);//renders the cube
                    }
                }
            }

            GraphicsDevice.RasterizerState = originalRasterizerState;
            //GraphicsDevice.BlendState = orginalBlendState;
            //GraphicsDevice.DepthStencilState = orginalDepthState;

            base.Draw(gameTime);
        }

        /// <summary>
        /// Checks if a object has bounced out of the skycube
        /// </summary>
        /// <param name="position"> current position of object</param>
        /// <param name="scale">the scale of the current object</param>
        /// <param name="x">is set to -1 if the velocity in x needs to be inverted. Set to 1 by default</param>
        /// <param name="y">is set to -1 if the velocity in y needs to be inverted. Set to 1 by default</param>
        /// <param name="z">is set to -1 if the velocity in z needs to be inverted. Set to 1 by default</param>
        /// <param name = "position">updated position</param>
        void Bounce(Vector3 position, float scale,  out Vector3 v, out Vector3 pos)
        {
            pos = Vector3.Zero;
            v = Vector3.One;

            if (position.X + scale >= 100f)
            {
                pos = new Vector3(-0.5f, 0, 0);
                v = new Vector3(-1f, 1f, 1f);
            }
            else if (position.X - scale <= -100f)
            {
                pos = new Vector3(0.5f, 0, 0);
                v = new Vector3(-1f, 1f, 1f);
            }


            if (position.Y + scale >= 100f)
            {
                pos = new Vector3(0, -0.5f, 0);
                v = new Vector3(1f, -1f, 1f);
            }
            else if (position.Y - scale <= -100f)
            {
                pos = new Vector3(0, 0.5f, 0);
                v = new Vector3(1f, -1f, 1f);
            }


            if (position.Z + scale >= 100f)
            {
                pos = new Vector3(0, 0, -0.5f);
                v = new Vector3(1f, 1f, -1f);
            }
            else if (position.Z - scale <= -100f)
            {
                pos = new Vector3(0, 0, 0.5f);
                v = new Vector3(1f, 1f, -1f);
            }

        }
    }
}
