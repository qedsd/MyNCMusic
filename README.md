# 网郁云音乐

使用UWP开发的网易云第三方客户端

## 系统要求
windows 10 1809及更高

## 使用说明
- 项目仅提供UWP前端，后端使用[NeteaseCloudMusicApi](https://github.com/Binaryify/NeteaseCloudMusicApi)，请自行配置后端并在应用的设置里填写上你的服务器IP。（非程序员看不懂黑话？就是说这个应用并不是直接登录账号就能用，还需要使用[NeteaseCloudMusicApi](https://github.com/Binaryify/NeteaseCloudMusicApi)在本地/云搭建个服务器）

- 如果你是使用本地服务器，需要注意UWP默认是不允许访问localhost的，要想成功连接上本地服务器，需要把当前应用的localhost打开，具体请百度。

- 初次使用请在应用设置里填写服务器ip（必填）、账号（仅支持手机号和163邮箱）以及密码。

## 代码规范？
这个，怎么说呢，嗯，凑合看吧 :)

整体代码较为简单粗暴，功能不复杂，写的时候就怎么方便怎么来了

## 安装包

只提供X86的安装包，其他的请自行打包

[MyNCMusic_1.0.0.0](https://github.com/qedsd/MyNCMusic/releases/download/1.0.0.0/MyNCMusic_1.0.0.0_Test.zip)

## 已实现功能

- 每日推荐歌单
- 每日推荐歌曲
- 搜索（音乐、歌手、专辑、歌单）
- 创建/收藏的歌单
- 收藏的歌手、专辑
- 歌单、音乐评论
- 歌词（滚动有点问题，凑合吧）

一句话：凑合能用

## 没图你说个JB

![初始化](https://github.com/qedsd/MyNCMusic/blob/master/Img/initial.png)

![主界面](https://github.com/qedsd/MyNCMusic/blob/master/Img/home.png)

![列表](https://github.com/qedsd/MyNCMusic/blob/master/Img/list.png)

![播放界面](https://github.com/qedsd/MyNCMusic/blob/master/Img/playing.png)

![歌单](https://github.com/qedsd/MyNCMusic/blob/master/Img/playlist.png)

![评论](https://github.com/qedsd/MyNCMusic/blob/master/Img/comment.png)

![专辑](https://github.com/qedsd/MyNCMusic/blob/master/Img/album.png)
