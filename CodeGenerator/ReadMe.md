# PMS Code Generator

The **PMS Code Generator** is a SQL-first scaffolding tool that automatically generates boilerplate code for the Parking Management System from the database schema.

The generator reads SQL Server metadata (tables, stored procedures, and functions) and produces strongly typed C# classes using Scriban templates.

---

## Features

- Generate Entity classes from database tables.
- Generate Request models from stored procedure parameters.
- Generate Response models from stored procedure result sets.
- Generate Repository interfaces.
- Generate Repository implementations using Dapper.
- Generate Dependency Injection registrations.
- Delete orphaned generated files when database objects are removed.
- SQL-first approach with minimal manual coding.

---

## Project Structure

```
PMS.CodeGenerator
│
├── Constants
├── Generators
├── Helpers
├── Mappers
├── MetadataReaders
├── Models
├── Services
├── Templates
│   ├── Entity.sbn
│   ├── RepositoryInterface.sbn
│   ├── RepositoryImplementation.sbn
│   ├── Request.sbn
│   ├── Response.sbn
│   └── RepositoryDependencyInjection.sbn
└── Program.cs
```

---

## Generated Output

The generator creates code in the following projects:

```
PMS.Domain
│
└── Entities

PMS.Application
│
├── Repositories
├── Requests
├── Responses
└── Common

PMS.Infrastructure
│
├── Repositories
└── DependencyInjection
```

---

## Generation Flow

```
SQL Server
     │
     ▼
Read Tables
Read Procedures
Read Functions
     │
     ▼
Build Metadata
     │
     ▼
Repository Mapper
     │
     ▼
Scriban Templates
     │
     ▼
Generated C# Files
```

---

## Database Conventions

### Tables

Tables should use lowercase names with underscores.

Example

```sql
users
roles
parking_spaces
tickets
```

Primary keys

```sql
user_id
role_id
ticket_id
```

---

### Stored Procedures

Naming convention

```
usp_add_user
usp_edit_user
usp_delete_user
usp_get_users
usp_get_user_by_id
usp_login_user
```

---

### Functions

Naming convention

```
fn_get_users
fn_get_roles
fn_get_parking_spaces
```

---

## Request Model Generation

Request models are generated from stored procedure parameters.

Example

Stored Procedure

```sql
CREATE PROCEDURE usp_login_user
(
    @user_name NVARCHAR(100),
    @password NVARCHAR(100)
)
```

Generated

```csharp
public class LoginUserRequest
{
    public string UserName { get; set; }

    public string Password { get; set; }
}
```

---

## Response Model Generation

Response models are generated from the result set.

Example

```sql
SELECT
    user_id,
    user_name,
    role_name
```

Generated

```csharp
public class LoginUserResponse
{
    public int UserId { get; set; }

    public string UserName { get; set; }

    public string RoleName { get; set; }
}
```

---

## Repository Generation

Repository interfaces

```csharp
public interface IUserRepository
{
    Task<LoginUserResponse> LoginUserAsync(LoginUserRequest request);

    Task<List<GetUserResponse>> GetUsersAsync();
}
```

Repository implementations

```csharp
public class UserRepository : IUserRepository
{
    private readonly IDbConnection _connection;

    public UserRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<LoginUserResponse> LoginUserAsync(LoginUserRequest request)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@user_name", request.UserName);
        parameters.Add("@password", request.Password);

        return await _connection.QueryFirstOrDefaultAsync<LoginUserResponse>(
            "usp_login_user",
            parameters,
            commandType: CommandType.StoredProcedure);
    }
}
```

---

## Dependency Injection

Repositories are automatically registered.

Generated example

```csharp
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IRoleRepository, RoleRepository>();
services.AddScoped<ITicketRepository, TicketRepository>();
```

---

## Naming Conventions

### SQL

```
user_name
first_name
parking_space_no
created_date
```

### C#

```
UserName
FirstName
ParkingSpaceNo
CreatedDate
```

The generator automatically converts snake_case into PascalCase.

---

## Type Mapping

| SQL Type | C# Type |
|----------|----------|
| int | int |
| bigint | long |
| bit | bool |
| tinyint | byte |
| decimal | decimal |
| float | double |
| uniqueidentifier | Guid |
| datetime | DateTime |
| date | DateOnly |
| time | TimeOnly |
| nvarchar | string |
| varchar | string |
| text | string |
| char | string |

Nullable SQL types are generated as nullable C# types.

Example

```
int NULL
```

↓

```csharp
int?
```

---

## Automatic Cleanup

Before generating files, the generator checks the output folders.

Files that no longer exist in the database are automatically removed.

Example

```
users table removed
```

↓

```
User.cs deleted
UserRepository.cs deleted
IUserRepository.cs deleted
```

---

## Technologies

- .NET 10
- SQL Server
- Dapper
- Scriban
- Reflection
- ADO.NET Metadata
- Source Generation

---

## Advantages

- SQL is the single source of truth.
- No Entity Framework required.
- Strongly typed repositories.
- Automatic model generation.
- Automatic repository generation.
- Minimal manual coding.
- Consistent project structure.
- Easy to maintain.
- Easy to extend with new templates.

---

## Usage

1. Update the SQL Server database.
2. Add or modify tables, stored procedures, or functions.
3. Run the Code Generator.
4. Build the solution.

Generated code will automatically reflect the latest database changes.

---

## Future Enhancements

- Enum generation
- FluentValidation generation
- Controller generation
- API endpoint generation
- Unit test generation
- DTO mapping generation
- Stored procedure documentation generation
- Incremental generation
- Multi-database support