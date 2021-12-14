#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Dx29.Web.Management/Dx29.Web.Management.csproj", "Dx29.Web.Management/"]
COPY ["Dx29.Services/Dx29.Services.csproj", "Dx29.Services/"]
COPY ["Dx29.Data/Dx29.Data.csproj", "Dx29.Data/"]
COPY ["Dx29/Dx29.csproj", "Dx29/"]
COPY ["Dx29.Azure/Dx29.Azure.csproj", "Dx29.Azure/"]
RUN dotnet restore "Dx29.Web.Management/Dx29.Web.Management.csproj"
COPY . .
WORKDIR "/src/Dx29.Web.Management"
RUN dotnet build "Dx29.Web.Management.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dx29.Web.Management.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dx29.Web.Management.dll"]