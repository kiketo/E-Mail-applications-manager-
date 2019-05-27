using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMAM.UI.Mappers
{
    public interface IViewModelMapper<TEntity, TViewModel>
    {
        TViewModel MapFrom(TEntity entity);
    }
}
