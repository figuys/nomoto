using System.Windows;
using System.Windows.Media;

namespace lenovo.mbg.service.lmsa.toolbox.GifMaker.Gif.PlotCore;

public interface IPlotBase
{
	Size OwnerSize { get; set; }

	Point Start { get; set; }

	AchorType Achor { get; set; }

	PlotType PlType { get; set; }

	bool IsEditModel { get; set; }

	PlotSetModel SetModel { get; set; }

	int Number { get; set; }

	bool IsOnAchor();

	bool IsValidate();

	void OnMouseDown(Point pt);

	void OnMouseMove(Point pt);

	void OnMouseUp(Point pt);

	void PlotElement(DrawingContext context);
}
