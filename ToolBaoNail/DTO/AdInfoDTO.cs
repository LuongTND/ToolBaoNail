using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolBaoNail.DTO
{
    public class AdInfoDTO
    {

        //public int AdInfoId { get; set; }

        public int? StateInfoId { get; set; }
  

        public string? StoreName { get; set; }
        public string? Location { get; set; }
        public string? State { get; set; }
        public string? AdTitle { get; set; }
        public string? AdMessage { get; set; }
        public string? AdId { get; set; }
        public string? StoreUrl { get; set; }
    }
}
