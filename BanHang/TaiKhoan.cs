using BLL.DoAn;
using DAL.D.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BanHang
{
    public partial class TaiKhoan : Form
    {
        private QLTaiKhoanService QLTaiKhoanService;
        public TaiKhoan()
        {
            InitializeComponent();
            QLTaiKhoanService = new QLTaiKhoanService(new CafeModel());
            LoadDataGridView();
        }

        private void LoadDataGridView()
        {
            // Lấy danh sách khách hàng và gán vào DataGridView
            var taiKhoans = QLTaiKhoanService.LayTatCaTaiKhoan();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.DataSource = taiKhoans.Select(tk => new
            {
                tk.MaTaiKhoan,
                tk.TenDangNhap,
                tk.MatKhau
            }).ToList();
        }
        private void TaiKhoan_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0) // Kiểm tra chỉ số hàng
            {
                var selectedRow = dataGridView1.Rows[e.RowIndex];
                lblTaiKhoan.Text = selectedRow.Cells["TenDangNhap"].Value.ToString();
                txtMatKhau.Text = selectedRow.Cells["MatKhau"].Value.ToString();
            }

        }

        public void lblDoiMatKhau_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(lblTaiKhoan.Text))
            {
                // Lấy mã tài khoản từ DataGridView
                int maTaiKhoan = Convert.ToInt32(dataGridView1.CurrentRow.Cells["MaTaiKhoan"].Value);

                // Tạo đối tượng TaiKhoan và cập nhật mật khẩu
                var taiKhoan = new DAL.D.Model.TaiKhoan // Đảm bảo sử dụng đúng namespace của lớp TaiKhoan
                {
                    MaTaiKhoan = maTaiKhoan, // Gán mã tài khoản
                    MatKhau = txtMatKhau.Text // Lấy mật khẩu mới từ TextBox
                };

                // Gọi phương thức để sửa mật khẩu
                bool result = QLTaiKhoanService.SuaMatKhau(taiKhoan);
                if (result)
                {
                    MessageBox.Show("Mật khẩu đã được cập nhật thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataGridView(); // Tải lại dữ liệu trong DataGridView
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi cập nhật mật khẩu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn tài khoản để đổi mật khẩu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
