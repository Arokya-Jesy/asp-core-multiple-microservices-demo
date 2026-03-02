FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SharedExtensions/SharedExtensions.csproj", "SharedExtensions/"]
COPY ["OrderMicroservice/OrderMicroservice.csproj", "OrderMicroservice/"]
RUN dotnet restore "OrderMicroservice/OrderMicroservice.csproj"
COPY . .
RUN dotnet build "OrderMicroservice/OrderMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderMicroservice/OrderMicroservice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderMicroservice.dll"]
