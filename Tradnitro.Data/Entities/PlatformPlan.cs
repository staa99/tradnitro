using System;
using System.Collections.Generic;
using Tradnitro.Shared.Enums;


namespace Tradnitro.Data.Entities
{
    public abstract class BaseEntity : ISoftDeletable, IModificationAuditedEntity, ICreationAuditedEntity
    {
        public virtual int Id { get; set; }

        public virtual DateTime UtcDateCreated { get; set; }

        public virtual DateTime? UtcDateModified { get; set; }

        public virtual string CreatedBy { get; set; } = null!;

        public virtual string? ModifiedBy { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }

    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
    }

    public interface IModificationAuditedEntity
    {
        DateTime? UtcDateModified { get; set; }

        string? ModifiedBy { get; set; }
    }

    public interface ICreationAuditedEntity
    {
        DateTime UtcDateCreated { get; set; }

        string CreatedBy { get; set; }
    }


    public interface IOwnedByCompanyEntity
    {
        int CompanyId { get; set; }
        Company? Company { get; set; }
    }
    public class PlatformPlan: BaseEntity
    {
        public virtual string Name { get; set; } = null!;
        public virtual int MOUGenerationLimit { get; set; }
        public virtual int AdminUserLimit { get; set; }
        public decimal Price { get; set; }
        public virtual int ValidityInDays { get; set; }
    }


    public class Company: BaseEntity
    {
        public virtual string FullName { get; set; } = null!;
        public virtual string UniqueName { get; set; } = null!;
        public ICollection<CompanyDomain> Domains { get; set; } = new HashSet<CompanyDomain>();
    }


    public class CompanyPlatformPlan: BaseEntity, IOwnedByCompanyEntity
    {
        public virtual string Name { get; set; } = null!;
        public virtual int MOUGenerationLimit { get; set; }
        public virtual int AdminUserLimit { get; set; }
        public decimal Price { get; set; }
        public virtual int ValidityInDays { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        public virtual int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
    }


    public class CompanyDomain: BaseEntity, IOwnedByCompanyEntity
    {
        public virtual string DomainName { get; set; } = null!;
        
        public virtual int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
    }


    public class InvestmentPlan: BaseEntity, IOwnedByCompanyEntity
    {
        public virtual decimal ROIPercent { get; set; }
        public virtual int NumberOfDays { get; set; }
        public virtual bool WorkingDaysOnly { get; set; }
        public virtual ICollection<Field> Fields { get; set; } = new HashSet<Field>();
        public virtual string MOUFileName => $"{Id}.tv";

        public virtual int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
    }

    public class Field: BaseEntity, IOwnedByCompanyEntity
    {
        public virtual string Name { get; set; } = null!;
        public virtual bool Generated { get; set; }
        public virtual string? Formula { get; set; }
        public virtual string? DefaultValue { get; set; }
        public virtual FieldType FieldType { get; set; }
        
        public virtual int PlanId { get; set; }
        public virtual InvestmentPlan? Plan { get; set; }

        public virtual int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
    }


    public class Record: BaseEntity, IOwnedByCompanyEntity
    {
        public virtual string UniqueReference { get; set; } = null!;
        public virtual ICollection<RecordField> Fields { get; set; } = new HashSet<RecordField>();
        
        public virtual int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
    }


    public class RecordField: BaseEntity
    {
        public virtual int RecordId { get; set; }
        public virtual string RawValue { get; set; } = null!;

        public virtual int FieldId { get; set; }
        public virtual Field? Field { get; set; }
    }

    public class Config : BaseEntity
    {
        public ConfigName Name { get; set; }
        public virtual string Description { get; set; } = null!;
        public virtual string Value { get; set; } = null!;
        
        public virtual int? CompanyId { get; set; }
    }
}