#region Using Statments
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Primitives;
#endregion
namespace Assignment4

{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        #region Fields
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Effect skyBoxEffect;
        TextureCube skyboxTexture;

        Cube skybox;
        Matrix world;
        Matrix view;
        Matrix projection;
        Vector3 cameraPosition = new Vector3(0, 0, 20);
        Vector3 cameraTarget = new Vector3(0, 0, 0);
        float temp = 0f;

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
            Matrix world = Matrix.Identity;
            Matrix view = Matrix.CreateLookAt(cameraPosition, new Vector3(0, 0, 0), Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.DisplayMode.AspectRatio, 0.1f, 200f);
            skybox = new Cube();
            skybox.Scale = 100;

            base.Initialize();
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

            // TODO: Add your update logic here

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
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;


            foreach (EffectTechnique technique in skyBoxEffect.Techniques)
            {
                foreach (EffectPass pass in technique.Passes)
                {
                    pass.Apply();

                    skyBoxEffect.Parameters["World"].SetValue(world);
                    skyBoxEffect.Parameters["View"].SetValue(view);
                    skyBoxEffect.Parameters["Projection"].SetValue(projection);
                    skyBoxEffect.Parameters["SkyBoxTexture"].SetValue(skyboxTexture);
                    skyBoxEffect.Parameters["CameraPosition"].SetValue(cameraPosition);

                    skybox.Render(GraphicsDevice);
                }
            }

            GraphicsDevice.RasterizerState = originalRasterizerState;

            base.Draw(gameTime);
        }
    }
}
