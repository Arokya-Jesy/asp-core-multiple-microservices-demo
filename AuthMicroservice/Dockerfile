FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SharedExtensions/SharedExtensions.csproj", "SharedExtensions/"]
COPY ["AuthMicroservice/AuthMicroservice.csproj", "AuthMicroservice/"]
RUN dotnet restore "AuthMicroservice/AuthMicroservice.csproj"
COPY . .
RUN dotnet build "AuthMicroservice/AuthMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthMicroservice/AuthMicroservice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthMicroservice.dll"]