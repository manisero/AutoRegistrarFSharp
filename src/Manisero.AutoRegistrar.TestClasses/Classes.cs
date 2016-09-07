namespace Manisero.AutoRegistrar.TestClasses
{
    // R = Root
    // C = Child
    // I = Interface

    public interface I1 {}
    public class R1 : I1 {}

    public interface I2_1 { }
    public interface I2_2 { }
    public abstract class R2_Base {}
    public class R2 : R2_Base, I2_1, I2_2 {}

    public class I_R1 {}
    public class C_R1 : I_R1
    {
        public C_R1(R1 p1) {}
    }

    public interface I_R1_R2 {}
    public class C_R1_R2 : I_R1_R2
    {
        public C_R1_R2(R1 p1, R2 p2) {}
    }

    public interface I_R1_R1 {}
    public class C_R1_R1 : I_R1_R1
    {
        public C_R1_R1(R1 p1, R1 p2) {}
    }

    public class MultiCtors
    {
        public MultiCtors() {}
        public MultiCtors(R1 p1) {}
    }
}
