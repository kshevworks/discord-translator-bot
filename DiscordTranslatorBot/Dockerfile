FROM mcr.microsoft.com/dotnet/runtime:5.0

COPY ${APP_PATH} bin/Release/net5.0/publish App/
WORKDIR /App
COPY ${KEY_PATH} ./
ENTRYPOINT ["dotnet", "DiscordTranslatorBot.dll"]
ENV DOTNET_EnableDiagnostics = 0
