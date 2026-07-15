using PMS.CodeGenerator.Database;
using PMS.CodeGenerator.Generators;
using PMS.CodeGenerator.Helpers;

string connectionString = "Server=DESKTOP-AT7A4NQ\\SQLEXPRESS;Database=ParkingManagementSystem;Trusted_Connection=True;TrustServerCertificate=True;";

var reader = new SqlReader(connectionString);

var tables = await reader.GetTableMetadataAsync();

var entityPath = Path.GetFullPath(@"Templates\Entity.sbn");
var entityGenerator = new EntityGenerator(entityPath);

var solutionRoot = SolutionFinder.FindRoot();
var output = Path.Combine(solutionRoot, "PMS.Domain", "Entities");

await entityGenerator.GenerateAsync(tables, output);



Console.WriteLine("Entities Generated!");