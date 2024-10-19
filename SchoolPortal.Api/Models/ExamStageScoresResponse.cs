namespace SchoolPortal.Api.Models
{
    public record ExamStageScoresResponse
    {
        public int StagesCount { get; set; } = 0;
        public List<ExamStageScoresModel> ExamStageScores { get; set; } = [];
    }

    public record ExamStageScoresModel
    (
        int StageId,
        int StageNumber,
        int FreePositionsTotal,
        int FreePositionsMen,
        int FreePositionsWomen,
        bool IsAggregatedScore,
        int ProfileId,
        string ProfileName,
        int SchoolYear,
        decimal MinTotalScore,
        decimal MinMaleScore,
        decimal MinFemaleScore,
        decimal MaxTotalScore,
        decimal MaxMaleScore,
        decimal MaxFemaleScore
    );
}
