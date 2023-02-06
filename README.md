# Hide And Beep　

# description 
* タイトル:Hide & Beep
* ジャンル:アクション&ホラー
* ターゲットプラットフォーム:PC
* 制作期間 2022/11/14〜2022/12/14
* [Proposal Document](./HideAndBeep_Proposal.pdf)


# Demo
**Pleaze click image, then video will start.**
[!['altテキスト'](./topImage.png)](https://youtu.be/KxBCKCH05N8)

# Rule
* プレイヤーは門を開けて小学校を**脱出すること**が目的です。<br>
* 敵のゾンビに触れてしまうと**ゲームオーバー**です。<br>
* 各部屋には、**インタラクトできるオブジェクト**があります。<br>
* 大きな音を出すオブジェクトに触ると、**ゾンビを怯ませる**ことができます。<br>
* しかし、**音のなるオブジェクトは、ゾンビを引き寄せる原因**にもなります。<br>
* うまくゾンビを誘導して、**隙を狙って脱出**しましょう。<br>

# Features
* マップ上の音を管理し、優先度の高い順にゾンビが索敵します。<br>
![サウンドのデモ画像](./soundDemo.gif "サウンド管理のデモ画像")<br>
* PUN2（PhotonUnityNetworking2）を用いたP2P通信<br>
![P2P通信のデモ画像](./SampleP2P.gif "P2P通信のデモ画像")<br>

# Requirement
* Unity 2021.3.11f1

# Note
this repository rid font assets off because the size is too large.
so you need to download font assets corresponding to Japanese.

# Author
* 一ノ宮綾平
* 九州デザイナー学院ゲームCG学科ゲームプログラミング専攻

# Class Diagram (now on Creating)
```
graph TD
    A[Christmas] -->|Get money| B(Go shopping)
    B --> C{Let me think}
    C -->|One| D[Laptop]
    C -->|Two| E[iPhone]
    C -->|Three| F[fa:fa-car Car]
```
