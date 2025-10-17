namespace TvShowTrackerAPI.DTOs.Common;

/// <summary>
/// Generic DTO for paginated API responses
/// 
/// Example response:
/// {
///   "items": [...],
///   "pageNumber": 1,
///   "pageSize": 20,
///   "totalPages": 5,
///   "totalCount": 95,
///   "hasPreviousPage": false,
///   "hasNextPage": true
/// }
/// </summary>
public class PaginatedResponse<T>
{
    /// <summary>
    /// Data items for this page
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Whether there's a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Whether there's a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Helper method to create paginated response
    /// </summary>
    public static PaginatedResponse<T> Create(List<T> items, int totalCount, int pageNumber, int pageSize)
    {
        return new PaginatedResponse<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
}