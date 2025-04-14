# Youtube Data API Client

## Architecture

This project follows the principles of **Clean Architecture** as described by *Robert C. Martin (Uncle Bob)*. I chose this architecture to separate concerns and enforce clear dependency rules between layers. The business logic is placed at the center of the architecture and remains independent of frameworks, databases, and UI technologies. For simpicity reasons i chose not to put tools like persistence and file-export into dedicated projects for independant dependency management. Having a single project for each layer keeps it simple - wich fits just fine for this simple app.

### Project Structure

```
📁 YoutubeAnalyzer
├── 📁 YoutubeAnalyzer.Domain               # Core business logic, entities, and value objects
├── 📁 YoutubeAnalyzer.Application          # Use cases, DTOs, and interfaces
├── 📁 YoutubeAnalyzer.Infrastructure       # External concerns like database access and APIs
├── 📁 YoutubeAnalyzer.Web                  # Blazor Server frontend (UI)
├── 📁 YoutubeAnalyzer.Tests                # Unit and integration tests
```

### Architectural Decisions

- **Domain Layer** is the innermost layer and holds all business rules, entities, and logic that are independent of any technical concerns.
- **Application Layer** defines use cases and application-specific logic. It orchestrates flows and uses interfaces to abstract infrastructure.
- **Infrastructure Layer** implements interfaces defined in Domain or Application and handles external dependencies like database access and third-party APIs (e.g., YouTube API).
- **Web Layer** contains the Blazor Server application and handles UI logic. It communicates only with the Application and Domain layers.
- Dependencies point *inward*, and outer layers have no influence on inner layers. This allows for high testability, loose coupling, and long-term maintainability.

This layered approach provides a clean separation of concerns, easy testability, and flexibility for future changes such as switching database technologies or exposing new UI frontends.
