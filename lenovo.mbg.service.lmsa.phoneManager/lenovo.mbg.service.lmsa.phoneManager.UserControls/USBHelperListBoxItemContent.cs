using System.Windows;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControls;

public class USBHelperListBoxItemContent
{
	public Visibility CircleVisibility { get; set; }

	public Visibility LeftLineVisibility { get; set; }

	public Visibility RightLineVisibility { get; set; }

	public int Sequence { get; set; }

	public object MaskLayer { get; set; }

	public ImageSource ImgSource { get; set; }
}
