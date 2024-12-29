using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using lenovo.themes.generic.ViewModels;

namespace lenovo.themes.generic.Controls;

public partial class UserControl1 : UserControl, IComponentConnector
{
	public static readonly DependencyProperty OkCandelViewModelTestProperty = DependencyProperty.Register("OkCandelViewModelTest", typeof(OKCancelViewModel), typeof(UserControl1), new PropertyMetadata(null));

	public OKCancelViewModel OkCandelViewModelTest
	{
		get
		{
			return (OKCancelViewModel)GetValue(OkCandelViewModelTestProperty);
		}
		set
		{
			SetValue(OkCandelViewModelTestProperty, value);
		}
	}

	public UserControl1()
	{
		OkCandelViewModelTest = OKCancelViewModel.DefaultValues();
		OkCandelViewModelTest.Title = "Delete the selected values?";
		OkCandelViewModelTest.Content = "yes,i am sure delete the selecte values";
		OkCandelViewModelTest.CancelButtonText = "K0208";
		OkCandelViewModelTest.OKButtonText = "K0583";
		InitializeComponent();
	}
}
