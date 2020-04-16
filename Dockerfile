FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["./SuperGamblino"]