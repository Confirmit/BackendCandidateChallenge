
using System.Data;
using Dapper;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using QuizService.Model.Domain;
using QuizService.Model;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;

namespace QuizService
{
    public class QuizService : IQuizService
    {

        private readonly IDbConnection _connection;

        public QuizService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<QuizResponseModel>> GetAllAsync()
        {
            var quizs = _connection.Query<Quiz>(Queries.SelectAllQuizzes);
            return quizs.Select(quiz =>
                new QuizResponseModel
                {
                    Id = quiz.Id,
                    Title = quiz.Title
                }).ToList();
        }

        public async Task<QuizResponseModel> GetAsyncById(int id)
        {
            try
            {
                var quiz = _connection.QuerySingleOrDefault<Quiz>(Queries.SelectAllQuizzesById, new { Id = id });
                var questions = _connection.Query<Question>(Queries.SelectAllQuestionsById, new { QuizId = id });
                var answers = _connection.Query<Answer>(Queries.SelectAnswerByQuizId, new { QuizId = id })
                    .Aggregate(new Dictionary<int, IList<Answer>>(), (dict, answer) =>
                    {
                        if (!dict.ContainsKey(answer.QuestionId))
                            dict.Add(answer.QuestionId, new List<Answer>());
                        dict[answer.QuestionId].Add(answer);
                        return dict;
                    });
                return new QuizResponseModel
                {
                    Id = quiz.Id,
                    Title = quiz.Title,
                    Questions = questions.Select(question => new QuizResponseModel.QuestionItem
                    {
                        Id = question.Id,
                        Text = question.Text,
                        Answers = answers.ContainsKey(question.Id)
                            ? answers[question.Id].Select(answer => new QuizResponseModel.AnswerItem
                            {
                                Id = answer.Id,
                                Text = answer.Text
                            })
                            : Array.Empty<QuizResponseModel.AnswerItem>(),
                        CorrectAnswerId = question.CorrectAnswerId
                    }),
                    Links = new Dictionary<string, string>
            {
                {"self", $"/api/quizzes/{id}"},
                {"questions", $"/api/quizzes/{id}/questions"}
            }
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
