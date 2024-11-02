using DAL.D.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;


namespace BLL.DoAn
{
    public class QLDonHang
    {
        private readonly CafeModel dbContext;

        public QLDonHang()
        {
            dbContext = new CafeModel(); // Đảm bảo rằng CafeModel đã được cấu hình đúng
        }



        // Phương thức lưu tất cả khách hàng tạm thời vào cơ sở dữ liệu

        public List<DonHang> danhSachDonHang;
        public List<DonHang> LayDanhSachDonHang()
        {
            using (var context = new CafeModel())
            {
                danhSachDonHang = context.DonHangs.ToList(); // Gán danh sách đơn hàng vào biến
            }
            return danhSachDonHang; // Trả về danh sách
        }
        public bool LuuDonHang(DonHang donHang, List<ChiTietDonHang> chiTietDonHangList)
        {
            try
            {
                using (var db = new CafeModel())
                {
                    // Thêm đơn hàng vào cơ sở dữ liệu
                    db.DonHangs.Add(donHang);
                    db.SaveChanges(); // Lưu để lấy MaDonHang tự động tạo

                    // Thêm chi tiết đơn hàng vào cơ sở dữ liệu
                    foreach (var chiTiet in chiTietDonHangList)
                    {
                        chiTiet.MaDonHang = donHang.MaDonHang; // Gán MaDonHang cho mỗi chi tiết
                        db.ChiTietDonHangs.Add(chiTiet);
                    }

                    // Lưu tất cả các thay đổi
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
                return false;
            }
        }
        public bool ThemDonHang(DonHang donHang, List<ChiTietDonHang> chiTietDonHangs)
        {
            // Kiểm tra tham số đầu vào
            if (donHang == null || chiTietDonHangs == null || chiTietDonHangs.Count == 0)
            {
                return false; // Tham số không hợp lệ
            }

            try
            {
                using (var context = new CafeModel())
                {
                    // Thêm đơn hàng
                    context.DonHangs.Add(donHang);
                    context.SaveChanges(); // Lưu đơn hàng trước để có MaDonHang

                    // Thêm chi tiết đơn hàng
                    foreach (var chiTiet in chiTietDonHangs)
                    {
                        chiTiet.MaDonHang = donHang.MaDonHang; // Gán MaDonHang cho từng chi tiết
                        context.ChiTietDonHangs.Add(chiTiet);
                    }
                    context.SaveChanges(); // Lưu chi tiết đơn hàng
                }
                return true; // Thêm thành công
            }
            catch
            {
                return false; // Thêm không thành công
            }
        }

        public List<DonHang> LayDanhSachDonHangTheoNgay(DateTime ngay)
        {
            if (ngay.Date > DateTime.Today)
            {
                throw new ArgumentException("Ngày không thể là ngày trong tương lai.");
            }

            var batDau = ngay.Date; // Lấy thời gian đầu ngày
            var ketThuc = batDau.AddDays(1); // Thời gian đầu ngày tiếp theo

            using (var context = new CafeModel())
            {
                return context.DonHangs
                    .Where(dh => dh.NgayTao >= batDau && dh.NgayTao < ketThuc)
                    .ToList();
            }
        }




        // Tính tổng số lượng hóa đơn
        public decimal TinhTongDoanhThu(DateTime ngay)
        {
            var danhSachDonHangTrongNgay = LayDanhSachDonHangTheoNgay(ngay);
            return danhSachDonHangTrongNgay.Sum(dh => (decimal?)dh.TongTien) ?? 0;
        }

        public decimal TinhDoanhThuChuyenKhoan(DateTime ngay)
        {
            var danhSachDonHangChuyenKhoan = LayDanhSachDonHangTheoNgay(ngay)
                .Where(dh => dh.HinhThucThanhToan == "Chuyển khoản");

            return danhSachDonHangChuyenKhoan.Sum(dh => dh.TongTien);
        }

        public decimal TinhDoanhThuMomo(DateTime ngay)
        {
            var danhSachDonHangMomo = LayDanhSachDonHangTheoNgay(ngay)
                .Where(dh => dh.HinhThucThanhToan == "Momo");

            return danhSachDonHangMomo.Sum(dh => dh.TongTien);
        }

        public decimal TinhDoanhThuTienMat(DateTime ngay)
        {
            var danhSachDonHangTienMat = LayDanhSachDonHangTheoNgay(ngay)
                .Where(dh => dh.HinhThucThanhToan == "Tiền mặt");

            return danhSachDonHangTienMat.Sum(dh => dh.TongTien);
        }

        // Tính tổng số lượng hóa đơn
        public int TinhTongHoaDon(DateTime ngay)
        {
            var danhSachDonHangTrongNgay = LayDanhSachDonHangTheoNgay(ngay);
            return danhSachDonHangTrongNgay.Count;
        }


        public bool LuuDonHang(DonHang donHang, KhachHang khachHang, List<ChiTietDonHang> chiTietDonHangs)
        {
            using (var context = new CafeModel())
            {
                try
                {
                    // Thêm đơn hàng
                    context.DonHangs.Add(donHang);
                    context.SaveChanges(); // Lưu đơn hàng trước để có mã đơn hàng

                    if (khachHang != null && !string.IsNullOrWhiteSpace(khachHang.SoDienThoai))
                    {
                        // Kiểm tra xem khách hàng đã tồn tại chưa
                        var existingKhachHang = context.KhachHangs
                            .FirstOrDefault(kh => kh.SoDienThoai == khachHang.SoDienThoai);

                        if (existingKhachHang == null)
                        {
                            context.KhachHangs.Add(khachHang);
                            context.SaveChanges(); // Lưu khách hàng
                            donHang.MaKhachHang = khachHang.MaKhachHang; // Liên kết mã khách hàng vào đơn hàng
                        }
                        else
                        {
                            donHang.MaKhachHang = existingKhachHang.MaKhachHang; // Sử dụng mã khách hàng đã tồn tại
                        }
                    }

                    // Thêm chi tiết đơn hàng
                    foreach (var chiTiet in chiTietDonHangs)
                    {
                        chiTiet.MaDonHang = donHang.MaDonHang; // Gán mã đơn hàng cho chi tiết
                        context.ChiTietDonHangs.Add(chiTiet);
                    }

                    context.SaveChanges(); // Lưu tất cả chi tiết đơn hàng
                    return true;
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu cần
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

    }
}
