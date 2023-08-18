# URP12-Ext 

项目使用线性工空间，场景相机和UI相机各自独立的分辨率，保持UI的清晰度，同时降底主相机的能耗。
同时UI能够保持在 Gamma 颜色空间下工作。

适合FairyGUI工程流。

相机栈：

  Base-Camera 【线性空间 renderScale: <1】
  
  Overlay-OtherCamera 【缺省，线性空间 renderScale <1】
  
  Overlay-UICamera 【Gamma空间 renderScale 1】

特性1：

fsr fxaa等finalPost时机移到UICamera相机之前。

  1 避免抗抗锯齿影响UI效果。
  
  2 fsr借机升采。（经测试：移动端推荐不开，仅分离分辨率就行）

特性2：

使用URP12的SwapBuffer，Gamma以及分离分辨率特性都不会额外产生RT开销
Gamma以及分离分辨率特性可独立开启。


使用方法：

Overlay相机 有一个选项Color Space选项:

  Uninitialized URP原始模式
  
  Gamma 本相机开启Gamma，同时启用了分离分辨率特性，此时renderScale仅对排序在此相机之前的相机生效。
  
  Linear  启用了分离分辨率特性，此时renderScale仅对排序在此相机之前的相机生效。
