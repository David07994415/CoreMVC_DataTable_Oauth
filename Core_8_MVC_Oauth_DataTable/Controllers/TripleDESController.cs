using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace Core_8_MVC_Oauth_DataTable.Controllers
{
    public class TripleDESController : Controller
    {
        public IActionResult Index(string type,string plaintext)
        {
           
            // 處裡Key
            string key = "12345678-0000-0000-0000-ABCDEFGHIJKL";

            TripleDESEncryption tripleDESEncryption = new TripleDESEncryption(key);

            if (type == "Encode")
            {
                string Encrypt = tripleDESEncryption.Encrypt(plaintext);
                ViewBag.Text = Encrypt;
            }
            else if (type == "Decode") {
                string Decrypt = tripleDESEncryption.Decrypt(plaintext);
                ViewBag.Text = Decrypt;
            }
            return View();
        }


    }

    public class TripleDESEncryption 
    {
        private readonly string _key; // 你的加密金鑰
        //private readonly byte[] _iv;  // 初始化向量 (IV)，非強制

        public TripleDESEncryption(string key)
        {
            _key = key;
            // IV 長度應為 8 位元組 (64 bits)，這裡是範例的靜態 IV，根據需求生成或固定
            //_iv = Encoding.UTF8.GetBytes("12345678");//非強制
        }

        public string Encrypt(string plainText)
        {
            using (var tripleDES = TripleDES.Create())
            {
                tripleDES.Key = GetKeyBytes(_key);
                //tripleDES.Mode = CipherMode.CBC;
                //tripleDES.Padding = PaddingMode.PKCS7;
                //// 設定隨機 IV
                //tripleDES.GenerateIV();

                // 非強制
                //tripleDES.IV = _iv;
                //tripleDES.Mode = CipherMode.CBC; // 模式
                //tripleDES.Padding = PaddingMode.PKCS7; // 填充方式
                //默認
                //IV(默認): 00 - 00 - 00 - 00 - 00 - 00 - 00 - 00
                //Mode(默認): CBC
                //Padding(默認): PKCS7

                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                using (var encryptor = tripleDES.CreateEncryptor())
                {
                    var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(encryptedBytes); // 加密後的 Base64 字串
                }
            }
        }

        public string Decrypt(string encryptedText)
        {
            using (var tripleDES = TripleDES.Create())
            {
                tripleDES.Key = GetKeyBytes(_key);
                //tripleDES.Mode = CipherMode.CBC;
                //tripleDES.Padding = PaddingMode.PKCS7;
                //// 設定隨機 IV
                //tripleDES.GenerateIV();
                //非強制
                //tripleDES.IV = _iv;
                //tripleDES.Mode = CipherMode.CBC;
                //tripleDES.Padding = PaddingMode.PKCS7;

                var encryptedBytes = Convert.FromBase64String(encryptedText);
                using (var decryptor = tripleDES.CreateDecryptor())
                {
                    var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }

        private byte[] GetKeyBytes(string key)
        {
            // Step 1: 移除 '-'
            string cleanKey = key.Replace("-", "");
            // Step 2: 將 32 個十六進位字元轉換為 16 bytes 的位元組
            byte[] keyBytes = HexStringToBytes(cleanKey);
            // Step 3: 如果需要 24 bytes 的金鑰，補足額外的 8 bytes
            //Array.Resize(ref keyBytes, 24);

            // TripleDES 的金鑰長度必須是 24 bytes，根據需求截取或補足
            //var keyBytes = Encoding.UTF8.GetBytes(key);
            //if (keyBytes.Length < 24)
            //{
            //    Array.Resize(ref keyBytes, 24);
            //}
            return keyBytes;
        }
        public byte[] HexStringToBytes(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }


    }

}
