using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace lenovo.mbg.service.lmsa.ResourcesCleanUp.View;

public partial class DeleteResourceView : UserControl, IComponentConnector
{
	private Rectangle m_rect = null;

	private TextBlock m_text = null;

	public DeleteResourceView()
	{
		InitializeComponent();
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		progress.ApplyTemplate();
		m_rect = progress.Template.FindName("Animation", progress) as Rectangle;
		m_rect.SizeChanged += M_rect_SizeChanged;
		m_text = progress.Template.FindName("perecetage", progress) as TextBlock;
	}

	private void M_rect_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		if (e.WidthChanged && e.NewSize.Width > m_text.ActualWidth)
		{
			m_text.Width = e.NewSize.Width;
		}
	}
}
