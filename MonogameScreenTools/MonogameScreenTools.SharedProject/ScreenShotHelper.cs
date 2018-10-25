using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace MonogameScreenTools
{
	public class ScreenShotHelper : IScreenShotHelper
	{
		#region Properties

		private GraphicsDevice GraphicsDevice { get; set; }

		#endregion //Properties

		#region Methods

		#endregion //Methods

		public ScreenShotHelper(GraphicsDevice graphicsDevice)
		{
			GraphicsDevice = graphicsDevice;
		}

		/// <summary>
		/// Generate and save a screenshot.
		/// </summary>
		/// <param name="filename">The filename you'd like to use, with no path or file extension. Leave blank for the default filename</param>
		/// <param name="appendTimeStamp">Whether or not to append the current date/time to the end of the filename. </param>
		/// <returns>The full path, filename, and extension of the file that was created.</returns>
		public string SaveScreenshot(string filename = "", bool appendTimeStamp = true)
		{
			//Setup the filename
			if (string.IsNullOrEmpty(filename))
			{
				filename = $"screenshot_";
				appendTimeStamp = true;
			}
			filename = FileSystemHelper.CreateFilename(filename, ".png", appendTimeStamp);

			//Dump the screen contents to a file
			var width = GraphicsDevice.PresentationParameters.BackBufferWidth;
			var height = GraphicsDevice.PresentationParameters.BackBufferHeight;
			var colors = new Color[width * height];
			GraphicsDevice.GetBackBufferData<Color>(colors);
			using (var tex2D = new Texture2D(GraphicsDevice, width, height))
			{
				tex2D.SetData<Color>(colors);
				using (var stream = File.Create(filename))
				{
					tex2D.SaveAsPng(stream, width, height);
				}
			}

			return filename;
		}
	}
}
