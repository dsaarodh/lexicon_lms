namespace LMS.Migrations
{
    using LMS.Models.Identity;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
	using LMS.DataAccess;
    using LMS.Controllers;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "LMS.DataAccess.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            var roleNames = new[] { Role.Student, Role.Teacher };

            foreach (var roleName in roleNames)
            {
                if (context.Roles.Any(r => r.Name == roleName)) continue;

                var role = new IdentityRole { Name = roleName };
                var result = roleManager.Create(role);
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join("\n", result.Errors));
                }
            }

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var students = new[] {
                new ApplicationUser {
                    UserName = "bobbygunnar.nilsson@lexicon.se",
                    Email = "bobbygunnar.nilsson@lexicon.se",
                    FirstName = "Bobby-Gunnar",
                    LastName = "Nilsson",
                    PersonalIdentityNumber = "19721224-1963"
                },
                new ApplicationUser {
                    UserName = "bengtelvis.andersson@lexicon.se",
                    Email = "bengtelvis.andersson@lexicon.se",
                    FirstName = "Bengt-Elvis",
                    LastName = "Andersson",
                    PersonalIdentityNumber = "19901102-2048"
                },
                new ApplicationUser {
                    UserName = "kurtsune.beritsdottir@lexicon.se",
                    Email = "kurtsune.beritsdottir@lexicon.se",
                    FirstName = "Kurt-Sune",
                    LastName = "Beritsdottir",
                    PersonalIdentityNumber = "19810719-7098"
                },
                new ApplicationUser {
                    UserName = "moa.karlsson@lexicon.se",
                    Email = "moa.karlsson@lexicon.se",
                    FirstName = "Moa",
                    LastName = "Karlsson",
                    PersonalIdentityNumber = "20010125-4785"
                },
                new ApplicationUser {
                    UserName = "marit.haraldsson@lexicon.se",
                    Email = "marit.haraldsson@lexicon.se",
                    FirstName = "Marit",
                    LastName = "Haraldsson",
                    PersonalIdentityNumber = "20000403-9046"
                },
                new ApplicationUser {
                    UserName = "beatrice.hansson@lexicon.se",
                    Email = "beatrice.hansson@lexicon.se",
                    FirstName = "Beatrice",
                    LastName = "Hansson",
                    PersonalIdentityNumber = "20010706-4769"
                },
                new ApplicationUser {
                    UserName = "svante.vantesson@lexicon.se",
                    Email = "svante.vantesson@lexicon.se",
                    FirstName = "Svante",
                    LastName = "Vantesson",
                    PersonalIdentityNumber = "19851004-3756"
                },
                new ApplicationUser {
                    UserName = "camilla.felg@lexicon.se",
                    Email = "camilla.felg@lexicon.se",
                    FirstName = "Camilla",
                    LastName = "Felg",
                    PersonalIdentityNumber = "19990822-8459"
                },
                new ApplicationUser {
                    UserName = "pelle.pop@lexicon.se",
                    Email = "pelle.pop@lexicon.se",
                    FirstName = "Pelle",
                    LastName = "Pop",
                    PersonalIdentityNumber = "19870228-8563"
                },
                new ApplicationUser {
                    UserName = "chuck.norris@lexicon.se",
                    Email = "chuck.norris@lexicon.se",
                    FirstName = "Chuck",
                    LastName = "Norris",
                    PersonalIdentityNumber = "11920523-4748"
                },
            };

            // Add students
            foreach (var student in students)
            {
                if (context.Users.Any(u => u.UserName == student.Email)) continue;

                var result = userManager.Create(student, "sommar");
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join("\n", result.Errors));
                }

                var user = userManager.FindByName(student.UserName);
                userManager.AddToRole(user.Id, "Student");
            }


            var teachers = new[] {
                new ApplicationUser {
                    UserName = "lorenzo.larsson@lexicon.se",
                    Email = "lorenzo.larsson@lexicon.se",
                    FirstName = "Lorenzo",
                    LastName = "Larsson",
                    PersonalIdentityNumber = "19640913-2029"
                },
                new ApplicationUser {
                    UserName = "Lennart.Gundesson@lexicon.se",
                    Email = "Lennart.Gundesson@lexicon.se",
                    FirstName = "Lennart",
                    LastName = "Gundesson",
                    PersonalIdentityNumber = "19700515-4096"
                },
                new ApplicationUser {
                    UserName = "marika.pilgrimsdotter@lexicon.se",
                    Email = "marika.pilgrimsdotter@lexicon.se",
                    FirstName = "Marika",
                    LastName = "Pilgrimsdotter",
                    PersonalIdentityNumber = "19820308-4649"
                },
                new ApplicationUser {
                    UserName = "knut.vanhelsing@lexicon.se",
                    Email = "knut.vanhelsing@lexicon.se",
                    FirstName = "Knut",
                    LastName = "van Helsing",
                    PersonalIdentityNumber = "19811008-9235"
                }
            };

            // Add teachers
            foreach (var teacher in teachers)
            {
                if (context.Users.Any(u => u.UserName == teacher.Email)) continue;

                var result = userManager.Create(teacher, "sommar");
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join("\n", result.Errors));
                }

                var adminUser = userManager.FindByName(teacher.UserName);
                userManager.AddToRole(adminUser.Id, "Teacher");
            }

        }
    }
}
