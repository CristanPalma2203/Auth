
using Domain.Specifications;
using Domain.Models;
using Domain.Repositories.Extenciones;
using Domain.Repositories.Extensiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        TEntity GetById(int id);
        TEntity Create(TEntity entity);
        void SaveAll(IList<TEntity> entity);
        TEntity Update(int id, TEntity entity);
        TEntity Delete(int id);
        TEntity Delete(TEntity entity);
        IQueryable<TEntity> Set();
        IEnumerable<TEntity> Filter(Func<TEntity, bool> predicate);

        IEnumerable<TEntity> Filter(ISpecification<TEntity> especificacion, params Expression<Func<TEntity, object>>[] includes);
        IEnumerable<TEntity> Filter(ISpecification<TEntity> especificaciones);

        IPagina<TEntity> GetPaged(IConsulta ownerParameters, ISpecification<TEntity> busqueda);
        IPagina<TEntity> GetPaged(IConsulta ownerParameters);

        TEntity GetById(int id, params Expression<Func<TEntity, object>>[] includes);

    }
}
