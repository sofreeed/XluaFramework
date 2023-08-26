using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YooAsset;

public class BundleDecryptionServices : IDecryptionServices
{
    public ulong LoadFromFileOffset(DecryptFileInfo fileInfo)
    {
        return 32;
    }
    
    public byte[] LoadFromMemory(DecryptFileInfo fileInfo)
    {
        // 如果没有内存加密方式，可以返回空
        throw new NotImplementedException();
    }

    public Stream LoadFromStream(DecryptFileInfo fileInfo)
    {
        // 如果没有流加密方式，可以返回空
        throw new NotImplementedException();
    }
    
    public uint GetManagedReadBufferSize()
    {
        return 1024;
    }
}
