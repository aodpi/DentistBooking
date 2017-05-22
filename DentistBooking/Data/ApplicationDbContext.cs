using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DentistBooking.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

namespace DentistBooking.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public async static void Seed(IApplicationBuilder app)
        {

            // Get an instance of the DbContext from the DI container
            using (var context = app.ApplicationServices.GetRequiredService<ApplicationDbContext>())
            {
                context.Database.EnsureCreated();
                if (!context.Medics.Any())
                {
                    var roleManager = app.ApplicationServices.GetRequiredService<RoleManager<IdentityRole>>();
                    var role = new IdentityRole("Admin");
                    await roleManager.CreateAsync(role);

                    var user = new ApplicationUser
                    {
                        Email = "balan.valeriu@live.com",
                        UserName = "balan.valeriu@live.com"
                    };

                    var userManager = app.ApplicationServices.GetRequiredService<UserManager<ApplicationUser>>();
                    var result = await userManager.CreateAsync(user, "AlphaOmega_1994");

                    await userManager.AddToRoleAsync(user, "Admin");
                    context.Medics.AddRange(
                        new Medic
                        {
                            Name = "Valeriu Balan"
                        },
                        new Medic
                        {
                            Name = "Vitalie Andone"
                        },
                        new Medic
                        {
                            Name = "Jora Vartanov"
                        },
                        new Medic
                        {
                            Name = "Benedict Cumberbatch"
                        });
                }

                if (!context.Procedures.Any())
                {
                    context.Procedures.AddRange(
                        new Procedure
                        {
                            Name = "A",
                            Description = "Teeth Removal"
                        },
                        new Procedure
                        {
                            Name = "B",
                            Description = "Teeth whitening"
                        },
                        new Procedure
                        {
                            Name = "C",
                            Description = "Dummy Procedure"
                        },
                        new Procedure
                        {
                            Name = "D",
                            Description = "Dummy Procedure 2"
                        },
                        new Procedure
                        {
                            Name = "E",
                            Description = "Dummy Procedure 3"
                        });
                    context.SaveChanges();
                }
            }
        }
        public DbSet<Medic> Medics { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
    }
}
