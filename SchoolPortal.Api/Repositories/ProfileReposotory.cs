using System.Data;
using Microsoft.Data.SqlClient;
using SchoolPortal.Api.Models;

namespace SchoolPortal.Api.Repositories
{
    public interface IProfileRepository
    {
        Task<IEnumerable<ProfileModel>> GetFilteredProfiles(ProfileFilterModel filters, CancellationToken cancellationToken);
    }
    public class ProfileRepository : IProfileRepository
    {
        private readonly IConfiguration configuration;
        private readonly Serilog.ILogger logger;

        public ProfileRepository(IConfiguration configuration, Serilog.ILogger logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }
        public async Task<IEnumerable<ProfileModel>> GetFilteredProfiles(ProfileFilterModel filters, CancellationToken cancellationToken)
        {
            var connectionString = configuration.GetConnectionString("DatabaseConnection");
            var profiles = new List<ProfileModel>();

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            using var command = new SqlCommand("[Application].[GetFilteredProfiles]", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@SchoolYear", filters.SchoolYear ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Grade", filters.Grade ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@SpecialtyId", filters.SpecialtyId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ProfessionId", filters.ProfessionId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ProfessionalDirectionId", filters.ProfessionalDirectionId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ScienceId", filters.ScienceId ?? (object)DBNull.Value);

            using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                try
                {
                    var profile = ProfileReader(reader);
                    profiles.Add(profile);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }

            }

            if (filters.SchoolYear.HasValue && filters.SchoolYear.Value > 0)
                profiles = profiles.Where(p => p.SchoolYear == filters.SchoolYear.Value).ToList();

            if (filters.Grade.HasValue && filters.Grade.Value > 0)
                profiles = profiles.Where(p => p.Grade == filters.Grade.Value).ToList();

            if (filters.SpecialtyId.HasValue && filters.SpecialtyId > 0)
                profiles = profiles.Where(p => p.SpecialtyId == filters.SpecialtyId).ToList();

            if (filters.ProfessionId.HasValue && filters.ProfessionId > 0)
                profiles = profiles.Where(p => p.ProfessionId == filters.ProfessionId).ToList();

            if (filters.ProfessionalDirectionId.HasValue && filters.ProfessionalDirectionId > 0)
                profiles = profiles.Where(p => p.ProfessionalDirectionId == filters.ProfessionalDirectionId).ToList();

            if (filters.ScienceId.HasValue && filters.ScienceId > 0)
                profiles = profiles.Where(p => p.ScienceId == filters.ScienceId).ToList();

            return profiles;
        }

        private static ProfileModel ProfileReader(SqlDataReader reader)
        {

            return new ProfileModel
            {
                ProfileId = Convert.ToInt32(reader["ProfileId"]),
                ProfileName = Convert.ToString(reader["ProfileName"]),
                Type = ConvertToNullableString(reader["Type"]),
                Grade = GetNullableInt32(reader["Grade"]),
                StudyPeriod = ConvertToNullableString(reader["StudyPeriod"]),
                InstitutionId = GetNullableInt32(reader["InstitutionId"]),
                GradingFormulas = ConvertToNullableString(reader["GradingFormulas"]),
                StudyMethod = ConvertToNullableString(reader["StudyMethod"]),
                EducationType = ConvertToNullableString(reader["EducationType"]),
                ClassesCount = ConvertToNullableDecimal(reader["ClassesCount"]),
                FirstForeignLanguage = ConvertToNullableString(reader["FirstForeignLanguage"]),
                SchoolYear = GetNullableInt32(reader["SchoolYear"]),
                IsPaperOnly = ConvertToBoolean(reader["IsPaperOnly"]),
                ExternalId = GetNullableInt32(reader["ExternalId"]),
                QuotasTotal = GetNullableInt32(reader["QuotasTotal"]),
                QuotasMale = GetNullableInt32(reader["QuotasMale"]),
                QuotasFemale = GetNullableInt32(reader["QuotasFemale"]),
                SpecialtyId = GetNullableInt32(reader["SpecialtyId"]),
                Specialty = ConvertToNullableString(reader["Specialty"]),
                SpecialtyExternalId = GetNullableInt32(reader["SpecialtyExternalId"]),
                ProfessionalQualificationLevel = GetNullableInt32(reader["ProfessionalQualificationLevel"]),
                IsProtected = ConvertToBoolean(reader["IsProtected"]),
                HasExpectedShortage = ConvertToBoolean(reader["HasExpectedShortage"]),
                SpecialtyDescription = ConvertToNullableString(reader["SpecialtyDescription"]),
                ProfessionId = GetNullableInt32(reader["ProfessionId"]),
                Profession = ConvertToNullableString(reader["Profession"]),
                ProfessionalDirectionId = GetNullableInt32(reader["ProfessionalDirectionId"]),
                ProfessionExternalId = GetNullableInt32(reader["ProfessionExternalId"]),
                ProfessionalDirection = ConvertToNullableString(reader["ProfessionalDirection"]),
                ProfessionalDirectionExternalId = GetNullableInt32(reader["ProfessionalDirectionExternalId"]),
                ScienceId = GetNullableInt32(reader["ScienceId"]),
                Science = ConvertToNullableString(reader["Science"]),
                ScienceExternalId = GetNullableInt32(reader["ScienceExternalId"])
            };
        }

        private static string? ConvertToNullableString(object readerResult)
        {
            if (readerResult == DBNull.Value)
                return null;

            return Convert.ToString(readerResult);
        }

        private static bool ConvertToBoolean(object readerResult)
        {
            if (readerResult == DBNull.Value)
                return false;

            return Convert.ToInt32(readerResult) == 1;
        }

        private static decimal? ConvertToNullableDecimal(object readerResult)
        {
            if (readerResult == DBNull.Value)
                return null;

            return Convert.ToDecimal(readerResult);
        }

        private static int? GetNullableInt32(object dbValue)
        {
            if (dbValue == DBNull.Value || dbValue == null)
                return null;

            return Convert.ToInt32(dbValue);
        }
    }
}
