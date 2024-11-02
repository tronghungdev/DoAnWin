using BLL.DoAn;
using DAL.D.Model;
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
    public partial class ThongTinKhachHang : Form
    {
        private KhachHangService khachHangService;
        public ThongTinKhachHang()
        {
            InitializeComponent();
            khachHangService = new KhachHangService(new CafeModel());
            LoadDataGridView();
        }

        private void LoadDataGridView()
        {
            var khachHangs = khachHangService.LayTatCaKhachHang();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.DataSource = khachHangs.Select(kh => new
            {
                kh.MaKhachHang,
                kh.TenKhachHang,
                kh.SoDienThoai
            }).ToList();
        }
        private void ThongTinKhachHang_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedRow = dataGridView1.Rows[e.RowIndex];
                lblMaKH.Text = selectedRow.Cells["MaKhachHang"].Value.ToString();
                txtHoVaTen.Text = selectedRow.Cells["TenKhachHang"].Value.ToString();
                txtSDT.Text = selectedRow.Cells["SoDienThoai"].Value.ToString();
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                int maKhachHang = Convert.ToInt32(lblMaKH.Text);
                khachHangService.XoaKhachHang(maKhachHang);
                MessageBox.Show("Khách hàng đã được xóa thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(lblMaKH.Text))
            {
                var khachHang = new KhachHang
                {
                    MaKhachHang = Convert.ToInt32(lblMaKH.Text),
                    TenKhachHang = txtHoVaTen.Text,
                    SoDienThoai = txtSDT.Text
                };

                var result = khachHangService.SuaKhachHang(khachHang);
                if (result)
                {
                    MessageBox.Show("Cập nhật thông tin khách hàng thành công.");
                    LoadDataGridView(); 
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi cập nhật thông tin.");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn khách hàng để sửa.");
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            string soDienThoai = txtSDT.Text.Trim();
            string tenKhachHang = txtHoVaTen.Text.Trim();

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
                txtHoVaTen.Clear();
                txtSDT.Clear();
                lblMaKH.Enabled = false;
            }
            else
            {
                var newKhachHang = new KhachHang
                {
                    TenKhachHang = tenKhachHang,
                    SoDienThoai = soDienThoai
                };

                khachHangService.ThemKhachHang(newKhachHang);
                MessageBox.Show("Khách hàng đã được lưu thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataGridView();
                txtHoVaTen.Text = string.Empty;
                txtSDT.Text = string.Empty;
                lblMaKH.Text = string.Empty;
            }
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            string tuKhoa = txtTimKiem.Text.Trim();

            if (!string.IsNullOrEmpty(tuKhoa))
            {
                var ketQuaTimKiem = khachHangService.TimKiemKhachHang(tuKhoa);
                dataGridView1.DataSource = ketQuaTimKiem.Select(kh => new
                {
                    kh.MaKhachHang,
                    kh.TenKhachHang,
                    kh.SoDienThoai
                }).ToList();
            }
            else
            {
                LoadDataGridView(); 
            }
        }
    }
}
