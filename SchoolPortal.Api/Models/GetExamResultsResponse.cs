namespace SchoolPortal.Api.Models;

public record GetExamResultsResponse
{
    public int ExamResultsCount { get; set; } = 0;
    public IReadOnlyCollection<ExamResultModel> ExamResults { get; set; } = [];
}

public record ExamResultModel
(
    int SchoolYear,
    int InstitutionId,
    string? ExamAbbreviation,
    string? SubjectName,
    string? SubjectAbbreviation,
    string? PreparationType,
    int? CandidateCount,
    decimal? AverageScore,
    int Grade,
    string? LevelType
);
