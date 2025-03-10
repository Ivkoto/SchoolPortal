using System.Data;
using Dapper;
using SchoolPortal.Api.Extensions;
using SchoolPortal.Api.Models;

namespace SchoolPortal.Api.Repositories;

public interface IInstitutionRepository
{
    Task<InstitutionModel> GetInstitutionById(int institutionId);
    Task<List<ProfileModel>> GetInstitutionProfiles(int institutionId, int schoolYear, int? grade);
    Task<List<ExamResultModel>> GetInstitutionAverageSuccesses(int institutionId, int[] schoolYear, int grade);
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

    public async Task<List<ProfileModel>> GetInstitutionProfiles(int institutionId, int schoolYear, int? grade)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();
        var parameters = new DynamicParameters();

        parameters.Add("@InstitutionId", institutionId, DbType.Int32);
        parameters.Add("@SchoolYear", schoolYear, DbType.Int32);
        parameters.Add("@Grade", grade, DbType.Int32);

        return (await connection.QueryAsync<ProfileModel>(
                sql: "[Application].[usp_GetFilteredProfiles]",
                param: parameters,
                commandType: CommandType.StoredProcedure
        )).ToList();
    }

    public async Task<List<ExamResultModel>> GetInstitutionAverageSuccesses(int institutionId, int[] schoolYears, int grade)
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

        return (await connection.QueryAsync<ExamResultModel>(
                sql: "[Application].[usp_GetExamResults]",
                param: parameters,
                commandType: CommandType.StoredProcedure
        )).ToList();
    }
}
