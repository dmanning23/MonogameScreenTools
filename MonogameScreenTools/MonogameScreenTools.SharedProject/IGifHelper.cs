using System;

namespace MonogameScreenTools
{
	public interface IGifHelper
	{
		event EventHandler<GifCreatedEventArgs> OnGifCreated;

		void Export(ImageList imageList, string filename = "", bool appendTimeStamp = true, int scale = 4, float endPause = 0.6f);
	}
}