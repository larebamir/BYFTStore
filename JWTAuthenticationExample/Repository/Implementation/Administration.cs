using JWTAuthenticationExample.Repository.Interface;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuthenticationExample.Repository.Implementation
{
    public class Administration : IAdministration
    {
        public DataSet ValidateUser(string UserName, string Password)
        {
            DataSet dataSet;
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ValidateUser";
            cmd.Parameters.Add("@UserName", MySqlDbType.VarChar).Value = UserName;
            cmd.Parameters.Add("@Password", MySqlDbType.VarChar).Value = Password;

            return dataSet = DBEngine.GetDataSet(cmd);
        }
    }
}
