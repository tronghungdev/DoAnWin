using DAL.D.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace BLL.DoAn
{
    public class QLSanPamService
    {
        private CafeModel dbContext;

        public QLSanPamService()
        {
            dbContext = new CafeModel();
        }
        public List<SanPham> GetAllMonAn()
        {
            using (var context = new CafeModel()) // Giả định bạn đang sử dụng Entity Framework
            {
                // Lấy danh sách tất cả món ăn từ cơ sở dữ liệu
                return context.SanPhams.ToList();
            }
        }
        public bool UpdateSanPham(int maSanPham, string tenSanPham, decimal gia)
        {
            try
            {
                // Tìm sản phẩm theo mã sản phẩm
                var sanPham = dbContext.SanPhams.Find(maSanPham);

                // Kiểm tra xem sản phẩm có tồn tại không
                if (sanPham == null)
                {
                    return false; // Không tìm thấy sản phẩm
                }

                // Cập nhật thông tin sản phẩm
                sanPham.TenSanPham = tenSanPham;
                sanPham.Gia = gia;

                // Lưu các thay đổi vào cơ sở dữ liệu
                dbContext.SaveChanges();

                return true; // Cập nhật thành công
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ (nếu cần có thể ghi log)
                throw new Exception($"Lỗi khi cập nhật sản phẩm: {ex.Message}");
            }
        }

        // Kiểm tra tên món đã tồn tại
        public bool TenMonDaTonTai(string tenMon, int maSanPham)
        {
            return dbContext.SanPhams.Any(m => m.TenSanPham == tenMon && m.MaSanPham != maSanPham);
        }

        // Thêm món mới
        public void ThemMon(string tenMon, decimal gia, int maLoaiNuoc)
        {
            var monAn = new SanPham
            {
                TenSanPham = tenMon,
                Gia = gia,
                MaLoaiNuoc = maLoaiNuoc
            };

            dbContext.SanPhams.Add(monAn);
            dbContext.SaveChanges();
        }

        // Sửa món
        public void SuaMon(int maSanPham, string tenSanPham, decimal gia, int maLoaiNuoc)
        {
            var sanPham = dbContext.SanPhams.SingleOrDefault(sp => sp.MaSanPham == maSanPham);
            if (sanPham != null)
            {
                sanPham.TenSanPham = tenSanPham;
                sanPham.Gia = gia;
                sanPham.MaLoaiNuoc = maLoaiNuoc;
                dbContext.SaveChanges();
            }
            else
            {
                throw new Exception("Mã sản phẩm không tồn tại.");
            }
        }

        // Xóa món
        public void XoaMon(int maSanPham)
        {
            var monAn = dbContext.SanPhams.Find(maSanPham);
            if (monAn != null)
            {
                dbContext.SanPhams.Remove(monAn);
                dbContext.SaveChanges();
            }
            else
            {
                throw new Exception("Món ăn không tồn tại.");
            }
        }


        // Lấy món ăn theo mã
        public SanPham LayMonAnTheoId(int maSanPham)
        {
            return dbContext.SanPhams.FirstOrDefault(sp => sp.MaSanPham == maSanPham);
        }

        // Lấy tất cả món ăn
        public List<SanPham> LayMonAn()
        {
            return dbContext.SanPhams.Include(m => m.LoaiNuoc).ToList();
        }

        // Lấy món theo loại nước
        public List<SanPham> LayMonAnTheoLoai(int maLoaiNuoc)
        {
            return dbContext.SanPhams.Where(s => s.MaLoaiNuoc == maLoaiNuoc).ToList();
        }
    }
}
