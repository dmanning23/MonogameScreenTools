using System;

namespace MonogameScreenTools
{
	public interface IGifHelper
	{
		event EventHandler<GifCreatedEventArgs> OnGifCreated;

		void Export(ImageList imageList, string filename = "", bool appendTimeStamp = true, float scale = 0.35f, float endPause = 0.6f);
	}
}