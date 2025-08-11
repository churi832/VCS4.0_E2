using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Data;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace Sineva.VHL.Library.InteropServices
{
    public class InteropServiceExcel
    {
        public static void DataSet_ExcelSave(DataSet dsSource, string filePath)
        {
            Excel.Application excel = null;
            Excel.Workbook book = null;
            Excel.Worksheet sheet = null;
            object TypMissing = Type.Missing;

            try
            {
                excel = new Excel.Application();
                excel.Visible = false;  //엑셀 작업 하는 내용 보이지 않음
                excel.Interactive = false; // 유저 조작 방해 받지 않음
                excel.ScreenUpdating = false;
                excel.EnableEvents = false;

                if (System.IO.File.Exists(filePath))
                {
                    book = excel.Workbooks.Open(filePath, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing);
                    //book = excel.Workbooks.Open(filePath, 0, TypMissing, 5, TypMissing, TypMissing, false, TypMissing, TypMissing, true, false, TypMissing, false, false, false);
                    if (book.ReadOnly)
                    {
                        book.Close();
                        book = excel.Workbooks.Open(filePath, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing, TypMissing);
                    }
                }
                else
                {
                    book = excel.Workbooks.Add(1); //Sheet 추가 (n) :n개, () : 3개 
                }

                if (book.ReadOnly) // Excel이 열려 있으면 저장 불가
                {
                    MessageBox.Show("Excel file read only!!, Please close the Excel...", "Excel Error", MessageBoxButtons.OK);
                    excel.Workbooks.Close();
                    excel.Quit();
                    excel.Interactive = true; // 유저 조작 방해 받지 않음

                    ReleaseExcelObject(sheet);
                    ReleaseExcelObject(book);
                    ReleaseExcelObject(excel);
                    return;
                }
                int sheetCount = book.Worksheets.Count;
                foreach (DataTable table in dsSource.Tables)
                {
                    for (int i = 1; i <= sheetCount; i++)
                    {
                        if (((Excel.Worksheet)book.Worksheets.get_Item(i)).Name == table.TableName) //Excel Sheet이름과 Data Table 이름 찾음
                        {
                            sheet = book.Worksheets.Item[table.TableName]; //찾아서 존재하면 그 Sheet를 그대로 사용한다.
                            break; // 만약에 Sheet 이름 같은게 있다면 앞에 있는 Sheet 사용
                        }
                    }
                    if (sheet == null)
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            sheet = book.Worksheets.Add(After: book.Worksheets[sheetCount]); //파일이 있는 경우 기존에 있는 마지막 Sheet 뒤에 새로운 Sheet 추가
                        }
                        else sheet = book.Worksheets.Item[1]; //파일이 없어서 새로 만들 경우에는 Sheet1에 덮어쓰기
                        sheet.Name = table.TableName;
                    }
                    
                    int excelRowNo = 0;
                    int maxColumnNo = table.Columns.Count;
                    int maxRowNo = table.Rows.Count;

                    foreach (DataRow row in table.Rows)
                    {
                        int a = table.Rows.Count;
                        excelRowNo++;
                        for (int j = 0; j < maxColumnNo; j++)
                        {
                            if (row.ItemArray[j].ToString() == string.Empty) continue;
                            //objArray[excelRowNo, j + 1] = row.ItemArray[j].ToString();
                            sheet.Cells[excelRowNo, j + 1] = row.ItemArray[j].ToString();
                        }
                    }
                }

                if ((System.IO.File.Exists(filePath))) book.Save();
                else book.SaveAs(filePath);
                excel.Interactive = true;

                excel.Workbooks.Close();
                excel.Quit();
            }
            catch (Exception err)
            {
                ExceptionLog.WriteLog(err.ToString());
            }

            finally //Clean Up...
            {
                ReleaseExcelObject(sheet);
                ReleaseExcelObject(book);
                ReleaseExcelObject(excel);
            }
        }

        public static DataSet DataSet_ExcelRead(string filePath)
        {
            Excel.Application excel = null;
            Excel.Workbook book = null;
            Excel.Worksheet sheet = null;
            Excel.Range range = null;
            DataSet data_set = new DataSet("SetupWizards");
            object TypMissing = Type.Missing;

            try
            {
                excel = new Excel.Application();
                excel.Visible = false;  //엑셀 작업 하는 내용 보이지 않음
                excel.Interactive = false; // 유저 조작 방해 받지 않음

                if (System.IO.File.Exists(filePath))
                {
                    book = excel.Workbooks.Open(filePath, 0, true, 5, TypMissing, TypMissing, false, TypMissing, TypMissing, true, false, TypMissing, false, false, false);
                    for (int i = 0; i < book.Worksheets.Count; i++)
                    {
                        sheet = (Excel.Worksheet)book.Worksheets.get_Item(i + 1);
                        range = sheet.UsedRange;  // 전체 범위
                        object[,] data = range.Value;
                        if (data == null) continue;
                        for (int r = 1; r <= data.GetLength(0); r++)
                        {
                            for (int c = 1; c <= data.GetLength(1); c++)
                            {
                                if (data[r, c] == null) continue;
                            }
                        }
                        DataTable data_table = new DataTable(sheet.Name);
                        DataColumn column;
                        DataRow row;
                        for (int a = 1; a <= data.GetLength(1); a++)
                        {
                            // Item
                            column = new DataColumn();
                            column.DataType = Type.GetType("System.String"); //데이터 형 지정
                            column.ColumnName = string.Empty;
                            //if (data[1, a] == null) column.ColumnName = string.Empty;
                            //else column.ColumnName = data[1, a].ToString();

                            column.AllowDBNull = true;
                            data_table.Columns.Add(column);
                        }

                        for (int r = 1; r <= data.GetLength(0); r++)
                        {
                            row = data_table.NewRow();
                            for (int c = 1; c <= data.GetLength(1); c++)
                            {
                                if (data[r, c] == null) row[c - 1] = " ";
                                else row[c - 1] = data.GetValue(r, c);

                            }
                            data_table.Rows.Add(row);
                        }
                        data_set.Tables.Add(data_table);
                    }
                    book.Close(false, Type.Missing, Type.Missing);
                }
            }
            catch (Exception err)
            {
                ExceptionLog.WriteLog(err.ToString());
            }
            finally //Clean Up...
            {
                if (excel != null)
                {
                    excel.Workbooks.Close();
                    excel.DisplayAlerts = false;
                    excel.Quit();
                    excel.Interactive = true;

                    ReleaseExcelObject(range);
                    ReleaseExcelObject(sheet);
                    ReleaseExcelObject(book);
                    ReleaseExcelObject(excel);
                }
            }
            return data_set;
        }

        public static void ReleaseExcelObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                obj = null;
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }

        public static bool ExportDataSetToExcel2(DataSet dsSource, string filePath)
        {
            string MessageString = String.Empty;

            if (dsSource.Tables.Count == 0)
            {
                MessageString = string.Format("[Excel][저장]할 DATA가 존재 하지 않습니다!\n\n다시 시도 하십시오!");
                MessageBox.Show(MessageString, "[XL_SAVE][Excel][저장]Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            if (System.IO.File.Exists(filePath))
            {
                MessageString = string.Format("[Excel][저장]할 File이 이미 존재 합니다!\n\n다시 시도 하십시오!");
                MessageBox.Show(MessageString, "[XL_SAVE][Excel][저장]Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }

            Excel.Application objExcel = new Excel.Application();
            if (objExcel == null)
            {
                MessageBox.Show("ERROR: EXCEL couldn't be started!");
                System.Windows.Forms.Application.Exit();
                return false;
            }
            Excel.Workbook objWorkBook = objExcel.Workbooks.Add(Missing.Value);
            //Excel.Workbook objWorkBook = objExcel.Workbooks.Open(filePath, 0, true, 5, Type.Missing, Type.Missing, true, Excel.XlPlatform.xlWindows, "", true, false, 0, true, Type.Missing, Type.Missing);

            int defaultSheetCount = objWorkBook.Sheets.Count;
            int curSheetId = 1;
            foreach (DataTable table in dsSource.Tables)
            {
                Excel.Worksheet sheet;
                sheet = objWorkBook.Sheets.Add();

                // Sheet Name Length Limit : 31 characters
                sheet.Name = table.TableName.Length > 31 ? table.TableName.Substring(0, 31) : table.TableName;
                curSheetId++;

                int excelRowNo = 1;
                int maxColumnNo = table.Columns.Count;
                for (int i = 0; i < maxColumnNo; i++)
                {
                    sheet.Cells[excelRowNo, i + 1] = table.Columns[i].Caption;
                }
                foreach (DataRow row in table.Rows)
                {
                    excelRowNo++;
                    for (int i = 0; i < maxColumnNo; i++)
                    {
                        string temp = row.ItemArray[i].ToString();
                        sheet.Cells[excelRowNo, i + 1] = temp;
                    }
                }
                Marshal.ReleaseComObject(sheet);
            }

            try
            {
                string temp_path = filePath;
                temp_path = temp_path.Replace('<', '_');
                temp_path = temp_path.Replace('>', '_');
                temp_path = temp_path.Replace('?', '_');
                temp_path = temp_path.Replace('[', '_');
                temp_path = temp_path.Replace(']', '_');
                temp_path = temp_path.Replace('-', '_');
                temp_path = temp_path.Replace('|', '_');
                temp_path = temp_path.Replace('*', '_');
                //char[] trims = { '<', '>', '?', '[', ']', ':', '|', '*' };
                //temp_path = temp_path.TrimStart(trims);
                filePath = temp_path;
                objWorkBook.SaveAs(filePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }

            objWorkBook.Close(false, Type.Missing, Type.Missing);
            return true;
        }

        public static void ExportToExcel(DataSet dsSource, string fileName)
        {
            string temp_path = fileName;
            char[] trims = { '<', '>', '?', '[', ']', ':', '|', '*' };
            foreach (char ch in trims) temp_path = temp_path.Replace('<', '_');
            fileName = temp_path;

            System.IO.FileInfo fileinfo = new System.IO.FileInfo(fileName);
            if (fileinfo.Extension.Contains("xls")) fileName = fileinfo.DirectoryName + System.IO.Path.DirectorySeparatorChar + fileinfo.Name.Remove(fileinfo.Name.IndexOf(fileinfo.Extension)) + ".xls";
            else fileName += ".xls";

            if (!System.IO.File.Exists(fileName))
            {
                Excel.Application objExcel = new Excel.Application();
                if (objExcel == null) return;

                Excel.Workbook objWorkBook = objExcel.Workbooks.Add(Missing.Value);
                try
                {
                    objWorkBook.SaveAs(fileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                }
                catch (Exception ex)
                {
                    ExceptionLog.WriteLog(ex.ToString());
                    objWorkBook.Close(false, Type.Missing, Type.Missing);
                    objExcel.Workbooks.Close();
                    objExcel.DisplayAlerts = false;
                    objExcel.Quit();
                    objExcel.Interactive = true;

                    ReleaseExcelObject(objWorkBook);
                    ReleaseExcelObject(objExcel);
                    return;
                }

                objWorkBook.Close(false, Type.Missing, Type.Missing);
                objExcel.Workbooks.Close();
                objExcel.DisplayAlerts = false;
                objExcel.Quit();
                objExcel.Interactive = true;
                ReleaseExcelObject(objWorkBook);
                ReleaseExcelObject(objExcel);
            }

            System.IO.StreamWriter excelDoc;
            try
            {
                excelDoc = new System.IO.StreamWriter(fileName);
                const string startExcelXML = "<xml version>\r\n<Workbook " +
                      "xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" +
                      " xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n " +
                      "xmlns:x=\"urn:schemas-microsoft-com:office:" +
                      "excel\"\r\n xmlns:ss=\"urn:schemas-microsoft-com:" +
                      "office:spreadsheet\">\r\n <Styles>\r\n " +
                      "<Style ss:ID=\"Default\" ss:Name=\"Normal\">\r\n " +
                      "<Alignment ss:Vertical=\"Bottom\"/>\r\n <Borders/>" +
                      "\r\n <Font/>\r\n <Interior/>\r\n <NumberFormat/>" +
                      "\r\n <Protection/>\r\n </Style>\r\n " +
                      "<Style ss:ID=\"BoldColumn\">\r\n <Font " +
                      "x:Family=\"Swiss\" ss:Bold=\"1\"/>\r\n </Style>\r\n " +
                      "<Style ss:ID=\"StringLiteral\">\r\n <NumberFormat" +
                      " ss:Format=\"@\"/>\r\n </Style>\r\n <Style " +
                      "ss:ID=\"Decimal\">\r\n <NumberFormat " +
                      "ss:Format=\"0.0000\"/>\r\n </Style>\r\n " +
                      "<Style ss:ID=\"Integer\">\r\n <NumberFormat " +
                      "ss:Format=\"0\"/>\r\n </Style>\r\n <Style " +
                      "ss:ID=\"DateLiteral\">\r\n <NumberFormat " +
                      "ss:Format=\"mm/dd/yyyy;@\"/>\r\n </Style>\r\n " +
                      "</Styles>\r\n ";
                const string endExcelXML = "</Workbook>";

                int rowCount = 0;
                int sheetCount = 1;
                excelDoc.Write(startExcelXML);
                foreach (DataTable table in dsSource.Tables)
                {
                    excelDoc.Write("<Worksheet ss:Name=\"" + table.TableName + "\">");
                    excelDoc.Write("<Table>");
                    foreach (DataRow x in table.Rows)
                    {
                        rowCount++;
                        //if the number of rows is > 64000 create a new page to continue output
                        if (rowCount == 64000)
                        {
                            rowCount = 0;
                            sheetCount++;
                            excelDoc.Write("</Table>");
                            excelDoc.Write(" </Worksheet>");
                            excelDoc.Write("<Worksheet ss:Name=\"" + table.TableName + "\">");
                            excelDoc.Write("<Table>");
                        }
                        excelDoc.Write("<Row>"); //ID=" + rowCount + "
                        for (int y = 0; y < table.Columns.Count; y++)
                        {
                            if (x.ItemArray[y].ToString() == string.Empty) continue;

                            System.Type rowType;
                            rowType = x[y].GetType();
                            switch (rowType.ToString())
                            {
                                case "System.String":
                                    string XMLstring = x[y].ToString();
                                    XMLstring = XMLstring.Trim();
                                    XMLstring = XMLstring.Replace("&", "&");
                                    XMLstring = XMLstring.Replace(">", ">");
                                    XMLstring = XMLstring.Replace("<", "<");
                                    excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                                   "<Data ss:Type=\"String\">");
                                    excelDoc.Write(XMLstring);
                                    excelDoc.Write("</Data></Cell>");
                                    break;
                                case "System.DateTime":
                                    //Excel has a specific Date Format of YYYY-MM-DD followed by  
                                    //the letter 'T' then hh:mm:sss.lll Example 2005-01-31T24:01:21.000
                                    //The Following Code puts the date stored in XMLDate 
                                    //to the format above
                                    DateTime XMLDate = (DateTime)x[y];
                                    string XMLDatetoString = ""; //Excel Converted Date
                                    XMLDatetoString = XMLDate.Year.ToString() +
                                         "-" +
                                         (XMLDate.Month < 10 ? "0" +
                                         XMLDate.Month.ToString() : XMLDate.Month.ToString()) +
                                         "-" +
                                         (XMLDate.Day < 10 ? "0" +
                                         XMLDate.Day.ToString() : XMLDate.Day.ToString()) +
                                         "T" +
                                         (XMLDate.Hour < 10 ? "0" +
                                         XMLDate.Hour.ToString() : XMLDate.Hour.ToString()) +
                                         ":" +
                                         (XMLDate.Minute < 10 ? "0" +
                                         XMLDate.Minute.ToString() : XMLDate.Minute.ToString()) +
                                         ":" +
                                         (XMLDate.Second < 10 ? "0" +
                                         XMLDate.Second.ToString() : XMLDate.Second.ToString()) +
                                         ".000";
                                    excelDoc.Write("<Cell ss:StyleID=\"DateLiteral\">" +
                                                 "<Data ss:Type=\"DateTime\">");
                                    excelDoc.Write(XMLDatetoString);
                                    excelDoc.Write("</Data></Cell>");
                                    break;
                                case "System.Boolean":
                                    excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                                "<Data ss:Type=\"String\">");
                                    excelDoc.Write(x[y].ToString());
                                    excelDoc.Write("</Data></Cell>");
                                    break;
                                case "System.Int16":
                                case "System.Int32":
                                case "System.Int64":
                                case "System.Byte":
                                    excelDoc.Write("<Cell ss:StyleID=\"Integer\">" +
                                            "<Data ss:Type=\"Number\">");
                                    excelDoc.Write(x[y].ToString());
                                    excelDoc.Write("</Data></Cell>");
                                    break;
                                case "System.Decimal":
                                case "System.Double":
                                    excelDoc.Write("<Cell ss:StyleID=\"Decimal\">" +
                                          "<Data ss:Type=\"Number\">");
                                    excelDoc.Write(x[y].ToString());
                                    excelDoc.Write("</Data></Cell>");
                                    break;
                                case "System.DBNull":
                                    excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                          "<Data ss:Type=\"String\">");
                                    excelDoc.Write("");
                                    excelDoc.Write("</Data></Cell>");
                                    break;
                                default:
                                    throw (new Exception(rowType.ToString() + " not handled."));
                            }
                        }
                        excelDoc.Write("</Row>");
                    }
                    excelDoc.Write("</Table>");
                    excelDoc.Write(" </Worksheet>");
                }
                excelDoc.Write(endExcelXML);
                excelDoc.Close();
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }

        public static DataSet ImportExcelXLS(string fileName, bool hasHeaders)
        {
            string HDR = hasHeaders ? "Yes" : "No";
            string strConn = "";
            if (fileName.Substring(fileName.LastIndexOf('.')).ToLower() == ".xlsx")
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=\"Excel 12.0;HDR=" + HDR + ";IMEX=0\"";
            else
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties=\"Excel 8.0;HDR=" + HDR + ";IMEX=0\"";

            DataSet output = new DataSet();

            try
            {
                using(System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(strConn))
                {
                    conn.Open();

                    DataTable schemaTable = conn.GetOleDbSchemaTable(
                        System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                    foreach(DataRow schemaRow in schemaTable.Rows)
                    {
                        string sheet = schemaRow["TABLE_NAME"].ToString();

                        if(!sheet.EndsWith("_"))
                        {
                            try
                            {
                                System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand("SELECT * FROM [" + sheet + "]", conn);
                                cmd.CommandType = CommandType.Text;

                                DataTable outputTable = new DataTable(sheet);
                                output.Tables.Add(outputTable);
                                new System.Data.OleDb.OleDbDataAdapter(cmd).Fill(outputTable);
                            }
                            catch(Exception ex)
                            {
                                throw new Exception(ex.Message + string.Format("Sheet:{0}.File:F{1}", sheet, fileName), ex);
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
            return output;
        }

        public static DataTable ExcelImport(string Ps_FileName)
        {
            try
            {
                string ExcelConn = "Provider = Microsoft.ACE.OLEDB.12.0;Data Source=" + Ps_FileName +
                       ";Extended Properties='Excel 12.0;HDR=YES'";
                System.Data.OleDb.OleDbConnection excelConn = new System.Data.OleDb.OleDbConnection(string.Format(ExcelConn, Ps_FileName));
                excelConn.Open();

                if(excelConn.State != ConnectionState.Open)
                {
                    //MessageBox.Show("엑셀파일에 연결할 수 없습니다", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                var worksheets = excelConn.GetSchema("Tables");
                string Query = string.Empty;

                Query += " select A.* ";
                Query += string.Format(" from [{0}] as A ", worksheets.Rows[0]["TABLE_NAME"]);
                System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(Query, excelConn);
                System.Data.OleDb.OleDbDataAdapter da = new System.Data.OleDb.OleDbDataAdapter(cmd);
                DataSet ds = new DataSet();

                da.Fill(ds);

                excelConn.Close();

                return ds.Tables[0];
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return null;
            }
        }
    }
}
