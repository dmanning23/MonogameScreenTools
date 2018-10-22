using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonogameScreenTools
{
	/// <summary>
	/// This is a single frame of the animation
	/// </summary>
	public class ImageData
	{
		#region Properties

		public Color[] Data { get; set; }

		public int DelayMS { get; set; }

		#endregion //Properties

		#region Methods

		public ImageData(GraphicsDevice graphicsDevice, int delyaMS)
		{
			Data = new Color[graphicsDevice.Viewport.Width * graphicsDevice.Viewport.Height];
			graphicsDevice.GetBackBufferData<Color>(Data);

			DelayMS = delyaMS;
		}

		#endregion //Methods
	}
}
