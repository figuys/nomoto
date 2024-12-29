using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.utilities;
using lenovo.mbg.service.common.webservices;
using lenovo.mbg.service.common.webservices.WebApiModel;
using lenovo.mbg.service.framework.services.Model;
using lenovo.mbg.service.lmsa.hostproxy;
using lenovo.themes.generic.ViewModelV6;

namespace lenovo.mbg.service.lmsa.ViewModels;

public class SurveyWindowV2ViewModel : lenovo.themes.generic.ViewModelV6.ViewModelBase
{
	protected Stopwatch sw;

	protected volatile bool LoadSuccess = false;

	private ObservableCollection<SurveyDataModel> _bindData;

	private string m_userId = string.Empty;

	private long locker = 0L;

	private string _SurveyTitle = string.Empty;

	private string _SurveySubTitle = string.Empty;

	private string _SelectedScore = "0";

	private string _VerifyErrorText;

	private Visibility _VerifyErrorVisibility = Visibility.Hidden;

	private ObservableCollection<SurveyDataModel> _QuestionList;

	private string _SubmitThanksText;

	private bool _QuestionVisibility;

	private bool _SubmitBtnEnable;

	public ReplayCommand SubmitClickCommand { get; set; }

	public ReplayCommand CloseWinCommand { get; set; }

	public ReplayCommand RadioBtnScoreCommand { get; set; }

	public string SurveyTitle
	{
		get
		{
			return _SurveyTitle;
		}
		set
		{
			if (!(_SurveyTitle == value))
			{
				_SurveyTitle = value;
				OnPropertyChanged("SurveyTitle");
			}
		}
	}

	public string SurveySubTitle
	{
		get
		{
			return _SurveySubTitle;
		}
		set
		{
			if (!(_SurveySubTitle == value))
			{
				_SurveySubTitle = value;
				OnPropertyChanged("SurveySubTitle");
			}
		}
	}

	public string SelectedScore
	{
		get
		{
			return _SelectedScore;
		}
		set
		{
			if (!(_SelectedScore == value))
			{
				_SelectedScore = value;
				RefreshData();
				OnPropertyChanged("SelectedScore");
			}
		}
	}

	public string VerifyErrorText
	{
		get
		{
			return _VerifyErrorText;
		}
		set
		{
			if (!(_VerifyErrorText == value))
			{
				_VerifyErrorText = value;
				OnPropertyChanged("VerifyErrorText");
			}
		}
	}

	public Visibility VerifyErrorVisibility
	{
		get
		{
			return _VerifyErrorVisibility;
		}
		set
		{
			if (_VerifyErrorVisibility != value)
			{
				_VerifyErrorVisibility = value;
				OnPropertyChanged("VerifyErrorVisibility");
			}
		}
	}

	public ObservableCollection<SurveyDataModel> QuestionList
	{
		get
		{
			return _QuestionList;
		}
		set
		{
			if (_QuestionList != value)
			{
				_QuestionList = value;
				OnPropertyChanged("QuestionList");
			}
		}
	}

	public string SubmitThanksText
	{
		get
		{
			return _SubmitThanksText;
		}
		set
		{
			if (!(_SubmitThanksText == value))
			{
				_SubmitThanksText = value;
				OnPropertyChanged("SubmitThanksText");
			}
		}
	}

	public bool QuestionVisibility
	{
		get
		{
			return _QuestionVisibility;
		}
		set
		{
			if (_QuestionVisibility != value)
			{
				_QuestionVisibility = value;
				OnPropertyChanged("QuestionVisibility");
			}
		}
	}

	public bool SubmitBtnEnable
	{
		get
		{
			return _SubmitBtnEnable;
		}
		set
		{
			if (_SubmitBtnEnable != value)
			{
				_SubmitBtnEnable = value;
				OnPropertyChanged("SubmitBtnEnable");
			}
		}
	}

	public SurveyWindowV2ViewModel(string _userId)
	{
		m_userId = _userId;
		sw = new Stopwatch();
		sw.Start();
		SubmitClickCommand = new ReplayCommand(_SubmitClickCommandCommandHandler);
		CloseWinCommand = new ReplayCommand(_CloseWinCommandCommandHandler);
		RadioBtnScoreCommand = new ReplayCommand(_RadioBtnScoreCommand);
		SurveyTitle = "K1019";
		SurveySubTitle = "K1020";
		QuestionVisibility = true;
		DefaultData();
		GetQuestionFromWebsite();
	}

