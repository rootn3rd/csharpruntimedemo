Report(Runtime.New<Base>());
Report(Runtime.New<Derived>());

void Report(Base b) => Console.WriteLine($"{Runtime.Call<string>(b, "WhoAmI"),10} - {Runtime.Call<string>(b, "WhatCanIDo")}");


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

    internal static Dictionary<string, TypeMetadata> _loadedTypes = new();

    private static TypeMetadata LoadOrGetType<T>() where T : IInstance
    {
        if (!_loadedTypes.TryGetValue(typeof(T).FullName, out TypeMetadata metadata))
        {
            metadata = T.LoadType();
            _loadedTypes.Add(typeof(T).FullName, metadata);
        }
        return metadata;
    }

    public static T New<T>(params object[] args) where T : ImplicitLayout, IInstance
    {
        TypeMetadata metadata = LoadOrGetType<T>();
        object[] parameters = new object[] { metadata }.Concat(args).ToArray();
        return (T)Activator.CreateInstance(typeof(T), parameters);
    }
}

internal interface IInstance
{
    static abstract TypeMetadata LoadType();
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

class Base : ImplicitLayout, IInstance
{
    public Base(TypeMetadata metadata) : base(metadata) { }
    internal static Type GetType(Base @this) => @this._metadata._type;
    internal static string WhoAmI(Base @this) => Base.GetType(@this).Name;
    internal static string WhatCanIDo(Base @this) => "I do stuff";

    public static TypeMetadata LoadType() => new()
    {
        _type = typeof(Base),
        _vtable = new Dictionary<string, virtual_function>()
            .Register<Base, string>("WhoAmI", WhoAmI)
            .Register<Base, string>("WhatCanIDo", WhatCanIDo)

    };
}


class Derived : Base, IInstance
{
    public Derived(TypeMetadata metadata) : base(metadata) { }
    internal static string WhatCanIDo(Derived @this) => "I can do more!!";

    public static TypeMetadata LoadType() => new()
    {
        _type = typeof(Derived),
        _vtable = Base.LoadType()._vtable.Register<Derived, string>("WhatCanIDo", WhatCanIDo),
    };
}