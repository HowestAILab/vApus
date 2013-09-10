/*
 * Copyright 2007 (c) Sizing Servers Lab
 * Technical University Kortrijk, Department GKG
 *  
 * Author(s):
 *    Vandroemme Dieter
 */
using System;

namespace vApus.Util {
    /// <summary>
    ///     How data will be send.
    /// </summary>
    public enum SendType {
        Bytes = 0,
        Text = 1,
        Binary = 2,
        SOAP = 3
    }

    /// <summary>
    ///     Encoding for SendType.Text or SendType.SOAP.
    /// </summary>
    public enum Encoding {
        ASCII = 0,
        BigEndianUnicode = 1,

        /// <summary>
        ///     Default system encoding, depends on your settings.
        /// </summary>
        Default = 2,
        Unicode = 3,
        UTF32 = 4,
        UTF7 = 5,
        UTF8 = 6
    }

    /// <summary>
    ///     A general class for sending messages from one endpoint to another.
    ///     Note: Use the 'SOAPMessage' class for SOAP, because SOAP does not support generics.
    /// </summary>
    /// <typeparam name="TKey"> Defines what should happen with the content. Can be whatever serializable type you like, ex. an enumeration.</typeparam>
    [Serializable]
    public struct Message<TKey> {

        #region Fields
        /// <summary>
        ///     The actual data you want to transmit.
        /// </summary>
        public object Content;

        /// <summary>
        ///     Defines what should happen with the content. Can be whatever serializable type you like, ex. an enumeration.
        /// </summary>
        public TKey Key;
        #endregion

        #region Constructor
        /// <summary>
        ///     A general class for sending messages from one endpoint to another.
        ///     Note: Use the 'SOAPMessage' class for 'SendType.SOAP', because SOAP does not support generics.
        /// </summary>
        /// <param name="key">Defines what should happen with the content. Can be whatever serializable type you like, ex. an enumeration.</param>
        /// <param name="content">The actual data you want to transmit.</param>
        public Message(TKey key, object content) {
            Key = key;
            Content = content;
        }
        #endregion
    }

    /// <summary>
    ///     A general struct for sending messages from one endpoint to another.
    ///     Works for SendType.Binary too, personally I like the generic approach more.
    /// </summary>
    [Serializable]
    public struct SOAPMessage {

        #region Fields
        /// <summary>
        ///     The actual data you want to transmit.
        /// </summary>
        public object Content;

        /// <summary>
        ///     Defines what should happen with the content.
        /// </summary>
        public string Key;
        #endregion

        #region Constructor
        public SOAPMessage(string key, object content) {
            Key = key;
            Content = content;
        }
        #endregion
    }
}