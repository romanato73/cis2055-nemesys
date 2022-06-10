using cis2055_nemesys.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace cis2055_nemesys.Models;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Entities
    public DbSet<Report> Reports { get; set; }
    public DbSet<Investigation> Investigations { get; set; }
    public DbSet<Upvote> Upvotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Setup schema for Identity Framework
        base.OnModelCreating(modelBuilder);

        // Do not delete report on upvote delete
        modelBuilder.Entity<Upvote>()
            .HasOne(uv => uv.User)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        // Do not delete report on investigation delete
        modelBuilder.Entity<Investigation>()
            .HasOne(i => i.Report)
            .WithOne(r => r.Investigation)
            .OnDelete(DeleteBehavior.NoAction);

        // Password Hasher
        PasswordHasher<User> passwordHasher = new();

        // Create Admin User
        User admin = new()
        {
            Id = "b19fe869-0b67-470c-ae88-3921fea1bef1",
            UserName = "admin@nemesys.com",
            FullName = "NEMESYS",
            NormalizedUserName = "ADMIN@NEMESYS.COM",
            Email = "admin@nemesys.com",
            NormalizedEmail = "ADMIN@NEMESYS.COM",
            LockoutEnabled = false,
            EmailConfirmed = true,
            PhoneNumber = "",
        };
        admin.PasswordHash = passwordHasher.HashPassword(admin, "password");
        modelBuilder.Entity<User>().HasData(admin);

        // Create Reporter
        User reporter = new()
        {
            Id = "b19fe869-0b67-471c-ae88-3921fea1bef1",
            UserName = "reporter@nemesys.com",
            FullName = "REPORTER",
            NormalizedUserName = "REPORTER@NEMESYS.COM",
            Email = "reporter@nemesys.com",
            NormalizedEmail = "REPORTER@NEMESYS.COM",
            LockoutEnabled = false,
            EmailConfirmed = true,
            PhoneNumber = "",
        };
        reporter.PasswordHash = passwordHasher.HashPassword(reporter, "password");
        modelBuilder.Entity<User>().HasData(reporter);

        // Create Investigator
        User investigator = new()
        {
            Id = "b19fe869-0b67-472c-ae88-3921fea1bef1",
            UserName = "investigator@nemesys.com",
            FullName = "INVESTIGATOR",
            NormalizedUserName = "INVESTIGATOR@NEMESYS.COM",
            Email = "investigator@nemesys.com",
            NormalizedEmail = "INVESTIGATOR@NEMESYS.COM",
            LockoutEnabled = false,
            EmailConfirmed = true,
            PhoneNumber = "",
        };
        investigator.PasswordHash = passwordHasher.HashPassword(investigator, "password");
        modelBuilder.Entity<User>().HasData(investigator);

        // Create Roles
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole()
            {
                Id = "86d8d191-c517-4410-a8e2-a6c7b892f2d4",
                Name = "Admin",
                ConcurrencyStamp = "1",
                NormalizedName = "ADMIN"
            },
            new IdentityRole()
            {
                Id = "4ee3b560-ea8d-4ca9-a3fc-fd5ee8ce4e66",
                Name = "Reporter",
                ConcurrencyStamp = "1",
                NormalizedName = "REPORTER"
            },
            new IdentityRole()
            {
                Id = "62906a75-2189-4edf-8f98-311705447a72",
                Name = "Investigator",
                ConcurrencyStamp = "1",
                NormalizedName = "INVESTIGATOR"
            },
            new IdentityRole()
            {
                Id = "10996569-c42d-47e3-9821-65860ac72fcc",
                Name = "Guest",
                ConcurrencyStamp = "1",
                NormalizedName = "GUEST"
            }
        );

        // Assign roles
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>()
            {
                RoleId = "86d8d191-c517-4410-a8e2-a6c7b892f2d4",
                UserId = "b19fe869-0b67-470c-ae88-3921fea1bef1"
            },
            new IdentityUserRole<string>()
            {
                RoleId = "4ee3b560-ea8d-4ca9-a3fc-fd5ee8ce4e66",
                UserId = "b19fe869-0b67-471c-ae88-3921fea1bef1"
            },
            new IdentityUserRole<string>()
            {
                RoleId = "62906a75-2189-4edf-8f98-311705447a72",
                UserId = "b19fe869-0b67-472c-ae88-3921fea1bef1"
            }
        );
    }
}

