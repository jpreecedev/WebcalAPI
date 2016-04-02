namespace WebcalAPI.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class PagedResponse<T>
    {
        public PagedResponse(IQueryable<T> data, int pageIndex, int pageSize)
        {
            Data = data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            Total = data.Count();
        }

        public PagedResponse(IEnumerable<T> data, int pageIndex, int pageSize)
        {
            Data = data.Skip((pageIndex - 1)*pageSize).Take(pageSize).ToList();
            Total = data.Count();
        }

        public int Total { get; set; }
        public ICollection<T> Data { get; set; }
    }
}