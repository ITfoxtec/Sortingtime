using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sortingtime.SqlMigrations;

namespace Sortingtime.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long>
    {
        public DbSet<Log> Logs { get; set; }

        public DbSet<Demo> Demos { get; set; }

        public DbSet<Partition> Partitions { get; set; }
        public DbSet<Organization> Organizations { get; set; }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Ttask> Tasks { get; set; }
        public DbSet<TtaskItem> TaskItems { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<MaterialItem> MaterialItems { get; set; }

        public DbSet<DefaultGroup> DefaultGroups { get; set; }
        public DbSet<DefaultTask> DefaultTasks { get; set; }
        public DbSet<DefaultMaterial> DefaultMaterials { get; set; }

        public DbSet<ReportSetting> ReportSettings { get; set; }
        public DbSet<Report> Reports { get; set; }

        public DbSet<InvoiceSetting> InvoiceSettings { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<HourPriceSetting> HourPriceSettings { get; set; }

        public ApplicationDbContext() : base(new DbContextOptions<ApplicationDbContext>())
        { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Startup.Configuration.GetConnectionString("DefaultConnection"));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Hack da StringLength attributterne vikke virker.
            builder.Entity<ApplicationUser>().Property(x => x.UserName).HasMaxLength(200);
            builder.Entity<ApplicationUser>().Property(x => x.Email).HasMaxLength(200);
            builder.Entity<ApplicationUser>().Property(x => x.PhoneNumber).HasMaxLength(100);

            // Sets SQL Date type
            builder.Entity<TtaskItem>(eb =>
            {
                eb.Property(b => b.Date).HasColumnType("Date");
            });
            builder.Entity<MaterialItem>(eb =>
            {
                eb.Property(b => b.Date).HasColumnType("Date");
            });
            builder.Entity<Report>(eb =>
            {
                eb.Property(b => b.FromDate).HasColumnType("Date");
                eb.Property(b => b.ToDate).HasColumnType("Date");
            });
            builder.Entity<Invoice>(eb =>
            {
                eb.Property(b => b.InvoiceDate).HasColumnType("Date");
            });           

            // To prevent duplicate foreign keys for IdentityUser POCO Navigation Properties
            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Logins)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
       
            ModelIndexes(builder);

            ModelRelations(builder);
        }

        private static void ModelIndexes(ModelBuilder builder)
        {
            builder.Entity<DefaultGroup>().HasIndex(x => x.UserId);
            builder.Entity<DefaultTask>().HasIndex(x => x.UserId);

            builder.Entity<Group>().HasIndex(x => new { x.PartitionId, x.Name });
            builder.Entity<Ttask>().HasIndex(x => new { x.GroupId, x.Name });
            builder.Entity<Material>().HasIndex(x => new { x.GroupId, x.Name });

            builder.Entity<TtaskItem>().HasIndex(x => new { x.UserId, x.Date });
            builder.Entity<TtaskItem>().HasIndex(x => x.TaskId);
            builder.Entity<MaterialItem>().HasIndex(x => new { x.UserId, x.Date });
            builder.Entity<MaterialItem>().HasIndex(x => x.MaterialId);

            builder.Entity<Report>().HasIndex(x => new { x.PartitionId, x.Status, x.FromDate, x.ToDate });
            builder.Entity<ReportSetting>().HasIndex(x => new { x.PartitionId, x.ReferenceType, x.ReferenceKey });

            builder.Entity<Invoice>().HasIndex(x => new { x.PartitionId, x.Status, x.InvoiceDate });
            builder.Entity<InvoiceSetting>().HasIndex(x => new { x.PartitionId, x.ReferenceType, x.ReferenceKey });
            builder.Entity<HourPriceSetting>().HasIndex(x => new { x.PartitionId, x.UserId, x.GroupReferenceKey, x.TaskReferenceKey });
        }

        private static void ModelRelations(ModelBuilder builder)
        {
            // Partition -1- Organization. Using the PartitionID as primary ID for both tables
            builder.Entity<Partition>()
                .HasOne(p => p.Organization)
                .WithOne(o => o.Partition)
                .HasForeignKey<Organization>(p => p.Id);

            // Partition --> Group
            builder.Entity<Group>()
                .HasOne(g => g.Partition)
                .WithMany(p => p.Groups)
                .HasForeignKey(g => g.PartitionId);

            // Partition --> ReportSetting
            builder.Entity<ReportSetting>()
                .HasOne(rs => rs.Partition)
                .WithMany(p => p.ReportSettings)
                .HasForeignKey(rs => rs.PartitionId);

            // Partition --> Report
            builder.Entity<Report>()
                .HasOne(r => r.Partition)
                .WithMany(p => p.Reports)
                .HasForeignKey(r => r.PartitionId);

            // Partition --> InvoiceSetting
            builder.Entity<InvoiceSetting>()
                .HasOne(ins => ins.Partition)
                .WithMany(p => p.InvoiceSettings)
                .HasForeignKey(ins => ins.PartitionId);

            // Partition --> Invoice
            builder.Entity<Invoice>()
                .HasOne(i => i.Partition)
                .WithMany(p => p.Invoices)
                .HasForeignKey(i => i.PartitionId);

            // Partition --> HourPriceSetting
            builder.Entity<HourPriceSetting>()
                .HasOne(rs => rs.Partition)
                .WithMany(p => p.HourPriceSettings)
                .HasForeignKey(rs => rs.PartitionId);

            // Group --> Task
            builder.Entity<Group>()
                .HasMany(g => g.Tasks)
                .WithOne(t => t.Group)
                .HasForeignKey(t => t.GroupId);
            // Group --> Material
            builder.Entity<Group>()
                .HasMany(g => g.Materials)
                .WithOne(t => t.Group)
                .HasForeignKey(t => t.GroupId);

            // Task --> Items
            builder.Entity<Ttask>()
                .HasMany(t => t.Items)
                .WithOne(i => i.Task)
                .HasForeignKey(w => w.TaskId);        
            // Material --> Items
            builder.Entity<Material>()
                .HasMany(t => t.Items)
                .WithOne(i => i.Material)
                .HasForeignKey(w => w.MaterialId);

            // Task --> TimeDefaultTasks
            builder.Entity<Ttask>()
                .HasMany(t => t.Defaults)
                .WithOne(tdt => tdt.Task)
                .HasForeignKey(tdt => tdt.TaskId);
            // Material --> TimeDefaultMaterials
            builder.Entity<Material>()
                .HasMany(t => t.Defaults)
                .WithOne(tdt => tdt.Material)
                .HasForeignKey(tdt => tdt.MaterialId);

            // User --> TaskItems
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.TaskItems)
                .WithOne(w => w.User)
                .HasForeignKey(w => w.UserId);
            // User --> MaterialItems
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.MaterialItems)
                .WithOne(w => w.User)
                .HasForeignKey(w => w.UserId);

            // User --> Report
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Reports)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId);

            // User --> Invoice
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Invoices)
                .WithOne(i => i.User)
                .HasForeignKey(i => i.UserId);
        }
    }
}