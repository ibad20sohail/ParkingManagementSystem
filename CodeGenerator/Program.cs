using CodeGenerator.Database;
using CodeGenerator.Generators;
using CodeGenerator.Helpers;

string connectionString = "Server=DESKTOP-AT7A4NQ\\SQLEXPRESS;Database=ParkingManagementSystem;Trusted_Connection=True;TrustServerCertificate=True;";

var reader = new SqlReader(connectionString);

var tables = await reader.GetTableMetadataAsync();

var entityPath = Path.GetFullPath(@"Templates\Entity.sbn");
var entityGenerator = new EntityGenerator(entityPath);

var solutionRoot = SolutionFinder.FindRoot();
var output = Path.Combine(solutionRoot, "PMS.Domain", "Entities");

await entityGenerator.GenerateAsync(tables, output);

var procedures = await reader.GetStoredProceduresAsync();

Console.WriteLine($"Procedures: {procedures.Count}");

foreach (var procedure in procedures)
{
    Console.WriteLine(procedure);
}
var parameters = await reader.GetProcedureParametersAsync("AddRole");

foreach (var parameter in parameters)
{
    Console.WriteLine($"{parameter.Name} - {parameter.SqlType}");
}
var result = await reader.GetProcedureResultColumnsAsync("AddRole");

Console.WriteLine("Result Columns");

foreach (var column in result)
{
    Console.WriteLine($"{column.Name} - {column.SqlType}");
}
Console.WriteLine("Entities Generated!");