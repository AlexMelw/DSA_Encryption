namespace DSAEncDecLib
{
    using System.Collections.Generic;

    static class PrimeFactorsMapper
    {
        public static Dictionary<int, int> PQRelation { get; } = new Dictionary<int, int>
        {
            { 1024, 160 }, // 3 moths to be cracked private key
            { 2048, 224 }, // 1902 mill. years to be cracked private key
            { 3072, 256 }  // 3x10^52 years to be cracked private key
        };
    }
}