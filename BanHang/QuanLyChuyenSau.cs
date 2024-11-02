using BLL.DoAn;
using DAL.D.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BanHang
{
    public partial class QuanLyChuyenSau : Form
    {
        private QLSanPamService qlCafeService;
        private QLLoaiSP QLLoaiSP;
        private int maLoaiNuoc;
        public QuanLyChuyenSau()
        {
            InitializeComponent();
            qlCafeService = new QLSanPamService();
            QLLoaiSP = new QLLoaiSP();



        }

        private void QuanLyChuyenSau_Load(object sender, EventArgs e)
        {
            LoadLoaiMon();

        }
        private void LoadLoaiMon()
        {
            var loaiMonList = QLLoaiSP.LayDanhSachLoai();
            cmbLoai.DataSource = null; // Đặt lại DataSource về null để làm mới
            cmbLoai.DataSource = loaiMonList; // Gán lại danh sách mới
            cmbLoai.DisplayMember = "TenLoaiNuoc"; // Tên hiển thị
            cmbLoai.ValueMember = "MaLoaiNuoc";

            // Tải danh sách món ăn cho loại nước đầu tiên
            if (loaiMonList.Count > 0)
            {
                maLoaiNuoc = (int)loaiMonList[0].MaLoaiNuoc; // Lưu mã loại nước đầu tiên
                LoadMonAn(maLoaiNuoc); // Tải món ăn cho loại nước này
                txtTenMon.Clear();
                txtGiaMon.Clear();
                lblMaSP.Enabled = false;

            }
        }

        private void cmbLoai_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLoai.SelectedValue is int selectedValue)
            {
                maLoaiNuoc = selectedValue; // Lấy mã loại nước đã chọn
                LoadMonAn(maLoaiNuoc); // Tải món ăn theo loại nước đã chọn


                // Xóa thông tin trong các TextBox khi chọn loại món mới
                txtTenMon.Clear();
                txtGiaMon.Clear();
                lblMaSP.Enabled = false;
            }
        }

        private void LoadMonAn(int maLoai)
        {
            var monAnList = qlCafeService.LayMonAnTheoLoai(maLoai);
            if (monAnList != null)
            {
                dataGridView1.DataSource = null;  // Đặt lại DataSource về null để làm mới
                dataGridView1.DataSource = monAnList;  // Gán lại danh sách mới
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Đảm bảo các cột hiển thị đúng
                dataGridView1.Columns["ChiTietDonHangs"].Visible = false; // Nếu bạn có cột này trong mô hình gốc
                dataGridView1.Columns["LoaiNuoc"].Visible = false;
            }
        }

        private void UpdateLoaiMon()
        {
            LoadLoaiMon(); // Gọi lại hàm LoadLoaiMon để cập nhật dữ liệu

            // Kiểm tra xem giá trị đã chọn còn tồn tại trong danh sách không
            if (cmbLoai.Items.Count > 0) // Đảm bảo có ít nhất một mục
            {
                // Nếu danh sách không rỗng, kiểm tra xem giá trị đã chọn còn hợp lệ không
                if (cmbLoai.SelectedValue != null)
                {
                    cmbLoai.SelectedValue = cmbLoai.SelectedValue; // Đặt lại giá trị đã chọn
                }
                else
                {
                    cmbLoai.SelectedIndex = 0; // Nếu không có giá trị đã chọn, chọn mục đầu tiên
                }
            }
            ResetForm();
        }
        private void UpdateMonAn()
        {
            try
            {
                // Kiểm tra mã sản phẩm không được để trống
                if (string.IsNullOrEmpty(lblMaSP.Text))
                {
                    MessageBox.Show("Vui lòng chọn món ăn cần sửa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy mã sản phẩm từ TextBox
                if (!int.TryParse(lblMaSP.Text, out int maSP))
                {
                    MessageBox.Show("Mã sản phẩm không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra tên món có hợp lệ không
                if (string.IsNullOrWhiteSpace(txtTenMon.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên món.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra giá có hợp lệ không
                if (!decimal.TryParse(txtGiaMon.Text, out decimal gia) || gia <= 0)
                {
                    MessageBox.Show("Giá không hợp lệ. Vui lòng nhập giá hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy mã loại nước từ ComboBox
                if (cmbLoai.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn loại nước.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int maLoaiNuoc = (int)cmbLoai.SelectedValue;

                // Gọi hàm cập nhật sản phẩm từ service
                bool isUpdated = qlCafeService.UpdateSanPham(maSP, txtTenMon.Text.Trim(), gia);

                // Nếu cập nhật thành công
                if (isUpdated)
                {
                    MessageBox.Show("Cập nhật món thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMonAn(maLoaiNuoc); // Tải lại danh sách món ăn
                }
                else
                {
                    MessageBox.Show("Không tìm thấy món ăn cần cập nhật.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

                LoadMonAn(maLoaiNuoc);
            }
        }



        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra ô txtTenMon có để trống không
                if (string.IsNullOrWhiteSpace(txtTenMon.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên sản phẩm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy tên món từ ô nhập
                string tenMon = txtTenMon.Text.Trim();

                // Kiểm tra tên món đã tồn tại
                if (qlCafeService.TenMonDaTonTai(tenMon, 0)) // 0 vì đang thêm món mới
                {
                    MessageBox.Show("Tên món ăn đã tồn tại. Vui lòng nhập tên khác!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra giá
                if (!decimal.TryParse(txtGiaMon.Text, out decimal gia) || gia <= 0)
                {
                    MessageBox.Show("Giá sản phẩm không hợp lệ. Vui lòng nhập giá hợp lệ!", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra loại nước đã chọn
                if (cmbLoai.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn loại nước!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int maLoaiNuoc = (int)cmbLoai.SelectedValue;

                // Gọi phương thức thêm món ăn trong dịch vụ
                qlCafeService.ThemMon(tenMon, gia, maLoaiNuoc); // Gọi phương thức thêm món

                // Hiển thị thông báo thành công
                MessageBox.Show("Thêm món thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Tải lại danh sách món ăn
                LoadMonAn(maLoaiNuoc); // Tải lại danh sách món ăn theo mã loại nước hiện tại

                // Làm sạch ô nhập liệu
                txtTenMon.Clear();
                txtGiaMon.Clear();
                lblMaSP.Enabled = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hàm hỗ trợ hiển thị thông báo cảnh báo


        private void thêmLoạiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Ẩn form hiện tại

            ThemLoaiMon form4 = new ThemLoaiMon();

            form4.Show();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateLoaiMon();
            LoadLoaiMon();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Kiểm tra nếu hàng được chọn là hợp lệ
            {
                var row = dataGridView1.Rows[e.RowIndex]; // Lấy hàng đã chọn

                // Hiển thị thông tin món ăn vào các TextBox
                txtTenMon.Text = row.Cells["TenSanPham"].Value.ToString(); // Tên món ăn
                txtGiaMon.Text = row.Cells["Gia"].Value.ToString(); // Giá món ăn

                // Lấy mã sản phẩm và gán vào txtMaSP
                lblMaSP.Text = row.Cells["MaSanPham"].Value.ToString(); // Gán mã sản phẩm
            }
        }

        private void btnSuaMon_Click(object sender, EventArgs e)
        {
            UpdateMonAn();
        }
        private void LoadMonAn()
        {
            try
            {
                // Lấy danh sách món ăn từ dịch vụ
                var danhSachMonAn = qlCafeService.GetAllMonAn();

                // Xóa dữ liệu cũ trong DataGridView/ListView
                dataGridView1.Rows.Clear(); // Thay đổi thành danh sách của bạn

                // Thêm dữ liệu mới vào DataGridView/ListView
                foreach (var monAn in danhSachMonAn)
                {
                    dataGridView1.Rows.Add(monAn.MaLoaiNuoc, monAn.TenSanPham, monAn.Gia); // Thay đổi các trường tương ứng
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tải danh sách món ăn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Lấy hàng đầu tiên được chọn
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Lấy mã sản phẩm từ hàng được chọn
                var maSanPham = selectedRow.Cells["MaSanPham"].Value; // Thay "MaSanPham" bằng tên cột tương ứng

                // Xác nhận xóa
                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa món ăn này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmResult == DialogResult.Yes)
                {
                    using (var dbContext = new CafeModel())
                    {
                        // Tìm sản phẩm theo mã sản phẩm
                        var productToDelete = dbContext.SanPhams.Find(maSanPham);
                        if (productToDelete != null)
                        {
                            dbContext.SanPhams.Remove(productToDelete); // Xóa sản phẩm
                            try
                            {
                                dbContext.SaveChanges(); // Lưu thay đổi
                                // Code cập nhật
                            }
                            catch (DbUpdateException ex)
                            {
                                // Hiển thị thông tin về InnerException
                                MessageBox.Show($"Đã xảy ra lỗi: {ex.InnerException?.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            MessageBox.Show("Xóa món thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Món ăn không tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    // Tải lại danh sách món ăn
                    LoadMonAn(maLoaiNuoc); // Tải lại danh sách món ăn theo mã loại nước hiện tại

                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một hàng để xóa.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ResetForm()
        {
            // Xóa sạch tất cả các ô nhập liệu

            txtTenMon.Clear();
            txtGiaMon.Clear();
            lblMaSP.Enabled = false;

            // Đặt lại ComboBox
            if (cmbLoai.Items.Count > 0)
            {
                cmbLoai.SelectedIndex = 0; // Chọn mục đầu tiên
            }
            dataGridView1.DataSource = null; // Hoặc gọi lại hàm load dữ liệu
        }

        private void xóaLoạiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThongKeBanHang frm = new ThongKeBanHang();
            frm.Show();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn thoát không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Kiểm tra lựa chọn của người dùng
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit(); // Thoát ứng dụng nếu người dùng chọn 'Có'
            }
        }

        private void thôngTinKháchHàngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThongTinKhachHang thongTinKhachHang = new ThongTinKhachHang();
            thongTinKhachHang.ShowDialog();
        }

        private void quảnLýTàiKhoảnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaiKhoan taiKhoan = new TaiKhoan();
            taiKhoan.ShowDialog();
        }
    }
}