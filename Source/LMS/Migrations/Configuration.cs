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
    using LMS.Models;

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

            var roleNames = new[] { "Student", "Teacher" };

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

            var courses = new[]
            {
                new CourseModel
                {
                    Name = ".NET-utbildning Hösten 2017",
                    Description = "MVC.NET kurs",
                    StartDate = new DateTime(2017, 08, 28),
                    EndDate = new DateTime(2017, 12, 15)
                }
            };

            foreach (var course in courses)
            {
                if (context.Courses.Any(c => c.Name == course.Name)) continue;

                context.Courses.AddOrUpdate(course);
            }

            // Week 1

            var modules = new[]
            {
                new ModuleModel
                {
                    Name = "Intro E-L 1.1, 1.2",
                    Description = "Kurs introduktion och E-learning kapitel 1.1, 1.2",
                    StartDate = new DateTime(2017, 8 , 28,  13, 00, 00),
                    EndDate =  new DateTime(2017, 8 , 28, 17, 00, 00)
                },
                new ModuleModel
                {
                    Name = "E-L 1.3",
                    Description = "E-Learning kapitel 1.3",
                    StartDate = new DateTime(2017, 8 , 29,  8, 30, 00),
                    EndDate =  new DateTime(2017, 8 , 29, 12, 00, 00)
                },
                new ModuleModel
                {
                    Name = "E-L 1.4, 1.5",
                    Description = "E-Learning kapitel 1.4, 1.5",
                    StartDate = new DateTime(2017, 8 , 29,  13, 00, 00),
                    EndDate =  new DateTime(2017, 8 , 29, 17, 00, 00)
                },
                new ModuleModel
                {
                    Name = "C# Intro",
                    Description = "Introduktion till C#",
                    StartDate = new DateTime(2017, 8 , 30, 8, 30, 00),
                    EndDate =  new DateTime(2017, 8 , 30, 12, 00, 00)
                },
                new ModuleModel
                {
                    Name = "C# Intro",
                    Description = "Introduktion till C#",
                    StartDate = new DateTime(2017, 8 , 30, 13, 00, 00),
                    EndDate =  new DateTime(2017, 8 , 30, 17, 00, 00)
                },
                new ModuleModel
                {
                    Name = "Övning 2",
                    StartDate = new DateTime(2017, 8 , 31, 8, 30, 00),
                    EndDate =  new DateTime(2017, 8 , 31, 12, 00, 00)
                },
                new ModuleModel
                {
                    Name = "Övning 2",
                    StartDate = new DateTime(2017, 8 , 31, 13, 00, 00),
                    EndDate =  new DateTime(2017, 8 , 31, 17, 00, 00)
                },
                new ModuleModel
                {
                    Name = "C# Grund",
                    StartDate = new DateTime(2017, 9 , 1, 8, 30, 00),
                    EndDate =  new DateTime(2017, 9 , 1, 12, 00, 00)
                },
                new ModuleModel
                {
                    Name = "C# Grund",
                    StartDate = new DateTime(2017, 9 , 1, 13, 00, 00),
                    EndDate =  new DateTime(2017, 9 , 1, 17, 00, 00)
                }


                // Week 2

                // TO DO
            };

            foreach (var module in modules)
            {
                if (context.Modules.Any(m => m.Name == module.Name)) continue;

                context.Modules.AddOrUpdate(module);
            }

        }
    }
}
