using NProjectMVC.Data;
using NProjectMVC.Interface;
using NProjectMVC.Models;
using System.Linq.Expressions;
using System.Security.AccessControl;

namespace NProjectMVC.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly NProjectContext _context;
        public Repository(NProjectContext nProjectContext)
        {
            _context = nProjectContext;
        }

        public bool Create(T entity, string userId)
        {
            _context.Set<T>().Add(entity);
            return _context.SaveChanges(userId) > 0 ? true : false;
        }

        public bool Delete(T entity, string userId)
        {
            _context.Set<T>().Remove(entity);
            return _context.SaveChanges(userId) > 0 ? true : false;
        }

        public IQueryable<T> FindAll()
        {
            return _context.Set<T>();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> condition)
        {
            return _context.Set<T>().Where(condition);
        }

        public bool Update(T entity, string userId)
        {
            _context.Set<T>().Update(entity);
            return _context.SaveChanges(userId) > 0 ? true : false;
        }

        public T1 GetShadowProperty<T1>(string propertyName, T entity) 
        {
            return _context.GetShadowProperty<T1>(propertyName, entity);
        }
    }
}
