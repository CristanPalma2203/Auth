using Domain.Specifications;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Models.Rules
{
    class ExternalUsersEmailSent : IImportadoresCorreoEnviado
    {
        private readonly IExternalUserRepository importadoreRepo;

        public ExternalUsersEmailSent(IExternalUserRepository importadoreRepo)
        {
            this.importadoreRepo = importadoreRepo;
        }
        public bool VerifyEmailEnviado(int id)
        {
            var user = importadoreRepo.Filter(new FindExternalUserWithEmailSent(id));
            return user.Count() > 0;
        }

    }

    public interface IImportadoresCorreoEnviado:IRule {
        bool VerifyEmailEnviado(int id);
    }

}