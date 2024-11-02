namespace DAL.D.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BaoCaoDoanhThu")]
    public partial class BaoCaoDoanhThu
    {
        [Key]
        public int MaBaoCao { get; set; }

        [Column(TypeName = "date")]
        public DateTime NgayBaoCao { get; set; }

        public decimal TongDoanhThu { get; set; }

        public int SoLuongDonHang { get; set; }
    }
}
