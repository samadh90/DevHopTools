using System.Security.Cryptography;

namespace DevHopTools.Cryptography.Hash
{
    public class HashTool
    {
        private readonly HashAlgorithm _hashAlgorithm;

        public HashTool() : this(SHA512.Create()) { }

        public HashTool(HashAlgorithm hashAlgorithm)
        {
            _hashAlgorithm = hashAlgorithm;
        }

        public byte[] Hash(byte[] data)
        {
            return _hashAlgorithm.ComputeHash(data);
        }
    }
}
