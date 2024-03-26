using NProjectMVC.Models;
using System.Linq.Expressions;

namespace NProjectMVC.Interface
{
    public interface IRepository<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> condition);
        bool Create(T entity);
        bool Update(T entity);
        bool Delete(T entity);
    }
}
