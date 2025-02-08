using Domain.DTO;
using Domain.SecondAppModels;
using Microsoft.AspNetCore.Mvc;
using Repository.Interface;

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

            if (questions == null || questions.Count == 0 ||
                databases == null || databases.Count == 0 ||
                users == null || users.Count == 0)
            {
                throw new Exception("Tables must not be empty");
            }

            // Perform the join and return the result
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

    }
}
