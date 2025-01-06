using Domain.SecondAppModels;
using Microsoft.AspNetCore.Identity;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implementation
{
    public class QuestionAppRepository : IQuestionAppRepository
    {
        private readonly QApplicationDbContext _context;

        public QuestionAppRepository(QApplicationDbContext context)
        {
            _context = context;
        }

        public List<Questions> getAllQuestions()
        {
            List<Questions> questions = _context.Questions.ToList();
            return questions;
        }

        public List<Databases> getAllDatabases()
        {
            List<Databases> databases = _context.Databases.ToList();
            return databases;
        }

        public List<Users> getAllUsers()
        {
            List<Users> users = _context.Users
                .Select(u => new Users
                {
                    Id = u.Id,
                    UserName = u.UserName,
                })
                .ToList();
            return users;
        }
    }
}
