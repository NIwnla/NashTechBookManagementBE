using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace NashTechProjectBE.Application.Common.Models;

public class PaginatedList<T1,T2>
{
    public List<T1> Items { get; private set; }
    public int PageIndex { get; private set; }
    public int TotalPages { get; private set; }
    public int TotalCount { get; private set; }

    public PaginatedList(List<T1> items, int count, int pageIndex, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }

    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedList<T1,T2>> CreateAsync(IQueryable<T2> source, int pageIndex, int pageSize, IMapper mapper)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        var itemDtos = mapper.Map<List<T1>>(items);
        return new PaginatedList<T1,T2>(itemDtos, count, pageIndex, pageSize);
    }
}
