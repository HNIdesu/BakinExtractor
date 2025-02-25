import os
import ctypes
from ctypes import CDLL
import zipfile
import zlib
import sys
import os.path as Path
from hashlib import md5
from io import BufferedWriter,BytesIO
import json as JSON

def resource_path(relative_path: str) -> str:
    try:
        base_path = sys._MEIPASS
    except AttributeError:
        base_path = Path.abspath(".")
    return Path.join(base_path, relative_path)

KEY1:bytes=b"\x00\x08\x07\x03\x05\x0C\x0B\x0A\x09\x01\x02\x0E\x04\x0D\x06\x0F"
KEY2:bytes=b"\x02\x0E\x04\x0A\x06\x0B\x03\x00\x09\x0F\x07\x01\x0D\x08\x0C\x05"

utility=CDLL(resource_path("utility.dll"))
utility.encrypt.restype=ctypes.c_int32
utility.encrypt.argtypes=[ctypes.c_char_p,ctypes.c_int32,ctypes.c_char_p,ctypes.c_int32]

def encrypt(data:bytes,key:bytes,start:int=0)->bytes:
    if utility.encrypt(ctypes.c_char_p(data),ctypes.c_int32(len(data)),ctypes.c_char_p(key),ctypes.c_int32(start))!=len(data):
        print("Encrypt failed")
        raise SystemExit(1)
    return data

def writeString(file:BufferedWriter,s:str):
    str_data=s.encode("utf-8")
    str_length=len(str_data)
    file.write(int.to_bytes(str_length,length=1,byteorder="little"))
    file.write(str_data)
    return

def isDefaultCheckFile(filepath:str)->bool:
    suffix_list=(".cg",".cgh",".dlp_d",".exe",".dll",".dlp",".webm")
    for suffix in suffix_list:
        if filepath.endswith(suffix):
            return True
    return False

def writeEncryptedString(file:BufferedWriter,s:str):
    str_data=s.encode("utf-8")
    str_length=len(str_data)
    file.write(int.to_bytes(str_length,length=1,byteorder="little"))
    offset=file.tell()
    file.write(encrypt(str_data,KEY2,offset))
    return

def encryptAndCompressAllEntries(file:BufferedWriter,entry_list:dict):
    print("Encrypting and compressing all entries")
    file.write(int.to_bytes(len(entry_list),length=4,byteorder="little"))
    ms=BytesIO()
    for entry in entry_list:
        writeEncryptedString(file,entry)
        file_path=entry_list[entry]
        print(f"Adding {file_path} to resource list")
        with open(file_path,"rb") as file1:
            raw_data=encrypt(file1.read(),KEY2,0)
            compressed_data= zlib.compress(raw_data)
            if(len(compressed_data)<len(raw_data)):
                ms.write(compressed_data)
                file.write(int.to_bytes(len(compressed_data),length=8,byteorder="little"))
                file.write(int.to_bytes(len(raw_data),length=8,byteorder="little"))
            else:
                ms.write(raw_data)
                file.write(int.to_bytes(len(raw_data),length=8,byteorder="little"))
                file.write(int.to_bytes(len(raw_data),length=8,byteorder="little"))
    file.write(ms.getvalue())
    ms.close()

def encryptAndCompressZipFile(directory:str, zip_file:str):
    print("Encrypting and compressing zip file")
    with zipfile.ZipFile(zip_file, 'w', zipfile.ZIP_DEFLATED) as zipf:
        for root, dirs, files in os.walk(directory):
            if not files and not dirs:
                zipf.write(root,root[root.find("unpack.zip")+11:])
            else:
                for file in files:
                    print(f"Adding {Path.join(root,file)} to unpack.zip")
                    entry_name=Path.join(root[root.find("unpack.zip")+11:],file)
                    with open(Path.join(root,file),"rb") as file1:
                        data=file1.read()
                        if isDefaultCheckFile(file):
                            encrypt(data,KEY1,0)
                        else:
                            encrypt(data,KEY2,0)
                        zipf.writestr(entry_name,data)

def pack(unpack_dir:str,packed_file_path:str):
    if not Path.exists(unpack_dir):
        print(f"Directory {unpack_dir} not exists,aborted!")
        raise SystemExit(1)
    info_path = Path.join(unpack_dir,"$pack_info.json")
    if not Path.exists(info_path):
        print(f"File {info_path} not exists,aborted!")
    with open(info_path,"r",encoding="utf-8") as sr:
        pack_info = JSON.loads(sr.read())
    if not (Path.dirname(packed_file_path)=="" or Path.exists(Path.dirname(packed_file_path))):
        os.makedirs(Path.dirname(packed_file_path),exist_ok=True)
    if Path.exists(Path.join(unpack_dir,"unpack.zip")) and Path.isdir(Path.join(unpack_dir,"unpack.zip")):
        encryptAndCompressZipFile(Path.join(unpack_dir,"unpack.zip"),"unpack.zip")
    else:
        print("Directory unpack.zip not exists,aborted!")
        raise SystemExit(1)
    with open(packed_file_path,"wb") as dest_file:
        signatures=b"BKNPAK"
        dest_file.write(signatures)
        data_version = pack_info["data_version"]
        if data_version.find("_") != -1:
            data_version = int(data_version[:data_version.find("_")])
        else:
            data_version = int(data_version)
        dest_file.write(int.to_bytes(data_version,length=2,byteorder="big"))
        dest_file.write(int.to_bytes(0,length=8))# Offset of resource table,reserved
        data_version = pack_info["data_version"]
        loading_form_title:str = pack_info["loading_form_title"]
        if data_version == "1":
            writeString(dest_file,loading_form_title)
        else:# For data version 1_1 or greater
            if(loading_form_title == ""):
                dest_file.write(b"\x00")
            else:
                dest_file.write(b"\x01")
                writeString(dest_file,loading_form_title)
        verify_code=0
        dest_file.write(int.to_bytes(verify_code,length=4,byteorder="little"))
        verify_code_diggest=md5(int.to_bytes(verify_code+2525,length=4,byteorder="little")).digest()
        dest_file.write(int.to_bytes(len(verify_code_diggest),length=1,byteorder="little"))
        dest_file.write(verify_code_diggest)
        with open("unpack.zip","rb") as zip_file:
            zip_file.seek(8,0)
            zip_data=zip_file.read()
            zip_file_length=len(zip_data)+8
            dest_file.write(int.to_bytes(zip_file_length,length=8,byteorder="little"))
            offset=dest_file.tell()
            dest_file.write(encrypt(zip_data,KEY1,offset))
        os.remove("unpack.zip")
        entry_list=dict()
        for root, _, files in os.walk(unpack_dir):
            if "unpack.zip" in root:
                continue
            for file in files:
                if file == "$pack_info.json":
                    continue
                entry_name=Path.join(root,file)[len(unpack_dir):].replace(os.sep,"/")
                if(entry_name.startswith("/")):
                    entry_name=entry_name[1:]
                entry_list[entry_name]=Path.join(root,file)
        resource_table_offset=dest_file.tell()-16
        encryptAndCompressAllEntries(dest_file,entry_list)
        dest_file.seek(8,0)#write resource table offset
        dest_file.write(int.to_bytes(resource_table_offset,length=8,byteorder="little"))
        print("Completed!")
