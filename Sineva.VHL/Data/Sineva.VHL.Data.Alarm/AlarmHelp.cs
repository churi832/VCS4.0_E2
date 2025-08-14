using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using UglyToad.PdfPig;
using Microsoft.Win32;
using Sineva.VHL.Library;

namespace Sineva.VHL.Data.Alarm
{
    public static class AlarmHelp
    {

        public static void OpenTroubleshootingManual(string alarmUnit, string alarmName)
        {
            try
            {
                string troubleshootingDir = AppConfig.Instance.TroubleshootingManualPath.SelectedFolder;
                string manualPath = Path.Combine(troubleshootingDir, $"{alarmUnit}.pdf");
                if (!File.Exists(manualPath))
                {
                    FolderBrowserDialog dlg = new FolderBrowserDialog
                    {
                        Description = "Select Troubleshooting Manual Folder",
                        SelectedPath = AppConfig.GetSolutionPath(),
                        ShowNewFolderButton = true
                    };
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        string selectedPath = dlg.SelectedPath;
                        manualPath = Path.Combine(selectedPath, $"{alarmUnit}.pdf");

                        if (MessageBox.Show("Do you want to save the selected folder as default path?",
                                      "Troubleshooting Manual Folder", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            AppConfig.Instance.TroubleshootingManualPath.SelectedFolder = selectedPath;
                            AppConfig.Instance.WriteXml();
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                int page = FindPageByAlarmName(manualPath, alarmName);
                if (page < 0)
                {
                    return;
                }
                OpenPdfAtPage(manualPath, page);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static int FindPageByAlarmName(string manualPath, string alarmName)
        {
            try
            {
                using (var doc = PdfDocument.Open(manualPath))
                {
                    for (int i = doc.NumberOfPages; i >= 1; i--)
                    {
                        var text = doc.GetPage(i).Text;
                        if (text.Contains(alarmName))
                        {
                            return i;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取 PDF 出错：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return -1;
        }

        private static void OpenPdfAtPage(string manualPath, int pageNumber)
        {
            string edgePath = GetEdgePath();
            string fileUri = new Uri(manualPath).AbsoluteUri;
            string url = $"{fileUri}#page={pageNumber}";

            try
            {
                if (File.Exists(edgePath))
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = edgePath,
                        Arguments = $"\"{url}\"",
                        UseShellExecute = false
                    });
                }
                else
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = manualPath,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("打开 PDF 出错：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string GetEdgePath()
        {
            var key = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\msedge.exe");
            return key?.GetValue(null)?.ToString();
        }
    }
}
