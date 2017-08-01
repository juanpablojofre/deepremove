using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace DeepRemove
{

    public partial class deepRemoveUIfrm : Form
    {
        private enum logLevels
        {
            Minimum,
            Medium,
            Verbose            
        }
        private const int MAX_PATH = 260;
        private const uint MAX_DEEP_PATH = 32767;

        private const uint FILE_ATTRIBUTE_READONLY            = 0x00000001;
        private const uint FILE_ATTRIBUTE_HIDDEN              = 0x00000002;
        private const uint FILE_ATTRIBUTE_SYSTEM              = 0x00000004;
        private const uint FILE_ATTRIBUTE_DIRECTORY           = 0x00000010;
        private const uint FILE_ATTRIBUTE_ARCHIVE             = 0x00000020;
        private const uint FILE_ATTRIBUTE_DEVICE              = 0x00000040;
        private const uint FILE_ATTRIBUTE_NORMAL              = 0x00000080;
        private const uint FILE_ATTRIBUTE_TEMPORARY           = 0x00000100;
        private const uint FILE_ATTRIBUTE_SPARSE_FILE         = 0x00000200;
        private const uint FILE_ATTRIBUTE_REPARSE_POINT       = 0x00000400;
        private const uint FILE_ATTRIBUTE_COMPRESSED          = 0x00000800;
        private const uint FILE_ATTRIBUTE_OFFLINE             = 0x00001000;
        private const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000;
        private const uint FILE_ATTRIBUTE_ENCRYPTED           = 0x00004000;

        private const int ERROR_SUCCESS = 0;
        private const int ERROR_FILE_NOT_FOUND = 2;
        private const int ERROR_NO_MORE_FILES = 18;
        private const int ERROR_FILENAME_EXCED_RANGE = 206;


        [StructLayout(LayoutKind.Sequential)]
        struct WIN32_FIND_FILETIME
        {
            public UInt32 dwLowDateTime;
            public UInt32 dwHighDateTime;
        }

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
        struct WIN32_FIND_DATAW
        {
            public uint dwFileAttributes;
            public WIN32_FIND_FILETIME ftCreationTime;
            public WIN32_FIND_FILETIME ftLastAccessTime;
            public WIN32_FIND_FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=MAX_PATH)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=14)]
            public string cAlternateFileName;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetCurrentDirectoryW([MarshalAs(UnmanagedType.LPWStr)]string lpPathName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern uint GetCurrentDirectoryW(uint nBufferLength,  StringBuilder lpBuffer);
                
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr FindFirstFileW([MarshalAs(UnmanagedType.LPWStr)]string lpFileName, out WIN32_FIND_DATAW lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FindNextFileW(IntPtr hFindFile, out WIN32_FIND_DATAW lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FindClose(IntPtr hFindFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetFileAttributesW(string lpFileName, uint dwFileAttributes);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool RemoveDirectoryW(string lpPathName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DeleteFileW([MarshalAs(UnmanagedType.LPWStr)]string lpFileName);

        Stack<string> foldersStack = new Stack<string>();
        int iFilesDeleted = 0;
        int iFoldersScanned = 0;
        int iFoldersRemoved = 0;
        logLevels logLevel = logLevels.Minimum;
        string logFileFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

        public deepRemoveUIfrm()
        {
            InitializeComponent();
            folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            folderBrowserDialog1.ShowNewFolderButton = false;
        }

        private void selectFolderRoot_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                rootFolder.Text = folderBrowserDialog1.SelectedPath;
                enableDeepRemove.CheckState = CheckState.Indeterminate;
            }
            else
            {
                rootFolder.Text = string.Empty;
                enableDeepRemove.CheckState = CheckState.Indeterminate;
            }
        }

        private void enableDeepRemove_CheckedChanged(object sender, EventArgs e)
        {
            bool checkedState = enableDeepRemove.CheckState == CheckState.Checked;
            bool rootFolderNotempty = rootFolder.Text != string.Empty;
            bool folderExists = Directory.Exists(rootFolder.Text);

            if (enableDeepRemove.CheckState == CheckState.Checked && rootFolder.Text != string.Empty && Directory.Exists(rootFolder.Text))
                doDeepRemove.Enabled = true;
            else
                doDeepRemove.Enabled = false;
        }

        private void enableDeepRemove_CheckStateChanged(object sender, EventArgs e)
        {
            if (enableDeepRemove.CheckState == CheckState.Checked && rootFolder.Text != string.Empty && Directory.Exists(rootFolder.Text))
                doDeepRemove.Enabled = true;
            else
                doDeepRemove.Enabled = false;
        }

        private void doDeepRemove_Click(object sender, EventArgs e)
        {
            iFilesDeleted = 0;
            iFoldersRemoved = 0;
            iFoldersScanned = 0;
            doDeepRemove.Enabled = false;
            enableDeepRemove.Enabled = false;
            selectFolderRoot.Enabled = false;
            logLevelGroup.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            filesDeleted.Text = iFilesDeleted.ToString("#,##0");
            foldersRemoved.Text = iFoldersRemoved.ToString("#,##0");
            foldersScanned.Text = iFoldersScanned.ToString("#,##0");
            try
            {
                deepRemoveDirectory(rootFolder.Text);
                MessageBox.Show("Deep Remove finished successfully!", "Deep Remove", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception eDeepRemove)
            {
                MessageBox.Show(eDeepRemove.Message, "ERROR: Deep Remove", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                enableDeepRemove.Enabled = true;
                enableDeepRemove.CheckState = CheckState.Indeterminate;
                selectFolderRoot.Enabled = true;
                logLevelGroup.Enabled = true;
                this.Cursor = Cursors.Arrow;
            }
        }
        private void deepRemoveDirectory(string targetDirectory)
        {
            
            TextWriter logFile = new StreamWriter(string.Format("{0}\\DeepRemove[{1:yyyy-MM-dd HHmmss ffff}].log", logFileFolder,DateTime.Now), false);
            int levelDeep = 0;
            foldersStack.Clear();

            WIN32_FIND_DATAW findFileData;
            IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
            
            string currentFolder = @"\\?\" + targetDirectory;
            foldersStack.Push(currentFolder);
            levelDeep++;
            while (foldersStack.Count > 0)
            {
                currentFolder = foldersStack.Pop();
                levelDeep = relativePathDepth(@"\\?\" + targetDirectory, currentFolder);
                if(logLevel==logLevels.Verbose)logFile.WriteLine("{0,6:#,##0} < {1} current Folder: {2}, path length: {3}", levelDeep, new string(' ', levelDeep + 1), currentFolder, currentFolder.Length);

                bool childFolderFound = false;

                IntPtr findHandle = FindFirstFileW(currentFolder + @"\*", out findFileData);
                #region handle any error in FindFirstFileW
                int findFirstFileError = Marshal.GetLastWin32Error();

                if ((findHandle == INVALID_HANDLE_VALUE) && (findFirstFileError != ERROR_FILE_NOT_FOUND))
                {
                    string win32ErrorMessage = new Win32Exception(findFirstFileError).Message;
                    logFile.WriteLine(string.Format("Failed to find first file at '{1}': error={0} {2}", findFirstFileError, currentFolder, win32ErrorMessage));
                    logFile.WriteLine("Files Deleted = {0:#,##0}", iFilesDeleted);
                    logFile.WriteLine("Folders Removed = {0:#,##0}", iFoldersRemoved);
                    logFile.Flush();
                    logFile.Close();
                    throw new SystemException(string.Format("Failed to find first file at '{1}': error={0}, {2}", findFirstFileError, currentFolder, win32ErrorMessage));
                }
                #endregion
                if (findFirstFileError != ERROR_FILE_NOT_FOUND)
                {
                    #region process children files or folders
                    do
                    {
                        string currentFilePath = currentFolder + @"\" + findFileData.cFileName;
                        if (logLevel == logLevels.Verbose) logFile.WriteLine("{0,6:#,##0} + {1}{2}{3} at {4}", levelDeep, new string(' ', levelDeep + 3), findFileData.cFileName, expandFileAttributes(findFileData.dwFileAttributes), currentFolder);
                        if (   ((findFileData.dwFileAttributes & FILE_ATTRIBUTE_READONLY) != 0)
                            || ((findFileData.dwFileAttributes & FILE_ATTRIBUTE_HIDDEN) != 0)
                            || ((findFileData.dwFileAttributes & FILE_ATTRIBUTE_SYSTEM) != 0) 
                            )
                        {
                            if (!SetFileAttributesW(currentFilePath, FILE_ATTRIBUTE_NORMAL))
                            {
                                #region Handle Failed remove ReadOnly/System/Hidden attributes
                                int lastError = Marshal.GetLastWin32Error();
                                string win32ErrorMessage = new Win32Exception(lastError).Message;

                                logFile.WriteLine(string.Format("Failed remove ReadOnly/System/Hidden attributes in '{1}' at '{2}': error={0}, {3}", lastError, findFileData.cFileName, currentFolder, win32ErrorMessage));
                                logFile.WriteLine("Files Deleted = {0:#,##0}", iFilesDeleted);
                                logFile.WriteLine("Folders Removed = {0:#,##0}", iFoldersRemoved);
                                logFile.Flush();
                                logFile.Close();

                                throw new SystemException(string.Format("Failed remove ReadOnly/System/Hidden attributes in '{1}' at '{2}': error={0}, {3}", lastError, findFileData.cFileName, currentFolder, win32ErrorMessage));
                                #endregion
                            }
                            if (logLevel == logLevels.Verbose) logFile.WriteLine("{0,6:#,##0} . {1} READONLY/SYSTEM/HIDDEN attributes removed", levelDeep, new string(' ', levelDeep + 6));
                        }

                        if ((findFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0)
                        {
                            if ((findFileData.cFileName != ".") && (findFileData.cFileName != ".."))
                            {
                                if (!childFolderFound)
                                {
                                    foldersStack.Push(currentFolder);
                                    childFolderFound = true;
                                }
                                foldersStack.Push(currentFolder + @"\" + findFileData.cFileName);
                                if (logLevel == logLevels.Verbose) logFile.WriteLine("{0,6:#,##0} > {1} folder pushed into stack", levelDeep, new string(' ', levelDeep + 6));
                                iFoldersScanned++;
                            }
                        }
                        else
                        {
                            if (!DeleteFileW(currentFilePath))
                            {
                                #region Handle Failed to delete file
                                int lastError = Marshal.GetLastWin32Error();
                                string win32ErrorMessage = new Win32Exception(lastError).Message;
                                logFile.WriteLine(string.Format("Failed to delete file '{1}': error={0}, {2}", lastError, currentFilePath, win32ErrorMessage));
                                logFile.WriteLine("Files Deleted = {0:#,##0}", iFilesDeleted);
                                logFile.WriteLine("Folders Removed = {0:#,##0}", iFoldersRemoved);
                                logFile.Flush();
                                logFile.Close();
                                throw new SystemException(string.Format("Failed to delete file '{1}': error={0}, {2}", lastError, currentFilePath, win32ErrorMessage));
                                #endregion
                            }
                            else
                            {
                                WIN32_FIND_DATAW deletedFileData;
                                int deletedFileRetries = 0;
                                int deletedFileError;
                                IntPtr ptrDeletedFile;
                                do
                                {
                                    if (deletedFileRetries > 0)
                                        System.Threading.Thread.Sleep(50 * deletedFileRetries);
                                    ptrDeletedFile = FindFirstFileW(currentFilePath, out deletedFileData);
                                    deletedFileError = Marshal.GetLastWin32Error();
                                    deletedFileRetries++;                                    
                                } while ((ptrDeletedFile != INVALID_HANDLE_VALUE) && (deletedFileError != ERROR_FILE_NOT_FOUND) && (deletedFileRetries < 3));

                                if (deletedFileRetries > 3)
                                {
                                    string win32ErrorMessage = new Win32Exception(findFirstFileError).Message;
                                    logFile.WriteLine(string.Format("Suposedly Deleted file still available  '{0}'", currentFilePath));
                                    logFile.Close();
                                    throw new SystemException(string.Format("Suposedly Deleted file still available  '{0}'", currentFilePath));
                                }
                            }
                            if (logLevel >= logLevels.Medium) logFile.WriteLine("{0,6:#,##0} - {1} file deleted", levelDeep, new string(' ', levelDeep + 6));
                            iFilesDeleted++;
                        }

                    } while (FindNextFileW(findHandle, out findFileData));
                    #endregion
                    #region handle any error with children files or folders
                    int findNextFileError = Marshal.GetLastWin32Error();
                    int findCloseError = ERROR_SUCCESS; // No error, yet! :-)
                    if (!FindClose(findHandle))
                        findCloseError = Marshal.GetLastWin32Error();

                    if (findNextFileError != ERROR_NO_MORE_FILES || findCloseError != ERROR_SUCCESS)
                    {
                        StringBuilder errors = new StringBuilder();
                        if (findNextFileError != ERROR_NO_MORE_FILES)
                            errors.AppendLine("Find Next File error: " + findNextFileError.ToString());
                        if (findCloseError != ERROR_SUCCESS)
                            errors.AppendLine("Find Close Handle exited with error: " + findCloseError.ToString() + " for file: " + findFileData.cFileName);

                        logFile.WriteLine(errors.ToString());
                        logFile.WriteLine("Files Deleted = {0:#,##0}", iFilesDeleted);
                        logFile.WriteLine("Folders Removed = {0:#,##0}", iFoldersRemoved);
                        logFile.Flush();
                        logFile.Close();
                        throw new SystemException(errors.ToString());
                    }
                    #endregion
                }
                else
                {
                    if (logLevel == logLevels.Verbose) logFile.WriteLine("{0,6:#,##0} * {1} current folder '{2}' has no children", levelDeep, new string(' ', levelDeep + 3), currentFolder);

                }
                if (!childFolderFound)
                {
                    if (logLevel == logLevels.Verbose) logFile.WriteLine("{0,6:#,##0} ? {1} Starting to remove current folder", levelDeep, new string(' ', levelDeep + 4));
                    if (!RemoveDirectoryW(currentFolder))
                    {
                        #region Handle Failed to remove current directory
                        int lastError = Marshal.GetLastWin32Error();
                        string win32ErrorMessage = new Win32Exception(lastError).Message;
                        logFile.WriteLine(string.Format("Failed to remove current directory '{1}': error={0}, {2}", lastError, currentFolder, win32ErrorMessage));
                        logFile.WriteLine("Files Deleted = {0:#,##0}", iFilesDeleted);
                        logFile.WriteLine("Folders Removed = {0:#,##0}", iFoldersRemoved);
                        logFile.Flush();
                        logFile.Close();
                        throw new SystemException(string.Format("Failed to remove current directory '{1}': error={0}", lastError, currentFolder));
                        #endregion
                    }
                    else
                    {
                        WIN32_FIND_DATAW removedFolderData;
                        int removedFolderRetries = 0;
                        int removedFolderError;
                        IntPtr ptrRemovedFolder;
                        do
                        {
                            if (removedFolderRetries > 0)
                                System.Threading.Thread.Sleep(50 * removedFolderRetries);
                            ptrRemovedFolder = FindFirstFileW(currentFolder, out removedFolderData);
                            removedFolderError = Marshal.GetLastWin32Error();
                            removedFolderRetries++;
                        } while ((ptrRemovedFolder != INVALID_HANDLE_VALUE) && (removedFolderError != ERROR_FILE_NOT_FOUND) && (removedFolderRetries < 3));

                        if (removedFolderRetries > 3)
                        {
                            string win32ErrorMessage = new Win32Exception(findFirstFileError).Message;
                            logFile.WriteLine(string.Format("Suposedly Removed Folder still available  '{0}'", currentFolder));
                            logFile.WriteLine("Files Deleted = {0:#,##0}", iFilesDeleted);
                            logFile.WriteLine("Folders Removed = {0:#,##0}", iFoldersRemoved);
                            logFile.Flush();
                            logFile.Close();
                            throw new SystemException(string.Format("Suposedly Removed Folder still available  '{0}'", currentFolder));
                        }
                    }

                    iFoldersRemoved++;
                    if (logLevel >= logLevels.Medium) logFile.WriteLine("{0,6:#,##0} ! {1} folder '{2}' removed", levelDeep, new string(' ', levelDeep + 3), currentFolder);
                }
                filesDeleted.Text = iFilesDeleted.ToString("#,##0");
                foldersRemoved.Text = iFoldersRemoved.ToString("#,##0");
                foldersScanned.Text = iFoldersScanned.ToString("#,##0");
                Application.DoEvents();
            }
            logFile.WriteLine("Files Deleted = {0:#,##0}", iFilesDeleted);
            logFile.WriteLine("Folders Removed = {0:#,##0}", iFoldersRemoved );
            logFile.WriteLine("Successful end");
            logFile.Flush();
            logFile.Close();
        }

        private string getCurrentDirectory()
        {
            StringBuilder nameBuffer = new StringBuilder((int)MAX_DEEP_PATH + 3);
            uint folderNameLength = GetCurrentDirectoryW(MAX_DEEP_PATH, nameBuffer);
            if (folderNameLength == 0)
            {
                int lastError = Marshal.GetLastWin32Error();
                string win32ErrorMessage = new Win32Exception(lastError).Message;


                throw new SystemException("Failed to get initial working directory; error = '" + lastError.ToString() + "', " + win32ErrorMessage);                
            }
            if (folderNameLength > MAX_DEEP_PATH)
            {
                throw new SystemException("Failed to get initial working directory; allocated buffer is shorter than required: '" + MAX_DEEP_PATH.ToString() + "'<'" + folderNameLength.ToString() + "'");                
            }
            return nameBuffer.ToString();
        }

        private string expandFileAttributes(uint dwFileAttributes)
        {
            StringBuilder strFileAttributes = new StringBuilder();
            if ((dwFileAttributes & FILE_ATTRIBUTE_READONLY) != 0) strFileAttributes.Append("| READONLY");
            if ((dwFileAttributes & FILE_ATTRIBUTE_HIDDEN) != 0) strFileAttributes.Append("| HIDDEN");
            if ((dwFileAttributes & FILE_ATTRIBUTE_SYSTEM) != 0) strFileAttributes.Append("| SYSTEM");
            if ((dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0) strFileAttributes.Append("| DIRECTORY");
            if ((dwFileAttributes & FILE_ATTRIBUTE_ARCHIVE) != 0) strFileAttributes.Append("| ARCHIVE");
            if ((dwFileAttributes & FILE_ATTRIBUTE_DEVICE) != 0) strFileAttributes.Append("| DEVICE");
            if ((dwFileAttributes & FILE_ATTRIBUTE_NORMAL) != 0) strFileAttributes.Append("| NORMAL");
            if ((dwFileAttributes & FILE_ATTRIBUTE_TEMPORARY) != 0) strFileAttributes.Append("| TEMPORARY");
            if ((dwFileAttributes & FILE_ATTRIBUTE_SPARSE_FILE) != 0) strFileAttributes.Append("| SPARSE_FILE");
            if ((dwFileAttributes & FILE_ATTRIBUTE_REPARSE_POINT) != 0) strFileAttributes.Append("| REPARSE_POINT");
            if ((dwFileAttributes & FILE_ATTRIBUTE_COMPRESSED) != 0) strFileAttributes.Append("| COMPRESSED");
            if ((dwFileAttributes & FILE_ATTRIBUTE_OFFLINE) != 0) strFileAttributes.Append("| OFFLINE");
            if ((dwFileAttributes & FILE_ATTRIBUTE_NOT_CONTENT_INDEXED) != 0) strFileAttributes.Append("| NOT_CONTENT_INDEXED");
            if ((dwFileAttributes & FILE_ATTRIBUTE_ENCRYPTED) != 0) strFileAttributes.Append("| ENCRYPTED");
            return strFileAttributes.ToString();
        }

        private int relativePathDepth(string rootPath, string actualPath)
        {
            if ((rootPath.Length >= actualPath.Length) || (actualPath.IndexOf(rootPath, 0) != 0))
                return 0;
            char dirSeparator = Path.DirectorySeparatorChar;
            int relativeDepth = 0;
            int currentPosition = rootPath.Length;

            while ((currentPosition = actualPath.IndexOf(dirSeparator, currentPosition)+1) > 0)
                relativeDepth++;


            return relativeDepth;
        }

        private void loglevelMinimum_CheckedChanged(object sender, EventArgs e)
        {
            if (loglevelMinimum.Checked)
                logLevel = logLevels.Minimum;
        }

        private void loglevelMedium_CheckedChanged(object sender, EventArgs e)
        {
            if (loglevelMedium.Checked)
                logLevel = logLevels.Medium;
        }

        private void loglevelMaximum_CheckedChanged(object sender, EventArgs e)
        {
            if (loglevelMaximum.Checked)
                logLevel = logLevels.Verbose;
        }

        private void logFileAtUserDocs_CheckedChanged(object sender, EventArgs e)
        {
            if(logFileAtUserDocs.Checked)
                logFileFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        }

        private void logFileAtRootFolder_CheckedChanged(object sender, EventArgs e)
        {
            if (logFileAtRootFolder.Checked)
                logFileFolder = Path.GetPathRoot(logFileFolder);
        }
    }
}
