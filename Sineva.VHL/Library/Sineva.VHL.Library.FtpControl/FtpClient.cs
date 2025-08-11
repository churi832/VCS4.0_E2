using System.Diagnostics;
using System.Data;
using System.Collections;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace Sineva.VHL.Library.FtpControl
{
    public struct FtpUploadFileInfo
    {
        public string LocalPath;
        public string RemotePath;
    }

    public class FtpClient
    {
        #region Field
        private string m_HostName;
        private string m_UserName;
        private string m_Password;
        private string m_FolderPath;

        private Queue<FtpUploadFileInfo> m_QueueUploadFilePath = new Queue<FtpUploadFileInfo>();
        private Thread m_ThreadFileUpload = null;
        private static object m_LockKey = new object();
        #endregion

        #region Property
        #endregion

        #region Constructor
        public readonly static FtpClient Instance = new FtpClient();
        private FtpClient()
        {
            m_HostName = AppConfig.Instance.FtpServerAddress;
            m_UserName = AppConfig.Instance.FtpUserName;
            m_Password = AppConfig.Instance.FtpPassword;
            m_FolderPath = AppConfig.Instance.FtpFolderPath;
        }
        public FtpClient GetConsole(string serverip, string root, string userid, string password)
        {
            FtpClient instance = new FtpClient();
            instance.m_HostName = serverip;
            instance.m_FolderPath = root;
            instance.m_UserName = userid;
            instance.m_Password = password;
            return instance;
        }
        #endregion

        #region Method
        public void PutUploadFile(FtpUploadFileInfo info)
        {
            m_QueueUploadFilePath.Enqueue(info);

            //m_ThreadFileUpload = new Thread(new ThreadStart(CallbackFileUpload));
            //m_ThreadFileUpload.IsBackground = true;
            //m_ThreadFileUpload.Start();
            
            Thread uploadThread = new Thread(new ThreadStart(CallbackFileUpload));
            uploadThread.IsBackground = true;
            uploadThread.Start();
        }
        public void PutFile(FtpUploadFileInfo info)
        {
            try
            {
                if(File.Exists(info.LocalPath))
                {
                    FileInfo file = new FileInfo(info.LocalPath);
                    Upload(file, info.RemotePath);
                }
            }
            catch
            {
            }
        }
        private void CallbackFileUpload()
        {
            lock(m_LockKey)
            {
                try
                {
                    if(m_QueueUploadFilePath.Count > 0)
                    {
                        FtpUploadFileInfo fileInfo = m_QueueUploadFilePath.Dequeue();

                        if(File.Exists(fileInfo.LocalPath))
                        {
                            FileInfo file = new FileInfo(fileInfo.LocalPath);
                            Upload(file, fileInfo.RemotePath);
                        }
                    }
                }
                catch(Exception ex)
                {
                    ExceptionLog.WriteLog(string.Format("{0}\t{1}", this.ToString(), ex.Message));
                }
                finally
                {
                    //m_ThreadFileUpload.Abort();
                }
            }
        }
        #endregion

        public List<string> ListDirectory(string directory)
        {
            System.Net.FtpWebRequest ftp = GetRequest(GetDirectory(directory));
            ftp.Method = System.Net.WebRequestMethods.Ftp.ListDirectory;

            string str = GetStringResponse(ftp);
            str = str.Replace("\r\n", "\r").TrimEnd('\r');

            List<string> result = new List<string>();
            result.AddRange(str.Split('\r'));
            return result;
        }

        private FtpDirectory ListDirectoryDetail(string directory)
        {
            System.Net.FtpWebRequest ftp = GetRequest(GetDirectory(directory));
            ftp.Method = System.Net.WebRequestMethods.Ftp.ListDirectoryDetails;

            string str = GetStringResponse(ftp);
            str = str.Replace("\r\n", "\r").TrimEnd('\r');
            return new FtpDirectory(str, _lastDirectory);
        }

        private void CreateFilePutPath(string path)
        {
            path = path.Replace("\\", "/");
            string[] subPath = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string curPath = string.Empty;
            for(int i = 0; i < subPath.Length; i++)
            {
                FtpDirectory dir = ListDirectoryDetail(curPath);
                curPath += "/" + subPath[i];

                bool pathExist = false;
                for(int j = 0; j < dir.Count; j++)
                {
                    if(dir[j].FileType == FtpFileInfo.DirectoryEntryTypes.Directory && dir[j].Filename == subPath[i])
                    {
                        pathExist = true;
                        break;
                    }
                }

                if(!pathExist)
                {
                    FtpCreateDirectory(curPath);
                }
            }
        }

        private bool Upload(FileInfo sourceFile, string uploadRemotePath)
        {
            uploadRemotePath = uploadRemotePath.Replace(" ", "_");
            uploadRemotePath = uploadRemotePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            string rootPath = m_FolderPath + Path.AltDirectorySeparatorChar;
            string fileName = Path.GetFileNameWithoutExtension(sourceFile.Name);
            string extension = Path.GetExtension(sourceFile.Name);
            string path = rootPath + uploadRemotePath;
            CreateFilePutPath(path);

            if(sourceFile == null) return true;

            string target;
            if(path.Trim() == "")
            {
                target = this.CurrentDirectory + path;
            }
            else if(path.Contains("/"))
            {
                target = AdjustDir(path) + Path.AltDirectorySeparatorChar + fileName + extension;
            }
            else
            {
                target = CurrentDirectory + uploadRemotePath;
            }

            string URI = Hostname + target;
            System.Net.FtpWebRequest ftp = GetRequest(URI);

            ftp.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
            ftp.UseBinary = true;

            ftp.ContentLength = sourceFile.Length;

            const int BufferSize = 2048;
            byte[] content = new byte[BufferSize - 1 + 1];
            int dataRead;

            using(FileStream fs = sourceFile.OpenRead())
            {
                try
                {
                    using(Stream rs = ftp.GetRequestStream())
                    {
                        try
                        {
                            do
                            {
                                dataRead = fs.Read(content, 0, BufferSize);
                                rs.Write(content, 0, dataRead);
                            } while(!(dataRead < BufferSize));
                        }
                        catch(Exception ex)
                        {
                            ExceptionLog.WriteLog(ex.Message);
                        }
                        finally
                        {
                            rs.Close();
                        }
                    }
                }
                catch(Exception ex)
                {
                    ExceptionLog.WriteLog(ex.Message);
                }
                finally
                {
                    fs.Close();
                }
            }

            ftp = null;
            return true;
        }

        private void CheckFilename(ref string filename)
        {
            char[] invalidChar = Path.GetInvalidFileNameChars();
            for(int i = 0; i < invalidChar.Length; i++)
            {
                if(filename.IndexOf(invalidChar[i]) > 0)
                    filename.Replace(invalidChar[i], '_');
            }
        }

        public bool Download(string sourceFilename, string localFilename, bool PermitOverwrite)
        {
            FileInfo fi = new FileInfo(localFilename);
            return this.Download(sourceFilename, fi, PermitOverwrite);
        }

        public bool Download(FtpFileInfo file, string localFilename, bool PermitOverwrite)
        {
            return this.Download(file.FullName, localFilename, PermitOverwrite);
        }

        public bool Download(FtpFileInfo file, FileInfo localFI, bool PermitOverwrite)
        {
            return this.Download(file.FullName, localFI, PermitOverwrite);
        }

        public bool Download(string sourceFilename, FileInfo targetFI, bool PermitOverwrite)
        {
            if(targetFI.Exists && !(PermitOverwrite))
            {
                throw (new ApplicationException("Target file already exists"));
            }

            string target;
            if(sourceFilename.Trim() == "")
            {
                throw (new ApplicationException("File not specified"));
            }
            else if(sourceFilename.Contains("/"))
            {
                target = AdjustDir(sourceFilename);
            }
            else
            {
                target = CurrentDirectory + sourceFilename;
            }

            string URI = Hostname + target;

            System.Net.FtpWebRequest ftp = GetRequest(URI);
            ftp.Method = System.Net.WebRequestMethods.Ftp.DownloadFile;
            ftp.UseBinary = true;

            using(FtpWebResponse response = (FtpWebResponse)ftp.GetResponse())
            {
                using(Stream responseStream = response.GetResponseStream())
                {
                    using(FileStream fs = targetFI.OpenWrite())
                    {
                        try
                        {
                            byte[] buffer = new byte[2048];
                            int read = 0;
                            do
                            {
                                read = responseStream.Read(buffer, 0, buffer.Length);
                                fs.Write(buffer, 0, read);
                            } while(!(read == 0));
                            responseStream.Close();
                            fs.Flush();
                            fs.Close();
                        }
                        catch(Exception)
                        {
                            fs.Close();
                            targetFI.Delete();
                            throw;
                        }
                    }

                    responseStream.Close();
                }

                response.Close();
            }


            return true;
        }

        public bool FtpDelete(string filename)
        {
            string URI = this.Hostname + GetFullPath(filename);

            System.Net.FtpWebRequest ftp = GetRequest(URI);
            ftp.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;
            try
            {
                string str = GetStringResponse(ftp);
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }

        public bool FtpFileExists(string filename)
        {
            try
            {
                long size = GetFileSize(filename);
                return true;

            }
            catch(Exception ex)
            {
                if(ex is System.Net.WebException)
                {
                    if(ex.Message.Contains("550"))
                    {
                        return false;
                    }
                    else
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        public long GetFileSize(string filename)
        {
            string path;
            if(filename.Contains("/"))
            {
                path = AdjustDir(filename);
            }
            else
            {
                path = this.CurrentDirectory + filename;
            }
            string URI = this.Hostname + path;
            System.Net.FtpWebRequest ftp = GetRequest(URI);
            ftp.Method = System.Net.WebRequestMethods.Ftp.GetFileSize;
            string tmp = this.GetStringResponse(ftp);
            return GetSize(ftp);
        }

        public bool FtpRename(string sourceFilename, string newName)
        {
            string source = GetFullPath(sourceFilename);
            if(!FtpFileExists(source))
            {
                throw (new FileNotFoundException("File " + source + " not found"));
            }

            string target = GetFullPath(newName);
            if(target == source)
            {
                throw (new ApplicationException("Source and target are the same"));
            }
            else if(FtpFileExists(target))
            {
                throw (new ApplicationException("Target file " + target + " already exists"));
            }

            string URI = this.Hostname + source;

            System.Net.FtpWebRequest ftp = GetRequest(URI);
            ftp.Method = System.Net.WebRequestMethods.Ftp.Rename;
            ftp.RenameTo = target;

            try
            {
                string str = GetStringResponse(ftp);
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }

        private bool FtpCreateDirectory(string dirpath)
        {
            string URI = this.Hostname + AdjustDir(dirpath);
            System.Net.FtpWebRequest ftp = GetRequest(URI);
            ftp.Method = System.Net.WebRequestMethods.Ftp.MakeDirectory;

            try
            {
                string str = GetStringResponse(ftp);
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool FtpDeleteDirectory(string dirpath)
        {
            string URI = this.Hostname + AdjustDir(dirpath);
            System.Net.FtpWebRequest ftp = GetRequest(URI);
            ftp.Method = System.Net.WebRequestMethods.Ftp.RemoveDirectory;

            try
            {
                string str = GetStringResponse(ftp);
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }

        private FtpWebRequest GetRequest(string URI)
        {
            FtpWebRequest result = (FtpWebRequest)FtpWebRequest.Create(URI);
            result.Credentials = GetCredentials();
            result.KeepAlive = false;

            return result;
        }

        private System.Net.ICredentials GetCredentials()
        {
            if(string.IsNullOrEmpty(Username))
                return new System.Net.NetworkCredential();
            return new System.Net.NetworkCredential(Username, Password);
        }

        private string GetFullPath(string file)
        {
            if(file.Contains("/"))
            {
                return AdjustDir(file);
            }
            else
            {
                return this.CurrentDirectory + file;
            }
        }

        private string AdjustDir(string path)
        {
            return ((path.StartsWith("/")) ? "" : "/").ToString() + path;
        }

        private string GetDirectory(string directory)
        {
            string URI;
            if(directory == "")
            {
                URI = Hostname + this.CurrentDirectory;
                _lastDirectory = this.CurrentDirectory;
            }
            else
            {
                if(!directory.StartsWith("/"))
                {
                    throw (new ApplicationException("Directory should start with /"));
                }
                URI = this.Hostname + directory;
                _lastDirectory = directory;
            }
            return URI;
        }

        private string _lastDirectory = "";

        private string GetStringResponse(FtpWebRequest ftp)
        {
            string result = "";
            using(FtpWebResponse response = (FtpWebResponse)ftp.GetResponse())
            {
                long size = response.ContentLength;
                using(Stream datastream = response.GetResponseStream())
                {
                    using(StreamReader sr = new StreamReader(datastream))
                    {
                        result = sr.ReadToEnd();
                        sr.Close();
                    }

                    datastream.Close();
                }

                response.Close();
            }

            return result;
        }

        private long GetSize(FtpWebRequest ftp)
        {
            long size;
            using(FtpWebResponse response = (FtpWebResponse)ftp.GetResponse())
            {
                size = response.ContentLength;
                response.Close();
            }

            return size;
        }


        public string Hostname
        {
            get
            {
                if(m_HostName.StartsWith("ftp://"))
                {
                    return m_HostName;
                }
                else
                {
                    return "ftp://" + m_HostName;
                }
            }
            set
            {
                m_HostName = value;
            }
        }

        public string Username
        {
            get
            {
                return (m_UserName == "" ? "anonymous" : m_UserName);
            }
            set
            {
                m_UserName = value;
            }
        }

        public string Password
        {
            get
            {
                return m_Password;
            }
            set
            {
                m_Password = value;
            }
        }

        private string _currentDirectory = "/";
        public string CurrentDirectory
        {
            get
            {
                return _currentDirectory + ((_currentDirectory.EndsWith("/")) ? "" : "/").ToString();
            }
            set
            {
                if(!value.StartsWith("/"))
                {
                    throw (new ApplicationException("Directory should start with /"));
                }
                _currentDirectory = value;
            }
        }
    }

    public class FtpFileInfo
    {
        public string FullName
        {
            get
            {
                return Path + Filename;
            }
        }
        public string Filename
        {
            get
            {
                return _filename;
            }
        }
        public string Path
        {
            get
            {
                return _path;
            }
        }
        public DirectoryEntryTypes FileType
        {
            get
            {
                return _fileType;
            }
        }
        public long Size
        {
            get
            {
                return _size;
            }
        }
        public DateTime FileDateTime
        {
            get
            {
                return _fileDateTime;
            }
        }
        public string Permission
        {
            get
            {
                return _permission;
            }
        }
        public string Extension
        {
            get
            {
                int i = this.Filename.LastIndexOf(".");
                if(i >= 0 && i < (this.Filename.Length - 1))
                {
                    return this.Filename.Substring(i + 1);
                }
                else
                {
                    return "";
                }
            }
        }
        public string NameOnly
        {
            get
            {
                int i = this.Filename.LastIndexOf(".");
                if(i > 0)
                {
                    return this.Filename.Substring(0, i);
                }
                else
                {
                    return this.Filename;
                }
            }
        }
        private string _filename;
        private string _path;
        private DirectoryEntryTypes _fileType;
        private long _size;
        private DateTime _fileDateTime;
        private string _permission;

        public enum DirectoryEntryTypes
        {
            File,
            Directory
        }

        public FtpFileInfo(string line, string path)
        {
            Match m = GetMatchingRegex(line);
            if(m == null)
            {
                //throw (new ApplicationException("Unable to parse line: " + line));
                ExceptionLog.WriteLog("Unable to parse line: " + line);
            }
            else
            {
                _filename = m.Groups["name"].Value;
                _path = path;

                Int64.TryParse(m.Groups["size"].Value, out _size);

                _permission = m.Groups["permission"].Value;
                string _dir = m.Groups["dir"].Value;
                if(_dir != "" && _dir != "-")
                {
                    _fileType = DirectoryEntryTypes.Directory;
                }
                else
                {
                    _fileType = DirectoryEntryTypes.File;
                }

                try
                {
                    _fileDateTime = DateTime.Parse(m.Groups["timestamp"].Value);
                }
                catch(Exception)
                {
                    _fileDateTime = Convert.ToDateTime(null);
                }

            }
        }

        private Match GetMatchingRegex(string line)
        {
            Regex rx;
            Match m;
            for(int i = 0; i <= _ParseFormats.Length - 1; i++)
            {
                rx = new Regex(_ParseFormats[i]);
                m = rx.Match(line);
                if(m.Success)
                {
                    return m;
                }
            }
            return null;
        }

        private static string[] _ParseFormats = new string[] { 
            "(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\w+\\s+\\w+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{4})\\s+(?<name>.+)", 
            "(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\d+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{4})\\s+(?<name>.+)", 
            "(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\d+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{1,2}:\\d{2})\\s+(?<name>.+)", 
            "(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\w+\\s+\\w+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{1,2}:\\d{2})\\s+(?<name>.+)", 
            "(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})(\\s+)(?<size>(\\d+))(\\s+)(?<ctbit>(\\w+\\s\\w+))(\\s+)(?<size2>(\\d+))\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{2}:\\d{2})\\s+(?<name>.+)", 
            "(?<timestamp>\\d{2}\\-\\d{2}\\-\\d{2}\\s+\\d{2}:\\d{2}[Aa|Pp][mM])\\s+(?<dir>\\<\\w+\\>){0,1}(?<size>\\d+){0,1}\\s+(?<name>.+)" };
    }

    public class FtpDirectory : List<FtpFileInfo>
    {


        public FtpDirectory()
        {

        }

        public FtpDirectory(string dir, string path)
        {
            foreach(string line in dir.Replace("\n", "").Split(System.Convert.ToChar('\r')))
            {
                if(line != "")
                {
                    this.Add(new FtpFileInfo(line, path));
                }
            }
        }

        public FtpDirectory GetFiles(string ext)
        {
            return this.GetFileOrDir(FtpFileInfo.DirectoryEntryTypes.File, ext);
        }

        public FtpDirectory GetDirectories()
        {
            return this.GetFileOrDir(FtpFileInfo.DirectoryEntryTypes.Directory, "");
        }

        private FtpDirectory GetFileOrDir(FtpFileInfo.DirectoryEntryTypes type, string ext)
        {
            FtpDirectory result = new FtpDirectory();
            foreach(FtpFileInfo fi in this)
            {
                if(fi.FileType == type)
                {
                    if(ext == "")
                    {
                        result.Add(fi);
                    }
                    else if(ext == fi.Extension)
                    {
                        result.Add(fi);
                    }
                }
            }
            return result;

        }

        public bool FileExists(string filename)
        {
            foreach(FtpFileInfo ftpfile in this)
            {
                if(ftpfile.Filename == filename)
                {
                    return true;
                }
            }
            return false;
        }

        private const char slash = '/';

        public static string GetParentDirectory(string dir)
        {
            string tmp = dir.TrimEnd(slash);
            int i = tmp.LastIndexOf(slash);
            if(i > 0)
            {
                return tmp.Substring(0, i - 1);
            }
            else
            {
                throw (new ApplicationException("No parent for root"));
            }
        }
    }
}

