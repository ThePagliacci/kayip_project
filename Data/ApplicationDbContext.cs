using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kayip_project.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace kayip_project.Data
{
    public class ApplicationDbContext: IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }

        //creating table
        public DbSet<Post> Posts { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Post>().HasData(
                new Post {Id = 1, Title = "person1",Description =  "loremipsum", ContactInfo = "person1@gmail.com", Image = "", ApplicationUserId = "d5634d7e-4f17-4261-900f-bb33d4f21633"},
                new Post {Id = 2, Title = "person2",Description =  "loremipsum", ContactInfo = "person2@gmail.com", Image = "", ApplicationUserId = "d5634d7e-4f17-4261-900f-bb33d4f21633"},
                new Post {Id = 3, Title = "person3",Description =  "loremipsum", ContactInfo = "person3@gmail.com", Image = "",ApplicationUserId = "d5634d7e-4f17-4261-900f-bb33d4f21633"},
                new Post {Id = 4, Title = "person4",Description =  "loremipsum", ContactInfo = "person4@gmail.com", Image = "", ApplicationUserId = "d5634d7e-4f17-4261-900f-bb33d4f21633"}
            );
        }
    }
}