	private void DefaultData()
	{
		SelectedScore = "0";
		_bindData = new ObservableCollection<SurveyDataModel>();
		ObservableCollection<QuestionItem> observableCollection = new ObservableCollection<QuestionItem>();
		observableCollection.Add(new QuestionItem(QuestionType.Sroce, "0"));
		observableCollection.Add(new QuestionItem(QuestionType.Sroce, "1"));
		observableCollection.Add(new QuestionItem(QuestionType.Sroce, "2"));
		observableCollection.Add(new QuestionItem(QuestionType.Sroce, "3"));
		observableCollection.Add(new QuestionItem(QuestionType.Sroce, "4"));
		observableCollection.Add(new QuestionItem(QuestionType.Sroce, "5"));
		observableCollection.Add(new QuestionItem(QuestionType.Sroce, "6"));
		observableCollection.Add(new QuestionItem(QuestionType.Sroce, "7"));
		observableCollection.Add(new QuestionItem(QuestionType.Sroce, "8"));
		observableCollection.Add(new QuestionItem(QuestionType.Sroce, "9"));
		observableCollection.Add(new QuestionItem(QuestionType.Sroce, "10"));
		_bindData.Add(new SurveyDataModel("1", "K1021", observableCollection, _required: true, "K1023")
		{
			IsSatisfied = Visibility.Visible
		});
		SubmitBtnEnable = true;
		QuestionList = _bindData;
	}

	private Task<SurveyResponseModel> GetQuestionFromWebsiteAsync()
	{
		return Task.Run(delegate
		{
			if (!LoadSuccess && Interlocked.Read(ref locker) == 0)
			{
				try
				{
					Interlocked.Exchange(ref locker, 1L);
					var parameter = new
					{
						type = "Quit"
					};
					ResponseModel<SurveyResponseModel> responseModel = AppContext.WebApi.Request<SurveyResponseModel>(WebApiUrl.SURVEY_GET_QUESTIONS, parameter);
					if (responseModel.code != "0000")
					{
						return (SurveyResponseModel)null;
					}
					LoadSuccess = true;
					return responseModel.content;
				}
				finally
				{
					Interlocked.Exchange(ref locker, 0L);
				}
			}
			return (SurveyResponseModel)null;
		});
	}

	private void GetQuestionFromWebsite()
	{
		GetQuestionFromWebsiteAsync().ContinueWith(delegate(Task<SurveyResponseModel> ar)
		{
			if (ar.Result != null)
			{
				SurveyResponseModel result = ar.Result;
				PostRefreshTrigger();
				SelectedScore = "0";
				SurveyTitle = result.lables.thankStr;
				SurveySubTitle = result.lables.pleaseStr;
				int num = -1;
				for (int i = 0; i < _bindData[0].QuestionItems.Count; i++)
				{
					if (_bindData[0].QuestionItems[i].IsSelected)
					{
						SelectedScore = _bindData[0].QuestionItems[i].ItemText;
						num = i;
						break;
					}
				}
				SurveyMsg surveyMsg = result.msg[0];
				ObservableCollection<SurveyDataModel> observableCollection = new ObservableCollection<SurveyDataModel>();
				ObservableCollection<QuestionItem> observableCollection2 = new ObservableCollection<QuestionItem>
				{
					new QuestionItem(QuestionType.Sroce, "0"),
					new QuestionItem(QuestionType.Sroce, "1"),
					new QuestionItem(QuestionType.Sroce, "2"),
					new QuestionItem(QuestionType.Sroce, "3"),
					new QuestionItem(QuestionType.Sroce, "4"),
					new QuestionItem(QuestionType.Sroce, "5"),
					new QuestionItem(QuestionType.Sroce, "6"),
					new QuestionItem(QuestionType.Sroce, "7"),
					new QuestionItem(QuestionType.Sroce, "8"),
					new QuestionItem(QuestionType.Sroce, "9"),
					new QuestionItem(QuestionType.Sroce, "10")
				};
				if (num > -1)
				{
					observableCollection2[num].IsSelected = true;
				}
				observableCollection.Add(new SurveyDataModel(surveyMsg.id, "1. " + surveyMsg.question, observableCollection2, _required: true, result.lables.rate)
				{
					IsSatisfied = Visibility.Visible
				});
				QuestionList = observableCollection;
				Application.Current.Dispatcher.Invoke(delegate
				{
					LoadOthers(ar.Result);
				});
			}
		});
	}

