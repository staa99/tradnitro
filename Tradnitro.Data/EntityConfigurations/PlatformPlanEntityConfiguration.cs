using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tradnitro.Data.Entities;


namespace Tradnitro.Data.EntityConfigurations
{
    public abstract class BaseEntityTypeConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public void Configure (EntityTypeBuilder<T> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(e => e.CreatedBy).HasMaxLength(50);
            builder.Property(e => e.ModifiedBy).HasMaxLength(50);
            DoConfigure(builder);
        }

        protected abstract void DoConfigure (EntityTypeBuilder<T> builder);
    }


    public class PlatformPlanEntityConfiguration: BaseEntityTypeConfiguration<PlatformPlan>
    {
        /// <inheritdoc />
        protected override void DoConfigure(EntityTypeBuilder<PlatformPlan> builder)
        {
            builder.ToTable("PlatformPlans");
            builder.Property(p => p.Name).HasMaxLength(50);
        }
    }


    public class CompanyPlatformPlanEntityConfiguration: BaseEntityTypeConfiguration<CompanyPlatformPlan>
    {
        /// <inheritdoc />
        protected override void DoConfigure(EntityTypeBuilder<CompanyPlatformPlan> builder)
        {
            builder.ToTable("CompanyPlatformPlans");
            builder.Property(p => p.Name).HasMaxLength(50);

            builder.HasOne(p => p.Company)
                   .WithMany()
                   .HasForeignKey(p => p.CompanyId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }


    public class CompanyEntityConfiguration: BaseEntityTypeConfiguration<Company>
    {
        /// <inheritdoc />
        protected override void DoConfigure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");
            builder.Property(p => p.FullName).HasMaxLength(100);
            builder.Property(p => p.UniqueName).HasMaxLength(50);

            builder.HasMany(c => c.Domains).WithOne().HasForeignKey(d => d.CompanyId).IsRequired();
        }
    }


    public class InvestmentPlanEntityConfiguration: BaseEntityTypeConfiguration<InvestmentPlan>
    {
        /// <inheritdoc />
        protected override void DoConfigure(EntityTypeBuilder<InvestmentPlan> builder)
        {
            builder.ToTable("InvestmentPlans");

            builder.HasMany(p => p.Fields)
                   .WithOne()
                   .HasForeignKey(p => p.PlanId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }


    public class FieldEntityConfiguration: BaseEntityTypeConfiguration<Field>
    {
        /// <inheritdoc />
        protected override void DoConfigure(EntityTypeBuilder<Field> builder)
        {
            builder.ToTable("Fields");

            builder.Property(f => f.Name).HasMaxLength(100);
            builder.Property(f => f.Formula).HasMaxLength(256);
            builder.Property(f => f.DefaultValue).HasMaxLength(256);

            // a field name is unique within a plan
            builder.HasIndex(f => new { f.PlanId, f.Name }).IsUnique();
        }
    }


    public class RecordEntityConfiguration: BaseEntityTypeConfiguration<Record>
    {
        /// <inheritdoc />
        protected override void DoConfigure(EntityTypeBuilder<Record> builder)
        {
            builder.ToTable("Records");

            builder.HasMany(p => p.Fields)
                   .WithOne()
                   .HasForeignKey(p => p.RecordId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }


    public class RecordFieldEntityConfiguration: BaseEntityTypeConfiguration<RecordField>
    {
        /// <inheritdoc />
        protected override void DoConfigure(EntityTypeBuilder<RecordField> builder)
        {
            builder.ToTable("RecordFields");

            builder.HasIndex(rf => new { rf.RecordId, rf.FieldId }).IsUnique();
            builder.Property(f => f.RawValue).HasMaxLength(256);

            builder.HasOne(p => p.Field)
                   .WithMany()
                   .HasForeignKey(p => p.FieldId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}