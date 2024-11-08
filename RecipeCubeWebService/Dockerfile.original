# 使用 .NET 8 SDK 作為基底映像
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# 設定工作目錄
WORKDIR /app

# 將專案檔案複製到容器中
COPY . .

# 還原套件
RUN dotnet restore

# 建構專案
RUN dotnet publish -c Release -o /out

# 使用 .NET 8 運行時作為基底映像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# 設定運行時工作目錄
WORKDIR /app

# 複製建置的應用程式
COPY --from=build /out .

# 暴露應用程式的埠
EXPOSE 80

# 設定容器啟動命令
ENTRYPOINT ["dotnet", "RecipeCubeWebService.dll"]