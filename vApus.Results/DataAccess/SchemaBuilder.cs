using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vApus.Results
{
    public static class SchemaBuilder
    {
        public static void Build()
        {
            var databaseActions = GetDatabaseActions();
            CreateDatabase(databaseActions);
            databaseActions = GetDatabaseActionsTovApus(databaseActions);
            CreateVersionTable(databaseActions);
            CreateTestsTable(databaseActions);
        }
        private static void CreateDatabase(DatabaseActions databaseActions)
        {
            var databases = databaseActions.GetDataTable("Show Databases");

            foreach(DataRow row in databases.Rows)
                if (row.ItemArray[0].ToString().ToLower() == "vapus")
                    return;

            databaseActions.ExecuteSQL("Create Database vApus");
        }
        private static void CreateVersionTable(DatabaseActions databaseActions)
        {
            if (!TableExists(databaseActions, "Version"))
                databaseActions.ExecuteSQL("Create Table Version(RequiredVersion varchar(20) NOT NULL, PRIMARY KEY(RequiredVersion))");
        }
        private static void CreateTestsTable(DatabaseActions databaseActions)
        {
            if(!TableExists(databaseActions, "Tests"))
                databaseActions.ExecuteSQL("Create Table Tests(TimeStamp datetime NOT NULL AUTO_INCREMENT, PRIMARY KEY(TimeStamp))");
        }

        private static DatabaseActions GetDatabaseActions()
        {
            string user, host, password;
            int port;
            SettingsManager.GetCurrentCredentials(out user, out host, out port, out password);

            return new DatabaseActions
            {
                ConnectionString =
                string.Format("Server={0};Port={1};Uid={2};Pwd={3}", host, port, user, password)
            };
        }
        private static DatabaseActions GetDatabaseActionsTovApus(DatabaseActions databaseActions)
        {
            ReleaseConnection(databaseActions);

            string user, host, password;
            int port;
            SettingsManager.GetCurrentCredentials(out user, out host, out port, out password);

            databaseActions = new DatabaseActions
            {
                ConnectionString =
                string.Format("Server={0};Port={1};Database=vApus;Uid={2};Pwd={3}", host, port, user, password)
            };

            return databaseActions;
        }
        private static void ReleaseConnection(DatabaseActions databaseActions)
        {
            try { databaseActions.ReleaseConnection(); }
            catch { }
            databaseActions = null;
        }

        private static bool TableExists(DatabaseActions databaseActions, string tableName)
        {
            tableName = tableName.ToLower();
            var tables = databaseActions.GetDataTable("Show Tables");
            foreach (DataRow row in tables.Rows)
                if (row.ItemArray[0].ToString().ToLower() == tableName)
                    return true;
            return false;
        }
    }
}
