using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Servo;

namespace Sineva.VHL.Library.Ajin
{
    class AxtAxisCtrl
    {
        #region Field
        private AxisBlockAXT m_AxisBlock = null;
        private List<AxtBoard> m_AxtBoards = null;
        #endregion

        #region Property
        public List<AxtBoard> AXTBoards
        {
            get { return m_AxtBoards; }
            set { m_AxtBoards = value; }
        }
        #endregion

        #region Constructor
        public AxtAxisCtrl(AxisBlockAXT block)
        {
            m_AxisBlock = block;
            m_AxtBoards = new List<AxtBoard>();
        }
        #endregion

        #region Override
        #endregion

        #region Method
        public bool Initialize()
        {
            OpenCommunication();
            bool rv = IsOpened();

            TaskHandler.Instance.RegTask( new TaskAxtAxisCtrl( this ), 10, System.Threading.ThreadPriority.Normal );
            return rv;
        }
        #endregion
        
        #region [AXT Interface Method]
        private uint OpenCommunication()
        {
            uint rv = 0;
            try
            {
                if(0 == CAXL.AxlIsOpened() ) CAXL.AxlOpen( 7 );
                m_AxisBlock.Connected = CAXL.AxlIsOpened() != 0;

                if( true == m_AxisBlock.Connected )
                {
                    rv = CAXM.AxmMotLoadParaAll( m_AxisBlock.MotPath.SelectedFile );

                    if(rv == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS ||rv==(uint)AXT_FUNC_RESULT.AXT_RT_OPEN_ALREADY )
                    {
                        rv = ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS;
                        LoadBoardModules();
                    }
                    else
                    {
                        rv = ( uint )AXT_FUNC_RESULT.AXT_RT_OPEN_ERROR;
                    }
                }
            }
            catch(Exception ex)
            {
                ex.ToString();
                rv = ( uint )AXT_FUNC_RESULT.AXT_RT_OPEN_ERROR;
            }
            return rv;
        }
        private int CloseCommunication()
        {
            int rv = CAXL.AxlClose();
            m_AxisBlock.Connected = CAXL.AxlIsOpened() == 0;

            return rv;
        }

        private bool IsOpened()
        {
            m_AxisBlock.Connected = CAXL.AxlIsOpened() != 0;
            return m_AxisBlock.Connected;
        }
        public void LoadBoardModules()
        {
            uint rv;
            int boardCount = 0;
            if((rv = CAXL.AxlGetBoardCount(ref boardCount)) == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                uint uintVar = 0;
                byte byteVar = 0;

                for(int i=0; i<boardCount; i++)
                {
                    AxtBoard board = new AxtBoard();
                    board.BoardNo = i;

                    if( ( rv = CAXDev.AxlGetBoardID( board.BoardNo, ref uintVar ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                    {
                        board.BoardType = ( AXT_BASE_BOARD )uintVar;
                    }
                    if( ( rv = CAXDev.AxlGetBoardVersion( board.BoardNo, ref uintVar ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS ) board.BoardVersion = uintVar;
                    if( ( rv = CAXDev.AxlGetBoardAddress( board.BoardNo, ref uintVar ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS ) board.BoardAddress = uintVar;

                    m_AxtBoards.Add( board );
                }
            }

            int axisCount = 0;
            if((rv = CAXM.AxmInfoGetAxisCount(ref axisCount)) == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                for(int i=0; i<axisCount; i++)
                {
                    int boardNo = 0;
                    int modulePos = 0;
                    uint moduleId = 0;
                    uint moduleVer = 0;
                    if( ( rv = CAXM.AxmInfoGetAxis( i, ref boardNo, ref modulePos, ref moduleId ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                    {
                        uint netNo = 0;
                        uint nodeAddr = 0;
                        //if( ( rv = CAXDev.AxlGetModuleNodeInfo( boardNo, modulePos, ref netNo, ref nodeAddr ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS )
                        //{
                        AxtAxis module = new AxtAxis( m_AxisBlock, m_AxisBlock.Axes.ElementAt( i ) );
                        module.AxisId = i;
                        module.ModuleType = ( AXT_MODULE )moduleId;
                        if( ( rv = CAXDev.AxlGetModuleVersion( boardNo, modulePos, ref moduleVer ) ) == ( uint )AXT_FUNC_RESULT.AXT_RT_SUCCESS ) module.ModuleVersion = moduleVer;

                        foreach( AxtBoard axtboard in m_AxtBoards )
                        {
                            if( axtboard.BoardNo == boardNo ) axtboard.Modules.Add( module );
                        }
                        //}
                    }
                }
                foreach(AxtBoard axtBoard in m_AxtBoards)
                {
                    axtBoard.WriteMotionParam();
                }
            }
        }

        private void UpdateAxisStatus()
        {
            if( false == IsOpened() ) return;
            if( 0 == m_AxtBoards.Count ) return;
            try
            {
                foreach(AxtBoard axtboard in m_AxtBoards)
                {
                    foreach(AxtAxis axtaxis in axtboard.Modules)
                    {
                        axtaxis.RefreshAxisStatus();
                        axtaxis.DoAxis();
                        System.Threading.Thread.Sleep( 1 );
                    }
                }
            }
            catch(Exception ex)
            {
                ExceptionLog.WriteLog( ex.Message );
            }
        }
        #endregion
        
        #region Thread
        internal class TaskAxtAxisCtrl : XSequence
        {
            #region Field
            private AxtAxisCtrl m_Control = null;
            SeqUpdate m_SeqUpdate = null;
            #endregion

            #region Constructor
            public TaskAxtAxisCtrl(AxtAxisCtrl _control)
            {
                m_Control = _control;
                m_SeqUpdate = new SeqUpdate( m_Control );
                RegSeq( m_SeqUpdate );
            }
            #endregion

            #region Override
            protected override void ExitRoutine()
            {
                m_Control.CloseCommunication();   
            }
            #endregion
        }

        class SeqUpdate:XSeqFunc
        {
            AxtAxisCtrl m_Control = null;

            public SeqUpdate(AxtAxisCtrl _control)
            {
                m_Control = _control;
            }

            public override int Do()
            {
                //if(AppConfig.Instance.Simulation.AXT)
                //{
                //    return -1;
                //}
                m_Control.UpdateAxisStatus();
                return -1;
            }
        }
        #endregion
    }
}
