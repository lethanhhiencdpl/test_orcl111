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
using static iTextSharp.text.pdf.AcroFields;

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
                string query = "SELECT bn.MABN, bn.HOTEN, bn.NAMSINH,p.ten as PHAI, (bn.sonha ||','|| px.TENPXA ||','|| qu.TENQUAN ||','|| tt.TENTT)as DIACHI,  dt.DIDONG \r\nFROM MQSOFTDAIPHUOC.BTDBN bn \r\ninner join mqsoftdaiphuoc.dienthoai dt on bn.mabn=dt.mabn\r\nINNER JOIN mqsoftdaiphuoc.btdtt tt ON tt.matt = bn.matt\r\nINNER JOIN mqsoftdaiphuoc.btdquan qu ON qu.maqu = bn.maqu\r\nINNER JOIN mqsoftdaiphuoc.btdpxa px ON px.maphuongxa = bn.maphuongxa\r\ninner join mqsoftdaiphuoc.dmphai p on bn.phai=p.ma\r\nWHERE bn.MABN = :code ";

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
                                DIDONG = reader["DIDONG"].ToString(),
                                PHAI = reader["PHAI"].ToString(),
                                DIACHI = reader["DIACHI"].ToString()
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
            //HttpContext.Current.Session.Clear(); //test xóa session để kiểm tra
            //HttpContext.Current.Session.Abandon(); // Đảm bảo xóa hoàn toàn session
            // Kiểm tra session
            if (HttpContext.Current.Session["ID"] == null || string.IsNullOrEmpty(HttpContext.Current.Session["ID"].ToString()))
            {
                // Hiển thị thông báo cảnh báo và chuyển hướng đến trang login
                HttpContext.Current.Response.Redirect("~/HomeAdmin/Index?message=Session đã hết hạn. Vui lòng đăng nhập lại.", false);
                HttpContext.Current.ApplicationInstance.CompleteRequest(); // Ngừng xử lý luồng hiện tại
                return null; // Ngừng thực thi hàm
            }
            mabs = HttpContext.Current.Session["ID"].ToString();// as string;
            List<string> yearArray = new List<string>();
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    // Lấy danh sách MMYY từ bảng BTDBN
                    string queryMMYY = "SELECT MMYY FROM MQSOFTDAIPHUOC.TABLES";
                    using (OracleCommand commandMMYY = new OracleCommand(queryMMYY, connection))
                    {
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

                                //string query = $@"SELECT distinct td.mabn,cd.mabs,td.ngay, bn.hoten, bn.namsinh,bn.phai,bn.socmnd FROM MQSOFTDAIPHUOC{item}.tiepdon td inner join mqsoftdaiphuoc{item}.v_chidinh cd on td.mabn=cd.mabn inner join mqsoftdaiphuoc.btdbn bn on td.mabn=bn.mabn WHERE cd.mabs = :mabs AND td.NGAY BETWEEN TO_DATE(:startDate, 'DD-MON-RR') AND TO_DATE(:endDate, 'DD-MON-RR')+1 group by td.mabn,cd.mabs,td.ngay, bn.hoten, bn.namsinh,bn.phai,bn.socmnd";
                                string query = $@"SELECT td.mabn,cd.mabs,MIN(td.ngay) AS ngay,MAX(bn.hoten) AS hoten,MAX(bn.namsinh) AS namsinh,MAX(bn.phai) AS phai,
                            MAX(bn.socmnd) AS socmnd FROM mqsoftdaiphuoc{item}.tiepdon td 
                            INNER JOIN  mqsoftdaiphuoc{item}.v_chidinh cd ON td.mabn = cd.mabn INNER JOIN mqsoftdaiphuoc.btdbn bn ON td.mabn = bn.mabn 
                           WHERE cd.mabs = :mabs AND td.ngay BETWEEN TO_DATE(:startDate, 'DD-MON-RR') AND TO_DATE(:endDate, 'DD-MON-RR') + 1
                           GROUP BY td.mabn, cd.mabs";
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
            }
            catch (Exception ex)
            {
                // Truyền thông báo lỗi thông qua HttpContext hoặc ViewBag
                HttpContext.Current.Items["DatabaseError"] = "Không truy cập được máy chủ." + ex;
            }

           

            return lists.OrderByDescending(n => n.NGAY).ToList();
            //return lists.OrderBy(n=>n.MABN).ToList();
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

        #region Lấy chi tiết xét nghiệm  
        public List<Analysis> KetQuaXN_BS(string doctorCode, DateTime ngayxn, string code)
        {
            List<Analysis> lists = new List<Analysis>();
            try
            {
                if (string.IsNullOrEmpty(doctorCode))
                {
                    // Nếu doctorCode null, lấy từ Session
                    if (HttpContext.Current.Session["ID"] != null)
                    {
                        doctorCode = HttpContext.Current.Session["ID"].ToString();
                    }
                    else
                    {
                        return lists; // Không có thông tin doctorCode, trả về danh sách rỗng
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving doctorCode from session", ex);
            }

            string[] yearArray = null; // Khởi tạo mảng năm

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
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

                                foreach (var item in yearArray)
                                {
                                    string query = $@" SELECT DISTINCT kq.id_ten AS MAXN, bn.HOTEN, bn.mabn, bn.PHAI, bn.NAMSINH,  (bn.sonha ||', '|| px.TENPXA ||', '|| qu.TENQUAN ||', '|| tt.TENTT)as DIACHI,
                                     xnpct.MABS, kq.KETQUA, xndv.TEN AS DONVI, xnpct.chandoan,
                                     t.TEN AS TENXETNGHIEM_CON, xns.TEN AS NHOMXETNGHIEM, ct.STT, xnm.ten as MAYXETNGHIEM,
                                     kq.GHICHU, t.CSBT_NU, t.CSBT_NAM, xn.NGAY, bs.hoten as HOTENBS, xn.ngaylaymau, xn.stt as STTLAYMAU, kq.ngayduyet
                                     FROM mqsoftdaiphuoc{item}.xn_phieu xn 
                                     LEFT JOIN mqsoftdaiphuoc{item}.xn_ketqua kq ON xn.ID = kq.ID 
                                     LEFT JOIN mqsoftdaiphuoc.xn_bv_chitiet ct ON ct.ID = kq.ID_TEN 
                                     LEFT JOIN mqsoftdaiphuoc.xn_bv_ten bvt ON bvt.ID = ct.ID_BV_TEN 
                                    LEFT JOIN mqsoftdaiphuoc.xn_lab_mayxn xnm ON xnm.id = kq.IDMAY 
                                     LEFT JOIN mqsoftdaiphuoc.xn_ten t ON t.ID = ct.ID_TEN 
                                     LEFT JOIN mqsoftdaiphuoc.xn_donvi xndv ON xndv.ID = t.DONVI 
                                     LEFT JOIN mqsoftdaiphuoc.xn_bv_so xns ON xns.ID = bvt.ID_BV_SO 
                                     INNER JOIN mqsoftdaiphuoc.BTDBN bn ON bn.MABN = xn.MABN 
                                    INNER JOIN mqsoftdaiphuoc.btdtt tt ON tt.matt = bn.matt
                                    INNER JOIN mqsoftdaiphuoc.btdquan qu ON qu.maqu = bn.maqu
                                    INNER JOIN mqsoftdaiphuoc.btdpxa px ON px.maphuongxa = bn.maphuongxa
                                     LEFT JOIN mqsoftdaiphuoc{item}.xn_phieu_chitiet xnpct ON xnpct.ID = xn.ID 
                                    left JOIN mqsoftdaiphuoc.dmbs bs on xnpct.mabs = bs.ma
                                     WHERE xnpct.MABS = :doctorCode AND bn.MABN = :code 
                                     AND TO_DATE(xn.ngaylaymau, 'DD/MM/YYYY') = TO_DATE(:ngayxn, 'DD/MM/YYYY')
                                     ORDER BY xn.ngaylaymau";
                                    using (OracleCommand command = new OracleCommand(query, connection))
                                    {
                                        command.Parameters.Add(new OracleParameter(":doctorCode", OracleDbType.Varchar2, doctorCode, ParameterDirection.Input));
                                        command.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));
                                        command.Parameters.Add(new OracleParameter(":ngayxn", OracleDbType.Date, ngayxn, ParameterDirection.Input));

                                        using (OracleDataReader reader = command.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                Analysis analysis = new Analysis
                                                {
                                                    MAXN = reader["MAXN"] != DBNull.Value ? reader["MAXN"].ToString() : string.Empty,
                                                    TENXETNGHIEM = reader["TENXETNGHIEM_CON"] != DBNull.Value ? reader["TENXETNGHIEM_CON"].ToString() : string.Empty,
                                                    NHOMXETNGHIEM = reader["NHOMXETNGHIEM"] != DBNull.Value ? reader["NHOMXETNGHIEM"].ToString() : string.Empty,
                                                    STT = reader["STT"] != DBNull.Value ? Convert.ToInt32(reader["STT"]) : 0,
                                                    MAYXETNGHIEM = reader["MAYXETNGHIEM"] != DBNull.Value ? reader["MAYXETNGHIEM"].ToString() : string.Empty,
                                                    HOTEN = reader["HOTEN"] != DBNull.Value ? reader["HOTEN"].ToString() : string.Empty,
                                                    MABN = reader["MABN"] != DBNull.Value ? reader["MABN"].ToString() : string.Empty,
                                                    NAMSINH = reader["NAMSINH"] != DBNull.Value ? Convert.ToInt32(reader["NAMSINH"]) : 0,
                                                    DIACHI = reader["DIACHI"] != DBNull.Value ? reader["DIACHI"].ToString() : string.Empty,
                                                    CHANDOAN = reader["CHANDOAN"] != DBNull.Value ? reader["CHANDOAN"].ToString() : string.Empty,
                                                    KETQUA = reader["KETQUA"] != DBNull.Value ? reader["KETQUA"].ToString() : string.Empty,
                                                    MABS = reader["MABS"] != DBNull.Value ? reader["MABS"].ToString() : string.Empty,
                                                    DONVI = reader["DONVI"] != DBNull.Value ? reader["DONVI"].ToString() : string.Empty,
                                                    GHICHU = reader["GHICHU"] != DBNull.Value ? reader["GHICHU"].ToString() : string.Empty,
                                                    CSBT = reader["PHAI"] != DBNull.Value && reader["PHAI"].ToString() == "1"
                                                       ? (reader["CSBT_NU"] != DBNull.Value ? reader["CSBT_NU"].ToString() : string.Empty)
                                                       : (reader["CSBT_NAM"] != DBNull.Value ? reader["CSBT_NAM"].ToString() : string.Empty),
                                                    NGAYXETNGHIEM = reader["NGAY"] != DBNull.Value ? Convert.ToDateTime(reader["NGAY"]) : DateTime.MinValue,
                                                    HOTENBS = reader["HOTENBS"] != DBNull.Value ? reader["HOTENBS"].ToString() : string.Empty,
                                                    STTLAYMAU = reader["STTLAYMAU"] != DBNull.Value ? Convert.ToInt32(reader["STTLAYMAU"]) : 0,
                                                    NGAYLAYMAU = reader["NGAYLAYMAU"] != DBNull.Value ? Convert.ToDateTime(reader["NGAYLAYMAU"]) : DateTime.MinValue,
                                                    NGAYDUYETKQ = reader["NGAYDUYET"] != DBNull.Value ? Convert.ToDateTime(reader["NGAYDUYET"]) : DateTime.MinValue,
                                                    PHAI = reader["PHAI"] != DBNull.Value ? Convert.ToInt32(reader["PHAI"]) : 0
                                                };
                                                lists.Add(analysis);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi truy xuất dữ liệu", ex);
                }
            }

            return lists.OrderBy(p => p.NHOMXETNGHIEM).ToList();
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
            string[] yearArray = null; // Khởi tạo mảng năm
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
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

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching analysis data", ex);
                }
            }

            //return lists;
            return lists.Where(n => n.MABN == code && n.MABS == doctorCode).ToList();
        }
        #endregion


        // Get hình
        public List<PatientImage> GetImage(string code, string doctorCode)
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
                                string query = $@"SELECT  bnll.NGAY AS NGAYKETQUA, bnll.mabn,  bnll.ID,  bnct.DONGIA,  bnct.MOTA,  bnct.KETLUAN,  dm.HOTEN AS HOTENBS, bnct.MABS, kt.TEN AS TENKYTHUAT,lkt.TEN AS TENLOAIKT, im.image 
                                            FROM mqsoftdaiphuoc0824.cdha_bnct bnct
                                            INNER JOIN mqsoftdaiphuoc0824.cdha_bnll bnll ON bnll.ID = bnct.ID
                                            LEFT JOIN mqsoftdaiphuoc.dmbs dm ON bnct.MABS = dm.MA AND dm.HIDE = 0
                                            LEFT JOIN mqsoftdaiphuoc.cdha_kythuat kt ON kt.ID = bnct.MAKT
                                            LEFT JOIN mqsoftdaiphuoc.cdha_loai lkt ON lkt.ID = kt.ID_LOAI
                                            LEFT JOIN mqsoftdaiphuocimage0824.cdha_image im ON im.id = bnct.ID
                                            WHERE bnct.mabs = :doctorCode  AND bnll.mabn = :code;";

                                using (OracleCommand command = new OracleCommand(query, connection))
                                {
                                    command.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));
                                    command.Parameters.Add(new OracleParameter(":doctorCode", OracleDbType.Varchar2, doctorCode, ParameterDirection.Input));
                                    using (OracleDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            byte[] imageData = (byte[])reader["Image"]; // Thay đổi tên cột tương ứng trong SELECT
                                            string base64String = $"data:image/png;base64,{Convert.ToBase64String(imageData)}";

                                            PatientImage benhnhan = new PatientImage
                                            {
                                                Value = base64String,
                                                ID = Convert.ToInt64(reader["ID"]).ToString(),
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
        public List<PatientDataImage> GetPatientDataImageByCode(string code, string doctorCode)
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
                                string query = $@"SELECT  bnll.NGAY AS NGAYKETQUA, bnll.mabn,bn.HOTEN, bn.PHAI, bn.NAMSINH,dt.didong,(bn.sonha ||', '|| px.TENPXA ||', '|| qu.TENQUAN ||', '|| tt.TENTT)as DIACHI,  bnll.ID, bnct.chandoan,  bnct.DONGIA,  bnct.MOTA,  bnct.KETLUAN,  bscd.HOTEN AS BSCHIDINH,  bsth.HOTEN AS BSTHUCHIEN, bnct.MABS, kt.TEN AS TENKYTHUAT,lkt.TEN AS TENLOAIKT, im.image 
                                            FROM mqsoftdaiphuoc{year}.cdha_bnct bnct
                                            INNER JOIN mqsoftdaiphuoc{year}.cdha_bnll bnll ON bnll.ID = bnct.ID
                                            inner JOIN mqsoftdaiphuoc.dmbs bscd ON bnct.MABS = bscd.MA AND bscd.HIDE = 0
                                            inner JOIN mqsoftdaiphuoc.dmbs bsth ON bsth.MA = bnll.MABS 
                                            LEFT JOIN mqsoftdaiphuoc.cdha_kythuat kt ON kt.ID = bnct.MAKT
                                            LEFT JOIN mqsoftdaiphuoc.cdha_loai lkt ON lkt.ID = kt.ID_LOAI
                                            LEFT JOIN mqsoftdaiphuocimage{year}.cdha_image im ON im.id = bnct.ID
                                            inner join mqsoftdaiphuoc.btdbn bn on bn.mabn=bnll.mabn
                                            inner join mqsoftdaiphuoc.dienthoai dt on dt.mabn=bnll.mabn
                                            INNER JOIN mqsoftdaiphuoc.btdtt tt ON tt.matt = bn.matt
                                            INNER JOIN mqsoftdaiphuoc.btdquan qu ON qu.maqu = bn.maqu
                                            INNER JOIN mqsoftdaiphuoc.btdpxa px ON px.maphuongxa = bn.maphuongxa                                           
                                            WHERE bnct.mabs = :doctorCode  AND bnll.mabn = :code";

                                using (OracleCommand command = new OracleCommand(query, connection))
                                {                                  
                                    command.Parameters.Add(new OracleParameter(":doctorCode", OracleDbType.Varchar2, doctorCode, ParameterDirection.Input));
                                    command.Parameters.Add(new OracleParameter(":code", OracleDbType.Varchar2, code, ParameterDirection.Input));

                                    using (OracleDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            string base64Image = string.Empty;
                                            if (reader["image"] != DBNull.Value )
                                            {
                                                byte[] imageData = (byte[])reader["image"];
                                                base64Image = $"data:image/png;base64,{Convert.ToBase64String(imageData)}";
                                            }
                                            // Tìm xem bệnh nhân đã tồn tại trong danh sách chưa
                                            var existingDetail = cds.FirstOrDefault(p => p.ID == reader["ID"].ToString());

                                            if (existingDetail != null)
                                            {
                                                existingDetail.ImageBase64List.Add(base64Image);
                                            }
                                            else
                                            {
                                                PatientDataImage cd = new PatientDataImage
                                                {
                                                    ID = Convert.ToInt64(reader["ID"]).ToString(),
                                                    MABN = reader["MABN"].ToString(),
                                                    HOTEN = reader["HOTEN"].ToString() ,
                                                    DIACHI = reader["DIACHI"].ToString(),
                                                    NAMSINH= reader["NAMSINH"].ToString(),
                                                    PHAI= reader["PHAI"].ToString(),
                                                    DIDONG= reader["DIDONG"].ToString(),
                                                    CHANDOAN= reader["CHANDOAN"].ToString(),
                                                    DONGIA = Convert.ToDouble(reader["DONGIA"]),
                                                    MOTA = RtfExtensions.ConvertRtfToStr(reader["MOTA"].ToString()),
                                                    KETLUAN = RtfExtensions.ConvertRtfToStr(reader["KETLUAN"].ToString()),
                                                    BSCHIDINH = reader["BSCHIDINH"].ToString(),
                                                    BSTHUCHIEN = reader["BSTHUCHIEN"].ToString(),
                                                    TENKYTHUAT = reader["TENKYTHUAT"].ToString(),
                                                    TENLOAIKT = reader["TENLOAIKT"].ToString(),
                                                    //NGAYCHUP = reader["NGAYCHUP"].ToString(),
                                                    NGAYKETQUA = Convert.ToDateTime(reader["NGAYKETQUA"]).Date,
                                                    MABS = reader["MABS"].ToString(),
                                                    ImageBase64List = string.IsNullOrEmpty(base64Image) ? new List<string>(): new List<string> { base64Image }                                               
                                                    
                                                };
                                                    cds.Add(cd);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //return cds.GroupBy(n=>n.NGAYKETQUA).Select(g=>g.First()).OrderByDescending(n=>n.NGAYKETQUA).ToList();
            return cds.ToList();
        }



        #region Get ngày kết quả siêu âm (chưa dùng)
        public List<CountNgayCDHA> NgayCDHA(string doctorCode, string code)
        {
            List<CountNgayCDHA> lists = new List<CountNgayCDHA>();
            try
            {
                if (doctorCode == null)
                { return lists; }

            }
            catch (Exception)
            {

                throw;
            }
            string[] yearArray = null; // Khởi tạo mảng năm

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
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
                                    string query = $@"SELECT distinct NVL(a.NGAY, SYSDATE) as NGAYKETQUA, a.mabn,a.ID,b.DONGIA,b.MOTA,b.KETLUAN,dm.HOTEN as HOTENBS, b.MABS,kt.TEN as TENKYTHUAT,lkt.TEN as TENLOAIKT, NGAYCHUP as NGAYCHUP
                                        FROM mqsoftdaiphuoc{item}.cdha_bnll a LEFT JOIN mqsoftdaiphuoc{item}.cdha_bnct b ON a.ID = b.ID
                                        left join mqsoftdaiphuoc.dmbs dm on b.MABS = dm.MA and dm.hide = 0
                                        left join mqsoftdaiphuoc.cdha_kythuat kt on kt.ID = b.MAKT
                                        left join mqsoftdaiphuoc.cdha_loai lkt on lkt.ID = kt.ID_LOAI where b.mabs = :doctorCode and a.mabn = :code";

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
                                                        //NGAYCDHA = Convert.ToDateTime(reader["NGAYKETQUA"].ToString()),
                                                        NGAYCDHA = Convert.ToDateTime(reader["NGAYKETQUA"]).Date,
                                                        MABN = reader["MABN"].ToString(),
                                                        MABS = reader["MABS"].ToString()
                                                    };

                                                    lists.Add(ngayCDHA);
                                                }
                                            }                                       
                                    }
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching analysis data", ex);
                }

            }
            return lists.GroupBy(n=>n.NGAYCDHA).Select(g=>g.First()).OrderByDescending(p => p.NGAYCDHA).ToList();
                 // Nhóm theo NGAYCDHA
                // Chọn phần tử đầu tiên trong mỗi nhóm
                // Sắp xếp theo ngày tăng dần
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