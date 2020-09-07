# To be run from top level

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
COPY sln /sln
RUN dotnet publish --configuration Release --output /out /sln/Frontend

FROM nginx:alpine
COPY --from=build /out/wwwroot /usr/share/nginx/html/
COPY docker/nginx.conf /etc/nginx/nginx.conf
WORKDIR /usr/share/nginx/html/
