using SoftUni.Data;
using SoftUni.Models;
using System.Globalization;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            using SoftUniContext context = new SoftUniContext();

            Console.WriteLine(GetEmployeesByFirstNameStartingWithSa(context));
        }


        //03. Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .OrderBy(e => e.EmployeeId)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //04. Employees with Salary Over 50 000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Salary > 50000)
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //05. Employees from Research and Development
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.Salary,
                    e.FirstName,
                    e.LastName,
                    e.Department.Name
                })
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.Name} - ${e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //06. Adding a New Address and Updating Employee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Addresses.Add(address);

            Employee employee = context.Employees
                .Where(e => e.LastName == "Nakov")
                .FirstOrDefault()!;

            employee!.Address = address;
            context.SaveChanges();

            string[] employeesAddressTexts = context.Employees
                .OrderByDescending(x => x.AddressId)
                .Take(10)
                .Select(x => (string)x.Address!.AddressText)
                .ToArray();

            return String.Join(Environment.NewLine, employeesAddressTexts);
        }

        //07. Employees and Projects
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager!.FirstName,
                    ManagerLastName = e.Manager.LastName,
                    Projects = e.EmployeesProjects.Where(ep => ep.Project.StartDate.Year >= 2001 & ep.Project.StartDate.Year <= 2003)
                        .Select(ep => new
                        {
                            ProjectName = ep.Project.Name,
                            StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                            EndDate = ep.Project.EndDate != null
                                ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                                : "not finished"
                        })
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");

                if (e.Projects.Any())
                {
                    sb.AppendLine(String.Join(Environment.NewLine, e.Projects
                        .Select(p => $"--{p.ProjectName} - {p.StartDate} - {p.EndDate}")));
                }
            }

            return sb.ToString().TrimEnd();
        }

        //08. Addresses by Town
        public static string GetAddressesByTown(SoftUniContext context)
        {
            string[] addressesInfo = context.Addresses
                .OrderByDescending(a => a.Employees.Count)
                .ThenBy(a => a.Town!.Name)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .Select(a => $"{a.AddressText}, {a.Town!.Name} - {a.Employees.Count} employees")
                .ToArray();

            return string.Join(Environment.NewLine, addressesInfo);
        }

        //09. Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    Projects = e.EmployeesProjects
                               .Where(ep => ep.EmployeeId == 147)
                               .Select(ep => new
                               {
                                   ep.Project.Name
                               })
                               .OrderBy(p => p.Name)
                               .ToArray()
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employee)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");

                foreach (var p in e.Projects)
                {
                    sb.AppendLine($"{p.Name}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //10. Departments with More Than 5 Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    d.Name,
                    d.Manager.FirstName,
                    d.Manager.LastName,
                    Employees = d.Employees
                                .Select(e => new
                                {
                                    e.FirstName,
                                    e.LastName,
                                    e.JobTitle
                                })
                                .OrderBy(e => e.FirstName)
                                .ThenBy(e => e.LastName)
                                .ToArray()
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var d in departments)
            {
                sb.AppendLine($"{d.Name} - {d.FirstName}  {d.LastName}");
                foreach (var e in d.Employees)
                {
                    sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //11. Find Latest 10 Projects
        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    Date = p.StartDate.ToString("M/d/yyyy h:mm:ss tt")
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var p in projects)
            {
                sb.AppendLine($"{p.Name}");
                sb.AppendLine($"{p.Description}");
                sb.AppendLine($"{p.Date}");
            }

            return sb.ToString().TrimEnd();
        }

        //12. Increase Salaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Engineering" ||
                            e.Department.Name == "Tool Design" ||
                            e.Department.Name == "Marketing" ||
                            e.Department.Name == "Information Services")
                .ToArray();

            foreach (var e in employees)
            {
                decimal n = e.Salary * 0.12M;
                e.Salary += n;
            }

            context.SaveChanges();

            var list = employees
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var e in list)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }

        //13. Find Employees by First Name Starting With Sa
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.FirstName.StartsWith("Sa"))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Salary,
                    e.JobTitle
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})");
            }

            return sb.ToString().TrimEnd();

        }

        //14. Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
            var employeesProjectsToDelete = context.EmployeesProjects.Where(ep => ep.ProjectId == 2);
            context.EmployeesProjects.RemoveRange(employeesProjectsToDelete);

            var projectToDelete = context.Projects.Where(p => p.ProjectId == 2);
            context.Projects.RemoveRange(projectToDelete);

            context.SaveChanges();

            string[] projectsNames = context.Projects
                .Take(10)
                .Select(p => p.Name)
                .ToArray();

            return string.Join(Environment.NewLine, projectsNames);
        }

        //15. Remove Town
        public static string RemoveTown(SoftUniContext context)
        {
            Town townToDelete = context.Towns
                .Where(t => t.Name == "Seattle")
                .FirstOrDefault()!;

            Address[] addressesToDelete = context.Addresses
                .Where(a => a.TownId == townToDelete.TownId)
                .ToArray();

            Employee[] employeesToRemoveAddressFrom = context.Employees
                .Where(e => addressesToDelete
                .Contains(e.Address))
                .ToArray();

            foreach (Employee e in employeesToRemoveAddressFrom)
            {
                e.AddressId = null;
            }

            context.Addresses.RemoveRange(addressesToDelete);
            context.Towns.Remove(townToDelete);
            context.SaveChanges();

            return $"{addressesToDelete.Count()} addresses in Seattle were deleted";
        }
    }
}