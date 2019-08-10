using GameTimer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonogameScreenTools
{
	public class ScreenGrabber : IScreenGrabber
	{
		#region Properties

		SpriteBatch SpriteBatch { get; set; }
		GraphicsDevice GraphicsDevice { get; set; }

		RenderTargetBinding[] prevRenderTargets;
		RenderTarget2D sceneRenderTarget;

		public ImageList CurrentImageList { get; set; }

		float FrameDelay { get; set; }
		CountdownTimer frameTimer;

		public bool Enabled { get; set; }

		#endregion //Properties

		#region Methods

		public ScreenGrabber(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, float frameDelay = 0.1f)
		{
			Enabled = true;
			SpriteBatch = spriteBatch;
			GraphicsDevice = graphicsDevice;

			// Look up the resolution and format of our main backbuffer.
			PresentationParameters pp = GraphicsDevice.PresentationParameters;

			int width = pp.BackBufferWidth;
			int height = pp.BackBufferHeight;

			SurfaceFormat format = pp.BackBufferFormat;

			// Create a texture for rendering the main scene, prior to applying bloom.
			sceneRenderTarget = new RenderTarget2D(GraphicsDevice, width, height, false,
												   format, pp.DepthStencilFormat, pp.MultiSampleCount,
												   RenderTargetUsage.PreserveContents);

			CurrentImageList = new ImageList(GraphicsDevice);

			FrameDelay = frameDelay;
			frameTimer = new CountdownTimer();
			frameTimer.Start(FrameDelay);
		}

		#endregion //Methods

		#region Draw

		/// <summary>
		/// This should be called at the very start of the scene rendering. The bloom
		/// component uses it to redirect drawing into its custom rendertarget, so it
		/// can capture the scene image in preparation for applying the bloom filter.
		/// </summary>
		public void BeginDraw()
		{
			if (Enabled)
			{
				prevRenderTargets = GraphicsDevice.GetRenderTargets();
				GraphicsDevice.SetRenderTarget(sceneRenderTarget);
			}
		}

		/// <summary>
		/// This is where it all happens. Grabs a scene that has already been rendered,
		/// and uses postprocess magic to add a glowing bloom effect over the top of it.
		/// </summary>
		/// <param name="gameTime"></param>
		public void Draw(GameTime gameTime)
		{
			if (Enabled)
			{
				frameTimer.Update(gameTime);

				//Grab a frame if the timer has expired
				if (!frameTimer.Paused && !frameTimer.HasTimeRemaining)
				{
					CurrentImageList.AddFrame(sceneRenderTarget, (int)(FrameDelay * 1000));
					frameTimer.Start(FrameDelay);
				}

				//Draw the entire rendered frame to the screen.
				GraphicsDevice.SetRenderTargets(prevRenderTargets);

				int width = GraphicsDevice.PresentationParameters.BackBufferWidth;
				int height = GraphicsDevice.PresentationParameters.BackBufferHeight;

				SpriteBatch.Begin(SpriteSortMode.Deferred,
								  BlendState.Opaque,
								  null, null, null, null);
				SpriteBatch.Draw(sceneRenderTarget, new Rectangle(0, 0, width, height), Color.White);
				SpriteBatch.End();
			}
		}

		#endregion
	}
}
