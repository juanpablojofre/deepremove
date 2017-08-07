/*
	Taking Object Ownership in C++
    ==============================

	https://msdn.microsoft.com/en-us/library/windows/desktop/aa379620(v=vs.85).aspx

	The following example tries to change the DACL of a file object by taking ownership of that object. 
	This will succeed only if the caller has WRITE_DAC access to the object or is the owner of the object. 
	If the initial attempt to change the DACL fails, an administrator can take ownership of the object. 
	To give the administrator ownership, the example enables the SE_TAKE_OWNERSHIP_NAME privilege in the 
	caller's access token, and makes the local system's Administrators group the owner of the object. 
	If the caller is a member of the Administrators group, the code will then be able to change the object's DACL.

    Enabling and Disabling Privileges in C++
    ========================================

    https://msdn.microsoft.com/en-us/library/windows/desktop/aa446619(v=vs.85).aspx

    The following example shows how to enable or disable a privilege in an access token. 
    The example calls the LookupPrivilegeValue function to get the locally unique identifier (LUID) 
    that the local system uses to identify the privilege. 
    Then the example calls the AdjustTokenPrivileges function, which either enables or disables 
    the privilege that depends on the value of the bEnablePrivilege parameter.

*/


#include "stdafx.h"

#include "tracing.h"
#include "TakeFileOwnership.h"
#include <stdio.h>
#include <accctrl.h>
#include <aclapi.h>

#include <iostream>
#include <string>

BOOL SetPrivilege(
	HANDLE hToken,          // access token handle
	LPCWSTR lpszPrivilege,  // name of privilege to enable/disable
	BOOL bEnablePrivilege,  // to enable or disable privilege
    LogLevel loglevel       // to enable tracing/log
)
{
	TOKEN_PRIVILEGES tp;
	LUID luid;
    DWORD dwError;

	if (!LookupPrivilegeValueW(
		NULL,            // lookup privilege on local system
		lpszPrivilege,   // privilege to lookup 
		&luid))          // receives LUID of privilege
	{
        if ((loglevel  != LogLevel::NoLog) && (loglevel <= LogLevel::ErrorLog)) 
            writeWclogErrorMsg(GetLastError(), std::wstring(L"ERROR: LookupPrivilegeValue()"));
        return FALSE;
	}

	tp.PrivilegeCount = 1;
	tp.Privileges[0].Luid = luid;
	if (bEnablePrivilege)
		tp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
	else
		tp.Privileges[0].Attributes = 0;

	// Enable the privilege or disable all privileges.

	if (!AdjustTokenPrivileges(
		hToken,
		FALSE,
		&tp,
		sizeof(TOKEN_PRIVILEGES),
		(PTOKEN_PRIVILEGES)NULL,
		(PDWORD)NULL))
	{
        if ((loglevel  != LogLevel::NoLog) && (loglevel <= LogLevel::ErrorLog)) 
            writeWclogErrorMsg(GetLastError(), std::wstring(L"ERROR: AdjustTokenPrivileges()"));
        return FALSE;
	}

	if (dwError = GetLastError() == ERROR_NOT_ALL_ASSIGNED)

	{
        if ((loglevel  != LogLevel::NoLog) && (loglevel <= LogLevel::ErrorLog)) 
            writeWclogErrorMsg(GetLastError(), std::wstring(L"ERROR: The token does not have the specified privilege."));
        return FALSE;
	}

	return TRUE;
}

