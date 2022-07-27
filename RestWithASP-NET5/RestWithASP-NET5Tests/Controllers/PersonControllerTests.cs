using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestWithASP_NET5.Business;
using Moq;
using RestWithASP_NET5.Data.VO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RestWithASP_NET5.Controllers.Tests
{
    [TestClass()]
    public class PersonControllerTests
    {
        private Mock<IPersonBusiness> personBusiness;

        public PersonControllerTests()
        {
            this.personBusiness = new Mock<IPersonBusiness>();
        }

        [TestMethod("GetByEmail(). Não encontra e-mail. retorna status code 404")]
        public void testeCase1()
        {
            var iLogger = new Mock<ILogger<PersonController>>().Object;
            PersonController unit = new PersonController(iLogger, personBusiness.Object);

            personBusiness.Setup(x => x.FindByEmail(It.IsAny<string>())).Returns(new List<PersonVO>());

            IActionResult result = unit.GetByEmail(String.Empty);
            Xunit.Assert.IsAssignableFrom<NotFoundResult>(result);
        }

        [TestMethod("GetByEmail(). Encontra e-mail. retorna status code 200")]
        public void testeCase2()
        {
            var iLogger = new Mock<ILogger<PersonController>>().Object;
            PersonController unit = new PersonController(iLogger, personBusiness.Object);

            List<PersonVO> expected = GetListEmail();

            personBusiness.Setup(x => x.FindByEmail(It.IsAny<string>())).Returns(expected);

            IActionResult result = unit.GetByEmail(String.Empty);
            Xunit.Assert.IsAssignableFrom<OkObjectResult>(result);
        }

        [TestMethod("Post(). Dados esperados nulos. Retorna status code 400")]
        public void testeCase3()
        {
            var iLogger = new Mock<ILogger<PersonController>>().Object;
            PersonController unit = new PersonController(iLogger, personBusiness.Object);

            IActionResult result = unit.Post(null);
            Xunit.Assert.IsAssignableFrom<BadRequestResult>(result);
        }


        [TestMethod("Post(). Dados esperados corretos. Retorna status code 200")]
        public void testeCase4()
        {
            var iLogger = new Mock<ILogger<PersonController>>().Object;
            PersonController unit = new PersonController(iLogger, personBusiness.Object);
            PersonVO person = new PersonVO();

            personBusiness.Setup(x => x.Create(It.IsAny<PersonVO>())).Returns(person);

            IActionResult result = unit.Post(person);
            Xunit.Assert.IsAssignableFrom<OkObjectResult>(result);
        }

        [TestMethod("Put(). Dados esperados nulos. Retorna status code 400")]
        public void testeCase5()
        {
            var iLogger = new Mock<ILogger<PersonController>>().Object;
            PersonController unit = new PersonController(iLogger, personBusiness.Object);

            IActionResult result = unit.Put(null);
            Xunit.Assert.IsAssignableFrom<BadRequestResult>(result);
        }


        [TestMethod("Put(). Dados esperados corretos. Retorna status code 200")]
        public void testeCase6()
        {
            var iLogger = new Mock<ILogger<PersonController>>().Object;
            PersonController unit = new PersonController(iLogger, personBusiness.Object);
            PersonVO person = new PersonVO();

            personBusiness.Setup(x => x.Update(It.IsAny<PersonVO>())).Returns(person);

            IActionResult result = unit.Put(person);
            Xunit.Assert.IsAssignableFrom<OkObjectResult>(result);
        }

        [TestMethod("Delete(). Dados esperados corretos. Retorna status code 204")]
        public void testeCase7()
        {
            var iLogger = new Mock<ILogger<PersonController>>().Object;
            PersonController unit = new PersonController(iLogger, personBusiness.Object);        

            IActionResult result = unit.Delete(String.Empty);
            Xunit.Assert.IsAssignableFrom<NoContentResult>(result);
        }

        private List<PersonVO> GetListEmail()
        {
            PersonVO elemento = new PersonVO();
            elemento.CompletName = "Nome completo";
            elemento.Id = long.MinValue;
            elemento.Email = "E-mail";

            return new List<PersonVO>() { elemento };          
        }
    }
}