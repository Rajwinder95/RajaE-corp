using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RajaEcorp.Models;

namespace RajaEcorp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<RajaEcorp.Models.Item> Item { get; set; }
        public DbSet<RajaEcorp.Models.Sale> Sale { get; set; }
        public DbSet<RajaEcorp.Models.Cart> Cart { get; set; }
    }
}
