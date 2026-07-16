using CodeGenerator.Constants;
using CodeGenerator.Database;
using CodeGenerator.Generators;
using CodeGenerator.Helpers;

var solutionRoot = SolutionFinder.FindRoot();

var reader = new SqlReader(Cons.ConnectionString);

var tables = await reader.GetTableMetadataAsync();

var entityPath = Path.GetFullPath(Cons.EntityTemplatePath);
var entityGenerator = new EntityGenerator(entityPath);
var outputFoEntity = Path.Combine(solutionRoot, Cons.EntityGenerationPath);
await entityGenerator.GenerateAsync(tables, outputFoEntity);

var procedures = await reader.GetProceduresMetadataAsync();

var requestPath = Path.GetFullPath(Cons.RequestTemplatePath);
var requestGenerator = new RequestGenerator(requestPath);
var outputForRequest = Path.Combine(solutionRoot, Cons.RequestGenerationPath);
await requestGenerator.GenerateAsync(procedures, outputForRequest);

var responsePath = Path.GetFullPath(Cons.ResponseTemplatePath);
var operationResponsePath = Path.GetFullPath(Cons.OperationResponseTemplatePath);
var responseGenerator = new ResponseGenerator(requestPath, operationResponsePath);
var outputForResponse = Path.Combine(solutionRoot, Cons.ResponseGenerationPath);
await responseGenerator.GenerateAsync(procedures, outputForResponse);