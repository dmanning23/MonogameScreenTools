using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace MonogameScreenTools
{
	public class GifHelper : IGifHelper
	{
		private GraphicsDevice GraphicsDevice { get; set; }

		/// <summary>
		/// Uses GifEncoder to Queue multiple frames and write them to file.
		/// </summary>
		/// <param name="device">The graphics device used to grab a backbuffer.</param>
		public GifHelper(GraphicsDevice graphicsDevice)
		{
			GraphicsDevice = graphicsDevice;
		}

		/// <summary>
		/// Writes all images to file as a single GIF. 
		/// IF no filename is specified, a default filename using DateTime.Now.ToFileTime() is used.
		/// </summary>
		/// <param name="filename">The output filename</param>
		public string Export(ImageList imageList, string filename = "", bool appendTimeStamp = true)
		{
			//Setup the filename
			if (string.IsNullOrEmpty(filename))
			{
				filename = $"screencapture_";
				appendTimeStamp = true;
			}
			filename = FileSystemHelper.CreateFilename(filename, ".gif", appendTimeStamp);

			//scratch buffer for writing stuff out
			var rgbaBuffer = new Rgba32[GraphicsDevice.Viewport.Width * GraphicsDevice.Viewport.Height];

			//open the file
			using (var stream = File.OpenWrite(filename))
			{
				//create the image to be used
				using (var image = new Image<Rgba32>(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height))
				{
					//Convert all the monogame info to ImageSharp data
					var frames = image.Frames;
					foreach (var imageData in imageList.Images)
					{
						ConvertColorData(imageData.Data, rgbaBuffer);
						var frame = frames.AddFrame(rgbaBuffer);
						frame.MetaData.FrameDelay = imageData.DelayMS;
					}

					//Save it all out!
					image.SaveAsGif(stream, new GifEncoder());
				}
			}

			return filename;
		}

		private static void ConvertColorData(Color[] imageData, Rgba32[] imageBuffer)
		{
			for (var i = 0; i < imageData.Length; i++)
			{
				var data = imageData[i];
				imageBuffer[i] = new Rgba32(data.R, data.G, data.B, data.A);
			}
		}
	}
}
