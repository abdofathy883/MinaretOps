# Start from the ASP.NET runtime image, as the application is already built
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 5001

# Set the final stage
FROM base AS final
WORKDIR /app

# Copy the pre-published application files into the image
COPY . .

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "ClientAPI.dll"]