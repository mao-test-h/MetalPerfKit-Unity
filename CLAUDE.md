# CLAUDE.md

## プロジェクト概要

こちらは Unity for iOS 環境で Metal の開発機能(Metal Performance HUD など) を利用するためのプロジェクトです。
パッケージは他のプロジェクトからも参照できるように Unity Package Manager 形式で分離してます。

### 開発環境

- Unity 2022.3.62f2
  - JetBrains Rider
  - VSCode
- Xcode 26.0.1

### プロジェクト構造

- **Assets**
  - **_Example/**  : プラグインの実装例
    - **Runtime/**        : 実行時コード (.cs)
    - **Scenes/**         : シーンファイル
    - **Settings/**       : Input Systemなどの設定ファイル

------------------------------

# ネイティブプラグイン開発

iOS のネイティブプラグインの実装方法については以下のドキュメントを参照すること。

- ./Docs/Unity-iOS-Plugin-Patterns.md
