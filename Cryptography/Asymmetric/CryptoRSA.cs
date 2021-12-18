using System;
using System.Security.Cryptography;
using System.Text;

namespace DevHopTools.Cryptography.Asymmetric
{
    public class CryptoRSA : ICryptoRSA
    {
        private RSACryptoServiceProvider _cryptoServiceProvider;
        private Encoding _encoding;

        public byte[] BothBinaryKeys
        {
            get { return _cryptoServiceProvider.ExportCspBlob(true); }
        }

        public string BothXmlKeys
        {
            get { return _cryptoServiceProvider.ToXmlString(true); }
        }

        public byte[] PublicBinaryKey
        {
            get { return _cryptoServiceProvider.ExportCspBlob(false); }
        }

        public string PublicXmlKey
        {
            get { return _cryptoServiceProvider.ToXmlString(false); }
        }

        public CryptoRSA() : this(KeySizes.RSA2048) { }

        public CryptoRSA(KeySizes keySize) : this(keySize, Encoding.Unicode) { }

        public CryptoRSA(KeySizes KeySize, Encoding encoding)
        {
            _cryptoServiceProvider = new RSACryptoServiceProvider((int)KeySize);
            _encoding = encoding;
        }

        public CryptoRSA(string XmlKeys) : this()
        {
            if (XmlKeys != null) ImportXmlKeys(XmlKeys);
        }

        public CryptoRSA(byte[] BinaryKeys) : this()
        {
            if (BinaryKeys != null) ImportBinaryKeys(BinaryKeys);
        }

        public byte[] Encrypt(string data)
        {
            byte[] binaries = _encoding.GetBytes(data);
            return Encrypt(binaries);
        }

        public byte[] Encrypt(byte[] data)
        {
            if (data.Length > (_cryptoServiceProvider.KeySize / 8) - 42)
                throw new InvalidOperationException(string.Format("Chaine trop longue!!! (Taille Maximale : {0})", ((_cryptoServiceProvider.KeySize / 8) - 42) / 2));

            return _cryptoServiceProvider.Encrypt(data, true);
        }

        public string DecryptAsString(byte[] binaries)
        {
            binaries = Decrypt(binaries);
            return _encoding.GetString(binaries);
        }

        public byte[] Decrypt(byte[] binaries)
        {
            if (_cryptoServiceProvider.PublicOnly) throw new InvalidOperationException("Pour décrypter il vous faut la clé privée!!");
            return _cryptoServiceProvider.Decrypt(binaries, true);
        }

        public void ImportBinaryKeys(byte[] keys)
        {
            _cryptoServiceProvider.ImportCspBlob(keys);
        }

        public void ImportXmlKeys(string xml)
        {
            _cryptoServiceProvider.FromXmlString(xml);
        }
    }
}
