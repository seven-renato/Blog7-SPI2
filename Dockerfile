FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5085

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Blog7/Blog7.csproj", "Blog7/"]
RUN dotnet restore "Blog7/Blog7.csproj"
COPY . .
WORKDIR "/src/Blog7"
RUN dotnet build "Blog7.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Blog7.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blog7.dll"]
