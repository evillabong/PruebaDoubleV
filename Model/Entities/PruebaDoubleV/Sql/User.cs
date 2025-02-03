using System;
using System.Collections.Generic;

namespace Model.Entities.PruebaDoubleV.Sql;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTimeOffset? CreatedAt { get; set; }

    public int? PersonId { get; set; }

    public virtual Person? Person { get; set; }
}
