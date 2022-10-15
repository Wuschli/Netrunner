using System;
using System.Collections.Generic;

namespace Netrunner.Shared.Chat;

public record ChatRoom
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<Guid> Members { get; init; }
    public ICollection<Guid> Invitations { get; init; }
}