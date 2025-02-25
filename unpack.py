import os
import os.path as Path
import io
from hashlib import md5
from io import BufferedReader
import zlib
from zipfile import ZipFile
from ctypes import CDLL
import ctypes
import sys
import json as JSON

def resource_path(relative_path: str) -> str:
    try:
        base_path = sys._MEIPASS
    except AttributeError:
        base_path = Path.abspath(".")
    return Path.join(base_path, relative_path)

utility=CDLL(resource_path("utility.dll"))
utility.decrypt.restype=ctypes.c_int32
utility.decrypt.argtypes=[ctypes.c_char_p,ctypes.c_int32,ctypes.c_char_p,ctypes.c_int32]

KEY1:bytes=b"\x00\x08\x07\x03\x05\x0C\x0B\x0A\x09\x01\x02\x0E\x04\x0D\x06\x0F"
KEY2:bytes=b"\x02\x0E\x04\x0A\x06\x0B\x03\x00\x09\x0F\x07\x01\x0D\x08\x0C\x05"

def guess_data_version(filepath:str)->str:
    with open(filepath,"rb") as br:
        br.seek(6,1)
        data_version=int.from_bytes(br.read(2),"big")
        br.seek(8,1)
        if data_version == 1:
            if br.read(1)[0] <= 1:
                return "1_1"
        return str(data_version)
    raise Exception("Not supported data version")

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
        raise SystemExit(1)
    return data

def isDefaultCheckFile(filepath:str)->bool:
    suffix_list=(".cg",".cgh",".dlp_d",".exe",".dll",".dlp",".webm")
    for suffix in suffix_list:
        if filepath.endswith(suffix):
            return True
    return False

def extractZipFileAndDecrypt(file:BufferedReader,save_directory:str):
    print("Extracting and decrypt zip files...")
    
    zip_length=int.from_bytes(file.read(8),"little")
    offset=file.tell()
    zip_extract_path=Path.join(save_directory,"unpack.zip")
    if not Path.exists(zip_extract_path):
        os.mkdir(zip_extract_path)
    zipfile=ZipFile(io.BytesIO(b"\x50\x4B\x03\x04\x14\x00\x00\x00"+decrypt(file.read(zip_length-8),KEY1,offset)))

    for entry_name in zipfile.namelist():
        save_path=Path.join(zip_extract_path,entry_name.replace("/",Path.sep))
        if(entry_name.endswith("/")):# Directory entry
            os.makedirs(save_path,exist_ok=True)
            continue
        os.makedirs(Path.dirname(save_path),exist_ok=True)
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
    for entry in entry_list:
        offset=file.tell()
        entry_name:str=entry[0]
        compressed_size:int=entry[1]
        original_size:int=entry[2]
        print(f"Extracting {entry_name},offset:{offset},size:{original_size}")
        data=file.read(compressed_size)
        if(compressed_size!=original_size):
            data=zlib.decompress(data)
        data=decrypt(data,KEY2,0)

        dest_path=Path.join(save_dir,entry_name.replace("/",Path.sep))
        if not Path.exists(Path.dirname(dest_path)):
            os.makedirs(Path.dirname(dest_path),exist_ok=True)
        with open(dest_path,"wb") as fs:
            fs.write(data)

def unpack(archiver_path:str,save_directory:str):
    os.makedirs(save_directory,exist_ok=True)
    if not Path.exists(archiver_path):
        print(f"Error: File '{archiver_path}' not found!")
        raise SystemExit(1)
    known_data_versions = set(["1","1_1"])
    data_version = guess_data_version(archiver_path)
    print(f"Guessed data version: {data_version}")
    if data_version not in known_data_versions:
        print(f"Warning: The data version '{data_version}' may not be supported.")
    with open(archiver_path,"rb") as file:
        signature=file.read(6).decode("utf-8")
        if signature != "BKNPAK":
            print("Error: Signature mismatch!")
            raise SystemExit(1)
        file.seek(2,1)# Skip the data version
        resource_table_offset=int.from_bytes(file.read(8),"little")+file.tell()
        if data_version == "1":
            loading_form_title=readString(file)
        else:# For data version 1_1 or greater
            has_title = file.read(1)[0] != 0
            if has_title:
                loading_form_title = readString(file)
            else:
                loading_form_title = ""
        verify_code=int.from_bytes(file.read(4),"little")
        verify_code_md5_length=file.read(1)[0]
        verify_code_md5=file.read(verify_code_md5_length)
        if not verify(verify_code,verify_code_md5):
            print("Error: Hash verification failed!")
            raise SystemExit(1)
        extractZipFileAndDecrypt(file,save_directory)
        file.seek(resource_table_offset,0)
        entry_count=int.from_bytes(file.read(4),"little")
        entry_list=list()
        for _ in range(0,entry_count):
            entry_name=readEncryptedString(file)
            compressed_size=int.from_bytes(file.read(8),"little")
            original_size=int.from_bytes(file.read(8),"little")
            entry_list.append((entry_name,compressed_size,original_size))
        extractAllEntries(file,entry_list,save_directory)
        with open(Path.join(save_directory,"$pack_info.json"),"w",encoding="utf-8") as sw:
            sw.write(JSON.dumps({"data_version":data_version,"loading_form_title":loading_form_title}))
        print("Completed!")