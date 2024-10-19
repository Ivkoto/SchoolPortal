namespace SchoolPortal.Api.Models
{
    public record InstitutionModel(
        int InstitutionId,
        string Kind,
        string Director,
        string Websites,
        string Emails,
        string PhoneNumbers,
        string AddressOfActivity,
        string Area,
        string Municipality,
        string Region,
        string Settlement,
        string SettlementType,
        string Neighbourhood,
        string PostalCode,
        decimal GeoLatitude,
        decimal GeoLongitide,
        int ExternalId,
        bool IsActive,
        string EIK,
        string FullName,
        string ShortName,
        string PreparationType,
        string InstStatus,
        string FinancingType,
        string InstOwnership
     );
}
