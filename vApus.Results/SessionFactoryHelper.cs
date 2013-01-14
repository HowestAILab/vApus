/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace vApus.Results
{
    public static class SessionFactoryHelper
    {
        /// <summary>
        /// The session factory is used to create a new session to the database enabeling writing data to it.
        /// </summary>
        /// <param name="server">Must be a resolvable hostname. localhost works also.</param>
        /// <param name="database">This database will be created if it does not exist, it will be cleared if it does.</param>
        /// <param name="username">The given user must have elevated rights.</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static ISessionFactory BuildSessionFactory(string server, string database, string username, string password, bool createDB)
        {
            var model = CreateMappings();

            if (createDB)
                return Fluently.Configure()
                    .Database(MySQLConfiguration.Standard
                    .ConnectionString(c => c
                        .Server(server)
                        .Database(database)
                        .Username(username)
                        .Password(password)))
                    .Mappings(m => m
                        .AutoMappings.Add(model))
                    .ExposeConfiguration(BuildSchema)
                    .BuildSessionFactory();
            return Fluently.Configure()
                .Database(MySQLConfiguration.Standard
                .ConnectionString(c => c
                    .Server(server)
                    .Database(database)
                    .Username(username)
                    .Password(password)))
                .Mappings(m => m
                    .AutoMappings.Add(model))
                .BuildSessionFactory();
        }
        private static AutoPersistenceModel CreateMappings()
        {
            var autoMap = AutoMap
                .Assembly(System.Reflection.Assembly.GetCallingAssembly())
                .Where(t => t.Namespace == "vApus.Results.Model");

            return autoMap;
        }
        private static void BuildSchema(Configuration config)
        {
            new SchemaExport(config).Create(false, true);
        }
    }
}
