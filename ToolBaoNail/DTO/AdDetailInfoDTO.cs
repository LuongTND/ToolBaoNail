using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolBaoNail.DTO
{
    public class AdDetailInfoDTO
    {
        public int AdDetailInfoId { get; set; }

        public int StateInfoId { get; set; }

        public int AdInfoId { get; set; }

        public string? TitleVi { get; set; }
        public string? TitleEn { get; set; }
        public string? AdId { get; set; }
        public string? LastUpdated { get; set; }
        public string? ContentVi { get; set; }
        public string? ContentEn { get; set; }
        public string? ContactInfo1 { get; set; }
        public string? ContactInfo2 { get; set; }
        public string? ContactInfo3 { get; set; }
        public string? StoreAddress { get; set; }
        public List<string>? Images { get; set; } = new List<string>();
    }
}
