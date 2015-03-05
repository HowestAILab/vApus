/*
 * Copyright 2007 (c) Sizing Servers Lab
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Threading;

namespace vApus.Util {
    /// <summary>
    ///     This class provides functionality to connect to an endpoint and send data to and receive data from it.
    ///     This works with every type of socket and only one method is needed to send or receive.
    ///     This can be simply bytewise, binary, soap or text.
    /// </summary>
    public class SocketWrapper {

        #region Fields

        /// <summary>
        ///     The default buffer size in bytes.
        /// </summary>
        public const int DEFAULTBUFFERSIZE = 8912;

        private static object _lock = new object();
        public readonly IPAddress IP;
        public readonly int Port;

        private readonly ManualResetEvent _connectWaitHandle;

        private byte[] _buffer;
        private SocketFlags _receiveSocketFlags = SocketFlags.None;
        private EndPoint _remoteEP;
        private SocketFlags _sendSocketFlags = SocketFlags.None;
        private Socket _socket;

        #endregion

        #region Properties

        /// <summary>The inner socket.</summary>
        public Socket Socket {
            get { return _socket; }
            set {
                if (value == null)
                    throw new ArgumentNullException("socket");
                _socket = value;
            }
        }

        /// <summary></summary>
        public byte[] Buffer {
            get { return _buffer; }
            set { _buffer = value; }
        }

        /// <summary></summary>
        public SocketFlags SendSocketFlags {
            get { return _sendSocketFlags; }
            set { _sendSocketFlags = value; }
        }

        /// <summary></summary>
        public SocketFlags ReceiveSocketFlags {
            get { return _receiveSocketFlags; }
            set { _receiveSocketFlags = value; }
        }

        /// <summary>
        ///     The receive time-out value, in milliseconds. If you set the property with a value between 1 and 499, the value will be changed to 500. The default value is 0, which indicates an infinite time-out period. Specifying -1 also indicates an infinite time-out period.
        ///     Make sure the socket is not null.
        ///     This is a socket property, so you should set it when you create one.
        /// </summary>
        public int ReceiveTimeout {
            get { return _socket.ReceiveTimeout; }
            set { _socket.ReceiveTimeout = value; }
        }

        /// <summary>
        ///     The send time-out value, in milliseconds. If you set the property with a value between 1 and 499, the value will be changed to 500. The default value is 0, which indicates an infinite time-out period. Specifying -1 also indicates an infinite time-out period.
        ///     Make sure the socket is not null.
        ///     This is a socket property, so you should set it when you create one.
        /// </summary>
        public int SendTimeout {
            get { return _socket.SendTimeout; }
            set { _socket.SendTimeout = value; }
        }

        /// <summary>
        ///     Gets or sets the receive buffer size. Make sure the socket is not null.
        ///     This is a socket property, so you should set it when you create one.
        /// </summary>
        public int ReceiveBufferSize {
            get { return _socket.ReceiveBufferSize; }
            set { _socket.ReceiveBufferSize = value; }
        }

        /// <summary>
        ///     Gets or sets the send buffer size. Make sure the socket is not null.
        ///     This is a socket property, so you should set it when you create one.
        /// </summary>
        public int SendBufferSize {
            get { return _socket.SendBufferSize; }
            set { _socket.SendBufferSize = value; }
        }

        /// <summary>
        ///     Check if the socket is connected.
        ///     This is blocking meaning if you call this property from an other thread while connecting the result will be returned after completion.
        /// </summary>
        public bool Connected {
            get {
                _connectWaitHandle.WaitOne();
                return (_socket != null && _socket.Connected);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     This class provides functionality to connect to an endpoint and send data to and receive data from it.
        ///     This works with every type of socket and only one method is needed to send or receive.
        ///     This can be simply bytewise, binary, soap or text.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="socket"></param>
        /// <param name="connectTimeout">If smaller then or equals 0, the timeout is infinite.</param>
        /// <param name="receiveSocketFlags"></param>
        /// <param name="sendSocketFlags"></param>
        public SocketWrapper(string ip, int port, Socket socket, SocketFlags receiveSocketFlags = SocketFlags.None,
                             SocketFlags sendSocketFlags = SocketFlags.None)
            : this(IPAddress.Parse(ip), port, socket, receiveSocketFlags, sendSocketFlags) {
        }

        /// <summary>
        ///     This class provides functionality to connect to an endpoint and send data to and receive data from it.
        ///     This works with every type of socket and only one method is needed to send or receive.
        ///     This can be simply bytewise, binary, soap or text.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="socket"></param>
        /// <param name="connectTimeout">If smaller then or equals 0, the timeout is infinite.</param>
        /// <param name="receiveSocketFlags"></param>
        /// <param name="sendSocketFlags"></param>
        public SocketWrapper(IPAddress ip, int port, Socket socket, SocketFlags receiveSocketFlags = SocketFlags.None,
                             SocketFlags sendSocketFlags = SocketFlags.None) {
            if (ip == null)
                throw new ArgumentNullException("ip");
            if (port < 0)
                throw new ArgumentOutOfRangeException("port", "A port can not be negative");
            if (socket == null)
                throw new ArgumentNullException("socket");

            // Establish the remote endpoint for the socket.
            IP = ip;
            Port = port;
            _remoteEP = new IPEndPoint(ip, port);
            _socket = socket;
            _sendSocketFlags = sendSocketFlags;
            _receiveSocketFlags = receiveSocketFlags;
            _connectWaitHandle = new ManualResetEvent(true);
        }

        #endregion

        #region Functions

        #region Connect and close

        /// <summary>
        ///     Connects to a socket.
        ///     Throws an exception if it is not able too.
        ///     You must check the connected property first before calling this.
        /// </summary>
        public void Connect() { Connect(0); }

        /// <summary>
        ///     Connects to a socket.
        ///     Throws an exception if it is not able too.
        ///     You must check the connected property first before calling this.
        /// </summary>
        /// <param name="connectTimeout">In ms. If smaller then or equals 0, the timeout is infinite. If a timeout is given, connecting will happen async</param>
        /// <param name="retries">If the timeout isn't sufficient you can set a retry count.</param>
        public void Connect(int connectTimeout, int retries = 0) {
            Exception exception = null;
            for (int i = 0; i != retries + 1; i++)
                try {
                    _connectWaitHandle.Reset();
                    exception = null;
                    if (connectTimeout < 1) {
                        _socket.Connect(_remoteEP);
                        _connectWaitHandle.Set();
                    } else {
                        //Connect async to the remote endpoint.
                        _socket.BeginConnect(_remoteEP, ConnectCallback, _socket);
                        //Use a timeout to connect.
                        _connectWaitHandle.WaitOne(connectTimeout, false);
                        if (!_socket.Connected)
                            throw new Exception("Connecting to the server timed out.");
                    }
                    break;
                } catch (Exception ex) {
                    //Wait for the end connect call.
                    //_connectWaitHandle.WaitOne(connectTimeout, false);

                    //Reuse the socket for re-trying to connect.
                    try {
                        if (_socket.Connected)
                            _socket.Disconnect(true);
                    } catch {
                        //Ignore.
                    }
                    _socket = new Socket(_socket.AddressFamily, _socket.SocketType, _socket.ProtocolType);


                    exception = ex;
                }

            _connectWaitHandle.Set();

            if (exception != null) throw exception;
        }

        /// <summary>
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallback(IAsyncResult ar) {
            try {
                if (_socket.Connected)
                    _socket.EndConnect(ar);
            } catch {
                //Ignore. You will know if the socket is not connected.
            }
            _connectWaitHandle.Set();
        }

        /// <summary>Releases and disposes the socket.</summary>
        public void Close() {
            if (_socket != null) {
                try {
                    _connectWaitHandle.Set();
                    if (_socket.Connected)
                        _socket.Disconnect(false);
                    _socket.Close();
                } catch {
                    //Ignore
                } finally {
                    try { _socket = null; } catch {
                        //Ignore.
                    }
                }
            }
        }

        #endregion

        #region Send

        #region Base send

        /// <summary>
        /// </summary>
        /// <param name="data">Can be any (serializable) object.</param>
        /// <param name="sendType">Binary, Bytes (if you can possibly find an other encoding that is not in here), SOAP or Text (normal string).</param>
        /// <param name="encoding">Only for 'SendType.Text' and 'SendType.SOAP' (is encoded as a string too), this will be ignored in the other cases.</param>
        public void Send(object data, SendType sendType, Encoding encoding = Encoding.Default) {
            switch (sendType) {
                case SendType.Binary:
                    SendBinary(data);
                    break;
                case SendType.Bytes:
                    SendBytes(data as byte[]);
                    break;
                case SendType.SOAP:
                    SendSoap(data, encoding);
                    break;
                case SendType.Text:
                    SendText(data as string, encoding);
                    break;
            }
        }

        #endregion

        #region Bytes send

        /// <summary>
        ///     Sends bytes.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="direct"></param>
        public void SendBytes(byte[] data) {
            _socket.SendTo(data, _sendSocketFlags, _remoteEP);
        }

        #endregion

        #region Object send

        #region Binary

        public byte[] ObjectToByteArray(object obj) {
            byte[] buffer = null;

            //Set the initial buffer size to 1 byte (default == 256 bytes), this way we do not have '\0' bytes in buffer.
            using (var ms = new MemoryStream(1)) {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);

                //.ToArray() was also possible (no '\0' bytes) but this makes a copy of the buffer and results in having twice the buffer in memory.
                buffer = ms.GetBuffer();
                bf = null;
            }
            return buffer;
        }

        /// <summary>
        ///     Convert an object to a byte[] using deflate or gzip compression. Use gzip only for text, deflate for everything else.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="deflate">false for gzip compression.</param>
        /// <returns></returns>
        ////public byte[] ObjectToByteArray(object obj, bool deflate = true) {
        ////    return deflate ? ObjectToByteArrayDeflate(obj) : ObjectToByteArrayGZip(obj);
        ////}
        public byte[] ObjectToByteArrayDeflate(object obj) {
            byte[] buffer = null;

            //Set the initial buffer size to 1 byte (default == 256 bytes), this way we do not have '\0' bytes in buffer.
            using (var ms = new MemoryStream(1)) {
                var bf = new BinaryFormatter();
                using (var dfStream = new DeflateStream(ms, CompressionLevel.Optimal))
                    bf.Serialize(dfStream, obj);

                //.ToArray() was also possible (no '\0' bytes) but this makes a copy of the buffer and results in having twice the buffer in memory.
                buffer = ms.GetBuffer();
                bf = null;
            }
            return buffer;
        }
        public byte[] ObjectToByteArrayGZip(object obj) {
            byte[] buffer = null;

            //Set the initial buffer size to 1 byte (default == 256 bytes), this way we do not have '\0' bytes in buffer.
            using (var ms = new MemoryStream(1)) {
                var bf = new BinaryFormatter();
                using (var dfStream = new GZipStream(ms, CompressionLevel.Optimal))
                    bf.Serialize(dfStream, obj);

                //.ToArray() was also possible (no '\0' bytes) but this makes a copy of the buffer and results in having twice the buffer in memory.
                buffer = ms.GetBuffer();
                bf = null;
            }
            return buffer;
        }


        /// <summary>
        ///     Sends an object binary.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="direct"></param>
        private void SendBinary(object data) {
            SendBytes(ObjectToByteArray(data));
        }

        #endregion

        #region Soap

        /// <summary>
        ///     Converts an object to a soapstring.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string ObjectToSoap(object obj) {
            //TODO: surrogate for non-serializable classes?
            var sf = new SoapFormatter();
            var ms = new MemoryStream();
            sf.Serialize(ms, obj);
            _buffer = new byte[ms.Capacity];
            ms.Seek(0, 0);
            ms.Read(_buffer, 0, _buffer.Length);
            //TODO: The stream can be transformed to a string correctly with every encoding type, except BigEndianUnicod, Unicode and UTF32.
            //		Check if ASCII encoding always works.
            return System.Text.Encoding.ASCII.GetString(_buffer, 0, _buffer.Length);
        }

        /// <summary>
        ///     Sends an object via Soap, make sure it and its childobjects are serializable.
        ///     Encoding for converting to bytes must be given, because soap is a string.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        private void SendSoap(object data, Encoding encoding) {
            SendText(ObjectToSoap(data), encoding);
        }

        #endregion

        #endregion

        #region Text send

        /// <summary>
        ///     Encodes a string to a byte[] using the given encoding.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public byte[] Encode(string data, Encoding encoding) {
            switch (encoding) {
                case Encoding.ASCII:
                    return System.Text.Encoding.ASCII.GetBytes(data);
                case Encoding.BigEndianUnicode:
                    return System.Text.Encoding.BigEndianUnicode.GetBytes(data);
                case Encoding.Default:
                    return System.Text.Encoding.Default.GetBytes(data);
                case Encoding.Unicode:
                    return System.Text.Encoding.Unicode.GetBytes(data);
                case Encoding.UTF32:
                    return System.Text.Encoding.UTF32.GetBytes(data);
                case Encoding.UTF7:
                    return System.Text.Encoding.UTF7.GetBytes(data);
                case Encoding.UTF8:
                    return System.Text.Encoding.UTF8.GetBytes(data);
            }
            return null;
        }

        /// <summary>
        ///     Sends an encoded string. TODO: use GZIP compression.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <param name="direct"></param>
        private void SendText(string data, Encoding encoding) {
            SendBytes(Encode(data, encoding));
        }

        #endregion

        #endregion

        #region Receive

        #region Base receive

        /// <summary>
        /// </summary>
        /// <param name="sendType">Binary, Bytes (if you can possibly find an other encoding that is not in here), SOAP or Text (normal string).</param>
        /// <param name="encoding">Only for 'SendType.Text' and 'SendType.SOAP' (is encoded as a string too), this will be ignored in the other cases.</param>
        /// <returns></returns>
        public object Receive(SendType sendType, Encoding encoding = Encoding.Default) {
            object data = null;
            switch (sendType) {
                case SendType.Binary:
                    data = ReceiveBinary();
                    break;
                case SendType.Bytes:
                    data = ReceiveBytes();
                    break;
                case SendType.SOAP:
                    data = ReceiveSoap(encoding);
                    break;
                case SendType.Text:
                    data = ReceiveText(encoding);
                    break;
            }
            return data;
        }

        #endregion

        #region Bytes receive

        /// <summary>
        ///     Receives bytes.
        /// </summary>
        /// <returns></returns>
        public byte[] ReceiveBytes() {
            _buffer = new byte[_socket.ReceiveBufferSize];
            // Read data from the remote device.
            _socket.ReceiveFrom(_buffer, _receiveSocketFlags, ref _remoteEP);
            return _buffer;
        }

        #endregion

        #region Object receive

        #region Binary

        public object ByteArrayToObject(byte[] buffer, bool deflate = true) {
            object obj = null;

            using (var ms = new MemoryStream(buffer)) {
                var bf = new BinaryFormatter();
                obj = bf.Deserialize(ms);

                bf = null;
            }
            return obj;
        }

        /// <summary>
        ///     Converts a byte[] to an object using deflate of gzip compression. Use gzip only for text, deflate for everything else.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="deflate">false for gzip compression.</param>
        /// <returns></returns>
        //public object ByteArrayToObject(byte[] buffer, bool deflate = true) {
        //    return deflate ? ByteArrayToObjectDeflate(buffer) : ByteArrayToObjectGzip(buffer);
        //}
        private object ByteArrayToObjectDeflate(byte[] buffer) {
            object obj = null;

            using (var ms = new MemoryStream(buffer)) {
                var bf = new BinaryFormatter();
                using (var dfStream = new DeflateStream(ms, CompressionMode.Decompress))
                    obj = bf.Deserialize(dfStream);

                bf = null;
            }
            return obj;
        }
        private object ByteArrayToObjectGzip(byte[] buffer) {
            object obj = null;

            using (var ms = new MemoryStream(buffer)) {
                var bf = new BinaryFormatter();
                using (var dfStream = new GZipStream(ms, CompressionMode.Decompress))
                    obj = bf.Deserialize(dfStream);

                bf = null;
            }
            return obj;
        }

        /// <summary>
        ///     Receives binary.
        /// </summary>
        /// <returns></returns>
        private object ReceiveBinary() {
            ReceiveBytes();
            return ByteArrayToObject(_buffer);
        }

        #endregion

        #region Soap

        /// <summary>
        ///     Converts a soapstring to an object.
        /// </summary>
        /// <param name="soap"></param>
        /// <returns></returns>
        private object SoapToObject(string soap) {
            //TODO: surrogate for non-serializable classes?
            var sf = new SoapFormatter();
            var ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(soap));
            return sf.Deserialize(ms);
        }

        /// <summary>
        ///     Receives soap.
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private object ReceiveSoap(Encoding encoding) {
            return SoapToObject(Decode(ReceiveBytes(), encoding));
        }

        #endregion

        #endregion

        #region Text receive

        /// <summary>
        ///     Decodes a byte[] to a string using the given encoding.  TODO: use GZIP compression.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string Decode(byte[] buffer, Encoding encoding) {
            switch (encoding) {
                case Encoding.ASCII:
                    return System.Text.Encoding.ASCII.GetString(buffer).Trim('\0');
                case Encoding.BigEndianUnicode:
                    return System.Text.Encoding.BigEndianUnicode.GetString(buffer).Trim('\0');
                case Encoding.Default:
                    return System.Text.Encoding.Default.GetString(buffer).Trim('\0');
                case Encoding.Unicode:
                    return System.Text.Encoding.Unicode.GetString(buffer).Trim('\0');
                case Encoding.UTF32:
                    return System.Text.Encoding.UTF32.GetString(buffer).Trim('\0');
                case Encoding.UTF7:
                    return System.Text.Encoding.UTF7.GetString(buffer).Trim('\0');
                case Encoding.UTF8:
                    return System.Text.Encoding.UTF8.GetString(buffer).Trim('\0');
            }
            return null;
        }

        /// <summary>
        ///     Receives a string.
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private string ReceiveText(Encoding encoding) {
            return Decode(ReceiveBytes(), encoding);
        }

        #endregion

        #endregion

        #endregion
    }
}