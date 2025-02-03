using System;
using System.Collections.Generic;

namespace Model.Entities.PruebaDoubleV.Sql;

public partial class Person
{
    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Identification { get; set; } = null!;

    public string? Email { get; set; }

    public string IdentificationType { get; set; } = null!;

    public DateTimeOffset? CreatedAt { get; set; }

    public virtual ICollection<User> User { get; set; } = new List<User>();
}
