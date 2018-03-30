# Setup the environment

## Install libindy
    apt-key adv --keyserver keyserver.ubuntu.com --recv-keys 68DB5E88
    sudo add-apt-repository "deb https://repo.sovrin.org/sdk/deb xenial master"
    sudo apt-get update
    sudo apt-get install -y libindy

## Install indy-cli
    apt-key adv --keyserver keyserver.ubuntu.com --recv-keys 68DB5E88
    sudo add-apt-repository "deb https://repo.sovrin.org/sdk/deb xenial stable"
    sudo apt-get update
    sudo apt-get install -y indy-cli

## Start local nodes pool with docker
    docker build -f indy_pool.dockerfile -t indy_pool .
    docker run -itd -p 9701-9708:9701-9708 indy_pool

## Install SQL Server in Docker
https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker

