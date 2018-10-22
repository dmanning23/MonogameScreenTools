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

		public List<ImageData> Images { get; private set; }

		#endregion //Properties

		#region Methods

		/// <summary>
		/// Uses GifEncoder to Queue multiple frames and write them to file.
		/// </summary>
		/// <param name="device">The graphics device used to grab a backbuffer.</param>
		public ImageList()
		{
			Images = new List<ImageData>();
		}

		/// <summary>
		/// Adds a single frame to the Frame Queue
		/// </summary>
		/// <param name="delayMilliseconds">Optional delay for this frame in milliseconds </param>
		public void AddFrame(GraphicsDevice graphicsDevice, int delayMilliseconds = 0)
		{
			Images.Add(new ImageData(graphicsDevice, delayMilliseconds));
		}

		public void AppendFrames(ImageList nextFrames)
		{
			Images.AddRange(nextFrames.Images);
		}

		#endregion //Methods
	}
}
