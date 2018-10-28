using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MonogameScreenTools
{
	public class GifHelper : IGifHelper
	{
		#region Properties

		TimeSpan totalTime;

		private GraphicsDevice GraphicsDevice { get; set; }

		public List<ImageData> ImageList { get; set; }
		int width;
		int height;

		string Filename { get; set; }

		int Scale { get; set; }

		/// <summary>
		/// The length of the pause to add at the end of the gif
		/// </summary>
		float EndPause { get; set; }

		public event EventHandler<GifCreatedEventArgs> OnGifCreated;

		#endregion //Properties

		#region Methods

		/// <summary>
		/// Uses GifEncoder to Queue multiple frames and write them to file.
		/// </summary>
		/// <param name="device">The graphics device used to grab a backbuffer.</param>
		public GifHelper()
		{
		}

		/// <summary>
		/// Writes all images to file as a single GIF. 
		/// IF no filename is specified, a default filename using DateTime.Now.ToFileTime() is used.
		/// </summary>
		/// <param name="filename">The output filename</param>
		public void Export(ImageList imageList, string filename = "", bool appendTimeStamp = true, int scale = 4, float endPause = 0.6f)
		{
			ImageList = imageList.Images.ToList();
			width = imageList.Width;
			height = imageList.Height;
			Scale = scale;
			EndPause = endPause;

			//Setup the filename
			if (string.IsNullOrEmpty(filename))
			{
				filename = $"screencapture";
				appendTimeStamp = true;
			}
			Filename = FileSystemHelper.CreateFilename(filename, ".gif", appendTimeStamp);

			Task.Run(() => WorkerThread());
		}

		#endregion //Methods

		#region Background Thread

		void WorkerThread()
		{
			CreateGif();
			Cleanup();
		}

		private void CreateGif()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			//open the file
			using (var stream = File.OpenWrite(Filename))
			{
				var rgbaBuffer = new Rgba32[(width * height) / Scale];

				//create the image to be used
				using (var image = new Image<Rgba32>(width / Scale, height / Scale))
				{
					//Convert all the monogame info to ImageSharp data
					int imageIndex = 0;
					foreach (var imageData in ImageList)
					{
						ConvertColorData(imageData.Data, rgbaBuffer);
						var frame = image.Frames.AddFrame(rgbaBuffer);
						var metaData = frame.MetaData.GetFormatMetaData(GifFormat.Instance);

						if (imageIndex == ImageList.Count - 1)
						{
							metaData.FrameDelay = (int)(EndPause * 1000) / 10;
						}
						else
						{
							metaData.FrameDelay = imageData.DelayMS / 10;
						}

						imageIndex++;
					}

					// remove the frame created with image creation
					image.Frames.RemoveFrame(0);

					//Save it all out!
					image.SaveAsGif(stream, new GifEncoder() { ColorTableMode = GifColorTableMode.Local });
				}
			}

			stopWatch.Stop();
			totalTime = stopWatch.Elapsed;
		}

		private void ConvertColorData(Color[] imageData, Rgba32[] imageBuffer)
		{
			var index = 0;
			for (int j = 0; j < height; j += Scale)
			{
				for (int i = 0; i < width; i += Scale)
				{
					var dataIndex = (j * width) + i;
					var data = imageData[dataIndex];
					if (index < imageBuffer.Count())
					{
						imageBuffer[index] = new Rgba32(data.R, data.G, data.B, data.A);
						index++;
					}
				}
			}
		}

		private void Cleanup()
		{
			//clean up all the memory from those other screens
			GC.Collect();

			if (null != OnGifCreated)
			{
				OnGifCreated(this, new GifCreatedEventArgs(Filename, totalTime));
			}
		}

		#endregion //Background Thread
	}
}
