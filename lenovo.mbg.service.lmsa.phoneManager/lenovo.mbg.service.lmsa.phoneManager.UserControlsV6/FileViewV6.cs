using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using lenovo.mbg.service.lmsa.phoneManager.ViewContextV6;
using lenovo.mbg.service.lmsa.phoneManager.ViewModelV6;

namespace lenovo.mbg.service.lmsa.phoneManager.UserControlsV6;

public partial class FileViewV6 : UserControl, IComponentConnector
{
	public FileViewV6()
	{
		InitializeComponent();
	}

	private void pcDirecotries_MouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
	}

	private void pcDirecotries_MouseMove(object sender, MouseEventArgs e)
	{
		if (e.LeftButton == MouseButtonState.Pressed && Context.FindViewModel<FileViewModel>(typeof(FileViewV6)).PCDataGridSelected is FileDataViewModel { IsFolder: false } fileDataViewModel)
		{
			DataObject data = new DataObject(typeof(DrapDropFileParameter), new DrapDropFileParameter(0, fileDataViewModel));
			DragDrop.DoDragDrop(pcDirecotries, data, DragDropEffects.Move);
		}
	}

	private void pcDirecotries_Drop(object sender, DragEventArgs e)
	{
		if (e.Data.GetData(typeof(DrapDropFileParameter)) is DrapDropFileParameter { Owner: not 0 } drapDropFileParameter)
		{
			Context.FindViewModel<FileViewModel>(typeof(FileViewV6)).Export(drapDropFileParameter.FolderFile as FileDataViewModel);
		}
	}

	private void phoneDirecotries_MouseMove(object sender, MouseEventArgs e)
	{
		if (e.LeftButton == MouseButtonState.Pressed && Context.FindViewModel<FileViewModel>(typeof(FileViewV6)).PhoneDataGridSelected is FileDataViewModel { IsFolder: false } fileDataViewModel)
		{
			DataObject data = new DataObject(typeof(DrapDropFileParameter), new DrapDropFileParameter(1, fileDataViewModel));
			DragDrop.DoDragDrop(phoneDirecotries, data, DragDropEffects.Move);
		}
	}

	private void phoneDirecotries_Drop(object sender, DragEventArgs e)
	{
		if (e.Data.GetData(typeof(DrapDropFileParameter)) is DrapDropFileParameter { Owner: not 1 } drapDropFileParameter)
		{
			Context.FindViewModel<FileViewModel>(typeof(FileViewV6)).Import(drapDropFileParameter.FolderFile as FileDataViewModel);
		}
	}
}
