using RestWithASP_NET5.Model;
using RestWithASP_NET5.Model.Context;
using RestWithASP_NET5.Repository.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestWithASP_NET5.Repository.Implementations
{
    public class PersonRepositoryImplementation : GenericRepository<Person>, IPersonRepository
    {
        public PersonRepositoryImplementation(MySQLContext context) : base(context) { }

        public List<Person> FindByEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                return _context.Persons.Where(p => p.Email.Contains(email)).ToList();
            }
            
            return _context.Persons.ToList();
        }
    }
}
