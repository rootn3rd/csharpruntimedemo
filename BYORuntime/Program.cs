Report(new Base());
Report(new Derived());

void Report(Base b) => Console.WriteLine($"{b.WhoAmI(),10} - {Runtime.Call<string>(b,"WhatCanIDo")}");


delegate object virtual_function(params object[] args);

static class Runtime
{
    public static T Call<T>(ImplicitLayout instance, string functionName, params object[] args)
    {
        virtual_function function = instance._metadata._vtable[functionName];
        return (T)function.Invoke(args);
    }
}

class TypeMetadata
{
    internal Dictionary<string, virtual_function> _vtable = new();

}

class ImplicitLayout
{
    internal TypeMetadata _metadata = new();

    protected void Register<T>(string functionName, Func<T> f) =>
        _metadata._vtable[functionName] = f.DynamicInvoke;
}

class Base : ImplicitLayout
{
    public Base() : base()
    {
        Register<string>("WhatCanIDo", () => "I do stuff");
    }

    // _metadata comes here
    public virtual string WhoAmI() => this.GetType().Name;
}


class Derived : Base
{
    public Derived() : base()
    {
        Register<string>("WhatCanIDo", () => "I can do more!!");

    }
    // _metadata comes here
}