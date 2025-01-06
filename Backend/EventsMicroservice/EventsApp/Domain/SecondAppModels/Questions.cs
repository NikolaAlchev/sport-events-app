using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SecondAppModels
{
    public class Questions
    {
        public Guid Id { get; set; }
        public string QuestionText { get; set; }
        public string QuestionAnswer { get; set; }
        public Guid DatabaseId { get; set; }

    }
}
