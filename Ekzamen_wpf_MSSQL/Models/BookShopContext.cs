using System.Data.Entity;
using Ekzamen_wpf_MSSQL.Models;

namespace Ekzamen_wpf_MSSQL.DataLayer
{
    public class BookShopContext : DbContext
    {
        public BookShopContext() : base("name=Company_db")
        {
            // Отключаем инициализатор базы данных
            Database.SetInitializer<BookShopContext>(null);

        }

        public DbSet<BookModel> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Theme> Themes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка таблицы books
            modelBuilder.Entity<BookModel>()
                .ToTable("books")
                .HasRequired(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);

            modelBuilder.Entity<BookModel>()
                .HasRequired(b => b.Theme)
                .WithMany(t => t.Books)
                .HasForeignKey(b => b.ThemeId);
        }
    }
}