using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kayip_project.Data;
using kayip_project.Models;
using kayip_project.Repository.IRepository;

namespace kayip_project.Repository
{ 
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public IPostRepository Post {get; private set;}
        public IApplicationUserRepository ApplicationUser {get; private set;}
        public UnitOfWork(ApplicationDbContext db)
        {
            _db= db;
            Post = new PostRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}