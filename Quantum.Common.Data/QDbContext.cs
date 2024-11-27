using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.EntityFramework.Interfaces;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Quantum.Data.Entities;
using Quantum.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Quantum.Data
{
    public class QDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>

    //ApiAuthorizationDbContext<IdentityUser>
    {
        private IConfiguration _config = null;
        //private IMemoryCache _memCache = null;

        public QDbContext(DbContextOptions options, IConfiguration config
            //,
            //IMemoryCache memCache
            )
        //: 
        //base(options
        //   //, operationalStoreOptions
        //)
        {
            _config = config;
            //_memCache = memCache;
        }

        public QDbContext(DbContextOptions options) : base(options)
        {


        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            //if (_memCache != null)
            //{
            //    optionsBuilder.UseMemoryCache(_memCache);
            //}

            if (_config != null)
            {
                optionsBuilder.UseSqlServer(_config["Data:connectionString"],b => b.MigrationsAssembly("Quantum.Data.Migrations"));
            }
        }

        internal class EntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
        {
            public void Configure(EntityTypeBuilder<T> builder)
            {
                //builder.Property(p => p.ID);

                //builder.HasIndex(p => p.IsDeleted);
                //builder.HasIndex(p => p.CreatedDate);
            }
        }

        //public DbSet<Audit> Audits { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemTag> ItemTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<View> Views { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ReportedContent> ReportedContets { get; set; }
        public DbSet<ReportedContentReason> ReportedContentReasons { get; set; }
        public DbSet<File> Files { get; set; }

        public DbSet<FileDetails> FileDetails { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<FileType> FileTypes { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<CLR_Type> CLRTypes { get; set; }
        public DbSet<SaveSearchResults> SaveSearchResults { get; set; }
        public DbSet<SaveSearchTags> SaveSearchTags { get; set; }

        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.ApplyConfiguration(new FavouriteConfiguration());
            builder.ApplyConfiguration(new LikeConfiguration());
            //builder.ApplyConfiguration(new ItemConfiguration(builder));
            builder.ApplyConfiguration(new ItemConfiguration());
            builder.ApplyConfiguration(new EventConfiguration());
            builder.ApplyConfiguration(new UserNotificationConfiguration());
            builder.ApplyConfiguration(new FileDetailsConfiguration());


            base.OnModelCreating(builder);
        }

        //public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    //var auditEntries = OnBeforeSaveChanges();
        //    var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        //    await OnAfterSaveChanges(auditEntries);
        //    return result;
        //}

        //private List<AuditEntry> OnBeforeSaveChanges()
        //{
        //    ChangeTracker.DetectChanges();
        //    var auditEntries = new List<AuditEntry>();
        //    foreach (var entry in ChangeTracker.Entries())
        //    {
        //        if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
        //            continue;

        //        var auditEntry = new AuditEntry(entry)
        //        {
        //            TableName = entry.Metadata.Relational().TableName
        //        };

        //        auditEntries.Add(auditEntry);

        //        foreach (var property in entry.Properties)
        //        {
        //            if (property.IsTemporary)
        //            {
        //                // value will be generated by the database, get the value after saving
        //                auditEntry.TemporaryProperties.Add(property);
        //                continue;
        //            }

        //            string propertyName = property.Metadata.Name;
        //            if (property.Metadata.IsPrimaryKey())
        //            {
        //                auditEntry.KeyValues[propertyName] = property.CurrentValue;
        //                continue;
        //            }

        //            switch (entry.State)
        //            {
        //                case EntityState.Added:
        //                    auditEntry.NewValues[propertyName] = property.CurrentValue;
        //                    break;

        //                case EntityState.Deleted:
        //                    auditEntry.OldValues[propertyName] = property.OriginalValue;
        //                    break;

        //                case EntityState.Modified:
        //                    if (property.IsModified)
        //                    {
        //                        auditEntry.OldValues[propertyName] = property.OriginalValue;
        //                        auditEntry.NewValues[propertyName] = property.CurrentValue;
        //                    }
        //                    break;
        //            }
        //        }
        //    }

        //    // Save audit entities that have all the modifications
        //    foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
        //    {
        //        Audits.Add(auditEntry.ToAudit());
        //    }

        //    // keep a list of entries where the value of some properties are unknown at this step
        //    return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
        //}

        //private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        //{
        //    if (auditEntries == null || auditEntries.Count == 0)
        //    {
        //        return Task.CompletedTask;
        //    }

        //    foreach (var auditEntry in auditEntries)
        //    {
        //        // Get the final value of the temporary properties
        //        foreach (var prop in auditEntry.TemporaryProperties)
        //        {
        //            if (prop.Metadata.IsPrimaryKey())
        //            {
        //                auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
        //            }
        //            else
        //            {
        //                auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
        //            }
        //        }

        //        // Save the Audit entry
        //        Audits.Add(auditEntry.ToAudit());
        //    }

        //    return SaveChangesAsync();
        //}

    }
}
