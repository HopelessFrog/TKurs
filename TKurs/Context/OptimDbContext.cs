using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TKurs.Model;

namespace TKurs.Context
{
    public class OptimDbContext : DbContext
    {
        public OptimDbContext()
        {

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=optim.db");
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Method> Mehods { get; set; }
        public DbSet<Model.Task> Tasks { get; set; }


    }
}
