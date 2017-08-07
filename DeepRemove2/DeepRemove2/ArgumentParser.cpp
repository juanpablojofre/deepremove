#include "stdafx.h"
#include "ArgumentParser.h"
#include <iostream>

bool parseArguments(int argc, wchar_t * argv[], std::wstring &folder, LogLevel &loglevel)
{
    bool res = false;
    if (argc == 2 || argc == 3)
    {
        folder = std::wstring(argv[1]);
        res = true;
    }

    if (argc == 3)
    {
        if (argv[2] == L"0")
        {
            loglevel = LogLevel::NoLog;
        }
        else if (argv[2] == L"2")
        {
            loglevel = LogLevel::WarningLog;

        }
        else if (argv[2] == L"3")
        {
            loglevel = LogLevel::TraceLog;

        }
        else if (argv[2] == L"4")
        {
            loglevel = LogLevel::VerboseLog;

        }
        else
        {
            loglevel = LogLevel::ErrorLog;
        }
    }

    return res;
}

void usage()
{
    std::wcout << L"Usage:" << std::endl;
    std::wcout << std::endl;
    std::wcout << L"DeepRemove2 <folder-path> [<log-level>]" << std::endl;
    std::wcout << std::endl;
    std::wcout << L"    <folder-path> : The full path to the folder to be removed." << std::endl;
    std::wcout << L"                    <drive>:\\<full-path>\\<folder-to-be-removed>" << std::endl;
    std::wcout << std::endl;
    std::wcout << L"    <log-level>   : 0 | 1 | 2 | 3 | 4" << std::endl;
    std::wcout << std::endl;
    std::wcout << L"                    0 = NoLog      : Silent execution, no log generated." << std::endl;
    std::wcout << L"                 ** 1 = ErrorLog   : Only error (exception) log generated." << std::endl;
    std::wcout << L"                                     This is the default value if none is provided or" << std::endl;
    std::wcout << L"                                     value is not a valid one." << std::endl;
    std::wcout << L"                    2 = WarningLog : Error and warning log generated." << std::endl;
    std::wcout << L"                    3 = TraceLog   : Error, warning and tracing log generated." << std::endl;
    std::wcout << L"                    4 = VerboseLog : All log messages generated." << std::endl;
    std::wcout << std::endl;
    std::wcout << L"Notes:" << std::endl;
    std::wcout << std::endl;
    std::wcout << L"*    For better results, execute with elevated privileges." << std::endl;
    std::wcout << std::endl;
    std::wcout << L"*    All logging goes to standard error stream." << std::endl;
}
