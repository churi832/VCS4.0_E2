using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Text;
using Sineva.VHL.Library.EasySocket.Tools;
using System.Data;


namespace Sineva.VHL.Library.EasySocket
{

    #region Delegates

    /*Some of this selegates are in use at the EasyServer too. */

    /// <summary>
    /// Been called when a communication test begins.
    /// </summary>
    public delegate void ComTestStart_EventHandler();

    /// <summary>
    /// Been called when a communication test ends.
    /// </summary>
    public delegate void ComTestEnd_EventHandler();

    /// <summary>
    /// Been called when the 'DisconnectFromServer' function is called.
    /// </summary>
    public delegate void Disconnecting_EventHandler();

    /// <summary>
    /// Been called when the client disconnected from the server.
    /// </summary>
    public delegate void Disconnected_EventHandler();

    /// <summary>
    /// Been called when the client attempting to connect to the server.
    /// </summary>
    public delegate void Connecting_EventHandler();

    /// <summary>
    /// Been called when the client connected to the server.
    /// </summary>
    public delegate void Connected_EventHandler();

    /// <summary>
    /// Been called when data is sent to the server.
    /// </summary>
    public delegate void DataSent_EventHandler(Object Data);

    /// <summary>
    /// Been called when data is arrived from the server.
    /// </summary>
    public delegate void DataArrived2Client_EventHandler(Object Data);

    //Errors delegates
    //*******************************

    /// <summary>
    /// Been called when a connection attempt to the server is failed,
    /// (Attempted to connect to server for some time without results).
    /// </summary>
    public delegate void ConnectionTimeOutError_EventHandler();

    /// <summary>
    /// Been called when error accures while sending data asynchroniclly using <c>DataSendAsync</c>.
    /// </summary>
    public delegate void DataSendAsyncError_EventHandler(String ErrorMessage);

    /// <summary>
    /// Been called when error accures while testing communication asynchroniclly <c>TestCommunicationAsync</c>.
    /// </summary>
    public delegate void TestComAsyncError_EventHandler(String ErrorMessage);

    #endregion

    /// <summary>
    /// <c>EasyClient</c> is a advanced implementation of the .NET Winsock TCP\IP Client,
    /// It has almost everything build in.
    /// It was developeed and designed with the <c>EasyServer</c> class (Although they work
    /// excellent together and advice to do so, it can workwith any other server implementation).
    /// </summary>
    public sealed class EasyClient : IDisposable
    {
        #region Public members

        //-------------------------------------------------------------------------------
        /// <summary>Representing the status of communication test.</summary>
        public enum TestingStatus
        {
            /// <summary>
            ///Indicates that no communication test was made.
            /// </summary>
            NotTested,
            /// <summary>
            ///Indicates communication test was made and went OK.
            /// </summary>summary>
            TestedOK,
            ///Indicates communication test was made and went wrong.
            TestFailed,
            /// <summary>
            /// Indicates communication test is running.
            /// </summary>
            Testing
        };
        #endregion

        #region EasyServer & EasyClient common data
        /// <summary>
        /// Set of commands to store commands from the server or the client.
        /// Not for client programmers, it's hidden from other users.
        /// </summary>
        private enum EasySocketCommands
        {
            /// <summary>
            /// Informing that the sended\arrived is a communication test
            /// and should be responded with <c>TestComOk</c> command.
            /// </summary>
            TestCom,
            /// <summary>
            /// An OK repsonce to the TestCom command.
            /// </summary>
            TestComOK,
            /// <summary>
            /// This command tells that the sender is disconnecting. 
            /// </summary>
            Disconnecting
        }
        #endregion

        //-------------------------------------------------------------------------------
        #region private members

        /// <summary>
        /// Holds the status of the last communiation test with the server.
        /// </summary>
        private TestingStatus _ComTestStatus = TestingStatus.NotTested;

        /// <summary>The socket to be use during connection and data
        /// transmission wth the server
        /// </summary>
        private Socket clientSocket = null;

        /// <summary>
        /// Holds both the IP end point and the port number of the target server.
        /// (<c>EasyServer</c>'s objects listens by default on port 2222).
        /// </summary>
        private ServerInfo _TargetServerInfo = null;

        /// <summary>
        /// Holds the name of the <c>EasyClient</c> object;
        /// </summary>
        private string ClientName = "Anonymous";

