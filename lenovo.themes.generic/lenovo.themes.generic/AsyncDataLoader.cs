using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using lenovo.themes.generic.Component;
using lenovo.themes.generic.Controls;
using lenovo.themes.generic.ControlsV6;

namespace lenovo.themes.generic;

public class AsyncDataLoader
{
	public static HostMaskLayerWrapper MaskLayer => new HostMaskLayerWrapper(global::Smart.HostOperationService);

	public static void Loading(Action action)
	{
		Window window = new LoadingWindow();
		action.BeginInvoke(delegate(IAsyncResult ac)
		{
			Action action2 = ac.AsyncState as Action;
			try
			{
				action2.EndInvoke(ac);
			}
			catch (Exception)
			{
			}
			finally
			{
				Application.Current.Dispatcher.Invoke(delegate
				{
					window.Close();
				});
			}
		}, action);
		MaskLayer.New(window, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
		{
			window.ShowDialog();
		});
	}

	public static void BeginLoading(Action action, ILoading view)
	{
		object handler = new object();
		if (action == null)
		{
			return;
		}
		action.BeginInvoke(delegate(IAsyncResult ar)
		{
			try
			{
				(ar.AsyncState as Action).EndInvoke(ar);
				Application.Current.Dispatcher.Invoke(delegate
				{
					view?.Hiden(handler);
				});
			}
			catch (Exception)
			{
			}
		}, action);
		view?.Show(handler);
	}

	public static void BeginLoading(Action<object> action, Action<object> callBack, ILoading view)
	{
		object handler = new object();
		object dataContainer = new ExpandoObject();
		if (action != null)
		{
			action.BeginInvoke(dataContainer, delegate(IAsyncResult ar)
			{
				(ar.AsyncState as Action<object>).EndInvoke(ar);
				view?.Hiden(handler);
				callBack?.Invoke(dataContainer);
			}, action);
			view?.Show(handler);
		}
	}

	public static void BeginLoading(Func<object, Tuple<bool, string, string>> action, Action<object> callBack, ILoading view)
	{
		object dataContainer = new ExpandoObject();
		if (action == null)
		{
			return;
		}
		object handler = new object();
		action.BeginInvoke(dataContainer, delegate(IAsyncResult ar)
		{
			Func<object, Tuple<bool, string, string>> func = ar.AsyncState as Func<object, Tuple<bool, string, string>>;
			view?.Hiden(handler);
			callBack?.Invoke(dataContainer);
			Tuple<bool, string, string> result = func.EndInvoke(ar);
			if (result != null && result.Item1)
			{
				Application.Current.Dispatcher.Invoke(delegate
				{
					LenovoPopupWindow win = new OkWindowModel().CreateWindow(global::Smart.Host.HostMainWindowHandle, result.Item2, result.Item3, "K0327", null);
					MaskLayer.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(delegate
					{
						win.ShowDialog();
					});
				});
			}
		}, action);
		view?.Show(handler);
	}

	public static void BeginLoading(Func<Tuple<bool, string, string>> action, ILoading view, Action<object> callback = null)
	{
		if (action == null)
		{
			return;
		}
		object handler = new object();
		action.BeginInvoke(delegate(IAsyncResult ar)
		{
			Func<Tuple<bool, string, string>> func = ar.AsyncState as Func<Tuple<bool, string, string>>;
			view?.Hiden(handler);
			Tuple<bool, string, string> result = func.EndInvoke(ar);
			if (result != null && result.Item1)
			{
				Application.Current.Dispatcher.Invoke(delegate
				{
					MessageBoxEx.Show(MaskLayer, result.Item3, MessageBoxButton.OK);
				});
			}
			callback?.Invoke(result);
		}, action);
		view?.Show(handler);
	}

	public static void BeginLoading(ILoading view, Action<object, Exception> callBack, params Func<object, Tuple<bool, Tuple<bool, string, string>>>[] actions)
	{
		object dataContainer = new ExpandoObject();
		if (actions == null && actions.Count() == 0)
		{
			return;
		}
		object handler = new object();
		Task.Factory.StartNew(delegate
		{
			Exception arg = null;
			try
			{
				Func<object, Tuple<bool, Tuple<bool, string, string>>>[] array = actions;
				foreach (Func<object, Tuple<bool, Tuple<bool, string, string>>> func in array)
				{
					Tuple<bool, Tuple<bool, string, string>> result = func?.Invoke(dataContainer);
					if (result == null)
					{
						break;
					}
					Application.Current.Dispatcher?.Invoke(delegate
					{
						if (result.Item1)
						{
							view?.Show(handler);
						}
						else
						{
							view?.Hiden(handler);
						}
						Tuple<bool, string, string> item = result.Item2;
						if (item != null && item.Item1)
						{
							LenovoPopupWindow win = new OkWindowModel().CreateWindow(global::Smart.Host.HostMainWindowHandle, item.Item2, item.Item3, "K0327", null);
							MaskLayer.New(win, closeMasklayerAfterWinClosed: true).ProcessWithMask(() => win.ShowDialog());
						}
					});
				}
			}
			catch (Exception ex)
			{
				arg = ex;
			}
			callBack?.Invoke(dataContainer, arg);
		});
	}

	public static void BeginLoading(params Func<bool>[] actions)
	{
		if (actions == null && actions.Count() == 0)
		{
			return;
		}
		new object();
		Task.Factory.StartNew(delegate
		{
			Func<bool>[] array = actions;
			for (int i = 0; i < array.Length; i++)
			{
				bool? flag = array[i]?.Invoke();
				if (!flag.HasValue || !flag.Value)
				{
					break;
				}
			}
		});
	}
}
