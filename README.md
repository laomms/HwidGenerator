.net写的数字证书生成工具

![image](https://github.com/laomms/gatherosstate/blob/master/pic.png)   

![image](https://github.com/laomms/gatherosstate/blob/master/test.png)   


基本流程：
```php
# GetVersionExW 获取系统版本信息 转成格式 OSMajorVersion=%d;OSMinorVersion=%d;OSPlatformId=%d
# HwidGetCurrentEx 获取系统硬件信息数组  
# HwidCreateBlock 通过算法和合并转成新的数组
# Base64Encode 将数组BASE64加密,取加密的结果连接成Hwid=%s格式,与刚才取到的系统信息拼接成拼接字符串:OSMajorVersion=%d;OSMinorVersion=%d;OSPlatformId=%d;PP=%d;Hwid=%s
# GetActiveWindowsSkuStatus 获取系统激活状态
# CompareSkuChannel 判断默认SKU是Retail还是GVLK
# SkuGetUpgradeProductKeyEx 获取默认PFN(package family name),组成Pfn=%s格式,并根据该PFN查表得到默认的密钥及ID(DPID3)值
# SLGetServiceInformation 是否为BiosProductKey
# SLGetWindowsInformationDWORD 判断当前系统的激活状态.字符串后面加"DownlevelGenuineState=1;")
# Base64Encode 将组合后的字符串用base64加密,将得到的加密结果就是sessionid
# SLGetGenuineInformation 根据SL_GET_GENUINE_AUTHZ:%s判断当前系统是否有有效的用户证书
# GetSystemTime(&SystemTime)获取系统时间
# UtcTimeToIso8601组成TimeStampClient=系统时间格式,与sessionid拼接
# ComputeHash 利用CryptCreateHash、CryptHashData、CryptGetHashParam组成的RSA加密算法,生成32位数组
# VRSAVaultSignPKCS RSA签名数组算法,申请一个256大小的数组空间，通过ApplyPKCS1SigningFormat是逆序RSA数组然后连接一个固定数组,利用VbnRsaVault_ModExpPriv_clear将这个256字节的数组进行加密算法.最后生成256位数组.加密签名数组成base64字符串.
# 将字符串"downlevelGTkey"和固定公钥key=BgIAAACkAABSU0ExAAgAAAEAAQARq+V11k+dvHMCaLWVCaSbeQNlOdWTLkkl0hdMh5V3YhLU2R4h0Jd+7k7qfZ4aIo4ussduwGgmyDRikj5L2R77GG2ciHk4i8siK8qg7frOU0KT5rEks3qVj38C3dS1wS6D67shBFrxPlOEP8+JlelgP7Gxmwdao7NF4LXZ3+KdbJ//9jkmN8iAOP0N2XzW0/cJp9P1q6hE7eeqc/3Qn3zMr0q1Dx7vstN98oV17hNYCwumOxxS1rH+3n7ap2JKRSelo8Jvi214jZLBL+hOtYaGpxs7zIL3ofpoaYy5g7pc/DaTvyfpJho5634jK7dXVFMpzJZMn9w0F/3rkquk0Amm"以及签名数组的base64字符串拼接成完整的签名字符串.将sppclient与<genuineproperties origin="%s">拼接成<genuineProperties origin="sppclient">.把剩下的XML标签补全,<properties>%s</properties> <signatures>%s</signatures> <signature name="downlevelGTkey" method="rsa-sha256" key="%s" </signature>...
# 生成数字证书

```


