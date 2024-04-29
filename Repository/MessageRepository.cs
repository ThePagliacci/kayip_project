using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kayip_project.Data;
using kayip_project.Models;
using kayip_project.Repository.IRepository;

namespace kayip_project.Repository
{
    public class MessageRepository: Repository<Message>, IMessageRepository
    {
        private ApplicationDbContext _db;
        public MessageRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
        public void Update(Message message)
        {
            var objFromDb = _db.Messages.FirstOrDefault(u => u.Id == message.Id);
            if(objFromDb != null)
            {
                objFromDb.Subject = message.Subject;
                objFromDb.Body = message.Body;
                objFromDb.Name = message.Name;
            }
        }
        
    }
}