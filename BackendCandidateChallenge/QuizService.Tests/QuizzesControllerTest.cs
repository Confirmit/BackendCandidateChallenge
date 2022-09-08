using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using QuizService.Model;
using Xunit;

namespace QuizService.Tests;

public class QuizzesControllerTest
{
    const string QuizApiEndPoint = "/api/quizzes/";

    [Fact]
    public async Task PostNewQuizAddsQuiz()
    {
        var quiz = new QuizCreateModel("Test title");
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(quiz));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"),
                content);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
        }
    }

    [Fact]
    public async Task AQuizExistGetReturnsQuiz()
    {
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 1;
            var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content);
            var quiz = JsonConvert.DeserializeObject<QuizResponseModel>(await response.Content.ReadAsStringAsync());
            Assert.Equal(quizId, quiz.Id);
            Assert.Equal("My first quiz", quiz.Title);
        }
    }

    [Fact]
    public async Task AQuizDoesNotExistGetFails()
    {
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 999;
            var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Fact]

    public async Task AQuizDoesNotExists_WhenPostingAQuestion_ReturnsNotFound()
    {
        const string QuizApiEndPoint = "/api/quizzes/999/questions";

        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 999;
            var question = new QuestionCreateModel("The answer to everything is what?");
            var content = new StringContent(JsonConvert.SerializeObject(question));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"), content);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    /// <summary>
    /// This Test method created to test functionality to generate a single Quiz with Id=3,
    /// with creating two question and ,
    /// each question having two answer ,
    ///  updating Question by adding CorrectAnswer by Put method.
    /// </summary>
    /// <returns></returns>

    [Fact]
    public async Task CreateQuizWithTwoQuestionsAndTwoAnswersEach()
    {
        const string QuizApiEndPoint = "/api/quizzes";
        const int quizId = 3;

        using (var testHost = new TestServer(new WebHostBuilder().UseStartup<Startup>()))
        {
            int i;
            var client = testHost.CreateClient();

            var quiz = new QuizCreateModel("Creating a new Quiz.");
            var content = new StringContent(JsonConvert.SerializeObject(quiz));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"), content);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);

            /*-----------This loop is created to Post Question-----------------------------------------------------------------------------*/

            for (i = 1; i < 3; i++)
            {
                var QuestPostApiEndPoint = $"/api/quizzes/{quizId}/questions";
                var question = new QuestionCreateModel($"This is Question id: {i}");
                content = new StringContent(JsonConvert.SerializeObject(question));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var responseQuestion = await client.PostAsync(new Uri(testHost.BaseAddress, QuestPostApiEndPoint), content);
                Assert.Equal(HttpStatusCode.Created, responseQuestion.StatusCode);
                Assert.NotNull(responseQuestion.Headers.Location);

                /*-----------This loop is created to Post two answer for each question, created in above loop-------------------------------------*/

                for (int j = 1; j < 3; j++)
                {
                    var answerPostApiEndPoint = $"/api/quizzes/{quizId}/questions/{i}/answers";
                    var answer = new AnswerCreateModel($"This is answer {j} for question id: {i}");
                    content = new StringContent(JsonConvert.SerializeObject(answer));
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var responseAnswer = await client.PostAsync(new Uri(testHost.BaseAddress, answerPostApiEndPoint), content);
                    Assert.Equal(HttpStatusCode.Created, responseAnswer.StatusCode);
                    Assert.NotNull(responseAnswer.Headers.Location);
                }

                /*---------Here we are updating Question, adding CorrectAnswerId and Text for each Question after generating Question and Answer--------*/

                var questionPutApiEndPoint = $"/api/quizzes/{quizId}/questions/{i}";
                var questiontoupdate = new QuestionUpdateModel { CorrectAnswerId = 1, Text = $"This question with id: {i} is updated" };
                content = new StringContent(JsonConvert.SerializeObject(questiontoupdate));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var responseupdateQuestion = await client.PutAsync(new Uri(testHost.BaseAddress, questionPutApiEndPoint), content);
                Assert.Equal(HttpStatusCode.NoContent, responseupdateQuestion.StatusCode);

            }


        }
    }
}