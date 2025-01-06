using Domain.DTO;
using Domain.SecondAppModels;
using Microsoft.AspNetCore.Mvc;
using Repository.Interface;
using Service.Implementation;
using Service.Interface;

namespace EventsApp.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly IQuestionAppRepository _questionRepository;

        public QuestionsController(IQuestionAppRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }
        public List<UserQuestionsDTO> Index()
        {
            List<Questions> questions = _questionRepository.getAllQuestions();
            List<Databases> databases = _questionRepository.getAllDatabases();
            List<Users> users = _questionRepository.getAllUsers();

            if (questions != null && databases != null && users != null)
            {
                var userQuestions = from user in users
                                    join db in databases on user.Id equals db.OwnerId
                                    join question in questions on db.Id equals question.DatabaseId
                                    select new UserQuestionsDTO
                                    {
                                        UserName = user.UserName,
                                        QuestionText = question.QuestionText,
                                        QuestionAnswer = question.QuestionAnswer
                                    };

                return userQuestions.ToList();
            }

            throw new Exception("Tables must not be empty");

        }
    }
}
