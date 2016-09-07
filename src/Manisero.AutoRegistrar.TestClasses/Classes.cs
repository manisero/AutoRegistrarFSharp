namespace Manisero.AutoRegistrar.TestClasses
{
    // R = Root
    // C = Child

    public class R1 {}

    public class R2 {}

    public class C_R1
    {
        public C_R1(R1 p1) {}
    }

    public class C_R1_R2
    {
        public C_R1_R2(R1 p1, R2 p2) {}
    }

    public class C_R1_R1
    {
        public C_R1_R1(R1 p1, R1 p2) {}
    }

    public class MultiCtors
    {
        public MultiCtors() {}
        public MultiCtors(R1 p1) {}
    }
}
