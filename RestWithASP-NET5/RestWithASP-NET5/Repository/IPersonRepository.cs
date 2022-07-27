using RestWithASP_NET5.Model;
using System.Collections.Generic;

namespace RestWithASP_NET5.Repository
{
    public interface IPersonRepository : IRepository<Person>
    {
        List<Person> FindByEmail(string email);
    }
}