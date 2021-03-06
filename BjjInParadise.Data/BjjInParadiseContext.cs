﻿using BjjInParadise.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BjjInParadise.Data
{
    public class BjjInParadiseContext : DbContext
    {
        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Camp> Camps { get; set; }
        public DbSet<CampRoomOption> CampRoomOptions { get; set; }
         public DbSet<User> ApplicationUsers { get; set; }

        public BjjInParadiseContext() : base("name=DefaultConnection")
        {
        }
        static BjjInParadiseContext()
        {
            Database.SetInitializer<BjjInParadiseContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            // configures one-to-many relationship
            modelBuilder.Entity<Booking>()
                .HasOptional<User>(s => s.User)
                .WithMany(g => g.Bookings)
                .HasForeignKey(s => s.UserId).WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);

        }

    }


}
