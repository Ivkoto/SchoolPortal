using System.Data;
using Dapper;
using SchoolPortal.Api.Extensions;
using SchoolPortal.Api.Models;

namespace SchoolPortal.Api.Repositories;

public interface IProfileRepository
{
    Task<(IReadOnlyCollection<ProfileModel> Profiles, int TotalPages, int TotalProfiles)> GetFilteredProfiles(GetFilteredProfilesRequest filters);
    Task<ProfileModel> GetProfileById(int profileId);
    Task<IReadOnlyCollection<ScienceModel>> GetAllSciences(int schoolYear);
    Task<IReadOnlyCollection<ProfessionalDirectionModel>> GetProfessionalDirectionsByScienceId(int scienceId);
    Task<IReadOnlyCollection<ProfessionModel>> GetProfessionsByProfessionalDirectionId(int professionalDirectionId);
    Task<IReadOnlyCollection<SpecialtyModel>> GetSpecialtiesByProfessionId(int professionId);
    Task<IReadOnlyCollection<ExamStageScoresModel>> GetAllExamStageScores(int ProfileId, int SchoolYear);
}

public class ProfileRepository : IProfileRepository
{
    private readonly ILogger<ProfileRepository> logger;
    private readonly IDbConnectionFactory connectionFactory;

    public ProfileRepository(ILogger<ProfileRepository> logger, IDbConnectionFactory connectionFactory)
    {
        this.logger = logger;
        this.connectionFactory = connectionFactory;
    }

    public async Task<(IReadOnlyCollection<ProfileModel> Profiles, int TotalPages, int TotalProfiles)> GetFilteredProfiles(GetFilteredProfilesRequest filters)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();
        var parameters = new DynamicParameters();

        parameters.Add("@SchoolYear", filters.SchoolYear, DbType.Int32);
        parameters.Add("@Grade", filters.Grade, DbType.Int32);
        parameters.Add("@Settlement", filters.Settlement ?? (object)DBNull.Value, DbType.String);
        parameters.Add("@Neighbourhood", filters.Neighbourhood ?? (object)DBNull.Value, DbType.String);

        if (filters.GeoLocationFilter != null)
        {
            parameters.Add("@Latitude", filters.GeoLocationFilter.Latitude, DbType.Decimal);
            parameters.Add("@Longitude", filters.GeoLocationFilter.Longitude, DbType.Decimal);
            parameters.Add("@Radius", filters.GeoLocationFilter.Radius, DbType.Decimal);
        }

        if (filters.ProfileType is not null)
        {
            var isProfessional = filters.ProfileType.ToLower() == CustomEnums.ProfileTypes.Professional ? 1 : 0;
            parameters.Add("@IsProfessional", isProfessional, DbType.Int32);
        }
        else
        {
            parameters.Add("@IsProfessional", (object)DBNull.Value, DbType.Int32);
        }

        parameters.Add("@SpecialtyId", filters.SpecialtyId ?? (object)DBNull.Value, DbType.Int32);
        parameters.Add("@ProfessionId", filters.ProfessionId ?? (object)DBNull.Value, DbType.Int32);
        parameters.Add("@ProfessionalDirectionId", filters.ProfessionalDirectionId ?? (object)DBNull.Value, DbType.Int32);
        parameters.Add("@ScienceId", filters.ScienceId ?? (object)DBNull.Value, DbType.Int32);
        parameters.Add("@PageNumber", filters.PageNumber ?? (object)DBNull.Value, DbType.Int32);
        parameters.Add("@PageSize", filters.PageSize ?? (object)DBNull.Value, DbType.Int32);

        using (var result = await connection.QueryMultipleAsync(
               sql: "[Application].[usp_GetFilteredProfiles]",
               param: parameters,
               commandType: CommandType.StoredProcedure))
        {
            var profiles = (await result.ReadAsync<ProfileModel>()).ToList();

            int totalPages = await result.ReadFirstOrDefaultAsync<int>();

            int totalProfiles = await result.ReadFirstOrDefaultAsync<int>();

            return (Profiles: profiles, TotalPages: totalPages, totalProfiles);
        }
    }

    public async Task<ProfileModel> GetProfileById(int profileId)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();

        var profile = await connection.QuerySingleOrDefaultAsync<ProfileModel>(
                      sql: "[Application].[usp_GetProfileById]",
                      param: new { profileId },
                      commandType: CommandType.StoredProcedure);

        return profile ?? throw new KeyNotFoundException($"No Profile found with the ID {profileId}");
    }

    public async Task<IReadOnlyCollection<ScienceModel>> GetAllSciences(int schoolYear)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();

        return [.. (await connection.QueryAsync<ScienceModel>(
                sql: "[Application].[usp_GetAllSciences]",
                param: new { schoolYear },
                commandType: CommandType.StoredProcedure
        ))];
    }

    public async Task<IReadOnlyCollection<ProfessionalDirectionModel>> GetProfessionalDirectionsByScienceId(int scienceId)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();

        return [.. (await connection.QueryAsync<ProfessionalDirectionModel>(
               sql: "[Application].[usp_GetProfessionalDirectionsByScienceId]",
               param: new { ScienceId = scienceId},
               commandType: CommandType.StoredProcedure
        ))];
    }

    public async Task<IReadOnlyCollection<ProfessionModel>> GetProfessionsByProfessionalDirectionId(int professionalDirectionId)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();

        return [.. (await connection.QueryAsync<ProfessionModel>(
               sql: "[Application].[usp_GetProfessionsByProfessionalDirectionId]",
               param: new { ProfessionalDirectionId = professionalDirectionId},
               commandType: CommandType.StoredProcedure
        ))];
    }

    public async Task<IReadOnlyCollection<SpecialtyModel>> GetSpecialtiesByProfessionId(int professionId)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();

        var parameters = new DynamicParameters();

        parameters.Add("@IsProfessional", professionId > 0 ? 1 : 0, DbType.Int32);
        parameters.Add("@ProfessionId", professionId > 0 ? professionId : (object)DBNull.Value, DbType.Int32);

        return [.. (await connection.QueryAsync<SpecialtyModel>(
                sql: "[Application].[usp_GetSpecialtiesByProfessionId]",
                param: parameters,
                commandType: CommandType.StoredProcedure
        ))];
    }

    public async Task<IReadOnlyCollection<ExamStageScoresModel>> GetAllExamStageScores(int profileId, int schoolYear)
    {
        await using var connection = await connectionFactory.CreateConnectionAsync();

        return [.. (await connection.QueryAsync<ExamStageScoresModel> (
               sql: "[Application].[usp_GetExamStageScoresByProfileId]",
               param: new { ProfileId = profileId, SchoolYear = schoolYear },
               commandType: CommandType.StoredProcedure
        ))];
    }
}
