# HomeCare Appointment

## Overview

A simple **ASP.NET Core MVC (.NET 8, C# 12)** web application for managing homecare appointments. 
Healthcare personnel can publish available time slots, and patients (older adults) can book, 
view, update, and cancel appointments with optional notes for tasks. 
This MVP demonstrates full CRUD operations for the main entities: **Appointments** and **AvailableDays**,
with supporting entities for **Patients** and **Personnel**.

---

## Features

- **Available Days Management**: Personnel can create/edit/delete time slots (Full CRUD)
- **Appointment Booking**: Patients select an available slot and book it(one appointment per slot), create/edit/delete (Full CRUD)
- Repository pattern with Entity Framework Core and SQLite
- Data Seeding: Automatic seed in Development mode via `DBInit.Seed`
- Edit Appointments Change notes or move booking to a different available slot (deletes old, creates new)
  - Admin & Patient Views: 
  - Admin: See all appointments ordered by date
  - Patient: Filter appointments by selected patient


---
## Prerequisites
- .NET SDK 8.0 (https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Node.js v22.20.0 *(not required — used only if front-end dependencies are added later)*

## Installation and Setup 

```
cd HomeCareAppointment
dotnet restore
dotnet build
dotnet ef database update
dotnet run
```
---

## Documentation & plagiarism
- No third‑party code snippets were directly copied into this project.  
- Some scaffolding was used to generate initial views and controllers; these were then customized to fit the requirements of the web app.  
- We used LLM tools (e.g., ChatGPT) and Google as learning aids — mainly to understand build errors, configuration issues, and some general implementation patterns as we made this project while learning ASP.NET.
- External libraries used are standard frameworks included via NuGet/CDN (Entity Framework Core, SQLite).  
---
## To Do / Known Limitations (MVP)

- **Authentication**: Not implemented yet, but we designed the web app with this in mind (e.g., separate admin vs. patient filtered views).  
- **Basic UX**: Functional but minimal; UI can be improved visually for better usability.  
- **Unit Testing**: Not included; only manual testing performed for this version.
---

##  MVC Endpoints (Main Entities, Appointments and AvailableDays)

These are **MVC routes** returning Razor views, not JSON APIs. Default routing: `/{controller}/{action}/{id?}`.

### Appointments Controller

| HTTP Method | Endpoint | Description |
|-------------|----------|-------------|
| **GET** | `/Appointments` or `/Appointments/Index` | Lists all available days with patient dropdown for booking |
| **GET** | `/Appointments/Details/{id}` | Shows details of a specific appointment |
| **GET** | `/Appointments/Create?availableDayId={id}` | Displays form to book the selected available day |
| **POST** | `/Appointments/Create` | Creates a new appointment; sets `PersonnelId` and `Date` from chosen slot |
| **GET** | `/Appointments/Edit/{id}` | Displays edit form (change notes or move to another open slot) |
| **POST** | `/Appointments/Edit/{id}` | Updates notes; if slot changed, creates new appointment and deletes old |
| **GET** | `/Appointments/Delete/{id}` | Shows delete confirmation page |
| **POST** | `/Appointments/Delete/{id}` | Deletes the appointment and redirects to Index |
| **GET** | `/Appointments/ManageAdmin` | Admin view: all appointments ordered by date |
| **POST** | `/Appointments/ManagePatient` | Patient view: filter appointments by patient |
| **GET** | `/Appointments/ManagePatient?patientId={id}` | Patient view: filter appointments by patient |

**Key Logic**:
- **Create**: Validates slot is not already booked; derives `PersonnelId` and `Date` from `AvailableDay`
- **Edit**: If `AvailableDayId` changes, checks new slot availability, creates new appointment, deletes old
- **Delete**: Frees up the `AvailableDay` slot
- **Manage views**: Shared `Manage.cshtml` view with `ViewData["PatientMode"]` to toggle between admin/patient modes

---

### AvailableDays Controller

| HTTP Method | Endpoint | Description |
|-------------|----------|-------------|
| **GET** | `/AvailableDays` or `/AvailableDays/Index` | Lists all available time slots |
| **GET** | `/AvailableDays/Create` | Form to create a new available day |
| **POST** | `/AvailableDays/Create` | Creates a new available day (personnel, date, time range) |
| **GET** | `/AvailableDays/Edit/{id}` | Edit form for available day |
| **POST** | `/AvailableDays/Edit/{id}` | Updates available day |
| **GET** | `/AvailableDays/Delete/{id}` | Delete confirmation |
| **POST** | `/AvailableDays/Delete/{id}` | Deletes available day (only if no appointment booked) |

***Note***: Each `AvailableDay` can have at most one `Appointment` (1:1 relationship).
