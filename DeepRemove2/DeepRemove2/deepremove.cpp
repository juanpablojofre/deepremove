#include "stdafx.h"
#include "tracing.h"
#include "deepremove.h"
#include "TakeFileOwnership.h"
#include <windows.h>
#include <queue>
#include <set>
#include <iostream>
#include <sstream>



std::wstring dotFolder(L".");
std::wstring doubledotFolder(L"..");

bool folderExists(std::wstring path)
{
	WIN32_FIND_DATA ffd;
	HANDLE hFind = INVALID_HANDLE_VALUE;

	hFind = FindFirstFileW(path.c_str(), &ffd);

	if (INVALID_HANDLE_VALUE == hFind)
	{
		return false;
	}

	return ffd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY;
}

void waitUntilFolderDisappears(std::wstring path, DWORD maxsleep = 128)
{
	DWORD sleeptime = 4; // milliseconds

	do
	{
		Sleep(sleeptime);
		sleeptime <<= 1;
	} while (folderExists(path) && sleeptime < maxsleep);
}

std::wstring formatLongFolderSearch(std::wstring path)
{
	return L"\\\\?\\" + std::wstring(path) + L"\\*";
}

std::wstring formatLongPath(std::wstring path)
{
	return L"\\\\?\\" + std::wstring(path);
}

std::wstring getNextVersion(std::wstring txt, unsigned long long &seq)
{
	std::wostringstream oss;
	oss << txt << ++seq;
	oss.flush();
	return oss.str();
}

