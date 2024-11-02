using DAL.D.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace BLL.DoAn
{
    public class QLOderService
    {
        private CafeModel dbContext;
        private List<SelectedSanPham> selectedSanPhams;

        public KhachHang KhachHangTamThoi { get; set; }

        public QLOderService()
        {
            dbContext = new CafeModel();  // Khởi tạo repository
            selectedSanPhams = new List<SelectedSanPham>();
        }
        public List<LoaiNuoc> GetAllLoaiNuoc()
        {
            using (var context = new CafeModel())
            {
                return context.LoaiNuocs.ToList(); // Đảm bảo bạn đã có dữ liệu trong bảng LoaiNuoc
            }
        }


       

        // Lấy danh sách sản phẩm theo loại từ SQL
        public List<SanPham> GetSanPhamByLoai(int loaiId)
        {
            return dbContext.SanPhams.Where(sp => sp.MaLoaiNuoc == loaiId).ToList();

        }

        public bool RemoveSanPham(SelectedSanPham sanPham)
        {
            if (sanPham != null)
            {
                selectedSanPhams.Remove(sanPham); // Xóa sản phẩm khỏi danh sách
                return true; // Trả về true nếu xóa thành công
            }
            return false; // Trả về false nếu sanPham null
        }
        public List<SelectedSanPham> GetSelectedSanPhams()
        {
            return selectedSanPhams;
        }

        public (int tongSoLuong, decimal tongTien) TinhTong(List<SelectedSanPham> selectedSanPhams)
        {
            int tongSoLuong = selectedSanPhams.Sum(sp => sp.SoLuong);
            decimal tongTien = selectedSanPhams.Sum(sp => sp.Gia * sp.SoLuong);
            return (tongSoLuong, tongTien);
        }


        public bool UpdateSanPhamQuantity(int maSanPham, int soLuongMoi)
        {
            var sanPham = selectedSanPhams.FirstOrDefault(sp => sp.MaSanPham == maSanPham);
            if (sanPham != null)
            {
                sanPham.SoLuong = soLuongMoi; // Cập nhật số lượng mới
                return true; // Trả về true nếu cập nhật thành công
            }
            return false; // Trả về false nếu không tìm thấy sản phẩm
        }

        public void AddSelectedSanPham(SelectedSanPham sanPham)
        {
            if (sanPham != null)
            {
                selectedSanPhams.Add(sanPham);
            }
        }

        // Phương thức tính tổng tiền sau khi giảm giá
        public decimal TinhTongSauGiamGia(decimal giamGia)
        {
            var (tongSoLuong, tongTien) = TinhTong(selectedSanPhams);
            decimal tongSauGiamGia = tongTien - (tongTien * (giamGia / 100));
            return tongSauGiamGia;
        }

        public List<SelectedSanPham> GetSelectedSanPham()
        {
            return selectedSanPhams;
        }
        public SanPham GetSanPhamById(int maSanPham)
        {
            return dbContext.SanPhams.FirstOrDefault(sp => sp.MaSanPham == maSanPham);
        }
        public string LayTenSanPham(int maSanPham)
        {
            var sanPham = GetSanPhamById(maSanPham);
            return sanPham?.TenSanPham ?? "Không tìm thấy sản phẩm";
        }
    }



}

