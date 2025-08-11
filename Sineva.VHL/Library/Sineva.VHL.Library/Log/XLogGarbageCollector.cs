/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team HJYOU
 * Issue Date	: 23.01.16
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.IO;
using System.Threading;

namespace Sineva.VHL.Library
{
    public class XLogGarbageCollector
    {
        public static readonly XLogGarbageCollector Instance = new XLogGarbageCollector();

        #region Fields
        private static AppConfig _Config = null;
        private FileCompressor _Compressor = FileCompressor.Instance;
        private ThreadFunction _LogManagerThread = new ThreadFunction();

        private FileInfo[] _TemporaryFiles = null;
        private DirectoryInfo[] _SubFolders = null;
        private DirectoryInfo _LogFolder = null;
        private DateTime _LastDateTime = DateTime.Now.AddHours(-1);

        public event Action<string> LogFileStreamClose;
        //2022.10.10 Program End ..by HS
        private bool _ThreadState = true;

        private bool m_Initialized = false;
        #endregion

        #region Properties
        #endregion

        #region Events
        #endregion

        #region Constructors
        private XLogGarbageCollector()
        {
        }
        #endregion

        #region Methods
        public bool Initialize()
        {
            if (m_Initialized) return true;
            try
            {
                bool result = true;
                if (_Config == null)
                {
                    _Config = AppConfig.Instance;
                }

                _LogFolder = new DirectoryInfo(_Config.LogPath.SelectedFolder);
                if (!_LogFolder.Exists)
                {
                    _LogFolder.Create();
                }

                _LogManagerThread.Start(LogManagerThread);

                m_Initialized = true;
            }
            catch
            {
                m_Initialized = false;
            }
            return m_Initialized;
        }
        private void CompressFile(string source, string compressFileName)
        {
            try
            {
                _Compressor.CompressFiles(source, compressFileName + ".zip");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        private void RecursiveDeleteFolder(DirectoryInfo directoryInfo)
        {
            string sExceptionInfo = string.Empty;
            string sExceptionLog = string.Empty;

            try
            {
                DirectoryInfo[] subDirInfo = directoryInfo.GetDirectories();

                if (subDirInfo.Length == 0)
                {
                    //1. 하위 Directory가 없으므로 File을 삭제한다.
                    FileInfo[] files = directoryInfo.GetFiles();

                    foreach (FileInfo file in files)
                    {
                        if (file.Exists) file.Delete();
                    }

                    //2. File을 모두 삭제 했으면 Directory를 삭제하고 빠져나간다.
                    directoryInfo.Delete(true);
                }
                else
                {
                    //1. Directory 하위의 File을 먼저 지운다.
                    FileInfo[] files = directoryInfo.GetFiles();

                    foreach (FileInfo file in files)
                    {
                        if (file.Exists) file.Delete();
                    }

                    //2. Sub-Directory를 지운다.
                    foreach (DirectoryInfo subDir in subDirInfo)
                    {
                        RecursiveDeleteFolder(subDir);   //하위 Directory를 지우기 위한 Recursive Call
                    }

                    //3. 자신을 지운다.
                    directoryInfo.Delete(true);
                }
            }
            catch
            {
            }
        }
        #endregion

        #region Methods - Thread Function
        private void LogManagerThread()
        {
            while (_ThreadState)
            {
                try
                {
                    DateTime now = DateTime.Now;

                    if (now.Date != _LastDateTime.Date || now.Hour != _LastDateTime.Hour)
                    {
                        _LastDateTime = now;
                        #region Delete temporary files
                        _TemporaryFiles = _LogFolder.GetFiles("*.tmp", SearchOption.AllDirectories);
                        if ((_TemporaryFiles != null) && (_TemporaryFiles.Length > 0))
                        {
                            foreach (FileInfo file in _TemporaryFiles)
                            {
                                try
                                {
                                    if (file.Exists)
                                    {
                                        file.Delete();
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                        #endregion

                        _SubFolders = _LogFolder.GetDirectories("????-??-??");

                        #region Compress Log Foder and Delete
                        for (int index = 0; index < _SubFolders.Length; index++)
                        {
                            DateTime logTime;
                            #region 1. Compress Log Folder
                            if (DateTime.TryParseExact(_SubFolders[index].Name, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out logTime))
                            {
                                if ((now.Date.Subtract(logTime.Date).TotalDays >= _Config.OriginalLogKeepingDays) && (File.Exists(_Config.LogPath.SelectedFolder + _SubFolders[index] + ".zip") == false))
                                {
                                    if (LogFileStreamClose != null)
                                    {
                                        LogFileStreamClose(_SubFolders[index].FullName);
                                    }

                                    CompressFile(_SubFolders[index].FullName, _SubFolders[index].FullName);
                                }
                            }
                            #endregion
                            #region 2. Delete Log Folder
                            if ((now.Date.Subtract(logTime.Date).TotalDays >= _Config.OriginalLogKeepingDays) && (Directory.Exists(_SubFolders[index].FullName) == true))
                            {
                                if (LogFileStreamClose != null)
                                {
                                    LogFileStreamClose(_SubFolders[index].FullName);
                                }

                                _SubFolders[index].Delete(true);
                            }
                            #endregion
                        }
                        #endregion
                        #region Delete compressed file, passed keeping days.
                        if (_LogFolder.Exists == true)
                        {
                            DateTime logTime;
                            FileInfo[] files = _LogFolder.GetFiles("????-??-??.zip");
                            foreach (FileInfo file in files)
                            {
                                if (DateTime.TryParseExact(file.Name.Substring(0, 10), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out logTime))
                                {
                                    if (now.Date.Subtract(logTime.Date).TotalDays >= _Config.CompressedLogKeepingDays)
                                    {
                                        file.Delete();
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
                catch
                {
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
        }

        //2022.10.10 Program End ..by HS
        public void Exit()
        {
            _ThreadState = false;
        }
        #endregion
    }
}
