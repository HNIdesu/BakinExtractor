## Data Structure

**DefaultByteOrder: LittleEndian**

**BKNPACK**
| Name | Size(Byte) | Type | Value | Comment |
| --- | --- | --- | --- | --- |
| signature | 6 | string | BKNPAK |  |
| dataVersionNo | 2 | ushort |  | big endian byte order |
| resourceTableOffset | 8 | long |  | seek after read the value |
| loadingFormTitle | * | CustomString | Loading... | title of the loading form |
| verifyCode | 4 | int |  |  |
| verifyCodeDiggest | * | MD5Hash | | md5 hash of verifyCode+2525(try verifyCode+5252 if failed) |
| zipLength | 8 | long |  | the length of the zip file to be extracted |
| zipData | zipLength-8 | byte[] |  | the data of encrypted zip file(without zip header { 80, 75, 3, 4, 20, 0, 0, 0 }) |
| entryCount | 4 | int |  |  |
| entries | * | Entry[] |  |  |
| data | * | byte[] |  |  |

**Entry**
| Name | Size(Byte) | Type | Comment |
| --- | ---| --- | --- |
| entryName | * | CustomString | encryped |  |
| compressedSize | 8 | int64 |  |
| originalSize | 8 | int64 |  |

**MD5Hash**
| Name | Size(Byte) | Type | Comment |
| --- | ---| --- | --- |
| length | 1 | byte | hash length |
| data | length | byte[] | md5 value |


**CustomString**
| Name | Size(Byte) | Type | Comment |
| --- | ---| --- | --- |
| length | 1 | byte | string length |
| data | length | string | string encoded in utf-8 |

