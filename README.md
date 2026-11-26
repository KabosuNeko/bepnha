# Bếp Nhà

A recipe sharing website: people post home-cooked recipes, search them and rate them.
Course project for Basic Web Programming, built by Nguyễn Hoàng Long (25DH110432).

Live site: https://bepnha.somee.com

## What it does

- Browse recipes by category, search by dish name or ingredient, filter by difficulty and cooking time.
- Sign up and log in; post a recipe with a photo; edit or delete your own recipes.
- Rate a recipe once (1 to 5 stars, with a comment) and save recipes to "Món đã lưu".
- Admins approve or reject new posts and manage categories.

## Built with

- ASP.NET MVC 5 on .NET Framework 4.8
- SQL Server with Entity Framework 6 (Database First, `Models/BepNhaModel.edmx`)
- Bootstrap 5, jQuery Validation

## Run it locally

1. Open `BepNhaDB.sql` in SSMS (server `.\SQLEXPRESS`) and run it. It creates the `BepNhaDB` database with sample data.
2. Open `RecipeSharingWebsite.sln` in Visual Studio 2022 and press F5.
3. If your SQL Server has a different name, change `data source` in the `BepNhaDBEntities` connection string in `RecipeSharingWebsite/Web.config`.

Sample admin and user accounts are listed at the top of `BepNhaDB.sql`.

## Project layout

```
RecipeSharingWebsite/
  Controllers/     Home, Recipes, Account, Admin, Categories
  Models/          EF model, validation rules, upload and permission rules
  Views/           one folder per controller, shared layout and partials
  Content/         Site.css (shared), pages/ (CSS for one page each), Images/
  Scripts/pages/   JavaScript for the recipe form and recipe page
RecipeSharingWebsite.Tests/   unit tests for upload and permission rules
BepNhaDB.sql                  database script with sample data
```

## Tests

Open Test Explorer in Visual Studio and run all tests.

## Credits

Food photos come from Wikimedia Commons under free licenses. Authors and licenses are listed in `RecipeSharingWebsite/Content/Images/NGUON-ANH.txt`.
