namespace P01_StudentSystem
{
    using P01_StudentSystem.Data;

    public class StartUp
    {
        static void Main()
        {
            using StudentSystemContext context = new StudentSystemContext();

            context.Database.EnsureCreated();
        }
    }
}