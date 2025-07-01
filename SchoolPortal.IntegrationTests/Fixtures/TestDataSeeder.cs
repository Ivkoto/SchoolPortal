
using Microsoft.Data.SqlClient;

namespace SchoolPortal.IntegrationTests.Fixtures;

public class TestDataSeeder
{
    private readonly string connectionString;

    public TestDataSeeder(DatabaseFixture dbFixture)
    {
        connectionString = dbFixture.ConnectionString;
    }

    public async Task<int> SeedSchoolYear(int schoolYear)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO [Application].[SchoolYear] (Year)
            VALUES (@SchoolYear);
            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

        command.Parameters.Clear();
        command.Parameters.AddWithValue("@SchoolYear", schoolYear);

        var result = await command.ExecuteScalarAsync() ?? throw new InvalidOperationException("Failed to insert school year.");
        return (int)result;
    }

    public async Task<int> SeedInstitution(int externalId, string fullName, string shortName)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText = @"
           INSERT INTO [Application].[Institution] (ExternalId, FullName, ShortName)
           VALUES (@ExternalId, @FullName, @ShortName);
           SELECT CAST(SCOPE_IDENTITY() as int);
       ";

        command.Parameters.Clear();
        command.Parameters.AddWithValue("@ExternalId", externalId);
        command.Parameters.AddWithValue("@FullName", fullName);
        command.Parameters.AddWithValue("@ShortName", shortName);

        var result = await command.ExecuteScalarAsync() ?? throw new InvalidOperationException("Failed to insert institution.");
        return (int)result;
    }

    public async Task<int> SeedSubInstitution(int institutionId, int addressId)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText = @"
           INSERT INTO [Application].[SubInstitution] (InstitutionId, AddressOfActivityId)
           VALUES (@InstitutionId, @AddressOfActivityId);
           SELECT CAST(SCOPE_IDENTITY() as int);
       ";

        command.Parameters.Clear();
        command.Parameters.AddWithValue("@InstitutionId", institutionId);
        command.Parameters.AddWithValue("@AddressOfActivityId", addressId);

        var result = await command.ExecuteScalarAsync() ?? throw new InvalidOperationException("Failed to insert sub-institution.");
        return (int)result;
    }

    public async Task<int> SeedProfile(string name, string type, int grade, int subInstitutionId)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO [Application].[Profile] (Name, Type, Grade, SubInstitutionId)
            VALUES (@Name, @Type, @Grade, @SubInstitutionId);
            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

        command.Parameters.Clear();
        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@Type", type);
        command.Parameters.AddWithValue("@Grade", grade);
        command.Parameters.AddWithValue("@SubInstitutionId", subInstitutionId);

        var result = await command.ExecuteScalarAsync() ?? throw new InvalidOperationException("Failed to insert profile.");
        return (int)result;
    }

    public async Task SeedProfileDetails(int externalId, int profileId, int schoolYearId)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO [Application].[ProfileDetails] (ExternalId, ProfileId, SchoolYearId)
            VALUES (@ExternalId, @ProfileId, @SchoolYearId)
        ";
        command.Parameters.Clear();
        command.Parameters.AddWithValue("@ExternalId", externalId);
        command.Parameters.AddWithValue("@ProfileId", profileId);
        command.Parameters.AddWithValue("@SchoolYearId", schoolYearId);

        var rowsAffected = await command.ExecuteNonQueryAsync();

        if (rowsAffected == 0)
        {
            throw new InvalidOperationException("Failed to insert profile details.");
        }
    }

    public async Task SeedExamResults(
        int candidateCount, decimal averageScore, int grade, int subjectAbbreviationId,
        int subInstitutionId, int schoolYearId, int examAbbreviationId)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO [Application].[ExamResult] (CandidateCount, AverageSuccess, Grade,
                SchoolYearId, SubjectAbbreviationId, SubInstitutionId, ExamAbbreviationId)
            VALUES (@CandidateCount, @AverageScore, @Grade, @SchoolYearId, @SubjectAbbreviationId,
                @SubInstitutionId, @ExamAbbreviationId)
        ";

        command.Parameters.Clear();
        command.Parameters.AddWithValue("@CandidateCount", candidateCount);
        command.Parameters.AddWithValue("@AverageScore", averageScore);
        command.Parameters.AddWithValue("@Grade", grade);
        command.Parameters.AddWithValue("@SchoolYearId", schoolYearId);
        command.Parameters.AddWithValue("@SubjectAbbreviationId", subjectAbbreviationId);
        command.Parameters.AddWithValue("@SubInstitutionId", subInstitutionId);
        command.Parameters.AddWithValue("@ExamAbbreviationId", examAbbreviationId);

        var rowsAffected = await command.ExecuteNonQueryAsync();

        if (rowsAffected == 0)
        {
            throw new InvalidOperationException("Failed to insert exam results.");
        }
    }

    public async Task<int> SeedExamAbbreviation(string abbreviation)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO [Application].[ExamAbbreviation] (Abbreviation)
            VALUES (@Abbreviation);
            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

        command.Parameters.Clear();
        command.Parameters.AddWithValue("@Abbreviation", abbreviation);

        var result = await command.ExecuteScalarAsync() ?? throw new InvalidOperationException("Failed to insert exam abbreviation.");
        return (int)result;
    }

    public async Task<int> SeedSubjectAbbreviation(string shortName)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO [Application].[SubjectAbbreviation] (ShortName)
            VALUES (@ShortName);
            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

        command.Parameters.Clear();
        command.Parameters.AddWithValue("@ShortName", shortName);

        var result = await command.ExecuteScalarAsync() ?? throw new InvalidOperationException("Failed to insert subject abbreviation.");
        return (int)result;
    }

    public async Task<int> SeedNeighbourhood(string settlement, string neighbourhood)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO [Application].[Address] (Settlement, Neighbourhood)
            VALUES (@Settlement, @Neighbourhood);
            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

        command.Parameters.Clear();
        command.Parameters.AddWithValue("@Settlement", settlement);
        command.Parameters.AddWithValue("@Neighbourhood", neighbourhood);

        var result = await command.ExecuteScalarAsync() ?? throw new InvalidOperationException("Failed to insert settlement & neighbourhoods.");
        return (int)result;
    }

    public async Task<int> SeedScience(string name, int externalId, int schoolYearId)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO [Application].[Science] (Name, ExternalId, SchoolYearId)
            VALUES (@Name, @ExternalId, @SchoolYearId);
            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

        command.Parameters.Clear();
        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@ExternalId", externalId);
        command.Parameters.AddWithValue("@SchoolYearId", schoolYearId);

        var result = await command.ExecuteScalarAsync() ?? throw new InvalidOperationException("Failed to insert science.");
        return (int)result;
    }

    public async Task<int> SeedProfessionalDirection(string name, int externalId, int scienceId)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO [Application].[ProfessionalDirection] (Name, ExternalId, ScienceId)
            VALUES (@Name, @ExternalId, @ScienceId);
            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

        command.Parameters.Clear();
        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@ExternalId", externalId);
        command.Parameters.AddWithValue("@ScienceId", scienceId);

        var result = await command.ExecuteScalarAsync() ?? throw new InvalidOperationException("Failed to insert professional direction.");
        return (int)result;
    }

    public async Task<int> SeedProfession(string name, int externalId, int professionalDirectionId)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO [Application].[Profession] (Name, ExternalId, ProfessionalDirectionId)
            VALUES (@Name, @ExternalId, @ProfessionalDirectionId);
            SELECT CAST(SCOPE_IDENTITY() as int);
        ";

        command.Parameters.Clear();
        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@ExternalId", externalId);
        command.Parameters.AddWithValue("@ProfessionalDirectionId", professionalDirectionId);

        var result = await command.ExecuteScalarAsync() ?? throw new InvalidOperationException("Failed to insert profession.");
        return (int)result;
    }

    public async Task<int> SeedSpecialty(string name, int externalId, bool isProfessional, int? ProfessionId = null)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();

        if (ProfessionId.HasValue)
        {
            command.CommandText = @"
                INSERT INTO [Application].[Specialty] (Name, ExternalId, IsProfessional, ProfessionId)
                VALUES (@Name, @ExternalId, @IsProfessional, @ProfessionId);
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            command.Parameters.Clear();
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@ExternalId", externalId);
            command.Parameters.AddWithValue("@IsProfessional", isProfessional);
            command.Parameters.AddWithValue("@ProfessionId", ProfessionId);

            var result = await command.ExecuteScalarAsync() ?? throw new InvalidOperationException("Failed to insert specialty.");
            return (int)result;
        }
        else
        {
            command.CommandText = @"
                INSERT INTO [Application].[Specialty] (Name, ExternalId, IsProfessional)
                VALUES (@Name, @ExternalId, @IsProfessional);
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            command.Parameters.Clear();
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@ExternalId", externalId);
            command.Parameters.AddWithValue("@IsProfessional", isProfessional);

            var result = await command.ExecuteScalarAsync() ?? throw new InvalidOperationException("Failed to insert specialty.");
            return (int)result;
        }        
    }
}

