# Youtube Data API Client

## Project Structure

📁 YoutubeApi
├── 📁 YoutubeApi.Presentation         ← UI Components
│   ├── Pages/
│   ├── Components/
│   └── Services/ 					← ViewModel Helper oder Adapter für UseCases
│
├── 📁 YoutubeApi.Application          ← UseCases, Interfaces, DTOs
│   ├── Interfaces/
│   ├── UseCases/
│   └── DTOs/
│
├── 📁 YoutubeApi.Domain               ← Reine Fachlogik
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Enums/
│   ├── Events/
│   └── Interfaces/ 				← z. B. IYoutubeVideoFetcher
│
├── 📁 YoutubeApi.Infrastructure       ← API-Clients, Datenbank, Export
│   ├── YoutubeApi/ ← Dein YoutubeApiClient
│   ├── Export/ ← CSV, Excel, SketchEngine
│   ├── Persistence/ ← EF Core, DbContext
│   └── Mappers/
│
├── 📁 YoutubeApi.Tests				← Unittests
│
├── YoutubeApi.sln
