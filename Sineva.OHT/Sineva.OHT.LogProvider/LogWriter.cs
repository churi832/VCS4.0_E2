using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Sineva.OHT.LogProvider
{
    public class LogWriter
    {
        #region Fields
        private string _LogName = string.Empty;
        private string _LogPath = string.Empty;
        private string _SubPath = string.Empty;
        private int _LogSplitType = 0;
        private int _LogMaxLine = 1000000;
        private FileStream _LogFileStream = null;
        private StreamWriter _LogStreamWriter = null;
        private string _OldStreamName = string.Empty;
        private string _LogDate = string.Empty;
        private string _LogDateFolder = string.Empty;
        private Queue<string> _LogQueue = new Queue<string>();

        private Thread _LogWriteThread;
        private bool _ThreadState = true;

        private bool _DateFolderExist = false;
        private int _WriteCount = 0;
        private int _FileCount = 0;
        private int _RecordLine = 0;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public LogWriter(string logPath, string logName)
        {
            try
            {
                _LogPath = logPath;
                _LogName = logName;

                _FileCount = 1;

                _LogWriteThread = new Thread(new ThreadStart(LogWriteThread));
                _LogWriteThread.IsBackground = true;
                _LogWriteThread.Start();

                LogManager.Instance.LogFileStreamClose += this.CloseLogFileStreamByFolderName;
            }
            catch
            {
            }
        }
        public LogWriter(string logPath, int logSplitType, int logMaxLines, string logName)
        {
            try
            {
                _LogPath = logPath;
                _LogName = logName;
                _LogSplitType = logSplitType;
                _LogMaxLine = logMaxLines;

                _LogWriteThread = new Thread(new ThreadStart(LogWriteThread));
                _LogWriteThread.IsBackground = true;
                _LogWriteThread.Start();

                if (logSplitType == 2)
                {
                    string dateFolderName = string.Empty;// string.Format("{0}\\{1}", _LogPath, DateTime.Now.ToString("yyyyMMdd"));
                    if (_SubPath == string.Empty)
                    {
                        dateFolderName = string.Format("{0}\\{1}", _LogPath, DateTime.Now.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        dateFolderName = string.Format("{0}\\{1}\\{2}", _LogPath, DateTime.Now.ToString("yyyy-MM-dd"), _SubPath);
                    }

                    DirectoryInfo dateFolder = new DirectoryInfo(dateFolderName);

                    if (dateFolder.Exists == true)
                    {
                        FileInfo[] files = dateFolder.GetFiles();
                        foreach (FileInfo file in files)
                        {
                            if (file.Name.Contains(logName) == false) continue;
                            if (file.Name.Contains(".log") == false) continue;

                            string fileName = file.Name.Split('.')[0];
                            string[] splitName = fileName.Split('_');
                            string fileCountText = splitName[splitName.Length - 1];

                            if (fileCountText.Contains("H") == true) continue;

                            if (_FileCount < Convert.ToInt32(fileCountText))
                            {
                                _FileCount = Convert.ToInt32(fileCountText);
                            }
                        }

                        _FileCount += 1;

                        _OldStreamName = GetFileStreamName(dateFolderName, DateTime.Now);
                        CreateFileStream(_OldStreamName);
                    }
                }

                LogManager.Instance.LogFileStreamClose += this.CloseLogFileStreamByFolderName;
            }
            catch
            {
            }
        }
        public LogWriter(string logPath, string subPath, int logSplitType, int logMaxLines, string logName)
        {
            try
            {
                _LogPath = logPath;
                _SubPath = subPath;
                _LogName = logName;
                _LogSplitType = logSplitType;
                _LogMaxLine = logMaxLines;

                _LogWriteThread = new Thread(new ThreadStart(LogWriteThread));
                _LogWriteThread.IsBackground = true;
                _LogWriteThread.Start();

                if (logSplitType == 2)
                {
                    string dateFolderName = string.Empty;// string.Format("{0}\\{1}", _LogPath, DateTime.Now.ToString("yyyyMMdd"));
                    if (_SubPath == string.Empty)
                    {
                        dateFolderName = string.Format("{0}\\{1}", _LogPath, DateTime.Now.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        dateFolderName = string.Format("{0}\\{1}\\{2}", _LogPath, DateTime.Now.ToString("yyyy-MM-dd"), _SubPath);
                    }
                    DirectoryInfo dateFolder = new DirectoryInfo(dateFolderName);

                    if (dateFolder.Exists == true)
                    {
                        FileInfo[] files = dateFolder.GetFiles();
                        foreach (FileInfo file in files)
                        {
                            if (file.Name.Contains(logName) == false) continue;
                            if (file.Name.Contains(".log") == false) continue;

                            string fileName = file.Name.Split('.')[0];
                            string[] splitName = fileName.Split('_');
                            string fileCountText = splitName[splitName.Length - 1];

                            if (fileCountText.Contains("H") == true) continue;

                            if (_FileCount < Convert.ToInt32(fileCountText))
                            {
                                _FileCount = Convert.ToInt32(fileCountText);
                            }
                        }

                        _FileCount += 1;

                        _OldStreamName = GetFileStreamName(dateFolderName, DateTime.Now);
                        CreateFileStream(_OldStreamName);
                    }
                }

                LogManager.Instance.LogFileStreamClose += this.CloseLogFileStreamByFolderName;
            }
            catch
            {
            }
        }
        #endregion

        #region Public Methods
        public void SetLog(string logText)
        {
            try
            {
                lock (_LogQueue)
                {
                    string formattedText = string.Format("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), logText);
                    _LogQueue.Enqueue(formattedText);
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void SetLog(string[] logText)
        {
            try
            {
                foreach (string text in logText)
                {
                    SetLog(text);
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void Exit()
        {
            _ThreadState = false;
        }
        #endregion

        #region Private Methods
        private void CloseLogFileStreamByFolderName(string folderName)
        {
            try
            {

            }
            catch
            {
            }
        }
        private void CreateFileStream(string streamName)
        {
            try
            {
                CloseFileStream();

                _OldStreamName = streamName;

                _LogFileStream = new FileStream(streamName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                _LogStreamWriter = new StreamWriter(_LogFileStream);
            }
            catch
            {
            }
        }
        private string GetFileStreamName(string folderName, DateTime createTime)
        {
            try
            {
                string result = string.Empty;

                if (_LogSplitType == 0)
                {
                    result = string.Format("{0}\\{1}.log", folderName, _LogName);
                }
                else if (_LogSplitType == 1)
                {
                    result = string.Format("{0}\\{1}_{2}H.log", folderName, _LogName, createTime.ToString("HH"));
                }
                else if (_LogSplitType == 2)
                {
                    result = string.Format("{0}\\{1}_{2}.log", folderName, _LogName, _FileCount);
                }

                return result;
            }
            catch
            {
                return "Error";
            }
        }
        private void CloseFileStream()
        {
            try
            {
                if (_LogStreamWriter != null) _LogStreamWriter.Close();
                if (_LogFileStream != null) _LogFileStream.Close();
            }
            catch
            {
            }
        }
        private void WriteLog(string logText)
        {
            try
            {
                string fileName = string.Format("\\{0}.Log", _LogName);
                string dateFolder = string.Empty;
                if (_SubPath == string.Empty)
                {
                    dateFolder = string.Format("{0}\\{1}", _LogPath, DateTime.Now.ToString("yyyy-MM-dd"));
                }
                else
                {
                    dateFolder = string.Format("{0}\\{1}\\{2}", _LogPath, DateTime.Now.ToString("yyyy-MM-dd"), _SubPath);
                }

                string currentStreamName = dateFolder + fileName;

                if ((_DateFolderExist == false) || (dateFolder != _LogDateFolder))
                {
                    if (Directory.Exists(dateFolder) == false)
                    {
                        Directory.CreateDirectory(dateFolder);
                    }
                    _DateFolderExist = true;
                }

                currentStreamName = GetFileStreamName(dateFolder, DateTime.Now);

                if (_LogSplitType == 2)
                {
                    if ((_RecordLine >= _LogMaxLine) && (_LogSplitType == 2))
                    {
                        _RecordLine = 0;
                        _FileCount += 1;
                        currentStreamName = GetFileStreamName(dateFolder, DateTime.Now);
                    }
                    else if (_OldStreamName != currentStreamName)
                    {
                        _RecordLine = 0;
                        _FileCount = 1;
                        currentStreamName = GetFileStreamName(dateFolder, DateTime.Now);
                    }
                }

                _LogDateFolder = dateFolder;

                if (_OldStreamName != currentStreamName)
                {
                    CreateFileStream(currentStreamName);
                }

                _LogFileStream.Seek(_LogFileStream.Length, SeekOrigin.Begin);
                _LogStreamWriter.WriteLine(logText);
                _LogStreamWriter.Flush();

                _RecordLine++;
            }
            catch
            {
            }
        }
        #endregion

        #region Thread
        private void LogWriteThread()
        {
            string temp = string.Empty;
            while (_ThreadState)
            {
                try
                {
                    lock (_LogQueue)
                    {
                        while ((_LogQueue.Count > 0) && (_WriteCount < 10000))
                        {
                            temp = _LogQueue.Dequeue();
                            if (string.IsNullOrWhiteSpace(temp) == false)
                            {
                                _WriteCount += 1;
                                WriteLog(temp);
                            }
                        }
                    }
                    _WriteCount = 0;
                }
                catch
                {
                }
                finally
                {
                    Thread.Sleep(100);
                }
            }
        }
        #endregion
    }
}
