FROM ubuntu
ENV DEBIAN_FRONTEND="noninteractive"
RUN apt update && apt upgrade -y && apt install wget unzip openjdk-17-jdk-headless libicu-dev libgdiplus -y
ENV JAVA_HOME=/usr/lib/jvm/java-17-openjdk-amd64

RUN wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
RUN chmod +x dotnet-install.sh
RUN ./dotnet-install.sh --version 8.0.110 --install-dir /usr/share/dotnet
ENV DOTNET_ROOT=/usr/share/dotnet

ENV PATH="$PATH:$ANDROID_SDK_ROOT/cmdline-tools/bin:$DOTNET_ROOT:$DOTNET_ROOT/tools"

RUN dotnet workload install maui-android

RUN wget https://dl.google.com/android/repository/commandlinetools-linux-11076708_latest.zip
RUN unzip commandlinetools-linux-11076708_latest.zip -d /usr/lib/android-sdk
ENV ANDROID_SDK_ROOT=/usr/lib/android-sdk
RUN yes | sdkmanager --sdk_root=/usr/lib/android-sdk "platform-tools" "build-tools;34.0.0" "platforms;android-34" "platforms;android-34-ext8" "platforms;android-34-ext10" "platforms;android-34-ext11" "platforms;android-34-ext12"
RUN yes | sdkmanager --sdk_root=/usr/lib/android-sdk --licenses
