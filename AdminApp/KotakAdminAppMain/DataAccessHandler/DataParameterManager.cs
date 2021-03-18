using System.Data;
using Oracle.ManagedDataAccess.Client;


namespace KMBL.StepupAuthentication.CoreComponents.DataAccessHandler
{ 
    public class DataParameterManager
    {
        public static OracleParameter CreateParameter(string providerName, string name, object value, OracleDbType dbType, ParameterDirection direction = ParameterDirection.Input)
        {
             switch (providerName.ToLower())
            {
                case "system.data.oracleclient":
                    return CreateOracleParameter(name, value, dbType, direction);
            }

            return null;
        }

        public static OracleParameter CreateParameter(string providerName,string name, OracleDbType dbType, ParameterDirection direction)
        {
            switch (providerName.ToLower())
            {
                case "system.data.oracleclient":
                    return CreateOracleParameter(name, dbType, direction);
            }

            return null;
        }

        public static OracleParameter CreateParameter(string providerName, string name, int size, object value, OracleDbType dbType, ParameterDirection direction = ParameterDirection.Input)
        {
            switch (providerName.ToLower())
            {
                case "system.data.oracleclient":
                    return CreateOracleParameter(name, size, value, dbType, direction);
            }

            return null;
        }

        private static OracleParameter CreateOracleParameter(string name, OracleDbType dbType, ParameterDirection direction)
        {
            return new OracleParameter
            {
                OracleDbType = dbType,
                ParameterName = name,
                Direction = direction,
            };
        }



        private static OracleParameter CreateOracleParameter(string name, object value, OracleDbType dbType, ParameterDirection direction)
        {
            return new OracleParameter
            {
                OracleDbType = dbType,
                ParameterName = name,
                Direction = direction,
                Value = value
            };
        }

        private static OracleParameter CreateOracleParameter(string name, int size, object value, OracleDbType dbType, ParameterDirection direction)
        {
            return new OracleParameter
            {
                OracleDbType = dbType,
                Size = size,
                ParameterName = name,
                Direction = direction,
                Value = value
            };
        }

        private static OracleParameter CreateOracleParameter(string name, int size, OracleDbType dbType, ParameterDirection direction)
        {
            return new OracleParameter
            {
                OracleDbType = dbType,
                Size = size,
                ParameterName = name,
                Direction = direction
                
            };
        }
    }
}
