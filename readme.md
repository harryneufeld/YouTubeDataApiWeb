# Youtube Data API Client
This is a web application designed to extract and analyze metadata and comments from YouTube videos. 
It allows users to fetch structured data using the official YouTube Data API, store it in a database, 
and export it in various formats for further analysis.

This app is built on the latest **.NET 9** platform and leverages **Blazor Server** to deliver a highly interactive and responsive user experience. 
Blazor Server maintains a persistent SignalR connection between client and server, enabling **real-time updates**, **streamed content rendering**, and **low-latency interactivity** 
without requiring full page reloads.
All fetched data is being stored in a **lightweight, file-based SQLite database**, making local setups fast and portable.

## Key Features
- [x] Fetch video metadata and all available comments from YouTube
- [x] Store structured video and comment data in a persistent database
- [x] Export data in multiple formats:
  - [x] **CSV** for spreadsheet tools
  - [x] **Excel (.xlsx)** for advanced editing
  - [x] **Sketch Engine XML** for corpus linguistics and textual analysis
- [x] Built with Clean Architecture principles for maintainability and testability
- [x] Includes a modern Blazor Server UI for an interactive and responsive experience

This app is made for researchers, analysts, and content professionals who need structured access to YouTube data for qualitative or quantitative analysis.

## Technologies Used
- **.NET 9** with **C#**
- **Blazor Server** with reactive, streamed UI via SignalR
- **Entity Framework Core** with **SQLite** as the default database
- **YouTube Data API v3** for fetching video metadata and comments
- **Docker** & **Docker Compose** for containerized fast & simple deployment
- Follows **Clean Architecture** and **Domain-Driven Design (DDD)** principles
- Entirely built on **open source software**

## Getting Started

You can run this app locally using Docker, on a web server with asp.net-core 9 or windows running the exe or with iis. An example `docker-compose.yml` is included in the repository to get you up and running quickly.

### Run with Docker

1. **Clone the repository:**

   ```bash
   git clone https://github.com/harryneufeld/YouTubeDataApiWeb
   ```

2. **Start the application:**

   ```bash
   docker-compose up -d
   ```

3. **Set correct volume permissions** (optional, required for mounted volumes):

   ```bash
   # Get the container ID
   docker ps

   # Check the user ID used in the container
   docker exec -it <container-id> id
   # or by container name
   docker exec -it youtubeapiweb id

   # Set file permissions for the data directory (example UID: 1654)
   sudo chown -R 1337:1337 /var/youtubeapi/data
   ```

> 🔗 **Live Demo:** You can try out a working instance at  
> [youtube.api.harry-neufeld.de](https://youtube.api.harry-neufeld.de)

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
