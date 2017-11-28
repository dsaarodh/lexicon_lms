namespace LMS.Migrations
{
	using LMS.Models.Identity;
	using Microsoft.AspNet.Identity;
	using Microsoft.AspNet.Identity.EntityFramework;
	using System;
	using System.Data.Entity.Migrations;
	using System.Linq;
	using LMS.DataAccess;
	using LMS.Controllers;
	using LMS.Models.AppData;


	internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "LMS.DataAccess.ApplicationDbContext";
        }

		protected override void Seed(ApplicationDbContext context)
		{
#if DEBUG
//			if (!System.Diagnostics.Debugger.IsAttached)
//				System.Diagnostics.Debugger.Launch();
#endif // DEBUG

			var lmsSeedData = CreateLMSSeedData();

			// Seed ActivityType
			foreach (var activityType in lmsSeedData.ActivityTypes)
				context.ActivityTypes.AddOrUpdate(at => at.Name, activityType);
			context.SaveChanges();

			// Seed Course
			foreach (var course in lmsSeedData.Courses)
				context.Courses.AddOrUpdate(c => c.Name, course);
			context.SaveChanges();

			// Seed Module
			foreach (var module in lmsSeedData.Modules)
				context.Modules.AddOrUpdate(m => m.Name, module);
			context.SaveChanges();

			// Seed Activity
			foreach (var activity in lmsSeedData.Activities)
				context.Activities.AddOrUpdate(a => a.Name, activity);
			context.SaveChanges();

			SeedIdentities(context, lmsSeedData.Courses);

		}

		private (Course[] Courses, Module[] Modules, ActivityType[] ActivityTypes, Activity[] Activities) CreateLMSSeedData()
		{
			var activityTypes = new[]
			{
				new ActivityType { Name = "E-learning" },
				new ActivityType { Name = "Lecture" },
				new ActivityType { Name = "Workshop" },
				new ActivityType { Name = "Assignment" }
			};

			var activities = new[]
			{
				new Activity
				{
					Name = "C# Intro 1",
					Description = "Introduktion till C#",
					StartDate = new DateTime(2017, 8 , 30, 8, 30, 00),
					EndDate =  new DateTime(2017, 8 , 30, 12, 00, 00),
				//	Module = modules[0],
					ActivityType = activityTypes[0]
				},
				new Activity
				{
					Name = "C# Intro 2",
					Description = "Introduktion till C#",
					StartDate = new DateTime(2017, 8 , 30, 13, 00, 00),
					EndDate =  new DateTime(2017, 8 , 30, 17, 00, 00),
				//	Module = modules[0],
					ActivityType = activityTypes[0]
				},
				new Activity
				{
					Name = "C# Intro 3",
					Description = "E-learning kapitel 1.1, 1.2",
					StartDate = new DateTime(2017, 9 , 4,  13, 00, 00),
					EndDate =  new DateTime(2017, 9 , 4, 17, 00, 00),
				//	Module = modules[0],
					ActivityType = activityTypes[0]
				},
				new Activity
				{
					Name = "C# Intro 4",
					Description = "E-Learning kapitel 1.3",
					StartDate = new DateTime(2017, 9 , 6,  8, 30, 00),
					EndDate =  new DateTime(2017, 9 , 6, 12, 00, 00),
				//	Module = modules[0],
					ActivityType = activityTypes[0]
				},
				new Activity
				{
					Name = "C# Intro 5",
					Description = "E-Learning kapitel 1.4, 1.5",
					StartDate = new DateTime(2017, 9 , 12,  13, 00, 00),
					EndDate =  new DateTime(2017, 9 , 12, 17, 00, 00),
				//	Module = modules[0],
					ActivityType = activityTypes[0]
				},
				new Activity
				{
					Name = "Övning 2-1",
					Description = "Bygg ett garage för fordon",
					StartDate = new DateTime(2017, 9 , 15, 8, 30, 00),
					EndDate =  new DateTime(2017, 9 , 15, 12, 00, 00),
				//	Module = modules[1],
					ActivityType = activityTypes[2]
				},
				new Activity
				{
					Name = "Övning 2-2",
					Description = "Bygg ett garage för fordon",
					StartDate = new DateTime(2017, 9 , 24, 13, 00, 00),
					EndDate =  new DateTime(2017, 9 , 24, 17, 00, 00),
				//	Module = modules[1],
					ActivityType = activityTypes[2]
				},
				new Activity
				{
					Name = "C# Grund 1",
					Description = "Klasser och arv",
					StartDate = new DateTime(2017, 10 , 1, 8, 30, 00),
					EndDate =  new DateTime(2017, 10 , 1, 12, 00, 00),
				//	Module = modules[1],
					ActivityType = activityTypes[2]
				},
				new Activity
				{
					Name = "C# Grund 2",
					Description = "Klasser och arv",
					StartDate = new DateTime(2017, 10, 11, 13, 00, 00),
					EndDate =  new DateTime(2017, 10, 11, 17, 00, 00),
				//	Module = modules[2],
					ActivityType = activityTypes[1]
				},
				new Activity
				{
					Name = "Java introduktion 1",
					Description = "Nybörjarlektion",
					StartDate = new DateTime(2017, 5, 3, 13, 00, 00),
					EndDate =  new DateTime(2017, 5, 3, 17, 00, 00),
				//	Module = modules[3],
					ActivityType = activityTypes[1]
				},
				new Activity
				{
					Name = "Java introduktion 2",
					Description = "Nybörjarlektion fortsättning",
					StartDate = new DateTime(2017, 5, 10, 13, 00, 00),
					EndDate =  new DateTime(2017, 5, 10, 17, 00, 00),
				//	Module = modules[3],
					ActivityType = activityTypes[1]
				},
				new Activity
				{
					Name = "Övning 1-1",
					Description = "Garage övning",
					StartDate = new DateTime(2017, 8, 3, 13, 00, 00),
					EndDate =  new DateTime(2017, 8, 3, 17, 00, 00),
				//	Module = modules[4],
					ActivityType = activityTypes[2]
				},
				new Activity
				{
					Name = "Övning 1-2",
					Description = "Garage övning",
					StartDate = new DateTime(2017, 8, 10, 13, 00, 00),
					EndDate =  new DateTime(2017, 8, 10, 17, 00, 00),
				//	Module = modules[4],
					ActivityType = activityTypes[2]
				},
			};

			var modules = new[]
			{
				new Module
				{
					Name = "C# Intro",
					Description = "Introduktion till C#",
					StartDate = new DateTime(2017, 8 , 30, 00, 00, 00),
					EndDate =  new DateTime(2017, 9 , 14, 00, 00, 00),
					Activities = new[] { activities[0], activities[1], activities[2], activities[3], activities[4] }
				//	Course = courses[0]
				},
				new Module
				{
					Name = "Övning 2",
					Description = "Bygg ett garage för fordon",
					StartDate = new DateTime(2017, 9 , 15, 00, 00, 00),
					EndDate =  new DateTime(2017, 9 , 29, 00, 00, 00),
					Activities = new[] { activities[5], activities[6], activities[7] }
				//	Course = courses[0]
				},
				new Module
				{
					Name = "C# Grund",
					Description = "Klasser och arv",
					StartDate = new DateTime(2017, 10 , 1, 00, 00, 00),
					EndDate =  new DateTime(2017, 10 , 14, 00, 00, 00),
					Activities = new[] { activities[8] }
				//	Course = courses[0]
				},
				new Module
				{
					Name = "Java introduktion",
					Description = "Nybörjarlektion",
					StartDate = new DateTime(2017, 5, 3, 00, 00, 00),
					EndDate =  new DateTime(2017, 5, 17, 00, 00, 00),
					Activities = new[] { activities[9], activities[10] }
				//	Course = courses[1]
				},
				new Module
				{
					Name = "Övning 1",
					Description = "Garage övning",
					StartDate = new DateTime(2017, 8 , 1, 00, 00, 00),
					EndDate =  new DateTime(2017, 8 , 17, 00, 00, 00),
					Activities = new[] { activities[11], activities[12] }
				//	Course = courses[1]
				},
			};

			var courses = new[]
			{
				new Course
				{
					Name = ".NET-utbildning Hösten 2017",
					Description = "MVC.NET kurs",
					StartDate = new DateTime(2017, 08, 28),
					EndDate = new DateTime(2017, 12, 15),
					Modules = new[] { modules[0], modules[1], modules[2] }					
				},
				new Course
				{
					Name = "Java för Pingviner 2017",
					Description = "Grundläggande java utbildning för ungfåglar",
					StartDate = new DateTime(2017, 05, 01),
					EndDate = new DateTime(2017, 12, 20),
					Modules = new[] { modules[3], modules[4] }
				},
			};

			return (courses, modules, activityTypes, activities);
		}

		private void SeedIdentities(ApplicationDbContext context, Course[] courses)
		{
			// Add Roles
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


			// Add students
			var students = new[]
			{
				new ApplicationUser {
					UserName = "bobbygunnar.nilsson@lexicon.se",
					Email = "bobbygunnar.nilsson@lexicon.se",
					FirstName = "Bobby-Gunnar",
					LastName = "Nilsson",
					PersonalIdentityNumber = "19721224-1963",
					Course = courses[0]

				},
				new ApplicationUser {
					UserName = "bengtelvis.andersson@lexicon.se",
					Email = "bengtelvis.andersson@lexicon.se",
					FirstName = "Bengt-Elvis",
					LastName = "Andersson",
					PersonalIdentityNumber = "19901102-2048",
					Course = courses[0]
				},
				new ApplicationUser {
					UserName = "kurtsune.beritsdottir@lexicon.se",
					Email = "kurtsune.beritsdottir@lexicon.se",
					FirstName = "Kurt-Sune",
					LastName = "Beritsdottir",
					PersonalIdentityNumber = "19810719-7098",
					Course = courses[0]
				},
				new ApplicationUser {
					UserName = "moa.karlsson@lexicon.se",
					Email = "moa.karlsson@lexicon.se",
					FirstName = "Moa",
					LastName = "Karlsson",
					PersonalIdentityNumber = "20010125-4785",
					Course = courses[0]
				},
				new ApplicationUser {
					UserName = "marit.haraldsson@lexicon.se",
					Email = "marit.haraldsson@lexicon.se",
					FirstName = "Marit",
					LastName = "Haraldsson",
					PersonalIdentityNumber = "20000403-9046",
					Course = courses[0]
				},
				new ApplicationUser {
					UserName = "beatrice.hansson@lexicon.se",
					Email = "beatrice.hansson@lexicon.se",
					FirstName = "Beatrice",
					LastName = "Hansson",
					PersonalIdentityNumber = "20010706-4769",
					Course = courses[0]
				},
				new ApplicationUser {
					UserName = "svante.vantesson@lexicon.se",
					Email = "svante.vantesson@lexicon.se",
					FirstName = "Svante",
					LastName = "Vantesson",
					PersonalIdentityNumber = "19851004-3756",
					Course = courses[1]
				},
				new ApplicationUser {
					UserName = "camilla.felg@lexicon.se",
					Email = "camilla.felg@lexicon.se",
					FirstName = "Camilla",
					LastName = "Felg",
					PersonalIdentityNumber = "19990822-8459",
					Course = courses[1]
				},
				new ApplicationUser {
					UserName = "pelle.pop@lexicon.se",
					Email = "pelle.pop@lexicon.se",
					FirstName = "Pelle",
					LastName = "Pop",
					PersonalIdentityNumber = "19870228-8563",
					Course = courses[1]
				},
				new ApplicationUser {
					UserName = "chuck.norris@lexicon.se",
					Email = "chuck.norris@lexicon.se",
					FirstName = "Chuck",
					LastName = "Norris",
					PersonalIdentityNumber = "11920523-4748",
					Course = courses[1]
				},
			};

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



			// Add teachers
			var teachers = new[]
			{
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

			context.SaveChanges();
		}
    }
}
