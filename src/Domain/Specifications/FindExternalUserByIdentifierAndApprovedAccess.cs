using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Specifications
{
    public class FindExternalUserByIdentifierAndApprovedAccess : ISpecification<ExternalUser>
    { 
    private readonly string identifier;

    public FindExternalUserByIdentifierAndApprovedAccess(string identifier)
    {
        this.identifier = identifier;
    }

    public Func<ExternalUser, bool> Traer()
    {
        return new Func<ExternalUser, bool>(c => c.AccessApproved==true && c.Identifier.Replace("-", "").Trim() == identifier.Replace("-", "").Trim());

    }
}
}