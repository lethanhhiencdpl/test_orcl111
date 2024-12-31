using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using test_orcl.Models;
using test_orcl.ViewModels;
using test_orcl.Models.DataAccess;
using Oracle.ManagedDataAccess.Client;
using static test_orcl.Models.Ganeral;
using System.Net.NetworkInformation;
using System.Net;
using System.Web.UI.WebControls;
using System.Xml.Linq;

//pdf
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text.pdf.qrcode;
//using System.Web.UI.WebControls;
using System.Drawing.Printing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;





namespace test_orcl.Controllers
{
    public class DoctorController : Controller
    {
        private ListBenhAnPhongKham _repository = new ListBenhAnPhongKham();
        // GET: Doctor
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ChiTietXetNghiem(DateTime Date, string code, string doctorCode)
        {

            if (string.IsNullOrEmpty(code))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Hết hạn đăng nhập, vui lòng đặt nhập lại mã bác sĩ");
            }
            //List<Analysis> medicalRecords = _repository.KetQuaXN_BS(doctorCode, Date, code);
            var medicalRecords = _repository.KetQuaXN_BS(doctorCode, Date, code) ?? new List<Analysis>();

            var viewModel = new ChiTietXetNghiemViewModels
            {
                Records = medicalRecords.OrderBy(n => n.NHOMXETNGHIEM).ThenBy(n => n.STT).ToList(),
                Message = !medicalRecords.Any() ? "Không có dữ liệu xét nghiệm" : null
            };

            //truyền view pdf
            // Truyền dữ liệu qua ViewBag
            ViewBag.MABN = code;
            ViewBag.DoctorCode = doctorCode;
            ViewBag.Date = Date.ToString("yyyy-MM-dd");

            return View(viewModel);
        }

        #region Xuat PDF ket qua Xet Nghiem
        public ActionResult ExportPDF(DateTime Date, string code, string doctorCode)
        {
            // Lấy dữ liệu từ repository
            List<Analysis> analyses = _repository.KetQuaXN_BS(doctorCode, Date, code);

            if (analyses == null || !analyses.Any())
            {
                return RedirectToAction("ChiTietXetNghiem"); // Trường hợp không có dữ liệu
            }

            using (MemoryStream ms = new MemoryStream())
            {
                // Tạo tài liệu PDF
                // Tạo tài liệu PDF với kích thước phù hợp mobile
                Rectangle pageSize = new Rectangle(420, 595); // Kích thước mobile

                Document doc = new Document(PageSize.A4, 30, 30, 20, 20);
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // Đường dẫn đến font Unicode
                string fontPath = Server.MapPath("~/Content/Fonts/arial.ttf");
                BaseFont unicodeFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                // Định nghĩa các font cần sử dụng
                Font titleFont = new Font(unicodeFont, 16, Font.BOLD, BaseColor.RED);
                Font subTitleFont = new Font(unicodeFont, 12, Font.BOLD, BaseColor.BLACK);
                Font normalFont = new Font(unicodeFont, 12, Font.NORMAL);

                // **1. Thêm logo**
                string logoPath1 = Server.MapPath("~/Content/Images/logo.png"); // Đường dẫn logo 1
                string logoPath2 = Server.MapPath("~/Content/Images/logo2.png"); // Đường dẫn logo 2

                // Tải logo thứ nhất
                iTextSharp.text.Image logo1 = iTextSharp.text.Image.GetInstance(logoPath1);
                logo1.ScaleToFit(70, 70);

                // Tải logo thứ hai
                iTextSharp.text.Image logo2 = iTextSharp.text.Image.GetInstance(logoPath2);
                logo2.ScaleToFit(70, 70);

                // **2. Tạo bảng chứa logo và tiêu đề**
                PdfPTable headerTable = new PdfPTable(3);
                headerTable.WidthPercentage = 100;
                headerTable.SetWidths(new float[] { 2, 2, 6 }); // Tỷ lệ giữa các cột: logo 1, logo 2, tiêu đề
                headerTable.SpacingBefore = 0;
                headerTable.SpacingAfter = 10;

                // Cột 1: Logo thứ nhất
                PdfPCell cellLogo1 = new PdfPCell(logo1)
                {
                    Border = Rectangle.NO_BORDER,
                    Padding = 0,
                    HorizontalAlignment = Element.ALIGN_RIGHT // Căn giữa logo trong cột
                };
                headerTable.AddCell(cellLogo1);

                // Cột 2: Logo thứ hai
                PdfPCell cellLogo2 = new PdfPCell(logo2)
                {
                    Border = Rectangle.NO_BORDER,
                    Padding = 0,
                    HorizontalAlignment = Element.ALIGN_LEFT // Căn giữa logo trong cột
                };
                headerTable.AddCell(cellLogo2);

                // **3. Tiêu đề**
                PdfPCell cellTitle = new PdfPCell
                {
                    Border = Rectangle.NO_BORDER,
                    Padding = 0,
                    HorizontalAlignment = Element.ALIGN_CENTER, // Căn giữa nội dung trong cột
                    VerticalAlignment = Element.ALIGN_MIDDLE    // Căn giữa theo chiều dọc
                };

                // Tạo đoạn văn tiêu đề với khoảng cách giữa các dòng
                Paragraph titleParagraph = new Paragraph
                {
                    new Chunk("CÔNG TY TNHH Y TẾ ĐẠI PHƯỚC\n", subTitleFont),
                    new Chunk("PHÒNG XÉT NGHIỆM\n", subTitleFont),
                    new Chunk("PHIẾU KẾT QUẢ XÉT NGHIỆM", titleFont)
                };
                titleParagraph.Alignment = Element.ALIGN_CENTER;
                titleParagraph.SetLeading(15, 1.2f); // Điều chỉnh khoảng cách dòng: 15 điểm, tỷ lệ 1.2

                // Thêm đoạn văn tiêu đề vào ô
                cellTitle.AddElement(titleParagraph);
                headerTable.AddCell(cellTitle);

                doc.Add(headerTable); // Thêm bảng logo và tiêu đề vào PDF

                //**3.Thêm thông tin bệnh nhân**
                var firstRecord = analyses.FirstOrDefault();
                // Thêm thông tin bệnh nhân
                // Tạo bảng với 2 cột
                PdfPTable patientInfoTable = new PdfPTable(4)
                {
                    WidthPercentage = 100 // Chiếm 100% chiều rộng trang
                };
                patientInfoTable.SetWidths(new float[] { 2, 4, 2, 2 }); // Tỷ lệ chiều rộng giữa các cột: 3:7

                patientInfoTable.AddCell(new PdfPCell(new Phrase("Họ tên:", normalFont)) { Border = Rectangle.NO_BORDER });
                patientInfoTable.AddCell(new PdfPCell(new Phrase(firstRecord.HOTEN, normalFont)) { Border = Rectangle.NO_BORDER });

                patientInfoTable.AddCell(new PdfPCell(new Phrase("Năm sinh:", normalFont)) { Border = Rectangle.NO_BORDER });
                patientInfoTable.AddCell(new PdfPCell(new Phrase(firstRecord.NAMSINH.ToString(), normalFont)) { Border = Rectangle.NO_BORDER });

                patientInfoTable.AddCell(new PdfPCell(new Phrase("Mã BN:", normalFont)) { Border = Rectangle.NO_BORDER });
                patientInfoTable.AddCell(new PdfPCell(new Phrase(firstRecord.MABN, normalFont)) { Border = Rectangle.NO_BORDER });

                patientInfoTable.AddCell(new PdfPCell(new Phrase("Giới tính:", normalFont)) { Border = Rectangle.NO_BORDER });
                patientInfoTable.AddCell(new PdfPCell(new Phrase(firstRecord.PHAI.ToString() == "1" ? "Nữ" : "Nam", normalFont)) { Border = Rectangle.NO_BORDER });

                patientInfoTable.AddCell(new PdfPCell(new Phrase("Ngày lấy mẫu:", normalFont)) { Border = Rectangle.NO_BORDER });
                patientInfoTable.AddCell(new PdfPCell(new Phrase(Date.ToString("dd/MM/yyyy"), normalFont)) { Border = Rectangle.NO_BORDER });

                patientInfoTable.AddCell(new PdfPCell(new Phrase("Loại mẫu:", normalFont)) { Border = Rectangle.NO_BORDER });
                patientInfoTable.AddCell(new PdfPCell(new Phrase("Máu", normalFont)) { Border = Rectangle.NO_BORDER });

                doc.Add(patientInfoTable);

                Paragraph patientInfo = new Paragraph(
                    $"Địa chỉ: {firstRecord.DIACHI}  \n" +
                    $"Bác sĩ chỉ định: {firstRecord.HOTENBS}\n" +
                    $"Chẩn đoán: {firstRecord.CHANDOAN}\n",
                    normalFont
                )
                {
                    SpacingAfter = 20
                };
                doc.Add(patientInfo);


                // **4. Thêm bảng kết quả**
                PdfPTable table = new PdfPTable(5) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 6, 3, 3, 2, 4 }); // Định tỷ lệ giữa các cột

