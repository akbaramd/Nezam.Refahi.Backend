namespace Nezam.Refahi.Shared.Application.Common.Models;

/// <summary>
/// مدل درخواست صفحه‌بندی برای استفاده در کوئری‌ها
/// </summary>
public class PaginationRequest
{
    private int _pageNumber = 1;
    private int _pageSize = 10;
    
    private const int MaxPageSize = 100;
    
    /// <summary>
    /// شماره صفحه (از 1 شروع می‌شود)
    /// </summary>
    public int PageNumber 
    { 
        get => _pageNumber; 
        set => _pageNumber = value < 1 ? 1 : value; 
    }
    
    /// <summary>
    /// تعداد آیتم در هر صفحه (حداکثر 100)
    /// </summary>
    public int PageSize 
    { 
        get => _pageSize; 
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 10 : value; 
    }
}
