using System;
using System.Runtime.InteropServices;

namespace lenovo.mbg.service.lmsa.toolbox.Ringtone.Core;

[UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true, CharSet = CharSet.Ansi)]
internal delegate void delGenreCallback(int index, string genre, IntPtr cookie);
