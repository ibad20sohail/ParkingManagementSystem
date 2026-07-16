namespace CodeGenerator.Constants
{
    public static class Cons
    {
        public const string ConnectionString = "Server=DESKTOP-AT7A4NQ\\SQLEXPRESS;Database=parking_management_system;Trusted_Connection=True;TrustServerCertificate=True;";

        public const string Add = "Add";
        public const string Edit = "Edit";
        public const string Delete = "Delete";

        public const string RequestModel = "RequstModel";
        public const string ResponseModel = "ResponseModel";

        public const string EntityTemplatePath = @"Templates\Entity.sbn";
        public const string RequestTemplatePath = @"Templates\Request.sbn";
        public const string ResponseTemplatePath = @"Templates\Response.sbn";
        public const string OperationResponseTemplatePath = @"Templates\OperationResponse.sbn";

        public const string EntityGenerationPath = @"PMS.Domain\Entities";
        public const string RequestGenerationPath = @"PMS.Application\Models\Requests";
        public const string ResponseGenerationPath = @"PMS.Application\Models\Responses";
    }
}
