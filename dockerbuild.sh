#!/usr/bin/env bash
cd "${0%/*}"
dotnet build OmniCore.Maui/OmniCore.Maui.csproj -c Release -f net8.0-android34.0 -p:AndroidSdkDirectory=/usr/lib/android-sdk
dotnet publish ./OmniCore.Maui/OmniCore.Maui.csproj -c Release -f net8.0-android34.0 -p:AndroidSdkDirectory=/usr/lib/android-sdk -o /build