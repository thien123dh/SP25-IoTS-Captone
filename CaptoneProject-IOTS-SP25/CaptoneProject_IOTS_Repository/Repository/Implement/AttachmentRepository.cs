using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Repository.Repository.Implement
{
    public class AttachmentRepository : RepositoryBase<Attachment>
    {
        public List<Attachment>? GetAttachmentsByEntityId(int entityId, int entityType)
        {
            List<Attachment>? res = _dbSet.Where(att => att.EntityId == entityId && att.EntityType == entityType)
                                            .ToList();

            return res;
        }
    }
}
