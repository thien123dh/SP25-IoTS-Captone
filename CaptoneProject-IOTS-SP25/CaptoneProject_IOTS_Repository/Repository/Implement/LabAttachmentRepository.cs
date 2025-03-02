using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace CaptoneProject_IOTS_Repository.Repository.Implement
{
    public class LabAttachmentRepository : RepositoryBase<LabAttachment>
    {
        public List<LabAttachment> GetByLabId(int labId)
        {
            return _dbSet
                .Where(i => i.LabId == labId)
                .ToList();
        }
    }
}
