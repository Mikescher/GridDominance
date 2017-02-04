// @Date : 15th July 2012
// @Author : Arpan Jati (arpan4017@yahoo.com; arpan4017@gmail.com)
// @Library : ArpanTECH.RSAx
// @CodeProject: http://www.codeproject.com/Articles/421656/RSA-Library-with-Private-Key-Encryption-in-Csharp  

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Numerics;
using System.Linq;
using System.Text;
using System.IO;

namespace ArpanTECH
{
    /// <summary>
    /// The main RSAx Class
    /// </summary>
    public class RSAx : IDisposable
    {
        private RSAxParameters rsaParams;
        private RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        /// <summary>
        /// Initialize the RSA class.
        /// </summary>
        /// <param name="rsaParams">Preallocated RSAxParameters containing the required keys.</param>
        public RSAx(RSAxParameters rsaParams)
        {
            this.rsaParams = rsaParams;
            UseCRTForPublicDecryption = true;
        }

        /// <summary>
        /// Initialize the RSA class from a XML KeyInfo string.
        /// </summary>
        /// <param name="keyInfo">XML Containing Key Information</param>
        /// <param name="ModulusSize">Length of RSA Modulus in bits.</param>
        public RSAx(String keyInfo, int ModulusSize)
        {
            this.rsaParams = RSAxUtils.GetRSAxParameters(keyInfo, ModulusSize);
            UseCRTForPublicDecryption = true;
        }

        /// <summary>
        /// Hash Algorithm to be used for OAEP encoding.
        /// </summary>
        public RSAxParameters.RSAxHashAlgorithm RSAxHashAlgorithm
        {
            set
            {
                rsaParams.HashAlgorithm = value;
            }
        }

        /// <summary>
        /// If True, and if the parameters are available, uses CRT for private key decryption. (Much Faster)
        /// </summary>
        public bool UseCRTForPublicDecryption
        {
            get;  set;
        }

        /// <summary>
        /// Releases all the resources.
        /// </summary>
        public void Dispose()
        {
            rsaParams.Dispose();
        }

        #region PRIVATE FUNCTIONS

        /// <summary>
        /// Low level RSA Process function for use with private key.
        /// Should never be used; Because without padding RSA is vulnerable to attacks.  Use with caution.
        /// </summary>
        /// <param name="PlainText">Data to encrypt. Length must be less than Modulus size in octets.</param>
        /// <param name="usePrivate">True to use Private key, else Public.</param>
        /// <returns>Encrypted Data</returns>
        public byte[] RSAProcess(byte[] PlainText, bool usePrivate)
        {

            if (usePrivate && (!rsaParams.Has_PRIVATE_Info))
            {
                throw new CryptographicException("RSA Process: Incomplete Private Key Info");
            }

            if ((usePrivate == false) && (!rsaParams.Has_PUBLIC_Info))
            {
                throw new CryptographicException("RSA Process: Incomplete Public Key Info");
            }            

            BigInteger _E;
            if (usePrivate)
                _E = rsaParams.D; 
            else
                _E = rsaParams.E;

            BigInteger PT = RSAxUtils.OS2IP(PlainText, false);
            BigInteger M = BigInteger.ModPow(PT, _E, rsaParams.N);
            
            if (M.Sign == -1)
                return RSAxUtils.I2OSP(M + rsaParams.N, rsaParams.OctetsInModulus, false);            
            else
                return RSAxUtils.I2OSP(M, rsaParams.OctetsInModulus, false);                   
        }

        /// <summary>
        /// Low level RSA Decryption function for use with private key. Uses CRT and is Much faster.
        /// Should never be used; Because without padding RSA is vulnerable to attacks. Use with caution.
        /// </summary>
        /// <param name="Data">Data to encrypt. Length must be less than Modulus size in octets.</param>
        /// <returns>Encrypted Data</returns>
        public byte[] RSADecryptPrivateCRT(byte[] Data)
        {
            if (rsaParams.Has_PRIVATE_Info && rsaParams.HasCRTInfo)
            {
                BigInteger C = RSAxUtils.OS2IP(Data, false);

                BigInteger M1 = BigInteger.ModPow(C, rsaParams.DP, rsaParams.P);
                BigInteger M2 = BigInteger.ModPow(C, rsaParams.DQ, rsaParams.Q);
                BigInteger H = ((M1 - M2) * rsaParams.InverseQ) % rsaParams.P;
                BigInteger M = (M2 + (rsaParams.Q * H));

                if (M.Sign == -1)
                    return RSAxUtils.I2OSP(M + rsaParams.N, rsaParams.OctetsInModulus, false);
                else
                    return RSAxUtils.I2OSP(M, rsaParams.OctetsInModulus, false); 
            }
            else
            {
                throw new CryptographicException("RSA Decrypt CRT: Incomplete Key Info");
            }                             
        }        
        
