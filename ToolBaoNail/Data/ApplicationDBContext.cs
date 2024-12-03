using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Browser.Dom;
using Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace ToolBaoNail.Data
{
    public class ApplicationDBContext : DbContext
    {
        public DbSet<AdDetailInfo> AdDetailInfos { get; set; }
        public DbSet<AdInfo> AdInfos { get; set; }
        public DbSet<StateInfo> StateInfos { get; set; }


        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        
    }
}
