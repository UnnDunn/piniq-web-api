using Microsoft.EntityFrameworkCore;
using Pinball.Api.Data.Entities;

namespace Pinball.Api.Data
{
    public class PinballDbContext : DbContext
    {
        private const string SqlCaseInsensitiveCollationName = "SQL_Latin1_General_CP1_CS_AS";

        private string _caseInsensitiveCollationName = SqlCaseInsensitiveCollationName;
        public DbSet<OpdbCatalogSnapshot> CatalogSnapshots { get; set; } = null!;
        public DbSet<OpdbChangelog> OpdbChangelogs { get; set; } = null!;
        public DbSet<PinballMachine> PinballMachines { get; set; } = null!;
        public DbSet<PinballMachineFeature> PinballFeatures { get; set; } = null!;
        public DbSet<PinballMachineType> PinballTypes { get; set; } = null!;
        public DbSet<PinballMachineManufacturer> PinballManufacturers { get; set; } = null!;
        public DbSet<PinballMachineKeyword> PinballKeywords { get; set; } = null!;
        public DbSet<PinballMachineGroup> PinballMachineGroups { get; set; } = null!;

        public PinballDbContext(DbContextOptions<PinballDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // pinball machine
            modelBuilder.Entity<PinballMachine>().HasKey(pm => pm.Id);
            modelBuilder.Entity<PinballMachine>().HasMany(pm => pm.KeywordMappings).WithOne(km => km.Machine).HasForeignKey(km => km.MachineId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PinballMachine>().HasMany(pm => pm.FeatureMappings).WithOne(fm => fm.Machine).HasForeignKey(fm => fm.MachineId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PinballMachine>().HasOne(pm => pm.Manufacturer).WithMany(pm => pm.Machines).HasForeignKey(pm => pm.ManufacturerId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<PinballMachine>().HasOne(pm => pm.Type).WithMany().HasForeignKey(pm => pm.TypeId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PinballMachine>().Ignore(pm => pm.Keywords);
            modelBuilder.Entity<PinballMachine>().Ignore(pm => pm.Features);
            modelBuilder.Entity<PinballMachine>().HasOne(pm => pm.MachineGroup).WithMany(mg => mg.Machines).HasForeignKey(pm => pm.MachineGroupId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<PinballMachine>().Property(pm => pm.Id).UseCollation(_caseInsensitiveCollationName);
            modelBuilder.Entity<PinballMachine>().Property(pm => pm.MachineGroupId).UseCollation(_caseInsensitiveCollationName);

            // pinball features
            modelBuilder.Entity<PinballMachineFeatureMapping>().HasKey(pmfm => new { pmfm.FeatureId, pmfm.MachineId });
            modelBuilder.Entity<PinballMachineFeatureMapping>().Property(pmfm => pmfm.MachineId).UseCollation(_caseInsensitiveCollationName);
            modelBuilder.Entity<PinballMachineFeature>().HasKey(pmf => pmf.Id);
            modelBuilder.Entity<PinballMachineFeature>().HasMany(pmf => pmf.Mappings).WithOne(pmf => pmf.Feature).HasForeignKey(pmf => pmf.FeatureId).OnDelete(DeleteBehavior.Cascade);

            // pinball keywords
            modelBuilder.Entity<PinballMachineKeywordMapping>().HasKey(pmkm => new { pmkm.KeywordId, pmkm.MachineId });
            modelBuilder.Entity<PinballMachineKeywordMapping>().HasIndex(pmkm => pmkm.KeywordId);
            modelBuilder.Entity<PinballMachineKeywordMapping>().Property(pmkm => pmkm.MachineId).UseCollation(_caseInsensitiveCollationName);
            modelBuilder.Entity<PinballMachineKeyword>().HasKey(pmf => pmf.Id);
            modelBuilder.Entity<PinballMachineKeyword>().HasMany(pk => pk.Mappings).WithOne(pk => pk.Keyword).HasForeignKey(pk => pk.KeywordId).OnDelete(DeleteBehavior.Cascade);

            // machine types
            modelBuilder.Entity<PinballMachineType>().HasData(
                new PinballMachineType { Id = "ss", Name = "Solid-State" },
                new PinballMachineType { Id = "em", Name = "Electro-Mechanical" },
                new PinballMachineType { Id = "me", Name = "Mechanical" },
                new PinballMachineType { Id = "dmd", Name = "Dot-Matrix Display" }
                );

            // manufacturer
            modelBuilder.Entity<PinballMachineManufacturer>().Property(pmm => pmm.Id).ValueGeneratedNever();

            // machine groups
            modelBuilder.Entity<PinballMachineGroup>().Property(g => g.Id).UseCollation(_caseInsensitiveCollationName);
        }
    }
}
