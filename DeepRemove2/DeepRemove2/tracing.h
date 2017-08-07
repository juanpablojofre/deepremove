#pragma once

#ifndef TRACING
#define TRACING

#include <windows.h>
#include <string>

enum LogLevel { NoLog, ErrorLog, WarningLog, TraceLog, VerboseLog };
void writeWclogErrorMsg(DWORD dwErr, std::wstring title);

#endif // !TRACING
