FROM microsoft/dotnet:2.1-aspnetcore-runtime
RUN mkdir /app
WORKDIR /app
COPY publish/ ./
EXPOSE 9020
ENTRYPOINT ["dotnet", "WeText.Services.Accounts.dll"]
