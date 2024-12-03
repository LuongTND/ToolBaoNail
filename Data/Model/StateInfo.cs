using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class StateInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StateInfoId { get; set; }

        public string? Country { get; set; }
        public string? Title { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }

        public ICollection<AdDetailInfo>? AdDetailInfos { get; set; }

        public ICollection<AdInfo>? AdInfos { get; set; }

    }
}
