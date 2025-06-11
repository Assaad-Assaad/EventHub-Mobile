# 📱 Event Hub

**Event Hub** is a cross-platform mobile application built with **.NET MAUI**. It allows users to browse, register for, and manage events. The app features both online and offline capabilities, using a custom ASP.NET Core API and local storage for a seamless user experience.

---

## 🚀 Project Structure

The solution consists of **three main projects**:

1. **EventHub.API**  
   ASP.NET Core Web API responsible for:
   - Serving event data
   - Handling user authentication (via Identity Framework)
   - Providing secure endpoints using JWT tokens

2. **EventHub.Shared**  
   A shared .NET Standard or .NET Core class library:
   - Contains common DTOs
   - Includes shared utility classes used by both the API and mobile app

3. **EventHub.MobileApp**  
   A .NET MAUI mobile application:
   - Core UI and user-facing functionality
   - Works offline using SQLite
   - Authenticates with the API using JWT
   - Stores and retrieves user data and events locally when offline

---

## 📋 App Features

- **Login/Registration**  
  - Authenticate users with JWT tokens via API
  - Option to skip login and browse publicly

- **Home Page**  
  - View recently added events
  - Navigate to full event list

- **All Events Page**  
  - Browse all events
  - Search by title, filter by category or date

- **Event Details Page**  
  - View detailed event info
  - Register for events (requires internet)
  - Add/remove from favorites (works offline)

- **My Events Page**  
  - View all favorite events
  - Register or remove events with confirmation dialogs

- **Profile Page**  
  - Display user's personal details (name, email)

---

## 🛠 Technical Overview

| Feature             | Implementation                                                                 |
|---------------------|----------------------------------------------------------------------------------|
| **UI Framework**     | .NET MAUI                                                                       |
| **API**              | ASP.NET Core Web API with Identity Framework                                    |
| **Authentication**   | JWT Bearer tokens, Identity Framework                                           |
| **Local Storage**    | SQLite via Entity Framework Core (or similar ORM)                               |
| **Shared Code**      | DTOs and logic in a shared class library                                        |
| **Offline Support**  | Cached events and favorites using local storage                                 |
| **Validation**       | Data annotations in models and view models                                      |
| **Error Handling**   | Friendly messages shown on UI for failures and validation errors                |
| **Navigation**       | Tab-based or menu-driven navigation with consistent UX                          |

---

## 🎯 Motivation

This project was selected to explore and integrate multiple components of the .NET ecosystem. It provides real-world experience with:

- Hybrid mobile app development with MAUI
- Online/offline data handling
- Secure API design with Identity
- Practical event management functionality

---


## ⚙️ Getting Started

### Prerequisites

- .NET 9 or later
- Visual Studio 2022+ with .NET MAUI and ASP.NET workloads installed
- SQLite support for your development platform
- Android/iOS emulator or device for testing


