
namespace MonogameScreenTools
{
	public interface IGifHelper
	{
		string Export(ImageList imageList, string filename = "", bool appendTimeStamp = true);
	}
}