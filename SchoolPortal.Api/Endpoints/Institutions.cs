using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SchoolPortal.Api.Models;
using SchoolPortal.Api.Repositories;
using static SchoolPortal.Api.Validation.ProfileValidator;

namespace SchoolPortal.Api.Endpoints;

public class Institutions : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapGet("/api/v1/institutions/{institutionId:int}", GetInstitutionById)
            .WithName("GetInstitutionById")
            .Produces<InstitutionModel>(StatusCodes.Status200OK)
            .RequireCors("AllowedOriginsPolicy");

        app.MapGet("/api/v1/institutions/{institutionId:int}/profiles", GetInstitutionProfiles)
            .WithName("GetInstitutionProfiles")
            .Produces<GetFilteredProfilesResponse>(StatusCodes.Status200OK)
            .RequireCors("AllowedOriginsPolicy");

        app.MapGet("/api/v1/institutions/{institutionId:int}/average-successes", GetInstitutionAverageSuccesses)
            .WithName("GetInstitutionAverageSuccesses")
            .Produces<GetExamResultsResponse>(StatusCodes.Status200OK)
            .RequireCors("AllowedOriginsPolicy");
    }

    public void MapServices(IServiceCollection services)
    {
        services.TryAddScoped<IInstitutionRepository, InstitutionRepository>();
    }

    internal async Task<IResult> GetInstitutionById(
        int institutionId,
        HttpContext httpContext,
        [FromServices] IInstitutionRepository institutionRepository)
    {
        httpContext.Response.Headers["Deprecated"] = "False";

        var currentInstitution = await institutionRepository.GetInstitutionById(institutionId);

        return Results.Ok(currentInstitution);
    }

    internal async Task<IResult> GetInstitutionProfiles(
        int institutionId,
        HttpContext httpContext,
        [FromQuery] int schoolYear,
        [FromQuery] int grade,
        [FromServices] IInstitutionRepository institutionRepository)
    {
        httpContext.Response.Headers["Deprecated"] = "False";

        var schoolYearValidator = new SchoolYearValidator();
        var gradeValidator = new GradeValidator();

        var schoolYearValidationResult = schoolYearValidator.Validate(schoolYear);
        var gradeValidationResult = gradeValidator.Validate(grade);

        if (!schoolYearValidationResult.IsValid || !gradeValidationResult.IsValid)
        {
            var allErrors = schoolYearValidationResult.Errors.Concat(gradeValidationResult.Errors).ToList();
            throw new ValidationException(allErrors);
        }

        var profiles = await institutionRepository.GetInstitutionProfiles(institutionId, schoolYear, grade);

        return Results.Ok(
            new GetFilteredProfilesResponse
            {
                ProfilesCount = profiles.Count,
                Profiles = profiles
            }
        );
    }

    internal async Task<IResult> GetInstitutionAverageSuccesses(
        int institutionId,
        HttpContext httpContext,
        [FromQuery] int[] schoolYear,
        [FromQuery] int? grade,
        [FromServices] IInstitutionRepository institutionRepository)
    {
        httpContext.Response.Headers["Deprecated"] = "False";

        var schoolYearValidator = new SchoolYearValidator();
        var gradeValidator = new GradeValidator();

        var validationResults = new FluentValidation.Results.ValidationResult();

        // Validate that schoolYear array is not empty or null
        if (schoolYear == null || schoolYear.Length == 0)
        {
            validationResults.Errors.Add(new FluentValidation.Results.ValidationFailure(
                "schoolYear", "At least one school year must be provided."));
        }
        else
        {
            // Validate all school years using the same validator instance
            foreach (var year in schoolYear)
            {
                var yearValidationResult = schoolYearValidator.Validate(year);
                validationResults.Errors.AddRange(yearValidationResult.Errors);
            }
        }

        if (grade is not null)
        {
            var gradeValidationResult = gradeValidator.Validate(grade.Value);
            validationResults.Errors.AddRange(gradeValidationResult.Errors);
        }

        if (!validationResults.IsValid)
        {
            throw new ValidationException(validationResults.Errors);
        }

        var examResults = await institutionRepository.GetInstitutionAverageSuccesses(institutionId, schoolYear, grade);

        return Results.Ok(
            new GetExamResultsResponse
            {
                ExamResultsCount = examResults.Count,
                ExamResults = examResults
            }
        );
    }
}
