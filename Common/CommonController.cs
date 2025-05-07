using EquidCMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace EquidCMS
{
    public class CommonController : Controller
    {
        #region Encryption
        public static string EncryptSHAHash(string InputString)
        {
            int Password_saltArraySize = 16;
            string saltAndPwd = String.Concat(InputString, Password_saltArraySize.ToString());
            HashAlgorithm hashAlgorithm = SHA512.Create();
            List<byte> pass = new List<byte>(Encoding.Unicode.GetBytes(saltAndPwd));
            string hashedPwd = Convert.ToBase64String(hashAlgorithm.ComputeHash(pass.ToArray()));
            hashedPwd = String.Concat(hashedPwd, Password_saltArraySize.ToString());
            return hashedPwd;
        }
        public static string Encrypt(string plainText)
        {
            if (plainText == null) throw new ArgumentNullException("plainText");

            //encrypt data
            var data = Encoding.Unicode.GetBytes(plainText);
            UnicodeEncoding uEncode = new UnicodeEncoding();
            byte[] bytPassword = uEncode.GetBytes(plainText);

            //return as base64 string
            return Convert.ToBase64String(data);
        }
        public static string Decrypt(string cipher)
        {
            if (cipher == null) throw new ArgumentNullException("cipher");

            //parse base64 string
            byte[] data = Convert.FromBase64String(cipher);
            return Encoding.Unicode.GetString(data);
        }
        #endregion

        #region Rendom String
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion

        #region Guid
        public static string GetGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "").ToUpper() + DateTime.Now.ToString("yyyyMMddHHss");
        }

        #endregion

        #region list to datatable
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties by using reflection   
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names  
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
        #endregion

        #region Datatable to List

        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        public static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        if (pro.PropertyType.ToString().Contains("DateTime"))
                        {
                            pro.SetValue(obj, dr[column.ColumnName] == DBNull.Value ? DateTime.Now : dr[column.ColumnName], null);
                        }
                        else if (pro.PropertyType.ToString().Contains("Int32"))
                        {
                            pro.SetValue(obj, dr[column.ColumnName] == DBNull.Value ? 0 : dr[column.ColumnName], null);
                        }
                        else if (pro.PropertyType.ToString().Contains("String"))
                        {
                            pro.SetValue(obj, dr[column.ColumnName] == DBNull.Value ? "" : dr[column.ColumnName], null);
                        }
                        else
                            pro.SetValue(obj, dr[column.ColumnName], null);

                    }
                    else
                        continue;
                }
            }
            return obj;
        }

        #endregion

        

       

        
        private static readonly int Password_saltArraySize = 16;
        public static string CreatePasswordHash(string pwd)
        {
            string saltAndPwd = String.Concat(pwd, Password_saltArraySize.ToString());
            HashAlgorithm hashAlgorithm = SHA512.Create();
            List<byte> pass = new List<byte>(Encoding.Unicode.GetBytes(saltAndPwd));
            string hashedPwd = Convert.ToBase64String(hashAlgorithm.ComputeHash(pass.ToArray()));
            hashedPwd = String.Concat(hashedPwd, Password_saltArraySize.ToString());
            return hashedPwd;
        }
    }
}