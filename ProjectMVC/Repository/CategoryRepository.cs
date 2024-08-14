

using DataAccessLayer.Data;
using Entities.Models;

namespace ProjectMVC.Repository
{
    public class CategoryRepository :IGeneircRepository<Category>
    {
        private readonly ApplicationDbContext context;

        public CategoryRepository(ApplicationDbContext context )
        {
            this.context = context;
        }
        public void Add(Category obj)
        {
            context.Add(obj);
        }

        public List<Category> GetAll()
        {
            return context.categories.ToList();
        }

        //public Category GetById(int? id)
        //{
        //}

        public void Update(int id,Category obj)
        {
          var category = context.categories.SingleOrDefault(c=>c.id == id);
            category.name= obj.name;
            category.description= obj.description;
            category.CreatedTime=obj.CreatedTime;
        }
        public void Delete(int id)
        {

            var category=GetById(id);
            context.Remove(category);

        }
        public void Save() {
        
          context.SaveChanges();
        }

        public Category GetById(int id)
        {
            return (Category)context.categories.SingleOrDefault(c => c.id == id);

        }
    }
}
