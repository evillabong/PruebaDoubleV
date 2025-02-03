using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Base
{
    public class PersonBase
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Identification { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTimeOffset DateTimeOffSet {  get; set; }

    }
}
