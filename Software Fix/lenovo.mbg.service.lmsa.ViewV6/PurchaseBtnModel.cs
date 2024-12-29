using System.Windows.Media;

namespace lenovo.mbg.service.lmsa.ViewV6;

public class PurchaseBtnModel
{
	public bool IsSelected { get; set; }

	public string NameLangKey { get; set; }

	public ImageSource NormalIcon { get; set; }

	public ImageSource SelectedIcon { get; set; }
}
