//Somewhere in 2006, Dieter Vandroemme, mailto:dieter.vandroemme@gmail.com.

/* 19/02/2010 - Yves Wouters, mailto:wouters.yves@gmail.com
 * 
 *      Added a function to return scalars.
 *      Made this class sealed.
 *      
 * TODO:
 *      Named parameters/prepared statements per provider. Is this done already or should I add it? (Or just use Stored Procedures...)
 *      If so: don't use factory pattern, just hardcode everything here!
 * 
 * Notes:
 *      Is getting a datareader under a transaction useless?
 *      This class will only suffice for the most basic needs.
 *          If we want to keep the connection object open, or want to switch databases on the fly, don't use this.
 *          Just keep this as a template. Note that the above two ideas cannot be implemented together here so easily. 
 *          So don't do it!
 *      
 * Usage:
 *      Copy the compiled dll's to the new project folder. Keep this one as an intact template!!!
 * 
 *      Exceptions shouldn't be handled here, just send them up. 
 *      Also no logging here. It is cleaner to handle that in the implementing code.
 *      Make sure to call releaseConnection in the "finally" block of the external code.
 *      Transactions: Get a transaction, store it externally and pass along to executeSQL(),
 *                    getDataTable() or executeSalar(). Handle rollback or commit in external try catch code.
 *      Stored Procedures: Just pass a different CommandType and the name of the SP as the commandText.
 *      If you have no parameters: just pass along a null value. No need to make overloaded functions.
 */

//Stripped for the purpose of simplicity (@ 12/10/2012)

using System;
using System.Data;
using MySql.Data.MySqlClient;
using RandomUtils.Log;

namespace vApus.Util {
    public sealed class DatabaseActions : IDisposable {

        #region Fields

        /// <summary>
        ///     The connection.
        /// </summary>
        private MySqlConnection _connection;
        private int _commandTimeout = 60; //Default 1 minute.

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the connectionstring.
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Default 60 seconds.
        /// </summary>
        public int CommandTimeout {
            get { return _commandTimeout; }
            set { _commandTimeout = value; }
        }
        #endregion

        #region Functions

        public MySqlParameter CreateParameter(string name, object value) { return new MySqlParameter(name, value); }

        #region Connection management

        /// <summary>
        /// Opens and releases a connection. No Errors are logged.
        /// </summary>
        /// <returns></returns>
        public bool CanConnect() {
            try {
                try {
                    GetNewConnection();
                } finally {
                    ReleaseConnection();
                }
                return true;
            } catch {
                return false;
            }
        }

        /// <summary>
        ///     Gets the connection.
        /// </summary>
        private void GetNewConnection() {
            _connection = new MySqlConnection(ConnectionString);
            _connection.Open();
        }

        /// <summary>
        ///     Releases the connection.
        /// </summary>
        public void ReleaseConnection() {
            if (_connection != null) {
                try {
                    _connection.Dispose();
                } catch (Exception ex) {
                    Loggers.Log(Level.Error, "Failed disposing the connection.", ex);
                }
                _connection = null;
            }
        }

        #endregion

        #region Command management

        /// <summary>
        ///     Called by every other buildCommand function.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private MySqlCommand BuildCommand(MySqlConnection connection, string commandText, CommandType commandType, MySqlParameter[] parameters) {
            MySqlCommand command = connection.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = commandText;
            command.CommandTimeout = _commandTimeout;

            if (parameters != null)
                foreach (var parameter in parameters)
                    command.Parameters.Add(parameter);

            return command;
        }

        /// <summary>
        ///     For use without transactions.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private MySqlCommand BuildCommand(string commandText, CommandType commandType, MySqlParameter[] parameters) {
            if (_connection == null) {
                GetNewConnection();
            } else if (_connection.State != ConnectionState.Open || !_connection.Ping()) {
                ReleaseConnection();
                GetNewConnection();
            }

