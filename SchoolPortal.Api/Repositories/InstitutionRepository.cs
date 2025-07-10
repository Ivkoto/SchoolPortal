using System.Data;
using Dapper;
using SchoolPortal.Api.Extensions;
using SchoolPortal.Api.Models;

namespace SchoolPortal.Api.Repositories;

public interface IInstitutionRepository
{
    Task<InstitutionModel> GetInstitutionById(int institutionId);
    Task<IReadOnlyCollection<ProfileModel>> GetInstitutionProfiles(int institutionId, int schoolYear, int? grade);
    Task<IReadOnlyCollection<ExamResultModel>> GetInstitutionAverageSuccesses(int institutionId, int[] schoolYears, int grade);
}

public class InstitutionRepository : IInstitutionRepository
{
    private readonly IDbConnectionFactory connectionFactory;

    public InstitutionRepository(IDbConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task<InstitutionModel> GetInstitutionById(int institutionId)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();

        var institution = await connection.QuerySingleOrDefaultAsync<InstitutionModel>(
                          sql: "[Application].[usp_GetInstitutionById]",
                          param: new { institutionId },
                          commandType: CommandType.StoredProcedure);

        return institution ?? throw new KeyNotFoundException($"No Institution found with the ID {institutionId}");
    }

    public async Task<IReadOnlyCollection<ProfileModel>> GetInstitutionProfiles(int institutionId, int schoolYear, int? grade)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();
        var parameters = new DynamicParameters();

        parameters.Add("@SchoolYear", schoolYear, DbType.Int32);
        parameters.Add("@Grade", grade, DbType.Int32);
        parameters.Add("@Settlement", (object)DBNull.Value, DbType.String);
        parameters.Add("@Neighbourhood", (object)DBNull.Value, DbType.String);
        parameters.Add("@Latitude", (object)DBNull.Value, DbType.Decimal);
        parameters.Add("@Longitude", (object)DBNull.Value, DbType.Decimal);
        parameters.Add("@Radius", (object)DBNull.Value, DbType.Decimal);
        parameters.Add("@IsProfessional", (object)DBNull.Value, DbType.Int32);
        parameters.Add("@SpecialtyId", (object)DBNull.Value, DbType.Int32);
        parameters.Add("@ProfessionId", (object)DBNull.Value, DbType.Int32);
        parameters.Add("@ProfessionalDirectionId", (object)DBNull.Value, DbType.Int32);
        parameters.Add("@ScienceId", (object)DBNull.Value, DbType.Int32);
        parameters.Add("@InstitutionId", institutionId, DbType.Int32);
        parameters.Add("@PageNumber", (object)DBNull.Value, DbType.Int32);
        parameters.Add("@PageSize", (object)DBNull.Value, DbType.Int32);

        using var result = await connection.QueryMultipleAsync(
                sql: "[Application].[usp_GetFilteredProfiles]",
                param: parameters,
                commandType: CommandType.StoredProcedure);

        var profiles = (await result.ReadAsync<ProfileModel>()).ToList();

        // Read and consume the remaining result sets to avoid connection issues & to keep the connection clean
        await result.ReadFirstOrDefaultAsync<int>(); // Read TotalPages (2nd result set)
        await result.ReadFirstOrDefaultAsync<int>(); // Read TotalProfiles (3rd result set)

        return profiles;
    }

    public async Task<IReadOnlyCollection<ExamResultModel>> GetInstitutionAverageSuccesses(int institutionId, int[] schoolYears, int grade)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();
        var parameters = new DynamicParameters();

        var schoolYearsTable = new DataTable();
        schoolYearsTable.Columns.Add("Year", typeof(int));

        foreach (var year in schoolYears)
        {
            schoolYearsTable.Rows.Add(year);
        }

        parameters.Add("@InstitutionId", institutionId, DbType.Int32);
        parameters.Add("@SchoolYears", schoolYearsTable.AsTableValuedParameter("Application.SchoolYearsList"));
        parameters.Add("@Grade", grade, DbType.Int32);

        return [.. (await connection.QueryAsync<ExamResultModel>(
                sql: "[Application].[usp_GetExamResults]",
                param: parameters,
                commandType: CommandType.StoredProcedure
        ))];
    }
}