                // Thêm tiêu đề cột
                Font headerFont = new Font(unicodeFont, 12, Font.BOLD, BaseColor.BLACK);
                //Font normalFont = new Font(unicodeFont, 12, Font.NORMAL, BaseColor.BLACK);
                Font highlightFont = new Font(unicodeFont, 12, Font.BOLD, BaseColor.RED); // Font màu đỏ

                table.AddCell(new PdfPCell(new Phrase("TÊN XÉT NGHIỆM", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("KẾT QUẢ", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("GIÁ TRỊ THAM CHIẾU", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("ĐƠN VỊ", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("MÁY XN", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_CENTER });



                // **Nhóm các kết quả theo tiêu đề**
                string currentGroup = null;


                // Thêm dữ liệu xét nghiệm
                foreach (var record in analyses.OrderBy(n => n.NHOMXETNGHIEM).ThenBy(n => n.STT))
                {
                    //nhóm 

                    // Kiểm tra nhóm kết quả mới
                    if (record.NHOMXETNGHIEM != currentGroup)
                    {
                        currentGroup = record.NHOMXETNGHIEM;
                        PdfPCell groupCell = new PdfPCell(new Phrase(record.NHOMXETNGHIEM, headerFont))
                        {
                            Colspan = 5,
                            BackgroundColor = new BaseColor(220, 220, 220),
                            HorizontalAlignment = Element.ALIGN_LEFT,
                            PaddingTop = 5,
                            PaddingBottom = 5
                        };
                        table.AddCell(groupCell);
                    }




                    // Thêm Tên Xét Nghiệm------------------------------
                    table.AddCell(new PdfPCell(new Phrase(record.TENXETNGHIEM, normalFont)) { HorizontalAlignment = Element.ALIGN_LEFT });

                    // Kiểm tra giá trị Kết Quả

                    Font resultFont = normalFont; // Mặc định font màu đen
                    if (record.GHICHU?.Trim() == "H")
                    {
                        resultFont = highlightFont; // Font màu đỏ
                    }
                    else if (record.GHICHU?.Trim() == "L")
                    {
                        resultFont = new Font(unicodeFont, 12, Font.BOLD, BaseColor.BLUE); // Font màu xanh
                    }

                    // Thêm KẾT QUẢ với màu sắc phù hợp
                    table.AddCell(new PdfPCell(new Phrase(record.KETQUA, resultFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                    //// Nếu giá trị nằm ngoài khoảng, đổi màu đỏ
                    //table.AddCell(new PdfPCell(new Phrase(record.KETQUA, isOutOfRange ? highlightFont : normalFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                    // Thêm Giá Trị Tham Chiếu
                    table.AddCell(new PdfPCell(new Phrase(record.CSBT, normalFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                    // Thêm Đơn Vị
                    table.AddCell(new PdfPCell(new Phrase(record.DONVI, normalFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                    // Thêm Máy Xét Nghiệm
                    table.AddCell(new PdfPCell(new Phrase(record.MAYXETNGHIEM, normalFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                }

                // Thêm bảng vào tài liệu
                doc.Add(table);
                //Them ck
                Paragraph ckNV = new Paragraph(
                   //$"Họ tên: {firstRecord.HOTEN}" +                 
                   $"\n" +
                   "TRƯỞNG KHOA XÉT NGHIỆM\n" +
                   "\n" +
                   "\n" +
                   "\n" +
                   "\n" +
                   "CN.CK1 LÊ HỮU DŨNG",

                   normalFont
               )
                {
                    SpacingAfter = 20
                };
                ckNV.Alignment = Element.ALIGN_RIGHT;
                doc.Add(ckNV);




                //FOOTER
                // **1. Thêm hình ảnh footer**
                string footerImagePath = Server.MapPath("~/Content/Images/footer.png"); // Đường dẫn hình ảnh footer
                if (!System.IO.File.Exists(footerImagePath))
                {
                    throw new FileNotFoundException("Không tìm thấy file footer.", footerImagePath);
                }

                iTextSharp.text.Image footerImage = iTextSharp.text.Image.GetInstance(footerImagePath);
                footerImage.ScaleToFit(700, 70); // Điều chỉnh kích thước hình ảnh
                footerImage.Alignment = Element.ALIGN_CENTER;
                //Rectangle pageSize = doc.PageSize;
                footerImage.SetAbsolutePosition(pageSize.GetLeft(30), pageSize.GetBottom(20)); // Vị trí dưới cùng

                // Tính toán vị trí của footer để thêm vào cuối trang
                PdfPTable footerTable = new PdfPTable(1);
                footerTable.WidthPercentage = 100;
                footerTable.SpacingBefore = 10; // Khoảng cách từ nội dung đến footer

                PdfPCell footerCell = new PdfPCell(footerImage)
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                footerTable.AddCell(footerCell);
                doc.Add(footerTable);

                // Kết thúc tài liệu
                doc.Close();

                // Trả về file PDF
                //byte[] fileBytes = ms.ToArray();
                //return File(fileBytes, "application/pdf");
                // Trả về PDF
                Response.Headers.Add("Content-Disposition", "inline; filename="+firstRecord.MABN+"_XN.pdf");
                byte[] fileBytes = ms.ToArray();
                return File(fileBytes, "application/pdf");
            }

        }


        #endregion

        [HttpGet]
        public PartialViewResult _NgayXetNghiemPartial(string code, string doctorCode)
        {
            List<CountNgayXetNghiem> medicalRecords = _repository.NgayXetNghiem(doctorCode, code);
            return PartialView("_NgayXetNghiemPartial", medicalRecords.OrderByDescending(n => n.NGAYXETNGHIEM));
        }

        [HttpGet]
        public PartialViewResult _HienThiTTBenhNhanPartial(string code)
        {
            List<BTDBenhNhan> TTBenhNhan = _repository.GetThongTinBN(code);
            return PartialView("_HienThiTTBenhNhanPartial", TTBenhNhan);
        }


        //CDHA
        public ActionResult ChiTietCHDA(string code, string doctorCode , DateTime Date)
        {
            if (code == null || doctorCode == null)
            {
                return RedirectToAction("Login", "HomeAdmin");
            }
            List<PatientDataImage> medicalRecords = _repository.GetPatientDataImageByCode(code, doctorCode);

            // Xử lý mô tả để ngắt dòng theo STT
            foreach (var sieuAm in medicalRecords)
            {
                sieuAm.MOTA = FormatDescription(sieuAm.MOTA);
            }

            // Truyền dữ liệu qua ViewBag
            ViewBag.MABN = code;
            ViewBag.DoctorCode = doctorCode;
            ViewBag.Date = Date;
            return View(medicalRecords.ToList());
        }

        // Hàm xử lý mô tả ngắt dòng theo STT
        private string FormatDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                return description;
            // Tìm các vị trí của số thứ tự (1., 2., ...) và thêm thẻ <br> để ngắt dòng
            string pattern = @"(\d+\.)"; // Tìm số thứ tự kết thúc bằng dấu chấm
            string formattedDescription = Regex.Replace(description, pattern, "<br>$1");

            // Xóa <br> dư thừa ở đầu chuỗi
            formattedDescription = formattedDescription.TrimStart("<br>".ToCharArray());
            return formattedDescription;
        }



        //Ngày CDHA chưa dùng
        [HttpGet]
        public PartialViewResult _NgayCDHAPartial(string code, string doctorCode)
        {
            List<CountNgayCDHA> medicalRecords = _repository.NgayCDHA(doctorCode, code);
            return PartialView("_NgayCDHAPartial", medicalRecords.Distinct().OrderByDescending(n => n.NGAYCDHA));
        }



        #region Xuat PDF ket qua CDHA
        // đang dùng ok
        public ActionResult ExportPDF_CDHA(DateTime Date, string code, string doctorCode)
        {
            // Lấy dữ liệu từ repository
            List<PatientDataImage> cdha = _repository.GetPatientDataImageByCode(code, doctorCode);

            if (cdha == null || !cdha.Any())
            {
                return RedirectToAction("ChiTietCDHA"); // Trường hợp không có dữ liệu
            }

            using (MemoryStream ms = new MemoryStream())
            {
                Rectangle pageSize = new Rectangle(420, 595); // Kích thước mobile
                // Tạo tài liệu PDF
                Document doc = new Document(PageSize.A4, 30, 30, 20, 20);
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                //Document doc = null;
                //PdfWriter writer = null;

                // Đường dẫn đến font Unicode
                string fontPath = Server.MapPath("~/Content/Fonts/arial.ttf");
                BaseFont unicodeFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                // Định nghĩa các font cần sử dụng
                Font titleFont = new Font(unicodeFont, 14, Font.BOLD, BaseColor.BLUE);
                Font normalFont = new Font(unicodeFont, 10, Font.NORMAL);
                Font subTitleFont = new Font(unicodeFont, 10, Font.BOLD);

                // Lọc dữ liệu theo loại
                var sieuAmData = cdha.Where(c => c.TENLOAIKT.Contains("Siêu âm") && c.NGAYKETQUA == Date).ToList();
                var noiSoiData = cdha.Where(c => c.TENLOAIKT.Contains("Nội soi") && c.NGAYKETQUA == Date).ToList();
                var DodaymatData = cdha.Where(c => c.TENLOAIKT.Contains("Đo đáy mắt") && c.NGAYKETQUA == Date).ToList();
                var XquangData = cdha.Where(c => c.TENLOAIKT.Contains("X quang") && c.NGAYKETQUA == Date).ToList();
                var CtData = cdha.Where(c => c.TENLOAIKT.Contains("CT Scan") && c.NGAYKETQUA == Date).ToList();
                var DientimData = cdha.Where(c => c.TENLOAIKT.Contains("Điện tim") && c.NGAYKETQUA == Date).ToList();

                var FibroData = cdha.Where(c => c.TENLOAIKT.Contains("Fibroscan") && c.NGAYKETQUA == Date).ToList();
                var DiennaoData = cdha.Where(c => c.TENLOAIKT.Contains("Điện não") && c.NGAYKETQUA == Date).ToList();
                var DiencoData = cdha.Where(c => c.TENLOAIKT.Contains("Điện cơ") && c.NGAYKETQUA == Date).ToList();
                var GiaiphaubenhData = cdha.Where(c => c.TENLOAIKT.Contains("Giải phẩu bệnh") && c.NGAYKETQUA == Date).ToList();
                var HohapData = cdha.Where(c => c.TENLOAIKT.Contains("Hô hấp ký") && c.NGAYKETQUA == Date).ToList();
                var LoangxuongData = cdha.Where(c => c.TENLOAIKT.Contains("Đo loãng xương") && c.NGAYKETQUA == Date).ToList();
                var NhuanhData = cdha.Where(c => c.TENLOAIKT.Contains("Chụp nhũ ảnh") && c.NGAYKETQUA == Date).ToList();
                var ShptData = cdha.Where(c => c.TENLOAIKT.Contains("Sinh học phân tử") && c.NGAYKETQUA == Date).ToList();
                var C13Data = cdha.Where(c => c.TENLOAIKT.Contains("C13") && c.NGAYKETQUA == Date).ToList();

                //var XetnghiemData = analyses.Where(c => c.TENLOAIKT.Contains("Sinh học phân tử") && c.NGAYKETQUA == Date);

                var firRecord = cdha.FirstOrDefault();
                #region Kết quả riêng cho siêu âm
                // Xử lý dữ liệu Siêu âm
                if (sieuAmData.Any())
                {

                    foreach (var record in sieuAmData)
                    {

                        doc.NewPage();// Trang mới cho nội soi
                        // Tiêu đề
                        Paragraph noiSoiTitle = new Paragraph(record.TENKYTHUAT.ToUpper(), titleFont)
                        {
                            Alignment = Element.ALIGN_CENTER,
                            SpacingAfter = 10
                        };
                        doc.Add(noiSoiTitle);

                        //Thông tin
                        // Tạo bảng thông tin bệnh nhân
                        PdfPTable patientInfoTable = new PdfPTable(4)
                        {
                            WidthPercentage = 100,
                            SpacingBefore = 0, // Giảm khoảng cách trước bảng
                            SpacingAfter = 0  // Giảm khoảng cách sau bảng
                        };
                        patientInfoTable.SetWidths(new float[] { 2, 4, 2, 2 }); // Tỷ lệ chiều rộng giữa các cột: 

                        patientInfoTable.AddCell(new PdfPCell(new Phrase("Họ tên:", normalFont)) { Border = Rectangle.NO_BORDER });
                        patientInfoTable.AddCell(new PdfPCell(new Phrase(firRecord.HOTEN, normalFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT });

                        patientInfoTable.AddCell(new PdfPCell(new Phrase("Ngày sinh:", normalFont)) { Border = Rectangle.NO_BORDER });
                        patientInfoTable.AddCell(new PdfPCell(new Phrase(firRecord.NAMSINH, normalFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT });

                        patientInfoTable.AddCell(new PdfPCell(new Phrase("Mã BN:", normalFont)) { Border = Rectangle.NO_BORDER });
                        patientInfoTable.AddCell(new PdfPCell(new Phrase(firRecord.MABN, normalFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT });

                        patientInfoTable.AddCell(new PdfPCell(new Phrase("Điện thoại:", normalFont)) { Border = Rectangle.NO_BORDER });
                        patientInfoTable.AddCell(new PdfPCell(new Phrase(firRecord.DIDONG, normalFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT });

                        doc.Add(patientInfoTable);

                        Paragraph patientInfo = new Paragraph(
                            $"Địa chỉ: {firRecord.DIACHI}  \n" +
                            $"Bác sĩ chỉ định: {firRecord.BSCHIDINH}\n" +
                            $"Chẩn đoán: {firRecord.CHANDOAN}\n",
                            normalFont
                        )
                        {
                            Leading = 12, // Khoảng cách dòng
                            SpacingBefore = 3, // Giảm khoảng cách trước đoạn
                            SpacingAfter = 5 // Khoảng cách sau đoạn nếu cần
                        };
                        doc.Add(patientInfo);


                        PdfPTable layoutTable = new PdfPTable(2)
                        {
                            WidthPercentage = 100,
                            SpacingBefore = 10
                        };
                        layoutTable.SetWidths(new float[] { 2, 2 }); // Chia tỷ lệ mô tả và hình ảnh

                        // Phần mô tả
                        PdfPCell descriptionCell = new PdfPCell()
                        {
                            Border = Rectangle.NO_BORDER
                        };
                        // Mô tả
                        Paragraph mota = new Paragraph("Mô tả:\n" + record.MOTA, normalFont)
                        {
                            Leading = 12, // Khoảng cách dòng
                            SpacingAfter = 3 // Giảm khoảng cách sau đoạn
                        };

                        // Kết luận
                        Paragraph ketluan = new Paragraph("Kết luận:\n" + record.KETLUAN, subTitleFont)
                        {
                            Leading = 12, // Khoảng cách dòng
                            SpacingBefore = 3, // Giảm khoảng cách trước đoạn
                            SpacingAfter = 5 // Khoảng cách sau đoạn nếu cần
                        };
                        descriptionCell.AddElement(mota);
                        descriptionCell.AddElement(ketluan);
                        layoutTable.AddCell(descriptionCell);


                        // Phần hình ảnh
                        PdfPCell imageCell = new PdfPCell()
                        {
                            Border = Rectangle.NO_BORDER,
                            HorizontalAlignment = Element.ALIGN_CENTER
                        };
                        if (record.ImageBase64List.Any())
                        {
                            PdfPTable imageGrid = new PdfPTable(2) // Chia hình ảnh thành 2 cột
                            {
                                WidthPercentage = 100
                            };

                            foreach (var base64Image in record.ImageBase64List)
                            {
                                if (!string.IsNullOrWhiteSpace(base64Image) && base64Image.StartsWith("data:image/png;base64,"))
                                {
                                    try
                                    {
                                        string cleanBase64 = base64Image.Replace("data:image/png;base64,", "");
                                        byte[] imageBytes = Convert.FromBase64String(cleanBase64);
                                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageBytes);
                                        image.ScaleToFit(130f, 130f);
                                        PdfPCell imgCell = new PdfPCell(image)
                                        {
                                            Border = Rectangle.NO_BORDER,
                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                            Padding = 5
                                        };
                                        imageGrid.AddCell(imgCell);
                                    }
                                    catch { }
                                }
                            }

                            // Nếu số lượng hình ảnh lẻ, thêm ô trống để cân bằng
                            if (record.ImageBase64List.Count % 2 != 0)
                            {
                                PdfPCell emptyCell = new PdfPCell()
                                {
                                    Border = Rectangle.NO_BORDER
                                };
                                imageGrid.AddCell(emptyCell);
                            }

                            imageCell.AddElement(imageGrid);
                        }
                        else
                        {
                            Paragraph noImageText = new Paragraph("Không có hình ảnh", normalFont)
                            {
                                Alignment = Element.ALIGN_CENTER
                            };
                            imageCell.AddElement(noImageText);
                        }
                        layoutTable.AddCell(imageCell);

                        // Thêm bảng hình ảnh vào tài liệu
                        doc.Add(layoutTable);


                        // Thêm ngày tháng và tên bác sĩ ngay dưới hình ảnh
                        PdfPTable signatureTable = new PdfPTable(1)
                        {
                            WidthPercentage = 100,
                            SpacingBefore = 10 // Khoảng cách trước đoạn chữ ký
                        };
                        signatureTable.DefaultCell.Border = Rectangle.NO_BORDER;

                        // Thêm ngày tháng
                        PdfPCell dateCell = new PdfPCell(new Phrase($"Ngày {record.NGAYKETQUA:dd} tháng {record.NGAYKETQUA:MM} năm {record.NGAYKETQUA:yyyy}", normalFont))
                        {
                            Border = Rectangle.NO_BORDER,
                            HorizontalAlignment = Element.ALIGN_RIGHT,
                            PaddingBottom = 5
                        };
                        signatureTable.AddCell(dateCell);

                        // Thêm tên bác sĩ
                        PdfPCell doctorCell = new PdfPCell(new Phrase(record.BSTHUCHIEN, subTitleFont))
                        {
                            Border = Rectangle.NO_BORDER,
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                        signatureTable.AddCell(doctorCell);

                        // Thêm bảng chữ ký vào tài liệu
                        doc.Add(signatureTable);

                        doc.Add(new Paragraph("\n"));

                    }
                }
                #endregion

                #region Kết quả riêng cho nội soi                
                // Xử lý dữ liệu Nội soi
                if (noiSoiData.Any())
                {
                    foreach (var record in noiSoiData)
                    {
                        doc.NewPage();
                        // Tiêu đề
                        Paragraph sieuAmTitle = new Paragraph(record.TENKYTHUAT.ToUpper(), titleFont)
                        {
                            Alignment = Element.ALIGN_CENTER,
                            SpacingAfter = 10
                        };
                        doc.Add(sieuAmTitle);

                        //Thông tin

                        PdfPTable patientInfoTable = new PdfPTable(4)
                        {
                            WidthPercentage = 100
                        };
                        patientInfoTable.SetWidths(new float[] { 2, 4, 2, 2 }); // Tỷ lệ chiều rộng giữa các cột: 3:7

                        patientInfoTable.AddCell(new PdfPCell(new Phrase("Họ tên:", subTitleFont)) { Border = Rectangle.NO_BORDER });
                        patientInfoTable.AddCell(new PdfPCell(new Phrase(firRecord.HOTEN, normalFont)) { Border = Rectangle.NO_BORDER });

                        patientInfoTable.AddCell(new PdfPCell(new Phrase("Ngày sinh:", subTitleFont)) { Border = Rectangle.NO_BORDER });
                        patientInfoTable.AddCell(new PdfPCell(new Phrase(firRecord.NAMSINH, normalFont)) { Border = Rectangle.NO_BORDER });

                        patientInfoTable.AddCell(new PdfPCell(new Phrase("Mã BN:", subTitleFont)) { Border = Rectangle.NO_BORDER });
                        patientInfoTable.AddCell(new PdfPCell(new Phrase(firRecord.MABN, normalFont)) { Border = Rectangle.NO_BORDER });

                        patientInfoTable.AddCell(new PdfPCell(new Phrase("Điện thoại:", subTitleFont)) { Border = Rectangle.NO_BORDER });
                        patientInfoTable.AddCell(new PdfPCell(new Phrase(firRecord.DIDONG, normalFont)) { Border = Rectangle.NO_BORDER });

                        doc.Add(patientInfoTable);

                        Paragraph patientInfo = new Paragraph(
                            $"Địa chỉ: {firRecord.DIACHI}  \n" +
                            $"Bác sĩ chỉ định: {firRecord.BSCHIDINH}\n" +
                            $"Chẩn đoán: {firRecord.CHANDOAN}\n",
                            normalFont
                        )
                        {
                            Leading = 12, // Khoảng cách dòng
                            SpacingBefore = 3, // Giảm khoảng cách trước đoạn
                            SpacingAfter = 5 // Khoảng cách sau đoạn nếu cần
                        };
                        doc.Add(patientInfo);

                        PdfPTable layoutTable = new PdfPTable(2)
                        {
                            WidthPercentage = 100,
                            SpacingBefore = 10
                        };
                        layoutTable.SetWidths(new float[] { 2, 2 }); // Chia tỷ lệ mô tả và hình ảnh

                        // Phần mô tả
                        PdfPCell descriptionCell = new PdfPCell()
                        {
                            Border = Rectangle.NO_BORDER
                        };
                        // Mô tả
                        Paragraph mota = new Paragraph("Mô tả:\n" + record.MOTA, normalFont)
                        {
                            Leading = 12, // Khoảng cách dòng
                            SpacingAfter = 3 // Giảm khoảng cách sau đoạn
                        };

                        // Kết luận
                        Paragraph ketluan = new Paragraph("Kết luận:\n" + record.KETLUAN, subTitleFont)
                        {
                            Leading = 12, // Khoảng cách dòng
                            SpacingBefore = 3, // Giảm khoảng cách trước đoạn
                            SpacingAfter = 5 // Khoảng cách sau đoạn nếu cần
                        };
                        descriptionCell.AddElement(mota);
                        descriptionCell.AddElement(ketluan);
                        layoutTable.AddCell(descriptionCell);

                        // Phần hình ảnh
                        PdfPCell imageCell = new PdfPCell()
                        {
                            Border = Rectangle.NO_BORDER,
                            HorizontalAlignment = Element.ALIGN_CENTER
                        };
                        if (record.ImageBase64List.Any())
                        {
                            PdfPTable imageGrid = new PdfPTable(2) // Chia hình ảnh thành 2 cột
                            {
                                WidthPercentage = 100
                            };

                            foreach (var base64Image in record.ImageBase64List)
                            {
                                if (!string.IsNullOrWhiteSpace(base64Image) && base64Image.StartsWith("data:image/png;base64,"))
                                {
                                    try
                                    {
                                        string cleanBase64 = base64Image.Replace("data:image/png;base64,", "");
                                        byte[] imageBytes = Convert.FromBase64String(cleanBase64);

                                        // Gọi hàm cắt phần đen
                                        byte[] croppedImageBytes = CropBlackBorders(imageBytes);

                                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageBytes);
                                        image.ScaleToFit(130f, 130f);
                                        PdfPCell imgCell = new PdfPCell(image)
                                        {
                                            Border = Rectangle.NO_BORDER,
                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                            Padding = 5
                                        };
                                        imageGrid.AddCell(imgCell);
                                    }
                                    catch { }
                                }
                            }

                            // Nếu số lượng hình ảnh lẻ, thêm ô trống để cân bằng
                            if (record.ImageBase64List.Count % 2 != 0)
                            {
                                PdfPCell emptyCell = new PdfPCell()
                                {
                                    Border = Rectangle.NO_BORDER
                                };
                                imageGrid.AddCell(emptyCell);
                            }

                            imageCell.AddElement(imageGrid);
                        }
                        else
                        {
                            Paragraph noImageText = new Paragraph("Không có hình ảnh", normalFont)
                            {
                                Alignment = Element.ALIGN_CENTER
                            };
                            imageCell.AddElement(noImageText);
                        }
                        layoutTable.AddCell(imageCell);

                        // Thêm bảng hình ảnh vào tài liệu
                        doc.Add(layoutTable);

                        // **Thêm chữ ký và thông tin bác sĩ dưới hình ảnh**
                        Paragraph chuky = new Paragraph($"Ngày {record.NGAYKETQUA:dd} tháng {record.NGAYKETQUA:MM} năm {record.NGAYKETQUA:yyyy}\n", normalFont)
                        {
                            Alignment = Element.ALIGN_RIGHT,
                            SpacingBefore = 10, // Khoảng cách trước đoạn
                            SpacingAfter = 10
                        };
                        doc.Add(chuky);

                        Paragraph bscdha = new Paragraph(record.BSTHUCHIEN, subTitleFont)
                        {
                            Alignment = Element.ALIGN_RIGHT,
                            SpacingBefore = 3
                        };
                        doc.Add(bscdha);
                    }
                }
                #endregion

                #region Kết quả riêng cho Fibroscan
                //// Xử lý dữ liệu FibroData
                //if (FibroData.Any())
                //{
                //    foreach (var record in FibroData)
                //    {
                //        doc.NewPage(); // Tạo trang mới cho FibroData
                //        PdfPTable imageGrid = new PdfPTable(2) // Chia hình ảnh thành 2 cột
                //        {
                //            WidthPercentage = 100,
                //            SpacingBefore = 10
                //        };

                //        // Lặp qua danh sách hình ảnh
                //        foreach (var base64Image in record.ImageBase64List)
                //        {
                //            if (!string.IsNullOrWhiteSpace(base64Image) && base64Image.StartsWith("data:image/png;base64,"))
                //            {
                //                try
                //                {
                //                    string cleanBase64 = base64Image.Replace("data:image/png;base64,", "");
                //                    byte[] imageBytes = Convert.FromBase64String(cleanBase64);
                //                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageBytes);
                //                    image.ScaleToFit(500f, 600f); // Kích thước hình ảnh

                //                    PdfPCell imgCell = new PdfPCell(image)
                //                    {
                //                        Border = Rectangle.NO_BORDER,
                //                        HorizontalAlignment = Element.ALIGN_CENTER,
                //                        Padding = 5
                //                    };
                //                    imageGrid.AddCell(imgCell);
                //                }
                //                catch
                //                {
                //                    // Xử lý lỗi khi hình ảnh không hợp lệ
                //                    PdfPCell errorCell = new PdfPCell(new Phrase("Hình ảnh không hợp lệ", normalFont))
                //                    {
                //                        Border = Rectangle.NO_BORDER,
                //                        HorizontalAlignment = Element.ALIGN_CENTER
                //                    };
                //                    imageGrid.AddCell(errorCell);
                //                }
                //            }
                //        }

                //        // Nếu số lượng hình ảnh lẻ, thêm ô trống để cân bằng
                //        if (record.ImageBase64List.Count % 2 != 0)
                //        {
                //            PdfPCell emptyCell = new PdfPCell()
                //            {
                //                Border = Rectangle.NO_BORDER
                //            };
                //            imageGrid.AddCell(emptyCell);
                //        }

                //        // Thêm bảng hình ảnh vào tài liệu
                //        //doc.Add(imageGrid);
                //        // Đảm bảo tài liệu chưa bị đóng
                //        if (doc != null && doc.IsOpen())
                //        {
                //            // Thêm bảng hình ảnh vào tài liệu
                //            doc.Add(imageGrid);

                //            // Thêm khoảng trống giữa các trang (nếu cần)
                //            doc.Add(new Paragraph("\n"));
                //        }

                //        // Thêm khoảng trống giữa các trang (nếu cần)
                //        //doc.Add(new Paragraph("\n"));
                //    }
                //}
                #endregion


                #region Kết quả riêng cho điện tim
                // Xử lý các loại dữ liệu CDHA
                if (DientimData.Any())
                {
                    // Danh sách các loại kỹ thuật cần xử lý
                    var datasets = new Dictionary<string, List<PatientDataImage>>
                    {
                        { "Điện tim", DientimData },
                    };

                    // Xử lý từng loại kỹ thuật
                    foreach (var dataset in datasets)
                    {
                        var techniqueName = dataset.Key; // Tên loại kỹ thuật (Fibroscan, Điện tim, ...)
                        var dataList = dataset.Value;    // Danh sách kết quả tương ứng

                        if (dataList.Any())
                        {
                            foreach (var record in dataList)
                            {
                                doc.NewPage(); // Tạo trang mới cho từng bản ghi

                                // Tiêu đề
                                Paragraph noiSoiTitle = new Paragraph(record.TENKYTHUAT.ToUpper(), titleFont)
                                {
                                    Alignment = Element.ALIGN_CENTER,
                                    SpacingAfter = 10
                                };
                                doc.Add(noiSoiTitle);
                                

                                // Thông tin bệnh nhân
                                PdfPTable patientInfoTable = new PdfPTable(4)
                                {
                                    WidthPercentage = 100
                                };
                                patientInfoTable.SetWidths(new float[] { 2, 4, 2, 2 });

                                patientInfoTable.AddCell(new PdfPCell(new Phrase("Họ tên:", subTitleFont)) { Border = Rectangle.NO_BORDER });
                                patientInfoTable.AddCell(new PdfPCell(new Phrase(record.HOTEN, normalFont)) { Border = Rectangle.NO_BORDER });
                                patientInfoTable.AddCell(new PdfPCell(new Phrase("Ngày sinh:", subTitleFont)) { Border = Rectangle.NO_BORDER });
                                patientInfoTable.AddCell(new PdfPCell(new Phrase(record.NAMSINH, normalFont)) { Border = Rectangle.NO_BORDER });
                                patientInfoTable.AddCell(new PdfPCell(new Phrase("Mã BN:", subTitleFont)) { Border = Rectangle.NO_BORDER });
                                patientInfoTable.AddCell(new PdfPCell(new Phrase(record.MABN, normalFont)) { Border = Rectangle.NO_BORDER });
                                patientInfoTable.AddCell(new PdfPCell(new Phrase("Điện thoại:", subTitleFont)) { Border = Rectangle.NO_BORDER });
                                patientInfoTable.AddCell(new PdfPCell(new Phrase(record.DIDONG, normalFont)) { Border = Rectangle.NO_BORDER });

                                doc.Add(patientInfoTable);

                                // Thông tin mô tả và kết luận
                                Paragraph details = new Paragraph(
                                    $"Địa chỉ: {record.DIACHI}\n" +
                                    $"Bác sĩ chỉ định: {record.BSCHIDINH}\n" +
                                    $"Chẩn đoán: {record.CHANDOAN}\n",
                                    normalFont
                                )
                                {
                                    Leading = 12,
                                    SpacingBefore = 10,
                                    SpacingAfter = 3
                                };
                                doc.Add(details);

                                // Hình ảnh
                                if (record.ImageBase64List.Any())
                                {
                                    PdfPTable imageGrid = new PdfPTable(2)
                                    {
                                        WidthPercentage = 100,
                                        SpacingBefore = 10
                                    };

                                    foreach (var base64Image in record.ImageBase64List)
                                    {
                                        if (!string.IsNullOrWhiteSpace(base64Image) && base64Image.StartsWith("data:image/png;base64,"))
                                        {
                                            try
                                            {
                                                string cleanBase64 = base64Image.Replace("data:image/png;base64,", "");
                                                byte[] imageBytes = Convert.FromBase64String(cleanBase64);
                                                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageBytes);
                                                image.ScaleToFit(500f, 750f);
                                                PdfPCell imgCell = new PdfPCell(image)
                                                {
                                                    Border = Rectangle.NO_BORDER,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    Padding = 5
                                                };
                                                imageGrid.AddCell(imgCell);
                                            }
                                            catch
                                            {
                                                PdfPCell errorCell = new PdfPCell(new Phrase("Hình ảnh không hợp lệ", normalFont))
                                                {
                                                    Border = Rectangle.NO_BORDER,
                                                    HorizontalAlignment = Element.ALIGN_CENTER
                                                };
                                                imageGrid.AddCell(errorCell);
                                            }
                                        }
                                    }

                                    // Thêm ô trống nếu số lượng hình ảnh lẻ
                                    if (record.ImageBase64List.Count % 2 != 0)
                                    {
                                        PdfPCell emptyCell = new PdfPCell() { Border = Rectangle.NO_BORDER };
                                        imageGrid.AddCell(emptyCell);
                                    }

                                    doc.Add(imageGrid);
                                }
                                // Kết Luận
                                Paragraph Ketluan = new Paragraph(
                                   $"Kết luận: {record.KETLUAN}\n",
                                    subTitleFont
                                )
                                {
                                    Alignment = Element.ALIGN_LEFT,
                                    SpacingBefore = 10,
                                    SpacingAfter = 10
                                };
                                doc.Add(Ketluan);

                                // Chữ ký bác sĩ
                                Paragraph signature = new Paragraph(
                                    $"Ngày {record.NGAYKETQUA:dd} tháng {record.NGAYKETQUA:MM} năm {record.NGAYKETQUA:yyyy}\n{record.BSTHUCHIEN}",
                                    subTitleFont
                                )
                                {
                                    Alignment = Element.ALIGN_RIGHT,
                                    SpacingBefore = 10,
                                    SpacingAfter = 10
                                };
                                doc.Add(signature);
                            }
                        }
                    }
                }

                #endregion

                #region Kết quả riêng cho CT và Xquang
                // Xử lý các loại dữ liệu CDHA
                if (CtData.Any() || XquangData.Any() )
                {
                    // Danh sách các loại kỹ thuật cần xử lý
                    var datasets = new Dictionary<string, List<PatientDataImage>>
                    {
                        { "CT Scan", CtData },
                        { "X quang", XquangData },
                    };

                    // Xử lý từng loại kỹ thuật
                    foreach (var dataset in datasets)
                    {
                        var techniqueName = dataset.Key; // Tên loại kỹ thuật (Fibroscan, Điện tim, ...)
                        var dataList = dataset.Value;    // Danh sách kết quả tương ứng

                        if (dataList.Any())
                        {
                            foreach (var record in dataList)
                            {
                                doc.NewPage(); // Tạo trang mới cho từng bản ghi

                                // Tiêu đề
                                Paragraph XquangTitle = new Paragraph(record.TENKYTHUAT.ToUpper(), titleFont)
                                {
                                    Alignment = Element.ALIGN_CENTER,
                                    SpacingBefore = 20,
                                    SpacingAfter = 10
                                };
                                doc.Add(XquangTitle);

                                // Thông tin bệnh nhân
                                PdfPTable patientInfoTable = new PdfPTable(4)
                                {
                                    WidthPercentage = 100
                                };
                                patientInfoTable.SetWidths(new float[] { 2, 4, 2, 2 });

                                patientInfoTable.AddCell(new PdfPCell(new Phrase("Họ tên:", subTitleFont)) { Border = Rectangle.NO_BORDER });
                                patientInfoTable.AddCell(new PdfPCell(new Phrase(record.HOTEN, normalFont)) { Border = Rectangle.NO_BORDER });
                                patientInfoTable.AddCell(new PdfPCell(new Phrase("Ngày sinh:", subTitleFont)) { Border = Rectangle.NO_BORDER });
                                patientInfoTable.AddCell(new PdfPCell(new Phrase(record.NAMSINH, normalFont)) { Border = Rectangle.NO_BORDER });
                                patientInfoTable.AddCell(new PdfPCell(new Phrase("Mã BN:", subTitleFont)) { Border = Rectangle.NO_BORDER });
                                patientInfoTable.AddCell(new PdfPCell(new Phrase(record.MABN, normalFont)) { Border = Rectangle.NO_BORDER });
                                patientInfoTable.AddCell(new PdfPCell(new Phrase("Điện thoại:", subTitleFont)) { Border = Rectangle.NO_BORDER });
                                patientInfoTable.AddCell(new PdfPCell(new Phrase(record.DIDONG, normalFont)) { Border = Rectangle.NO_BORDER });

                                doc.Add(patientInfoTable);

                                // Thông tin mô tả và kết luận
                                Paragraph details = new Paragraph(
                                    $"Địa chỉ: {record.DIACHI}\n" +
                                    $"Bác sĩ chỉ định: {record.BSCHIDINH}\n" +
                                    $"Chẩn đoán: {record.CHANDOAN}\n\n\n" +
                                    $"Mô tả: {record.MOTA}\n" +
                                    $"Kết luận: {record.KETLUAN}\n",
                                    normalFont
                                )
                                {
                                    Leading = 12,
                                    SpacingBefore = 10,
                                    SpacingAfter = 10
                                };
                                doc.Add(details);

                                // Thông tin mô tả và kết luận
                                Paragraph ketluan = new Paragraph(
                                    $"Kết luận: {record.KETLUAN}\n",
                                    subTitleFont
                                )
                                {
                                    Leading = 12,
                                    SpacingBefore = 10,
                                    SpacingAfter = 10
                                };
                                doc.Add(ketluan);

                                // Chữ ký bác sĩ
                                Paragraph signature = new Paragraph(
                                    $"Ngày {record.NGAYKETQUA:dd} tháng {record.NGAYKETQUA:MM} năm {record.NGAYKETQUA:yyyy}\n{record.BSTHUCHIEN}",
                                    subTitleFont
                                )
                                {
                                    Alignment = Element.ALIGN_RIGHT,
                                    SpacingBefore = 10,
                                    SpacingAfter = 10
                                };
                                doc.Add(signature);
                            }
                        }
                    }
                }

                #endregion

                #region Các CLS CDHA chung khác

                // Xử lý các loại dữ liệu CDHA
                //if (FibroData.Any() || DiencoData.Any() || DiennaoData.Any() || GiaiphaubenhData.Any() || HohapData.Any() || LoangxuongData.Any() || NhuanhData.Any() || ShptData.Any() || C13Data.Any())
                //{
                //    // Danh sách các loại kỹ thuật cần xử lý
                //    var datasets = new Dictionary<string, List<PatientDataImage>>
                //    {
                //        { "Fibroscan", FibroData },                        
                //        { "Điện cơ", DiencoData },
                //        { "Điện não", DiennaoData },
                //        { "Giải phẫu bệnh", GiaiphaubenhData },
                //        { "Hô hấp ký", HohapData },
                //        { "Loãng xương", LoangxuongData },
                //        { "Sinh học phân tử", ShptData },
                //        { "C13", C13Data },
                //    };

                //    // Xử lý từng loại kỹ thuật
                //    foreach (var dataset in datasets)
                //    {
                //        var techniqueName = dataset.Key; // Tên loại kỹ thuật (Fibroscan, Điện tim, ...)
                //        var dataList = dataset.Value;    // Danh sách kết quả tương ứng

                //        if (dataList.Any())
                //        {
                //            foreach (var record in dataList)
                //            {
                //                doc.NewPage(); // Tạo trang mới cho từng bản ghi

                //                // Tiêu đề
                //                Paragraph title = new Paragraph(techniqueName.ToUpper(), titleFont)
                //                {
                //                    Alignment = Element.ALIGN_CENTER,
                //                    SpacingAfter = 10
                //                };
                //                doc.Add(title);

                //                // Hình ảnh
                //                if (record.ImageBase64List.Any())
                //                {
                //                    PdfPTable imageGrid = new PdfPTable(2)
                //                    {
                //                        WidthPercentage = 100,
                //                        SpacingBefore = 10
                //                    };

                //                    foreach (var base64Image in record.ImageBase64List)
                //                    {
                //                        if (!string.IsNullOrWhiteSpace(base64Image) && base64Image.StartsWith("data:image/png;base64,"))
                //                        {
                //                            try
                //                            {
                //                                string cleanBase64 = base64Image.Replace("data:image/png;base64,", "");
                //                                byte[] imageBytes = Convert.FromBase64String(cleanBase64);
                //                                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageBytes);
                //                                image.ScaleToFit(500f, 750f); // Kích thước hình ảnh
                //                                PdfPCell imgCell = new PdfPCell(image)
                //                                {
                //                                    Border = Rectangle.NO_BORDER,
                //                                    HorizontalAlignment = Element.ALIGN_CENTER,
                //                                    Padding = 5
                //                                };
                //                                imageGrid.AddCell(imgCell);
                //                            }
                //                            catch
                //                            {
                //                                PdfPCell errorCell = new PdfPCell(new Phrase("Hình ảnh không hợp lệ", normalFont))
                //                                {
                //                                    Border = Rectangle.NO_BORDER,
                //                                    HorizontalAlignment = Element.ALIGN_CENTER
                //                                };
                //                                imageGrid.AddCell(errorCell);
                //                            }
                //                        }
                //                    }

                //                    // Thêm ô trống nếu số lượng hình ảnh lẻ
                //                    if (record.ImageBase64List.Count % 2 != 0)
                //                    {
                //                        PdfPCell emptyCell = new PdfPCell() { Border = Rectangle.NO_BORDER };
                //                        imageGrid.AddCell(emptyCell);
                //                    }

                //                    doc.Add(imageGrid);
                //                }
                //            }
                //        }
                //    }
                //}
                #endregion

                #region CÁC CLS NEW
                // Xử lý các loại dữ liệu CDHA
                if (FibroData.Any() || DiencoData.Any() || DiennaoData.Any() || GiaiphaubenhData.Any() || HohapData.Any() || LoangxuongData.Any() || NhuanhData.Any() || ShptData.Any() || C13Data.Any())
                {
                    // Danh sách các loại kỹ thuật cần xử lý
                    var datasets = new Dictionary<string, List<PatientDataImage>>
                {
                    { "Fibroscan", FibroData },
                    { "Điện cơ", DiencoData },
                    { "Điện não", DiennaoData },
                    { "Giải phẫu bệnh", GiaiphaubenhData },
                    { "Hô hấp ký", HohapData },
                    { "Đo loãng xương", LoangxuongData },
                    { "Sinh học phân tử", ShptData },
                    { "C13", C13Data },
                };

                    // Xử lý từng loại kỹ thuật
                    foreach (var dataset in datasets)
                    {
                        var techniqueName = dataset.Key; // Tên loại kỹ thuật (Fibroscan, Điện tim, ...)
                        var dataList = dataset.Value;    // Danh sách kết quả tương ứng

                        if (dataList.Any())
                        {
                            foreach (var record in dataList)
                            {
                                // Nếu số lượng hình ảnh > 2, mỗi trang chỉ hiển thị 1 hình ảnh
                                if (record.ImageBase64List.Count > 1)
                                {
                                    foreach (var base64Image in record.ImageBase64List)
                                    {
                                        if (!string.IsNullOrWhiteSpace(base64Image) && base64Image.StartsWith("data:image/png;base64,"))
                                        {
                                            try
                                            {
                                                doc.NewPage(); // Tạo trang mới cho từng hình ảnh

                                                // Tiêu đề
                                                Paragraph noiSoiTitle = new Paragraph(record.TENKYTHUAT.ToUpper(), titleFont)
                                                {
                                                    Alignment = Element.ALIGN_CENTER,
                                                    SpacingAfter = 10
                                                };
                                                doc.Add(noiSoiTitle);

                                                // Hình ảnh
                                                string cleanBase64 = base64Image.Replace("data:image/png;base64,", "");
                                                byte[] imageBytes = Convert.FromBase64String(cleanBase64);
                                                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageBytes);
                                                image.ScaleToFit(500f, 750f); // Kích thước hình ảnh
                                                image.Alignment = Element.ALIGN_CENTER;
                                                doc.Add(image);
                                            }
                                            catch
                                            {
                                                doc.NewPage();
                                                Paragraph error = new Paragraph("Hình ảnh không hợp lệ", normalFont)
                                                {
                                                    Alignment = Element.ALIGN_CENTER
                                                };
                                                doc.Add(error);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    doc.NewPage(); // Tạo trang mới cho bản ghi

                                    // Tiêu đề
                                    Paragraph title = new Paragraph(techniqueName.ToUpper(), titleFont)
                                    {
                                        Alignment = Element.ALIGN_CENTER,
                                        SpacingAfter = 10
                                    };
                                    doc.Add(title);

                                    // Hình ảnh (dạng lưới nếu số lượng <= 2)
                                    PdfPTable imageGrid = new PdfPTable(2)
                                    {
                                        WidthPercentage = 100,
                                        SpacingBefore = 10
                                    };

                                    foreach (var base64Image in record.ImageBase64List)
                                    {
                                        if (!string.IsNullOrWhiteSpace(base64Image) && base64Image.StartsWith("data:image/png;base64,"))
                                        {
                                            try
                                            {
                                                string cleanBase64 = base64Image.Replace("data:image/png;base64,", "");
                                                byte[] imageBytes = Convert.FromBase64String(cleanBase64);
                                                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageBytes);
                                                image.ScaleToFit(500f, 750f); // Kích thước hình ảnh
                                                PdfPCell imgCell = new PdfPCell(image)
                                                {
                                                    Border = Rectangle.NO_BORDER,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    Padding = 5
                                                };
                                                imageGrid.AddCell(imgCell);
                                            }
                                            catch
                                            {
                                                PdfPCell errorCell = new PdfPCell(new Phrase("Hình ảnh không hợp lệ", normalFont))
                                                {
                                                    Border = Rectangle.NO_BORDER,
                                                    HorizontalAlignment = Element.ALIGN_CENTER
                                                };
                                                imageGrid.AddCell(errorCell);
                                            }
                                        }
                                    }

                                    // Thêm ô trống nếu số lượng hình ảnh lẻ
                                    if (record.ImageBase64List.Count % 2 != 0)
                                    {
                                        PdfPCell emptyCell = new PdfPCell() { Border = Rectangle.NO_BORDER };
                                        imageGrid.AddCell(emptyCell);
                                    }

                                    doc.Add(imageGrid);
                                }
                            }
                        }
                    }
                }

                #endregion




                #region Xét nghiệm chung tất cả CLS
                #endregion

                // Đóng tài liệu PDF nếu đã tạo
                if (doc != null && doc.IsOpen())
                {
                    doc.Close();
                }
                // Trả về file PDF
                Response.Headers.Add("Content-Disposition", "inline; filename="+firRecord.MABN+"_CDHA.pdf");
                byte[] fileBytes = ms.ToArray();
                return File(fileBytes, "application/pdf");
            }
        }


        //cắt ảnh nội soi
        private byte[] CropBlackBorders(byte[] imageBytes)
        {
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(ms))
                {
                    System.Drawing.Rectangle cropRect = new System.Drawing.Rectangle(0, 10, bitmap.Width, bitmap.Height - 10);

                    using (System.Drawing.Bitmap croppedBitmap = bitmap.Clone(cropRect, bitmap.PixelFormat))
                    {
                        using (MemoryStream croppedMs = new MemoryStream())
                        {
                            croppedBitmap.Save(croppedMs, ImageFormat.Png);
                            return croppedMs.ToArray();
                        }
                    }
                }
            }
        }

        #endregion


    }
}
