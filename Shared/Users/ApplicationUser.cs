using System;
using System.Collections.Generic;

namespace Netrunner.Shared.Users;

public class ApplicationUser
{
    public Guid Id { get; init; }

    public string Username { get; init; }
    public List<Guid> Rooms { get; set; } = new();
    public List<Guid> Invitations { get; set; } = new();
    public List<Contact> Contacts { get; set; } = new();

    public ApplicationUser(Guid id, string username)
    {
        Id = id;
        Username = username;
    }
}