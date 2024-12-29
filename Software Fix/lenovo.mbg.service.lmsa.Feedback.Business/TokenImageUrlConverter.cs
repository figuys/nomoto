using lenovo.themes.generic.AttachedProperty;

namespace lenovo.mbg.service.lmsa.Feedback.Business;

public class TokenImageUrlConverter : ImageDownloader.IUrlConverter
{
	private FeedBackBLL bll = new FeedBackBLL();

	public string Convert(string originalUrl)
	{
		return bll.GetUrlWithToken(originalUrl).Result;
	}
}
