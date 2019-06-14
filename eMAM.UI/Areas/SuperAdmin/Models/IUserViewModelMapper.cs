using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMAM.UI.Areas.SuperAdmin.Models
{
    public interface IUserViewModelMapper<TEntity, TViewModel>
    {
        Task<TViewModel> MapFrom(TEntity entity);
    }
}
