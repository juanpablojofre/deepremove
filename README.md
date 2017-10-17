# Deep Remove Folders and Directories in Windows

Welcome to the '**DeepRemove**' tool page!

The tool that removes folder or directory structures that are 
too deep to remove by traditional tools or shell commands in Windows.

## Little bit of history

I started this tool long time back in CodePlex to solve 
a recurrent problem in Windows. 
In Windows, some tools can create deeply nested folder structures,
with paths that exceed the 260 character max length (in the shell and UI);
once the path exceeds the limit, there were no tools 
to help you remove those folders.

Now that CodePlex is being retired (or already retired), 
I ported the tool to this place. And, also added a new version of the tool.

## [DeepRemove](DeepRemove/README.md) version 1

-  This is a Windows, window, UI application.
-  The tool was written in C# with calls into Kernel32.
-  The executable (installer) is [DeepRemove_1_0_26_58.zip](setup/DeepRemove_1_0_26_58.zip)
-  For the tool usage see [DeepRemove](DeepRemove/README.md)

## [DeepRemove2](DeepRemove2/README.md) version 2

-  This is a command line application.
-  The tool was written in C++ with calls into the standard library (STL) and "windows.h".
-  The executable (installer) is [DeepRemove2_1_0_3.zip](setup/DeepRemove2_1_0_3.zip)
-  Type 'DeepRemove2' at a console prompt to get help
   (assuming the tool folder is listed in your path).

## Developing and Contributing

At this moment I'm not monitoring this site as I should, 
so your contributions (either PRs or issues) might wait for a long time.


## Legal and Licensing

PowerShell is licensed under the [MIT license](LICENSE.md).

## Code of Conduct

This project has adopted the 
[Microsoft Open Source Code of Conduct](http://opensource.microsoft.com/codeofconduct/).

For more information see the [Code of Conduct FAQ](http://opensource.microsoft.com/codeofconduct/faq/)
or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
