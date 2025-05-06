# 🔎 RatingTracker

**RatingTracker** is a web application that tracks how a specified domain ranks in search engine results (Google, Bing) for a given keyword. It scrapes the top 100 search results and reports ranking positions.

---

## 🐳 Running with Docker Compose

The project is fully containerized and designed to be run using **Docker Compose**.

### 🔧 Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop) (with Docker Compose)
- [.NET 9 SDK (if you plan to run tests or build locally)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

### ▶️ Run the App

The docker compose file is located under Code folder in the root of the project. Run the following command.

```bash
docker compose up --build


Access the UI from the browser: http://localhost:8080

