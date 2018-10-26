﻿using System;
using System.Collections.Generic;
using System.Text;
using GameTimer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonogameScreenTools
{
	public class ScreenGrabber
	{
		#region Properties

		SpriteBatch SpriteBatch { get; set; }
		GraphicsDevice GraphicsDevice { get; set; }

		RenderTarget2D sceneRenderTarget;

		public ImageList CurrentImageList { get; set; }

		float FrameDelay { get; set; }
		CountdownTimer frameTimer;

		#endregion //Properties

		#region Methods

		public ScreenGrabber(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, float frameDelay = 0.1f)
		{
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
												   RenderTargetUsage.DiscardContents);

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
			GraphicsDevice.SetRenderTarget(sceneRenderTarget);
		}

		/// <summary>
		/// This is where it all happens. Grabs a scene that has already been rendered,
		/// and uses postprocess magic to add a glowing bloom effect over the top of it.
		/// </summary>
		/// <param name="gameTime"></param>
		public void Draw(GameTime gameTime)
		{
			frameTimer.Update(gameTime);

			if (!frameTimer.Paused && !frameTimer.HasTimeRemaining)
			{
				CurrentImageList.AddFrame(sceneRenderTarget, (int)(FrameDelay * 1000));
				frameTimer.Start(FrameDelay);
			}

			// Pass 4: draw both rendertarget 1 and the original scene
			// image back into the main backbuffer, using a shader that
			// combines them to produce the final bloomed result.
			GraphicsDevice.SetRenderTarget(null);

			// Look up the resolution and format of our main backbuffer.
			int width = GraphicsDevice.PresentationParameters.BackBufferWidth;
			int height = GraphicsDevice.PresentationParameters.BackBufferHeight;

			DrawFullscreenQuad(sceneRenderTarget, width, height);
		}

		/// <summary>
		/// Helper for drawing a texture into the current rendertarget,
		/// using a custom shader to apply postprocessing effects.
		/// </summary>
		void DrawFullscreenQuad(Texture2D texture, int width, int height)
		{
			SpriteBatch.Begin(SpriteSortMode.Deferred,
							  BlendState.Opaque,
							  null, null, null, null);
			SpriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
			SpriteBatch.End();
		}

		#endregion
	}
}