bool deepRemove(std::wstring rootFolder, LogLevel loglevel)
{
	if (!folderExists(rootFolder)) return false;
	std::queue<std::wstring> folders;
	std::set<std::wstring> folderNames;
	WIN32_FIND_DATA ffd;
	HANDLE hFind = INVALID_HANDLE_VALUE;
	unsigned long long foldersequence(0);
	std::wstring movedFolderRoot(L"~~");
	std::wstring movedFolder(getNextVersion(movedFolderRoot, foldersequence));


	// Process Root Level Folder
	hFind = FindFirstFileW(formatLongFolderSearch(rootFolder).c_str(), &ffd);

	do
	{
		std::wstring currentFolderName(ffd.cFileName);
		std::wstring currentFile(rootFolder + L"\\" + currentFolderName);

        if (currentFolderName == dotFolder || currentFolderName == doubledotFolder) continue;
        
        TakeOwnership(formatLongPath(currentFile).c_str(), loglevel);

		if (ffd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
		{
			folders.push(currentFile);
			folderNames.insert(currentFolderName);
		}
		else
		{
			if ((loglevel  != LogLevel::NoLog) && (loglevel <= LogLevel::VerboseLog)) 
                std::wclog << L"Deleting '" << currentFile << "'" << std::endl;

			if (deleteFile(currentFile, loglevel)
				&& (loglevel != LogLevel::NoLog)
				&& (loglevel <= LogLevel::TraceLog)) std::wclog << L"File deleted '" << currentFile << "'" << std::endl;
		}

	} while (FindNextFileW(hFind, &ffd) != 0);

    FindClose(hFind);

	// Deep Remove
	while (!folders.empty())
	{
		hFind = FindFirstFileW(formatLongFolderSearch(folders.front()).c_str(), &ffd);

		do
		{
			std::wstring currentFolderName(ffd.cFileName);
			std::wstring currentFile(folders.front() + L"\\" + currentFolderName);

            if (currentFolderName == dotFolder || currentFolderName == doubledotFolder) continue;
            
            TakeOwnership(formatLongPath(currentFile).c_str(), loglevel);

			if (ffd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
			{

				while (folderNames.find(movedFolder) != folderNames.end()) movedFolder = getNextVersion(movedFolderRoot, foldersequence);

				std::wstring movedFolderPath(rootFolder + L"\\" + movedFolder);

				if (MoveFileW(currentFile.c_str(), movedFolderPath.c_str()))
				{
					folders.push(movedFolderPath);
					folderNames.insert(movedFolder);
				}
				else
				{
					if ((loglevel  != LogLevel::NoLog) && (loglevel <= LogLevel::ErrorLog)) 
                        writeWclogErrorMsg(GetLastError(), std::wstring(L"ERROR: MoveFileW(" + currentFile + L")"));
				}

			}
			else
			{
				if ((loglevel  != LogLevel::NoLog) && (loglevel <= LogLevel::VerboseLog)) 
                    std::wclog << L"Deleting file '" << currentFile << "'" << std::endl;

				if (deleteFile(currentFile, loglevel) 
                    && (loglevel != LogLevel::NoLog) 
                    && (loglevel <= LogLevel::TraceLog)) std::wclog << L"File deleted '" << currentFile << "'" << std::endl;
			}

		} while (FindNextFileW(hFind, &ffd) != 0);

        FindClose(hFind);

		if ((loglevel  != LogLevel::NoLog) && (loglevel <= LogLevel::VerboseLog)) 
            std::wclog << L"Deleting folder '" << folders.front() << "'" << std::endl;

		if (deleteFolder(folders.front(), loglevel))
		{
			if((loglevel != LogLevel::NoLog) && (loglevel <= LogLevel::TraceLog)) 
				std::wclog << L"Folder deleted '" << folders.front() << "'" << std::endl;

			waitUntilFolderDisappears(folders.front());
		}

		folders.pop();
	}

    if ((loglevel != LogLevel::NoLog) && (loglevel <= LogLevel::VerboseLog))
        std::wclog << L"Deleting folder '" << rootFolder << "'" << std::endl;

    if (deleteFolder(rootFolder, loglevel) 
        && (loglevel != LogLevel::NoLog)
        && (loglevel <= LogLevel::TraceLog)) std::wclog << L"Folder deleted '" << rootFolder << "'" << std::endl;

	return true;
}

bool deleteFile(std::wstring currentFile, LogLevel loglevel)
{
    if ((loglevel != LogLevel::NoLog) && (loglevel <= LogLevel::VerboseLog))
        std::wclog << L"Deleting '" << currentFile << "'" << std::endl;

	if (SetFileAttributesW(formatLongPath(currentFile).c_str(), FILE_ATTRIBUTE_NORMAL))
	{
		if (DeleteFileW(formatLongPath(currentFile).c_str())) return true;

		if ((loglevel  != LogLevel::NoLog) && (loglevel <= LogLevel::ErrorLog)) 
            writeWclogErrorMsg(GetLastError(), std::wstring(L"ERROR: DeleteFileW(" + currentFile + L")"));

		return false;
	}

    if ((loglevel != LogLevel::NoLog) && (loglevel <= LogLevel::ErrorLog))
        writeWclogErrorMsg(GetLastError(), std::wstring(L"ERROR: SetFileAttributesW(" + currentFile + L", FILE_ATTRIBUTE_NORMAL)"));

	return false;
}

bool deleteFolder(std::wstring currentFile, LogLevel loglevel)
{
    if ((loglevel != LogLevel::NoLog) && (loglevel <= LogLevel::VerboseLog))
        std::wclog << L"Deleting '" << currentFile << "'" << std::endl;

    if (SetFileAttributesW(formatLongPath(currentFile).c_str(), FILE_ATTRIBUTE_NORMAL))
	{
		if (RemoveDirectoryW(formatLongPath(currentFile).c_str())) return true;

        if ((loglevel != LogLevel::NoLog) && (loglevel <= LogLevel::ErrorLog))
            writeWclogErrorMsg(GetLastError(), std::wstring(L"ERROR: RemoveDirectoryW(" + currentFile + L")"));

        return false;
    }

    if ((loglevel != LogLevel::NoLog) && (loglevel <= LogLevel::ErrorLog))
        writeWclogErrorMsg(GetLastError(), std::wstring(L"ERROR: SetFileAttributesW(" + currentFile + L", FILE_ATTRIBUTE_NORMAL)"));

    return false;
}

