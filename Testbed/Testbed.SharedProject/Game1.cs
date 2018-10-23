using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonogameScreenTools;
using System;
using System.Threading.Tasks;

namespace Testbed
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		#region Properties

		#region MonoGame junk

		const int millisPerFrame = 100;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Texture2D mgLogo;
		Rectangle mgLogoBox;

		float rot = 0;
		Vector2 center = new Vector2(200, 200);
		bool gifSaveRequested = false;
		bool doSingleScreenshot = false;
		double lastUpdate = millisPerFrame;

		#endregion //MonoGame junk

		IGifHelper gif;
		IScreenShotHelper ssh;
		ImageList images;

		#endregion //Properties

		#region Methods

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);

			// using a small size until we implement some compression in the gifmaker
			graphics.PreferredBackBufferHeight = 480;
			graphics.PreferredBackBufferWidth = 640;
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
			base.Initialize();
			ssh = new ScreenShotHelper(GraphicsDevice);
			gif = new GifHelper(GraphicsDevice);
			images = new ImageList();
			mgLogoBox = new Rectangle((graphics.PreferredBackBufferWidth / 3) + 200, (graphics.PreferredBackBufferHeight / 3), 400, 400);
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
			mgLogo = Content.Load<Texture2D>("mglogo");
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
#if !__IOS__
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
#endif

			if (Keyboard.GetState().IsKeyDown(Keys.F12))
			{
				//only send a frame every 250ms
				lastUpdate += gameTime.ElapsedGameTime.TotalMilliseconds;
				if (lastUpdate > millisPerFrame)
				{
					gifSaveRequested = true;
					images.AddFrame(GraphicsDevice, millisPerFrame);
					lastUpdate = 0;
				}
			}
			else if (gifSaveRequested)
			{
				var result = gif.Export(images);
				images = new ImageList();
				Console.WriteLine($"Wrote gif to: {result}");
				gifSaveRequested = false;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.F9))
			{
				if (!doSingleScreenshot)
				{
					var result = ssh.SaveScreenshot();
					Console.WriteLine($"Wrote screenshot to: {result}");
					doSingleScreenshot = true;
				}
			}
			if (Keyboard.GetState().IsKeyUp(Keys.F9))
			{
				doSingleScreenshot = false;
			}

			rot += .05f;
			if (rot > 180)
				rot = 0;

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();
			spriteBatch.Draw(mgLogo, mgLogoBox, null, Color.White, rot, center, SpriteEffects.None, 0);
			spriteBatch.End();

			base.Draw(gameTime);
		}

		#endregion //Methods
	}
}
