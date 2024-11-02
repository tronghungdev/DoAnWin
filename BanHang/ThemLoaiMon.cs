using BLL.DoAn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BanHang
{
    public partial class ThemLoaiMon : Form
    {
        private QLLoaiSP QLLoaiSP;
        private QLSanPamService qlCafeService;
        private int selectedMaLoaiNuoc;

        public ThemLoaiMon()
        {
            InitializeComponent();
            qlCafeService = new QLSanPamService();
            QLLoaiSP = new QLLoaiSP();

        }

        public event Action TypeChanged; // Sự kiện khi loại món được thay đổi

        // Khi thêm, sửa loại món thành công
        private void OnTypeChanged()
        {
            TypeChanged?.Invoke(); // Kích hoạt sự kiện
        }

        // Gọi OnTypeChanged() sau khi thêm hoặc sửa loại món
        private void btnAddOrEdit_Click(object sender, EventArgs e)
        {
            // Code thêm hoặc sửa loại món
            // Nếu thành công, gọi sự kiện
            OnTypeChanged();
        }

        private void LoadLoaiMon()
        {
            try
            {
                // Lấy danh sách loại món từ QLLoaiSP
                var loaiMonList = QLLoaiSP.LayDanhSachLoai().ToList(); // Thay QLLoaiSP.la() bằng phương thức tương ứng từ dịch vụ

                // Kiểm tra xem danh sách có dữ liệu không
                if (loaiMonList != null && loaiMonList.Any())
                {
                    // Cập nhật lại DataGridView
                    dataGridView1.DataSource = loaiMonList;
                    dataGridView1.Refresh(); // Làm mới DataGridView
                }
                else
                {
                    // Nếu danh sách rỗng, thông báo cho người dùng
                    dataGridView1.DataSource = null; // Đặt lại DataSource
                    MessageBox.Show("Không có loại món nào để hiển thị!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Cấu hình DataGridView
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Lấp đầy bảng
                dataGridView1.Columns["SanPhams"].Visible = false; // Ẩn cột "SanPhams" nếu không cần thiết
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có lỗi xảy ra
                MessageBox.Show($"Lỗi khi tải danh sách loại món: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }



        private void btnThem_Click_1(object sender, EventArgs e)
        {
            string tenLoai = txtTenLoai.Text.Trim(); // Tên loại nước
            int maLoaiNuoc; // Mã loại nước

            // Kiểm tra xem mã loại nước có hợp lệ không
            if (int.TryParse(txtMaLoai.Text.Trim(), out maLoaiNuoc)) // Chuyển đổi mã loại từ TextBox
            {
                if (!string.IsNullOrEmpty(tenLoai)) // Kiểm tra tên loại không rỗng
                {
                    try
                    {
                        QLLoaiSP.ThemLoai(maLoaiNuoc, tenLoai); // Gọi phương thức thêm loại nước
                        MessageBox.Show("Thêm loại nước thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadLoaiMon();// Tải lại danh sách loại sau khi thêm

                        txtTenLoai.Clear(); // Xóa TextBox tên loại
                        txtMaLoai.Clear(); // Xóa TextBox mã loại
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập tên loại nước!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập mã loại nước hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnXoa_Click_1(object sender, EventArgs e)
        {
            if (selectedMaLoaiNuoc > 0) // Kiểm tra xem đã chọn mã loại nước hợp lệ chưa
            {
                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa loại nước này cùng với tất cả sản phẩm thuộc loại này?",
                                                     "Xác nhận xóa",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question);
                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        QLLoaiSP.XoaLoai(selectedMaLoaiNuoc); // Gọi phương thức xóa loại nước
                        MessageBox.Show("Xóa loại nước và các sản phẩm thuộc loại này thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadLoaiMon(); // Tải lại danh sách loại sau khi xóa
                        txtTenLoai.Clear(); // Xóa TextBox tên loại
                        txtMaLoai.Clear(); // Xóa TextBox mã loại
                        selectedMaLoaiNuoc = 0; // Đặt lại mã loại nước đã chọn
                    }
                    catch (DbUpdateException dbEx)
                    {
                        MessageBox.Show($"Lỗi cập nhật cơ sở dữ liệu: {dbEx.Message}\n\nChi tiết: {dbEx.InnerException?.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi: {ex.Message}\n\nChi tiết: {ex.InnerException?.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn loại nước để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateDataGridViewRow(int maLoaiNuoc, string tenLoai)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["MaLoaiNuoc"].Value != null && (int)row.Cells["MaLoaiNuoc"].Value == maLoaiNuoc)
                {
                    row.Cells["TenLoaiNuoc"].Value = tenLoai; // Cập nhật tên loại nước
                    break; // Dừng vòng lặp khi đã cập nhật xong
                }
            }
        }
        private void btnSua_Click_1(object sender, EventArgs e)
        {
            int maLoaiNuoc;
            string tenLoai = txtTenLoai.Text.Trim();

            if (int.TryParse(txtMaLoai.Text.Trim(), out maLoaiNuoc))
            {
                if (!string.IsNullOrEmpty(tenLoai))
                {
                    try
                    {
                        // Kiểm tra xem loại món có tồn tại hay không
                        var existingLoai = QLLoaiSP.LayDanhSachLoai().FirstOrDefault(l => l.MaLoaiNuoc == maLoaiNuoc);
                        if (existingLoai != null)
                        {
                            // Gọi phương thức sửa loại
                            QLLoaiSP.SuaLoai(maLoaiNuoc, tenLoai);
                            MessageBox.Show("Sửa loại món thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadLoaiMon();
                            // Cập nhật dòng tương ứng trong DataGridView
                            UpdateDataGridViewRow(maLoaiNuoc, tenLoai);

                            // Xóa các trường nhập liệu
                            ClearInputFields();
                        }
                        else
                        {
                            MessageBox.Show("Mã loại món không tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (DbUpdateException dbEx)
                    {
                        MessageBox.Show($"Lỗi cập nhật cơ sở dữ liệu: {dbEx.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập tên loại món!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập mã loại món hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Đảm bảo rằng người dùng đã nhấn vào một hàng hợp lệ
            {
                var selectedRow = dataGridView1.Rows[e.RowIndex];

                // Hiển thị dữ liệu vào các TextBox
                txtMaLoai.Text = selectedRow.Cells["MaLoaiNuoc"].Value.ToString(); // Giả sử tên cột là "MaLoaiNuoc"
                txtTenLoai.Text = selectedRow.Cells["TenLoaiNuoc"].Value.ToString(); // Giả sử tên cột là "TenLoaiNuoc"

                selectedMaLoaiNuoc = int.Parse(txtMaLoai.Text); // Cập nhật mã loại nước đã chọn
            }
        }

        private void ThemLoaiMon_Load(object sender, EventArgs e)
        {
            LoadLoaiMon();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void ClearInputFields()
        {
            txtTenLoai.Clear();
            txtMaLoai.Clear();
        }
    }
}
