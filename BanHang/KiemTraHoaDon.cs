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
    public partial class KiemTraHoaDon : Form
    {
        private QLSanPamService QLCafeService;   // Service quản lý sản phẩm
        private QLLoaiSP LoaiSanPhamService;     // Service quản lý loại sản phẩm
        private ThongKeService thongke;

        public KiemTraHoaDon()
        {
            InitializeComponent();
            thongke = new ThongKeService();
            QLCafeService = new QLSanPamService(); // Khởi tạo QLSanPamService
            LoaiSanPhamService = new QLLoaiSP(); // Khởi tạo QLLoaiSP
        }

        private void LoadDanhSachDonHang(DateTime? ngayTao = null)
        {
            try
            {
                var danhSachDonHang = thongke.LayDanhSachDonHang();

                if (ngayTao != null)
                {
                    // Lọc hóa đơn theo ngày tạo được chọn
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
                        TenKhachHang = dh.KhachHang != null ? dh.KhachHang.TenKhachHang : "Không có",
                        HinhThucThanhToan = dh.HinhThucThanhToan
                    }).ToList();
                }
                else
                {
                    dataGridView1.DataSource = null;
                    MessageBox.Show("Không có đơn hàng nào cho ngày này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách đơn hàng: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadChiTietDonHang(int maDonHang)
        {
            var chiTietDonHang = thongke.LayChiTietDonHang(maDonHang);

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
        private int tongDonHang; // Tổng số đơn hàng
        private decimal tongDoanhThu;

        private void KiemTraHoaDon_Load(object sender, EventArgs e)
        {
            LoadDanhSachDonHang(DateTime.Today);

            // Lấy tổng đơn hàng và tổng doanh thu
            tongDonHang = thongke.LayTongDonHangTheoNgay(DateTime.Today); // Gán giá trị cho biến thành viên
            tongDoanhThu = thongke.LayTongDoanhThuTheoNgay(DateTime.Today); // Gán giá trị cho biến thành viên


        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
