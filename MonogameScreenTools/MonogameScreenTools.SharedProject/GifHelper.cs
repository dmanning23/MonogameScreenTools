using ImageSharp;
using ImageSharp.Formats;
using ImageSharp.Processing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.Primitives;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace MonogameScreenTools
{
	public class GifHelper : IGifHelper
	{
		private GraphicsDevice GraphicsDevice { get; set; }
		BackgroundWorker _backgroundThread;
		string Filename;
		ImageList ImageList;
		float Scale;
		TimeSpan elapsed;

		/// <summary>
		/// Uses GifEncoder to Queue multiple frames and write them to file.
		/// </summary>
		/// <param name="device">The graphics device used to grab a backbuffer.</param>
		public GifHelper(GraphicsDevice graphicsDevice)
		{
			GraphicsDevice = graphicsDevice;
			elapsed = new TimeSpan();
		}

		public event EventHandler<GifCreatedEventArgs> OnGifCreated;

		/// <summary>
		/// Writes all images to file as a single GIF. 
		/// IF no filename is specified, a default filename using DateTime.Now.ToFileTime() is used.
		/// </summary>
		/// <param name="filename">The output filename</param>
		public void Export(ImageList imageList, string filename = "", bool appendTimeStamp = true, float scale = 0.5f)
		{
			ImageList = imageList;
			Scale = scale;

			//Setup the filename
			if (string.IsNullOrEmpty(filename))
			{
				filename = $"screencapture_";
				appendTimeStamp = true;
			}
			Filename = FileSystemHelper.CreateFilename(filename, ".gif", appendTimeStamp);

			_backgroundThread = new BackgroundWorker();
			_backgroundThread.WorkerSupportsCancellation = true;
			_backgroundThread.DoWork += new DoWorkEventHandler(BackgroundWorkerThread);
			_backgroundThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CleanUp);
			_backgroundThread.RunWorkerAsync();
		}

		#region Background Thread

		/// <summary>
		/// Worker thread draws the loading animation and updates the network
		/// session while the load is taking place.
		/// </summary>
		void BackgroundWorkerThread(object sender, DoWorkEventArgs e)
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			var width = GraphicsDevice.PresentationParameters.BackBufferWidth;
			var height = GraphicsDevice.PresentationParameters.BackBufferHeight;

			//open the file
			using (var stream = File.OpenWrite(Filename))
			{
				//create the image to be used
				using (var image = new Image<Rgba32>(width, height))
				{
					//Convert all the monogame info to ImageSharp data
					foreach (var imageData in ImageList.Images)
					{
						var imageFrame = new ImageFrame<Rgba32>(width, height);
						ConvertColorData(imageData.Data, imageFrame);
						imageFrame.MetaData.FrameDelay = imageData.DelayMS / 10;
						image.Frames.Add(imageFrame);
					}

					//TODO: resizeing doesn't work in the alpha build :(
					//image.Resize((int)(image.Width * Scale), (int)(image.Height * Scale));

					//Save it all out!
					image.SaveAsGif(stream, new GifEncoder());
				}
			}

			stopWatch.Stop();
			elapsed = stopWatch.Elapsed;
		}

		private static void ConvertColorData(Color[] imageData, ImageFrame<Rgba32> imageBuffer)
		{
			for (var i = 0; i < imageData.Length; i++)
			{
				var data = imageData[i];
				imageBuffer.Pixels[i] = new Rgba32(data.R, data.G, data.B, data.A);
			}
		}

		void CleanUp(object sender, RunWorkerCompletedEventArgs e)
		{
			//clean up all the memory from those other screens
			GC.Collect();

			if (null != OnGifCreated)
			{
				OnGifCreated(this, new GifCreatedEventArgs(Filename, elapsed));
			}
		}

		#endregion //Background Thread
	}
}
