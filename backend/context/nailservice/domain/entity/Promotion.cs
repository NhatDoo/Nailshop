using System;
using backend.context.common.domain;

namespace backend.context.nailservice.domain.entity;

public class Promotion
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public decimal DiscountPercent { get; private set; } // 0-100
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    private Promotion() { } // cho EF Core

    public Promotion(string title, string description, decimal discountPercent, DateTime startDate, DateTime endDate)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        DiscountPercent = discountPercent;
        StartDate = startDate.ToUniversalTime();
        EndDate = endDate.ToUniversalTime();
    }
}