        /// <summary>
        /// Holds the float value of the <c>EasyClient</c> process priority.
        /// Controls the response time of the <c>EasyClient</c> to some events.
        /// View the <c>GeneralPriority</c> propertie for more info.
        /// </summary>
        private float _GeneralPriority = 1;

        private float _ScanSleepTime = 100;

        /// <summary>
        /// The actual priority of executing new data scan, over other system threads.
        /// </summary>
        /// <remarks>Default value is 'Normal'</remarks>
        private ThreadPriority _DataScanPriority = ThreadPriority.Normal;

        /// <summary>The <c>Networkstream</c> of the client's socket.
        /// </summary>
        private NetworkStream NetStream;

        /// <summary>A serialize tool to write objects to the NetStream member.</summary>
        private BinaryFormatter BinaryCaster = new BinaryFormatter();

        /// <summary>
        /// Keeps track of the last time a message was sent to the server.
        /// </summary>
        private DateTime _LastDataSentTime = DateTime.Now;

        /// <summary>
        /// This thread scans for data arrival from the server.
        /// </summary>
        private Thread NewDataScanner = null;

        /// <summary>
        /// Reffrence to object that is used in the <c>SendData</c> async overloads 
        /// functions. This is used to pass data to the <c>SendDataByPass</c> function
        /// without arguments (To activate as a separate thread).
        /// </summary>
        private Object ByPassDataToSend = null;

        /// <summary>
        /// Indicates for function is here current call is asynchronic.
        /// </summary>
        /// <remarks>By defalut is false and get so after every async call.</remarks>
        private bool IsAsyncMode = false;

        /// <summary>
        /// When this flag is off, all the user defined threads, calling <c>Abort()</c>
        /// on them selfs. This is done from <c>Dispose()</c>;
        /// </summary>
        private bool DoThreads = true;

        /// <summary>
        /// Flag is on when using the <c>TestCommunication()</c> asynchroniclly,
        /// and marks it not to throw exceptions but to use the proper event handler instead.
        /// </summary>
        private bool IsTestComAsyncOn = false;

        #endregion

        //-------------------------------------------------------------------------------
        #region Private EventHandlers

        /// <summary>Data arrived event handlers delegate.</summary>
        private DataArrived2Client_EventHandler OnDataArrived;

        /// <summary>Data sent event handlers delegate.</summary>
        private DataSent_EventHandler OnDataSent;

        /// <summary>Connecting to server event handlers delegate.</summary>
        private Connecting_EventHandler OnConnecting;

        /// <summary>Connected to server event handlers delegate.</summary>
        private Connected_EventHandler OnConnected;

        /// <summary>Disconnecting from server event handlers delegate.</summary>
        private Disconnecting_EventHandler OnDisconnecting;

        /// <summary>Disconnected from server event handlers delegate.</summary>
        private Disconnected_EventHandler OnDisconnected;

        /// <summary>Communication test start event handlers delegate.</summary>
        private ComTestStart_EventHandler OnComTestStarted;

        /// <summary>Communication ends start event handlers delegate.</summary>
        private ComTestEnd_EventHandler OnComTestEnded;

        //Errors event handlers

        /// <summary>Connection attempt TimeOut event handler.</summary>
        private ConnectionTimeOutError_EventHandler OnConnectionTimeOut;

        /// <summary>
        /// Been called when error accures while sending data using <c>DataSendAsync</c>.
        /// </summary>
        private DataSendAsyncError_EventHandler OnDataSendAsyncError;

        /// <summary>
        /// Been called when error accures while testing communication asynchroniclly <c>TestCommunicationAsync</c>.
        /// </summary>
        private TestComAsyncError_EventHandler OnTestComAsyncError;

        #endregion

        //---------------------------------------------------------------------------
        #region IDisposable Members

        /// <summary>
        /// Releasing resources
        /// </summary>
        public void Dispose()
        {
            //If this object was disposed before skip disposing.
            if (DoThreads == false) return;

            BinaryCaster = null;

            if (NetStream != null)
            {
                NetStream.Close();
                NetStream = null;
            }

            if (clientSocket != null && clientSocket.Connected)
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }

            //Closing the threads
            if (NewDataScanner != null) CloseThread();

