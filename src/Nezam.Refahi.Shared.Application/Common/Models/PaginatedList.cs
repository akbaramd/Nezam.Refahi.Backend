namespace Nezam.Refahi.Shared.Application.Common.Models;

/// <summary>
/// پیاده‌سازی لیست صفحه‌بندی شده برای نمایش داده‌ها به صورت صفحه‌بندی شده
/// </summary>
/// <typeparam name="T">نوع آیتم‌های لیست</typeparam>
public class PaginatedList<T>
{
    /// <summary>
    /// لیست آیتم‌های موجود در صفحه فعلی
    /// </summary>
    public IReadOnlyCollection<T> Items { get; }
    
    /// <summary>
    /// شماره صفحه فعلی (از 1 شروع می‌شود)
    /// </summary>
    public int PageNumber { get; }
    
    /// <summary>
    /// تعداد آیتم در هر صفحه
    /// </summary>
    public int PageSize { get; }
    
    /// <summary>
    /// تعداد کل آیتم‌ها
    /// </summary>
    public int TotalCount { get; }
    
    /// <summary>
    /// تعداد کل صفحات
    /// </summary>
    public int TotalPages { get; }
    
    /// <summary>
    /// آیا صفحه قبلی وجود دارد
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
    
    /// <summary>
    /// آیا صفحه بعدی وجود دارد
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedList(IReadOnlyCollection<T> items, int totalCount, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        Items = items;
    }

    /// <summary>
    /// ایجاد یک لیست صفحه‌بندی شده از یک لیست معمولی
    /// </summary>
    public static PaginatedList<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        
        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}
