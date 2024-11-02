using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace BanHang
{
    public partial class frmDangNhap : Form
    {



        public frmDangNhap()
        {

            InitializeComponent();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            sslHienThiNgayGio.Text = DateTime.Now.ToString(" dd/MM/yyyy HH:mm:ss");
        }

        private void btnQuanLy_Click(object sender, EventArgs e)
        {
            QuanLy ql = new QuanLy(); // Tạo đối tượng Form2
            ql.Show(); // Hiển thị Form2

        }

        private void btnBanHang_Click(object sender, EventArgs e)
        {
            BanHang bh = new BanHang();
            bh.Show();
        }
    }
}
