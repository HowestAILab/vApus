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

using System.Data;
using MySql.Data.MySqlClient;

public class DatabaseActions {
    #region Fields

    /// <summary>
    ///     The connection.
    /// </summary>
    private IDbConnection _connection;

    #endregion

    #region Properties

    /// <summary>
    ///     Gets the connectionstring.
    /// </summary>
    public string ConnectionString { get; set; }

    #endregion

    #region Functions

    public IDbDataParameter CreateParameter(string name, object value) {
        return new MySqlParameter(name, value);
    }

    #region Connection management

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
                _connection.Close();
            }
            catch {
            }
            try {
                _connection.Dispose();
            }
            catch {
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
    private IDbCommand BuildCommand(IDbConnection connection, string commandText, CommandType commandType,
                                    IDbDataParameter[] parameters) {
        IDbCommand command = connection.CreateCommand();

        command.CommandType = commandType;
        command.CommandText = commandText;

        if (parameters != null)
            foreach (IDbDataParameter parameter in parameters)
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
    private IDbCommand BuildCommand(string commandText, CommandType commandType, IDbDataParameter[] parameters) {
        if (_connection == null)
            GetNewConnection();

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
    private IDbCommand BuildCommand(IDbTransaction transaction, string commandText, CommandType commandType,
                                    IDbDataParameter[] parameters) {
        IDbConnection connection = transaction.Connection;
        IDbCommand command = BuildCommand(connection, commandText, commandType, parameters);
        return command;
    }

    #endregion

    #region DataAdapter

    /// <summary>
    ///     Gets the according DataAdapter.
    /// </summary>
    /// <returns></returns>
    private IDbDataAdapter GetNewDataAdapter() {
        return new MySqlDataAdapter();
    }

    #endregion

    #region Without transactions

    /// <summary>
    ///     Executes an SQL string or stored procedure.
    /// </summary>
    /// <param name="commandText">The SQL string or stored procedure name.</param>
    /// <param name="commandType">Text, stored procedure or table direct.</param>
    /// <param name="parameters"></param>
    public void ExecuteSQL(string commandText, CommandType commandType = CommandType.Text,
                           params IDbDataParameter[] parameters) {
        IDbCommand oCommand = BuildCommand(commandText, commandType, parameters);
        oCommand.ExecuteNonQuery();
    }

    /// <summary>
    ///     Gets data in a dataTable, recommended for windows apps.
    /// </summary>
    /// <param name="commandText">The SQL string or stored procedure name.</param>
    /// <param name="commandType">Text, stored procedure or table direct.</param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public DataTable GetDataTable(string commandText, CommandType commandType = CommandType.Text,
                                  params IDbDataParameter[] parameters) {
        try {
            IDbCommand command = BuildCommand(commandText, commandType, parameters);
            IDbDataAdapter dataAdapter = GetNewDataAdapter();
            dataAdapter.SelectCommand = command;

            var dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            return dataSet.Tables[0];
        }
        catch { }
        return new DataTable();
    }

    /// <summary>
    ///     Gets data in a dataReader, to execute it requires to close the connection, recommended for web apps.
    /// </summary>
    /// <param name="commandText">The SQL string or stored procedure name.</param>
    /// <param name="commandType">Text, stored procedure or table direct.</param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public IDataReader GetDataReader(string commandText, CommandType commandType = CommandType.Text,
                                     params IDbDataParameter[] parameters) {
        IDbCommand command = BuildCommand(commandText, commandType, parameters);
        IDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);

        return dataReader;
    }

    /// <summary>
    ///     Execute a SELECT with only one result (single cell)
    /// </summary>
    /// <param name="commandText"></param>
    /// <param name="commandType"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public object ExecuteScalar(string commandText, CommandType commandType = CommandType.Text,
                                params IDbDataParameter[] parameters) {
        IDbCommand command = BuildCommand(commandText, commandType, parameters);
        return command.ExecuteScalar();
    }

    #endregion

    #region With transactions

    /// <summary>
    ///     Begins the transaction.
    /// </summary>
    /// <returns></returns>
    public IDbTransaction BeginTransaction() {
        if (_connection == null)
            GetNewConnection();
        return _connection.BeginTransaction();
    }

    /// <summary>
    ///     Executes an SQL string or stored procedure in a transaction.
    /// </summary>
    /// <param name="transaction"></param>
    /// <param name="commandText">The SQL string or stored procedure name.</param>
    /// <param name="commandType">Text, stored procedure or table direct.</param>
    /// <param name="parameters"></param>
    public void ExecuteSQL(IDbTransaction transaction, string commandText, CommandType commandType = CommandType.Text,
                           params IDbDataParameter[] parameters) {
        IDbCommand command = BuildCommand(transaction, commandText, commandType, parameters);
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
    public DataTable GetDataTable(IDbTransaction transaction, string commandText,
                                  CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters) {
        IDbCommand command = BuildCommand(transaction, commandText, commandType, parameters);
        command.Transaction = transaction;

        IDbDataAdapter dataAdapter = GetNewDataAdapter();
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
    public object ExecuteScalar(IDbTransaction transaction, string commandText,
                                CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters) {
        IDbCommand command = BuildCommand(commandText, commandType, parameters);
        command.Transaction = transaction;
        return command.ExecuteScalar();
    }

    #endregion

    #endregion
}