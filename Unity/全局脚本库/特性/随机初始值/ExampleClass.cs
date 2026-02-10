#if UNITY_EDITOR
using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    public bool z;
    public zz zz;
    public zz[] ZZZzz;

    public bool zb;
    [布尔显示("zb",true)]
   public float 没有随机值;

    [随机初始值(0, 100)]
    public float AAAAAAAAAA;

    [随机初始值(0, 10)]
    public int BBBBBBB;

    [图层选择]
    public int 图层;

    [标签选择]
    public string 标签;

    [随机初始值(0, 100)]
    public long spawnLong;

    [随机初始值(0, 100)]
    public double spawnDouble = 50;

    [随机初始值(0, 255)]
    public byte spawnByte = 50;

    [随机初始值(0, 127)]
    public sbyte spawnSByte = 50;

    [随机初始值(0, 100)]
    public uint spawnUInt;

    [随机初始值(0, 100)]
    public ulong spawnULong;

    [随机初始值(0, 100)]
    public ushort spawnUShort;

    [方法按钮]
    private void TestMethod()
    {
        Debug.Log("方法已被调用！");
    }
}

[System.Serializable]
public class zz
{
    //public bool z;

    [布尔显示("z", true)]
    public ushort spawnUShort;
}
#endif