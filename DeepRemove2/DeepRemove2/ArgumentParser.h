#pragma once

#ifndef ARGPARSER
#define ARGPARSER

#include <string>
#include "tracing.h"

bool parseArguments(int argc, wchar_t * argv[], std::wstring &folder, LogLevel &loglevel);
void usage();
#endif // !ARGPARSER
