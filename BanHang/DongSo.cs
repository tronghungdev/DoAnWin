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

    public partial class DongSo : Form
    {
        private ThongKeService thongKeService;
        private QLDonHang qlDonHang; // Tạo đối tượng quản lý đơn hàng (BLL)
        private List<DonHang> danhSachDonHang; // Danh sách đơn hàng để xử lý

        // Khởi tạo các biến để lưu thông tin doanh thu
        private decimal tongDoanhThu;
        private int tongHoaDon;
        private decimal doanhThuChuyenKhoan;
        private decimal doanhThuMomo;
        private decimal doanhThuTienMat;

        public DongSo()
        {
            InitializeComponent();
            qlDonHang = new QLDonHang();
            KhoiTaoDanhSachDonHang();
            thongKeService = new ThongKeService();
        }
        /*public DongSo(decimal tongDoanhThu, int tongHoaDon, decimal doanhThuChuyenKhoan, decimal doanhThuMomo, decimal doanhThuTienMat)
        {
            InitializeComponent();
            qlDonHang = new QLDonHang();
            this.tongDoanhThu = tongDoanhThu;
            this.tongHoaDon = tongHoaDon;
            this.doanhThuChuyenKhoan = doanhThuChuyenKhoan;
            this.doanhThuMomo = doanhThuMomo;
            this.doanhThuTienMat = doanhThuTienMat;


        }*/
        public void KhoiTaoDanhSachDonHang()
        {
            // Lấy danh sách đơn hàng của ngày hôm nay
            danhSachDonHang = qlDonHang.LayDanhSachDonHangTheoNgay(DateTime.Today); // Gọi phương thức với ngày hôm nay

            // Tính toán doanh thu dựa trên danh sách đơn hàng hôm nay
            if (danhSachDonHang != null && danhSachDonHang.Any())
            {
                tongDoanhThu = qlDonHang.TinhTongDoanhThu(DateTime.Today); // Tính tổng doanh thu hôm nay
                tongHoaDon = qlDonHang.TinhTongHoaDon(DateTime.Today); // Tính tổng số hóa đơn hôm nay
                doanhThuChuyenKhoan = qlDonHang.TinhDoanhThuChuyenKhoan(DateTime.Today); // Tính doanh thu chuyển khoản hôm nay
                doanhThuMomo = qlDonHang.TinhDoanhThuMomo(DateTime.Today); // Tính doanh thu từ Momo hôm nay
                doanhThuTienMat = qlDonHang.TinhDoanhThuTienMat(DateTime.Today); // Tính doanh thu từ tiền mặt hôm nay
            }
            else
            {
                MessageBox.Show("Không có đơn hàng nào cho ngày hôm nay.");
            }
        }

        private void DongSo_Load(object sender, EventArgs e)
        {
            KhoiTaoDanhSachDonHang(); // Gọi phương thức để khởi tạo danh sách đơn hàng

            // Kiểm tra xem danh sách có được tải không
            if (danhSachDonHang == null || !danhSachDonHang.Any())
            {
                MessageBox.Show("Không có đơn hàng nào được tìm thấy.");
                return; // Nếu không có đơn hàng, không cần thực hiện thêm
            }

            // Cập nhật thông tin cho form
            lbtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            lblTime.Text = DateTime.Now.ToString("HH:mm:ss");

            // Hiển thị các thông tin về doanh thu
            lblTongDoanhThu.Text = tongDoanhThu.ToString("C2"); // Định dạng tiền tệ
            lblTongHoaDon.Text = tongHoaDon.ToString(); // Tổng hóa đơn là số lượng
            lblDoanhThuChuyenKhoan.Text = doanhThuChuyenKhoan.ToString("C2"); // Định dạng tiền tệ
            lblDoanhThuMomo.Text = doanhThuMomo.ToString("C2"); // Định dạng tiền tệ
            lblDoanhThuTienMat.Text = doanhThuTienMat.ToString("C2");

            // In ra để kiểm tra
            Console.WriteLine($"Tổng doanh thu: {tongDoanhThu}, Tổng hóa đơn: {tongHoaDon}, Doanh thu chuyển khoản: {doanhThuChuyenKhoan}, Doanh thu Momo: {doanhThuMomo}, Doanh thu tiền mặt: {doanhThuTienMat}");
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Close();
        }
    }

}
