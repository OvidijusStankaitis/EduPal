using Microsoft.EntityFrameworkCore;
using PSI_Project.Data;
using PSI_Project.Models;

namespace PSI_Project.Repositories
{
    public class CommentRepository : Repository<Comment>
    {
        public EduPalDatabaseContext EduPalContext => Context as EduPalDatabaseContext;

        public CommentRepository(EduPalDatabaseContext context) : base(context)
        {
        }
    }
}