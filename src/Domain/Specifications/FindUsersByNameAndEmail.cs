using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Domain.Specifications
{
   public class FindUserByNameAndEmail : ISpecification<AppUser>
    {
        private readonly string nombre;
        private readonly string correo;
        private readonly int idDepartamento;
        public FindUserByNameAndEmail(string nombre, string correo,int idDepartamento)
        {
            this.nombre = nombre;
            this.correo = correo;
            this.idDepartamento = idDepartamento;
        }
        public Func<AppUser, bool> Traer()
        {
            Func<AppUser, bool> expresionA = null;
            if (!string.IsNullOrWhiteSpace(nombre) && !string.IsNullOrWhiteSpace(correo) && idDepartamento !=0) expresionA = new Func<AppUser, bool>(c => c.Name.ToLower().Contains(nombre.ToLower()) && c.AccessIdentifier.Contains(correo)&& c.DepartmentId.ToString().Contains((idDepartamento.ToString())) && c.UserType == AppUser.internalUserType);

            else if ( !string.IsNullOrWhiteSpace(correo) && idDepartamento != 0) expresionA = new Func<AppUser, bool>(c =>  c.AccessIdentifier.Contains(correo) && c.DepartmentId.ToString().Contains((idDepartamento.ToString())) && c.UserType == AppUser.internalUserType);
            else if (!string.IsNullOrWhiteSpace(nombre)  && idDepartamento != 0) expresionA = new Func<AppUser, bool>(c => c.Name.ToLower().Contains(nombre.ToLower())  && c.DepartmentId.ToString().Contains((idDepartamento.ToString())) && c.UserType == AppUser.internalUserType);
            else if (!string.IsNullOrWhiteSpace(nombre) && !string.IsNullOrWhiteSpace(correo)) expresionA = new Func<AppUser, bool>(c => c.Name.ToLower().Contains(nombre.ToLower()) && c.AccessIdentifier.Contains(correo)  && c.UserType == AppUser.internalUserType);

            else if (!string.IsNullOrWhiteSpace(nombre)) expresionA = new Func<AppUser, bool>(c=>c.Name.ToLower().Contains(nombre.ToLower()) && c.UserType == AppUser.internalUserType);
            else if (!string.IsNullOrWhiteSpace(correo)) expresionA = new Func<AppUser, bool>(c => c.AccessIdentifier.Contains(correo) && c.UserType == AppUser.internalUserType);
            else if (idDepartamento != 0) expresionA = new Func<AppUser, bool>(c => c.DepartmentId.ToString().Contains((idDepartamento.ToString())) && c.UserType == AppUser.internalUserType);
            return expresionA;        
        }
    }

  
}
