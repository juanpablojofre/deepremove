#include "stdafx.h"

#include "tracing.h"
#include <windows.h>
#include <iostream>


void writeWclogErrorMsg(DWORD dwErr, std::wstring title)
{
    LPWSTR  lpMsgBuf = NULL;

    FormatMessage(
        FORMAT_MESSAGE_ALLOCATE_BUFFER |
        FORMAT_MESSAGE_FROM_SYSTEM |
        FORMAT_MESSAGE_IGNORE_INSERTS,
        NULL,
        dwErr,
        0,
        (LPWSTR)&lpMsgBuf,
        0, NULL);

    std::wstring errmsg(lpMsgBuf);
    std::wclog << L"ERROR: " << title << std::endl;
    std::wclog << L"       Failed with error: " << dwErr << std::endl;
    std::wclog << L"       Error message: " << errmsg << std::endl;
}