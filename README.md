# 概要
- ChatGPT APIとWhisper API（ともにOpenAI社）を活用したAIアシスタントです。
- 完成物
https://user-images.githubusercontent.com/44228430/224550419-ee4f1cf0-7b2c-4375-b1dd-4eb7e23de5c0.mp4

# 注意
- Constants.csに、自分のOpenAI APIキーを入れてください

# 構成
- Scene
  - AIAssistantScene.scene...音声認識とレスポンスを合わせた完成形です。
  - AIAssistantSceneDecoration.scene...上記sceneの背景やデザインを工夫したsceneです。
  - ChatGPTScene.scene...テキストベースのリクエスト・レスポンスのみを行っています（WhisperAPIを利用していません）
