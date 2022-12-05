using Microsoft.EntityFrameworkCore;

namespace SubcriptionManagement.Models
{
    public class SubcriptionDbContext : DbContext
    {
        public SubcriptionDbContext() { }
        public SubcriptionDbContext(DbContextOptions<SubcriptionDbContext> options) : base(options) { }

        public virtual DbSet<Subscriber> Subscribers { get; set; }
        public virtual DbSet<SubscriberChoice> SubscriberChoices { get; set; }

    }
}
