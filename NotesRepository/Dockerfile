#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NotesRepository/NotesRepository.csproj", "."]
RUN dotnet restore "./NotesRepository.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "NotesRepository/NotesRepository.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NotesRepository/NotesRepository.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
ENTRYPOINT ["dotnet", "NotesRepository.dll", "--environment=Development"]
