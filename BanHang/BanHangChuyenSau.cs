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
    public partial class BanHangChuyenSau : Form
    {
        private List<SanPham> selectedSanPhams = new List<SanPham>();
        QLDonHang QLDonHang = new QLDonHang();
        QLOderService QLOderService = new QLOderService();

        // Constructor nhận số bàn
        private Dictionary<int, List<SanPham>> selectedSanPhamsPerTable = new Dictionary<int, List<SanPham>>();

        public BanHangChuyenSau(int soban)
        {

            InitializeComponent();
            

        }
        private void HienThiFormBan(int soBan)
        {
            this.WindowState = FormWindowState.Normal;

            // Kiểm tra xem bàn đã có món đã chọn chưa
            if (!selectedSanPhamsPerTable.ContainsKey(soBan))
            {
                selectedSanPhamsPerTable[soBan] = new List<SanPham>(); // Tạo danh sách mới nếu chưa có
            }

            // Lấy danh sách món đã chọn cho bàn và mở form Oder với danh sách đó
            Oder formBan = new Oder(soBan, selectedSanPhamsPerTable[soBan]);
            formBan.Show();
        }
        private void btnBan1_Click(object sender, EventArgs e)
        {
            HienThiFormBan(1);
        }

        private void btnBan2_Click(object sender, EventArgs e)
        {
            HienThiFormBan(2);
        }

        private void btnBan3_Click(object sender, EventArgs e)
        {
            HienThiFormBan(3);
        }

        private void btnBan4_Click(object sender, EventArgs e)
        {
            HienThiFormBan(4);
        }

        private void btnBan5_Click(object sender, EventArgs e)
        {
            HienThiFormBan(5);
        }

        private void btnBan6_Click(object sender, EventArgs e)
        {
            HienThiFormBan(6);
        }

        private void btnBan7_Click(object sender, EventArgs e)
        {
            HienThiFormBan(7);
        }

        private void btnBan8_Click(object sender, EventArgs e)
        {
            HienThiFormBan(8);
        }

        private void btnBan9_Click(object sender, EventArgs e)
        {
            HienThiFormBan(9);
        }

        private void BanHangChuyenSau_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // Không cho phép thay đổi kích thước
            this.WindowState = FormWindowState.Normal;
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất không?",
                                           "Xác nhận đăng xuất",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Question);

            // Kiểm tra kết quả từ hộp thoại
            if (result == DialogResult.Yes)
            {
                // Thực hiện các hành động đăng xuất
                // Ví dụ: Đưa người dùng về màn hình đăng nhập
                this.Hide(); // Ẩn form hiện tại (có thể là form chính)
            }
        }

        private void btnDongso_Click(object sender, EventArgs e)
        {


            DongSo dongSo = new DongSo();
            dongSo.ShowDialog();
        }

        private void btnHoaDon_Click(object sender, EventArgs e)
        {
            KiemTraHoaDon kiemTraHoaDon = new KiemTraHoaDon();
            kiemTraHoaDon.ShowDialog();
        }
    }
}
