# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Create necessary directories
RUN mkdir -p Infrastructure DomainService Model/Entity Model/Model CommonLibrary/Common.Authorization CommonLibrary/Common.Constant CommonLibrary/Common.Settings CommonLibrary/Common.UnitOfWork CommonLibrary/Common.Utils APILaws

# Copy the project files and restore dependencies
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["DomainService/DomainService.csproj", "DomainService/"]
COPY ["Model/Entity/Entity.csproj", "Model/Entity/"]
COPY ["Model/Model/Model.csproj", "Model/Model/"]
COPY ["CommonLibrary/Common.Authorization/Common.Authorization.csproj", "CommonLibrary/Common.Authorization/"]
COPY ["CommonLibrary/Common.Constant/Common.Constant.csproj", "CommonLibrary/Common.Constant/"]
COPY ["CommonLibrary/Common.Settings/Common.Settings.csproj", "CommonLibrary/Common.Settings/"]
COPY ["CommonLibrary/Common.UnitOfWork/Common.UnitOfWork.csproj", "CommonLibrary/Common.UnitOfWork/"]
COPY ["CommonLibrary/Common.Utils/Common.Utils.csproj", "CommonLibrary/Common.Utils/"]
COPY ["APILaws/APILaws.csproj", "APILaws/"]

RUN dotnet restore "APILaws/APILaws.csproj"

# Copy the remaining source code and build the app
COPY . .
WORKDIR /src/APILaws
RUN dotnet build "APILaws.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the app to a folder
RUN dotnet publish "APILaws.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# Use the official .NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build app/publish .
ENTRYPOINT ["dotnet", "APILaws.dll"]