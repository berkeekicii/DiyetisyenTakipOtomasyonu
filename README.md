# Diyetisyen Takip Otomasyonu

A dietitian clinic management web application built with **ASP.NET MVC 5** and **Entity Framework 6** on SQL Server. A dietitian (admin) can register patients, assign them to doctors, record measurements and a diagnosis, and keep track of appointments. The interface is in Turkish.

## Features

- **Admin login and sign-up** (`Login` controller)
- **Patients:** list, search (name, surname, weight, height, index), create, edit, view details and delete. Each patient has a photo, contact details, weight, height, index value, gender, diagnosis, assigned doctor and appointment date.
- **Doctors:** list, search, create, edit, view details and delete, with a profile photo. City and district names are looked up from the public [turkiyeapi.dev](https://turkiyeapi.dev) API; if the lookup fails, the stored codes are shown instead.
- **Photo uploads** saved to the `Uploads/` folder.

## Tech stack

ASP.NET MVC 5 (.NET Framework 4.x), Razor views, Entity Framework 6 (Database First, `.edmx`), SQL Server, Bootstrap, jQuery, Newtonsoft.Json.

## Project structure

```text
DiyetisyenTakipOtomasyonu/
├── Controllers/   # Home, Login, Create (patients), Doktorlar, Detail
├── Models/        # EF model (.edmx), entities, view models
├── Views/         # Razor views
├── Content/ Scripts/ Uploads/
└── Web.config
diyetisyenTakipOtomasyyonuDb.bacpac   # database backup (schema and sample data)
```

## Getting started

Requirements: Windows, Visual Studio 2019 or newer with the ASP.NET workload, and SQL Server (Express or LocalDB is enough).

1. Clone the repository.
2. In SQL Server Management Studio, restore the database: right-click *Databases → Import Data-tier Application* and choose `diyetisyenTakipOtomasyyonuDb.bacpac`. Name the database `DiyetisyenTakipOtomasyon`.
3. Open `DiyetisyenTakipOtomasyonu.sln`. Restore the database to LocalDB (`(localdb)\MSSQLLocalDB`). If you use another SQL Server instance, change `data source` in the connection string `DiyetisyenTakipOtomasyonEntities2` in `Web.config`.
4. Restore the NuGet packages and press F5.

Only `DiyetisyenTakipOtomasyonEntities2` is used by the code. The other connection strings in `Web.config` are leftovers from earlier models.

## Security and fixes

- **Login required:** every page except the login page is protected by a session-based `[AdminAuthorize]` filter. A logout link is in the navigation bar. New admin accounts can only be created by a logged-in admin (or when no admin exists yet).
- **Hashed passwords:** passwords are stored with PBKDF2. Old plain-text rows from the sample database are accepted once and upgraded to a hash on the first successful login.
- **Safe deletes:** delete actions are POST requests with an anti-forgery token and a confirmation dialog. A doctor who still has patients cannot be deleted; a message is shown instead.
- **Safe uploads:** uploaded images are limited to jpg, jpeg, png, gif and webp and saved under a unique name, so files cannot overwrite each other and non-image files are rejected.
- The doctor list no longer drops a doctor when the city/district API is slow or unreachable.
- Detail and edit pages return "not found" instead of crashing for a missing record, and editing a patient handles multi-word names, updates the index value and works for patients without a diagnosis row.
- The default connection string points to SQL Server LocalDB, so the project runs after restoring the database without editing `Web.config`.

## Author

Berke Ekici, Computer Engineering student at Mersin University. [GitHub](https://github.com/berkeekicii)
