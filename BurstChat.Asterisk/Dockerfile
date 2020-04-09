# The os image.
FROM ubuntu

# Installing asterisk from the package manager.
RUN apt-get update
RUN apt-get install -y asterisk

# Transfering all edited configuration files to the new image.
COPY ./modules.conf /etc/asterisk/
COPY ./http.conf /etc/asterisk/
COPY ./pjsip.conf /etc/asterisk/
COPY ./rtp.conf /etc/asteriks/
COPY ./extensions.conf /etc/asterisk/

# Starting asterisk in the foreground with the cli.
# CMD ["/usr/sbin/asterisk", "-c"]
