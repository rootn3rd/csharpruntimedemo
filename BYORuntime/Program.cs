Report(new Base());
Report(new Derived());

void Report(Base b) => Console.WriteLine($"{b.WhoAmI(),10} - {b.WhatCanIDo()}");

class Base
{
    public virtual string WhoAmI() => this.GetType().Name;
    public virtual string WhatCanIDo() => "I can do this";
}


class Derived : Base
{
    public override string WhatCanIDo() => "I can do more!!";
}