BOOL TakeOwnership(
    LPCWSTR lpszOwnFile,
    LogLevel loglevel       // to enable tracing/log
)
{
	std::wstring ownFile(lpszOwnFile);

	BOOL bRetval = FALSE;

	HANDLE hToken = NULL;
	PSID pSIDAdmin = NULL;
	PSID pSIDEveryone = NULL;
	PACL pACL = NULL;
	SID_IDENTIFIER_AUTHORITY SIDAuthWorld = SECURITY_WORLD_SID_AUTHORITY;
	SID_IDENTIFIER_AUTHORITY SIDAuthNT = SECURITY_NT_AUTHORITY;

	const int NUM_ACES = 2;
	EXPLICIT_ACCESS ea[NUM_ACES];
	DWORD dwRes;
    DWORD dwError;

    if ((loglevel != LogLevel::NoLog) && (loglevel <= LogLevel::TraceLog))
        std::wclog << L"Taking ownership of '" << ownFile << "'" << std::endl;

	// Specify the DACL to use.
	// Create a SID for the Everyone group.
	if (!AllocateAndInitializeSid(&SIDAuthWorld, 1,
		SECURITY_WORLD_RID,
		0,
		0, 0, 0, 0, 0, 0,
		&pSIDEveryone))
	{
        if ((loglevel  != LogLevel::NoLog) && (loglevel <= LogLevel::ErrorLog)) 
            writeWclogErrorMsg(GetLastError(), std::wstring(L"ERROR: AllocateAndInitializeSid(Everyone)."));
        goto Cleanup;
	}

	// Create a SID for the BUILTIN\Administrators group.
	if (!AllocateAndInitializeSid(&SIDAuthNT, 2,
		SECURITY_BUILTIN_DOMAIN_RID,
		DOMAIN_ALIAS_RID_ADMINS,
		0, 0, 0, 0, 0, 0,
		&pSIDAdmin))
	{
        if ((loglevel  != LogLevel::NoLog) && (loglevel <= LogLevel::ErrorLog)) 
            writeWclogErrorMsg(GetLastError(), std::wstring(L"ERROR: AllocateAndInitializeSid(Admin)."));
        goto Cleanup;
	}

	ZeroMemory(&ea, NUM_ACES * sizeof(EXPLICIT_ACCESS));

	// Set read access for Everyone.
	ea[0].grfAccessPermissions = GENERIC_ALL;
	ea[0].grfAccessMode = SET_ACCESS;
	ea[0].grfInheritance = NO_INHERITANCE;
	ea[0].Trustee.TrusteeForm = TRUSTEE_IS_SID;
	ea[0].Trustee.TrusteeType = TRUSTEE_IS_WELL_KNOWN_GROUP;
	ea[0].Trustee.ptstrName = (LPTSTR)pSIDEveryone;

	// Set full control for Administrators.
	ea[1].grfAccessPermissions = GENERIC_ALL;
	ea[1].grfAccessMode = SET_ACCESS;
	ea[1].grfInheritance = NO_INHERITANCE;
	ea[1].Trustee.TrusteeForm = TRUSTEE_IS_SID;
	ea[1].Trustee.TrusteeType = TRUSTEE_IS_GROUP;
	ea[1].Trustee.ptstrName = (LPTSTR)pSIDAdmin;

	if (ERROR_SUCCESS != (dwError = SetEntriesInAcl(NUM_ACES,
		ea,
		NULL,
		&pACL)))
	{
        if ((loglevel  != LogLevel::NoLog) && (loglevel <= LogLevel::ErrorLog)) 
            writeWclogErrorMsg(dwError, std::wstring(L"ERROR: SetEntriesInAcl."));

        goto Cleanup;
	}

	// Try to modify the object's DACL.
	dwRes = SetNamedSecurityInfoW(
		&ownFile[0],                 // name of the object
		SE_FILE_OBJECT,              // type of object
		DACL_SECURITY_INFORMATION,   // change only the object's DACL
		NULL, NULL,                  // do not change owner or group
		pACL,                        // DACL specified
		NULL);                       // do not change SACL

	if (ERROR_SUCCESS == dwRes)
	{
        if ((loglevel != LogLevel::NoLog) && (loglevel <= LogLevel::TraceLog))
            std::wclog << L"Successfully changed DACL for '" << ownFile << "'" << std::endl;

        bRetval = TRUE;
		// No more processing needed.
		goto Cleanup;
	}

	if (dwRes != ERROR_ACCESS_DENIED)
	{
        if ((loglevel  != LogLevel::NoLog) && (loglevel <= LogLevel::ErrorLog)) 
            writeWclogErrorMsg(dwRes, std::wstring(L"ERROR: SetNamedSecurityInfoW."));

        goto Cleanup;
	}

	// If the preceding call failed because access was denied, 
	// enable the SE_TAKE_OWNERSHIP_NAME privilege, create a SID for 
	// the Administrators group, take ownership of the object, and 
	// disable the privilege. Then try again to set the object's DACL.

	// Open a handle to the access token for the calling process.
	if (!OpenProcessToken(GetCurrentProcess(),
		TOKEN_ADJUST_PRIVILEGES,
		&hToken))
	{
        if ((loglevel  != LogLevel::NoLog) && (loglevel <= LogLevel::ErrorLog)) 
            writeWclogErrorMsg(GetLastError(), std::wstring(L"ERROR: OpenProcessToken."));

        goto Cleanup;
	}

	// Enable the SE_TAKE_OWNERSHIP_NAME privilege.
	if (!SetPrivilege(hToken, SE_TAKE_OWNERSHIP_NAME, TRUE, loglevel))
	{
        if ((loglevel  != LogLevel::NoLog) && (loglevel <= LogLevel::ErrorLog)) 
            writeWclogErrorMsg(GetLastError(), std::wstring(L"ERROR: SetPrivilege() <-- You must be logged on as Administrator."));

        goto Cleanup;
	}

	// Set the owner in the object's security descriptor.
	dwRes = SetNamedSecurityInfoW(
		&ownFile[0],                 // name of the object
		SE_FILE_OBJECT,              // type of object
		OWNER_SECURITY_INFORMATION,  // change only the object's owner
		pSIDAdmin,                   // SID of Administrator group
		NULL,
		NULL,
		NULL);

	if (dwRes != ERROR_SUCCESS)
	{
        if ((loglevel  != LogLevel::NoLog) && (loglevel <= LogLevel::ErrorLog)) 
            writeWclogErrorMsg(dwRes, std::wstring(L"ERROR: SetNamedSecurityInfoW() <-- Could not set owner."));

        goto Cleanup;
	}

	// Disable the SE_TAKE_OWNERSHIP_NAME privilege.
	if (!SetPrivilege(hToken, SE_TAKE_OWNERSHIP_NAME, FALSE, loglevel))
	{
        if ((loglevel  != LogLevel::NoLog) && (loglevel <= LogLevel::ErrorLog)) 
            writeWclogErrorMsg(GetLastError(), std::wstring(L"ERROR: Failed SetPrivilege call unexpectedly."));

        goto Cleanup;
	}

	// Try again to modify the object's DACL,
	// now that we are the owner.
	dwRes = SetNamedSecurityInfoW(
		&ownFile[0],                 // name of the object
		SE_FILE_OBJECT,              // type of object
		DACL_SECURITY_INFORMATION,   // change only the object's DACL
		NULL, NULL,                  // do not change owner or group
		pACL,                        // DACL specified
		NULL);                       // do not change SACL

	if (dwRes == ERROR_SUCCESS)
	{
        if ((loglevel != LogLevel::NoLog) && (loglevel <= LogLevel::TraceLog))
            std::wclog << L"Successfully changed DACL for '" << ownFile << "'" << std::endl;

        bRetval = TRUE;
	}
	else
	{
        if ((loglevel  != LogLevel::NoLog) && (loglevel <= LogLevel::ErrorLog)) 
            writeWclogErrorMsg(dwRes, std::wstring(L"ERROR: Second SetNamedSecurityInfo call failed."));
    }

Cleanup:

	if (pSIDAdmin)
		FreeSid(pSIDAdmin);

	if (pSIDEveryone)
		FreeSid(pSIDEveryone);

	if (pACL)
		LocalFree(pACL);

	if (hToken)
		CloseHandle(hToken);

	return bRetval;

}
