using DAL.D.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;


namespace BLL.DoAn
{
    public class ThongKeService
    {
        private CafeModel dbContext;

        public ThongKeService()
        {
            dbContext = new CafeModel();
        }

        // Lấy danh sách đơn hàng
        public List<DonHang> LayDanhSachDonHang()
        {
            return dbContext.DonHangs.Include(d => d.ChiTietDonHangs)
                                     .Include(d => d.KhachHang)
                                     .Include(d => d.TaiKhoan).ToList();
        }

        // Lấy chi tiết đơn hàng theo mã
        public List<ChiTietDonHang> LayChiTietDonHang(int maDonHang)
        {
            return dbContext.ChiTietDonHangs.Where(ct => ct.MaDonHang == maDonHang).ToList();
        }

        // Xóa đơn hàng và các chi tiết liên quan
        public void XoaDonHang(string maDonHang)
        {
            var donHang = dbContext.DonHangs.Include(dh => dh.ChiTietDonHangs)
                                            .FirstOrDefault(dh => dh.MaDonHang.ToString() == maDonHang);

            if (donHang != null)
            {
                foreach (var chiTiet in donHang.ChiTietDonHangs.ToList())
                {
                    dbContext.ChiTietDonHangs.Remove(chiTiet);
                }

                dbContext.DonHangs.Remove(donHang);
                dbContext.SaveChanges();
            }
            else
            {
                throw new Exception("Đơn hàng không tồn tại.");
            }
        }

        // Lấy tổng số đơn hàng
        public int LayTongDonHangTheoNgay(DateTime ngay)
        {
            try
            {
                return dbContext.DonHangs.Count(dh => dh.NgayTao.Date == ngay.Date);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy tổng đơn hàng: {ex.Message}");
                return 0; // Hoặc xử lý khác nếu cần
            }
        }

        // Lấy tổng doanh thu theo ngày
        public decimal LayTongDoanhThuTheoNgay(DateTime ngay)
        {
            try
            {
                return dbContext.DonHangs
                    .Where(dh => dh.NgayTao.Date == ngay.Date)
                    .Sum(dh => (decimal?)dh.TongTien) ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tính tổng doanh thu: {ex.Message}");
                return 0; // Hoặc xử lý khác nếu cần
            }
        }
    }
}
