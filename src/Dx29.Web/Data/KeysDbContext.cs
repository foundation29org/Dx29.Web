using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dx29.Web.Data
{
    public class KeysDbContext : DbContext, IDataProtectionKeyContext
    {
        public KeysDbContext(DbContextOptions<KeysDbContext> options)
            : base(options)
        {
        }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
}
