namespace CodeGenerator.Constants
{
    public static class Cons
    {
        public const string ConnectionString = "Server=DESKTOP-AT7A4NQ\\SQLEXPRESS;Database=parking_management_system;Trusted_Connection=True;TrustServerCertificate=True;";

        public const string Add = "Add";
        public const string Edit = "Edit";
        public const string Delete = "Delete";

        public const string Request = "Request";
        public const string Response = "Response";
        public const string Operation = "Operation";
        public const string Repository = "Repository";

        public const string Async = "Async";

        public const string EntityTemplatePath = @"Templates\Entity.sbn";
        public const string RequestTemplatePath = @"Templates\Request.sbn";
        public const string ResponseTemplatePath = @"Templates\Response.sbn";
        public const string OperationResponseTemplatePath = @"Templates\OperationResponse.sbn";
        public const string RepositoryInterfaceTemplatePath = @"Templates\RepositoryInterface.sbn";
        public const string RepositoryImplementationTemplatePath = @"Templates\RepositoryImplementation.sbn";

        public const string EntityGenerationPath = @"PMS.Domain\Entities";
        public const string RequestGenerationPath = @"PMS.Application\Models\Requests";
        public const string ResponseGenerationPath = @"PMS.Application\Models\Responses";
        public const string RepositoryInterfaceGenerationPath = @"PMS.Application\IRepositories";
        public const string RepositoryImplementationGenerationPath = @"PMS.Infrastructure\Repositories";
    }
}
