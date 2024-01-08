namespace BSUIR.Repositories.Repository
{
    public interface IRepository<T>
    {
        public IEnumerable<T> GetAll();
        public T? Get(Guid id);
        public void Create(T user);
        public void Update(T user);
        public void Delete(Guid id);
    }
}