            if( _TargetServerInfo != null ) _TargetServerInfo.Dispose();
            _TargetServerInfo = null;
            ClientName = null;
            clientSocket = null;
            ByPassDataToSend = null;
        }

        //-------------------------------------------------------------------------------

        /// <summary>
        /// This function keep on going till it's make sure all 
        /// the threads are fully stoped.
        /// </summary>
        private void CloseThread()
        {
            //Stoping the infinite threads.
            DoThreads = false;

            /*Making the priority of th threads high so that they will
             * execute the Abort command as soone as possible.*/
            _GeneralPriority = 100000;

            //If the ThreadState is comb' of suspend requested & WaitSleepJoin
            while ((byte)NewDataScanner.ThreadState == 34)
                Thread.Sleep(100);

            //Before aborting the threads I must be sure non of them is in suspend mode
            if (NewDataScanner.ThreadState == ThreadState.Suspended ||
               (byte)NewDataScanner.ThreadState == 96)
                NewDataScanner.Resume();

            while (//Makeing sure no sub thread is active
                  NewDataScanner.ThreadState != ThreadState.Aborted &&
                  NewDataScanner.ThreadState != ThreadState.Stopped &&
                  NewDataScanner.ThreadState != ThreadState.Unstarted
                 )
                Thread.Sleep(100);
        }

        #endregion

        //---------------------------------------------------------------------------
        #region Public properties

