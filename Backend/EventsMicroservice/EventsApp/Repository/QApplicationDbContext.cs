using Domain.Identity;
using Domain.SecondAppModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class QApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Questions> Questions { get; set; }
        public DbSet<Databases> Databases { get; set; }

        public QApplicationDbContext(DbContextOptions<QApplicationDbContext> options)
            : base(options)
        {

        }
    }
}
