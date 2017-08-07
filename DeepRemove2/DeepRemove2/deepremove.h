#pragma once

#ifndef DEEPREMOVE
#define DEEPREMOVE

#include "tracing.h"
#include <windows.h>
#include <string>


bool folderExists(std::wstring path);
std::wstring formatLongFolderSearch(std::wstring path);
std::wstring formatLongPath(std::wstring path);
bool deepRemove(std::wstring path, LogLevel loglevel);
bool deleteFile(std::wstring path, LogLevel loglevel);
bool deleteFolder(std::wstring path, LogLevel loglevel);
#endif // !DEEPREMOVE
