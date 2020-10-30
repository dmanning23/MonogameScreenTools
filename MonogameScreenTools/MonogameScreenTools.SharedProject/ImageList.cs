using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonogameScreenTools
{
	/// <summary>
	/// This is a list of a bunch of frames of animation
	/// </summary>
	public class ImageList
	{
		#region Properties

		public int NumImages { get; private set; }

		public Queue<ImageData> Images { get; private set; }

		private Stack<ImageData> Warehouse { get; set; }

		public int Width { get; private set; }

		public int Height { get; private set; }

		private object _lock = new object();

		#endregion //Properties

		#region Methods

		/// <summary>
		/// Uses GifEncoder to Queue multiple frames and write them to file.
		/// </summary>
		/// <param name="device">The graphics device used to grab a backbuffer.</param>
		public ImageList(GraphicsDevice graphicsDevice, int numImages = 20)
		{
			NumImages = numImages;
			Images = new Queue<ImageData>();
			Warehouse = new Stack<ImageData>();

			Width = graphicsDevice.PresentationParameters.BackBufferWidth;
			Height = graphicsDevice.PresentationParameters.BackBufferHeight;

			for (int i = 0; i < NumImages; i++)
			{
				Warehouse.Push(new ImageData(graphicsDevice));
			}
		}

		public void CopyImageList(ImageList inst)
		{
			lock (_lock)
			{
				NumImages = inst.NumImages;
				Width = inst.Width;
				Height = inst.Height;

				foreach (var image in inst.Images)
				{
					AddImage(image);
				}
			}
		}

		public void AddFrame(Texture2D tex, int delayMilliseconds = 0)
		{
			lock (_lock)
			{
				if (Images.Count >= NumImages)
				{
					var image = Images.Dequeue();
					image.SetData(tex, delayMilliseconds);
					Images.Enqueue(image);
				}
				else if (Warehouse.Count > 0)
				{
					var image = Warehouse.Pop();
					image.SetData(tex, delayMilliseconds);
					Images.Enqueue(image);
				}
			}
		}

		protected void AddImage(ImageData imageInst)
		{
			if (Images.Count >= NumImages)
			{
				var image = Images.Dequeue();
				image.CopyImage(imageInst);
				Images.Enqueue(image);
			}
			else if (Warehouse.Count > 0)
			{
				var image = Warehouse.Pop();
				image.CopyImage(imageInst);
				Images.Enqueue(image);
			}
		}

		public void Clear()
		{
			lock (_lock)
			{
				while (Images.Count > 0)
				{
					var image = Images.Dequeue();
					if (Warehouse.Count < NumImages)
					{
						Warehouse.Push(image);
					}
				}
			}
		}

		#endregion //Methods
	}
}
