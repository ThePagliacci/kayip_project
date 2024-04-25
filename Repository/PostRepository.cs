using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kayip_project.Data;
using kayip_project.Models;
using kayip_project.Repository.IRepository;

namespace kayip_project.Repository
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        private ApplicationDbContext _db;
        public PostRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
        public void Update(Post post)
        {
            var objFromDb = _db.Posts.FirstOrDefault(u => u.Id == post.Id);
            if(objFromDb != null)
            {
                objFromDb.Title = post.Title;
                objFromDb.Description = post.Description;
                objFromDb.ContactInfo = post.ContactInfo;
                if(post.Image != null)
                {
                    objFromDb.Image = post.Image;
                }

            }
        }
    }
}