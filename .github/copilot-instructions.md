# Code Style and Quality Standards

## General Principles
- **Clarity over Brevity:** Prefer obvious and descriptive names for methods, variables, and classes, even if they are verbose. Avoid acronyms or ambiguous abbreviations.
- **Language:** Always write code (classes, methods, variables, and comments) in English.
- **Explicit Typing:** Always use the specific type (e.g., `int`, `string`, `List<User>`). **DO NOT** use the `var` keyword.

## C# Syntax and Formatting
- **Braces:** Always start braces on a new line (Allman style).
- **Naming Conventions:**
    - **Methods:** Use PascalCase. Names must be action-oriented and explicit (e.g., `GetActiveUsersByLastLoginDate` instead of `GetUsers`).
    - **Variables/Parameters:** Use camelCase. Names must describe the content clearly (e.g., `remainingRetryAttempts` instead of `retries`).
    - **Interfaces:** Always prefix with `I` (e.g., `IUserRepository`).
- **Namespaces:** Use file-scoped namespaces to reduce indentation.

## Architecture and Patterns (CQRS & Clean Architecture)
- **Commands/Queries:** Every Command must have a dedicated Handler.
- **Immutability:** Use `record` for DTOs, Commands, and Queries.
- **Logic Location:** Business logic must reside in Domain Services or Entities, never in the Controllers.
- **Validation:** Use Fail-Fast principle. Validate commands before they reach the Handler logic.

## Testing Standards
- **Framework:** Use xUnit.v3 and Moq.
- **Pattern:** Follow the `Arrange_Act_Assert` naming convention for test methods.
- **Assertion:** Use `FluentAssertions` for better readability.
- **Isolation:** Unit tests must mock all external dependencies (Database, APIs, Message Brokers).

## Error Handling
- **Exceptions:** Do not use exceptions for expected business flow. Use a `Result` or `Notification` pattern.
- **Logging:** Always log relevant business context when an error occurs, but never log sensitive user data.

## Architecture Context
- **Pattern:** CQRS (Command Query Responsibility Segregation).
- **Core Layers:**
    - **Domain:** Entities, Value Objects, and Domain Events. No dependencies.
    - **Application:** Commands, Queries, Handlers, and DTOs. Uses MediatR/MassTransit.
    - **Infrastructure:** EF Core Mappings, Repositories, and External Service implementations.
    - **API:** Controllers only delegate work to the Application layer.
- **Messaging:** We use MassTransit for asynchronous communication and Service Bus patterns.
- **Database:** Entity Framework Core with a focus on Repository and Unit of Work patterns.

## Development Workflow for New Endpoints
When creating a new feature or endpoint, always follow this structural pattern:

1. **Domain Layer:** - Define Entities or Value Objects first if they don't exist.
2. **Application Layer (CQRS):**
   - Create a folder named after the Feature (e.g., `Features/Auctions/CreateBid`).
   - Inside that folder, create:
     - `CreateBidCommand.cs` (as a `public sealed record`).
     - `CreateBidCommandHandler.cs` (implementing `IRequestHandler`).
     - `CreateBidValidator.cs` (using FluentValidation).
3. **Infrastructure Layer:**
   - Update the Repository or Unit of Work if new data access is required.
4. **API Layer:**
   - Create/Update the Controller. 
   - Methods must only call `_mediator.Send(command)`.
   - Use proper HTTP Status Codes (201 Created, 204 No Content, 400 BadRequest).
   
# Testing Strategy & Blueprint

## Project Structure
- **Project.Tests.Common:** Contains all `Builders` and `Fakers`. This project is referenced by both Unit and Integration tests.
- **Project.Tests.Unit:** Focuses on Domain Entities, Value Objects, and CommandHandlers (with Mocks).
- **Project.Tests.Integration:** Focuses on API Endpoints, Database Persistence (EF Core), and Service Bus (MassTransit) using real or containerized dependencies.

## Naming Conventions
- **Test Files:** Must match the class name followed by `Tests` (e.g., `AuctionTests.cs`).
- **Test Methods:** Use the `Action_Should_ExpectedResult` pattern (e.g., `PlaceBid_Should_UpdateCurrentPrice_WhenBidIsValid`).
- **Builders:** Use the `EntityNameBuilder` pattern (e.g., `AuctionBuilder.cs`).

## Test Data Builders (The Builder Pattern)
Always prefer using Builders located in the `Common` project to instantiate Domain Entities.
- **Method naming:** Use `With` prefix for properties (e.g., `.WithStatus()`, `.WithEndDate()`).
- **Default values:** The `.Build()` method must return a valid entity with consistent default data.

## Unit Testing Blueprint (xUnit + FluentAssertions)
```csharp
public class FeatureNameTests
{
    [Fact]
    public void Method_Should_Behavior_When_Condition()
    {
        // Arrange
        // Use Builders for Entities. Use specific types, NO 'var'.
        Auction auction = new AuctionBuilder()
            .WithStatus(AuctionStatus.OPEN)
            .Build();
        decimal bidAmount = 150m;

        // Act
        var (bool success, string errorMessage) = auction.CanPlaceBid(bidAmount);

        // Assert
        success.Should().BeTrue();
        errorMessage.Should().BeEmpty();
    }
}