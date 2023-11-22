using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using ProtoTemplate;
using UnityEngine;

public class PBTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MsgTemplate msgTemplate = new MsgTemplate();
        msgTemplate.B = false;
        msgTemplate.Str = "hello world";
        msgTemplate.F = 3.14f;
        msgTemplate.D = 3.1415926;
        msgTemplate.I32 = 32;
        msgTemplate.UI32 = 32;
        msgTemplate.SI32 = 32;
        
        msgTemplate.I64 = 32;
        msgTemplate.UI64 = 32;
        msgTemplate.SI64 = 32;

        msgTemplate.Fix32 = 32;
        msgTemplate.Fix64 = 32;

        msgTemplate.Sfix32 = 32;
        msgTemplate.Sfix64 = 32;

        msgTemplate.Strlist.Add("hhh");
        msgTemplate.Strlist.Add("aaa");
        
        msgTemplate.Stringmap.Add("111", 111);
        msgTemplate.Stringmap.Add("222", 222);

        msgTemplate.EnumField = EnumTemplate.Ccc;

        byte[] bytes = msgTemplate.ToByteArray();

        MsgTemplate msgTemplate1 = MsgTemplate.Parser.ParseFrom(bytes);

        
        Debug.LogError(msgTemplate1.Str);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
