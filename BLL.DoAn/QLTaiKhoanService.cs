using DAL.D.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DoAn
{
    public class QLTaiKhoanService
    {
        private readonly CafeModel _context;

        public QLTaiKhoanService(CafeModel context)
        {
            _context = context;
        }

        public List<TaiKhoan> LayTatCaTaiKhoan()
        {
            return _context.TaiKhoans.ToList();
        }
        public bool SuaMatKhau(TaiKhoan taikhoan)
        {
            try
            {
                var existingTaiKhoan = _context.TaiKhoans.Find(taikhoan.MaTaiKhoan);
                if (existingTaiKhoan != null)
                {
                    existingTaiKhoan.MatKhau = taikhoan.MatKhau;
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
    }
}
