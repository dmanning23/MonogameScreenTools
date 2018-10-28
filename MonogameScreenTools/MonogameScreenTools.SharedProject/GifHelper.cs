using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MonogameScreenTools
{
	public class GifHelper : IGifHelper
	{
		#region Properties

		BackgroundWorker _backgroundThread;

		TimeSpan resizeTime;
		TimeSpan totalTime;

		private GraphicsDevice GraphicsDevice { get; set; }

		public List<ImageData> ImageList { get; set; }
		int width;
		int height;

		string Filename { get; set; }

		float Scale { get; set; }

		/// <summary>
		/// The length of the pause to add at the end of the gif
		/// </summary>
		float EndPause { get; set; } = 0.5f;

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
		public void Export(ImageList imageList, string filename = "", bool appendTimeStamp = true, float scale = 0.35f, float endPause = 0.6f)
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

			Task.Run(() => TaskRunThread());

			//_backgroundThread = new BackgroundWorker();
			//_backgroundThread.WorkerReportsProgress = false;
			//_backgroundThread.WorkerSupportsCancellation = true;
			//_backgroundThread.DoWork += new DoWorkEventHandler(BackgroundWorkerThread);
			//_backgroundThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CleanUp);
			//_backgroundThread.RunWorkerAsync();
		}

		#endregion //Methods

		#region Background Thread

		/// <summary>
		/// Worker thread draws the loading animation and updates the network
		/// session while the load is taking place.
		/// </summary>
		void BackgroundWorkerThread(object sender, DoWorkEventArgs e)
		{
			CreateGif();
		}

		void TaskRunThread()
		{
			CreateGif();
			DoCleanup();
		}

		private void CreateGif()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			//open the file
			using (var stream = File.OpenWrite(Filename))
			{
				var rgbaBuffer = new Rgba32[width * height];

				//create the image to be used
				using (var image = new Image<Rgba32>(width, height))
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

					var resizeStopwatch = new Stopwatch();
					resizeStopwatch.Start();

					image.Mutate(ctx => ctx.Resize((int)(image.Width * Scale), (int)(image.Height * Scale)));

					resizeStopwatch.Stop();
					resizeTime = resizeStopwatch.Elapsed;

					//Save it all out!
					image.SaveAsGif(stream, new GifEncoder() { ColorTableMode = GifColorTableMode.Local });
				}
			}

			stopWatch.Stop();
			totalTime = stopWatch.Elapsed;
		}

		private static void ConvertColorData(Color[] imageData, Rgba32[] imageBuffer)
		{
			for (var i = 0; i < imageData.Length; i++)
			{
				var data = imageData[i];
				imageBuffer[i] = new Rgba32(data.R, data.G, data.B, data.A);
			}
		}

		void CleanUp(object sender, RunWorkerCompletedEventArgs e)
		{
			DoCleanup();
		}

		private void DoCleanup()
		{
			//clean up all the memory from those other screens
			GC.Collect();

			if (null != OnGifCreated)
			{
				OnGifCreated(this, new GifCreatedEventArgs(Filename, resizeTime, totalTime));
			}
		}

		#endregion //Background Thread
	}
}
