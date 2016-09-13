﻿namespace Manisero.AutoRegistrar.TestClasses
{
    // R = Root
    // Cx = Child, x => dependancy level
    // I = Interface

    public interface IR1 {}
    public class R1 : IR1 {}

    public interface IR2_1 { }
    public interface IR2_2 { }
    public abstract class R2_Base {}
    public class R2 : R2_Base, IR2_1, IR2_2 {}

    public class IC1A_R1 {}
    public class C1A_R1 : IC1A_R1
    {
        public C1A_R1(R1 p1) {}
    }

    public interface IC1B_R1_R2 {}
    public class C1B_R1_R2 : IC1B_R1_R2
    {
        public C1B_R1_R2(R1 p1, R2 p2) {}
    }

    public interface IC1C_R1_R1 {}
    public class C1C_R1_R1 : IC1C_R1_R1
    {
        public C1C_R1_R1(R1 p1, R1 p2) {}
    }

    public class MultiCtors
    {
        public MultiCtors() {}
        public MultiCtors(R1 p1) {}
    }
}
