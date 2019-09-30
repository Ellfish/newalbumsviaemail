## New Albums via Email

This app allows users to subscribe to artists that they follow on Spotify, and be notified via email when a new album is released for an artist they subscribe to.

I built it as a fun way to learn React. I also decided to create the .NET Core back-end from scratch, instead of depending on ASP.NET Boilerplate.

I used the Visual Studio React template, which utilises [Create React App](https://github.com/facebookincubator/create-react-app).


### Dev environment setup

- Clone the repo
- Have .NET Core 2.1 installed
- I use Visual Studio 2019 Community, I haven't tested this setup otherwise
- Build the solution
- In Package Manager Console, run Update-Database with 'NewAlbums.EntityFrameworkCore' selected
- Add the following user secrets, setting values for yourself (right-click NewAlbums.Web, choose 'Manage User Secrets') 
  - App:AdminEmailAddress
  - App:AdminFullName
- Set the following System environment variable: `ASPNETCORE_ENVIRONMENT=Development`

#### Troubleshooting

- In the NewAlbums.Web web deploy dialog, under Entity Framework Migrations, if you see an error about the "dotnet ef" command, run `dotnet tool install -g dotnet-ef` 