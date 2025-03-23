FROM ubuntu:24.10

ENV DEBIAN_FRONTEND=noninteractive
WORKDIR /root

# Setup
RUN apt update && \
	apt install -y git wget make apt-transport-https software-properties-common dotnet-sdk-8.0 vim build-essential python3-dev automake cmake flex bison libglib2.0-dev libpixman-1-dev python3-setuptools cargo libgtk-3-dev locales
RUN apt install -y lld-14 llvm-14 llvm-14-dev clang-14 || sudo apt-get install -y lld llvm llvm-dev clang
RUN apt install -y gcc-$(gcc --version|head -n1|sed 's/\..*//'|sed 's/.* //')-plugin-dev libstdc++-$(gcc --version|head -n1|sed 's/\..*//'|sed 's/.* //')-dev
RUN git clone https://github.com/Metalnem/sharpfuzz/

# AFL++
RUN git clone https://github.com/AFLplusplus/AFLplusplus && \
	cd AFLplusplus && \
	make all && \
	make install && \
	cd .. && \
	rm -rf AFLplusplus

# dotnet tools
RUN apt-get install -y dotnet-sdk-9.0
RUN dotnet tool install -g SharpFuzz.CommandLine
RUN dotnet tool install -g powershell
ENV PATH="${PATH}:/root/.dotnet/tools"

WORKDIR /Source/
COPY . .
RUN echo "pwsh /root/sharpfuzz/scripts/fuzz.ps1 /Source/FuzzingTests/FuzzingTests.csproj -i /Source/TestCases" > run.sh && \
    chmod +x run.shcd 