# Use the SDK image for building the project
FROM mcr.microsoft.com/dotnet/sdk:latest AS build

# Set the working directory
WORKDIR /src

# Copy the project files
COPY . .

# Rename conflicting files
RUN mv EducationPortalUI/appsettings.json EducationPortalUI/appsettings.UI.json
RUN mv EducationPortalUI/appsettings.Development.json EducationPortalUI/appsettings.Development.UI.json

# Restore dependencies
RUN dotnet restore EducationPortalUI/EducationPortalUI.csproj

# Publish the project
RUN dotnet publish EducationPortalUI/EducationPortalUI.csproj -c Release -o /app/out

# Use the ASP.NET runtime image for running the project
FROM mcr.microsoft.com/dotnet/aspnet:latest AS runtime

# Set the working directory
WORKDIR /app

# Copy the build output from the build stage
COPY --from=build /app/out ./

# Rename the configuration files back to their original names
RUN mv appsettings.UI.json appsettings.json
RUN mv appsettings.Development.UI.json appsettings.Development.json



# Uygulaman�n 7145 portunda dinlemesi i�in
ENV ASPNETCORE_URLS=http://+:7026

# Run the application
ENTRYPOINT ["dotnet", "EducationPortalUI.dll"]
