using lenovo.mbg.service.common.utilities;

namespace lenovo.themes.generic.ModelV6;

public class TreeModel
{
	public string ParentID { get; set; }

	public string Name { get; set; }

	public string ID { get; set; }

	public string Sort { get; set; }

	public string Data { get; set; }

	public string ImageKey { get; set; }

	public TreeModel(string id, string parentId, string name, string data, string imageKey = null)
	{
		ID = GlobalFun.GetStringMd5(id);
		ParentID = parentId;
		Name = name;
		Data = data;
		ImageKey = imageKey;
	}
}
