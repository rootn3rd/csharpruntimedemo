Report(new Base());
Report(new Derived());

void Report(Base b) => Console.WriteLine($"{b.WhoAmI(),10} - {Runtime.Call<string>(b, "WhatCanIDo")}");


delegate object virtual_function(params object[] args);

static class Runtime
{
    public static T Call<T>(ImplicitLayout instance, string functionName, params object[] args)
    {
        virtual_function function = instance._metadata._vtable[functionName];
        object[] parameters = new object[] { instance }.Concat(args).ToArray();
        return (T)function.Invoke(parameters);
    }

    internal static Dictionary<string, virtual_function> Register<TType, T>(this Dictionary<string, virtual_function> vtable, string functionName, Func<TType, T> f)
    {
        vtable[functionName] = f.DynamicInvoke;
        return vtable;
    }
}

class TypeMetadata
{
    internal Dictionary<string, virtual_function> _vtable = new();
    internal Type _type;
}

class ImplicitLayout
{
    public ImplicitLayout(TypeMetadata meta) => _metadata = meta;
    internal TypeMetadata _metadata;

}


/* Your class v/s what compiler sees
class Base : ImplicitLayout
{
    public Base(TypeMetadata metadata) : base(metadata) { }

    public Type GetType() => _metadata._type;
    // _metadata comes here
    public virtual string WhoAmI() => this.GetType().Name;
    public virtual string WhatCanIDo() => "I do stuff";
} 
*/

class Base : ImplicitLayout
{
    public Base(TypeMetadata metadata) : base(metadata) { }

    public static Type GetType(Base @this) => @this._metadata._type;
    // _metadata comes here
    public static string WhoAmI(Base @this) => Base.GetType(@this).Name;
    public static string WhatCanIDo(Base @this) => "I do stuff";
}


class Derived : Base
{
    public Derived(TypeMetadata metadata) : base(metadata) { }
    public static Type GetType(Derived @this) => @this._metadata._type;

    // _metadata comes here
    public static string WhatCanIDo(Derived @this) => "I can do more!!";

}