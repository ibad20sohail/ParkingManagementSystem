namespace PMS.CodeGenerator.Helpers
{
    public static class SolutionFinder
    {
        public static string FindRoot()
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

            while (directory != null)
            {
                var solution = directory.GetFiles("*.slnx").FirstOrDefault();

                if (solution != null)
                    return directory.FullName;

                directory = directory.Parent;
            }

            throw new Exception("Solution file not found");
        }
    }
}
