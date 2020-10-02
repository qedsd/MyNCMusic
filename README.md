# 网郁云音乐

使用UWP开发的网易云第三方客户端

## 系统要求
windows 10 1809及更高

## 使用说明
- 项目仅提供UWP前端，后端使用[NeteaseCloudMusicApi](https://github.com/Binaryify/NeteaseCloudMusicApi)，请自行配置后端并在应用的设置里填写上你的服务器IP。（非程序员看不懂黑话？就是说这个应用并不是直接登录账号就能用，还需要使用[NeteaseCloudMusicApi](https://github.com/Binaryify/NeteaseCloudMusicApi)在本地/云搭建个服务器）

- 如果你是使用本地服务器，需要注意UWP默认是不允许访问localhost的，要想成功连接上本地服务器，需要把当前应用的localhost打开，具体请百度。

- 初次使用请在应用设置里填写服务器ip（必填）、账号（仅支持手机号和163邮箱）以及密码。


## 安装包

只提供X86的安装包（内含安装说明），其他的请自行打包

[MyNCMusic_1.0.1.0](https://github.com/qedsd/MyNCMusic/releases/download/1.0.1.0/MyNCMusic_1.0.1.0_Test.zip)

## 已实现功能

- 每日推荐歌单
- 每日推荐歌曲
- 搜索（音乐、歌手、专辑、歌单）
- 创建/收藏的歌单
- 收藏的歌手、专辑
- 歌单、音乐评论
- 歌词（滚动有点问题）

一句话：凑合能用

## Q&A

1. **填了服务器还是无法获取数据？**

       服务器地址最后不要有`\`
1. **拖动进度条有时候卡住、没反应？**

       应该是没缓冲好，要不就是BUG了
1. **评论不全？**

       默认只显示100个评论
1. **日推歌单时多时少？**

        哈，神奇吧，我也不知道为什么
1. **可以随便使用别人的服务器吗？**

        千万别！服务器上面是可以看到你登录的账号信息的
        
1. **部分专辑图导致背景过白看不清文字？**

       界面大部分文字颜色都由MainPage的`mainSolidColorBrush`控制，默认是白色。一开始设想是由`GetMajorColorAndBlur`获取背景图片（专辑图）的主颜色，判断是否过白从而修改`mainSolidColorBrush`避免看不清文字，后来发现`GetMajorColorAndBlur`的结果并不佳，就先不启用。
1. **你写的什么jb代码，怎么全部挤到一个MyClass文件里了？**

        一开始这只是一个测试文件，后来......懒得挪了，挤就挤吧，又不是不能用（手动滑稽）
1. **同一个玩意的类为什么有好几个？**

       一键JSON转换类，你懂的
1. **代码规范？！**

       这个，怎么说呢，嗯，凑合看吧 :) 功能简单，代码较为简单粗暴，写的时候就怎么方便怎么来了


## 没图你说个文明用语
（看不了的去Img文件夹下看）

![初始化](https://github.com/qedsd/MyNCMusic/blob/master/Img/initial.png)

![主界面](https://github.com/qedsd/MyNCMusic/blob/master/Img/home.png)

![列表](https://github.com/qedsd/MyNCMusic/blob/master/Img/list.png)

![播放界面](https://github.com/qedsd/MyNCMusic/blob/master/Img/playing.png)

![歌单](https://github.com/qedsd/MyNCMusic/blob/master/Img/playlist.png)

![评论](https://github.com/qedsd/MyNCMusic/blob/master/Img/comment.png)

![专辑](https://github.com/qedsd/MyNCMusic/blob/master/Img/album.png)
