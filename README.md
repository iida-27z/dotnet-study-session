# 社内勉強会管理システム

![GitHub Repo](https://img.shields.io/badge/GitHub-repo-373e47?logo=github)
![For VS Code](https://img.shields.io/badge/for-VS%20Code-007ACC?logo=visualstudiocode&logoColor=white)
![.NET Version](https://img.shields.io/badge/dotnet-10-512BD4)
![xUnit](https://img.shields.io/badge/Test-xUnit-6d43b8)
![CI](https://github.com/iida-27z/dotnet-study-session/actions/workflows/test.yml/badge.svg?branch=main)
![GitHub Stars](https://img.shields.io/github/stars/iida-27z/dotnet-study-session?style=social)

## 概要
このリポジトリは、社内勉強会の管理を効率化するためのシステムのソースコードを管理するものです。

## 動作環境
- **C#**: 14
- **.NET SDK**: 10
- **テストフレームワーク**: xUnit
- **データベース**: PostgreSQL
- **コンテナ環境**: Docker
- **推奨開発環境**: GitHub Codespaces(VS Code + DevContainer )

## ディレクトリ構成
主なディレクトリ構成は以下の通りです。
```
.
├── .devcontainer/                         # DevContainer設定（Codespaces環境定義）
├── .github/                               # GitHub設定・CI/CDワークフロー
├── .githooks/                             # Gitフック設定
├── .vscode/                               # VS Code設定
├── docs/                                  # 各種ドキュメント
│   ├── acceptance/                        # 受け入れ基準
│   ├── guideline/                         # 開発規約
│   ├── mocks/                             # 画面モック
│   └── specs/                             # 仕様書
├── src/                                   # ソースコード
│   ├── StudySession.Domain/               # ドメイン層
│   ├── StudySession.Application/          # アプリケーション層
│   ├── StudySession.Infrastructure/       # インフラ層
│   ├── StudySession.Web/                  # プレゼンテーション層(Webアプリ)
│   └── StudySession.Batch/                # プレゼンテーション層(バッチ処理)
├── tests/                                 # テストプロジェクト (xUnit)
│   ├── StudySession.DomainTests/          # ドメイン層
│   ├── StudySession.ApplicationTests/     # アプリケーション層
│   ├── StudySession.InfrastructureTests/  # インフラ層
│   ├── StudySession.WebTests/             # プレゼンテーション層(Webアプリ)
│   ├── StudySession.BatchTests/           # プレゼンテーション層(バッチ処理)
│   └── StudySession.IntegrationTests/     # 統合テスト
├── .dockerignore                          # Dockerビルド除外設定
├── .editorconfig                          # エディタ・コードスタイル設定
├── .gitattributes                         # Git 属性設定
├── .gitignore                             # Git 除外設定
├── Directory.Build.props                  # 共通ビルド設定
├── dockerfile.batch                       # Batch 処理実行コンテナイメージ作成
├── dockerfile.web                         # Web アプリ実行コンテナイメージ作成
├── gitleaks.toml                          # gitleaks 設定ファイル
├── global.json                            # .NET SDK バージョン指定
├── README.md                              # 本ファイル
└── StudySession.slnx                      # ソリューションファイル
```

## クイックスタート
このプロジェクトは、.NET と PostgreSQL を前提に動作します。開発時は GitHub Codespaces や Dev Container を使うと、アプリと DB をまとめて起動できます。

### 1. 開発環境を起動
GitHub から GitHub Codespaces を利用するか、VS Code でこのリポジトリを開き、Dev Container で再度開いてください。  
`.devcontainer/docker-compose.yml` により、Web アプリ用の実行環境と PostgreSQL が自動で起動します。
なお、`.devcontainer/devcontainer.json` の `initializeCommand` は、Linux コマンドとして実行されるため、Windows 環境では、 WSL の利用を推奨します。

### 2. 依存関係の取得と検証
```bash
dotnet restore
dotnet build
dotnet test
```

### 3. Web アプリを起動
Web アプリを起動する場合は、`.vscode/launch.json` の設定を利用して、VS Code からデバッグ実行することも可能です。
また、コマンドラインから起動する場合は、以下のように実行します。
```bash
dotnet run --project src/StudySession.Web/StudySession.Web.csproj --launch-profile http
```

### 4. Batch 処理を起動
Batch 処理を起動する場合は、`.vscode/launch.json` の設定を利用できますが、デフォルトではWebアプリが起動するため、サイドバーの「実行とデバッグ」から対象の構成を選択して実行してください。
また、コマンドラインから直接実行することも可能です。

```bash
dotnet run --project src/StudySession.Batch/StudySession.Batch.csproj
```
