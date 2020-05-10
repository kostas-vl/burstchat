#! /bin/sh

create_database_conf() {
    mkdir database_conf || true
    cd database_conf
    git clone https://github.com/asterisk/asterisk.git
    cd asterisk
}

build_docker_image() {
    docker build -t burstchat_asterisk .
}


