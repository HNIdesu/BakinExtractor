#You can try it
from ctypes import CDLL
import ctypes

KEY1:bytes=b"\x00\x08\x07\x03\x05\x0C\x0B\x0A\x09\x01\x02\x0E\x04\x0D\x06\x0F"
KEY2:bytes=b"\x02\x0E\x04\x0A\x06\x0B\x03\x00\x09\x0F\x07\x01\x0D\x08\x0C\x05"

utility=CDLL("./utility.dll")
utility.encrypt.restype=ctypes.c_int32
utility.encrypt.argtypes=[ctypes.c_char_p,ctypes.c_int32,ctypes.c_char_p,ctypes.c_int32]

def encrypt(data:bytes,key:bytes,start:int=0)->bytes:
    if utility.encrypt(ctypes.c_char_p(data),ctypes.c_int32(len(data)),ctypes.c_char_p(key),ctypes.c_int32(start))!=len(data):
        print("Enctypt failed")
        exit()
    return data
