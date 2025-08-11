using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace Sineva.VHL.Data.Process
{
    public class BcrMapData
    {
        #region Fields
        private Dictionary<int, List<BcrMapItem>> m_BcrMapDatas = new Dictionary<int, List<BcrMapItem>>(); // <linkID, List<BcrMapItem>>
        private List<BcrMapItem> m_BcrMapItems = new List<BcrMapItem>();
        private bool m_DefaultMapUse = false;
        #endregion

        #region Constructor
        public BcrMapData() 
        { 
        }
        #endregion

        #region Methods
        public bool Initialize()
        {
            bool initialize = ReadBcrMapData();
            return initialize;
        }
        public bool IsDefaultMapUse()
        {
            return m_DefaultMapUse;
        }
        public bool ReadBcrMapData()
        {
            int index = 0;
            try
            {
                string file_name = AppConfig.Instance.AppConfigPath + "\\" + "BcrMapData\\BcrMapData.map";
                if (File.Exists(file_name) == false)
                {
                    if (MessageBox.Show("do you want to default BcrMapData ?", "Create", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        WriteDefaultMapData(file_name);
                    }
                    else
                    {
                        OpenFileDialog dlg = new OpenFileDialog();
                        dlg.Title = "Select BCR Map Data file : BcrMapData.map";
                        dlg.Filter = "map files (*.map)|*.map|All files (*.*)|*.*";
                        dlg.InitialDirectory = Application.StartupPath;
                        if (DialogResult.OK == dlg.ShowDialog())
                        {
                            file_name = dlg.FileName;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                using (StreamReader sr = new StreamReader(file_name))
                {
                    m_BcrMapDatas.Clear();
                    char[] separator = new char[] { ',', '\t' };
                    while (sr.Peek() > -1)
                    {
                        string text = sr.ReadLine();
                        string[] splitText = text.Split(separator);

                        if (splitText.Length > 0)
                        {
                            if (splitText[0] == string.Empty) continue;
                            
                            string name = splitText[0];
                            int linkId = int.Parse(splitText[1]);
                            if (linkId == 0) continue;

                            int fromId = 0, toId = 0;
                            List<int> fromIds = new List<int>();
                            List<int> toIds = new List<int>();
                            if (int.TryParse(splitText[2], out fromId)) fromIds.Add(fromId);
                            if (int.TryParse(splitText[3], out fromId)) fromIds.Add(fromId);
                            if (int.TryParse(splitText[4], out toId)) toIds.Add(toId);
                            if (int.TryParse(splitText[5], out toId)) toIds.Add(toId);
                            double x = double.Parse(splitText[6]);
                            double y = double.Parse(splitText[7]);
                            double z = x - y;

                            if (m_BcrMapDatas.ContainsKey(linkId))
                            {
                                m_BcrMapDatas[linkId].Add(new BcrMapItem(index, name, linkId, x, y, z, fromIds, toIds));
                            }
                            else
                            {
                                List<BcrMapItem> maps = new List<BcrMapItem>
                                {
                                    new BcrMapItem(index, name, linkId, x, y, z, fromIds, toIds)
                                };
                                m_BcrMapDatas.Add(linkId, maps);
                            }

                            index++;
                        }
                    }

                    sr.Close();

                    // Feature Set
                    m_BcrMapItems.Clear();
                    double maxLen = double.MinValue;
                    foreach (KeyValuePair<int, List<BcrMapItem>> pair in m_BcrMapDatas)
                    {
                        List<BcrMapItem> items = pair.Value;
                        double max_x = items.Select(item => item.x).ToList().Max();
                        double max_y = items.Select(item => item.y).ToList().Max();
                        double min_x = items.Select(item => item.x).ToList().Min();
                        double min_y = items.Select(item => item.y).ToList().Min();

                        int count = items.Count;
                        for (int i = 0; i < count; i++)
                        {
                            items[i].max_x = max_x;
                            items[i].min_x = min_x;
                            items[i].max_y = max_y;
                            items[i].min_y = min_y;
                            items[i].a = i > 0 ? (items[i].y - items[i - 1].y) / (items[i].x - items[i - 1].x) : 0.0f;
                            items[i].b = i < count - 1 ? (items[i + 1].y - items[i].y) / (items[i + 1].x - items[i].x) : 0.0f;

                            if (count > 1 && i < count - 1)
                            {
                                double len = Math.Max(Math.Abs(items[i + 1].x - items[i].x), Math.Abs(items[i + 1].y - items[i].y));
                                if (len > maxLen) maxLen = len;
                            }
                        }

                        m_BcrMapItems.AddRange(items);
                    }
                }

                int linkCount = DatabaseHandler.Instance.DictionaryLinkDataList.Count;
                int mapCount = m_BcrMapItems.Count;
                if (mapCount == 2 * linkCount)
                {
                    m_DefaultMapUse = true;
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(method, ex.ToString());
                return false;
            }
            return true;
        }
        public void WriteDefaultMapData(string fileName)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    foreach (KeyValuePair<int, DataItem_Link> item in DatabaseHandler.Instance.DictionaryLinkDataList)
                    {
                        string header = string.Empty;
                        header += string.Format("from:{0}~to:{1},", item.Value.FromNodeID, item.Value.ToNodeID);
                        header += string.Format("{0},", item.Value.LinkID);
                        List<DataItem_Link> fromNearLinks = DatabaseHandler.Instance.DictionaryLinkDataList.Values.Where(x => x.ToNodeID == item.Value.FromNodeID).ToList();
                        for (int i = 0; i < 2; i++)
                        {
                            if (i < fromNearLinks.Count) header += string.Format("{0},", fromNearLinks[i].LinkID);
                            else header += string.Format(",");
                        }
                        List<DataItem_Link> toNearLinks = DatabaseHandler.Instance.DictionaryLinkDataList.Values.Where(x => x.FromNodeID == item.Value.ToNodeID).ToList();
                        for (int i = 0; i < 2; i++)
                        {
                            if (i < toNearLinks.Count) header += string.Format("{0},", toNearLinks[i].LinkID);
                            else header += string.Format(",");
                        }

                        bool sCurve = false;
                        sCurve |= item.Value.Type == LinkType.LeftSBranch;
                        sCurve |= item.Value.Type == LinkType.RightSBranch;
                        sCurve &= item.Value.SteerChangeLeftBCR != 0.0f;
                        sCurve &= item.Value.SteerChangeRightBCR != 0.0f;
                        DataItem_Node toNode = DatabaseHandler.Instance.DictionaryNodeDataList.Values.Where(x => x.NodeID == item.Value.ToNodeID).FirstOrDefault();
                        if (toNode == null) continue;
                        double aN = toNode.LocationValue1;
                        double bN = toNode.LocationValue2;
                        if (sCurve)
                        {
                            double a0 = item.Value.LeftBCRBegin;
                            double b0 = item.Value.RightBCRBegin;
                            double a1 = item.Value.LeftBCREnd;
                            double b1 = item.Value.RightBCREnd;
                            double a2 = item.Value.SteerChangeLeftBCR;
                            double b2 = item.Value.SteerChangeRightBCR;
                            double pitch = 200.0f;
                            if (item.Value.BCRMatchType == "L" && a0 != 0.0f) // 왼쪽 SCurve
                            {
                                // 왼쪽 계산
                                double diff = (a2 - a0);
                                int count = (int)(diff / pitch + 1);
                                for (int i = 0; i < count + 1; i++)
                                {
                                    string buf = header;
                                    double distance = i * pitch;
                                    double leftBcr = a0 + distance;
                                    if (distance >= diff) leftBcr = a2 - 0.1f;
                                    if (leftBcr >= aN && aN >= a1) leftBcr = aN - 0.1f;
                                    buf += string.Format("{0:F1},0.0", leftBcr);
                                    sw.WriteLine(buf);
                                }
                                // 오른쪽 계산
                                diff = (b1 - b2);
                                count = (int)(diff / pitch + 1);
                                for (int i = 0; i < count + 1; i++)
                                {
                                    string buf = header;
                                    double distance = i * pitch;
                                    double rightBcr = b2 + distance;
                                    if (distance >= diff) rightBcr = b1 - 0.1f;
                                    if (rightBcr >= bN && bN >= b1) rightBcr = bN - 0.1f;
                                    buf += string.Format("0.0,{0:F1}", rightBcr);
                                    sw.WriteLine(buf);
                                }
                            }
                            else if (item.Value.BCRMatchType == "R" && b0 != 0.0f) // 오른쪽 SCurve
                            {
                                // 오른쪽 계산
                                double diff = (b2 - b0);
                                int count = (int)(diff / pitch + 1);
                                for (int i = 0; i < count + 1; i++)
                                {
                                    string buf = header;
                                    double distance = i * pitch;
                                    double rightBcr = b0 + distance;
                                    if (distance >= diff) rightBcr = b2 - 0.1f;
                                    if (rightBcr >= bN && bN >= b1) rightBcr = bN - 0.1f;
                                    buf += string.Format("0.0,{0:F1}", rightBcr);
                                    sw.WriteLine(buf);
                                }
                                // 왼쪽 계산
                                diff = (a1 - a2);
                                count = (int)(diff / pitch + 1);
                                for (int i = 0; i < count + 1; i++)
                                {
                                    string buf = header;
                                    double distance = i * pitch;
                                    double leftBcr = a2 + distance;
                                    if (distance >= diff) leftBcr = a1 - 0.1f;
                                    if (leftBcr >= aN && aN >= a1) leftBcr = aN - 0.1f;
                                    buf += string.Format("{0:F1},0.0", leftBcr);
                                    sw.WriteLine(buf);
                                }
                            }
                        }
                        else
                        {
                            double a0 = item.Value.LeftBCRBegin;
                            double b0 = item.Value.RightBCRBegin;
                            double a1 = item.Value.LeftBCREnd;
                            double b1 = item.Value.RightBCREnd;
                            double pitch = 200.0f;
                            int count = (int)(item.Value.Distance / pitch + 1);
                            for (int i = 0; i < count + 1; i++)
                            {
                                string buf = header;
                                double distance = i * pitch;
                                if (item.Value.BCRMatchType != "R" && a0 != 0.0f)
                                {
                                    double leftBcr = a0 + distance;
                                    if (distance >= item.Value.Distance) leftBcr = a1 - 0.1f;
                                    if (leftBcr >= aN && aN >= a1) leftBcr = aN - 0.1f;
                                    buf += string.Format("{0:F1},", leftBcr);
                                }
                                else buf += string.Format("0.0,");
                                if (item.Value.BCRMatchType != "L" && b0 != 0.0f)
                                {
                                    double rightBcr = b0 + distance;
                                    if (distance >= item.Value.Distance) rightBcr = b1 - 0.1f;
                                    if (rightBcr >= bN && bN >= b1) rightBcr = bN - 0.1f;
                                    buf += string.Format("{0:F1}", rightBcr);
                                }
                                else buf += string.Format("0.0");
                                sw.WriteLine(buf);
                            }
                        }
                    }

                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        public Dictionary<int, BcrMapItem> SearchSimilarLinks(double leftBCR, double rightBCR, double searchRange, double diffRange, enBcrCheckDirection bcrDirection, int curLinkId, bool nonPath)
        {
            try
            {
                double diff = leftBCR - rightBCR;

                //List<BcrMapItem> lists = m_BcrMapItems.Where(item =>
                //                            item.z + diffRange > diff &&
                //                            item.z - diffRange < diff &&
                //                            Math.Abs(item.x - leftBCR) < searchRange &&
                //                            Math.Abs(item.y - rightBCR) < searchRange).ToList();

                // Left/Right All 확인
                List<BcrMapItem> lists = m_BcrMapItems.Where(item =>
                                                            item.min_x <= leftBCR &&
                                                            item.max_x >= leftBCR &&
                                                            item.min_y <= rightBCR &&
                                                            item.max_y >= rightBCR &&
                                                            item.z + diffRange > diff &&
                                                            item.z - diffRange < diff &&
                                                            Math.Abs(item.x - leftBCR) < searchRange &&
                                                            Math.Abs(item.y - rightBCR) < searchRange).ToList();
                if (lists.Count > 0)
                {
                    lists.Sort((x, y) => Math.Sqrt(Math.Pow((x.x - leftBCR), 2) + Math.Pow((x.y - rightBCR), 2))
                          .CompareTo(Math.Sqrt(Math.Pow((y.x - leftBCR), 2) + Math.Pow((y.y - rightBCR), 2))));
                }

                if (lists.Count == 0)
                {
                    if (bcrDirection == enBcrCheckDirection.Left)
                    {
                        // Left BCR 확인
                        lists = m_BcrMapItems.Where(item => Math.Abs(item.y) < 1.0f &&
                                                    item.min_x <= leftBCR &&
                                                    item.max_x >= leftBCR &&
                                                    Math.Abs(item.x - leftBCR) < searchRange).ToList();
                        if (lists.Count > 0)
                            lists.Sort((x, y) => Math.Sqrt(Math.Pow((x.x - leftBCR), 2)).CompareTo(Math.Sqrt(Math.Pow((y.x - leftBCR), 2))));
                    }
                    else
                    {
                        // Right BCR 확인
                        lists = m_BcrMapItems.Where(item => Math.Abs(item.x) < 1.0f &&
                                item.min_y <= rightBCR &&
                                item.max_y >= rightBCR &&
                                Math.Abs(item.y - rightBCR) < searchRange).ToList();
                        if (lists.Count > 0)
                            lists.Sort((x, y) => Math.Sqrt(Math.Pow((x.y - rightBCR), 2)).CompareTo(Math.Sqrt(Math.Pow((y.y - rightBCR), 2))));
                    }
                }
                if (lists.Count == 0)
                {
                    if (bcrDirection == enBcrCheckDirection.Left)
                    {
                        // Right BCR 확인
                        lists = m_BcrMapItems.Where(item => Math.Abs(item.x) < 1.0f &&
                                                    item.min_y <= rightBCR &&
                                                    item.max_y >= rightBCR &&
                                                    Math.Abs(item.y - rightBCR) < searchRange).ToList();
                        if (lists.Count > 0)
                            lists.Sort((x, y) => Math.Sqrt(Math.Pow((x.y - rightBCR), 2)).CompareTo(Math.Sqrt(Math.Pow((y.y - rightBCR), 2))));
                    }
                    else
                    {
                        // Left BCR 확인
                        lists = m_BcrMapItems.Where(item => Math.Abs(item.y) < 1.0f &&
                                                    item.min_x <= leftBCR &&
                                                    item.max_x >= leftBCR &&
                                                    Math.Abs(item.x - leftBCR) < searchRange).ToList();
                        if (lists.Count > 0)
                            lists.Sort((x, y) => Math.Sqrt(Math.Pow((x.x - leftBCR), 2)).CompareTo(Math.Sqrt(Math.Pow((y.x - leftBCR), 2))));
                    }
                }

                Dictionary<int, BcrMapItem> maps = new Dictionary<int, BcrMapItem>();
                foreach (BcrMapItem item in lists)
                {
                    bool add_enable = true;
                    add_enable &= maps.ContainsKey(item.linkId) == false;
                    if (nonPath)
                    {
                        bool non_path_add = false;
                        non_path_add |= item.linkId == curLinkId;
                        non_path_add |= item.fromLinkId.Contains(curLinkId);
                        non_path_add |= item.toLinkId.Contains(curLinkId);
                        add_enable &= non_path_add;
                    }
                    if (add_enable) maps.Add(item.linkId, item);
                }
                if (nonPath && maps.Count == 0) // 만일 path가 없는데 current link와 연결된 전후 link를 찾지 못하는 경우
                {
                    foreach (BcrMapItem item in lists)
                    {
                        if (maps.ContainsKey(item.linkId) == false) maps.Add(item.linkId, item);
                    }
                }
                return maps;
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
            return null;
        }
        #endregion
    }

    public class BcrMapItem
    {
        public int no;
        public string name;
        public int linkId;
        public List<int> fromLinkId = new List<int>();
        public List<int> toLinkId = new List<int>();
        public double x;
        public double y;
        public double z;
        public double a;
        public double b;
        public double max_x;
        public double max_y;
        public double min_x;
        public double min_y;

        public BcrMapItem()
        {
        }
        public BcrMapItem(int _no, string _name, int _link, double x0, double y0, double z0, List<int> fromIds, List<int> toIds)
        {
            this.no = _no;
            this.name = _name;
            this.linkId = _link;
            this.fromLinkId = fromIds;
            this.toLinkId = toIds;
            this.x = Math.Round(x0, 1);
            this.y = Math.Round(y0, 1);
            this.z = Math.Round(z0, 1);
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", no, linkId, name, x, y, z, Math.Round(a, 5), Math.Round(b, 5));
        }
    }
}
