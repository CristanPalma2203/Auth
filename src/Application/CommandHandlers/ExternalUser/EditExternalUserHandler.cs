using Application.Commands.ExternalUser;
using Application.Dtos;
using Application.Helpers;
using MapsterMapper;
using Domain.Models;
using Domain.Specifications;
using Domain.Helpers;
using Domain.Repositories;
using Domain.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Application.CommandHandlers.ExternalUser
{
    public class EditExternalUserHandler : AbstractHandler<EditExternalUser>
    {
        private readonly IExternalUserRepository externalUserRepository;
        private readonly ITokenService tokenSrvice;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailHelper correoHelper;
        private readonly IMapper mapper;
        public EditExternalUserHandler(IExternalUserRepository externalUserRepository, ITokenService tokenSrvice,
             IUnitOfWork unitOfWork, IEmailHelper correoHelper, IMapper mapper)
        {
            this.mapper = mapper;
            this.externalUserRepository = externalUserRepository;
            this.tokenSrvice = tokenSrvice;
            this.unitOfWork = unitOfWork;
            this.correoHelper = correoHelper;
        }

        public override IResponse Handle(EditExternalUser message)
        {
            var importardor = externalUserRepository.GetById(message.ExternalUser.Id.Value);
            var impo = mapper.Map<Domain.Models.ExternalUser>(message.ExternalUser);
            var CorreoViejo = importardor.Email;
            var cambioCorreo = importardor.Email != message.ExternalUser.Email;
            importardor.ActulizarImportador(impo);
            if (cambioCorreo){
                externalUserRepository.Update(importardor.Id, importardor);
                unitOfWork.Save();
                correoHelper.SendEmailUpdateNotification(CorreoViejo, impo.VerificationToken,(DateTime)importardor.UpdatedAt,impo.Email);
            }else{
                externalUserRepository.Update(importardor.Id, importardor);
            }
            return new OkResponse();
        }
    }
}
