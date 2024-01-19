using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Configuration;
using test_orcl.Models;

namespace test_orcl.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        //public ActionResult Index()
        //{
        //    List<DangNhap> tks = new List<DangNhap>();
        //    string connectionString = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;

        //    using (OracleConnection connection = new OracleConnection(connectionString))
        //    {
        //        string query = "SELECT * FROM mqsoftdaiphuoc.DangNhap";

        //        using (OracleCommand command = new OracleCommand(query, connection))
        //        {
        //            connection.Open();

        //            using (OracleDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    DangNhap tk = new DangNhap
        //                    {
        //                        ID = Convert.ToInt32(reader["ID"]),
        //                        TENUSER = reader["TENUSER"].ToString(),
        //                        PASSWORD = reader["PASSWORD"].ToString(),
        //                        TENBACSI = reader["TENBACSI"].ToString(),
        //                        HIDE =Convert.ToInt16(reader["HIDE"]),
        //                        NGAYUD =Convert.ToDateTime(reader["NGAYUD"])
        //                    };
        //                    tks.Add(tk);
        //                }
        //            }
                   
        //        }
        //        //connection.Close();
        //    }
        //    return View(tks);
        //}

        // Đặt hẹn
        // Kết quả HSBA


        // Gói khám
        public PartialViewResult GoiKhamPartial()
        {
            List<GoiKham> tks = new List<GoiKham>();
            string connectionString = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string query = "SELECT * FROM mqsoftdaiphuoc.GoiKham";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    connection.Open();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            GoiKham tk = new GoiKham
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                TENGOIKHAM = reader["TENGOIKHAM"].ToString(),
                                MOTA = reader["MOTA"].ToString(),
                                NGAYUD = Convert.ToDateTime(reader["NGAYUD"])
                            };
                            tks.Add(tk);
                        }
                    }

                }
            }
            ViewBag.Count = tks.Count();
            return PartialView("GoiKhamPartial",tks);
        }
        public PartialViewResult _LoiThoaiDatHenPartial()
        {
            return PartialView("_LoiThoaiDatHenPartial");
        }

        //DS Benh Nhan
        public ActionResult BTDBNPartial( string mabn)
        {
            List<BTDBenhNhan> bns = new List<BTDBenhNhan>();
            string connectionString = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string query = "SELECT * FROM mqsoftdaiphuoc.btdbn a inner join mqsoftdaiphuoc.dienthoai b on a.mabn=b.mabn where a.mabn=" + mabn ;

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    connection.Open();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BTDBenhNhan bn = new BTDBenhNhan
                            {
                                MABN = Convert.ToInt32(reader["MABN"]),
                                HOTEN = reader["HOTEN"].ToString(),
                                NAMSINH = reader["NAMSINH"].ToString(),
                                DIDONG = reader["DIDONG"].ToString(),
                                HIDE = Convert.ToInt16(reader["HIDE"]),
                                NGAYUD = Convert.ToDateTime(reader["NGAYUD"])
                            };
                            bns.Add(bn);
                        }
                    }

                }
                //connection.Close();
            }
            return View(bns);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}