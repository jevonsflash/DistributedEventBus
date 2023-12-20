#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Producer.EcgExternal/Producer.EcgExternal.csproj", "src/Producer.EcgExternal/"]
COPY ["src/Producer.Core/Producer.Core.csproj", "src/Producer.Core/"]
COPY ["src/Infrastructure.Abp.Hangfire/Infrastructure.Abp.Hangfire.csproj", "src/Infrastructure.Abp.Hangfire/"]
COPY ["abp/Abp/Abp.csproj", "abp/Abp/"]
COPY ["abp/Abp.AutoMapper/Abp.AutoMapper.csproj", "abp/Abp.AutoMapper/"]
COPY ["abp/Abp.HangFire/Abp.HangFire.csproj", "abp/Abp.HangFire/"]
COPY ["abp/Abp.ZeroCore.EntityFrameworkCore/Abp.ZeroCore.EntityFrameworkCore.csproj", "abp/Abp.ZeroCore.EntityFrameworkCore/"]
COPY ["abp/Abp.ZeroCore/Abp.ZeroCore.csproj", "abp/Abp.ZeroCore/"]
COPY ["abp/Abp.Zero.Common/Abp.Zero.Common.csproj", "abp/Abp.Zero.Common/"]
COPY ["abp/Abp.EntityFrameworkCore/Abp.EntityFrameworkCore.csproj", "abp/Abp.EntityFrameworkCore/"]
COPY ["abp/Abp.EntityFramework.Common/Abp.EntityFramework.Common.csproj", "abp/Abp.EntityFramework.Common/"]
COPY ["src/Producer.Infrastructure/Producer.Infrastructure.csproj", "src/Producer.Infrastructure/"]
COPY ["src/Producer.EntityFrameworkCore/Producer.EntityFrameworkCore.csproj", "src/Producer.EntityFrameworkCore/"]
COPY ["src/Producer.Questionnaire/Producer.Questionnaire.csproj", "src/Producer.Questionnaire/"]
COPY ["src/Producer.IQuestionnaire/Producer.IQuestionnaire.csproj", "src/Producer.IQuestionnaire/"]
COPY ["src/Pattern/Soa/Soa.csproj", "src/Pattern/Soa/"]
COPY ["src/Producer.Tag/Producer.Tag.csproj", "src/Producer.Tag/"]
COPY ["src/Producer.Category/Producer.Category.csproj", "src/Producer.Category/"]
COPY ["src/Producer.Article/Producer.Article.csproj", "src/Producer.Article/"]
COPY ["src/Producer.Wechat/Producer.Wechat.csproj", "src/Producer.Wechat/"]
COPY ["src/Producer.ClientTracking/Producer.ClientTracking.csproj", "src/Producer.ClientTracking/"]
COPY ["src/Producer.IClientTracking/Producer.IClientTracking.csproj", "src/Producer.IClientTracking/"]
COPY ["src/Producer.Common/Producer.Common.csproj", "src/Producer.Common/"]
COPY ["src/Producer.ICommon/Producer.ICommon.csproj", "src/Producer.ICommon/"]
COPY ["src/Producer.Application.Shared/Producer.Application.Shared.csproj", "src/Producer.Application.Shared/"]
COPY ["src/Producer.Client/Producer.Client.csproj", "src/Producer.Client/"]
COPY ["src/Producer.IClient/Producer.IClient.csproj", "src/Producer.IClient/"]
COPY ["src/Producer.IM/Producer.IM.csproj", "src/Producer.IM/"]
COPY ["src/Producer.FileStorage.Application/Producer.FileStorage.Application.csproj", "src/Producer.FileStorage.Application/"]
COPY ["src/Producer.FileStorage/Producer.FileStorage.csproj", "src/Producer.FileStorage/"]
COPY ["src/Producer.Producer/Producer.Producer.csproj", "src/Producer.Producer/"]
COPY ["src/RocketChat/RocketChat/RocketChat.csproj", "src/RocketChat/RocketChat/"]
COPY ["src/Producer.FileStorage/Producer.FileStorage.csproj", "src/Producer.FileStorage/"]
COPY ["abp/Abp.Castle.Log4Net/Abp.Castle.Log4Net.csproj", "abp/Abp.Castle.Log4Net/"]
COPY ["src/FileStorage/FileStorage.csproj", "src/FileStorage/"]
COPY ["src/LiveStreaming/Rtmp/Rtmp.csproj", "src/LiveStreaming/Rtmp/"]
RUN dotnet restore "src/Producer.EcgExternal/Producer.EcgExternal.csproj"
COPY . .
WORKDIR "/src/src/Producer.EcgExternal"
RUN dotnet build "Producer.EcgExternal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Producer.EcgExternal.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Producer.EcgExternal.dll","-q"]