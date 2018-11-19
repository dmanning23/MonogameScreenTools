using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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

		public ImageData(ImageData inst)
		{
			Data = new Color[inst.Data.Length];
			CopyImage(inst);
			DelayMS = inst.DelayMS;
		}

		public void CopyImage(ImageData inst)
		{
			Array.Copy(inst.Data, Data, inst.Data.Length);
			DelayMS = inst.DelayMS;
		}

		public void SetData(Texture2D tex, int delayMS)
		{
			tex.GetData<Color>(Data);
			DelayMS = delayMS;
		}

		#endregion //Methods
	}
}
