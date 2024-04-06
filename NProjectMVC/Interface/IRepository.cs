using NProjectMVC.Models;
using System.Linq.Expressions;

namespace NProjectMVC.Interface
{
    public interface IRepository<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> condition);
        bool Create(T entity, string userId);
        bool Update(T entity, string userId);
        bool Delete(T entity, string userId);
        T1 GetShadowProperty<T1>(string propertyName, T entity);

	}
}
