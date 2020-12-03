using System.Collections.Generic;


namespace Tradnitro.Data.Entities
{
	public class PlatformPlan
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int MOUGenerationLimit { get; set; }
		public int AdminUserLimit { get; set; }
		public decimal Price { get; set; }
		public int ValidityInDays { get; set; }
	}


	public class Company
	{
		public int Id { get; set; }
		public string UniqueName { get; set; }
		public ICollection<CompanyDomain> Domains { get; set; }
	}


	public class CompanyPlatformPlan
	{
		public string Name { get; set; }
		public int MOUGenerationLimit { get; set; }
		public int AdminUserLimit { get; set; }
		public decimal Price { get; set; }
		public int ValidityInDays { get; set; }
	}


	public class CompanyDomain
	{
		public int CompanyId { get; set; }
		public string DomainName { get; set; }
	}
}
