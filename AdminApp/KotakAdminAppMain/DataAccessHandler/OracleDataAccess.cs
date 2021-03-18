using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace KMBL.StepupAuthentication.CoreComponents.DataAccessHandler
{
    public class OracleDataAccess
    {
        private string ConnectionString { get; set; } 

        public OracleDataAccess(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public OracleConnection CreateConnection()
        {
            return new OracleConnection(ConnectionString);
        }

        public void CloseConnection(OracleConnection connection)
        {
            connection.Close();
            //connection.Dispose();
        }

        public OracleCommand CreateCommand(string commandText, CommandType commandType, OracleConnection connection)
        {
            return new OracleCommand
            {
                CommandText = commandText,
                Connection =  connection,
                CommandType = commandType
            };
        }


        public OracleDataAdapter CreateAdapter(OracleCommand command)
        {
            return new OracleDataAdapter(command);
        }

        public OracleParameter CreateParameter(OracleCommand command)
        {
            return command.CreateParameter();
        }
    }
}
