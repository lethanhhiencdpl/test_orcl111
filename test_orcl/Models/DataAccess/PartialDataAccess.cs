using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using test_orcl.Models;
using test_orcl.Models.DataAccess;

namespace test_orcl.Models.DataAccess
{
    public class PartialDataAccess
    {
        private string connectionString;

        public PartialDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public BTDBenhNhan GetBenhNhan(int mabn)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string query = "SELECT bn.MABN, bn.HOTEN,bn.NAMSINH,bn.NAM dt.DIDONG, bn.HIDE, bn.NGAYUD FROM MQSOFTDAIPHUOC.BTDBN bn INNER JOIN MQSOFTDAIPHUOC.DIENTHOAI dt ON bn.MABN=dt.MABN  WHERE bn.MABN = :mabn";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    // Sử dụng Add để thêm tham số
                    command.Parameters.Add(new OracleParameter(":mabn", OracleDbType.Varchar2, mabn, ParameterDirection.Input));
                    //command.Parameters.Add(new OracleParameter(":password", OracleDbType.Varchar2, password, ParameterDirection.Input));


                    connection.Open();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new BTDBenhNhan
                            {
                                MABN = Convert.ToInt32(reader["MABN"]),
                                HOTEN = reader["HOTEN"].ToString(),
                                NAMSINH = reader["NAMSINH"].ToString(),
                                NAM = reader["NAM"].ToString(),
                                DIDONG = reader["DIDONG"].ToString(),
                                HIDE = Convert.ToInt16(reader["HIDE"]),
                                NGAYUD = Convert.ToDateTime(reader["NGAYUD"])
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

        public BTDBenhNhan GetLichSuKhamBenh(int mabn, string yymm, int mabs)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string query = "SELECT MABN, MAVAOVIEN, MAQL,NGAYUD FROM MQSOFTDAIPHUOC:yymm.BENHANPK  WHERE bn.MABN = :mabn";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    // Sử dụng Add để thêm tham số
                    command.Parameters.Add(new OracleParameter(":mabn", OracleDbType.Varchar2, mabn, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter(":yymm", OracleDbType.Varchar2, yymm, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter(":mabs", OracleDbType.Varchar2, mabs, ParameterDirection.Input));


                    connection.Open();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new BTDBenhNhan
                            {
                                MABN = Convert.ToInt32(reader["MABN"]),
                                HOTEN = reader["HOTEN"].ToString(),
                                NAMSINH = reader["NAMSINH"].ToString(),
                                DIDONG = reader["DIDONG"].ToString(),
                                HIDE = Convert.ToInt16(reader["HIDE"]),
                                NGAYUD = Convert.ToDateTime(reader["NGAYUD"])
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

        // danh sách bệnh án phòng khám
        public BenhAnPK GetBenhAnByDate(int mabn)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string query = "SELECT MABN, MAQL, MAVAOVIEN, CHANDOAN, MAKP, MABS, NGAY, NGAYTD FROM MQSOFTDAIPHUOC1223.BENHANPK WHERE MABN = :mabn";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    // Sử dụng Add để thêm tham số
                    command.Parameters.Add(new OracleParameter(":mabn", OracleDbType.Varchar2, mabn, ParameterDirection.Input));
                    connection.Open();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new BenhAnPK
                            {
                                MABN = reader["MABN"].ToString(),
                                MAQL = Convert.ToInt32(reader["MAQL"]),
                                MAVAOVIEN = Convert.ToInt32(reader["MAVAOVIEN"]),
                                CHANDOAN= reader["CHANDOAN"].ToString(),
                                MAKP = Convert.ToInt32(reader["MAKP"]),
                                MABS = Convert.ToInt16(reader["MABS"]),
                                NGAY = Convert.ToDateTime(reader["NGAY"]),
                                NGAYTD= Convert.ToDateTime(reader["NGAYTD"])
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