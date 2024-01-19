using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace test_orcl.Models.DataAccess
{
    public class MedicalRecordRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;

        public List<MedicalRecord> GetMedicalRecordsByPatientId(int patientId)
        {
            List<MedicalRecord> records = new List<MedicalRecord>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT RecordId, PatientId, VisitDate, Diagnosis, Treatment FROM MedicalRecords WHERE PatientId = @PatientId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PatientId", patientId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MedicalRecord record = new MedicalRecord
                            {
                                RecordId = Convert.ToInt32(reader["RecordId"]),
                                PatientId = Convert.ToInt32(reader["PatientId"]),
                                VisitDate = Convert.ToDateTime(reader["VisitDate"]),
                                Diagnosis = reader["Diagnosis"].ToString(),
                                Treatment = reader["Treatment"].ToString()
                            };
                            records.Add(record);
                        }
                    }
                }
            }

            return records;
        }

      
    }
}