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
    public partial class ThongKeBanHang : Form
    {
        private QLSanPamService QLCafeService;   // Service quản lý sản phẩm
        private QLLoaiSP LoaiSanPhamService;     // Service quản lý loại sản phẩm
        private ThongKeService thongKeService;   // Service thống kê

        public ThongKeBanHang()
        {
            InitializeComponent();
            QLCafeService = new QLSanPamService();
            LoaiSanPhamService = new QLLoaiSP();
            thongKeService = new ThongKeService();
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            LoadDanhSachDonHang();
        }
        private void LoadDanhSachDonHang(DateTime? ngayTao = null)
        {
            try
            {
                var danhSachDonHang = thongKeService.LayDanhSachDonHang();

                if (ngayTao != null)
                {
                    danhSachDonHang = danhSachDonHang
                        .Where(dh => dh.NgayTao.Date == ngayTao.Value.Date)
                        .ToList();
                }

                if (danhSachDonHang != null && danhSachDonHang.Count > 0)
                {
                    dataGridView1.DataSource = danhSachDonHang.Select(dh => new
                    {
                        dh.MaDonHang,
                        dh.NgayTao,
                        dh.TongTien,
                        TenKhachHang = dh.KhachHang?.TenKhachHang ?? "Không có",
                        HinhThucThanhToan = dh.HinhThucThanhToan
                    }).ToList();

                    CapNhatThongKe();
                }
                else
                {
                    dataGridView1.DataSource = null;
                    MessageBox.Show("Không có đơn hàng nào cho ngày này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách đơn hàng: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void ThongKeBanHang_Load(object sender, EventArgs e)
        {



            DateTime currentDate = DateTime.Now.Date; // Ngày hiện tại
            dateTimePicker1.Value = currentDate; // Đặt giá trị của DateTimePicker thành ngày hiện tại
            LoadDanhSachDonHang(currentDate); // Tải danh sách đơn hàng cho ngày hiện tại

            // Lấy tổng đơn hàng và tổng doanh thu cho ngày hiện tại
            int tongDonHang = thongKeService.LayTongDonHangTheoNgay(currentDate);
            decimal tongDoanhThu = thongKeService.LayTongDoanhThuTheoNgay(currentDate);

            // Hiển thị tổng số đơn hàng và tổng doanh thu lên TextBox
            lblDonHangTheoNgay.Text = tongDonHang.ToString();
            lblTongDoanhThu.Text = tongDoanhThu.ToString("C");
            CapNhatThongKe();
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = dateTimePicker1.Value; // Lấy giá trị từ DateTimePicker
            LoadDanhSachDonHang(selectedDate);
            CapNhatThongKe();


            // Cập nhật thống kê nếu cần

        }




        private void LoadChiTietDonHang(int maDonHang)
        {
            var chiTietDonHang = thongKeService.LayChiTietDonHang(maDonHang);

            if (chiTietDonHang != null && chiTietDonHang.Any())
            {
                // Gán dữ liệu vào DataGridView2
                dataGridView2.DataSource = chiTietDonHang.Select(ct => new
                {
                    ct.MaSanPham,
                    TenSanPham = QLCafeService.LayMonAnTheoId(ct.MaSanPham)?.TenSanPham, // Gọi hàm lấy tên sản phẩm từ QLSanPamService
                    ct.SoLuong,
                    ct.GiaBan
                }).ToList();
                dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
            {
                dataGridView2.DataSource = null; // Clear previous data
                MessageBox.Show("Không có chi tiết đơn hàng cho mã đơn hàng này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Kiểm tra nếu nhấp vào hàng hợp lệ
            {
                var selectedRow = dataGridView1.Rows[e.RowIndex];
                if (int.TryParse(selectedRow.Cells["MaDonHang"].Value.ToString(), out int maDonHang))
                {
                    LoadChiTietDonHang(maDonHang); // Gọi phương thức để tải chi tiết đơn hàng
                }
                else
                {
                    MessageBox.Show("Mã đơn hàng không hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];
                string maDonHang = selectedRow.Cells["MaDonHang"].Value.ToString();

                // Xác nhận xóa đơn hàng
                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa đơn hàng này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        // Gọi phương thức xóa đơn hàng trong QLCafeService
                        thongKeService.XoaDonHang(maDonHang);

                        // Tải lại danh sách đơn hàng sau khi xóa
                        LoadDanhSachDonHang();

                        // Cập nhật thông tin tổng đơn hàng và tổng doanh thu
                        CapNhatThongKe();

                        MessageBox.Show("Đơn hàng đã được xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xóa đơn hàng: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một đơn hàng để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void CapNhatThongKe()
        {
            try
            {
                // Kiểm tra nếu DataGridView có dữ liệu
                if (dataGridView1.Rows.Count > 0)
                {
                    // Tính tổng số đơn hàng
                    int tongDonHang = dataGridView1.Rows.Count; // Số lượng đơn hàng

                    // Tính tổng doanh thu
                    decimal tongDoanhThu = 0;

                    // Biến lưu tổng số đơn hàng trong ngày được chọn từ DateTimePicker
                    int tongDonHangTheoNgay = 0;

                    // Lấy ngày từ DateTimePicker để so sánh
                    DateTime ngayDuocChon = dateTimePicker1.Value.Date;

                    // Duyệt qua từng hàng trong DataGridView để tính tổng doanh thu và số đơn hàng theo ngày
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (!row.IsNewRow) // Bỏ qua dòng trống mới
                        {
                            // Tính tổng doanh thu
                            if (row.Cells["TongTien"].Value != DBNull.Value)
                            {
                                tongDoanhThu += Convert.ToDecimal(row.Cells["TongTien"].Value); // Cộng dồn giá trị
                            }

                            // Lấy thông tin ngày từ cột NgayTao
                            if (row.Cells["NgayTao"].Value != DBNull.Value)
                            {
                                DateTime ngayTaoDonHang = Convert.ToDateTime(row.Cells["NgayTao"].Value);

                                // Kiểm tra nếu đơn hàng được tạo vào ngày được chọn từ DateTimePicker
                                if (ngayTaoDonHang.Date == ngayDuocChon)
                                {
                                    tongDonHangTheoNgay++; // Cộng thêm số đơn hàng của ngày được chọn
                                }
                            }
                        }
                    }

                    // Cập nhật giao diện người dùng

                    lblTongDoanhThu.Text = tongDoanhThu.ToString("C"); // Định dạng tiền tệ

                    // Cập nhật số lượng đơn hàng trong ngày được chọn từ DateTimePicker
                    lblDonHangTheoNgay.Text = tongDonHangTheoNgay.ToString();
                }
                else
                {
                    // Nếu không có đơn hàng nào

                    lblTongDoanhThu.Text = "0";
                    lblDonHangTheoNgay.Text = "0";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật thông tin thống kê: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
