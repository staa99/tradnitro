using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tradnitro.Data.Entities;


namespace Tradnitro.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext()
        { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        private static LambdaExpression GetIsDeletedRestriction(Type type)
        {
            var parameterExpression = Expression.Parameter(type, "softDeletable");
            var prop = Expression.Property(parameterExpression,
                                       type.GetProperty(nameof(ISoftDeletable.IsDeleted))!);
            var condition = Expression.Equal(prop, Expression.Constant(false));
            var lambda = Expression.Lambda(condition,
                                           parameterExpression);
            return lambda;
        }

        private void ConfigureDeletableEntities(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entity.ClrType))
                {
                    modelBuilder
                       .Entity(entity.ClrType)
                       .HasQueryFilter(GetIsDeletedRestriction(entity.ClrType));
                }
            }
        }

        public override int SaveChanges()
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool              acceptAllChangesOnSuccess,
                                                   CancellationToken cancellationToken = default)
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess,
                                         cancellationToken);
        }

        private void UpdateSoftDeleteStatuses()
        {
            foreach (var entry in ChangeTracker.Entries().Where(e => e.Entity.GetType().IsAssignableTo(typeof(ISoftDeletable))))
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.CurrentValues[nameof(ISoftDeletable.IsDeleted)] = false;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues[nameof(ISoftDeletable.IsDeleted)] = true;
                        break;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // use all configurations in this assembly
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            ConfigureDeletableEntities(builder);
        }

        public virtual DbSet<PlatformPlan> PlatformPlans { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CompanyPlatformPlan> CompanyPlatformPlans { get; set; }
        public virtual DbSet<CompanyDomain> CompanyDomains { get; set; }
        public virtual DbSet<InvestmentPlan> InvestmentPlans { get; set; }
        public virtual DbSet<Field> Fields { get; set; }
        public virtual DbSet<Record> Records { get; set; }
        public virtual DbSet<RecordField> RecordFields { get; set; }
        public virtual DbSet<Config> Configs { get; set; }
    }
}