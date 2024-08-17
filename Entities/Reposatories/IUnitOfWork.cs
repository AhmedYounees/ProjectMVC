using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Reposatories
{
    public interface IUnitOfWork:IDisposable
    {
        ICategoryRepo Category { get; }
        IProductRepo Product { get; }

        int complete();
    }
}
