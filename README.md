## .net写的数字证书生成工具    
通过IDA反汇编gatherosstate后提取伪代码.再通过x64dbg调试验证写成的c#代码    

![image](https://github.com/laomms/gatherosstate/blob/master/pic.png)   

![image](https://github.com/laomms/gatherosstate/blob/master/test.png)   


获取硬件ID：
```c#
 long shortHwid = HWID.HwidGetCurrentExShort(out byte[] hwidBlock);

```


