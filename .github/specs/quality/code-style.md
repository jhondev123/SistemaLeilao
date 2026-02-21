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
- **Framework:** Use xUnit and Moq.
- **Pattern:** Follow the `Given_When_Then` or `Arrange_Act_Assert` naming convention for test methods.
- **Assertion:** Prefer `FluentAssertions` for better readability.
- **Isolation:** Unit tests must mock all external dependencies (Database, APIs, Message Brokers).

## Error Handling
- **Exceptions:** Do not use exceptions for expected business flow. Use a `Result` or `Notification` pattern.
- **Logging:** Always log relevant business context when an error occurs, but never log sensitive user data.