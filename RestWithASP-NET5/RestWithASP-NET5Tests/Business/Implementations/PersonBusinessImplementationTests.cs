using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestWithASP_NET5.Business.Implementations;
using RestWithASP_NET5.Data.Converter.Implementation;
using RestWithASP_NET5.Data.VO;
using RestWithASP_NET5.Model;
using RestWithASP_NET5.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestWithASP_NET5.Business.Implementations.Tests
{
    [TestClass()]
    public class PersonBusinessImplementationTests
    {

        private Mock<IPersonRepository> personRepository;
        private Mock<PersonConverter> _converter;

        private PersonBusinessImplementation unit;
        private PersonVO personVO;
        private Person person;

        private long newLong = long.MinValue;

        public PersonBusinessImplementationTests()
        {
            this.personRepository = new Mock<IPersonRepository>();
            this._converter = new Mock<PersonConverter>();
            this.unit = new PersonBusinessImplementation(personRepository.Object);
            this.personVO = new PersonVO();
            this.person = new Person();
        }

        [TestMethod("Delete(). testa fluxo de delete. Retorno void")]
        public void testeCase1()
        {
            unit.Delete(String.Empty);

        }

        [TestMethod("Create(). Testa criacao de um person. Retorna obj criado")]
        public void testeCase2()
        {        
            personRepository.Setup(x => x.Create(It.IsAny<Person>())).Returns(GetPerson());

            PersonVO result = unit.Create(personVO);
            
            Assert.IsNotNull(result);
        }

        [TestMethod("FindAll(). Localiza todos elementos person. Retorna lista encontrada")]
        public void testeCase3()
        {
            personRepository.Setup(x => x.FindAll()).Returns(new List<Person>());

            unit.FindAll();
        }

        [TestMethod("FindById(). Localiza determinado person. Retorna elemento")]
        public void testeCase4()
        {
            personRepository.Setup(x => x.FindById(newLong)).Returns(new Person());

            unit.FindById(newLong);
        }

        [TestMethod("Update(). Atualiza determinado person. Retorna elemento")]
        public void testeCase5()
        {
            personRepository.Setup(x => x.Update(It.IsAny<Person>())).Returns(GetPerson());

            PersonVO result = unit.Update(GetPersonVO());

            Assert.IsNotNull(result);
        }

        [TestMethod("FindByEmail(). Localiza person por e-mail. Retorna elemento")]
        public void testeCase6()
        {
            List<Person> listExpected = new List<Person>();
            listExpected.Add(GetPerson());

            personRepository.Setup(x => x.FindByEmail(It.IsAny<string>())).Returns(listExpected);

            List<PersonVO> result = unit.FindByEmail(String.Empty);

            Assert.IsNotNull(result);
        }

        private Person GetPerson()
        {
            return new Person()
            {
                CompletName = "Nome Completo",
                Email = "E-mail",
                Id = newLong
            };
        }

        private PersonVO GetPersonVO()
        {
            return new PersonVO()
            {
                CompletName = "Nome Completo",
                Email = "E-mail",
                Id = newLong
            };
        }
    }
}