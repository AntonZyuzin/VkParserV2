FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY bin/Release/net6.0/publish/ VkParserV1/
WORKDIR /VkParserV1
ENTRYPOINT ["dotnet", "NetCore.Docker.dll"]
ENV DOTNET_EnableDiagnostics=0
