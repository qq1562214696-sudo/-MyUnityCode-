#if UNITY_EDITOR
using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class 一键添加 : Attribute
{
    public string 标识符 { get; private set; }

    public 一键添加(string 标识符)
    {
        this.标识符 = 标识符;
    }
}
#endif