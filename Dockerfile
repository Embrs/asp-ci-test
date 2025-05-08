# ----- 建置階段 -----
  FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
  WORKDIR /app
  
  # 複製 csproj 並還原
  COPY *.csproj ./
  RUN dotnet restore
  
  # 複製所有原始碼並建置
  COPY . ./
  RUN dotnet publish -c Release -o /out
  
  # ----- 執行階段 -----
  FROM mcr.microsoft.com/dotnet/aspnet:9.0
  WORKDIR /app
  
  # 複製建置結果
  COPY --from=build /out .

  EXPOSE 5000 

  # 指定執行入口點
  ENTRYPOINT ["dotnet", "asp-api.dll"]
  