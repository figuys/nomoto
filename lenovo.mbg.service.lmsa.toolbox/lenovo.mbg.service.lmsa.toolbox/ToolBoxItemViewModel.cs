using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.toolbox;

public class ToolBoxItemViewModel : ViewModelBase
{
	public ToolBoxType ToolBoxType { get; set; }

	public string ToolboxIcon { get; set; }

	public string ToolboxTitle { get; set; }

	public string ToolboxDescrption { get; set; }

	public ReplayCommand ItemClickCommand { get; set; }
}
