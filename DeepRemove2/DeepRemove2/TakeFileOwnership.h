#pragma once

#ifndef TakeFileOwnership
#define TakeFileOwnership
#include <windows.h>
#include <string>


BOOL SetPrivilege(
    HANDLE hToken,          // access token handle
    LPCWSTR lpszPrivilege,  // name of privilege to enable/disable
    BOOL bEnablePrivilege,  // to enable or disable privilege
    LogLevel loglevel       // to enable tracing/log
);

BOOL TakeOwnership(LPCWSTR lpszOwnFile, LogLevel loglevel);
#endif // !TakeFileOwnership
