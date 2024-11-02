using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL.DoAn;
using static BLL.DoAn.QlDangNhap;

namespace BanHang
{
    public partial class QuanLy : Form
    {

        public QuanLy()
        {
            InitializeComponent();
            
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string username = txtTenDangNhap.Text.Trim();
            string password = txtMatKhau.Text.Trim();
            bool isAdmin;

            var loginService = new QlDangNhap.qlDangNhapService();

            // Kiểm tra đăng nhập
            if (loginService.KiemTraDangNhap(username, password, out isAdmin))
            {
                // Nếu là seller, mở form quản lý bán hàng
                if (isAdmin)
                {
                    QuanLyChuyenSau formQuanLy = new QuanLyChuyenSau();
                    formQuanLy.Show();
                    this.Hide(); // Ẩn form đăng nhập
                }
                else
                {
                    MessageBox.Show("Bạn không có quyền truy cập vào.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txtTenDangNhap_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Kiểm tra xem phím nhấn có phải là Enter không
            {
                btnDangNhap.PerformClick(); // Kích hoạt sự kiện nhấp vào nút Đăng Nhập
            }
        }

        private void txtMatKhau_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Kiểm tra xem phím nhấn có phải là Enter không
            {
                btnDangNhap.PerformClick(); // Kích hoạt sự kiện nhấp vào nút Đăng Nhập
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            txtMatKhau.UseSystemPasswordChar = !txtMatKhau.UseSystemPasswordChar;
        }
    }
}
