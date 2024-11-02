using DAL.D.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DoAn
{
    public class QlDangNhap
    {
        public class qlDangNhapService
        {
            private CafeModel _context; // Giả định bạn đang sử dụng Entity Framework

            public qlDangNhapService()
            {
                _context = new CafeModel();
            }

            // Phương thức kiểm tra đăng nhập
            public bool KiemTraDangNhap(string username, string password, out bool isAdmin)
            {
                // Kiểm tra nếu người dùng tồn tại trong cơ sở dữ liệu
                var user = _context.TaiKhoans
                    .FirstOrDefault(u => u.TenDangNhap == username && u.MatKhau == password);

                // Nếu không tìm thấy người dùng, trả về false và false cho isAdmin
                if (user == null)
                {
                    isAdmin = false;
                    return false;
                }

                // Kiểm tra nếu tài khoản là admin
                isAdmin = (user.TenDangNhap.Equals("admin", StringComparison.OrdinalIgnoreCase));

                return true; // Trả về true nếu đăng nhập thành công
            }
            public bool KiemTraDangNhap2(string username, string password, out bool isSeller)
            {
                // Kiểm tra nếu người dùng tồn tại trong cơ sở dữ liệu
                var user = _context.TaiKhoans
                    .FirstOrDefault(u => u.TenDangNhap == username && u.MatKhau == password);

                // Nếu không tìm thấy người dùng, trả về false và false cho isSeller
                if (user == null)
                {
                    isSeller = false;
                    return false;
                }

                // Kiểm tra nếu tài khoản là seller
                isSeller = (user.TenDangNhap.Equals("seller", StringComparison.OrdinalIgnoreCase));

                return true; // Trả về true nếu đăng nhập thành công
            }
        }
    }
}
