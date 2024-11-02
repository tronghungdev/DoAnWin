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
    public class KhachHangService
    {

        private readonly CafeModel _context;

        public KhachHangService(CafeModel context)
        {
            _context = context;
        }

        public List<KhachHang> LayTatCaKhachHang()
        {
            return _context.KhachHangs.ToList();
        }

        public List<KhachHang> TimKiemKhachHang(string tuKhoa)
        {
            using (var context = new CafeModel())
            {
                return context.KhachHangs
                    .Where(kh => kh.TenKhachHang.Contains(tuKhoa) || kh.SoDienThoai.Contains(tuKhoa)) // Giả sử HoTen và SDT là tên thuộc tính
                    .ToList();
            }
        }



        // Sửa thông tin khách hàng
        public bool SuaKhachHang(KhachHang khachHang)
        {
            try
            {
                var existingKhachHang = _context.KhachHangs.Find(khachHang.MaKhachHang);
                if (existingKhachHang != null)
                {
                    existingKhachHang.TenKhachHang = khachHang.TenKhachHang;
                    existingKhachHang.SoDienThoai = khachHang.SoDienThoai;
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // Xóa khách hàng
        public void XoaKhachHang(int maKhachHang)
        {
            using (var context = new CafeModel())
            {
                var khachHang = context.KhachHangs.Find(maKhachHang);
                if (khachHang == null)
                {
                    throw new Exception("Khách hàng không tồn tại.");
                }

                // Kiểm tra xem khách hàng có đơn hàng nào liên kết không
                var donHangs = context.DonHangs.Where(dh => dh.MaKhachHang == maKhachHang).ToList();
                if (donHangs.Any())
                {
                    // Nếu có đơn hàng liên quan, ném ra ngoại lệ với thông tin chi tiết
                    string danhSachDonHang = string.Join(", ", donHangs.Select(dh => dh.MaDonHang)); // Giả sử MaDonHang là mã đơn hàng
                    throw new Exception($"Không thể xóa khách hàng vì họ có đơn hàng liên quan: {danhSachDonHang}.");
                }

                context.KhachHangs.Remove(khachHang);

                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    throw new Exception("Có lỗi xảy ra khi xóa khách hàng. " + (ex.InnerException?.Message ?? ex.Message));
                }
            }
        }


        public KhachHang LayKhachHangTheoMa(int maKhachHang)
        {
            if (maKhachHang <= 0)
            {
                Console.WriteLine("Mã khách hàng không hợp lệ.");
                return null; // Trả về null nếu mã không hợp lệ
            }

            // Tìm kiếm khách hàng theo mã
            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.MaKhachHang == maKhachHang);

            if (khachHang == null)
            {
                Console.WriteLine("Không tìm thấy khách hàng với mã: " + maKhachHang);
            }

            return khachHang;
        }

        public KhachHang LayKhachHangTheoSDT(string soDienThoai)
        {
            using (var context = new CafeModel())
            {
                return context.KhachHangs.FirstOrDefault(kh => kh.SoDienThoai == soDienThoai);
            }
        }
        public void ThemKhachHang(KhachHang khachHang)
        {
            using (var context = new CafeModel())
            {
                context.KhachHangs.Add(khachHang);
                context.SaveChanges();
            }
        }

    }
}
