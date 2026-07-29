using Application.CommandHandlers.Role;
using Application.Dtos;
using AutoMapper;
using Domain.Models;
using Domain.Repositories;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Test.ComandHanlderTest
{
    public class ConsultarrolHandlerTest
    {

        [TestCase]
        public void consultaPermisos_retornaLitaEstructurada()
        {

            var mockRepo = new Mock<IRoleRepository>();

            mockRepo.Setup(p => p.GetById(It.IsAny<int>())).Returns(new Role());
            var MockMapper = new Mock<IMapper>();
            MockMapper.Setup(p => p.Map<RoleDto>(It.IsAny<Role>())).Returns(new RoleDto());

            var respuesta = new GetRoleHandler(mockRepo.Object, MockMapper.Object);

            Assert.IsInstanceOf<RoleDto>(respuesta);
        }
    }
}
