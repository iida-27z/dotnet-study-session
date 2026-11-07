# 開発フロー・CI/CDガイドライン

## 1. 目的
- 本ガイドラインは、開発からリリースまでの品質保証とデプロイ手順を標準化する。

## 2. 基本方針
- 作業ブランチの push では軽量 CI を実行する。
- `main` 向け PR では非軽量 CI を実行する。
- `main` へのマージ後デプロイは手動承認付きで実行する。
- CD の接続情報は GitHub Secrets から取得する。

## 3. 開発フロー
1. 作業ブランチを作成する（`<type>/<description>`）。
2. 実装・テスト・フォーマットをローカルで実行する。
3. push して軽量 CI の結果を確認する。
4. `main` 向け PR を作成する。
5. 非軽量 CI 成功後にレビューを受ける。
6. マージ後、手動承認を経て CD を実行する。

## 4. ローカル事前チェック

```bash
dotnet restore
dotnet build --no-restore
dotnet format --verify-no-changes
dotnet test
```

## 5. 軽量 CI（作業ブランチ push）

### 5.1 対象
- `main` 以外のブランチへの push。

### 5.2 目的
- フィードバックを高速化し、初期段階で明らかな不具合を検知する。

### 5.3 実施内容
- `dotnet restore`
- `dotnet build --no-restore`
- Unit Test 実行
- gitleaks 実行
- テストレポートの生成

## 6. 非軽量 CI（main 向け PR）

### 6.1 対象
- `main` 向け PR。

### 6.2 目的
- `main` 向け PR に対して包括的な CI を実行し、品質保証を行う。

### 6.3 実施内容
- `dotnet restore`
- `dotnet build --no-restore`
- `dotnet format --verify-no-changes`
- Unit Test
- Integration Test（TestContainers を含む）
- カバレッジ測定（閾値判定は実施しない）
- 追加セキュリティスキャン（gitleaks 以外）
- 依存関係脆弱性チェック（Dependabot/監査系）
- テストレポート等の生成

### 6.4 成果物
- テストレポートをアーティファクトとして保存する。
- セキュリティスキャン結果をアーティファクトとして保存する。

## 7. CD（手動承認付き）

### 7.1 実行方式
- GitHub Environments の Required reviewers を使用し、承認後にデプロイジョブを実行する。

### 7.2 デプロイ処理
1. 実行可能ファイルをビルドして成果物化する。
2. SSH 経由で Linux サーバーへ配置する。
3. 配置後にアプリケーションを起動または再起動する。

### 7.3 Secretsの利用
- デプロイ処理で必要な情報はすべて GitHub Secrets から取得する。
    - `DEPLOY_HOST`
    - `DEPLOY_PORT`
    - `DEPLOY_USER`
    - `DEPLOY_SSH_KEY`
    - `DEPLOY_TARGET_PATH`

## 8. GitHub 機能の活用
- Dependabot Alerts & Security Updates を有効化し、脆弱性修正を優先的に対応する。
- Dependabot Version Updates を設定し、依存関係のバージョン更新 PR を自動作成する。
- Dependency Review Action を導入し、プルリクエスト時の依存関係変更（脆弱性やライセンス）を自動チェックする。
- CodeQL を有効化し、コードの静的解析を行う。
- Secret Scanning & Push Protection を有効化し、リポジトリ内の機密情報漏洩を防止する。

## 9. 更新ルール
- CI/CD フローの変更はワークフロー定義変更と同一 PR で本ドキュメントを更新する。
