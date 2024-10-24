﻿namespace Application.Pagination;

public class PaginationParams
{
    private const int maxPageSize = 50;
    private int _pageSize = 10;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > maxPageSize ? maxPageSize : value;
    }
    public int PageNumber { get; set; } = 1;
}
