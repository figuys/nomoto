using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace lenovo.mbg.service.lmsa.Feedback.View;

public partial class FeedbackListView : UserControl, IComponentConnector
{
	public FeedbackListView()
	{
		InitializeComponent();
		base.Loaded += FeedbackListView_Loaded;
	}

	private void FeedbackListView_Loaded(object sender, RoutedEventArgs e)
	{
	}

	private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		ScrollViewer scrollViewer = (ScrollViewer)sender;
		scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - (double)e.Delta);
		e.Handled = true;
	}
}
