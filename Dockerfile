# docker build -t farukerat/NoteTree-image:latest .

# Build sdk image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App
EXPOSE 80
EXPOSE 443

# Restore packages
COPY *.csproj ./
RUN dotnet restore
COPY . ./

# Build
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "NoteTree.dll"]
