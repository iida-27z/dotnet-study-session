FROM node:26.13-trixie-slim AS node-env
FROM mcr.microsoft.com/dotnet/sdk:10.0.403 AS dotnet-env

COPY --from=node-env /usr/local/bin/node /usr/local/bin/node
COPY --from=node-env /usr/local/lib/node_modules /usr/local/lib/node_modules
RUN ln -s /usr/local/lib/node_modules/npm/bin/npm-cli.js /usr/local/bin/npm && \
    ln -s /usr/local/lib/node_modules/npm/bin/npx-cli.js /usr/local/bin/npx

ENV LANG=ja_JP.UTF-8
ENV LC_ALL=ja_JP.UTF-8
ENV LANGUAGE=ja_JP:ja

ARG USERNAME=dev
ARG USER_UID=1000
ARG USER_GID=1000

RUN if id -u ${USER_UID} >/dev/null 2>&1; then userdel -r $(id -nu ${USER_UID}); fi && \
    if getent group ${USER_GID} >/dev/null 2>&1; then groupdel ${USER_GID}; fi

RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    git \
    gh \
    curl \
    jq \
    gnupg \
    postgresql-client \
    locales \
    sudo && \
    groupadd --gid ${USER_GID} ${USERNAME} && \
    useradd --uid ${USER_UID} --gid ${USER_GID} --create-home --shell /bin/bash ${USERNAME} && \
    echo "${USERNAME} ALL=(ALL) NOPASSWD:ALL" > /etc/sudoers.d/${USERNAME} && \
    chmod 0440 /etc/sudoers.d/${USERNAME} && \
    locale-gen ja_JP.UTF-8 && \
    update-locale LANG=ja_JP.UTF-8 && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

ARG GITLEAKS_VERSION=8.30.1
RUN curl -fsSL \
    https://github.com/gitleaks/gitleaks/releases/download/v${GITLEAKS_VERSION}/gitleaks_${GITLEAKS_VERSION}_linux_x64.tar.gz \
    | tar -xz -C /usr/local/bin gitleaks

RUN mkdir -p /etc/apt/keyrings && \
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg | gpg --dearmor -o /etc/apt/keyrings/docker.gpg && \
    echo "deb [signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu noble stable" | tee /etc/apt/sources.list.d/docker.list && \
    apt-get update && \
    apt-get install -y --no-install-recommends docker-ce-cli docker-compose-plugin && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

USER ${USERNAME}

RUN curl -fsSL https://antigravity.google/cli/install.sh | bash

WORKDIR /workspace/app

CMD [ "sleep", "infinity" ]
