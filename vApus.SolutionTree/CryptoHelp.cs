using System;
using System.Security.Cryptography;
using System.IO;
namespace vApus.SolutionTree
{
    /// <summary>
    /// This is the exception class that is thrown throughout the Decryption process
    /// </summary>
    public class CryptoHelpException : ApplicationException
    {
        public CryptoHelpException(string msg) : base(msg) { }
    }

    /// <summary>
    /// Summary description for CryptoHelp.
    /// </summary>
    public class CryptoHelp
    {
        /// <summary>
        /// Tag to make sure this file is readable/decryptable by this class
        /// </summary>
        private const ulong FC_TAG = 0xFC010203040506CF;

        /// <summary>
        /// The amount of bytes to read from the file
        /// </summary>
        private const int BUFFER_SIZE = 128 * 1024;

        /// <summary>
        /// Checks to see if two byte array are equal
        /// </summary>
        /// <param name="b1">the first byte array</param>
        /// <param name="b2">the second byte array</param>
        /// <returns>true if b1.Length == b2.Length and each byte in b1 is
        /// equal to the corresponding byte in b2</returns>
        private static bool CheckByteArrays(byte[] b1, byte[] b2)
        {
            if (b1.Length == b2.Length)
            {
                for (int i = 0; i < b1.Length; ++i)
                {
                    if (b1[i] != b2[i])
                        return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates a Rijndael SymmetricAlgorithm for use in EncryptFile and DecryptFile
        /// </summary>
        /// <param name="password">the string to use as the password</param>
        /// <param name="salt">the salt to use with the password</param>
        /// <returns>A SymmetricAlgorithm for encrypting/decrypting with Rijndael</returns>
        private static SymmetricAlgorithm CreateRijndael(string password, byte[] salt)
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, salt, "SHA256", 1000);

            SymmetricAlgorithm sma = Rijndael.Create();
            sma.KeySize = 256;
            sma.Key = pdb.GetBytes(32);
            sma.Padding = PaddingMode.PKCS7;
            return sma;
        }

        /// <summary>
        /// Crypto Random number generator for use in EncryptFile
        /// </summary>
        private static RandomNumberGenerator rand = new RNGCryptoServiceProvider();

        /// <summary>
        /// Generates a specified amount of random bytes
        /// </summary>
        /// <param name="count">the number of bytes to return</param>
        /// <returns>a byte array of count size filled with random bytes</returns>
        private static byte[] GenerateRandomBytes(int count)
        {
            byte[] bytes = new byte[count];
            rand.GetBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// This takes an input file and encrypts it into the output file
        /// </summary>
        /// <param name="inFile">the file to encrypt</param>
        /// <param name="outFile">the file to write the encrypted data to</param>
        /// <param name="password">the password for use as the key</param>
        /// <param name="callback">the method to call to notify of progress</param>
        public static void EncryptFile(string inFile, string outFile, string password)
        {
            using (FileStream fin = File.OpenRead(inFile),
                      fout = File.OpenWrite(outFile))
            {
                long lSize = fin.Length; // the size of the input file for storing
                int size = (int)lSize;  // the size of the input file for progress
                byte[] bytes = new byte[BUFFER_SIZE]; // the buffer
                int read = -1; // the amount of bytes read from the input file
                int value = 0; // the amount overall read from the input file for progress

                // generate IV and Salt              
                byte[] IV = GenerateRandomBytes(16);
                byte[] salt = GenerateRandomBytes(16);

                // create the crypting object
                SymmetricAlgorithm sma = CryptoHelp.CreateRijndael(password, salt);
                sma.IV = IV;

                // write the IV and salt to the beginning of the file
                fout.Write(IV, 0, IV.Length);
                fout.Write(salt, 0, salt.Length);

                // create the hashing and crypto streams
                HashAlgorithm hasher = SHA256.Create();
                using (CryptoStream cout = new CryptoStream(fout, sma.CreateEncryptor(), CryptoStreamMode.Write),
                          chash = new CryptoStream(Stream.Null, hasher, CryptoStreamMode.Write))
                {
                    // write the size of the file to the output file
                    BinaryWriter bw = new BinaryWriter(cout);
                    bw.Write(lSize);

                    // write the file cryptor tag to the file
                    bw.Write(FC_TAG);

                    // read and the write the bytes to the crypto stream in BUFFER_SIZEd chunks
                    while ((read = fin.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        cout.Write(bytes, 0, read);
                        chash.Write(bytes, 0, read);
                        value += read;
                    }
                    // flush and close the hashing object
                    chash.Flush();
                    chash.Close();

                    // read the hash
                    byte[] hash = hasher.Hash;

                    // write the hash to the end of the file
                    cout.Write(hash, 0, hash.Length);

                    // flush and close the cryptostream
                    cout.Flush();
                    cout.Close();
                }
            }
        }

        /// <summary>
        /// takes an input file and decrypts it to the output file
        /// </summary>
        /// <param name="inFile">the file to decrypt</param>
        /// <param name="outStream">the to write the decrypted data to</param>
        /// <param name="password">the password used as the key</param>
        /// <param name="callback">the method to call to notify of progress</param>
        public static void DecryptFile(string inFile, Stream outStream, string password)
        {
            // NOTE:  The encrypting algo was so much easier...

            // create and open the file streams
            using (FileStream fin = File.OpenRead(inFile))
            {
                int size = (int)fin.Length; // the size of the file for progress notification
                byte[] bytes = new byte[BUFFER_SIZE]; // byte buffer
                int read = -1; // the amount of bytes read from the stream
                int value = 0;
                int outValue = 0; // the amount of bytes written out

                // read off the IV and Salt
                byte[] IV = new byte[16];
                fin.Read(IV, 0, 16);
                byte[] salt = new byte[16];
                fin.Read(salt, 0, 16);

                // create the crypting stream
                SymmetricAlgorithm sma = CryptoHelp.CreateRijndael(password, salt);
                sma.IV = IV;

                value = 32; // the value for the progress
                long lSize = -1; // the size stored in the input stream

                // create the hashing object, so that we can verify the file
                HashAlgorithm hasher = SHA256.Create();

                // create the cryptostreams that will process the file
                using (CryptoStream cin = new CryptoStream(fin, sma.CreateDecryptor(), CryptoStreamMode.Read),
                          chash = new CryptoStream(Stream.Null, hasher, CryptoStreamMode.Write))
                {
                    // read size from file
                    BinaryReader br = new BinaryReader(cin);
                    lSize = br.ReadInt64();
                    ulong tag = br.ReadUInt64();

                    if (FC_TAG != tag)
                        throw new CryptoHelpException("File Corrupted!");

                    //determine number of reads to process on the file
                    long numReads = lSize / BUFFER_SIZE;

                    // determine what is left of the file, after numReads
                    long slack = (long)lSize % BUFFER_SIZE;

                    // read the buffer_sized chunks
                    for (int i = 0; i < numReads; ++i)
                    {
                        read = cin.Read(bytes, 0, bytes.Length);
                        outStream.Write(bytes, 0, read);
                        chash.Write(bytes, 0, read);
                        value += read;
                        outValue += read;
                    }

                    // now read the slack
                    if (slack > 0)
                    {
                        read = cin.Read(bytes, 0, (int)slack);
                        outStream.Write(bytes, 0, read);
                        chash.Write(bytes, 0, read);
                        value += read;
                        outValue += read;
                    }
                    // flush and close the hashing stream
                    chash.Flush();
                    chash.Close();

                    outStream.Flush();

                    // read the current hash value
                    byte[] curHash = hasher.Hash;

                    // get and compare the current and old hash values
                    byte[] oldHash = new byte[hasher.HashSize / 8];
                    read = cin.Read(oldHash, 0, oldHash.Length);
                    if ((oldHash.Length != read) || (!CheckByteArrays(oldHash, curHash)))
                        throw new CryptoHelpException("File Corrupted!");
                }

                // make sure the written and stored size are equal
                if (outValue != lSize)
                    throw new CryptoHelpException("File Sizes don't match!");
            }
        }
    }
}