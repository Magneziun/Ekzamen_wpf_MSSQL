// BookShopContext.cs
using System.Data.Entity;
using Ekzamen_wpf_MSSQL.Models;

namespace Ekzamen_wpf_MSSQL.DataLayer
{
    // Контекст базы данных для Entity Framework
    public class BookShopContext : DbContext
    {
        // Конструктор с именем подключения из конфига
        public BookShopContext() : base("name=Company_db")
        {
            // Отключаем инициализатор базы данных (используем существующую БД)
            Database.SetInitializer<BookShopContext>(null);
        }

        // DbSet'ы для работы с таблицами
        public DbSet<BookModel> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Theme> Themes { get; set; }

        // Настройка модели (Fluent API)
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка таблицы books и связей
            modelBuilder.Entity<BookModel>()
                .ToTable("books")
                .HasRequired(b => b.Author) // Книга обязана иметь автора
                .WithMany(a => a.Books)     // У автора много книг
                .HasForeignKey(b => b.AuthorId);

            modelBuilder.Entity<BookModel>()
                .HasRequired(b => b.Theme)  // Книга обязана иметь тему
                .WithMany(t => t.Books)     // У темы много книг
                .HasForeignKey(b => b.ThemeId);
        }
    }
}