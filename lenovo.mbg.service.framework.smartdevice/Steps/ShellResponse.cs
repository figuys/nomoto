using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using lenovo.mbg.service.common.utilities;

namespace lenovo.mbg.service.framework.smartdevice.Steps;

public class ShellResponse
{
	public enum ShellCmdStatus
	{
		None = 0,
		Connecting = 1,
		Connected = 2,
		Downloading = 3,
		Completed = 4,
		Outputing = 5,
		Authenticating = 6,
		Writing = 7,
		RomUnMatchError = 8,
		AuthorizedError = 9,
		FastbootError = 10,
		FileLostError = 11,
		FastbootDegrade = 12,
		ConditionQuit = 13,
		Error = -1
	}

	public enum ShellCmdType
	{
		None,
		MTekFlashTool,
		MTekSpFlashTool,
		MTekCfcFlashTool,
		QComFlashTool,
		QFileTool,
		PnPTool,
		CmdDloader,
		QFileSaharaTool,
		CmdDloaderTablet
	}

	private static Dictionary<string, ShellCmdType> ExeStringToShellCmdType = new Dictionary<string, ShellCmdType>
	{
		{
			"flash_tool",
			ShellCmdType.MTekFlashTool
		},
		{
			"spflashtool",
			ShellCmdType.MTekSpFlashTool
		},
		{
			"qdowloader",
			ShellCmdType.QComFlashTool
		},
		{
			"qcomdloader",
			ShellCmdType.QComFlashTool
		},
		{
			"qfil",
			ShellCmdType.QFileTool
		},
		{
			"upgrade_tool",
			ShellCmdType.PnPTool
		},
		{
			"cfc_flash",
			ShellCmdType.MTekCfcFlashTool
		},
		{
			"update_flash",
			ShellCmdType.MTekCfcFlashTool
		},
		{
			"Qsaharaserver",
			ShellCmdType.QFileSaharaTool
		},
		{
			"fh_loader",
			ShellCmdType.QFileSaharaTool
		},
		{
			"CmdDloader",
			ShellCmdType.CmdDloader
		}
	};

	private static Dictionary<string, ShellCmdType> ExeStringToShellCmdType_Tablet = new Dictionary<string, ShellCmdType>
	{
		{
			"flash_tool",
			ShellCmdType.MTekFlashTool
		},
		{
			"spflashtool",
			ShellCmdType.MTekSpFlashTool
		},
		{
			"qdowloader",
			ShellCmdType.QComFlashTool
		},
		{
			"qcomdloader",
			ShellCmdType.QComFlashTool
		},
		{
			"qfil",
			ShellCmdType.QFileTool
		},
		{
			"upgrade_tool",
			ShellCmdType.PnPTool
		},
		{
			"cfc_flash",
			ShellCmdType.MTekCfcFlashTool
		},
		{
			"update_flash",
			ShellCmdType.MTekCfcFlashTool
		},
		{
			"Qsaharaserver",
			ShellCmdType.QFileSaharaTool
		},
		{
			"fh_loader",
			ShellCmdType.QFileSaharaTool
		},
		{
			"CmdDloader",
			ShellCmdType.CmdDloaderTablet
		}
	};

