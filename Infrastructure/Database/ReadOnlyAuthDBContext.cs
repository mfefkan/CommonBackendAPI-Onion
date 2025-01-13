using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database
{
    public class ReadOnlyAuthDBContext : BaseDbContext
    {
        public ReadOnlyAuthDBContext(DbContextOptions<ReadOnlyAuthDBContext> options) : base(options) { }
    }
}
