# Используем официальный образ .NET SDK для сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем файлы проекта и восстанавливаем зависимости
COPY . ./
RUN dotnet restore

# Собираем приложение
RUN dotnet publish -c Release -o /out

# Используем легковесный образ для выполнения
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Копируем собранное приложение
COPY --from=build /out .

# Указываем порт, который будет слушать контейнер
EXPOSE 5000

# Запускаем приложение
CMD ["dotnet", "run"]
    