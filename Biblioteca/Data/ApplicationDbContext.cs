using Biblioteca.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Biblioteca.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Livro> Livros { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Movimentacao> Movimentacoes { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Livro>().ToTable("Livros");
            builder.Entity<Genero>().ToTable("Generos");
            builder.Entity<Movimentacao>().ToTable("Movimentacoes");
            builder.Entity<Reserva>().ToTable("Reservas");
            builder.Entity<Avaliacao>().ToTable("Avaliacoes");
            builder.Entity<Usuario>().ToTable("Usuarios");

            Guid AdminGuid = Guid.NewGuid();
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = AdminGuid.ToString(),
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Aluno",
                    NormalizedName = "ALUNO"
                }
            );

            Guid UserGuid = Guid.NewGuid();
            var hasher = new PasswordHasher<IdentityUser>();

            builder.Entity<IdentityUser>().HasData(
                new IdentityUser
                {
                    Id = UserGuid.ToString(),
                    UserName = "admin@admin.com",
                    NormalizedUserName = "ADMIN@ADMIN.COM",
                    Email = "admin@admin.com",
                    NormalizedEmail = "ADMIN@ADMIN.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Admin@2025"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            );

            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = UserGuid.ToString(),
                    RoleId = AdminGuid.ToString()
                }
            );

        }


    }
}
