using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
    public partial class FileExplore : Form
    {
        // 프로그램 실행 API
        [System.Runtime.InteropServices.DllImport("shell32.dll")]
        public static extern int ShellExecute(int hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
        public const int SW_SHOWNORMAL = 1;

        LoadingWnd m_frmSearch = null;  // 파일 검출시에 로딩 메시지 출력
        public string SelectedFileName = "";

        public FileExplore()
        {
            InitializeComponent();
            InitTreeDriveSetting();
        }

        private void InitImage()
        {
            imglstSmall.Images.Clear();
            imglstLarge.Images.Clear();
            listView.Items.Clear();
        }

        // 리스트뷰에 폴더및 파일정보 삽입
        private void ListViewSetting(String strPath)
        {
            InitImage();

            GetSystemImgList sysimglst = new GetSystemImgList();

            strPath = strPath + "\\";

            DirectoryInfo directoryInfo = new DirectoryInfo(strPath);


            try
            {
                int count = directoryInfo.GetDirectories("*").Length + directoryInfo.GetFiles("*.*").Length;

                CallLoadingWnd(count);

                // 폴더 정보를 얻기
                foreach (DirectoryInfo subdirectoryInfo in directoryInfo.GetDirectories("*"))
                {
                    // 리스트뷰에 입력
                    imglstSmall.Images.Add(sysimglst.GetIcon(subdirectoryInfo.FullName, true, false));
                    imglstLarge.Images.Add(sysimglst.GetIcon(subdirectoryInfo.FullName, false, false));

                    count = imglstSmall.Images.Count;

                    listView.Items.Add(subdirectoryInfo.Name, count - 1);
                    listView.Items[count - 1].SubItems.Add("");
                    listView.Items[count - 1].SubItems.Add("파일 폴더");
                    listView.Items[count - 1].SubItems.Add(subdirectoryInfo.LastWriteTime.ToString());
                    listView.Items[count - 1].SubItems.Add(subdirectoryInfo.Attributes.ToString());

                    Application.DoEvents();
                    RepaintLoadingWnd();

                }

                // 파일 정보(리스트뷰 입력)

                foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.*"))
                {

                    imglstSmall.Images.Add(sysimglst.GetIcon(fileInfo.FullName, true, false));
                    imglstLarge.Images.Add(sysimglst.GetIcon(fileInfo.FullName, false, false));

                    count = imglstSmall.Images.Count;

                    listView.Items.Add(fileInfo.Name, count - 1);
                    listView.Items[count - 1].SubItems.Add(fileInfo.Length.ToString());
                    listView.Items[count - 1].SubItems.Add((fileInfo.Extension.ToString()).Substring(1, 3));
                    listView.Items[count - 1].SubItems.Add(fileInfo.LastWriteTime.ToString());
                    listView.Items[count - 1].SubItems.Add(fileInfo.Attributes.ToString());

                    Application.DoEvents();
                    RepaintLoadingWnd();
                }
            }
            catch
            {
                // 디렉토리/파일 정보 읽을때 예외 발생하면 무시(파일 속성때문에 예외 발생함)
            }

            CloseLoadingWnd();
        }

        // node에 하위 폴더 삽입
        private void TreeNodeSetting(TreeNode node)
        {
            String strPath;
            int count;
            GetSystemImgList sysimglst = new GetSystemImgList();

            strPath = node.FullPath + "\\";

            DirectoryInfo directoryInfo = new DirectoryInfo(strPath);

            // 폴더 정보
            try
            {
                // 폴더 정보를 얻기
                foreach (DirectoryInfo subdirectoryInfo in directoryInfo.GetDirectories("*"))
                {
                    count = node.Nodes.Add(new TreeNode(subdirectoryInfo.Name, imglstTree.Images.Count - 2, imglstTree.Images.Count - 1));
                    if (HasSubDirectory(node.Nodes[count].FullPath))
                    {
                        node.Nodes[count].Nodes.Add("XXX");
                    }
                }
            }
            catch
            {
                CloseLoadingWnd();
                // 디렉토리/파일 정보 읽을때 예외 발생하면 무시(파일 속성때문에 예외 발생함)
            }

        }

        // 트리 노드 찾기
        // 현재 선택된 노드의 자식들중에서 찾는다.
        private TreeNode FindNode(String strDirectory)
        {

            TreeNode temp_node;

            // 현재 선택된 노드의 처음 자식 노드를 찾음
            temp_node = treeView.SelectedNode.FirstNode;

            if (temp_node != null)
            {
                for (; ; )
                {
                    if (temp_node.Text.Equals(strDirectory))
                        return temp_node;
                    temp_node = temp_node.NextNode;
                }
            }
            else
            {
                return null;
            }
        }

        // 트리뷰에 드라이브 정보 입력
        private void InitTreeDriveSetting()
        {
            String[] strDrives = null;
            String strTemp;
            GetSystemImgList sysimglst = new GetSystemImgList();

            int count;

            try
            {
                strDrives = Directory.GetLogicalDrives();
                imglstTree.Images.Clear();

                foreach (string str in strDrives)
                {
                    imglstTree.Images.Add(sysimglst.GetIcon(str, true, false));

                    count = treeView.Nodes.Add(new TreeNode(str.Substring(0, 2), imglstTree.Images.Count - 1, imglstTree.Images.Count - 1));

                    if (HasSubDirectory(str.Substring(0, 2)))
                    {
                        treeView.Nodes[count].Nodes.Add("XXX");
                    }

                }

                DirectoryInfo directoryInfo = new DirectoryInfo(@"C:\");

                foreach (DirectoryInfo subdirectoryInfo in directoryInfo.GetDirectories("*"))
                {
                    imglstTree.Images.Add(sysimglst.GetIcon(subdirectoryInfo.FullName, true, false));
                    imglstTree.Images.Add(sysimglst.GetIcon(subdirectoryInfo.FullName, true, true));
                }

                treeView.SelectedNode = treeView.Nodes[0];
                treeView.Nodes[0].ExpandAll();
            }
            catch(Exception err)
            {

            }
            // 초기화면 C:\Vision\PPAData드라이브 선택 효과
            //string sPath = "C:\\Vision\\PPAData";
            //TreeNode treeNodeTemp;
            //treeView.SelectedNode.Expand();
            //treeNodeTemp = FindNode(sPath);
            //if (treeNodeTemp != null)
            //{
            //    treeView.SelectedNode = treeNodeTemp;
            //    treeNodeTemp.Expand();
            //}

        }

        // 하위 폴더가 있는지 검사한다.
        private bool HasSubDirectory(String strPath)
        {
            strPath = strPath + "\\";

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(strPath);

                if (directoryInfo.GetDirectories("*").Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }

        // 폴더 정보를 출력하는 폼을 생성한다.
        private void CallLoadingWnd(int count)
        {
            // 이미 출력된 상태면 
            if (m_frmSearch != null) return;

            // 출력
            m_frmSearch = new LoadingWnd();
            m_frmSearch.CenterParentFrm(this, count);
            m_frmSearch.Show();
        }

        // 폴더 정보를 출력하는 폼을 제거한다.
        private void CloseLoadingWnd()
        {
            // 출력되기 전이라면 
            if (m_frmSearch == null) return;

            // 제거
            m_frmSearch.Close();
            m_frmSearch.Dispose();
            m_frmSearch = null;

        }

        private void RepaintLoadingWnd()
        {
            m_frmSearch.LoadingImage();
        }

        // 트리노드 축소후 발생 이벤트
        private void treeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            // 하위노드 모두 제거
            for (; ; )
            {
                if (e.Node.FirstNode == null)
                    break;
                else
                    e.Node.Nodes[0].Remove();
            }

            // 하위폴더가 있으면 XXX 추가
            if (HasSubDirectory(e.Node.FullPath))
                e.Node.Nodes.Add("XXX");

            treeView.SelectedNode = e.Node;

        }

        // 트리노드 확장 후 발생 이벤트
        private void treeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            // 하위노드 모두 제거
            for (; ; )
            {
                if (e.Node.FirstNode == null)
                    break;
                else
                    e.Node.Nodes[0].Remove();
            }

            TreeNodeSetting(e.Node);
        }

        // 트리 노드 선택 변경
        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            status_txt.Text = e.Node.FullPath;
            ListViewSetting(e.Node.FullPath);
        }

        private void FileExplore_Load(object sender, EventArgs e)
        {
            //			SearchTimer.Start();   
        }

        private void listView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point();
                p.X = this.Location.X + e.X + 120;
                p.Y = this.Location.Y + e.Y + 40;
                contextMenuStrip.Show(p.X, p.Y);
            }
        }

        private void Open_Click(object sender, EventArgs e)
        {
            String strSel;
            String strSize;

            strSel = listView.SelectedItems[0].SubItems[0].Text;
            strSize = listView.SelectedItems[0].SubItems[1].Text;

            if (strSize.Equals("")) // 폴더일 경우트리뷰에 경로 표시
            {
                TreeNode treeNodeTemp;
                treeView.SelectedNode.Expand();
                treeNodeTemp = FindNode(strSel);

                if (treeNodeTemp != null)
                {
                    treeView.SelectedNode = treeNodeTemp;
                    treeNodeTemp.Expand();
                }
            }
            else //파일일 경우 실행
            {
                ShellExecute(0, "open", status_txt.Text + "\\" + strSel, null, null, SW_SHOWNORMAL);
            }
        }

        private void Select_Click(object sender, EventArgs e)
        {
            String strSel;
            String strSize;

            strSel = listView.SelectedItems[0].SubItems[0].Text;
            strSize = listView.SelectedItems[0].SubItems[1].Text;

            if (strSize.Equals("")) // 폴더일 경우트리뷰에 경로 표시
            {
                TreeNode treeNodeTemp;
                treeView.SelectedNode.Expand();
                treeNodeTemp = FindNode(strSel);

                if (treeNodeTemp != null)
                {
                    treeView.SelectedNode = treeNodeTemp;
                    treeNodeTemp.Expand();
                }
            }
            else //파일일 경우 실행
            {
                SelectedFileName = status_txt.Text + "\\" + strSel;
            }

            this.Dispose();
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            String strSel;
            String strSize;

            strSel = listView.SelectedItems[0].SubItems[0].Text;
            strSize = listView.SelectedItems[0].SubItems[1].Text;

            if (strSize.Equals("")) // 폴더일 경우트리뷰에 경로 표시
            {
                TreeNode treeNodeTemp;
                treeView.SelectedNode.Expand();
                treeNodeTemp = FindNode(strSel);

                if (treeNodeTemp != null)
                {
                    treeView.SelectedNode = treeNodeTemp;
                    treeNodeTemp.Expand();
                }
            }
        }
    }
}
