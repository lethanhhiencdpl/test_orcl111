using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using test_orcl.Models;
using test_orcl.Models.DataAccess;
using System.Web.SessionState;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Threading.Tasks;
using System.Text;
using static test_orcl.Models.Ganeral;
using System.Net.NetworkInformation;
using System.Collections;
using WebGrease.Css.Extensions;
using System.Reflection;
using System.Web.WebPages;
using Microsoft.Ajax.Utilities;
using System.Runtime.InteropServices.ComTypes;
using System.Data.SqlClient;
using test_orcl.Extensions;

namespace test_orcl.Models.DataAccess
{
    public class ListBenhAnPhongKham
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;

        #region //Get bệnh án
        [HttpPost]
        public List<BenhAnPK> GetBenhAn(string mabs/*, DateTime startDate*/)
        {
            List<BenhAnPK> benhans = new List<BenhAnPK>();
            // Lấy giá trị PatientId từ Session
            mabs = HttpContext.Current.Session["ID"].ToString();// as string;


            //// Lấy giá trị 'mabs' từ Session
            //string mabs1 = Session["mabs"] as string;

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string query = "SELECT MABN, MAQL, MAVAOVIEN, CHANDOAN, MAKP, MABS, NGAY, NGAYTD FROM MQSOFTDAIPHUOC1123.BENHANPK WHERE MABS = :mabs and to_char(ngaytd,'dd/mm/yyyy')='20/11/2023' ";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter(":mabs", OracleDbType.Varchar2, mabs, ParameterDirection.Input));
                    //command.Parameters.Add(new OracleParameter(":startDate", OracleDbType.Varchar2, startDate, ParameterDirection.Input));
                    //command.Parameters.Add(new OracleParameter(":endDate", OracleDbType.Varchar2, endDate, ParameterDirection.Input));
                    connection.Open();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BenhAnPK benhan = new BenhAnPK
                            {
                                MABN = (reader["MABN"]).ToString(),
                                MAQL = Convert.ToInt64(reader["MAQL"]),
                                MAVAOVIEN = Convert.ToInt64(reader["MAVAOVIEN"]),
                                CHANDOAN = reader["CHANDOAN"].ToString(),
                                MAKP = Convert.ToInt32(reader["MAKP"]),
                                MABS = Convert.ToInt16(reader["MABS"]),
                                NGAY = Convert.ToDateTime(reader["NGAY"]),
                                NGAYTD = Convert.ToDateTime(reader["NGAYTD"])
                            };
                            benhans.Add(benhan);
                        }
                    }
                }
            }

            return benhans.OrderByDescending(n => n.NGAY).ToList();
        }
        #endregion 

        #region  //get mabn
        public List<BTDBenhNhan> GetThongTinBN(string code)
        {
            List<BTDBenhNhan> benhnhans = new List<BTDBenhNhan>();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string query = "SELECT bn.MABN, bn.HOTEN, bn.NAMSINH, bn.THON, dt.DIDONG FROM MQSOFTDAIPHUOC.BTDBN bn inner join mqsoftdaiphuoc.dienthoai dt on bn.mabn=dt.mabn WHERE bn.MABN = :code ";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));
                    connection.Open();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BTDBenhNhan benhnhan = new BTDBenhNhan
                            {
                                MABN = Convert.ToInt32(reader["MABN"]),
                                HOTEN = reader["HOTEN"].ToString(),
                                NAMSINH = reader["NAMSINH"].ToString(),
                                DIDONG= reader["DIDONG"].ToString(),
                                
                            };
                            benhnhans.Add(benhnhan);
                        }
                    }
                }
            }

            return benhnhans;
        }
        #endregion

        #region // Get Chỉ định
        public List<V_ChiDinh> GetChiDinh(string code, string years)
        {
            //if (string.IsNullOrEmpty(code)) return null;
            List<V_ChiDinh> v_cds = new List<V_ChiDinh>();
            //var listMMYY = years.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries).Reverse().ToList();
            //years = GetThongTinBN(FirstOrDefault().NAM.Trim());

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string query = $"SELECT MABN, MAQL, MAVAOVIEN, CHANDOAN, MAKP, MABS, NGAY FROM MQSOFTDAIPHUOC{years}.V_CHIDINH WHERE MABN = :code ";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));
                    connection.Open();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            V_ChiDinh cd = new V_ChiDinh
                            {
                                MABN = reader["MABN"].ToString()
                            };
                            v_cds.Add(cd);
                        }
                    }
                }
            }

            return v_cds;
        }
        #endregion

        #region MyRegion GetThongTinBN_new
        public List<V_ChiDinh> GetThongTinBN_new(string code)
        {
            List<V_ChiDinh> benhnhans = new List<V_ChiDinh>();
            string[] yearArray;  // Mảng chứa các năm MMYY

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                // Lấy danh sách MMYY từ bảng BTDBN
                string queryMMYY = "SELECT NAM FROM MQSOFTDAIPHUOC.BTDBN WHERE MABN = :code";

                using (OracleCommand commandMMYY = new OracleCommand(queryMMYY, connection))
                {
                    commandMMYY.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));
                    connection.Open();

                    using (OracleDataReader readerMMYY = commandMMYY.ExecuteReader())
                    {
                        while (readerMMYY.Read())
                        {
                            string years = readerMMYY["NAM"].ToString();
                            yearArray = years.Split('+');
                            foreach (var item in yearArray)
                            {
                                // Sử dụng MMYY để tạo câu truy vấn cho bảng V_CHIDINH
                                string query = $"SELECT MABN, MAQL, MAVAOVIEN, CHANDOAN, MAKP, MABS, NGAY FROM MQSOFTDAIPHUOC{item}.V_CHIDINH WHERE MABN = :code";

                                using (OracleCommand command = new OracleCommand(query, connection))
                                {
                                    command.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));

                                    using (OracleDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            V_ChiDinh benhnhan = new V_ChiDinh
                                            {
                                                MABN = reader["MABN"].ToString(),
                                                CHANDOAN = reader["CHANDOAN"].ToString()
                                                // Lấy các trường dữ liệu khác tương ứng
                                            };
                                            benhnhans.Add(benhnhan);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return benhnhans;
        }
        #endregion

        #region GetMonth
        private List<string> GetMonths(DateTime sdate, DateTime edate)
        {
            List<string> listMonth = new List<string>();

            // Calculate the total number of months between the start and end date, including both dates
            int totalMonths = (edate.Year - sdate.Year) * 12 + (edate.Month - sdate.Month) + 1;

            // Iterate over each month and add it to the list
            for (int i = 0; i < totalMonths; i++)
            {
                // Calculate the current month by adding `i` months to the start date
                DateTime currentMonth = sdate.AddMonths(i);

                // Format the current month as "MMyy" and add it to the list
                listMonth.Add(currentMonth.ToString("MMyy"));
            }

            return listMonth;
        }

        //private async Task<List<string>> GetMonthSearch(string startDate, string endDate)
        //{
        //    List<string> listMonth = new List<string>();
        //    string format = "dd/MM/yyyy HH:mm";
        //    string culture = "vi-vn";
        //    DateTime sdateTime; 
        //    DateTime edateTime;

        //    if (DateTime.TryParseExact(startDate.TrimEnd(), format, CultureInfo.CreateSpecificCulture(culture), DateTimeStyles.None, out sdateTime) &&
        //        DateTime.TryParseExact(endDate.TrimEnd(), format, CultureInfo.CreateSpecificCulture(culture), DateTimeStyles.None, out edateTime))
        //    {
        //        listMonth = GetMonths(sdateTime, edateTime);
        //    }


        //    var listMMYY = await GetThangNam(yymm);
        //    var resultMMYY = listMMYY.Intersect(listMonth).ToList();
        //    return resultMMYY;
        //}
        #endregion


        #region GetDanhSachBNChiDinhBSTrongNgay
        public List<DanhChiDinhBSTrongNgay> GetDanhChiDinhBSTrongNgay(string mabs, DateTime startDate, DateTime endDate)
        {
            List<DanhChiDinhBSTrongNgay> lists = new List<DanhChiDinhBSTrongNgay>();
            mabs = HttpContext.Current.Session["ID"].ToString();// as string;
            List<string> yearArray = new List<string>();
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                // Lấy danh sách MMYY từ bảng BTDBN
                string queryMMYY = "SELECT MMYY FROM MQSOFTDAIPHUOC.TABLES";  
                using (OracleCommand commandMMYY = new OracleCommand(queryMMYY, connection))
                {
                    //commandMMYY.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));
                    connection.Open();
                   
                    using (OracleDataReader readerMMYY = commandMMYY.ExecuteReader())
                    {
                        while (readerMMYY.Read())
                        {
                            string years = readerMMYY["MMYY"].ToString();
                            yearArray.Add(years);                            
                        }
                        foreach (var item in yearArray)
                        {
                            // Sử dụng MMYY để tạo câu truy vấn cho bảng V_CHIDINH
                            string query = $"SELECT td.mabn,cd.mabs,td.ngay, bn.hoten, bn.namsinh,bn.phai,bn.socmnd FROM MQSOFTDAIPHUOC{item}.tiepdon td inner join mqsoftdaiphuoc{item}.v_chidinh cd on td.mabn=cd.mabn inner join mqsoftdaiphuoc.btdbn bn on td.mabn=bn.mabn WHERE cd.mabs = :mabs and td.makp='116' AND td.NGAY BETWEEN TO_DATE(:startDate, 'DD-MON-RR') AND TO_DATE(:endDate, 'DD-MON-RR')+1 group by td.mabn,cd.mabs,td.ngay, bn.hoten, bn.namsinh,bn.phai,bn.socmnd";

                            using (OracleCommand command = new OracleCommand(query, connection))
                            {
                                command.Parameters.Add(new OracleParameter(":mabs", OracleDbType.Varchar2, mabs, ParameterDirection.Input));
                                command.Parameters.Add(new OracleParameter(":startDate", OracleDbType.Date, startDate, ParameterDirection.Input));
                                command.Parameters.Add(new OracleParameter(":endDate", OracleDbType.Date, endDate, ParameterDirection.Input));

                                using (OracleDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        DanhChiDinhBSTrongNgay list = new DanhChiDinhBSTrongNgay
                                        {
                                            MABN = reader["MABN"].ToString(),
                                            MABS = reader["MABS"].ToString(),
                                            HOTEN = reader["HOTEN"].ToString(),
                                            SOCMND = reader["SOCMND"].ToString(),
                                            NAMSINH = reader["NAMSINH"].ToString(),
                                            PHAI = Convert.ToInt16(reader["PHAI"].ToString()),
                                            NGAY = Convert.ToDateTime(reader["NGAY"].ToString())
                                        };
                                        lists.Add(list);
                                    }
                                }
                            }
                        }
                    }
                }
            }


            return lists.OrderBy(n=>n.MABN).ToList();
        }
        #endregion


        #region MyRegion333333
        public List<V_ChiDinh> GetChiDinh_New(string code)
        {
            List<V_ChiDinh> cds = new List<V_ChiDinh>();
            string years = "";  // Chuỗi các năm MMYY
            string[] yearArray;  // Mảng chứa các năm MMYY

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                // Lấy danh sách MMYY từ bảng BTDBN
                string queryMMYY = "SELECT MMYY FROM MQSOFTDAIPHUOC.BTDBN WHERE MABN = :code";

                using (OracleCommand commandMMYY = new OracleCommand(queryMMYY, connection))
                {
                    commandMMYY.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));
                    connection.Open();

                    using (OracleDataReader readerMMYY = commandMMYY.ExecuteReader())
                    {
                        while (readerMMYY.Read())
                        {
                            if (string.IsNullOrEmpty(years))
                            {
                                years += readerMMYY["MMYY"].ToString();
                            }
                            else
                            {
                                years += "+" + readerMMYY["MMYY"].ToString();
                            }
                        }
                    }
                }

                // Chia chuỗi years thành mảng các năm
                yearArray = years.Split('+');

                // Sử dụng mảng years để xây dựng câu truy vấn
                foreach (var year in yearArray)
                {
                    string query = $"SELECT MABN, MAQL, MAVAOVIEN, CHANDOAN, MAKP, MABS, NGAY FROM MQSOFTDAIPHUOC{year}.V_CHIDINH WHERE MABN = :code";

                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                V_ChiDinh cd = new V_ChiDinh
                                {
                                    MABN = reader["MABN"].ToString(),
                                    CHANDOAN = reader["CHANDOAN"].ToString()
                                    // Lấy các trường dữ liệu khác tương ứng
                                    // ...
                                };
                                cds.Add(cd);
                            }
                        }
                    }
                }
            }

            return cds;
        }
        #endregion



        #region //get tháng năm
        public List<Tables> GetThangNam(string yymm)
        {
            List<Tables> tbs = new List<Tables>();
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string query = "SELECT YYMM FROM MQSOFTDAIPHUOC.TABLES WHERE YYMM = :yymm ";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter(":yymm", OracleDbType.Varchar2, yymm, ParameterDirection.Input));
                    connection.Open();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Tables tb = new Tables
                            {
                                MMYY = reader["MMYY"].ToString()
                            };
                            tbs.Add(tb);
                        }
                    }
                }
            }
            return tbs;
        }
        #endregion

        #region Triệu chứng
        public List<TrieuChung> GetTrieuChung(string code, string years)
        {
            List<TrieuChung> trieuChungs = new List<TrieuChung>();
            List<string> yearArray = new List<string>();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string queryMMYY = "SELECT MMYY FROM MQSOFTDAIPHUOC.TABLES";
                using (OracleCommand commandYYMM = new OracleCommand(queryMMYY, connection))
                {
                    //commandYYMM.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));
                    connection.Open();

                    using (OracleDataReader readerMMYY = commandYYMM.ExecuteReader())
                    {
                        while (readerMMYY.Read())
                        {
                             years = readerMMYY["MMYY"].ToString();
                            yearArray.Add(years);
                        }
                        foreach (var item in yearArray)
                        {
                            // Sử dụng MMYY để tạo câu truy vấn cho bảng V_CHIDINH
                            string query = $"SELECT MAQL,TEN FROM mqsoftdaiphuoc{item}.trieuchung WHERE MAQL = :code";
                            using (OracleCommand command = new OracleCommand(query, connection))
                            {
                                command.Parameters.Add(new OracleParameter(":mabs", OracleDbType.Varchar2, code, ParameterDirection.Input));                             
                                using (OracleDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        TrieuChung trieuchung = new TrieuChung
                                        {
                                            MAQL = reader["MAQL"].ToString(),
                                            TEN = reader["TEN"].ToString(),
                                            MA = reader["MA"].ToString()
                                        };
                                        trieuChungs.Add(trieuchung);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            return trieuChungs;

        }

        #endregion

        #region Dùng hàm Analysis GetPatientDoctorDetail Lấy chi tiết xét nghiệm
        public List<Analysis> KetQuaXN_BS(string doctorCode, DateTime ngayxn, string code)
        {
            List<Analysis> lists = new List<Analysis>();
            try
            {
                if (doctorCode==null)
                {  return lists; }    
                 else
                     doctorCode = HttpContext.Current.Session["ID"].ToString();
            }
            catch (Exception)
            {

                throw;
            }
            //doctorCode = HttpContext.Current.Session["ID"].ToString();// as string;
            List<string> yearArray = new List<string>();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string queryMMYY = "SELECT MMYY FROM MQSOFTDAIPHUOC.TABLES";
                using (OracleCommand commandYYMM = new OracleCommand(queryMMYY, connection))
                {                 
                    connection.Open();

                    using (OracleDataReader readerMMYY = commandYYMM.ExecuteReader())
                    {
                        while (readerMMYY.Read())
                        {
                            string years = readerMMYY["MMYY"].ToString();
                            yearArray.Add(years);
                        }
                        foreach (var item in yearArray)
                        {                      
                            string query = $"SELECT distinct kq.id_ten as maxn, bn.HOTEN,bn.mabn,bn.PHAI,bn.NAMSINH, xnpct.MABS, kq.KETQUA , xndv.TEN AS DONVI,t.TEN as TENXETNGHIEM_CON,bvt.TEN as TENXETNGHIEM_CHA, kq.GHICHU, t.CSBT_NU ,t.CSBT_NAM,xn.NGAY FROM mqsoftdaiphuoc{item}.xn_phieu xn LEFT JOIN mqsoftdaiphuoc{item}.xn_ketqua kq on xn.ID = kq.ID LEFT JOIN mqsoftdaiphuoc.xn_bv_chitiet ct on ct.ID = kq.ID_TEN LEFT JOIN mqsoftdaiphuoc.xn_bv_ten bvt on bvt.ID = ct.ID_BV_TEN LEFT JOIN mqsoftdaiphuoc.xn_ten t on t.ID = ct.ID_TEN LEFT JOIN mqsoftdaiphuoc.xn_donvi xndv on xndv.ID = t.DONVI LEFT JOIN mqsoftdaiphuoc.xn_bv_so xns on xns.ID = bvt.ID_BV_SO inner join BTDBN bn on bn.MABN = xn.MABN LEFT JOIN  mqsoftdaiphuoc{item}.xn_phieu_chitiet xnpct on xnpct.ID = xn.ID where xnpct.MABS = :doctorCode and bn.mabn=:code";
                            using (OracleCommand command = new OracleCommand(query, connection))
                            {
                                command.Parameters.Add(new OracleParameter(":doctorCode", OracleDbType.Varchar2, doctorCode, ParameterDirection.Input));
                                command.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));
                                using (OracleDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    { 
                                        Analysis analysis = new Analysis
                                        {
                                            MAXN = reader["MAXN"].ToString(),
                                            TENXETNGHIEM = reader["TENXETNGHIEM_CON"].ToString(),
                                            HOTEN = reader["HOTEN"].ToString(),
                                            MABN = reader["MABN"].ToString(),
                                            //PHAI = Convert.ToInt16(reader["PHAI"].ToString() != "1" ? "NỮ" : "NAM"),
                                            NAMSINH = Convert.ToInt16(reader["NAMSINH"].ToString()),
                                            KETQUA = reader["KETQUA"].ToString(),
                                            MABS= reader["MABS"].ToString(),                                     
                                            DONVI = reader["DONVI"].ToString(),                                          
                                            GHICHU = reader["GHICHU"].ToString(),
                                            //CSBT_NU = reader["CSBT_NU"].ToString(),
                                            //CSBT_NAM = reader["CSBT_NAM"].ToString(),
                                            CSBT = reader["PHAI"].ToString() != "1" ? reader["CSBT_NAM"].ToString() : reader["CSBT_NU"].ToString(),
                                            NGAYXETNGHIEM =Convert.ToDateTime(reader["NGAY"].ToString()),
                                            //GIODANGKY = reader["GIODANGKY"].ToString(),
                                            //Sex = reader["Sex"].ToString(),
                                            //IsRange =Convert.ToBoolean(reader["IsRange"].ToString())
                                        };
                                        
                                        lists.Add(analysis);
                                    }
                                }
                            }
                        }
                        return lists.Where(n => n.MABN == code && n.MABS == doctorCode && n.NGAYXETNGHIEM.Date == ngayxn.Date).ToList();
                    }
                }
            }
        }
        #endregion


        // Get ngày kết quả xét nghiệm
        #region Get ngày kết quả xét nghiệm
        public List<CountNgayXetNghiem> NgayXetNghiem(string doctorCode, string code)
        {
            List<CountNgayXetNghiem> lists = new List<CountNgayXetNghiem>();
            try
            {
                if (doctorCode == null)
                { return lists; }

            }
            catch (Exception)
            {

                throw;
            }
            doctorCode = HttpContext.Current.Session["ID"].ToString();// as string;
            List<string> yearArray = new List<string>();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string queryMMYY = "SELECT MMYY FROM MQSOFTDAIPHUOC.TABLES";
                using (OracleCommand commandYYMM = new OracleCommand(queryMMYY, connection))
                {
                    connection.Open();

                    using (OracleDataReader readerMMYY = commandYYMM.ExecuteReader())
                    {
                        while (readerMMYY.Read())
                        {
                            string years = readerMMYY["MMYY"].ToString();
                            yearArray.Add(years);
                        }
                        foreach (var item in yearArray)
                        {
                            string query = $"SELECT distinct xn.ngay as ngayxetnghiem,bn.mabn,xnpct.MABS \r\nFROM mqsoftdaiphuoc{item}.xn_phieu xn LEFT JOIN mqsoftdaiphuoc{item}.xn_ketqua kq on xn.ID = kq.ID LEFT JOIN mqsoftdaiphuoc.xn_bv_chitiet ct on ct.ID = kq.ID_TEN \r\nLEFT JOIN mqsoftdaiphuoc.xn_bv_ten bvt on bvt.ID = ct.ID_BV_TEN LEFT JOIN mqsoftdaiphuoc.xn_ten t on t.ID = ct.ID_TEN LEFT JOIN mqsoftdaiphuoc.xn_donvi xndv on xndv.ID = t.DONVI \r\nLEFT JOIN mqsoftdaiphuoc.xn_bv_so xns on xns.ID = bvt.ID_BV_SO inner join BTDBN bn on bn.MABN = xn.MABN LEFT JOIN  mqsoftdaiphuoc{item}.xn_phieu_chitiet xnpct on xnpct.ID = xn.ID \r\nwhere xnpct.MABS = :doctorCode and bn.mabn=:code";
                            using (OracleCommand command = new OracleCommand(query, connection))
                            {
                                command.Parameters.Add(new OracleParameter(":doctorCode", OracleDbType.Varchar2, doctorCode, ParameterDirection.Input));
                                command.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));
                                using (OracleDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        CountNgayXetNghiem ngayXetNghiem = new CountNgayXetNghiem
                                        {
                                            NGAYXETNGHIEM = Convert.ToDateTime(reader["NGAYXETNGHIEM"].ToString()),
                                            MABN = reader["MABN"].ToString(),
                                            MABS = reader["MABS"].ToString()
                                        };

                                        lists.Add(ngayXetNghiem);
                                    }
                                }
                            }
                        }
                        return lists.Where(n => n.MABN == code && n.MABS == doctorCode).ToList();
                    }
                }
            }
        }
        #endregion


        // Get kết quả SIêu âm hình
        public List<PatientImage> GetPatientImageByCode(string code)
        {           
            List<PatientImage> benhnhans = new List<PatientImage>();
            string[] yearArray;  // Mảng chứa các năm MMYY

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                // Lấy danh sách MMYY từ bảng BTDBN
                string queryMMYY = "SELECT NAM FROM MQSOFTDAIPHUOC.BTDBN WHERE MABN = :code";

                using (OracleCommand commandMMYY = new OracleCommand(queryMMYY, connection))
                {
                    commandMMYY.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));
                    connection.Open();

                    using (OracleDataReader readerMMYY = commandMMYY.ExecuteReader())
                    {
                        while (readerMMYY.Read())
                        {
                            string years = readerMMYY["NAM"].ToString();
                            yearArray = years.Split('+');
                            foreach (var item in yearArray)
                            {
                                // Sử dụng MMYY để tạo câu truy vấn cho bảng V_CHIDINH
                                string query = $"SELECT IMAGE FROM MQSOFTDAIPHUOCIMAGE{item}.cdha_image WHERE id = :code";

                                using (OracleCommand command = new OracleCommand(query, connection))
                                {
                                    command.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));

                                    using (OracleDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            byte[] imageData = (byte[])reader["Image"]; // Thay đổi tên cột tương ứng trong SELECT
                                            string base64String = $"data:image/png;base64,{Convert.ToBase64String(imageData)}";

                                            PatientImage benhnhan = new PatientImage
                                            {
                                                Value = base64String
                                            };
                                            benhnhans.Add(benhnhan);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return benhnhans;
        }

        // Get kết quả SIêu âm GetPatientImageByCode
        public List<PatientDataImage> GetPatientDataImageByCode(string code, DateTime ngaycdha, string doctorCode)
        {
            List<PatientDataImage> cds = new List<PatientDataImage>();
            string[] yearArray;  // Mảng chứa các năm MMYY

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                // Lấy danh sách MMYY từ bảng BTDBN
                string queryMMYY = "SELECT NAM FROM MQSOFTDAIPHUOC.BTDBN WHERE MABN = :code";

                using (OracleCommand commandMMYY = new OracleCommand(queryMMYY, connection))
                {
                    commandMMYY.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));
                    connection.Open();

                    using (OracleDataReader readerMMYY = commandMMYY.ExecuteReader())
                    {
                        while (readerMMYY.Read())
                        {
                            string years = readerMMYY["NAM"].ToString();
                            yearArray = years.Trim().Split('+').Where(s => !string.IsNullOrEmpty(s)).ToArray();
                            foreach (var year in yearArray)
                            {
                                string query = $"SELECT a.MABS,a.MABN,a.ID,b.DONGIA,b.MOTA,b.KETLUAN,dm.HOTEN as HOTENBS,kt.TEN as TENKYTHUAT,lkt.TEN as TENLOAIKT,to_char(NGAYCHUP,'dd/mm/yyyy hh24:mi')as NGAYCHUP,to_char(NGAY,'dd/mm/yyyy hh24:mi')as NGAYKETQUA FROM mqsoftdaiphuoc{year}.cdha_bnll a LEFT JOIN mqsoftdaiphuoc{year}.cdha_bnct b ON a.ID= b.ID left join mqsoftdaiphuoc.dmbs dm on a.MABS = dm.MA and dm.hide =0 left join mqsoftdaiphuoc.cdha_kythuat kt on kt.ID = b.MAKT left join mqsoftdaiphuoc.cdha_loai lkt on lkt.ID = kt.ID_LOAI where a.mabn =:code and a.mabs=:doctorCode";

                                using (OracleCommand command = new OracleCommand(query, connection))
                                {
                                    command.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));
                                    command.Parameters.Add(new OracleParameter(":doctorCode", OracleDbType.Varchar2, doctorCode, ParameterDirection.Input));

                                    using (OracleDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            PatientDataImage cd = new PatientDataImage
                                            {
                                                ID = Convert.ToInt64(reader["ID"]).ToString(),
                                                MABN = reader["MABN"].ToString(),
                                                DONGIA = Convert.ToDouble(reader["DONGIA"]),
                                                MOTA = RtfExtensions.ConvertRtfToStr(reader["MOTA"].ToString()),
                                                KETLUAN = RtfExtensions.ConvertRtfToStr( reader["KETLUAN"].ToString()),
                                                HOTENBS = reader["HOTENBS"].ToString(),
                                                TENKYTHUAT = reader["TENKYTHUAT"].ToString(),
                                                TENLOAIKT = reader["TENLOAIKT"].ToString(),
                                                NGAYCHUP = reader["NGAYCHUP"].ToString(),
                                                NGAYKETQUA = reader["NGAYKETQUA"].ToString(),
                                                //NGAYKETQUA = Convert.ToDateTime(reader["NGAY"].ToString()),
                                                MABS = reader["MABS"].ToString()
                                            };
                                      
                                            cds.Add(cd);
                                        }
                                    }
                                }
                            }
                            //return cds.Where(n => n.ID == code && n.MABS == doctorCode && Convert.ToDateTime(n.NGAYKETQUA).Date == ngaycdha.Date).ToList();
                            //return cds.Where(n => n.ID == code && n.MABS == doctorCode).ToList();
                        }
                    }
                }
                
            }

            return cds;
        }



        #region Get ngày kết quả siêu âm
        public List<CountNgayCDHA> NgayCDHA(string doctorCode, string code)
        {
            List<CountNgayCDHA> lists = new List<CountNgayCDHA>();
            string[] yearArray;  // Mảng chứa các năm MMYY
            try
            {
                if (doctorCode == null)
                { return lists; }

            }
            catch (Exception)
            {

                throw;
            }
            doctorCode = HttpContext.Current.Session["ID"].ToString();// as string;

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string queryMMYY = "SELECT NAM FROM MQSOFTDAIPHUOC.BTDBN where mabn=:code ";
                using (OracleCommand commandYYMM = new OracleCommand(queryMMYY, connection))
                {
                    commandYYMM.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));
                    connection.Open();

                    using (OracleDataReader readerMMYY = commandYYMM.ExecuteReader())
                    {
                        while (readerMMYY.Read())
                        {
                            string years = readerMMYY["NAM"].ToString();
                            yearArray = years.Trim().Split('+').Where(s => !string.IsNullOrEmpty(s)).ToArray();
                            foreach (var item in yearArray)
                            {
                                string query = $"SELECT distinct a.NGAY as NGAYKETQUA, a.mabn,a.ID,b.DONGIA,b.MOTA,b.KETLUAN,dm.HOTEN as HOTENBS, a.MABS,kt.TEN as TENKYTHUAT,lkt.TEN as TENLOAIKT, NGAYCHUP as NGAYCHUP  FROM mqsoftdaiphuoc{item}.cdha_bnll a LEFT JOIN mqsoftdaiphuoc{item}.cdha_bnct b ON a.ID = b.ID left join mqsoftdaiphuoc.dmbs dm on a.MABS = dm.MA and dm.hide = 0 left join mqsoftdaiphuoc.cdha_kythuat kt on kt.ID = b.MAKT left join mqsoftdaiphuoc.cdha_loai lkt on lkt.ID = kt.ID_LOAI where a.mabn = :code and a.mabs = :doctorCode";
                                using (OracleCommand command = new OracleCommand(query, connection))
                                {
                                    command.Parameters.Add(new OracleParameter(":doctorCode", OracleDbType.Varchar2, doctorCode, ParameterDirection.Input));
                                    command.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));
                                    using (OracleDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            CountNgayCDHA ngayCDHA = new CountNgayCDHA
                                            {
                                                NGAYCDHA = Convert.ToDateTime(reader["NGAYKETQUA"].ToString()),
                                                MABN = reader["MABN"].ToString(),
                                                MABS = reader["MABS"].ToString()
                                            };

                                            lists.Add(ngayCDHA);
                                        }
                                    }
                                }
                            }
                        }
                        return lists.Where(n => n.MABN == code && n.MABS == doctorCode).ToList();
                        //return lists.Distinct().Where(n => n.MABN == code ).ToList();
                    }
                }
            }
        }
        #endregion





        // Get Đơn thuốc
        public List<BTDBenhNhan> GetDonThuoc(string code)
        {
            List<BTDBenhNhan> benhnhans = new List<BTDBenhNhan>();
            // Lấy giá trị PatientId từ Session
            //mabs = HttpContext.Current.Session["MABS"].ToString();// as string;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string query = "SELECT MABN, MAQL, MAVAOVIEN, CHANDOAN, MAKP, MABS, NGAY, NGAYTD FROM MQSOFTDAIPHUOC.BTDBN WHERE MABN = :code ";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));
                    //command.Parameters.Add(new OracleParameter(":startDate", OracleDbType.Varchar2, startDate, ParameterDirection.Input));
                    //command.Parameters.Add(new OracleParameter(":endDate", OracleDbType.Varchar2, endDate, ParameterDirection.Input));
                    connection.Open();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BTDBenhNhan benhnhan = new BTDBenhNhan
                            {
                                MABN = Convert.ToInt32(reader["MABN"]),
                                //NGAY = Convert.ToDateTime(reader["NGAY"]),
                                //NGAYTD = Convert.ToDateTime(reader["NGAYTD"])
                            };
                            benhnhans.Add(benhnhan);
                        }
                    }
                }
            }

            return benhnhans;
        }

    }
}