namespace ProjectMVC.Repository
{
    public interface IGeneircRepository<T>
    {
        public T GetById(int id);   
        public List<T> GetAll();
        public void Add(T obj);
        public void Update(int id,T obj);
        public void Delete(int id);
        public void Save();


    }
}
