namespace lenovo.mbg.service.lmsa.toolbox.Business;

internal interface IDeviceClipboard
{
	bool ImportClipboardInfo(string ClipBoardContent);

	string GetClipboardInfo();
}
