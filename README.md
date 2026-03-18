# Task Management System

A task management application featuring a .NET 8 backend, a React frontend, and a RabbitMQ-powered background service for task reminders.

## Prerequisites

To run this application locally, ensure you have the following installed:
* **.NET 8.0 SDK**
* **Node.js** (v18+ recommended)
* **Docker Desktop** (for running RabbitMQ and SQL Server)

---

## Local Setup & Execution Instructions

### Step 1: Start the Infrastructure (RabbitMQ and SQL Server)
A `docker-compose.yml` file is included at the root for easy setup.
1. Open a terminal at the root of the solution.
2. Run the following command to spin up RabbitMQ and SqlServer:
   ```bash
   docker-compose up -d
   ```
For rabbitMQ UI you can visit http://localhost:15672

### Step 2: Create the Database
Before running the backend, you need to apply the Entity Framework migrations to create the database schema in the Docker SQL instance.
1. Open a terminal inside the **SuperComApi** folder.
2. Run the following command:
   ```bash
   dotnet ef database update
   ```

### Step 3: Run the Backend (API & Worker Service)
The backend requires both the REST API and the Windows Service (Worker) to run simultaneously.

**Using Visual Studio:**
1. Right-click the Solution (`.sln`) in Solution Explorer and select **Configure Startup Projects...**
2. Choose **Multiple startup projects**.
3. Set the action for both **SuperComApi** and **SuperComWorker** to **Start**.
4. Press **F5** or click **Start**.

### Step 4: Run the Frontend (Client)
1. Open a new terminal and navigate to the frontend directory:
   ```bash
   cd SuperComClient
   ```
2. Install the required npm packages:
   ```bash
   npm install
   ```
3. Start the development server:
   ```bash
   npm run dev
   ```
4. The application will be running at the local host URL provided in the terminal (typically `http://localhost:5173`).

---

## Architectural Overview

* **Frontend:** Built with React, Redux Toolkit for state management, and Axios for API communication.
* **Backend API:** Built with .NET 8, implementing RESTful CRUD operations. Server-side validation with Data Annotations on DTOs.
* **Database:** SQL Server with Entity Framework Core. Tasks and Tags use a many-to-many relationship with a join table.
* **Background Processing:** A .NET Worker Service pulls tasks from the database. If a task is past its due date, it pushes a message to RabbitMQ, which is then consumed and logged (to simulate processing).

---

## Evaluation Requirement: SQL Query
As requested in Section 3, the following query returns tasks with at least two tags, including tag names, sorted by the tags amount descending:

```sql
SELECT 
    t.Id,
    t.Title, 
    COUNT(tt.TagId) AS TagCount, 
    STRING_AGG(tg.Name, ', ') AS TagNames
FROM Tasks t
JOIN TaskTags tt ON t.Id = tt.TaskId
JOIN Tags tg ON tt.TagId = tg.Id
GROUP BY t.Id, t.Title
HAVING COUNT(tt.TagId) >= 2
ORDER BY TagCount DESC;
```