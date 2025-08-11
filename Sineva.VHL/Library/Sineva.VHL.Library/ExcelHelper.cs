/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.12 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;

namespace Sineva.VHL.Library
{
    public class ExcelHelper<T>
    {
        public Type m_Type;

        public ExcelHelper()
        {
            m_Type = typeof(T);
        }

        public void Save(string path, object obj)
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            using (TextWriter textWriter= new StreamWriter(path))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(m_Type);
                    serializer.Serialize(textWriter, obj);
                }
                catch(Exception ex)
                {
                    ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
                }
                textWriter.Close();
            }
        }

        public void NewExcelFile(string FileName)
        {
            try
            {
                // 새로운 Excel File 만들기
                FileInfo file = new FileInfo(FileName);

                if (file.Exists)
                {
                    // Fiel 있는 경우 Backup본을 만들어 두자....
                    file.CopyTo(FileName + ".old", true);
                    file.Delete();
                }

                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook objExcelWorkbook;                            //엑셀 워크북 개체를 만듭니다.
                excel.Visible = false;                                            //엑셀화면을 표시하게 합니다.(확인용)

                objExcelWorkbook = excel.Workbooks.Add(Type.Missing);     //엑셀 응용프로그램 개체를 이용해서, 새 워크북을 추가 합니다.

                //워크시트가 추가된 워크북을 지정된 경로에 저장합니다. 앞에 @ 주의 하시고, 7번째 인자는 그대로 입력 하심 됩니다..
                objExcelWorkbook.SaveAs(@FileName, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing
                     , Type.Missing, Type.Missing, Type.Missing);
                //워크북을 닫습니다.
                objExcelWorkbook.Close(false, Type.Missing, Type.Missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objExcelWorkbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        public void AddWorkSheet(Microsoft.Office.Interop.Excel.Workbook workBook)
        {
            if (workBook == null) return;

            Microsoft.Office.Interop.Excel.Worksheet objExcelWorksheet;                            //엑셀 워크시트 개체를 만듭니다.
            objExcelWorksheet = workBook.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing) as Microsoft.Office.Interop.Excel.Worksheet;
            workBook.Save();
        }

        public bool NewExcelFile(List<string> ExcelData)
        {
            bool bRet = false;
            try
            {
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook objExcelWorkbook;                            //엑셀 워크북 개체를 만듭니다.
                Microsoft.Office.Interop.Excel.Worksheet objExcelWorksheet;                            //엑셀 워크시트 개체를 만듭니다.
                excel.Visible = false;                                            //엑셀화면을 표시하게 합니다.(확인용)

                objExcelWorkbook = excel.Workbooks.Add(Type.Missing);     //엑셀 응용프로그램 개체를 이용해서, 새 워크북을 추가 합니다.
                objExcelWorksheet = objExcelWorkbook.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing) as Microsoft.Office.Interop.Excel.Worksheet;
                for (int i = 0; i < ExcelData.Count; i++)
                {
                    string[] sArrary = ExcelData[i].Split(',');

                    for (int j = 0; j < sArrary.Length; j++)
                    {
                        objExcelWorksheet.Cells[i + 1, j + 1] = sArrary[j];
                    }
                }

                //워크시트가 추가된 워크북을 지정된 경로에 저장합니다. 앞에 @ 주의 하시고, 7번째 인자는 그대로 입력 하심 됩니다..
                objExcelWorkbook.SaveAs(@"d:\true.xls", Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing
                     , Type.Missing, Type.Missing, Type.Missing);
                //워크북을 닫습니다.
                objExcelWorkbook.Close(false, Type.Missing, Type.Missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objExcelWorksheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objExcelWorkbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
            }
            catch
            {
            }
            finally
            {
            }
            return bRet;
        }

        public bool NewExcelFile(string sourceExcelFile, string destExcelFile, string SheetName, List<string> ExcelData)
        {
            bool bRet = false;
            Microsoft.Office.Interop.Excel.Application excel = null;
            Microsoft.Office.Interop.Excel.Workbook objExcelWorkbook = null;                            //엑셀 워크북 개체를 만듭니다.
            Microsoft.Office.Interop.Excel.Worksheet objExcelWorksheet = null;                            //엑셀 워크시트 개체를 만듭니다.

            if (File.Exists(sourceExcelFile) == true)
            {
                try
                {
                    excel = new Microsoft.Office.Interop.Excel.Application();
                    excel.Visible = true;                                            //엑셀화면을 표시하게 합니다.(확인용)

                    objExcelWorkbook = excel.Workbooks.Open(sourceExcelFile, 0, false, 5, Type.Missing, Type.Missing, true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, Type.Missing, Type.Missing);

                    if (excel.Workbooks.Count == 0)
                    {
                        string msg = string.Format("[{0}][WorkBooks][Sheet]가 없습니다.!\n\n다시 시도 하십시오!", sourceExcelFile);
                        MessageBox.Show(msg, "[XL_SAVE][Excel][저장]Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return false;
                    }

                    //추가된 워크북 개체를 이용해서, 새 워크시트 를 추가 합니다. 타입 캐스팅은 해주셔야 되요.
                    objExcelWorksheet = objExcelWorkbook.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing) as Microsoft.Office.Interop.Excel.Worksheet;

                    // 동일 sheet 이름이 있을 경우는 .copy를 붙인다.
                    objExcelWorksheet.Name = SheetName;
                    for (int i = 0; i < ExcelData.Count; i++)
                    {
                        string[] sArrary = ExcelData[i].Split(',');

                        for (int j = 0; j < sArrary.Length; j++)
                        {
                            objExcelWorksheet.Cells[i + 1, j + 1] = sArrary[j];
                        }
                    }


                    objExcelWorkbook.Save();

                    //워크시트가 추가된 워크북을 지정된 경로에 저장합니다. 앞에 @ 주의 하시고, 7번째 인자는 그대로 입력 하심 됩니다..
                    objExcelWorkbook.SaveAs(@"d:\sample.xls", Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing
                         , Type.Missing, Type.Missing, Type.Missing);

                    FileInfo file = new FileInfo("d:\\sample.xls");
                    if (file.Exists)
                    {
                        file.CopyTo(destExcelFile);
                    }

                    //워크북을 닫습니다.
                    objExcelWorkbook.Close(false, Type.Missing, Type.Missing);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(objExcelWorksheet);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(objExcelWorkbook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
                }
                catch
                {
                    if (objExcelWorkbook != null) objExcelWorkbook.Close(false, Type.Missing, Type.Missing);
                    if (objExcelWorksheet != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(objExcelWorksheet);
                    if (objExcelWorkbook != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(objExcelWorkbook);
                    if (excel != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
                }
                finally
                {
                }
            }
            return bRet;

        }

    }
}
