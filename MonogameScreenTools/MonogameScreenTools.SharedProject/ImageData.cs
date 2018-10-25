using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;

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

		public ImageData(GraphicsDevice graphicsDevice)
		{
			Data = new Color[graphicsDevice.PresentationParameters.BackBufferWidth * graphicsDevice.PresentationParameters.BackBufferHeight];
		}

		public void SetData(GraphicsDevice graphicsDevice, int delayMS)
		{
			graphicsDevice.GetBackBufferData<Color>(Data);
			DelayMS = delayMS;
		}

		#endregion //Methods
	}
}
