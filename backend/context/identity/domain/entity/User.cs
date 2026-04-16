using System;
using System.Collections.Generic;
using backend.context.common.domain;
using backend.context.identity.domain.vo;
using backend.context.identity.domain.events;

namespace backend.context.identity.domain.entity;

public class User : AggregateRoot
{
    public UserIdVO Id { get; private set; }
    public string Ten { get; private set; }
    public PhoneNumberVO SDT { get; private set; }
    public EmailVO Email { get; private set; }
    public RoleVO Role { get; private set; }
    public Auth Auth { get; private set; } // Liên danh (Aggregate)

    // Constructor private để ép buộc sử dụng Factory Method
    private User(UserIdVO id, string ten, PhoneNumberVO sdt, EmailVO email, RoleVO role, string passwordHash)
    {
        Id = id;
        Ten = ten;
        SDT = sdt;
        Email = email;
        Role = role;
        Auth = Auth.Create(Id, passwordHash);
    }

    private User() 
    {
        Id = null!;
        Ten = null!;
        SDT = null!;
        Email = null!;
        Role = null!;
        Auth = null!;
    } // Required for EF Core

    // Factory Method
    public static User Create(string ten, string sdt, string email, string role, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(ten))
        {
            throw new ArgumentException("Tên không được để trống.");
        }

        var userId = UserIdVO.Create();
        var userEmail = new EmailVO(email);
        
        var user = new User(
            userId,
            ten,
            new PhoneNumberVO(sdt),
            userEmail,
            new RoleVO(role),
            passwordHash
        );

        // Raise Event
        user.AddDomainEvent(new UserRegisteredEvent(userId, userEmail));

        return user;
    }

    public void UpdatePassword(string newHash)
    {
        Auth.UpdatePasswordHash(newHash);
    }
}
