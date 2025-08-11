using Ionic.Zip;
using System;

namespace Sineva.OHT.Common
{
    public class FileCompressor : Singleton<FileCompressor>
    {
        #region Fields
        public delegate void ProgressEvent(long progress, int totalNums, int count); // 진행 상태 알리기 위한 대리자 
        public event ProgressEvent Compressing; // Zip 진행상황 처리하는 이벤트 

        public static string _ProgressValue;
        public static int _TotalFileNumber;
        public static int _FileCounter;
        #endregion

        #region Constructor
        private FileCompressor() { }
        #endregion

        #region 압축기능
        //폴더 압축 (압축할 폴더명 , 압축한 zip파일 이름)
        public void CompressFiles(string targetPath, string compressFileName)
        {
            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    // Zipping 될때 이벤트 핸들러를 등록 (현재 진행 상황을 위해)
                    zip.SaveProgress += new EventHandler<SaveProgressEventArgs>(Zip_SaveProgress);
                    zip.AddDirectory(targetPath);

                    Compressing += new FileCompressor.ProgressEvent(Zip_Compressing);
                    zip.Save(compressFileName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 이벤트 핸들러
        void Zip_SaveProgress(object sender, SaveProgressEventArgs e)
        {
            try
            {
                int totalFileNumbers = e.EntriesTotal;
                int zippedFileCount = e.EntriesSaved;

                if (Compressing != null) //이벤트가 진행 중이면 
                {
                    Compressing((long)((double)e.BytesTransferred / ((double)e.TotalBytesToTransfer + 1) * 100), totalFileNumbers, zippedFileCount);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void Zip_Compressing(long progress, int totalNums, int count)
        {
            try
            {
                FileCompressor._ProgressValue = progress.ToString();

                if (totalNums != 0)
                {
                    FileCompressor._TotalFileNumber = totalNums;
                }

                if (count != 0)
                {
                    FileCompressor._FileCounter = count;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
