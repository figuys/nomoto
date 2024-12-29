using System;
using System.Collections.Generic;

namespace lenovo.mbg.service.lmsa.Feedback.Model;

public class FeedbackLocalStoreModel
{
	public long UserId { get; set; }

	public DateTime SyncTimeline { get; set; }

	public List<FeedBackNodeModel> Feedbacks { get; set; }
}
