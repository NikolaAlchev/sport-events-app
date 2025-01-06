using Domain.SecondAppModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IQuestionAppRepository
    {
        public List<Questions> getAllQuestions();
        public List<Databases> getAllDatabases();
        public List<Users> getAllUsers();
    }
}
