using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace test_orcl.Models.DataAccess
{
    public class AccountDataAccess
    {

        private string connectionString;

        public AccountDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public UserDocTor GetUser(string username, string password)
        {
            //password = password+username;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                //string query = "SELECT TENUSER, PASSWORD,ID FROM DANGNHAP WHERE TENUSER = :username AND PASSWORD = :password";
                string query = "SELECT HOTEN, DIENTHOAI, MA FROM DMBS WHERE MA = :username AND DIENTHOAI = :password";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    // Sử dụng Add để thêm tham số
                    command.Parameters.Add(new OracleParameter(":username", OracleDbType.Varchar2, username, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter(":password", OracleDbType.Varchar2, password, ParameterDirection.Input));


                    connection.Open();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserDocTor
                            {
                                ID = reader["MA"].ToString(),
                                DIENTHOAI = reader["DIENTHOAI"].ToString() ,
                                HOTEN = reader["HOTEN"].ToString()
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }
    }
}