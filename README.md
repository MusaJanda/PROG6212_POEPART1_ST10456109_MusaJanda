# PROG6212_POEPART1_ST10456109_MusaJanda

## Overview 🚀

CMCS is a web-based application designed to streamline the process of submitting and approving monthly claims for contract lecturers. The system replaces a manual, paper-based workflow with a digital solution, improving efficiency and providing a clear audit trail.

This project was developed using **ASP.NET Core MVC** with **Entity Framework Core** and **ASP.NET Identity** for user management and role-based authorization.

---

## Features ✨

* **Role-Based Dashboards:** Users see a dashboard tailored to their role (Lecturer, Programme Coordinator, or Academic Manager).
* **Secure Authentication:** Secure user authentication and management are handled by ASP.NET Identity.
* **Claim Submission:** Lecturers can digitally submit their monthly claims, including hours worked, and attach multiple supporting documents (e.g., payslips, contracts).
* **Document Management:** The system supports uploading and storing supporting documents related to claims.
* **Multi-Stage Approval Workflow:** Claims follow a defined workflow:
    1.  **Lecturer** submits a claim.
    2.  **Programme Coordinator** reviews and approves the claim.
    3.  **Academic Manager** gives the final approval.
    4.  **Admin** manages user roles and account.
* **Database Management:** The application uses Entity Framework Core to manage the database schema and data, with a clear separation of models for Claims, Documents, Lecturers, and the different user roles.

---

## Getting Started ⚙️

### Prerequisites

* [.NET Core SDK](https://dotnet.microsoft.com/download)
* [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or a compatible database)
* [Visual Studio](https://visualstudio.microsoft.com/vs/) or [Visual Studio Code](https://code.visualstudio.com/)


---

## License 📜

This project is licensed under the MIT License - see the [LICENSE](#license) file for details.
