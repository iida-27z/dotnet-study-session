# Git・GitHubガイドライン

## 1. 目的
- 本ガイドラインは、変更履歴の可読性・再現性・レビュー効率を担保するための Git/GitHub 運用ルールを定義する。

## 2. ブランチ戦略
- ブランチ戦略は GitHub-Flow を採用する。
- `main` は常にデプロイ可能な状態を維持する。
- 開発は作業ブランチで行い、`main` へ直接 push しない。
- `main` への反映は Pull Request（以下 PR）経由のみとする。

## 3. ブランチ命名規則

### 3.1 形式
- `<type>/<description>`
- `description` は英小文字-kebab-case を使用する。

### 3.2 type 一覧
- `feat`: 機能追加
- `fix`: 不具合修正
- `perf`: 性能改善
- `refactor`: リファクタリング
- `test`: テスト追加・修正
- `docs`: ドキュメント変更
- `build`: ビルド設定変更
- `ci`: CI/CD 変更
- `chore`: 雑務・保守作業

### 3.3 命名例
- `feat/study-session-create-page`
- `fix/login-error-message`
- `ci/add-integration-test-job`

## 4. コミットメッセージ規約

### 4.1 形式
```text
<type>(<scope>): <subject>

<body>
```

### 4.2 記述ルール
- `type` は英語のまま記述する。
- `subject` の直前は半角スペースで区切る。
- `scope`、`subject`、`body` は日本語で記述する。
- 1コミット1目的を原則とする。

### 4.3 例
```text
feat(web): 勉強会作成画面の入力検証を追加

必須項目未入力時にサーバー側バリデーションでエラーを返すように修正。

```
### 4.4 補足
- コミットメッセージのテンプレートとして、 `.gitmessage` をリポジトリに配置している。
- `.gitmessage` は`devcontainer.json` の `postCreateCommand` で自動的に設定される。

## 5. Pull Request 運用

### 5.1 必須ルール
- `main` へのマージは PR 必須。
- 承認者数はチーム裁量とする。
- CI が成功していない PR はマージしない。

### 5.2 PR タイトル
- 原則としてコミット規約と同様に `type(scope): subject` 形式を使用する。

### 5.3 PR 説明欄テンプレート
```markdown
## 概要

このPRの背景や目的、効果を簡潔に記述してください。

## 変更内容

- 主な変更点を箇条書きで記述

## 動作確認

- [ ] ローカルでの動作確認済み
- [ ] すべてのテストが成功している

## 関連ドキュメント（あれば）
- docs/specs/xx_xxx.md

## 関連Issue（あれば）

Fixes #
```

## 5.4 補足
- PR 説明欄のテンプレートとして、 `.github/PULL_REQUEST_TEMPLATE.md` をリポジトリに配置している。

## 6. マージルール
- マージ前に最新の `main` を取り込み、競合を解消する。
- 履歴可読性を保つため、マージ方式は `Squash and merge` を推奨する。

## 7. セキュリティ・品質ルール
- pre-commit フック（gitleaks）を無効化しない。
- 秘密情報（トークン、鍵、パスワード）をコミットしない。
- Dependabot など GitHub の無料機能による通知・更新提案を活用する。

## 8. 禁止事項
- `main` への直接 push。
- 意図不明な大量変更コミット。
- コミット規約から逸脱したメッセージ。
- レビュー前の自己判断マージ。

## 9. 更新ルール
- ブランチ運用・コミット規約・PR運用を変更する場合、本ドキュメントを同一 PR で更新する。

