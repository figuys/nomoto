using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace lenovo.mbg.service.lmsa.Feedback.Model;

public class FeedBackNodeModel
{
	[JsonProperty("id")]
	public long? Id { get; set; }

	[JsonProperty("type")]
	public string Type { get; set; }

	[JsonProperty("date")]
	public DateTime Date { get; set; }

	[JsonProperty("title")]
	public string Title { get; set; }

	[JsonProperty("content")]
	public string Content { get; set; }

	[JsonProperty("helpful")]
	public int? HelpfulCode { get; set; }

	[JsonProperty("serverReplay")]
	public bool IsServerReplay { get; set; }

	[JsonProperty("tags")]
	public List<ContentItem> ContentItems { get; set; }

	[JsonProperty("children")]
	public List<FeedBackNodeModel> Children { get; set; }
}
