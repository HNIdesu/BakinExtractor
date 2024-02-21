import os
import sys
import io
from hashlib import md5
from io import BufferedReader
import zlib
from zipfile import ZipFile
from ctypes import CDLL
import ctypes

utility=CDLL("./utility.dll")
utility.decrypt.restype=ctypes.c_int32
utility.decrypt.argtypes=[ctypes.c_char_p,ctypes.c_int32,ctypes.c_char_p,ctypes.c_int32]


KEY1:bytes=b"\x00\x08\x07\x03\x05\x0C\x0B\x0A\x09\x01\x02\x0E\x04\x0D\x06\x0F"
KEY2:bytes=b"\x02\x0E\x04\x0A\x06\x0B\x03\x00\x09\x0F\x07\x01\x0D\x08\x0C\x05"

if len(sys.argv)<=2:
    print(f"Usage: {sys.argv[0]} rbpack_path output_directory")
    print(f"Example: {sys.argv[0]} data.rbpack data")
    exit()
archiver_path=sys.argv[1]
save_directory=sys.argv[2]
if not os.path.exists(save_directory):
    os.mkdir(save_directory)
if not os.path.exists(archiver_path):
    print(f"file {archiver_path} not found!")
    exit()

def equals(a1:bytes,a2:bytes)->bool:
    length=len(a1)
    if(length!=len(a2)):
        return False
    for i in range(0,length):
        if a1[i]!=a2[i]:
            return False
    return True

def readString(file:BufferedReader)->str:
    str_length=file.read(1)[0]
    return file.read(str_length).decode("utf-8")

def readEncryptedString(file:BufferedReader)->str:
    str_length=file.read(1)[0]
    offset=file.tell()
    return decrypt(file.read(str_length),KEY2,offset).decode("utf-8")

def verify(verify_code:int,verify_code_md5:bytes)->bool:
    if(equals(md5(int.to_bytes(verify_code+2525,4,byteorder="little")).digest(),verify_code_md5)):
        return True
    if(equals(md5(int.to_bytes(verify_code+5252,4,byteorder="little")).digest(),verify_code_md5)):
        return True
    return False


def decrypt(data:bytes,key:bytes,start:int=0)->bytes:
    if utility.decrypt(ctypes.c_char_p(data),ctypes.c_int32(len(data)),ctypes.c_char_p(key),ctypes.c_int32(start))!=len(data):
        print("Dectypt failed")
        exit()
    return data

def isDefaultCheckFile(filepath:str)->bool:
    suffix_list=(".cg",".cgh",".dlp_d",".exe",".dll",".dlp",".webm")
    for suffix in suffix_list:
        if filepath.endswith(suffix):
            return True
    return False

def extractZipFileAndDecrypt(file:BufferedReader):
    print("Extracting and decrypting zip files...")
    
    zip_length=int.from_bytes(file.read(8),"little")
    offset=file.tell()
    zip_extract_path=os.path.join(save_directory,"unpack.zip")
    if not os.path.exists(zip_extract_path):
        os.mkdir(zip_extract_path)
    zipfile=ZipFile(io.BytesIO(b"\x50\x4B\x03\x04\x14\x00\x00\x00"+decrypt(file.read(zip_length-8),KEY1,offset)))

    for entry_name in zipfile.namelist():
        save_path=os.path.join(zip_extract_path,entry_name.replace("/",os.path.sep))
        if(entry_name.endswith("/")):#directory entry
            os.makedirs(save_path,exist_ok=True)
            continue
        os.makedirs(os.path.dirname(save_path),exist_ok=True)
        print(f"Extracting {entry_name}")
        data=zipfile.read(entry_name)
        if(isDefaultCheckFile(save_path)):
            data=decrypt(data,KEY1,0)
        else:
            data=decrypt(data,KEY2,0)
        with open(save_path,"wb") as fs:
            fs.write(data)


def extractAllEntries(file:BufferedReader,entry_list:list,save_dir:str):
    print("Decrypting all entries...")
    offset=file.tell()
    for entry in entry_list:
        entry_name:str=entry[0]
        compressed_size:int=entry[1]
        original_size:int=entry[2]
        print(f"Extracting {entry_name},offset:{offset},size:{original_size}")
        data=file.read(compressed_size)
        if(compressed_size!=original_size):
            data=zlib.decompress(data)
        data=decrypt(data,KEY2,offset)

        dest_path=os.path.join(save_dir,entry_name.replace("/",os.path.sep))
        if not os.path.exists(os.path.dirname(dest_path)):
            os.makedirs(os.path.dirname(dest_path),exist_ok=True)
        with open(dest_path,"wb") as fs:
            fs.write(data)

with open(archiver_path,"rb") as file:
    signature=file.read(6).decode("utf-8")
    if(signature!="BKNPAK"):
        print("Signature mismatch!")
        exit()
    data_version=int.from_bytes(file.read(2),"big")
    resource_table_offset=int.from_bytes(file.read(8),"little")+file.tell()
    loading_form_title=readString(file)
    verify_code=int.from_bytes(file.read(4),"little")
    verify_code_md5_length=file.read(1)[0]
    verify_code_md5=file.read(verify_code_md5_length)
    if not verify(verify_code,verify_code_md5):
        print("Verify failed")
        exit()
    extractZipFileAndDecrypt(file)
    file.seek(resource_table_offset,0)
    entry_count=int.from_bytes(file.read(4),"little")
    entry_list=list()
    for i in range(0,entry_count):
        entry_name=readEncryptedString(file)
        compressed_size=int.from_bytes(file.read(8),"little")
        original_size=int.from_bytes(file.read(8),"little")
        entry_list.append((entry_name,compressed_size,original_size))
    extractAllEntries(file,entry_list,save_directory)
