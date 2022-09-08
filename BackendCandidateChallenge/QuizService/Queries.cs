namespace QuizService
{
    public static class Queries
    {
        public const string SelectAllQuizzes = "SELECT * FROM Quiz;";
        public const string SelectAllQuizzesById = "SELECT * FROM Quiz WHERE Id = @Id;";
        public const string SelectAllQuestionsById = "SELECT * FROM Question WHERE QuizId = @QuizId;";
        public const string SelectAnswerByQuizId = "SELECT a.Id, a.Text, a.QuestionId FROM Answer a INNER JOIN Question q ON a.QuestionId = q.Id WHERE q.QuizId = @QuizId;";
    }
}
