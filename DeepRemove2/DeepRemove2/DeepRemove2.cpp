// DeepRemove2.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "ArgumentParser.h"
#include "tracing.h"
#include "deepremove.h"
#include <iostream>
#include <string>

int wmain(int argc, wchar_t* argv[])
{
    LogLevel loglevel = LogLevel::ErrorLog;
    std::wstring folder;
    int res(-1);

    if (parseArguments(argc, argv, folder, loglevel))
    {
        deepRemove(folder, loglevel);
        res = 0;
    }
    else
    {
        usage();
    }

    return res;
}
