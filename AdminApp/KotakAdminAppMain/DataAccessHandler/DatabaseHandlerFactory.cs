using System.Configuration;

namespace KMBL.StepupAuthentication.CoreComponents.DataAccessHandler
{
    public class DatabaseHandlerFactory
    {
        private ConnectionStringSettings connectionStringSettings;

        public DatabaseHandlerFactory(string connectionStringName)
        {
            connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
        }

        public OracleDataAccess CreateDatabase()
        {

            switch (connectionStringSettings.ProviderName.ToLower())
            {
                case "system.data.oracleclient":
                    return new OracleDataAccess(connectionStringSettings.ConnectionString);  
            }
            return null;
        }

        public string GetProviderName()
        {
            return connectionStringSettings.ProviderName;
        }
    }
}
