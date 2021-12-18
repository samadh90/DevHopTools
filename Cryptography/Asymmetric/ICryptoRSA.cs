namespace DevHopTools.Cryptography.Asymmetric
{
    public interface ICryptoRSA
    {
        byte[] BothBinaryKeys { get; }
        string BothXmlKeys { get; }
        byte[] PublicBinaryKey { get; }
        string PublicXmlKey { get; }

        byte[] Decrypt(byte[] binaries);
        string DecryptAsString(byte[] binaries);
        byte[] Encrypt(byte[] data);
        byte[] Encrypt(string data);
        void ImportBinaryKeys(byte[] keys);
        void ImportXmlKeys(string xml);
    }
}