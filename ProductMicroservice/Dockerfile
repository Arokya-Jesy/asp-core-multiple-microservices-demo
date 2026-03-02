FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SharedExtensions/SharedExtensions.csproj", "SharedExtensions/"]
COPY ["ProductMicroservice/ProductMicroservice.csproj", "ProductMicroservice/"]
RUN dotnet restore "ProductMicroservice/ProductMicroservice.csproj"
COPY . .
RUN dotnet build "ProductMicroservice/ProductMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductMicroservice/ProductMicroservice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductMicroservice.dll"]