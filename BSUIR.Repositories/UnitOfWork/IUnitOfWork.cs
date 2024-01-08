using BSUIR.Repositories.Repository;

namespace BSUIR.Repositories.UnitOfWork
{
    public interface IUnitOfWork
    {
        public Task CommitAsync();
        public Repository<T> GetRepository<T>() where T : class;
    }
}