	private void LoadOthers(SurveyResponseModel data)
	{
		try
		{
			for (int i = 1; i < data.msg.Count; i++)
			{
				SurveyMsg surveyMsg = data.msg[i];
				QuestionType questionType = ConvertToQuestionType(surveyMsg.type);
				ObservableCollection<QuestionItem> observableCollection = new ObservableCollection<QuestionItem>();
				switch (questionType)
				{
				case QuestionType.MultipleChoice:
					foreach (SurveyNewOptions option in surveyMsg.options)
					{
						observableCollection.Add(new QuestionItem(questionType, option.id, option.option));
					}
					break;
				case QuestionType.SingleChoice:
				{
					string[] array = surveyMsg.content.Split(';');
					List<SurveyNewOptions> options = surveyMsg.options;
					int ix;
					for (ix = 0; ix < array.Length; ix++)
					{
						QuestionItem questionItem = new QuestionItem(questionType, array[ix])
						{
							GroupName = $"Question{i}"
						};
						if (options != null && options.Count > 0 && options.Exists((SurveyNewOptions n) => int.Parse(n.id) == ix))
						{
							questionItem.Width = 660.0;
							questionItem.Visibile = Visibility.Visible;
						}
						observableCollection.Add(questionItem);
					}
					break;
				}
				case QuestionType.Text:
					observableCollection.Add(new QuestionItem(questionType, surveyMsg.id, string.Empty));
					break;
				}
				QuestionList.Add(new SurveyDataModel(surveyMsg.id, $"{QuestionList.Count + 1}. {surveyMsg.question}", observableCollection));
			}
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"show more survey question exception:[{arg}]");
		}
	}

	private void RefreshData()
	{
		GetQuestionFromWebsiteAsync().ContinueWith(delegate(Task<SurveyResponseModel> ar)
		{
			if (ar.Result != null)
			{
				Application.Current.Dispatcher.Invoke(delegate
				{
					LoadOthers(ar.Result);
				});
			}
		});
	}

	private void PostRefreshTrigger()
	{
		Task.Factory.StartNew(delegate
		{
			var parameter = new
			{
				type = "Quit",
				clientUuid = GlobalFun.GetClientUUID()
			};
			AppContext.WebApi.RequestContent(WebApiUrl.SURVEY_REFRESH, parameter);
		});
	}

	private bool VerifyData()
	{
		VerifyErrorVisibility = Visibility.Hidden;
		foreach (SurveyDataModel question in QuestionList)
		{
			if (question.IsRequired != 0)
			{
				continue;
			}
			bool flag = false;
			foreach (QuestionItem questionItem in question.QuestionItems)
			{
				if (questionItem.IsSelected)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				VerifyErrorText = question.VerifyErrorText;
				VerifyErrorVisibility = Visibility.Visible;
				return flag;
			}
		}
		return true;
	}

	private void _SubmitClickCommandCommandHandler(object parameter)
	{
		try
		{
			if (!VerifyData())
			{
				return;
			}
			SurveySubmitModel _model = new SurveySubmitModel();
			_model.clientUuid = GlobalFun.GetClientUUID();
			_model.records = new List<SubmitItem>();
			foreach (SurveyDataModel question in QuestionList)
			{
				string text = string.Empty;
				foreach (QuestionItem questionItem in question.QuestionItems)
				{
					if (questionItem.QuestionType == "Text")
					{
						text = questionItem.ItemText;
					}
					else if (questionItem.IsSelected)
					{
						text = ((!string.IsNullOrEmpty(questionItem.Data)) ? (text + questionItem.Id + "(" + questionItem.Data + "),") : (text + questionItem.Id + ","));
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					text = text.TrimEnd(',');
					_model.records.Add(new SubmitItem(question.Id, text));
				}
			}
			SubmitBtnEnable = false;
			Task.Factory.StartNew(delegate
			{
				ResponseModel<object> responseModel = AppContext.WebApi.RequestBase(WebApiUrl.SURVEY_RECORD, _model);
				sw.Stop();
				HostProxy.BehaviorService.Collect(BusinessType.SURVEY_QUIT, new BusinessData(BusinessType.SURVEY_QUIT, null).Update(sw.ElapsedMilliseconds, BusinessStatus.SUCCESS, _model.records));
				QuestionVisibility = false;
				if (responseModel.code == "0000")
				{
					SubmitThanksText = responseModel.desc;
				}
				else
				{
					SubmitThanksText = "K1025";
				}
				Thread.Sleep(3000);
				AppContext.Single.CurrentDispatcher.Invoke(delegate
				{
					if (parameter is Window window)
					{
						window.Close();
					}
				});
			});
		}
		catch (Exception arg)
		{
			LogHelper.LogInstance.Error($"post survey result data. url:[{WebApiUrl.SURVEY_RECORD}] exception:[{arg}]");
		}
	}

	private void _CloseWinCommandCommandHandler(object parameter)
	{
		if (parameter is Window window)
		{
			window.Close();
		}
	}

	private void _RadioBtnScoreCommand(object parameter)
	{
		SelectedScore = parameter.ToString();
	}

	private QuestionType ConvertToQuestionType(string _type)
	{
		return _type switch
		{
			"rate" => QuestionType.Sroce, 
			"mt-select" => QuestionType.MultipleChoice, 
			"choose" => QuestionType.SingleChoice, 
			"text" => QuestionType.Text, 
			_ => QuestionType.SingleChoice, 
		};
	}
}
