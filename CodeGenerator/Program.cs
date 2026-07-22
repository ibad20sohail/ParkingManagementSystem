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
var appResponsePath = Path.GetFullPath(Cons.AppResponseTemplatePath);
var responseGenerator = new ResponseGenerator(requestPath, operationResponsePath, appResponsePath);
var outputForResponse = Path.Combine(solutionRoot, Cons.ResponseGenerationPath);
var outputForCommonResponse = Path.Combine(solutionRoot, Cons.CommonResponseGenerationPath);
await responseGenerator.GenerateAsync(procedures, outputForResponse, outputForCommonResponse);

var repoInterfacePath = Path.GetFullPath(Cons.RepositoryInterfaceTemplatePath);
var repoInterfaceGenerator = new RepositoryInterfaceGenerator(repoInterfacePath);
var outputForRepoInterface = Path.Combine(solutionRoot, Cons.RepositoryInterfaceGenerationPath);
await repoInterfaceGenerator.GenerateAsync(procedures, outputForRepoInterface);

var repoImplementationPath = Path.GetFullPath(Cons.RepositoryImplementationTemplatePath);
var repoImplementationGenerator = new RepositoryImplementationGenerator(repoImplementationPath);
var outputForRepoImplementation = Path.Combine(solutionRoot, Cons.RepositoryImplementationGenerationPath);
await repoImplementationGenerator.GenerateAsync(procedures, outputForRepoImplementation);

var repoDIPath = Path.GetFullPath(Cons.DependencyInjectionTemplatePath);
var repoDIGenerator = new DependencyInjectionGenerator(repoDIPath);
var outputForDI = Path.Combine(solutionRoot, Cons.DependencyInjectionGenerationPath);
await repoDIGenerator.GenerateAsync(procedures, outputForDI);

