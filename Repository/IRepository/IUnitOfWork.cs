using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kayip_project.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IPostRepository Post{get;}
        IApplicationUserRepository ApplicationUser{get;}
        IMessageRepository Message{get;}
        void Save();
    }
}