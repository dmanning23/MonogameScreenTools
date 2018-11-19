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

		public int NumImages { get; set; }

		public Queue<ImageData> Images { get; private set; }

		private Stack<ImageData> Warehouse { get; set; }

		public int Width { get; private set; }

		public int Height { get; private set; }

		#endregion //Properties

		#region Methods

		/// <summary>
		/// Uses GifEncoder to Queue multiple frames and write them to file.
		/// </summary>
		/// <param name="device">The graphics device used to grab a backbuffer.</param>
		public ImageList(GraphicsDevice graphicsDevice)
		{
			NumImages = 20;
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
			NumImages = inst.NumImages;
			Width = inst.Width;
			Height = inst.Height;

			foreach (var image in inst.Images)
			{
				AddImage(image);
			}
		}

		public void AddFrame(Texture2D tex, int delayMilliseconds = 0)
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
			foreach (var image in Images)
			{
				Warehouse.Push(Images.Dequeue());
			}
		}

		#endregion //Methods
	}
}
