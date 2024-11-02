using DAL.D.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace BLL.DoAn
{
    public class QLLoaiSP
    {
        private CafeModel dbContext;

        public QLLoaiSP()
        {
            dbContext = new CafeModel();
        }

        // Lấy danh sách loại nước
        public List<LoaiNuoc> LayDanhSachLoai()
        {
            return dbContext.LoaiNuocs.ToList();
        }

        // Thêm loại nước mới
        public void ThemLoai(int maLoaiNuoc, string tenLoai)
        {
            if (dbContext.LoaiNuocs.Any(l => l.MaLoaiNuoc == maLoaiNuoc))
            {
                throw new Exception("Mã loại nước đã tồn tại.");
            }
            if (dbContext.LoaiNuocs.Any(l => l.TenLoaiNuoc == tenLoai))
            {
                throw new Exception("Tên loại nước đã tồn tại");
            }

            var loai = new LoaiNuoc
            {
                MaLoaiNuoc = maLoaiNuoc,
                TenLoaiNuoc = tenLoai
            };

            dbContext.LoaiNuocs.Add(loai);
            dbContext.SaveChanges();
        }

        // Sửa loại nước
        public void SuaLoai(int maLoaiNuoc, string tenLoai)
        {
            var loaiNuoc = dbContext.LoaiNuocs.FirstOrDefault(l => l.MaLoaiNuoc == maLoaiNuoc);
            if (loaiNuoc != null)
            {
                loaiNuoc.TenLoaiNuoc = tenLoai;
                dbContext.SaveChanges();
            }
            else
            {
                throw new Exception("Loại nước không tồn tại.");
            }
        }

        // Xóa loại nước và các sản phẩm liên quan
        public void XoaLoai(int maLoaiNuoc)
        {
            var loaiNuoc = dbContext.LoaiNuocs.Include(l => l.SanPhams)
                                              .FirstOrDefault(l => l.MaLoaiNuoc == maLoaiNuoc);
            if (loaiNuoc != null)
            {
                foreach (var sanPham in loaiNuoc.SanPhams.ToList())
                {
                    dbContext.SanPhams.Remove(sanPham);
                }

                dbContext.LoaiNuocs.Remove(loaiNuoc);
                dbContext.SaveChanges();
            }
            else
            {
                throw new Exception("Loại nước không tồn tại.");
            }
        }
    }
}
