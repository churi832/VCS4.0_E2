using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace Sineva.VHL.Library.Ajin
{
    [Serializable]
    public class AxtBoard
    {
        #region Field
        private AXT_BASE_BOARD m_BoardType = AXT_BASE_BOARD.AXT_UNKNOWN;
        private int m_BoardNo = 0;
        private uint m_BoardVersion = 0;
        private uint m_BoardAddress = 0;
        private byte m_BoardFirmwareVersion = 0;

        private List<AxtAxis> m_Modules = new List<AxtAxis>();
        #endregion

        #region Property
        public AXT_BASE_BOARD BoardType
        {
            get { return m_BoardType; }
            set { m_BoardType = value;}
        }
        public int BoardNo
        {
            get { return m_BoardNo; }
            set { m_BoardNo = value; }
        }
        public uint BoardVersion
        {
            get { return m_BoardVersion; }
            set { m_BoardVersion = value; }
        }
        public uint BoardAddress
        {
            get { return m_BoardAddress; }
            set { m_BoardAddress = value; }
        }
        public byte BoardFirmwareVersion
        {
            get { return m_BoardFirmwareVersion; }
            set { m_BoardFirmwareVersion = value; }
        }

        public List<AxtAxis> Modules
        {
            get { return m_Modules; }
            set { m_Modules = value; }
        }
        #endregion

        #region Override
        public override string ToString()
        {
            return string.Format( "{0:00} : {1}", m_BoardNo, m_BoardType.ToString() );
        }
        #endregion

        #region Method
        public void ReadBoardInfo()
        {

        }
        public string GetBoardInfo()
        {
            string info = string.Empty;
            switch( m_BoardType )
            {
                //case AXT_BASE_BOARD.AXT_UNKNOWN: info = "Unknown Baseboard"; break;
                case AXT_BASE_BOARD.AXT_BIHR: info = "ISA bus, Half size"; break;
                case AXT_BASE_BOARD.AXT_BIFR: info = "ISA bus, Full size"; break;
                case AXT_BASE_BOARD.AXT_BPHR: info = "PCI bus, Half size"; break;
                case AXT_BASE_BOARD.AXT_BPFR: info = "PCI bus, Full size"; break;
                case AXT_BASE_BOARD.AXT_BV3R: info = "VME bus, 3U size"; break;
                case AXT_BASE_BOARD.AXT_BV6R: info = "VME bus, 6U size"; break;
                case AXT_BASE_BOARD.AXT_BC3R: info = "cPCI bus, 3U size"; break;
                case AXT_BASE_BOARD.AXT_BC6R: info = "cPCI bus, 6U size"; break;
                case AXT_BASE_BOARD.AXT_BEHR: info = "PCIe bus, Half size"; break;
                case AXT_BASE_BOARD.AXT_BEFR: info = "PCIe bus, Full size"; break;
                case AXT_BASE_BOARD.AXT_BEHD: info = "PCIe bus, Half size, DB-32T"; break;
                case AXT_BASE_BOARD.AXT_FMNSH4D: info = "ISA bus, Full size, DB-32T, SIO-2V03 * 2"; break;
                case AXT_BASE_BOARD.AXT_PCI_DI64R: info = "PCI bus, Digital IN 64점"; break;
                case AXT_BASE_BOARD.AXT_PCIE_DI64R: info = "PCIe bus, Digital IN 64점"; break;
                case AXT_BASE_BOARD.AXT_PCI_DO64R: info = "PCI bus, Digital OUT 64점"; break;
                case AXT_BASE_BOARD.AXT_PCIE_DO64R: info = "PCIe bus, Digital OUT 64점"; break;
                case AXT_BASE_BOARD.AXT_PCI_DB64R: info = "PCI bus, Digital IN 32점, OUT 32점"; break;
                case AXT_BASE_BOARD.AXT_PCIE_DB64R: info = "PCIe bus, Digital IN 32점, OUT 32점"; break;
                case AXT_BASE_BOARD.AXT_BPFR_COM: info = "PCI bus, Full size, For serial function modules."; break;
                case AXT_BASE_BOARD.AXT_PCIN204: info = "PCI bus, Half size On-Board 2 Axis controller."; break;
                case AXT_BASE_BOARD.AXT_BPHD: info = "PCI bus, Half size, DB-32T"; break;
                case AXT_BASE_BOARD.AXT_PCIN404: info = "PCI bus, Half size On-Board 4 Axis controller.    "; break;
                case AXT_BASE_BOARD.AXT_PCIN804: info = "PCI bus, Half size On-Board 8 Axis controller."; break;
                case AXT_BASE_BOARD.AXT_PCIEN804: info = "PCIe bus, Half size On-Board 8 Axis controller."; break;
                case AXT_BASE_BOARD.AXT_PCIEN404: info = "PCIe bus, Half size On-Board 4 Axis controller."; break;
                case AXT_BASE_BOARD.AXT_PCI_AIO1602HR: info = "PCI bus, Half size, AI-16ch, AO-2ch AI16HR"; break;
                case AXT_BASE_BOARD.AXT_PCI_R1604: info = "PCI bus[PCI9030], Half size, RTEX based 16 axis controller"; break;
                case AXT_BASE_BOARD.AXT_PCI_R3204: info = "PCI bus[PCI9030], Half size, RTEX based 32 axis controller"; break;
                case AXT_BASE_BOARD.AXT_PCI_R32IO: info = "PCI bus[PCI9030], Half size, RTEX based IO only."; break;
                case AXT_BASE_BOARD.AXT_PCI_REV2: info = "Reserved."; break;
                case AXT_BASE_BOARD.AXT_PCI_R1604MLII: info = "PCI bus[PCI9030], Half size, Mechatrolink-II 16/32 axis controller."; break;
                case AXT_BASE_BOARD.AXT_PCI_R0804MLII: info = "PCI bus[PCI9030], Half size, Mechatrolink-II 08 axis controller."; break;
                case AXT_BASE_BOARD.AXT_PCI_Rxx00MLIII: info = "Master PCI Board, Mechatrolink III AXT, PCI Bus[PCI9030], Half size, Max.32 Slave module support"; break;
                case AXT_BASE_BOARD.AXT_PCIE_Rxx00MLIII: info = "Master PCI Express Board, Mechatrolink III AXT, PCI Bus[], Half size, Max.32 Slave module support"; break;
                case AXT_BASE_BOARD.AXT_PCP2_Rxx00MLIII: info = "Master PCI/104-Plus Board, Mechatrolink III AXT, PCI Bus[], Half size, Max.32 Slave module support"; break;
                case AXT_BASE_BOARD.AXT_PCI_R1604SIIIH: info = "PCI bus[PCI9030], Half size, SSCNET III / H 16/32 axis controller."; break;
                case AXT_BASE_BOARD.AXT_PCI_R32IOEV: info = "PCI bus[PCI9030], Half size, RTEX based IO only. Economic version."; break;
                case AXT_BASE_BOARD.AXT_PCIE_R0804RTEX: info = "PCIe bus, Half size, Half size, RTEX based 08 axis controller."; break;
                case AXT_BASE_BOARD.AXT_PCIE_R1604RTEX: info = "PCIe bus, Half size, Half size, RTEX based 16 axis controller."; break;
                case AXT_BASE_BOARD.AXT_PCIE_R2404RTEX: info = "PCIe bus, Half size, Half size, RTEX based 24 axis controller."; break;
                case AXT_BASE_BOARD.AXT_PCIE_R3204RTEX: info = "PCIe bus, Half size, Half size, RTEX based 32 axis controller."; break;
                case AXT_BASE_BOARD.AXT_PCIE_Rxx04SIIIH: info = "PCIe bus, Half size, SSCNET III / H 16(or 32)-axis(CAMC-QI based) controller."; break;
                case AXT_BASE_BOARD.AXT_PCIE_Rxx00SIIIH: info = "PCIe bus, Half size, SSCNET III / H Max. 32-axis(DSP Based) controller."; break;
                case AXT_BASE_BOARD.AXT_ETHERCAT_RTOSV5: info = "EtherCAT, RTOS(On Time), Version 5.29"; break;
                case AXT_BASE_BOARD.AXT_PCI_Nx04_A: info = "PCI bus, Half size On-Board x Axis controller For Rtos."; break;
                case AXT_BASE_BOARD.AXT_PCI_R3200MLIII_IO: info = "PCI Board, Mechatrolink III AXT, PCI Bus[PCI9030], Half size, Max.32 IO only	controller"; break;
                case AXT_BASE_BOARD.AXT_PCIE_Rxx05MLIII: info = "PCIe bus, Half size, Mechatrolink III / H Max. 32-axis(DSP Based) controller."; break;
                case AXT_BASE_BOARD.AXT_PCIE_Rxx05SIIIH: info = "PCIe bus, Half size, Sscnet III / H  32 axis(DSP Based) controller."; break;
                case AXT_BASE_BOARD.AXT_PCIE_Rxx05RTEX: info = "PCIe bus, Half size, RTEX 32 axis(DSP Based) controller."; break;
                case AXT_BASE_BOARD.AXT_PCIE_Rxx05ECAT: info = "PCIe bus, Half size, ECAT(DSP Based) controller."; break;
                case AXT_BASE_BOARD.AXT_PCI_Rxx05MLIII: info = "PCI bus, Half size, Mechatrolink III 32 axis(DSP Based) controller."; break;
                case AXT_BASE_BOARD.AXT_PCI_Rxx05SIIIH: info = "PCI bus, Half size, Sscnet III / H  32 axis(DSP Based) controller."; break;
                case AXT_BASE_BOARD.AXT_PCI_Rxx05RTEX: info = "PCI bus, Half size, RTEX 32 axis(DSP Based) controller."; break;
                case AXT_BASE_BOARD.AXT_PCI_Rxx05ECAT: info = "PCI bus, Half size, ECAT(DSP Based) controller."; break;
                case AXT_BASE_BOARD.AXT_PCIE_Cxx05RTEX: info = "PCI bus, Half size, ECAT(DSP Based) controller."; break;
                default: info = "Unknown Baseboard"; break;
            }

            return info;
        }
        public void WriteMotionParam()
        {
            try
            {
                //HYEON : Path
                string filePath1 = "C:\\Sineva\\AXT_PARAM.mot";
                string filePath2 = "C:\\Sineva\\AXT_PARAM3.mot";

                using( StreamWriter sw = new StreamWriter( filePath1, false, Encoding.UTF8 ) )
                {
                    StringBuilder sb = new StringBuilder();
                    sw.WriteLine( "### Motion Parameter File ####=======================================================" );
                    foreach( AxtAxis module in m_Modules )
                    {
                        if( sb.Length > 0 ) sb.AppendLine( "#=======================================================" );
                        sb.AppendLine( string.Format( "{0:00}:{1}.={2}", ( ushort )AxtParamItem.AXIS_NO, AxtParamItem.AXIS_NO.ToString(), module.AxisId ) );
                        sb.AppendLine( string.Format( "{0:00}:{1}.={2}", ( ushort )AxtParamItem.PULSE_OUT_METHOD, AxtParamItem.PULSE_OUT_METHOD.ToString(), 4 ) );
                        sb.AppendLine( string.Format( "{0:00}:{1}.={2}", ( ushort )AxtParamItem.ENC_INPUT_METHOD, AxtParamItem.ENC_INPUT_METHOD.ToString(), 3 ) );

                        sb.AppendLine( string.Format( "{0:00}:{1}.={2}", ( ushort )AxtParamItem.INPOSITION, AxtParamItem.INPOSITION.ToString(), 2 ) );    // 1: Use, 2 : NoUse
                        sb.AppendLine( string.Format( "{0:00}:{1}.={2}", ( ushort )AxtParamItem.ALARM, AxtParamItem.ALARM.ToString(), 1 ) );      // 1: Use, 2 : NoUse
                        sb.AppendLine( string.Format( "{0:00}:{1}.={2}", ( ushort )AxtParamItem.NEG_END_LIMIT, AxtParamItem.NEG_END_LIMIT.ToString(), 1 ) );  // 1: Use, 2 : NoUse
                        sb.AppendLine( string.Format( "{0:00}:{1}.={2}", ( ushort )AxtParamItem.POS_END_LIMIT, AxtParamItem.POS_END_LIMIT.ToString(), 2 ) );  // 1: Use, 2 : NoUse

                        sb.AppendLine( string.Format( "{0:00}:{1}.={2:F6}", ( ushort )AxtParamItem.MIN_VELOCITY, AxtParamItem.MIN_VELOCITY.ToString(), 1 ) );
                        sb.AppendLine( string.Format( "{0:00}:{1}.={2:F6}", ( ushort )AxtParamItem.MAX_VELOCITY, AxtParamItem.MAX_VELOCITY.ToString(), 10000 ) );
                        sb.AppendLine( string.Format( "{0:00}:{1}.={2}", ( ushort )AxtParamItem.HOME_SIGNAL, AxtParamItem.HOME_SIGNAL.ToString(), ( uint )AXT_MOTION_HOME_DETECT.NegEndLimit ) );
                        sb.AppendLine( string.Format( "{0:00}:{1}.={2:F6}", ( ushort )AxtParamItem.HOME_FIRST_VELOCITY, AxtParamItem.HOME_FIRST_VELOCITY.ToString(), 1000 + module.AxisId ) );
                        sb.AppendLine( string.Format( "{0:00}:{1}.={2:F6}", ( ushort )AxtParamItem.HOME_SECOND_VELOCITY, AxtParamItem.HOME_SECOND_VELOCITY.ToString(), 100 + module.AxisId ) );
                        sb.AppendLine( string.Format( "{0:00}:{1}.={2:F6}", ( ushort )AxtParamItem.HOME_THIRD_VELOCITY, AxtParamItem.HOME_THIRD_VELOCITY.ToString(), 10 + module.AxisId ) );
                        sb.AppendLine( string.Format( "{0:00}:{1}.={2:F6}", ( ushort )AxtParamItem.HOME_LAST_VELOCITY, AxtParamItem.HOME_LAST_VELOCITY.ToString(), 1 + module.AxisId ) );
                        sb.AppendLine( string.Format( "{0:00}:{1}.={2:F6}", ( ushort )AxtParamItem.HOME_FIRST_ACCEL, AxtParamItem.HOME_FIRST_ACCEL.ToString(), 1000 + module.AxisId ) );
                        sb.AppendLine( string.Format( "{0:00}:{1}.={2:F6}", ( ushort )AxtParamItem.HOME_SECOND_ACCEL, AxtParamItem.HOME_SECOND_ACCEL.ToString(), 1000 + module.AxisId ) );
                        sb.AppendLine( string.Format( "{0:00}:{1}.={2:F6}", ( ushort )AxtParamItem.HOME_END_CLEAR_TIME, AxtParamItem.HOME_END_CLEAR_TIME.ToString(), 1000 + module.AxisId ) );
                        sb.AppendLine( string.Format( "{0:00}:{1}.={2:F6}", ( ushort )AxtParamItem.HOME_END_OFFSET, AxtParamItem.HOME_END_OFFSET.ToString(), 500 ) );
                    }
                    string tmp = sb.ToString();
                    sw.Write( sb.ToString() );
                    sw.Close();
                }

                uint rv = 0;
                rv = CAXM.AxmMotLoadParaAll( filePath1 );
                CAXM.AxmMotSaveParaAll( filePath2 );


                //rv = CAXM.AxmMotSaveParaAll(filePath);
                //rv = CAXM.AxmMotLoadParaAll(filePath2);
                //rv = CAXM.AxmMotSaveParaAll(filePath2);

            }
            catch
            {

            }
        }
        #endregion
    }
}

