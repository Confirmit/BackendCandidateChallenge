using System.Collections.Generic;
using System.Threading.Tasks;
using QuizService.Model;

namespace QuizService
{
    //Todo : This Interface is created to define more method like put,post and delete as we define for Get.
    //Todo : It can be helpfull in future to understand the flow of code and to extend the functionality.

    public interface IQuizService
    {
        Task<List<QuizResponseModel>> GetAllAsync();

        Task<QuizResponseModel> GetAsyncById(int id);
    }
}
