using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kayip_project.Models;

namespace kayip_project.Repository.IRepository
{
    public interface IPostRepository: IRepository<Post>
    {
        void Update(Post post);
    }
}