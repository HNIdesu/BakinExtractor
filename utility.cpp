extern "C"{
    int decrypt(char* data,int length,const char* key,int start){
        for(int i=0;i<length;i++){
            int j=(i+start)%16;
            data[i]-=(char)(j*key[j]);
        }
        return length;
    }

    int encrypt(char* data,int length,const char* key,int start){
        for(int i=0;i<length;i++){
            int j=(i+start)%16;
            data[i]+=(char)(j*key[j]);
        }
        return length;
    }
}