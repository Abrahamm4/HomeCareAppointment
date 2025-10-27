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
- **Double-Booking Prevention**: Server-side checks ensure a slot can only be booked once
- **Edit Appointments**: Change notes or move booking to a different available slot (deletes old, creates new)
- **Admin & Patient Views**: 
  - Admin: See all appointments ordered by date
  - Patient: Filter appointments by selected patient
- **Data Seeding**: Automatic seed in Development mode via `DBInit.Seed`'
- Dynamic views with ViewBag/ViewData for flexible UI
- Repository pattern with Entity Framework Core and SQLite
---
## Prerequisites
- .NET SDK 8.0 (https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Node.js v22.20.0 *(not required — used only if front-end dependencies are added later)*

## Installation and Setup 

```
cd HomeCareAppointment
dotnet restore
dotnet build (dotnet ef database update)
dotnet run
```
---

##  MVC Endpoints (Main Entities, Appointments and AvailableDays)

These are **MVC routes** returning Razor views, not JSON APIs. Default routing: `/{controller}/{action}/{id?}`.

### Appointments Controller

| HTTP Method | Endpoint | Description | Parameters |
|-------------|----------|-------------|------------|
| **GET** | `/Appointments` or `/Appointments/Index` | Lists all available days with patient dropdown for booking | — |
| **GET** | `/Appointments/Details/{id}` | Shows details of a specific appointment | `id` (route) |
| **GET** | `/Appointments/Create?availableDayId={id}` | Displays form to book the selected available day | `availableDayId` (query) |
| **POST** | `/Appointments/Create` | Creates a new appointment; sets `PersonnelId` and `Date` from chosen slot | `PatientId`, `AvailableDayId`, `Notes` (form body) |
| **GET** | `/Appointments/Edit/{id}` | Displays edit form (change notes or move to another open slot) | `id` (route) |
| **POST** | `/Appointments/Edit/{id}` | Updates notes; if slot changed, creates new appointment and deletes old | `AppointmentId`, `Notes`, `AvailableDayId` (form body) |
| **GET** | `/Appointments/Delete/{id}` | Shows delete confirmation page | `id` (route) |
| **POST** | `/Appointments/Delete/{id}` | Deletes the appointment and redirects to Index | `id` (route, form body via ActionName) |
| **GET** | `/Appointments/ManageAdmin` | Admin view: all appointments ordered by date | — |
| **POST** | `/Appointments/ManagePatient` | Patient view: filter appointments by `patientId` (POST form) | `patientId` (form body) |
| **GET** | `/Appointments/ManagePatient?patientId={id}` | Patient view: filter appointments by `patientId` (GET query) | `patientId` (query string) |

**Key Logic**:
- **Create**: Validates slot is not already booked; derives `PersonnelId` and `Date` from `AvailableDay`
- **Edit**: If `AvailableDayId` changes, checks new slot availability, creates new appointment, deletes old
- **Delete**: Frees up the `AvailableDay` slot
- **Manage views**: Shared `Manage.cshtml` view with `ViewData["PatientMode"]` to toggle between admin/patient modes

### AvailableDays Controller

| HTTP Method | Endpoint | Description | Parameters |
|-------------|----------|-------------|------------|
| **GET** | `/AvailableDays` or `/AvailableDays/Index` | Lists all available time slots | — |
| **GET** | `/AvailableDays/Create` | Form to create a new available day | — |
| **POST** | `/AvailableDays/Create` | Creates a new available day (personnel, date, time range) | Personnel, Date, StartTime, EndTime (form) |
| **GET** | `/AvailableDays/Edit/{id}` | Edit form for available day | `id` (route) |
| **POST** | `/AvailableDays/Edit/{id}` | Updates available day | `id`, updated fields (form) |
| **GET** | `/AvailableDays/Delete/{id}` | Delete confirmation | `id` (route) |
| **POST** | `/AvailableDays/Delete/{id}` | Deletes available day (only if no appointment booked) | `id` (route) |

***Note***: Each `AvailableDay` can have at most one `Appointment` (1:1 relationship).
---


