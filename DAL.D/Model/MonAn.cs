using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.D.Model
{
    public class MonAn
    {
        public int MaMon { get; set; }  // Mã món ăn
        public string TenMon { get; set; }  // Tên món ăn
        public decimal Gia { get; set; }  // Giá món ăn
        public int SoLuong { get; set; }  // Số lượng món ăn
    }
}
