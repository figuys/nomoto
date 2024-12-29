using System;
using System.Runtime.InteropServices;

namespace lenovo.mbg.service.lmsa.toolbox.Ringtone.Core;

[UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true, CharSet = CharSet.Ansi)]
internal delegate void delReportFunction(string fmt, IntPtr args);
