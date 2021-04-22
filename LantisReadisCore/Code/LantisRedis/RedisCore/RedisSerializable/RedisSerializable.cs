using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Lantis.Redis;
using Lantis.Pool;

namespace Lantis.Redis
{
    public class RedisSerializable
    {
        public static byte[] Serialize(object data)
        {
            byte[] bytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter b = new BinaryFormatter();
                try
                {
                    b.Serialize(ms, data);
                }
                catch (Exception e)
                {
                    Logger.Log(e.ToString());
                }
                ms.Position = 0;
                bytes = new byte[ms.Length];
                ms.Read(bytes, 0, bytes.Length);
            }
            return bytes;
        }

        public static T DeSerialize<T>(byte[] bytes)
        {
            T target = default(T);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                BinaryFormatter b = new BinaryFormatter();
                ms.Seek(0, SeekOrigin.Begin);
                target = (T)b.Deserialize(ms);
            }

            return target;
        }

        public static RedisTableDefineAttribute FindRedisTableDefineAttribute(object tableData)
        {
            var type = tableData.GetType();
            var attributes = type.GetCustomAttributes(true);
            RedisTableDefineAttribute redisAttribute = null;

            for (var i = 0; i < attributes.Length; ++i)
            {
                var attributeItem = attributes[i];

                if (attributeItem is RedisTableDefineAttribute)
                {
                    redisAttribute = attributeItem as RedisTableDefineAttribute;
                    break;
                }
            }

            return redisAttribute;
        }

        public static byte[] SerializableToBytes(object tableData)
        {
            var type = tableData.GetType();
            RedisTableDefineAttribute redisAttribute = FindRedisTableDefineAttribute(tableData);
            var redisSerializData = RedisCore.ExternTableDataToRedisSerializData(redisAttribute == null ? string.Empty : redisAttribute.GetDatabaseName(), tableData);
            var bytes = Serialize(redisSerializData);
            LantisPoolSystem.GetPool<RedisSerializableData>().DisposeObject(redisSerializData);

            return bytes;
        }

        public static RedisSerializableData BytesToSerializable(byte[] datas)
        {
            return DeSerialize<RedisSerializableData>(datas);
        }

        public static RedisSerializableData SerializableToRedisSerializableData(object tableData)
        {
            var type = tableData.GetType();
            RedisTableDefineAttribute redisAttribute = FindRedisTableDefineAttribute(tableData);
            var redisSerializData = RedisCore.ExternTableDataToRedisSerializData(redisAttribute == null ? string.Empty : redisAttribute.GetDatabaseName(), tableData);

            return redisSerializData;
        }
    }
}
