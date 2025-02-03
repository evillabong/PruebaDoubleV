using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entities.PruebaDoubleV.Sql
{
    public partial class DatabaseContext
    {
        public virtual DbSet<PersonModel> PersonModels { get; set; }
    }
}