	private static Dictionary<string, ShellCmdStatus> MTekResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"BROM connected",
			ShellCmdStatus.Connected
		},
		{
			"connect brom successed",
			ShellCmdStatus.Connected
		},
		{
			"% of image data has been sent",
			ShellCmdStatus.Downloading
		},
		{
			"WRITE TO PARTITION",
			ShellCmdStatus.Downloading
		},
		{
			"All command exec done!",
			ShellCmdStatus.Completed
		},
		{
			"STATUS_DOWNLOAD_EXCEPTION",
			ShellCmdStatus.Error
		},
		{
			"SearchUSBPortPool failed!",
			ShellCmdStatus.Error
		},
		{
			"Failed to find USB port",
			ShellCmdStatus.Error
		},
		{
			"Connect BROM failed: S_CHIP_TYPE_NOT_MATCH",
			ShellCmdStatus.RomUnMatchError
		},
		{
			"Connect BROM failed: STATUS_SCATTER_HW_CHIP_ID_MISMATCH",
			ShellCmdStatus.RomUnMatchError
		},
		{
			"Connect BROM failed: S_BROM_CMD_STARTCMD_FAIL",
			ShellCmdStatus.Error
		},
		{
			"Connect BROM failed: S_TIMEOUT",
			ShellCmdStatus.Error
		},
		{
			"Connect BROM failed: STATUS_BROM_CMD_FAIL",
			ShellCmdStatus.Error
		},
		{
			"[BROM] Can not pass bootrom start command! Possibly target power up too early.",
			ShellCmdStatus.Error
		},
		{
			"Failed to Connect DA: S_FT_ENABLE_DRAM_FAIL",
			ShellCmdStatus.Error
		},
		{
			"[EMI] Enable DRAM Failed!",
			ShellCmdStatus.Error
		},
		{
			"Please check your load matches to your target which is to be downloaded.",
			ShellCmdStatus.Error
		},
		{
			"[DA] DA binary file contains an unsupported version in its header! Please ask for help.",
			ShellCmdStatus.Error
		},
		{
			"Failed to Connect DA: S_UNSUPPORTED_VER_OF_DA",
			ShellCmdStatus.Error
		},
		{
			"Failed to Connect DA: STATUS_DEVICE_CTRL_EXCEPTION",
			ShellCmdStatus.Error
		},
		{
			"Chip mismatch",
			ShellCmdStatus.RomUnMatchError
		},
		{
			"STATUS_SEC_DL_FORBIDDEN",
			ShellCmdStatus.AuthorizedError
		},
		{
			"Exception: err_code",
			ShellCmdStatus.Error
		},
		{
			"lib DA NOT match",
			ShellCmdStatus.RomUnMatchError
		},
		{
			"Download failed",
			ShellCmdStatus.Error
		},
		{
			"S_FT_DOWNLOAD_FAIL ",
			ShellCmdStatus.Error
		},
		{
			"Invalid parameter.",
			ShellCmdStatus.Error
		},
		{
			"connect brom failed",
			ShellCmdStatus.Error
		}
	};

	private static Dictionary<string, ShellCmdStatus> QComResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Status=status_flash_dut_connected",
			ShellCmdStatus.Connected
		},
		{
			"Status=status_flash_download_percent_",
			ShellCmdStatus.Downloading
		},
		{
			"Status=status_flash_download_end",
			ShellCmdStatus.Completed
		},
		{
			"Status=status_flash_download_failed",
			ShellCmdStatus.Error
		}
	};

	private static Dictionary<string, ShellCmdStatus> QFileResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Start Download",
			ShellCmdStatus.Connected
		},
		{
			"{percent files transferred",
			ShellCmdStatus.Downloading
		},
		{
			"Download Succeed",
			ShellCmdStatus.Completed
		},
		{
			"Download Fail",
			ShellCmdStatus.Error
		}
	};

	private static Dictionary<string, ShellCmdStatus> QFileSaharaResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Start Download",
			ShellCmdStatus.Connected
		},
		{
			"{percent files transferred",
			ShellCmdStatus.Downloading
		},
		{
			"Sahara protocol completed",
			ShellCmdStatus.Completed
		},
		{
			"File transferred successfully",
			ShellCmdStatus.Outputing
		},
		{
			"Download Succeed",
			ShellCmdStatus.Completed
		},
		{
			"Download Fail",
			ShellCmdStatus.Error
		},
		{
			"All Finished Successfully",
			ShellCmdStatus.Completed
		},
		{
			"There is a chance your target is in SAHARA mode!!",
			ShellCmdStatus.Error
		},
		{
			"ERROR: XML not formed correctly",
			ShellCmdStatus.Error
		}
	};

	private static Dictionary<string, ShellCmdStatus> PnpResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Start to upgrade firmware",
			ShellCmdStatus.Connected
		},
		{
			"Download Image...",
			ShellCmdStatus.Downloading
		},
		{
			"Upgrade firmware ok",
			ShellCmdStatus.Completed
		},
		{
			"Download Firmware Fail",
			ShellCmdStatus.Error
		}
	};

	private static Dictionary<string, ShellCmdStatus> CmdDloaderResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Connecting",
			ShellCmdStatus.Connected
		},
		{
			"Downloading...",
			ShellCmdStatus.Downloading
		},
		{
			"DownLoad Passed",
			ShellCmdStatus.Completed
		},
		{
			"DownLoad Failed",
			ShellCmdStatus.Error
		},
		{
			"login http Get Fail!",
			ShellCmdStatus.AuthorizedError
		},
		{
			"Not find valid download devices",
			ShellCmdStatus.Error
		}
	};

	private static Dictionary<string, ShellCmdStatus> CmdDloaderTabletResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"Connecting",
			ShellCmdStatus.Connected
		},
		{
			"Downloading...",
			ShellCmdStatus.Downloading
		},
		{
			"DownLoad Passed",
			ShellCmdStatus.Completed
		},
		{
			"DownLoad Failed",
			ShellCmdStatus.Error
		},
		{
			"login http Get Fail!",
			ShellCmdStatus.AuthorizedError
		},
		{
			"Not find valid download devices",
			ShellCmdStatus.Error
		},
		{
			"Loading firmware",
			ShellCmdStatus.Connected
		},
		{
			"Download Image Total\\(",
			ShellCmdStatus.Downloading
		},
		{
			"Upgrade firmware ok",
			ShellCmdStatus.Completed
		},
		{
			"Download Firmware Fail",
			ShellCmdStatus.Error
		},
		{
			"Load PAC file successfully",
			ShellCmdStatus.Connected
		}
	};

	private static Dictionary<string, ShellCmdStatus> MTekCfcResponseToStatus = new Dictionary<string, ShellCmdStatus>
	{
		{
			"all download successful!!!",
			ShellCmdStatus.Completed
		},
		{
			"SearchUSBPortPool failed!",
			ShellCmdStatus.Error
		},
		{
			"Failed to find USB port",
			ShellCmdStatus.Error
		},
		{
			"Connect BROM failed: S_CHIP_TYPE_NOT_MATCH",
			ShellCmdStatus.RomUnMatchError
		},
		{
			"Connect BROM failed: STATUS_SCATTER_HW_CHIP_ID_MISMATCH",
			ShellCmdStatus.RomUnMatchError
		},
		{
			"Connect BROM failed: S_BROM_CMD_STARTCMD_FAIL",
			ShellCmdStatus.Error
		},
		{
			"Connect BROM failed: S_TIMEOUT",
			ShellCmdStatus.Error
		},
		{
			"Connect BROM failed: STATUS_BROM_CMD_FAIL",
			ShellCmdStatus.Error
		},
		{
			"[BROM] Can not pass bootrom start command! Possibly target power up too early.",
			ShellCmdStatus.Error
		},
		{
			"Failed to Connect DA: S_FT_ENABLE_DRAM_FAIL",
			ShellCmdStatus.Error
		},
		{
			"[EMI] Enable DRAM Failed!",
			ShellCmdStatus.Error
		},
		{
			"Please check your load matches to your target which is to be downloaded.",
			ShellCmdStatus.Error
		},
		{
			"[DA] DA binary file contains an unsupported version in its header! Please ask for help.",
			ShellCmdStatus.Error
		},
		{
			"Failed to Connect DA: S_UNSUPPORTED_VER_OF_DA",
			ShellCmdStatus.Error
		},
		{
			"Failed to Connect DA: STATUS_DEVICE_CTRL_EXCEPTION",
			ShellCmdStatus.Error
		},
		{
			"Chip mismatch",
			ShellCmdStatus.RomUnMatchError
		},
		{
			"STATUS_SEC_DL_FORBIDDEN",
			ShellCmdStatus.AuthorizedError
		},
		{
			"fastboot: error",
			ShellCmdStatus.FastbootError
		}
	};

	private static Dictionary<ShellCmdType, Dictionary<string, ShellCmdStatus>> ShellCmdToResponse = new Dictionary<ShellCmdType, Dictionary<string, ShellCmdStatus>>
	{
		{
			ShellCmdType.MTekFlashTool,
			MTekResponseToStatus
		},
		{
			ShellCmdType.MTekSpFlashTool,
			MTekResponseToStatus
		},
		{
			ShellCmdType.QComFlashTool,
			QComResponseToStatus
		},
		{
			ShellCmdType.QFileTool,
			QFileResponseToStatus
		},
		{
			ShellCmdType.PnPTool,
			PnpResponseToStatus
		},
		{
			ShellCmdType.CmdDloader,
			CmdDloaderResponseToStatus
		},
		{
			ShellCmdType.MTekCfcFlashTool,
			MTekCfcResponseToStatus
		},
		{
			ShellCmdType.QFileSaharaTool,
			QFileSaharaResponseToStatus
		},
		{
			ShellCmdType.CmdDloaderTablet,
			CmdDloaderTabletResponseToStatus
		}
	};

	private ShellCmdType mShellCmd;

	protected int MTKFileCount = 1;

	private double mPercentage;

	public ShellCmdType ShellCmd => mShellCmd;

	public ShellResponse(string exe, object data)
	{
		string text = Path.GetFileName(exe).ToLower();
		mShellCmd = ShellCmdType.CmdDloader;
		foreach (string key in ExeStringToShellCmdType.Keys)
		{
			if (text.StartsWith(key.ToLower()))
			{
				mShellCmd = ExeStringToShellCmdType[key];
				break;
			}
		}
		Init(data);
	}

	public ShellResponse(string exe, object data, string category)
	{
		string text = Path.GetFileName(exe).ToLower();
		mShellCmd = ShellCmdType.CmdDloader;
		if (category.Equals("Tablet", StringComparison.OrdinalIgnoreCase))
		{
			foreach (string key in ExeStringToShellCmdType_Tablet.Keys)
			{
				if (text.StartsWith(key.ToLower()))
				{
					mShellCmd = ExeStringToShellCmdType_Tablet[key];
					break;
				}
			}
		}
		else
		{
			foreach (string key2 in ExeStringToShellCmdType.Keys)
			{
				if (text.StartsWith(key2.ToLower()))
				{
					mShellCmd = ExeStringToShellCmdType[key2];
					break;
				}
			}
		}
		Init(data);
	}

	public ShellCmdStatus ParseResponse(string response, out string responseKey)
	{
		ShellCmdStatus result = ShellCmdStatus.None;
		responseKey = string.Empty;
		foreach (string key in ShellCmdToResponse[mShellCmd].Keys)
		{
			if (Regex.IsMatch(response, key, RegexOptions.IgnoreCase))
			{
				responseKey = key;
				result = ShellCmdToResponse[mShellCmd][key];
			}
		}
		return result;
	}

	public void CleanUp()
	{
		GlobalFun.TryDeleteFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Qualcomm\\QFIL\\QFIL.config"));
	}

	public double GetDownloadProgressPercent(string response, string key)
	{
		string s = "0";
		double result = 0.0;
		if (mShellCmd == ShellCmdType.QFileTool)
		{
			string pattern = $"(?<key>.+?{key})\\s+?(?<value>[\\d\\.]+)";
			s = Regex.Match(response, pattern, RegexOptions.IgnoreCase).Groups["value"].Value;
		}
		else if (mShellCmd == ShellCmdType.CmdDloader)
		{
			mPercentage += 3.3333333333333335;
			s = $"{mPercentage:0.00}";
		}
		else if (mShellCmd == ShellCmdType.QComFlashTool)
		{
			s = response.Substring(key.Length);
		}
		else if (mShellCmd == ShellCmdType.PnPTool)
		{
			s = Regex.Match(response, "\\((?<value>[\\d\\.]+)", RegexOptions.IgnoreCase).Groups["value"].Value;
		}
		else if (mShellCmd == ShellCmdType.MTekFlashTool || mShellCmd == ShellCmdType.MTekSpFlashTool)
		{
			if (key == "WRITE TO PARTITION")
			{
				mPercentage += 100.0 / (double)MTKFileCount;
				s = $"{mPercentage:0.00}";
			}
			else if (key == "% of image data has been sent")
			{
				s = response.Substring(0, response.IndexOf('%'));
				mPercentage = double.Parse(s);
			}
			else
			{
				s = mPercentage.ToString();
			}
		}
		if (double.TryParse(s, out var result2))
		{
			result = ((100.0 - result2 < 0.02) ? 100.0 : result2);
		}
		return result;
	}

	public void WriteInput(string message, string serialNumber, string logId, string clientReqType, string prodId, string keyType, string keyName, string inputFileName, Process process)
	{
	}

	private void Init(object data)
	{
		if (data == null || (ShellCmd != ShellCmdType.MTekFlashTool && ShellCmd != ShellCmdType.MTekSpFlashTool))
		{
			return;
		}
		string text = (data as List<object>).FirstOrDefault((object n) => n.ToString().EndsWith("xml"))?.ToString();
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		Dictionary<string, long> dictionary = new Dictionary<string, long>();
		foreach (Match item in Regex.Matches(File.ReadAllText(text), "<rom\\s+?index=\"(?<key>\\d+)\"\\s+?.*>(?<value>.+)</rom>", RegexOptions.Multiline))
		{
			dictionary.Add(item.Groups["key"].Value, new FileInfo(item.Groups["value"].Value).Length);
		}
		MTKFileCount = dictionary.Count;
	}
}
