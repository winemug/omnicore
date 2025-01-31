FROM ubuntu

ENV DEBIAN_FRONTEND="noninteractive"
ENV DOTNET_ROOT=/usr/share/dotnet
ENV ANDROID_SDK_ROOT=/usr/lib/android-sdk
ENV JAVA_HOME=/usr/lib/jvm/java-17-openjdk-amd64
ENV PATH="$PATH:$ANDROID_SDK_ROOT/cmdline-tools/bin:$DOTNET_ROOT:$DOTNET_ROOT/tools"

RUN apt update && apt upgrade -y && apt install wget unzip openjdk-17-jdk-headless libicu-dev libgdiplus -y

RUN wget https://dl.google.com/android/repository/commandlinetools-linux-11076708_latest.zip
RUN unzip commandlinetools-linux-11076708_latest.zip -d ${ANDROID_SDK_ROOT}

RUN yes | sdkmanager --sdk_root=${ANDROID_SDK_ROOT} "platform-tools" "build-tools;35.0.0" "platforms;android-35-ext14"
RUN yes | sdkmanager --sdk_root=${ANDROID_SDK_ROOT} --licenses

RUN wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
RUN chmod +x dotnet-install.sh
RUN ./dotnet-install.sh --version 9.0.102 --install-dir ${DOTNET_ROOT}

RUN dotnet workload install maui-android

