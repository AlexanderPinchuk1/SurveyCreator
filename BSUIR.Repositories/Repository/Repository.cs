using Microsoft.EntityFrameworkCore;

namespace BSUIR.Repositories.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;


        public Repository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }


        public IEnumerable<T> GetAll()
        {
            return _dbSet;
        }

        public T? Get(Guid id)
        {
            return _dbSet.Find(id);
        }

        public void Create(T item)
        {
            _dbSet.Add(item);
        }

        public void Update(T item)
        {
            _context.Entry(item).State = EntityState.Modified;
        }

        public void Delete(Guid id)
        {
            var item = _dbSet.Find(id);
            if (item == null)
            {
                return;
            }

            _dbSet.Remove(item);
        }
    }
}
