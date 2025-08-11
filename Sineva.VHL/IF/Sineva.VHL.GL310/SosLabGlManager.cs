using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Linq;
using Sineva.VHL.Library;

namespace Sineva.VHL.IF.GL310
{
    public partial class SosLabGlManager
    {
        #region Singleton
        public static readonly SosLabGlManager Instance = new SosLabGlManager();
        #endregion

        #region Fields
        //private ThreadFunction _MonitorThread;
        private bool m_Connected = false;
        private bool m_UpdateValue = false;

        private IntPtr m_Core = IntPtr.Zero;
        private IntPtr m_User = IntPtr.Zero;
        #endregion

        #region Properties
        public bool Connected
        {
            get { return m_Connected; }
            set { m_Connected = value; }
        }
        public bool UpdateValue
        {
            get { return m_UpdateValue; }
            set { m_UpdateValue = value; }
        }
        public IntPtr Core
        {
            get { return m_Core; }
            set { m_Core = value; }
        }
        public IntPtr User
        {
            get { return m_User; }
            set { m_User = value; }
        }
        #endregion

        #region Events
        #endregion

        #region Constructor
        private SosLabGlManager()
        {

        }
        #endregion

        #region Method
        public bool Initialize()
        {
            return true;
            try
            {
                m_Core = GL5LibraryCORE.GL3_CORE_createInstance();
                m_User = GL5LibraryUSER.GL3_USER_createInstance();
                m_Connected = GL5LibraryCORE.GL3_CORE_connectUDP(m_Core, "10.110.1.2", 2000, "10.110.1.3", 3000);

                // Serial Number Check
                IntPtr serialnum = Marshal.AllocHGlobal(16);
                UIntPtr serialnum_size = new UIntPtr(0);
                m_Connected &= GL5LibraryUSER.GL3_USER_getSerialNum(m_Core, m_User, serialnum, ref serialnum_size);
                if (m_Connected)
                {
                    string serialnumStr = Marshal.PtrToStringAnsi(serialnum, (int)serialnum_size.ToUInt32());
                    Marshal.FreeHGlobal(serialnum);
                    Console.WriteLine("Serial_num = " + serialnumStr);

                    m_Connected &= GL5LibraryUSER.GL3_USER_setStreamEnable(m_Core, m_User, true);
                }
                else
                {
                    Console.WriteLine("Unable to get a serial number");
                    Exit();
                }

                //SeqMonitor monitor = new SeqMonitor(this);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
                Exit();
            }
            return m_Connected;
        }
        public void Exit()
        {
            try
            {
                m_Connected = false;
                GL5LibraryUSER.GL3_USER_setStreamEnable(m_Core, m_User, false);
                GL5LibraryCORE.GL3_CORE_disconnect(m_Core);
                GL5LibraryCORE.GL3_CORE_releaseInstance(m_Core);
                GL5LibraryUSER.GL3_USER_releaseInstance(m_User);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }
        #endregion

        #region Sequence
        private class SeqMonitor : XSeqFunc
        {
            #region Field
            SosLabGlManager m_Manager = null;
            #endregion

            #region Constructor
            public SeqMonitor(SosLabGlManager manager)
            {
                this.SeqName = $"SeqMonitor_SosLabGlManager";
                m_Manager = manager;
                StartTicks = XFunc.GetTickCount();

                TaskGlControl.Instance.RegSeq(this);
            }
            #endregion

            #region Override
            public override int Do()
            {
                if (!m_Manager.Connected) return -1;

                int seqNo = SeqNo;

                switch (seqNo)
                {
                    case 0:
                        {
                            // Real Time Monitoring
                            //m_Manager.SensorMonitorThread();
                            try
                            {
                                UIntPtr data_size = new UIntPtr(0);
                                double[] distance = new double[2048];
                                double[] pulsewidth = new double[2048];
                                double[] angle = new double[2048];
                                double[] x = new double[2048];
                                double[] y = new double[2048];
                                UIntPtr input_area = new UIntPtr(0);
                                UIntPtr output_level = new UIntPtr(0);
                                UIntPtr error_bit = new UIntPtr(0);
                                UIntPtr dist_offset = new UIntPtr(0);
                                UIntPtr backreflector_pulse_width = new UIntPtr(0);
                                UIntPtr pd_hv = new UIntPtr(0);
                                UIntPtr ld_hv = new UIntPtr(0);
                                UIntPtr pd_temp = new UIntPtr(0);
                                UIntPtr ld_temp = new UIntPtr(0);

                                if (GL5LibraryUSER.GL3_USER_getLidarData(m_Manager.Core, m_Manager.User, ref data_size, distance, pulsewidth, angle, x, y, ref input_area, ref output_level, ref error_bit, ref dist_offset, ref backreflector_pulse_width, ref pd_hv, ref ld_hv, ref pd_temp, ref ld_temp, true))
                                {
                                    List<double> datas = new List<double>();
                                    //Console.WriteLine(data_size + " frames are received");
                                    //SequenceLog.WriteLog("SosLabGlManager frames are received");
                                    if (data_size.ToUInt64() > 0)
                                    {
                                        for (ulong i = 0; i < data_size.ToUInt64(); i++)
                                        {
                                            datas.Add(distance[i]);
                                            //Console.WriteLine("distance[{0}] = {1}", i, distance[i]);
                                            //Console.WriteLine("pulse_width[{0}] = {1}", i, pulsewidth[i]);
                                            //Console.WriteLine("angle[{0}] = {1}", i, angle[i]);
                                            //Console.WriteLine("x[{0}] = {1}", i, x[i]);
                                            //Console.WriteLine("y[{0}] = {1}\n", i, y[i]);
                                        }
                                        //Console.WriteLine("input_area = {0}", input_area);
                                        //Console.WriteLine("output_level = {0}", output_level);
                                        //Console.WriteLine("error_bit = {0}", error_bit);
                                        //Console.WriteLine("dist_offset = {0}", dist_offset);
                                        //Console.WriteLine("backreflector_pulse_width = {0}", backreflector_pulse_width);
                                        //Console.WriteLine("pd_hv = {0}", pd_hv);
                                        //Console.WriteLine("ld_hv = {0}", ld_hv);
                                        //Console.WriteLine("pd_temp = {0}", pd_temp);
                                        //Console.WriteLine("ld_temp = {0}", ld_temp);

                                        double min_distance = datas.Min();
                                        if (min_distance < 10.0f) min_distance = 10.0f;
                                        EventHandlerManager.Instance.InvokeSensorPv(0, min_distance);

                                        m_Manager.UpdateValue = true;
                                    }
                                }
                                else
                                {
                                    m_Manager.UpdateValue = false;
                                }
                            }
                            catch(Exception ex)
                            {
                                ExceptionLog.WriteLog(ex.ToString());
                            }
                            finally
                            {
                                seqNo = 0;
                            }
                        }
                        break;
                }

                SeqNo = seqNo;
                return -1;
            }
            #endregion
        }
        #endregion
    };
}
