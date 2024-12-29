using System;
using System.Xml.Serialization;

namespace lenovo.mbg.service.common.utilities;

[Serializable]
public class SysConfig
{
	[XmlElement(ElementName = "backupPath")]
	public string BackupPath { get; set; }

	[XmlElement(ElementName = "language")]
	public string Language { get; set; }

	[XmlElement(ElementName = "readyLang")]
	public string ReadyLang { get; set; }

	[XmlElement(ElementName = "gifSavePath")]
	public string GifSavePath { get; set; }
}
