docker build . -t mauidroid
docker run --rm -v ${PWD}:/src -v ${PWD}/build:/build mauidroid bash -c /src/dockerbuild.sh
