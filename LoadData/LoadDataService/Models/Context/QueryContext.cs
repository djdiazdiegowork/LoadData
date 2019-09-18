using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoadDataService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoadDataService.Models.Context
{
    public class QueryContext : DbContext
    {
        public DbSet<Session> Sessions { get; set; }

        public QueryContext(DbContextOptions<QueryContext> options) : base(options)
        {

        }
    }
}
