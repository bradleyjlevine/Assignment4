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
        //fields for the game; effects, shapes, and matrices
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
        float yaw = 0f, pitch = 0f, cameraSpeed = 5f;

        #endregion

        //this initalizes the game
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

        #region Create Cubes and Spheres
        /// <summary>
        /// This creates the other 80 cubes; position, scale, color, and velocity.  The random object is seeded with the current time.
        /// </summary>
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

        /// <summary>
        /// This creates 3 spheres; position, radius is 10 (scale) and tesselation is 10, color, and velocity.  The random object is seeded with the current time.
        /// </summary>
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

        #endregion

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //loads the shader code and texturecube for the skybox
            skyboxTexture = Content.Load<TextureCube>("Skyboxes/SkyBoxTexture");
            skyBoxEffect = Content.Load<Effect>("Skyboxes/skybox");

            cubeEffect = Content.Load<Effect>("Cubes/pointlight");
        }
        #endregion

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            skyBoxEffect.Dispose();
            skyboxTexture.Dispose();
            lightEffect.Dispose();
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

            //gets the state of the keyboards
            KeyboardState state = Keyboard.GetState();

            #region Rotation and Translations of View
            Matrix rotation;

            if (state.IsKeyDown(Keys.Up))
                pitch += 0.05f;
            else if (state.IsKeyDown(Keys.Down))
                pitch -= 0.05f;
            else if (state.IsKeyDown(Keys.Left))
                yaw += 0.05f;
            else if (state.IsKeyDown(Keys.Right))
                yaw -= 0.05f;

            rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0f);

            if (state.IsKeyDown(Keys.A))
            {
                cameraPosition += rotation.Left * cameraSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000f;
            }
            else if (state.IsKeyDown(Keys.S))
            {
                cameraPosition += rotation.Backward * cameraSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000f;
            }
            else if (state.IsKeyDown(Keys.D))
            {
                cameraPosition += rotation.Right * cameraSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000f;
            }
            else if (state.IsKeyDown(Keys.W))
            {
                cameraPosition += rotation.Forward * cameraSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000f;
            }

            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + rotation.Forward, rotation.Up);

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
            rasterizerState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;

            //renders skycube
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

            rasterizerState = RasterizerState.CullCounterClockwise;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;

            #region Rendering PointLights and the 80 cubes
            Vector3 pos, v;
            Vector3[] lightPositions = new Vector3[3];
            Vector4[] lightColors = new Vector4[3];

            //generates the spheres to represent the point lights
            for(int i = 0; i < lightData.Length; i++)
            {
                //updates the position based on the velocity multiplied by the seconds passed
                lightData[i].position += lightData[i].velocity * gameTime.ElapsedGameTime.Milliseconds / 1000f;

                Bounce(lightData[i].position, 10f, out v, out pos);

                lightData[i].position += pos;
                lightData[i].velocity *= v;

                lightPositions[i] = lightData[i].position;
                lightColors[i] = lightData[i].color.ToVector4();

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

            //renders the other 80 cubes
            for(int i = 1; i < cubeData.Length; i++)
            {
                //updates the position of the cube based on the velocity multiplied by the number of seconds that passed
                cubeData[i].position += cubeData[i].velocity * gameTime.ElapsedGameTime.Milliseconds / 1000f;

                Bounce(cubeData[i].position, cubeData[i].scale, out v, out pos);

                cubeData[i].position += pos;
                cubeData[i].velocity *= v;

                //shades each cube for the three point lights

                foreach (EffectTechnique technique in cubeEffect.Techniques)
                {
                    foreach (EffectPass pass in technique.Passes)
                    {
                        pass.Apply();

                        cubeEffect.Parameters["World"].SetValue(world * Matrix.CreateScale(cubeData[i].scale) * Matrix.CreateTranslation(cubeData[i].position));
                        cubeEffect.Parameters["View"].SetValue(view);
                        cubeEffect.Parameters["Projection"].SetValue(projection);

                        cubeEffect.Parameters["AmbientColor"].SetValue(cubeData[i].color.ToVector4());
                        cubeEffect.Parameters["AmbientIntensity"].SetValue(1f);

                        cubeEffect.Parameters["DiffuseColor"].SetValue(cubeData[i].color.ToVector4());
                        cubeEffect.Parameters["DiffuseIntensity"].SetValue(0.7f);
                        cubeEffect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert((world * Matrix.CreateScale(cubeData[i].scale) * Matrix.CreateTranslation(cubeData[i].position)))));
                        cubeEffect.Parameters["LightPosition"].SetValue(lightPositions);

                        cubeEffect.Parameters["Camera"].SetValue(cameraPosition);

                        cubeEffect.Parameters["Shininess"].SetValue(30f);
                        cubeEffect.Parameters["SpecularColor"].SetValue(lightColors);

                        cube.Render(GraphicsDevice);//renders the cube
                    }
                }
            }

            #endregion

            GraphicsDevice.RasterizerState = originalRasterizerState;

            base.Draw(gameTime);
        }

        /// <summary>
        /// This checks if the postion is out of the skybox and inverts the velocity and moves the the position back by 1.
        /// </summary>
        /// <param name="position">position of the object</param>
        /// <param name="scale">if you scaled the object</param>
        /// <param name="v">multiply this to your volicty vector to update it</param>
        /// <param name="pos">add this to your position vector to update it</param>
        void Bounce(Vector3 position, float scale,  out Vector3 v, out Vector3 pos)
        {
            pos = Vector3.Zero;
            v = Vector3.One;

            if (position.X + scale >= (100f))
            {
                pos = new Vector3(-0.5f, 0, 0);
                v = new Vector3(-1f, 1f, 1f);
            }
            else if (position.X - scale <= -(100f))
            {
                pos = new Vector3(0.5f, 0, 0);
                v = new Vector3(-1f, 1f, 1f);
            }


            if (position.Y + scale >= (100f))
            {
                pos = new Vector3(0, -0.5f, 0);
                v = new Vector3(1f, -1f, 1f);
            }
            else if (position.Y - scale  <= -(100f))
            {
                pos = new Vector3(0, 0.5f, 0);
                v = new Vector3(1f, -1f, 1f);
            }


            if (position.Z + scale >= (100f))
            {
                pos = new Vector3(0, 0, -0.5f);
                v = new Vector3(1f, 1f, -1f);
            }
            else if (position.Z - scale <= -(100f))
            {
                pos = new Vector3(0, 0, 0.5f);
                v = new Vector3(1f, 1f, -1f);
            }

        }
    }
}
