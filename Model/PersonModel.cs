using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class PersonModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Identification { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
    }
}
