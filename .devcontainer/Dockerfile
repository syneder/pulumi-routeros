FROM mcr.microsoft.com/devcontainers/base:ubuntu

RUN echo \
    && sudo apt-get update \
    && DEBIAN_FRONTEND=noninteractive sudo apt-get install --yes --purge --assume-yes \
        curl build-essential

SHELL ["/bin/bash", "-c"]

RUN apps=('pulumi/x64' 'esc/x64' 'pulumictl/amd64') && for app in "${apps[@]}"; do echo \
        && IFS="/" read -r -a APP_CONFIG <<< "${app}" \
        && export EXEC_CURL="/usr/bin/curl --silent --show-error --tlsv1.2 --location" \
        && export REPOSITORY="pulumi/${APP_CONFIG[0]}" \
        && export VERSION="$(${EXEC_CURL} https://api.github.com/repos/${REPOSITORY}/releases/latest | jq --raw-output .tag_name)" \
        && export URL="https://github.com/${REPOSITORY}/releases/download/${VERSION}/${APP_CONFIG[0]}-${VERSION}-linux-${APP_CONFIG[1]}.tar.gz" \
        && ${EXEC_CURL} ${URL} | tar xzvf - --directory /tmp ; \
    done

RUN echo \
    && sudo chmod 755 /tmp/pulumi/* \
    && sudo chown root:root /tmp/pulumi/* \
    && sudo mv /tmp/pulumi/* /usr/local/bin/ \
    && sudo install -m 755 -o root -g root /tmp/esc/esc /usr/local/bin/esc \
    && sudo install -m 755 -o root -g root /tmp/pulumictl /usr/local/bin/pulumictl

RUN sudo rm -rf \
    /var/lib/{apt,cache,log} \
    /usr/share/{doc,man,locale} \
    /var/cache/apt \
    /home/*/.cache \
    /root/.cache \
    /var/tmp/* \
    /tmp/*
