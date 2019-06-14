using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eMAM.UI.Utills
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            List<T> items;
            var count = await source.CountAsync();
            if (count > pageSize)
            {
                var lesPage = count - (pageIndex - 1) * pageSize;
                if (lesPage < pageSize)
                {
                    //pageSize = count - (pageIndex-1) * pageSize;
                    items = await source.Take(lesPage).ToListAsync();
                }
                else
                {
                    items = await source.Skip(count - pageIndex * pageSize).Take(pageSize).ToListAsync();
                }
            }
            else
            {
                items = await source.ToListAsync();
            }
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
