## New Albums via Email

This app allows users to subscribe to artists that they follow on Spotify, and be notified via email when a new album is released for an artist they subscribe to.

I built it as a fun way to learn React. I also decided to create the .NET Core back-end from scratch, instead of depending on ASP.NET Boilerplate.

I used the Visual Studio React template, which utilises [Create React App](https://github.com/facebookincubator/create-react-app).


### Windows dev environment setup

- Clone the repo
- Install the following dependencies:
  - .NET Core 2.1
  - Node 10.16+
  - npm 6.9+
- I use Visual Studio 2019 Community, I haven't tested this setup otherwise
- Build the solution
- Right click on NewAlbums.Web and choose "Set as StartUp Project"
- In Package Manager Console, run Update-Database with 'NewAlbums.EntityFrameworkCore' selected
- Open NewAlbums.Web/appsettings.json and add the user secrets that are indicated in that file, setting values for yourself (right-click NewAlbums.Web, choose 'Manage User Secrets')
- Set the following System environment variable: `ASPNETCORE_ENVIRONMENT=Development`

#### Troubleshooting

- In the NewAlbums.Web web deploy dialog, under Entity Framework Migrations, if you see an error about the "dotnet ef" command, run `dotnet tool install -g dotnet-ef` 