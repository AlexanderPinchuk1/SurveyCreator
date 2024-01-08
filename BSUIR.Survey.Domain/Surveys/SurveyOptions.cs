namespace BSUIR.Survey.Domain.Surveys
{
    [Flags]
    public enum SurveyOptions
    {
        Anonymity = 1,
        RandomOrderOfQuestions = 2,
        NumberedQuestions = 4,
        ProgressBar = 8
    }
}
