# TV Show Tracker - Full Stack Application

A full-stack TV show tracking application built with React (TypeScript), .NET 8, PostgreSQL, and Redis.

## Tech Stack

### Backend

- .NET 8
- PostgreSQL 16
- Redis 7
- Entity Framework Core
- JWT Authentication

### Frontend

- React 18
- TypeScript
- Vite
- TanStack Query (React Query)
- Tailwind CSS
- shadcn/ui
- Framer Motion

### Prerequisites

- Docker & Docker Compose
- Git

### Running with Docker (Recommended)

1. **Clone the repository**

```bash
   git clone https://github.com/jp4900/tvshowtracker.git
   cd tvshowtracker
```

2. **Start all services**

```bash
   docker-compose up --build
```

3. **Access the application**
   - Frontend: http://localhost:3000
   - Swagger UI: http://localhost:5000
   - Hangfire dashboard: http://localhost:5000/hangfire

### Test Account

- Email: `testuser@example.com`
- Password: `Test1234`

## Testing

The application comes with seeded data including:

- 25 TV shows (Breaking Bad, Game of Thrones, The Last of Us, etc.)
- 8 actors
- Multiple episodes
- Test user account
