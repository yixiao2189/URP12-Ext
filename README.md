# URP12-Ext 

项目使用线性工空间，场景相机和UI相机各自独立的分辨率，保持UI的清晰度，同时降底主相机的能耗。
同时UI能够保持在 Gamma 颜色空间下工作。

相机栈：

BaseCamera 【线性空间 renderScale: <1】

OtherCamera 【缺省，线性空间 renderScale <1】

UICamera 【Gamma空间 renderScale 1】

特性1：

同时能够在UICamera相机之前，处理fsr fxaa等后处理。

1 避免抗抗锯齿影响UI效果。

2 fsr借机升采。（经测试：移动端推荐不开，仅分离分辨率就行）

特性2：

使用URP12的SwapBuffer，Gamma以及分隔分辨率特性不会额外产生RT开销
