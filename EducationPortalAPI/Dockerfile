# Base image olarak resmi .NET SDK imajını kullan
FROM mcr.microsoft.com/dotnet/sdk:latest AS build
WORKDIR /app

# Proje dosyalarını kopyala ve restore et
COPY . ./
RUN dotnet restore

# Uygulamayı yayınla
RUN dotnet publish -c Release -o out

# Çalıştırılabilir imajı kullan
FROM mcr.microsoft.com/dotnet/aspnet:latest AS runtime
WORKDIR /app
COPY --from=build /app/out .


# Uygulaman�n 7145 portunda dinlemesi i�in
ENV ASPNETCORE_URLS=http://+:7145

ENTRYPOINT ["dotnet", "EducationPortalAPI.dll"]
