FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY /runtimes/debian-x64/native/libuv.so /app
COPY . .
ENTRYPOINT ["dotnet", "Lykke.Job.OffchainCashoutScheduler.dll"]
