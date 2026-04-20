using System;
using System.Collections.Generic;
using System.Linq;
using backend.context.common.domain;
using backend.context.nailservice.domain.enums;

namespace backend.context.nailservice.domain.entity;

public class NailService : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int Duration { get; private set; } // theo phút
    public ServiceCategory Category { get; private set; }
    public string? Image { get; private set; }
    public bool IsActive { get; private set; }

    public List<Promotion> Promotions { get; private set; } = new();

    private NailService()
    {
        Name = null!;
    }

    public static NailService Create(
        string name, string? description, decimal price, int duration, 
        ServiceCategory category, string? image)
    {
        if (price < 0) throw new ArgumentException("Giá không được âm.");
        if (duration <= 0) throw new ArgumentException("Thời gian phải lớn hơn 0.");

        return new NailService
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Price = price,
            Duration = duration,
            Category = category,
            Image = image,
            IsActive = true
        };
    }

    public void Update(string name, string? description, decimal price, int duration, ServiceCategory category, string? image)
    {
        Name = name;
        Description = description;
        Price = price;
        Duration = duration;
        Category = category;
        Image = image ?? Image;
    }

    public void ToggleStatus()
    {
        IsActive = !IsActive;
    }

    // --- Khuyến mãi ---
    public void AddPromotion(string title, string description, decimal discountPercent, DateTime startDate, DateTime endDate)
    {
        if (discountPercent < 0 || discountPercent > 100)
            throw new ArgumentException("Phần trăm giảm giá không hợp lệ (0-100%).");

        Promotions.Add(new Promotion(title, description, discountPercent, startDate, endDate));
    }

    public void RemovePromotion(Guid promotionId)
    {
        var promo = Promotions.FirstOrDefault(p => p.Id == promotionId);
        if (promo != null)
        {
            Promotions.Remove(promo);
        }
    }
}
