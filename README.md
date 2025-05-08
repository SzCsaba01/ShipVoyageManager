# Ship Voyage Management Application

## Overview

The Ship Voyage Management Application is a web-based system built using **Angular** for the frontend and **.NET** for the backend, with **Microsoft SQL Server** as the database. This application is designed to manage and visualize ship voyages, ports, and visited countries, with role-based user management. It uses **HTTPS** with a self-signed certificate for secure communication between the frontend and backend.

## Application Features

### User Authentication & Authorization

- **Registration**: Users can register by providing their email and password. After registration, an email verification link is sent.
- **Email Verification**: Upon clicking the verification link, the user's email is confirmed, and they can log in.
- **Login**: Users can log in using their credentials. Upon successful login, a **JWT** token is issued and stored in **HTTP-only cookies** for secure session management.
- **User Roles**:
  - **User**: Can only view data.
  - **Admin**: Can add, update, and delete data (ships, voyages, ports, and visited countries).

### Data Display

- **Tables**: 
  - Ships, voyages, and ports are displayed in tables.
  
- **Visited Countries Line Chart**:
  - A line chart displays the number of new countries visited each month over the last year, using **Chart.js**.

## Error Handling

- **Error Handling Middleware**:
  - The backend includes an **error handling middleware** that processes errors and generates custom error messages, which are then sent to the frontend.
  
- **Frontend Interceptor**:
  - The frontend has an **interceptor** that listens for error messages from the backend and displays them in a **toast message bar** for the user.
  
## Guards and State Management

- **Auth Guard**:
  - The frontend uses an **Auth Guard** to protect routes from unauthorized access.
  
- **Role Guard**:
  - The **Role Guard** ensures that only users with the appropriate roles can access specific routes and actions (e.g., only admins can add, update, or delete data).

- **NgRx Store**:
  - The application uses **NgRx** to manage the state, including storing the user's roles, which are then utilized by the guards to determine route access.

## Setup Instructions

### Backend Setup

1. **Configure the Database Connection**:
   - Open the `appsettings.json` file in the backend project.
   - Update the `DefaultConnection` string with your SQL Server database connection.

2. **Email Credentials**:
   - In the `appsettings.json`, add your email service credentials under the appropriate settings to allow the application to send verification and registration emails.

3. **Create the Database**:
   - Open **Package Manager Console** in Visual Studio.
   - Run the following command to update and create the database:
     ```
     Update-Database
     ```

4. **Run the Backend**:
   - Ensure that the project is set to use **HTTPS** (check the launch settings in Visual Studio).
   - Run the backend project in **HTTPS** mode.

### Frontend Setup

1. **Install Dependencies**:
   - Navigate to the frontend directory of the project.
   - Run the following command to install the required npm packages:
     ```
     npm install
     ```

2. **Start the Application**:
   - Run the following command to start the Angular application:
     ```
     npm start
     ```

   - The frontend will be available at `https://localhost:4200`.

## Unit Tests

The application includes unit tests for all services and repository methods. These tests ensure that the business logic and database interactions function correctly.