            return BuildCommand(_connection, commandText, commandType, parameters);
        }

        /// <summary>
        ///     With transactions. It takes the connection from the transaction.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private MySqlCommand BuildCommand(MySqlTransaction transaction, string commandText, CommandType commandType, MySqlParameter[] parameters) {
            return BuildCommand(transaction.Connection, commandText, commandType, parameters);
        }

        #endregion

        #region DataAdapter

        /// <summary>
        ///     Gets the according DataAdapter.
        /// </summary>
        /// <returns></returns>
        private MySqlDataAdapter GetNewDataAdapter() { return new MySqlDataAdapter(); }

        #endregion

        #region Without transactions

        /// <summary>
        ///     Executes an SQL string or stored procedure.
        /// </summary>
        /// <param name="commandText">The SQL string or stored procedure name.</param>
        /// <param name="commandType">Text, stored procedure or table direct.</param>
        /// <param name="parameters"></param>
        public void ExecuteSQL(string commandText, CommandType commandType = CommandType.Text, params MySqlParameter[] parameters) {
            BuildCommand(commandText, commandType, parameters).ExecuteNonQuery();
        }

        /// <summary>
        ///     Gets data in a dataTable, recommended for windows apps.
        /// </summary>
        /// <param name="commandText">The SQL string or stored procedure name.</param>
        /// <param name="commandType">Text, stored procedure or table direct.</param>
        /// <param name="parameters"></param>
        /// <returns>An empty datatable if data could not be retreived.</returns>
        public DataTable GetDataTable(string commandText, CommandType commandType = CommandType.Text, params MySqlParameter[] parameters) {
            try {
                var dataAdapter = GetNewDataAdapter();
                dataAdapter.SelectCommand = BuildCommand(commandText, commandType, parameters);

                var dt = new DataTable();
                dataAdapter.Fill(dt);

                return dt;
            } catch (Exception ex) {
                Loggers.Log(Level.Error, "Failed at getting data table", ex);
            }
            return new DataTable();
        }
        //public DataTable GetDataTable(string commandText, CommandType commandType = CommandType.Text, params MySqlParameter[] parameters) {
        //    var dt = new DataTable();
        //    dt.Load(GetDataReader(commandText, commandType, parameters));
        //    return dt;
        //}

        /// <summary>
        ///     Gets data in a dataReader, to execute it requires to close the connection, recommended for web apps.
        /// </summary>
        /// <param name="commandText">The SQL string or stored procedure name.</param>
        /// <param name="commandType">Text, stored procedure or table direct.</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IDataReader GetDataReader(string commandText, CommandType commandType = CommandType.Text, params MySqlParameter[] parameters) {
            return BuildCommand(commandText, commandType, parameters).ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        ///     Execute a SELECT with only one result (single cell)
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText, CommandType commandType = CommandType.Text, params MySqlParameter[] parameters) {
            return BuildCommand(commandText, commandType, parameters).ExecuteScalar();
        }

        #endregion

        #region With transactions

        /// <summary>
        ///     Begins the transaction.
        /// </summary>
        /// <returns></returns>
        public MySqlTransaction BeginTransaction() {
            if (_connection == null) {
                GetNewConnection();
            } else if (_connection.State != ConnectionState.Open || !_connection.Ping()) {
                ReleaseConnection();
                GetNewConnection();
            }
            return _connection.BeginTransaction();
        }

        /// <summary>
        ///     Executes an SQL string or stored procedure in a transaction.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandText">The SQL string or stored procedure name.</param>
        /// <param name="commandType">Text, stored procedure or table direct.</param>
        /// <param name="parameters"></param>
        public void ExecuteSQL(MySqlTransaction transaction, string commandText, CommandType commandType = CommandType.Text, params MySqlParameter[] parameters) {
            var command = BuildCommand(transaction, commandText, commandType, parameters);
            command.Transaction = transaction;
            command.ExecuteNonQuery();
        }

        /// <summary>
        ///     Gets data in a DataTable in a transaction.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandText">The SQL string or stored procedure name.</param>
        /// <param name="commandType">Text, stored procedure or table direct.</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable GetDataTable(MySqlTransaction transaction, string commandText, CommandType commandType = CommandType.Text, params MySqlParameter[] parameters) {
            var command = BuildCommand(transaction, commandText, commandType, parameters);
            command.Transaction = transaction;

            var dataAdapter = GetNewDataAdapter();
            dataAdapter.SelectCommand = command;

            var dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            return dataSet.Tables[0];
        }

        /// <summary>
        ///     Execute a SELECT with only one result (single cell) in a transaction.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(MySqlTransaction transaction, string commandText, CommandType commandType = CommandType.Text, params MySqlParameter[] parameters) {
            var command = BuildCommand(commandText, commandType, parameters);
            command.Transaction = transaction;
            return command.ExecuteScalar();
        }

        #endregion

        /// <summary>
        /// Returns the last inserted ID, unique for the connection.
        /// </summary>
        /// <returns></returns>
        public ulong GetLastInsertId() {
            var dt = GetDataTable("SELECT LAST_INSERT_ID()");
            foreach (DataRow dr in dt.Rows) return (ulong)dr.ItemArray[0];

            return 0;
        }

        public void Dispose() {
            ReleaseConnection();
        }
        #endregion
    }
}