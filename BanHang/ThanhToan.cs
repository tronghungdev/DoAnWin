using BLL.DoAn;
using DAL.D.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BanHang
{
    public partial class ThanhToan : Form
    {
        private QLOderService qloderService;

        public event Action OnOrderSuccess;
        private Oder oderForm;




        public ThanhToan(Oder oderForm, List<SelectedSanPham> sanPhams, int tongSoLuong, decimal tongTien)
        {
            InitializeComponent();
            this.oderForm = oderForm;

            // Hiển thị số lượng và tổng tiền
            txtTongMon.Text = $"{tongSoLuong}";
            txtTongGia.Text = $"{tongTien:C2}"; // Định dạng tiền tệ
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            qloderService = new QLOderService();

            // Thêm sản phẩm vào qloderService
            foreach (var sp in sanPhams)
            {
                qloderService.AddSelectedSanPham(sp);
            }

            // Chuyển dữ liệu từ danh sách sanPhams vào dataGridView1
            dataGridView1.DataSource = sanPhams.Select(sp => new
            {
                MaSanPham = sp.MaSanPham,
                TenSanPham = sp.TenSanPham,
                SoLuong = sp.SoLuong,
                Gia = sp.Gia,
                TongTien = sp.Gia * sp.SoLuong
            }).ToList();

            // Thêm các hình thức thanh toán vào ComboBox
            cmbHinhThucThanhToan.Items.Add("Tiền mặt");
            cmbHinhThucThanhToan.Items.Add("Chuyển khoản");
            cmbHinhThucThanhToan.Items.Add("Momo");
            cmbHinhThucThanhToan.SelectedIndex = 0;
            txtGiamGia.Text = "0";
        }

        private void txtGiamGia_TextChanged(object sender, EventArgs e)
        {

            decimal giamGia = 0;

            // Kiểm tra xem người dùng có nhập vào giá trị hợp lệ không
            if (decimal.TryParse(txtGiamGia.Text, out decimal inputGiamGia))
            {
                giamGia = inputGiamGia; // Nếu có, gán giá trị cho giamGia
            }

            // Kiểm tra điều kiện giảm giá
            if (giamGia < 0 || giamGia > 100)
            {
                MessageBox.Show("Giảm giá phải từ 0 đến 100%", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTongThanhToan.Text = string.Empty; // Xóa nội dung nếu không hợp lệ
                return;
            }

            var selectedSanPhams = qloderService.GetSelectedSanPhams(); // Lấy danh sách sản phẩm đã chọn

            if (selectedSanPhams.Count == 0)
            {
                MessageBox.Show("Không có sản phẩm nào đã chọn để thanh toán.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTongThanhToan.Text = string.Empty; // Xóa nội dung nếu không có sản phẩm
                return;
            }

            // Tính tổng tiền sau giảm giá
            decimal tongThanhToan = qloderService.TinhTongSauGiamGia(giamGia);

            // Hiển thị kết quả
            txtTongThanhToan.Text = tongThanhToan.ToString("C2"); // Định dạng tiền tệ
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void btnThanhToanCuoi_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTongThanhToan.Text) ||
       !decimal.TryParse(txtTongThanhToan.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal tongTien) ||
       tongTien <= 0)
            {
                MessageBox.Show("Giá trị tổng thanh toán không hợp lệ hoặc không được phép bằng 0."); // Thông báo lỗi
                return;
            }

            // Gán tổng tiền vào đơn hàng
            var donHang = new DonHang
            {
                NgayTao = DateTime.Now,
                HinhThucThanhToan = cmbHinhThucThanhToan.SelectedItem?.ToString(),
                MaTaiKhoan = 1, // Gán giá trị mặc định hoặc lấy từ thông tin người dùng
                TongTien = tongTien // Gán tổng tiền vào đơn hàng
            };

            // Khởi tạo danh sách chi tiết đơn hàng
            List<ChiTietDonHang> chiTietDonHangs = new List<ChiTietDonHang>();

            // Duyệt qua DataGridView để lấy thông tin chi tiết đơn hàng
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue; // Bỏ qua dòng mới (dòng trống)

                if (row.Cells["MaSanPham"].Value == null || row.Cells["SoLuong"].Value == null || row.Cells["Gia"].Value == null)
                {
                    MessageBox.Show("Thông tin sản phẩm không hợp lệ."); // Thông báo lỗi nếu dữ liệu sản phẩm không hợp lệ
                    return;
                }

                var chiTiet = new ChiTietDonHang
                {
                    MaSanPham = Convert.ToInt32(row.Cells["MaSanPham"].Value),
                    SoLuong = Convert.ToInt32(row.Cells["SoLuong"].Value),
                    GiaBan = Convert.ToDecimal(row.Cells["Gia"].Value)
                };

                chiTietDonHangs.Add(chiTiet);
            }

            // Khởi tạo đối tượng khách hàng với thông tin null
            KhachHang khachHang = null;

            // Kiểm tra thông tin khách hàng, nếu có thông tin thì khởi tạo đối tượng
            if (!string.IsNullOrWhiteSpace(txtTenKhachHang.Text) && !string.IsNullOrWhiteSpace(txtSDT.Text))
            {
                khachHang = new KhachHang
                {
                    TenKhachHang = txtTenKhachHang.Text,
                    SoDienThoai = txtSDT.Text
                };
            }

            // Gọi phương thức lưu đơn hàng và khách hàng
            QLDonHang donHangBLL = new QLDonHang();
            bool isAdded = donHangBLL.LuuDonHang(donHang, khachHang, chiTietDonHangs);

            // Thông báo cho người dùng về kết quả
            if (isAdded)
            {
                MessageBox.Show("Thêm đơn hàng thành công!"); // Thông báo thành công

                oderForm.Close();
                // Xóa các trường nhập liệu sau khi lưu thành công
                txtGiamGia.Text = string.Empty;
                txtTongThanhToan.Text = string.Empty;
                txtTenKhachHang.Text = string.Empty; // Xóa thông tin khách hàng
                txtSDT.Text = string.Empty; // Xóa số điện thoại
                dataGridView1.DataSource = null; // Xóa dữ liệu trong DataGridView
                cmbHinhThucThanhToan.SelectedIndex = 0; // Đặt lại ComboBox

                OnOrderSuccess?.Invoke(); // Gọi sự kiện khi đơn hàng thành công
                this.Close(); // Ẩn form ThanhToan
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi thêm đơn hàng."); // Thông báo lỗi
            }
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            string soDienThoai = txtSDT.Text.Trim();
            string tenKhachHang = txtTenKhachHang.Text.Trim();

            if (string.IsNullOrEmpty(soDienThoai) || string.IsNullOrEmpty(tenKhachHang))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin khách hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var khachHangService = new KhachHangService(new CafeModel());
            var khachHang = khachHangService.LayKhachHangTheoSDT(soDienThoai);

            if (khachHang != null)
            {
                MessageBox.Show("Khách hàng đã tồn tại trong cơ sở dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // Tạo mới đối tượng khách hàng
                var newKhachHang = new KhachHang
                {
                    TenKhachHang = tenKhachHang,
                    SoDienThoai = soDienThoai
                };
                khachHangService.ThemKhachHang(newKhachHang);
                MessageBox.Show("Khách hàng đã được lưu thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtSDT_TextChanged(object sender, EventArgs e)
        {
            string soDienThoai = txtSDT.Text.Trim();

            if (!string.IsNullOrEmpty(soDienThoai))
            {
                var khachHangService = new KhachHangService(new CafeModel());
                var khachHang = khachHangService.LayKhachHangTheoSDT(soDienThoai);

                if (khachHang != null)
                {
                    txtTenKhachHang.Text = khachHang.TenKhachHang;
                }
                else
                {
                    // Nếu không tìm thấy, xóa thông tin
                    txtTenKhachHang.Text = string.Empty;
                }
            }
        }



    }
}


