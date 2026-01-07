#Create New Project With MVC Patterb

```
dotnet new mvc -n DotnetSkeletonApp
```

#File Asset

- Folder wwwroot untuk meletakkan file-file statis

#Kegunaan package yang dipakai

```
<PackageReference Include="Hangfire" Version="1.8.21" /> -> Background job processing tanpa Windows Service / Cron manual
<PackageReference Include="Hangfire.MySqlStorage" Version="2.0.3" /> -> Penyimpanan data Hangfire di MySQL / MariaDB
<PackageReference Include="MailKit" Version="4.13.0" /> -> Library SMTP / Email Client modern
<PackageReference Include="Microsoft.AspNetCore.SignalR" Version="1.2.0" /> -> Real-time communication (WebSocket fallback)
<PackageReference Include="Microsoft.AspNetCore.Authentication.Cookies" Version="2.3.0" /> -> Authentication berbasis Cookie
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.20" /> -> User Management lengkap berbasis EF Core
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="8.0.3" /> -> Provider EF Core untuk MySQL / MariaDB
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.20" /> -> Support EF Core tools (Design-time) (Add migration , update database)
<PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="9.0.0" /> -> Scaffolding otomatis (generate controller, CRUD, Razor Page)
```
