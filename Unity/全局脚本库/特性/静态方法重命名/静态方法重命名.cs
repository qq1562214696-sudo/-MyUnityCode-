using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class 静态方法重命名 : Attribute
{
    public string 方法名 { get; }

    public 静态方法重命名(string 方法名)
    {
        this.方法名 = 方法名;
    }
}