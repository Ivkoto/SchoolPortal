CREATE OR ALTER PROC [Application].[usp_GetInstitutionById]
	@InstitutionId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		InstitutionId,
		Kind,
		Director,
		Websites,
		Emails,
		PhoneNumbers,
		AddressOfActivity,
		Area,
		Municipality,
		Region,
		Settlement,
		SettlementType,
		Neighbourhood,
		PostalCode,
		GeoLatitude,
		GeoLongitide,
		ExternalId,
		IsActive,
		EIK,
		FullName,
		ShortName,
		PreparationType,
		InstStatus,
		FinancingType,
		InstOwnership
	FROM
		[Application].[uv_InstitutionsDetails]
	WHERE
		[InstitutionId] = @InstitutionId
END;
GO
