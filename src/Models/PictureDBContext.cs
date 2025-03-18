using Microsoft.EntityFrameworkCore;

namespace CatDogBearMicroservice.Models
{

    public class PictureDbContext : DbContext
    {
        public PictureDbContext(DbContextOptions<PictureDbContext> options)
            : base(options)
        {
        }

        public DbSet<Picture> Pictures { get; set; }
    }
}