        /// <summary>
        /// Indicated weather the <c>EasyClient</c> is ready to connect the server.
        /// If not, the server info is unset or illegal. Use <c>TargetServerInfo</c>
        /// property to solve this problem.
        /// </summary>
        [Category("Misc"), Description("Indicated weather the EasyClient is " +
           " ready to connect the server.")
           ]
        public bool IsReadyToConnect
        {
            get
            {
                return (_TargetServerInfo != null && _TargetServerInfo.IsLegal);
            }
        }
        //--------------------------------------------------------------------------
        /// <summary>
        /// Gets the status of the communication test (Runs from the function <c>TestCommunication</c>)
        /// This test isn't runs by by itself and it is intended only for communication with
        /// <c>EasyServer</c> server type.
        /// </summary>
        [Category("Misc"), Description("Gets the status of the communication test.")]
        public TestingStatus ComTestStatus
        {
            get { return _ComTestStatus; }
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Gets the last time the client sent data to the server or the time that the 
        /// <c>EasyClient</c> object created (if no data was sent).
        /// </summary>
        [Category("Misc"),
       Description("Gets the last time the client sent data to the server or the time that the" +
           " EasyClient object created (if no data was sent).")
       ]
        public DateTime LastTimeDataSent
        {
            get { return _LastDataSentTime; }
        }

        //--------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the information on the server that this
        /// <c>EasyClient</c> will connect to.
        /// </summary>
        /// <exception cref="EasySocketException">
        /// When setting non legal ServerInfo.
        /// </exception>
        [Category("Configurations"),
           Description("Gets or sets the information on the server that this " +
               "EasyClient will connect to.")
       ]
        public ServerInfo TargetServerInfo
        {
            get { return _TargetServerInfo; }
            set
            {
                if (!value.IsLegal)
                    throw new EasySocketException("The server's IP address or name isn't legal.");
                _TargetServerInfo = value;

            }
        }
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the general <c>Priority</c> of this <c>EasyClient</c> process
        /// and controls the response time of the <c>EasyClient</c> to some events.
        /// </summary>
        /// <remarks>
        /// This priority isn't actually priority over other process instead,
        /// it's controls the number of times, the proccess will call during a time sequence.
        /// </remarks>
        [Category("Configurations"),
           Description("Gets or sets the general priority of this EasyClient process " +
                       "and controls the respoce time of the EasyClient to some events.")
        ]
        public Priority GeneralPriority
        {
            get { return PriorityTool.Resolve(_GeneralPriority); }
            set { _GeneralPriority = PriorityTool.GetValue(value); }
        }
        //-------------------------------------------------------------------------------

        public float ScanSleepTime
        {
            get { return _ScanSleepTime; }
            set { _ScanSleepTime = value; }
        }

        /// <summary>
        /// Gets or sets the actual priority of executing new data scan process
        /// over other system threads.
        /// </summary>
        [Category("Configurations"),
       Description("Gets or sets the actual priority of executing new data scan process" +
           " over other system threads..")
       ]
        public ThreadPriority DataScanPriority
        {
            get { return _DataScanPriority; }
            set
            {
                _DataScanPriority = value;
                if (NewDataScanner != null) NewDataScanner.Priority = value;
            }
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of this <c>EasyClient</c> object.
        /// </summary>
        [Category("Misc"),
           Description("Gets or sets the name of this EasyClient object.")
       ]
        public string Name
        {
            get { return ClientName; }
            set
            {
                if (value == null) throw
                      new EasySocketException("EasyClient name cannot be null.");
                ClientName = value;
            }
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Gets the <c>Socket</c> of this <c>EasyClient</c> object.
        /// </summary>
        [Category("Data"),
           Description("Gets the socket of this <c>EasyClient</c> object.")
       ]
        public Socket ClientSocket
        {
            get { return clientSocket; }
        }
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Gets the <c>NetworkStream</c> of the <c>EasyClient</c>'s socket.
        /// </summary>
        [Category("Data"),
       Description("Gets the <c>NetworkStream</c> of the <c>EasyClient</c>'s socket.")
       ]
        public NetworkStream NetworkStream
        {
            get { return NetStream; }
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Indicated whether the <c>EasyClient</c> is connected to a server.
        /// This info is reffrenced to the last data was 
        /// send\received\connection was made so don't trust on it right away
        /// unless you've just send\received\connection was made
        /// (Alternative is to call <c>ConnectionTest</c> function )
        /// </summary>
        [Category("Misc"),
           Description("Indicated whether the EasyClient is connected to a server.")
       ]
        public bool IsConnected
        {
            get { return clientSocket != null && clientSocket.Connected && NetStream != null; }
        }

        #endregion

        //-------------------------------------------------------------------------------
        #region Public events

        /// <summary>Invoked when data is sent from the client to the server.</summary>
        [Description("Invoked when data is sent from the client to the server."),
        Category("Misc")
        ]
        public event DataSent_EventHandler DataSent
        {
            add { OnDataSent += value; }
            remove { OnDataSent -= value; }
        }

        //-------------------------------------------------------------------------------
        /// <summary>Invoked when data is arriving from the client to the client.</summary>
        [Description("Invoked when data is arriving from the client to the client."),
        Category("Misc")
        ]
        public event DataArrived2Client_EventHandler DataArrived
        {
            add { OnDataArrived += value; }
            remove { OnDataArrived -= value; }
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Invoked when a function communication test function starts.
        /// </summary>
        [Description("Invoked when a fuction communication test function starts."),
        Category("Misc")
        ]
        public event ComTestStart_EventHandler ComTestStarted
        {
            add { OnComTestStarted += value; }
            remove { OnComTestStarted -= value; }
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Invoked when a function communication test function ends.
        /// </summary>
        [Description("Invoked when a fuction communication test function ends."),
        Category("Misc")
        ]
        public event ComTestEnd_EventHandler ComTestEnded
        {
            add { OnComTestEnded += value; }
            remove { OnComTestEnded -= value; }
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Invoked when the client attempts to connect the server.
        /// </summary>
        [Description("Invoked when the client attempts to connect the server."),
        Category("Misc")
        ]
        public event Connecting_EventHandler Connecting
        {
            add { OnConnecting = value; }
            remove { OnConnecting -= value; }
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Invoked when the client is successfully connected to the server.
        /// </summary>
        [Description("Invoked when the client is successfully connected to the server."),
        Category("Misc")
        ]
        public event Connected_EventHandler Connected
        {
            add { OnConnected += value; }
            remove { OnConnected -= value; }
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Invoked when the client is begins disconnecting from the server.
        /// </summary>
        [Description("Invoked when the client is begins disconnecting from the server."),
        Category("Misc")
        ]
        public event Disconnecting_EventHandler Disconnecting
        {
            add { OnDisconnecting = value; }
            remove { OnDisconnecting -= value; }
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Invoked when the client is begins disconnecting from the server.
        /// </summary>
        [Description("Invoked when the client is begins disconnecting from the server."),
        Category("Misc")
        ]
        public event Disconnected_EventHandler Disconnected
        {
            add { OnDisconnected += value; }
            remove { OnDisconnected -= value; }
        }
        //-------------------------------------------------------------------------------
        //Errors EventHandlers

        /// <summary>
        /// Involed when a connection attempt to the server is failed,
        /// (Attempted to connect to server for some time without results).
        /// </summary>
        [Description("Involed when a connection attempt to the server is failed " +
            "(Attempted to connect to server for some time without results)."),
         Category("Misc")
        ]
        public event ConnectionTimeOutError_EventHandler ConnectionTimeOut
        {
            add { OnConnectionTimeOut = value; }
            remove { OnConnectionTimeOut -= value; }
        }

        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Invoked when error accures while sending data asynchroniclly using 
        /// <c>DataSendAsync</c> only.
        /// </summary>
        [Description("Invoked when error accures while sending data asynchroniclly using" +
                    " DataSendAsync only."), Category("Misc")
        ]
        public event DataSendAsyncError_EventHandler DataSendAsyncError
        {
            add { OnDataSendAsyncError = value; }
            remove { OnDataSendAsyncError -= value; }
        }
        //-----------------------------------------------------------------------------------
        /// <summary>
        /// Invoked when error accures while testing communication asynchroniclly using 
        /// <c>TestCommunicationAsync</c> only.
        /// </summary>
        [Description("Invoked when error accures while testing communication" +
                    " asynchroniclly using TestCommunicationAsync only."),
        Category("Misc")
        ]
        public event TestComAsyncError_EventHandler TestComAsyncError
        {
            add { OnTestComAsyncError = value; }
            remove { OnTestComAsyncError -= value; }
        }
        #endregion

        //---------------------------------------------------------------------------
        #region Public functions

        /// <summary>
        /// Creates new instance of anonymous <c>EasyClient</c>.
        /// </summary>
        public EasyClient()
        {
            //No data on server to connect to.
            _TargetServerInfo = null;
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Creates new instance of <c>EasyClient</c> with a name.
        /// </summary>
        /// <param name="ClientName">The name of this EasyClient</param>
        public EasyClient(string ClientName)
        {
            //No data on server to connect to.
            _TargetServerInfo = null;
            Name = ClientName;
        }
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Creates new instance of anonymous <c>EasyClient</c>.
        /// </summary>
        /// <param name="TargetServer">The server to connect to.</param>
        public EasyClient(EasyServer TargetServer)
        {
            _TargetServerInfo = TargetServer.ServerInfo;
        }
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Creates new instance of <c>EasyClient</c> with a name.
        /// </summary>
        /// <param name="TargetServer">The server to connect to.</param>
        /// <param name="ClientName">The name of this EasyClient</param>
        public EasyClient(EasyServer TargetServer, string ClientName)
        {
            _TargetServerInfo = TargetServer.ServerInfo;
            Name = ClientName;
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Creates new instance of anonymous <c>EasyClient</c>.
        /// </summary>
        /// <param name="TargetServerInfo">Information about the server to connect to.</param>
        /// <exception cref="EasySocketException">
        /// When the given ServerInfo isn't legal.
        /// </exception>
        public EasyClient(ServerInfo TargetServerInfo)
        {
            if (!TargetServerInfo.IsLegal)
                throw new EasySocketException("The server's IP address or name isn't legal.");
            _TargetServerInfo = TargetServerInfo;
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Creates new instance of <c>EasyClient</c> with a name.
        /// </summary>
        /// <param name="TargetServerInfo">Information about the server to connect to.
        /// </param>
        /// <param name="ClientName">The name of this <c>EasyClient</c></param>
        /// <exception cref="EasySocketException">
        /// When the given ServerInfo isn't legal.
        /// </exception>
        public EasyClient(ServerInfo TargetServerInfo, string ClientName)
        {
            if (!TargetServerInfo.IsLegal)
                throw new EasySocketException("The server's IP address or name isn't legal.");
            _TargetServerInfo = TargetServerInfo;
            Name = ClientName;
        }
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Attempts to create a connection to the server if not connected already.
        /// This function isn't synchronized so the client will not be connected right after
        /// the function (using the <c>SendData()</c> functions is safe though and taking
        /// it under consideration). Any way you can always use the <c>IsConnected</c> property.
        /// </summary>
        ///<exception cref="EasySocketException">
        ///Thrown when the information about the server is
        ///incorrect or unset and on general connection errors.
        ///</exception>
        ///<remarks>
        /// This function calls another asynchronic function that attempts to establish
        /// a connection. Within this function, if no connection made, event handler 
        /// named <c>OnConnectionTimeOut()</c> is called instead of using Exceptions 
        /// (you  better use this one).
        ///</remarks>
        public void ConnectToServerAsync()
        {
            //If the connection was disconnect before, create new socket. 
            if (clientSocket == null)
            {
                clientSocket = new Socket(AddressFamily.InterNetwork,
                                          SocketType.Stream, ProtocolType.Tcp);
            }

            //No need to connect again.
            if (clientSocket.Connected) return;

			if (_TargetServerInfo == null)
			{
				throw new EasySocketException("EasyClient Cannot connect a server, " + "(Set the target server info first).");
			}
			if (_TargetServerInfo.IsLegal == false)
			{
				throw new EasySocketException("EasyClient Cannot connect a server, " + "(target server info is illegal).");
			}
          
            try
            {
                AsyncCallback CallBack = new AsyncCallback(ConnectingToServer);
                if (clientSocket != null)
                    clientSocket.BeginConnect(TargetServerInfo.ServerEndPoint, CallBack, clientSocket);
            }
            catch (Exception ex)
            {
				ExceptionLog.WriteLog(ex.ToString());
//                System.Windows.Forms.MessageBox.Show("Server is not Ready, cann't connection!");
//                throw new EasySocketException("Error, cannot connect to server " +
//                    TargetServerInfo, ex);
            }
        }
        //-------------------------------------------------------------------------------

        /// <summary>
        /// Disconnect from server and closes the client <c>Socket</c> if connected.
        /// </summary>
        public void DisconnectFromServer()
        {
            //Calling event handler
            if (OnDisconnecting != null) OnDisconnecting();

            //Stop scanning for new data (Thread restarts when new connection is made)
            if (NewDataScanner != null)
            {
                if (NewDataScanner.IsAlive)
                {
                    try
                    {
                        NewDataScanner.Suspend();
                    }
                    catch
                    {
						ExceptionLog.WriteLog("DisconnectFromServer Exception Error");
                    }
                }
            }

            //Give the server least 4 second to handle the last data we sent.
            while ((DateTime.Now - _LastDataSentTime).TotalSeconds < 4)
                Thread.Sleep(200);

            try
            {
                if (NetStream != null)
                    NetStream.Close();
                if (clientSocket != null)
                {
                    //clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
            catch (Exception)
			{
				ExceptionLog.WriteLog("Client Close Exception Error");
			}
            finally
            {
                clientSocket = null;
                NetStream = null;
                NetStream = null;
            }

            //Calling event handler
            if (OnDisconnected != null) OnDisconnected();
        }

        //-------------------------------------------------------------------------------

        /// <summary>
        /// This is the asynchronic overload of the function(Not waiting for results).
        /// Disconnect from server and closes the client socket if connected.
        /// </summary>
        public void DisconnectFromServerAsync()
        {
            //Run a separate thread to disconnect
            new Thread(new ThreadStart(DisconnectFromServer)).Start();
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Sending an object to the server.
        /// </summary>
        /// <param name="DataToSend">The object to send to the server.</param>
        /// <remarks>The <c>DataToSend</c> object must be serializable</remarks>
        /// <exception cref="EasySocketException">
        /// Thrown on attempt to send data when client is disconneted, or when
        /// connection was made during attempt to send data.
        /// </exception>
        public void SendData(Object DataToSend, bool serialize = true)
        {
            //VisionLog.WriteLog("CLIENT : SEND - 1");

            ByPassDataToSend = null; //Reseting to default value and getting ready to re-initilezed.

            lock (this)//Not allowing 2 async data sending simultaneously
            {
                //No connection attempt was made.
                if (clientSocket == null)
                {
                    //If this is asynchronic call from ByPassDataToSend() call evet handler instead of exception
                    if (IsAsyncMode)
                    {
                        IsAsyncMode = false; //Returning to default mode.
                        if (OnDataSendAsyncError != null)//Is there event handler call it
                            OnDataSendAsyncError("Error on attempt to send data when the client is disconnected.");
                        return;
                    }//end of inner if 
					else throw new EasySocketException("Error on attempt to send data" +
							  " when the client is disconnected.");
                }//end of if

                //In case we're during connection establishment wait 5 sec' for connection
                for (short i = 0; !IsConnected && i < 25; ++i)
                    Thread.Sleep(200);

                if (!IsConnected)//If still not connected throw exception
                {
					if (IsAsyncMode) //If this is asynchronic call from ByPassDataToSend()
					{
						IsAsyncMode = false; //Returning to default mode.
						if (OnDataSendAsyncError != null) //If there is event handler call it
							OnDataSendAsyncError("Error on attempt to send data during connection establishment.");
						return;
					}
					else return;
					//else throw new EasySocketException("Error on attempt to send data" +
					//	" during connection establishment.");
                }

                try
                {
                    if (serialize)
                    {
                        BinaryCaster.Serialize(NetStream, DataToSend);
                        NetStream.Flush();
                    }
                    else
                    {
                        string msg = (string)DataToSend;
                        byte[] buffer = Encoding.Default.GetBytes(msg);
                        clientSocket.Send(buffer);
                    }
                    //VisionLog.WriteLog("CLIENT : SEND - 2");
                }
                catch (Exception err)
                {
                    string msg = err.ToString();
					ExceptionLog.WriteLog(err.ToString());
//                    System.Windows.Forms.MessageBox.Show("Server is not Ready, cann't send Data!");
                }

                //Updating last send time
                _LastDataSentTime = DateTime.Now;

            }//end of locking block

            //don't invoke if the data was special command
            if (OnDataSent != null && DataToSend.GetType().Name != "EasySocketCommands")
                OnDataSent(DataToSend);
            //VisionLog.WriteLog("CLIENT : SEND - 3");
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// This is the asynchronic overload of the function(Not waiting for results).
        /// </summary>
        /// <param name="DataToSend">The object to send to the server.</param>
        ///<remarks>
        /// Since this function is asynchronic, no exceptions are thrown 
        /// Instead use the <c>OnDataSendAsyncError</c> event handler.
        /// The send object must be serializable. Sending nulled object will take no action.
        ///</remarks>
        public void SendDataAsync(object DataToSend, bool serialize = true)
        {
			if (!IsConnected) return;
            if (DataToSend == null) return;
            if (serialize)
            {
                while (ByPassDataToSend != null) Thread.Sleep(15); //Sleep till ready to use ByPassDataToSend
                                                                   //Run a separate thread to send data
                ByPassDataToSend = DataToSend;
                new Thread(new ThreadStart(SendDataByPass)).Start();
            }
            else
            {
                SendData(DataToSend, false);
            }
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Returns the string representation of the <c>EasyClient</c> Object.
        /// </summary>
        /// <returns>string representation of the <c>EasyClient</c> Object.</returns>
        public override string ToString()
        {
            if (IsReadyToConnect)
            {
                if (Name != "Anonymous")
                    return "Client: " + Name + " set on server: " + TargetServerInfo.ToString();
                else return "Anonymous client set on server: " + TargetServerInfo.ToString();
            }
            else
            {
                if (Name != "Anonymous")
                    return "Client: " + Name + " target server isn't set.";
                else return "Anonymous client " + "target server isn't set.";
            }

        }

        #endregion

        //---------------------------------------------------------------------------
        #region Private functions

        /// <summary>Called when a connection attempt with the server is made.
        /// </summary>
        /// <param name="ConnectionStatus">Information about the connection attempt.
        /// </param>
        private void ConnectingToServer(IAsyncResult ConnectionStatus)
        {
            if (OnConnecting != null) OnConnecting();

            for (byte i = 0; i < 15 && !ConnectionStatus.IsCompleted; ++i)
                Thread.Sleep(500);

			if (clientSocket == null)
				return;

            if (!clientSocket.Connected)//If connection failed, call the proper event
            {
                try { clientSocket.Close(); }
				catch (System.Exception) { ExceptionLog.WriteLog("Client Socket Close Exception Error"); }
                clientSocket = null;
                if (OnConnectionTimeOut != null) OnConnectionTimeOut();
                return;
            }

            try
            {
                //Set the networkStream (Socket must be connected by now)
                if (clientSocket != null)
                    NetStream = new NetworkStream(clientSocket);

                //creates new thread to handle new data comming from server.
                if (NewDataScanner == null)
                {
                    NewDataScanner = new Thread(new ThreadStart(ScanForData));
                    NewDataScanner.Priority = _DataScanPriority;
                    NewDataScanner.Start();
                }
                else
                {	//if the thread cannot be resume, recreate it
                    try { NewDataScanner.Resume(); }
                    catch (Exception)
                    {
                        NewDataScanner = new Thread(new ThreadStart(ScanForData));
                        NewDataScanner.Priority = _DataScanPriority;
                        NewDataScanner.Start();
                    }
                }

                if (OnConnected != null) OnConnected();
            }
            catch
            {
				ExceptionLog.WriteLog("Client Socket ScanForData Exception Error");
            }
        }

        //-------------------------------------------------------------------------------		

        /// <summary>
        /// Sending a spechial command or message to the server with
        /// the <c>SendData</c> function.
        /// </summary>
        /// <remarks>
        /// This should be used only when communicating with <c>EasyServer</c> server type.
        /// </remarks>
        /// <param name="Command">The command\message to the server.</param>
        private void SendCommand(EasySocketCommands Command)
        {
            SendData(Command);
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// This function scans for data dispached from server machine constantly.
        /// </summary>
        private void ScanForData()
        {
            while (true)
            {
                //Give the CPU a break
                //Thread.Sleep((int)(150 / _GeneralPriority));
                Thread.Sleep((int)(1000 / _ScanSleepTime));
                //VisionLog.WriteLog("CLIENT - Sleep = 1");

                if (clientSocket == null)
				{
					return;
				}
                //Requesting to close the thread( when Dispose() is called )
                try 
				{ 
					if (!DoThreads) 
						Thread.CurrentThread.Abort(); 
				}
				catch { ExceptionLog.WriteLog("Client Socket Current Thread Abort Exception Error"); }

                //If Socket isn't connected suspend this thread (resume on reconnect)
                try 
				{ 
					if (!clientSocket.Connected) 
						NewDataScanner.Suspend(); 
				}
				catch { ExceptionLog.WriteLog("Client Socket Data Scanner Suspend Exception Error"); }

				try
				{
					if (clientSocket.Available > 0)//Is there data for us?
					{
						/*If IOexceprion is thrown here, it's probably cos' the server sent data
						 *and closed the connection right away.(To fix let the server wait a bit
						 before closing the connection.*/
                        //VisionLog.WriteLog("CLIENT : RECV - 0");

						byte[] buf = new byte[1000];
						int len = clientSocket.Receive(buf);

                        //VisionLog.WriteLog("CLIENT : RECV - 1");

                        string stringBuf = "";
                        for (int i = 0; i < len; i++) stringBuf += string.Format("{0}", Convert.ToChar(buf[i]));

                        // Message 중복 처리
                        string[] MsgBlocks = null;
                        MsgBlocks = stringBuf.Split(new char[] { '\r' });
                        foreach (string block in MsgBlocks)
                        {
                            if (!string.IsNullOrEmpty(block))
                            {
                                object cmd = (object)(block);
                                OnDataArrived(cmd);
                            }
                        }
                        //MsgBlocks = stringBuf.Split(new char[] { '[' });
                        //foreach (string block in MsgBlocks)
                        //{
                        //    if (block.Contains("]"))
                        //    {
                        //        object cmd = (object)("[" + block);
                        //        OnDataArrived(cmd);
                        //    }
                        //}
                        //VisionLog.WriteLog("CLIENT : RECV - 3");
                    }
				}
				catch
				{
					ExceptionLog.WriteLog("Client Socket Data Send Exception Error");
				}
            }//end of while
        }//end of function

        //-------------------------------------------------------------------------------
        /// <summary>
        /// This function calls the <c>SendData</c> function and sends as an argument
        /// the <c>ByPassDataToSend</c> private member. It's should run as a separate
        /// thraed and by so cating asynchronic version of <c>SendData.</c>
        /// </summary>
        /// <remarks>
        /// <c>ByPassDataToSend</c> should refer the data to be sent.
        /// </remarks>
        private void SendDataByPass()
        {
            SendData(ByPassDataToSend);
            IsAsyncMode = false; //Returning to default mode.
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////
    }
}