        private byte[] RSAProcessEncodePKCS(byte[] Message, bool usePrivate)
        {
            if (Message.Length > rsaParams.OctetsInModulus - 11)
            {
                throw new ArgumentException("Message too long.");
            }
            else
            {
                // RFC3447 : Page 24. [RSAES-PKCS1-V1_5-ENCRYPT ((n, e), M)]
                // EM = 0x00 || 0x02 || PS || 0x00 || Msg 
                
                List<byte> PCKSv15_Msg = new List<byte>();
                
                PCKSv15_Msg.Add(0x00);
                PCKSv15_Msg.Add(0x02);

                int PaddingLength = rsaParams.OctetsInModulus - Message.Length - 3;

                byte[] PS = new byte[PaddingLength];
                rng.GetNonZeroBytes(PS);

                PCKSv15_Msg.AddRange(PS);
                PCKSv15_Msg.Add(0x00);

                PCKSv15_Msg.AddRange(Message);

                return RSAProcess(PCKSv15_Msg.ToArray() ,  usePrivate);
            }
        }

        /// <summary>
        /// Mask Generation Function
        /// </summary>
        /// <param name="Z">Initial pseudorandom Seed.</param>
        /// <param name="l">Length of output required.</param>
        /// <returns></returns>
        private byte[] MGF(byte[] Z, int l)
        {
            if (l > (Math.Pow(2, 32)))
            {
                throw new ArgumentException("Mask too long.");
            }
            else
            {
                List<byte> result = new List<byte>();
                for (int i = 0; i <= l / rsaParams.hLen; i++)
                {
                    List<byte> data = new List<byte>();
                    data.AddRange(Z);
                    data.AddRange(RSAxUtils.I2OSP(i, 4, false));
                    result.AddRange(rsaParams.ComputeHash(data.ToArray()));
                }

                if (l <= result.Count)
                {
                    return result.GetRange(0, l).ToArray();
                }
                else
                {
                    throw new ArgumentException("Invalid Mask Length.");
                }
            }
        }

       
        private byte[] RSAProcessEncodeOAEP(byte[] M, byte[] P, bool usePrivate)
        {
            //                           +----------+---------+-------+
            //                      DB = |  lHash   |    PS   |   M   |
            //                           +----------+---------+-------+
            //                                          |
            //                +----------+              V
            //                |   seed   |--> MGF ---> XOR
            //                +----------+              |
            //                      |                   |
            //             +--+     V                   |
            //             |00|    XOR <----- MGF <-----|
            //             +--+     |                   |
            //               |      |                   |
            //               V      V                   V
            //             +--+----------+----------------------------+
            //       EM =  |00|maskedSeed|          maskedDB          |
            //             +--+----------+----------------------------+

            int mLen = M.Length;
            if (mLen > rsaParams.OctetsInModulus - 2 * rsaParams.hLen - 2)
            {
                throw new ArgumentException("Message too long.");
            }
            else
            {
                byte[] PS = new byte[rsaParams.OctetsInModulus - mLen - 2 * rsaParams.hLen - 2];
                //4. pHash = Hash(P),
                byte[] pHash = rsaParams.ComputeHash(P);
                
                //5. DB = pHash||PS||01||M.
                List<byte> _DB = new List<byte>();
                _DB.AddRange(pHash);
                _DB.AddRange(PS);
                _DB.Add(0x01);
                _DB.AddRange(M);
                byte[] DB = _DB.ToArray();

                //6. Generate a random octet string seed of length hLen.                
                byte[] seed = new byte[rsaParams.hLen];
                rng.GetBytes(seed);

                //7. dbMask = MGF(seed, k - hLen -1).
                byte[] dbMask = MGF(seed, rsaParams.OctetsInModulus - rsaParams.hLen - 1);

                //8. maskedDB = DB XOR dbMask
                byte[] maskedDB = RSAxUtils.XOR(DB, dbMask);

                //9. seedMask = MGF(maskedDB, hLen)
                byte[] seedMask = MGF(maskedDB, rsaParams.hLen);

                //10. maskedSeed = seed XOR seedMask.
                byte[] maskedSeed = RSAxUtils.XOR(seed, seedMask);

                //11. EM = 0x00 || maskedSeed || maskedDB.
                List<byte> result = new List<byte>();
                result.Add(0x00);
                result.AddRange(maskedSeed);
                result.AddRange(maskedDB);

                return RSAProcess(result.ToArray(), usePrivate);
            }
        }

        
        private byte[] Decrypt(byte[] Message, byte [] Parameters, bool usePrivate, bool fOAEP)
        {
            byte[] EM = new byte[0];
            try
            {
                if ((usePrivate == true) && (UseCRTForPublicDecryption) && (rsaParams.HasCRTInfo))
                {
                    EM = RSADecryptPrivateCRT(Message);
                }
                else
                {
                    EM = RSAProcess(Message, usePrivate);
                }
            }
            catch (CryptographicException ex)
            {
                throw new CryptographicException("Exception while Decryption: " + ex.Message);
            }
            catch
            {
                throw new Exception("Exception while Decryption: ");
            }

            try
            {
                if (fOAEP) //DECODE OAEP
                {
                    if ((EM.Length == rsaParams.OctetsInModulus) && (EM.Length > (2 * rsaParams.hLen + 1)))
                    {
                        byte[] maskedSeed;
                        byte[] maskedDB;
                        byte[] pHash = rsaParams.ComputeHash(Parameters);
                        if (EM[0] == 0) // RFC3447 Format : http://tools.ietf.org/html/rfc3447
                        {
                            maskedSeed = EM.ToList().GetRange(1, rsaParams.hLen).ToArray();
                            maskedDB = EM.ToList().GetRange(1 + rsaParams.hLen, EM.Length - rsaParams.hLen - 1).ToArray();
                            byte[] seedMask = MGF(maskedDB, rsaParams.hLen);
                            byte[] seed = RSAxUtils.XOR(maskedSeed, seedMask);
                            byte[] dbMask = MGF(seed, rsaParams.OctetsInModulus - rsaParams.hLen - 1);
                            byte[] DB = RSAxUtils.XOR(maskedDB, dbMask);

                            if (DB.Length >= (rsaParams.hLen + 1))
                            {
                                byte[] _pHash = DB.ToList().GetRange(0, rsaParams.hLen).ToArray();
                                List<byte> PS_M = DB.ToList().GetRange(rsaParams.hLen, DB.Length - rsaParams.hLen);
                                int pos = PS_M.IndexOf(0x01);
                                if (pos >= 0 && (pos < PS_M.Count))
                                {
                                    List<byte> _01_M = PS_M.GetRange(pos, PS_M.Count - pos);
                                    byte[] M;
                                    if (_01_M.Count > 1)
                                    {
                                        M = _01_M.GetRange(1, _01_M.Count - 1).ToArray();
                                    }
                                    else
                                    {
                                        M = new byte[0];
                                    }
                                    bool success = true;
                                    for (int i = 0; i < rsaParams.hLen; i++)
                                    {
                                        if (_pHash[i] != pHash[i])
                                        {
                                            success = false;
                                            break;
                                        }
                                    }

                                    if (success)
                                    {
                                        return M;
                                    }
                                    else
                                    {
                                        M = new byte[rsaParams.OctetsInModulus]; //Hash Match Failure.
                                        throw new CryptographicException("OAEP Decode Error");
                                    }
                                }
                                else
                                {// #3: Invalid Encoded Message Length.
                                    throw new CryptographicException("OAEP Decode Error");
                                }
                            }
                            else
                            {// #2: Invalid Encoded Message Length.
                                throw new CryptographicException("OAEP Decode Error");
                            }
                        }
                        else // Standard : ftp://ftp.rsasecurity.com/pub/rsalabs/rsa_algorithm/rsa-oaep_spec.pdf
                        {//OAEP : THIS STADNARD IS NOT IMPLEMENTED
                            throw new CryptographicException("OAEP Decode Error");
                        }
                    }
                    else
                    {// #1: Invalid Encoded Message Length.
                        throw new CryptographicException("OAEP Decode Error");
                    }
                }
                else // DECODE PKCS v1.5
                {
                    if (EM.Length >= 11)
                    {
                        if ((EM[0] == 0x00) && (EM[1] == 0x02))
                        {
                            int startIndex = 2;
                            List<byte> PS = new List<byte>();
                            for (int i = startIndex; i < EM.Length; i++)
                            {
                                if (EM[i] != 0)
                                {
                                    PS.Add(EM[i]);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (PS.Count >= 8)
                            {
                                int DecodedDataIndex = startIndex + PS.Count + 1;
                                if (DecodedDataIndex < (EM.Length - 1))
                                {
                                    List<byte> DATA = new List<byte>();
                                    for (int i = DecodedDataIndex; i < EM.Length; i++)
                                    {
                                        DATA.Add(EM[i]);
                                    }
                                    return DATA.ToArray();
                                }
                                else
                                {
                                    return new byte[0];
                                    //throw new CryptographicException("PKCS v1.5 Decode Error #4: No Data");
                                }
                            }
                            else
                            {// #3: Invalid Key / Invalid Random Data Length
                                throw new CryptographicException("PKCS v1.5 Decode Error");
                            }
                        }
                        else
                        {// #2: Invalid Key / Invalid Identifiers
                            throw new CryptographicException("PKCS v1.5 Decode Error");
                        }
                    }
                    else
                    {// #1: Invalid Key / PKCS Encoding
                        throw new CryptographicException("PKCS v1.5 Decode Error");
                    }

                }
            }
            catch (CryptographicException ex)
            {
                throw new CryptographicException("Exception while decoding: " + ex.Message);
            }
            catch
            {
                throw new CryptographicException("Exception while decoding");
            }

           
        }

        #endregion

        #region PUBLIC FUNCTIONS

        /// <summary>
        /// Encrypts the given message with RSA, performs OAEP Encoding.
        /// </summary>
        /// <param name="Message">Message to Encrypt. Maximum message length is (ModulusLengthInOctets - 2 * HashLengthInOctets - 2)</param>
        /// <param name="OAEP_Params">Optional OAEP parameters. Normally Empty. But, must match the parameters while decryption.</param>
        /// <param name="usePrivate">True to use Private key for encryption. False to use Public key.</param>
        /// <returns>Encrypted message.</returns>
        public byte[] Encrypt(byte[] Message, byte[] OAEP_Params, bool usePrivate)
        {
            return RSAProcessEncodeOAEP(Message, OAEP_Params, usePrivate);
        }

        /// <summary>
        /// Encrypts the given message with RSA.
        /// </summary>
        /// <param name="Message">Message to Encrypt. Maximum message length is For OAEP [ModulusLengthInOctets - (2 * HashLengthInOctets) - 2] and for PKCS [ModulusLengthInOctets - 11]</param>
        /// <param name="usePrivate">True to use Private key for encryption. False to use Public key.</param>
        /// <param name="fOAEP">True to use OAEP encoding (Recommended), False to use PKCS v1.5 Padding.</param>
        /// <returns>Encrypted message.</returns>
        public byte[] Encrypt(byte[] Message, bool usePrivate, bool fOAEP)
        {
            if (fOAEP)
            {
                return RSAProcessEncodeOAEP(Message, new byte[0], usePrivate);
            }
            else
            {
                return RSAProcessEncodePKCS(Message, usePrivate);
            }
        }

        /// <summary>
        /// Encrypts the given message using RSA Public Key.
        /// </summary>
        /// <param name="Message">Message to Encrypt. Maximum message length is For OAEP [ModulusLengthInOctets - (2 * HashLengthInOctets) - 2] and for PKCS [ModulusLengthInOctets - 11]</param>
        /// <param name="fOAEP">True to use OAEP encoding (Recommended), False to use PKCS v1.5 Padding.</param>
        /// <returns>Encrypted message.</returns>
        public byte[] Encrypt(byte[] Message,  bool fOAEP)
        {
            if (fOAEP)
            {
                return RSAProcessEncodeOAEP(Message, new byte[0], false);
            }
            else
            {
                return RSAProcessEncodePKCS(Message, false);
            }
        }
        
        /// <summary>
        /// Decrypts the given RSA encrypted message.
        /// </summary>
        /// <param name="Message">The encrypted message.</param>
        /// <param name="usePrivate">True to use Private key for decryption. False to use Public key.</param>
        /// <param name="fOAEP">True to use OAEP.</param>
        /// <returns>Encrypted byte array.</returns>
        public byte[] Decrypt(byte[] Message, bool usePrivate, bool fOAEP)
        {
            return Decrypt(Message, new byte[0], usePrivate, fOAEP);
        }

        /// <summary>
        /// Decrypts the given RSA encrypted message.
        /// </summary>
        /// <param name="Message">The encrypted message.</param>
        /// <param name="OAEP_Params">Parameters to the OAEP algorithm (Must match the parameter while Encryption).</param>
        /// <param name="usePrivate">True to use Private key for decryption. False to use Public key.</param>
        /// <returns>Encrypted byte array.</returns>
        public byte[] Decrypt(byte[] Message, byte[] OAEP_Params, bool usePrivate)
        {
            return Decrypt(Message, OAEP_Params, usePrivate, true);
        }

        /// <summary>
        /// Decrypts the given RSA encrypted message using Private key.
        /// </summary>
        /// <param name="Message">The encrypted message.</param>
        /// <param name="fOAEP">True to use OAEP.</param>
        /// <returns>Encrypted byte array.</returns>
        public byte[] Decrypt(byte[] Message,  bool fOAEP)
        {
            return Decrypt(Message, new byte[0], true, fOAEP);
        }

        #endregion

    }
}

