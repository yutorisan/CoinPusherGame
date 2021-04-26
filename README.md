# MedalPusherGame

ゲームセンターでよくあるプッシャー系メダルゲームです。

## 概要

プッシャー系メダルゲームをUnityで作成しました。500枚の所持メダルを投入し、スロットを回転させ、ジャックポットを狙います。メダルの獲得時には心地よいサウンドとエフェクトが再生されるため、ジャックポットにより大量のメダルを一気に獲得する爽快感を味わうことができます。

## 注意事項

本リポジトリはソースコード一式を含んだUnityプロジェクトが格納されていますが、AssetStoreよりダウンロードした各種アセットは含まれていないため、クローンしても動作しません。あくまで、ソースコードの参照用としてご利用ください。

## デモンストレーション

下記動画をご覧ください。

[![](https://img.youtube.com/vi/g6-gXzIgOWM/0.jpg)](https://www.youtube.com/watch?v=g6-gXzIgOWM)

また、下記リンクからゲーム本体をダウンロード頂けます。

[macOS版](https://www.dropbox.com/s/nvjndcine736alj/MedalPusherGame_ver.0.1.app.zip?dl=0)
[Windows版](https://www.dropbox.com/s/hkabaknuigboj35/MedalPusherGame_ver.0.1.exe.zip?dl=0)

当方、Macのみ所持のためmacOS版のみ動作確認済みです。
動作確認環境は以下のとおりです。

- macOS BigSur 11.2.3
- 2.4GHz 8コア Intel Corei9
- 32GB RAM
- AMD Radeon Pro 5500M 8GB

※1920*1080以上の解像度を持つディスプレイが必要です。


## 操作方法

**スペースキー**：所持しているメダルを投入します。

## 遊び方

ゲームを起動すると、プッシャー台（以下フィールド）にメダルが補充されます。

![GameStart](https://user-images.githubusercontent.com/60573963/115537661-04133200-a2d6-11eb-82c2-071b1c2d87f5.gif)

ゲーム開始時に所持している500枚のメダルを投入して、フィールド上にあるメダルを手前に押し出し、メダルを獲得しましょう。
左右に落ちてしまったメダルは獲得できません。

### スロット

左右に動いているチェッカーにメダルが入ると、スロットによる抽選が開始されます。

![MedalChecker](https://user-images.githubusercontent.com/60573963/115537420-c44c4a80-a2d5-11eb-94e8-98efb1a7b897.gif)

スロットは一定の確率で「ハズレ」「リーチ」「あたり」のいずれかとなり、「リーチ」になった場合は3種類のリーチ演出の後、「はずれ」か「あたり」に遷移します。
「あたり」になった場合は、一律で30枚のメダルを獲得し、自動的にフィールド上に投入されます。
　　　
### ジャックポッド

スロットが「7」で揃うと、**ジャックポッドアイテムボール**がフィールド上に払い出されます。

<img width="495" alt="image" src="https://user-images.githubusercontent.com/60573963/115537955-53f1f900-a2d6-11eb-8b88-dcff56eaf768.png">

このボールを落として獲得することで、回転する抽選台を使用したジャックポッドチャンスに挑戦することができます。

ジャックポッドチャンスに突入すると、ゲーム画面が自動的に抽選台付近に移動します。
1個のボールが回転する抽選台に投入され、入った穴に書いてある枚数分だけメダルを獲得することができます。
ここで獲得したメダルは、通常の投入方法ではなくフィールドの上部から一気に払い出されます。

![2021-04-21 19 21 01](https://user-images.githubusercontent.com/60573963/115538388-c8c53300-a2d6-11eb-8e38-a55e6f798611.gif)

### ゲームの開始と終了

現状、ゲームの終了は実装されていません。
メダルが無くなった時点でゲーム終了となり、OS標準のアプリケーションを閉じるコマンドを使ってゲームを修了します。

またゲームのセーブ・ロード機能もまだ実装されていないため、ゲームを起動すると毎回初期状態から開始されます。
ゲームのセーブ・ロード機能は、今後実装予定です。

## ソフトウェア設計説明書

ソフトウェアの設計説明書は下記ドキュメントをご覧ください。
https://gist.github.com/yutorisan/e86ff9f169a22ac781535d6cb7504dce

## 開発環境

- Unity 2021.1.2

## 使用アセット・素材

- [UniRx](https://assetstore.unity.com/packages/tools/integration/unirx-reactive-extensions-for-unity-17276)
- [UniTask](https://github.com/Cysharp/UniTask)
- [Extenject](https://assetstore.unity.com/packages/tools/utilities/extenject-dependency-injection-ioc-157735)
- [Odin - Inspector and Serializer](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041)
- [DOTween Pro](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416)
- [Coins Mega Pack](https://assetstore.unity.com/packages/2d/gui/icons/coins-mega-pack-141998)
- [2D Game Starter Assets](https://assetstore.unity.com/packages/2d/environments/2d-game-starter-assets-24626)
- [LowPoly Water](https://assetstore.unity.com/packages/tools/particles-effects/lowpoly-water-107563)
- [無料効果音](https://taira-komori.jpn.org/freesound.html)
- [MusMus](https://musmus.main.jp/info.html)

## 作者

[@yutorisan19](https://twitter.com/yutorisan19)

## ライセンス

MIT
