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
3. Open `DiyetisyenTakipOtomasyonu.sln`. In `Web.config`, change `data source=BERKE` in the connection string `DiyetisyenTakipOtomasyonEntities2` to your own server name (for example `.\SQLEXPRESS` or `(localdb)\MSSQLLocalDB`).
4. Restore the NuGet packages and press F5.

Only `DiyetisyenTakipOtomasyonEntities2` is used by the code. The other connection strings in `Web.config` are leftovers from earlier models.

## Recent fixes

- Uploaded images are now validated (jpg, jpeg, png, gif, webp only) and saved under a unique name, so files can no longer overwrite each other and non-image files are rejected.
- The doctor list no longer drops a doctor when the city/district API is slow or unreachable.
- Detail and edit pages return "not found" instead of crashing for a missing record.
- Editing a patient keeps multi-word names correct, updates the index value and works for patients that have no diagnosis row yet.
- A doctor who still has patients can no longer be deleted (this used to fail with a foreign key error); a message is shown instead.

These changes were made by reading the code, and I could not build or run the application where I edited it, so please do a quick build and test in Visual Studio.

## Known limitations

- Passwords are stored as plain text and the other pages do not check whether you are logged in. This is acceptable for a coursework demo but not for real use; it should be replaced with hashed passwords and authentication (for example ASP.NET Identity).
- Delete actions are normal links (HTTP GET) without a confirmation or anti-forgery protection.

## Author

Berke Ekici, Computer Engineering student at Mersin University. [GitHub](https://github.com/berkeekicii)
