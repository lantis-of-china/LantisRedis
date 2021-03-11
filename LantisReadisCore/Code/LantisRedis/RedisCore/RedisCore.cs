using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Lantis.Extend;
using Lantis.Pool;
using Lantis.Redis.Message;
using System.Data;
using System.Text;

namespace Lantis.Redis
{
    /// <summary>
    /// memory readis core function define
    /// </summary>
    public class RedisCore
    {
        private static LantisDictronaryList<Type, string> typeCollects;

        public static string GetTypeName(Type type)
        {
            if (typeCollects == null)
            {
                typeCollects = new LantisDictronaryList<Type, string>();
                AddTypeCollect(typeof(Boolean));
                AddTypeCollect(typeof(Byte));
                AddTypeCollect(typeof(SByte));
                AddTypeCollect(typeof(UInt16));
                AddTypeCollect(typeof(Int16));
                AddTypeCollect(typeof(UInt32));
                AddTypeCollect(typeof(Int32));
                AddTypeCollect(typeof(UInt64));
                AddTypeCollect(typeof(Int64));
                AddTypeCollect(typeof(Single));
                AddTypeCollect(typeof(Double));
                AddTypeCollect(typeof(Decimal));
                AddTypeCollect(typeof(String));
            }

            if (typeCollects.HasKey(type))
            {
                return typeCollects[type];
            }

            if (IsList(type.Name))
            {
                return "List`1";
            }

            return type.Name;
        }

        public static void AddTypeCollect(Type type)
        {
            typeCollects.AddValue(type, type.Name);
        }
        /// <summary>
        /// General type determine
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsGeneralType(string type)
        {
            if (type == GetTypeName(typeof(Boolean)) ||
                type == GetTypeName(typeof(Byte)) ||
                type == GetTypeName(typeof(SByte)) ||
                type == GetTypeName(typeof(Int16)) ||
                type == GetTypeName(typeof(UInt16)) ||
                type == GetTypeName(typeof(Int32)) ||
                type == GetTypeName(typeof(UInt32)) ||
                type == GetTypeName(typeof(Int64)) ||
                type == GetTypeName(typeof(UInt64)) ||
                type == GetTypeName(typeof(Single)) ||
                type == GetTypeName(typeof(Double)) ||
                type == GetTypeName(typeof(Decimal)))
            {
                return true;
            }

            return false;
        }

        public static bool IsStringType(string type)
        {
            if (type == GetTypeName(typeof(string)))
            {
                return true;
            }

            return false;
        }

        public static bool IsList(string type)
        {
            if (type.Contains("List`1"))
            {
                return true;
            }

            return false;
        }

        public static string GetDatabaseNameFromData(object data)
        {
            var attribute = data.GetType().GetCustomAttribute<RedisTableDefineAttribute>();

            if (attribute != null)
            {
                return attribute.GetDatabaseName();
            }

            return "";
        }

        public static RedisCheckDatabase GetTypeField(Type type)
        {
            var attribute = type.GetCustomAttribute<RedisTableDefineAttribute>();

            if (attribute != null)
            {
                var requestRedisCheck = LantisPoolSystem.GetPool<RedisCheckDatabase>().NewObject();
                requestRedisCheck.databaseName = attribute.GetDatabaseName();
                requestRedisCheck.tableName = type.Name;
                requestRedisCheck.tableInfos = new List<RedisTableFieldDefine>();
                var poolHandle = LantisPoolSystem.GetPool<RedisTableFieldDefine>();
                var fields = type.GetFields();

                for (var i = 0; i < fields.Length; ++i)
                {
                    var field = fields[i];
                    var fieldDefine = poolHandle.NewObject();
                    fieldDefine.fieldName = field.Name;                    
                    fieldDefine.fieldType = GetTypeName(field.FieldType);
                    requestRedisCheck.tableInfos.Add(fieldDefine);
                }

                return requestRedisCheck;
            }
            else
            {
                Logger.Error($"can't find attribute at {type.Name}");

                return null;
            }
        }

        /// <summary>
        /// translate object data to string value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetStringValueObject(object value)
        {
            string resultString = "";

            var type = value.GetType();

            if (IsGeneralType(type.Name))
            {
                resultString = value.ToString();
            }
            else if (type == typeof(String))
            {
                resultString = $"'{value}'";
            }
            else
            {
                resultString = $"'{ Newtonsoft.Json.JsonConvert.SerializeObject(value) }'";
            }

            return resultString;
        }

        /// <summary>
        /// get value to string from memory redis base
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="field"></param>
        /// <param name="memoryRedisData"></param>
        /// <returns></returns>
        public static string GetStringValue(Type classType, FieldInfo field, RedisBase memoryRedisData)
        {
            var value = field.GetValue(memoryRedisData);
            var resultString = GetStringValueObject(value);

            return resultString;
        }

        /// <summary>
        /// memory redis base data translate to string data
        /// </summary>
        /// <param name="memoryRedisdata"></param>
        /// <returns></returns>
        public static string RedisBaseToStringData(RedisBase memoryRedisdata)
        {
            var redisDataType = memoryRedisdata.GetType();
            var fieldsArray = redisDataType.GetFields();
            var tableName = redisDataType.Name;
            var stringData = string.Format("{tableName:{0},data:{", tableName);

            for (var i = 0; i < fieldsArray.Length; ++i)
            {
                var fieldInfo = fieldsArray[i];
                var value = GetStringValue(redisDataType, fieldInfo, memoryRedisdata);
                var fieldStringInfo = string.Format("{'name':{0},'type':{1},value:{2}}", fieldInfo.Name, fieldInfo.FieldType.Name, value);

                if (i > 0)
                {
                    stringData += string.Format(",{0}", fieldStringInfo);
                }
                else
                {
                    stringData += fieldStringInfo;
                }
            }

            stringData += "}}";

            return stringData;
        }

        /// <summary>
        /// bytes data translate to redis table struct
        /// </summary>
        /// <param name="data"></param>
        /// <param name="redisTableData"></param>

        public static void DataToRedisTableData(byte[] data, RedisTableData redisTableData)
        {
            var redisSerializable = RedisSerializable.BytesToSerializable(data);

            for (var i = 0; i < redisSerializable.fields.Count; ++i)
            {
                var fieldItem = redisSerializable.fields[i];
                redisTableData.AddField(DataToRedisTableField(fieldItem));
            }
        }

        public static RedisTableData RedisSerializableToRedisTableData(RedisSerializableData redisSerializableData)
        {
            var redisTableData = LantisPoolSystem.GetPool<RedisTableData>().NewObject();

            for (var i = 0; i < redisSerializableData.fields.Count; ++i)
            {
                var fieldItem = redisSerializableData.fields[i];
                redisTableData.AddField(DataToRedisTableField(fieldItem));
            }

            return redisTableData;
        }

        public static RedisTableField DataToRedisTableField(RedisSerializableField redisSerializableField)
        {
            var redisField = LantisPoolSystem.GetPool<RedisTableField>().NewObject();
            redisField.fieldName = redisSerializableField.fieldName;
            redisField.fieldType = redisSerializableField.fieldType;

            if (IsGeneralType(redisSerializableField.fieldType))
            {
                redisField.fieldValue = redisSerializableField.fieldValue;
            }
            else if (IsStringType(redisSerializableField.fieldType))
            {
                redisField.fieldValue = redisSerializableField.fieldValue;
            }
            else if (IsList(redisSerializableField.fieldType))
            {
                var listType = redisSerializableField.fieldValue.GetType();
                var genericType = listType.GetGenericArguments()[0];

                if (IsGeneralType(genericType.Name))
                {
                    redisField.fieldValue = redisSerializableField.fieldValue;
                }
                else if (IsStringType(genericType.Name))
                {
                    redisField.fieldValue = redisSerializableField.fieldValue;
                }
                else
                {
                    var fieldList = new List<RedisTableData>();
                    redisField.fieldValue = fieldList;
                    var serializableData = redisSerializableField.fieldValue as IList;
                    var poolHandle = LantisPoolSystem.GetPool<RedisTableData>();

                    for (var i = 0; i < serializableData.Count; ++i)
                    {
                        var redisSerializable = serializableData[i] as RedisSerializableData;
                        var redisTableField = poolHandle.NewObject();
                        redisTableField.databaseName = redisSerializable.databaseName;
                        var dataFields = redisSerializable.fields;

                        for (var j = 0; j < dataFields.Count; ++j)
                        {
                            var serializableField = dataFields[j];
                            var tableField = DataToRedisTableField(serializableField);
                            redisTableField.AddField(tableField);
                        }

                        fieldList.Add(redisTableField);
                    }
                }
            }
            else
            {
                redisField.fieldValue = RedisSerializableToRedisTableData((RedisSerializableData)redisSerializableField.fieldValue);
            }

            return redisField;
        }

        /// <summary>
        /// redis table data translate to bytes
        /// </summary>
        /// <param name="redisTableData"></param>
        public static byte[] RedisTableDataToData(RedisTableData redisTableData)
        {
            var redisSerializableData = LantisPoolSystem.GetPool<RedisSerializableData>().NewObject();

            redisTableData.GetFieldCollects().SafeWhile(delegate (string key, RedisTableField redisField)
            {
                var redisSerializableField = RedisTableFieldToSerializableField(redisField);
                redisSerializableData.AddFieldData(redisSerializableField);
            });

            var datas = RedisSerializable.Serialize(redisSerializableData);
            LantisPoolSystem.GetPool<RedisSerializableData>().DisposeObject(redisSerializableData);

            return datas;
        }

        /// <summary>
        /// redis table data translate to redis serializable data
        /// </summary>
        /// <param name="redisTableData"></param>
        /// <returns></returns>
        public static RedisSerializableData RedisTableDataToRedisSerializableData(RedisTableData redisTableData)
        {
            var redisSerializableData = LantisPoolSystem.GetPool<RedisSerializableData>().NewObject();

            redisTableData.GetFieldCollects().SafeWhile(delegate (string key, RedisTableField redisField)
            {
                var redisSerializableField = RedisTableFieldToSerializableField(redisField);
                redisSerializableData.AddFieldData(redisSerializableField);
            });

            return redisSerializableData;
        }

        /// <summary>
        /// redis table field translate to serializable field
        /// </summary>
        /// <param name="redisTableField"></param>
        /// <returns></returns>
        public static RedisSerializableField RedisTableFieldToSerializableField(RedisTableField redisTableField)
        {
            var redisField = LantisPoolSystem.GetPool<RedisSerializableField>().NewObject();
            redisField.fieldName = redisTableField.fieldName;
            redisField.fieldType = redisTableField.fieldType;

            if (IsGeneralType(redisTableField.fieldType))
            {
                redisField.fieldValue = redisTableField.fieldValue;
            }
            else if (IsStringType(redisTableField.fieldType))
            {
                redisField.fieldValue = redisTableField.fieldValue;
            }
            else if (IsList(redisTableField.fieldType))
            {
                var type = redisTableField.fieldValue.GetType();
                var genericType = type.GetGenericArguments()[0];
                var listField = redisTableField.fieldValue as IList;

                if (IsGeneralType(genericType.Name))
                {
                    redisField.fieldValue = type.Assembly.CreateInstance(type.FullName, false);
                    var listHandle = redisField.fieldValue as IList;

                    for (var i = 0; i < listField.Count; ++i)
                    {
                        listHandle.Add(listField[i]);
                    }
                }
                else if (IsStringType(genericType.Name))
                {
                    redisField.fieldValue = type.Assembly.CreateInstance(type.FullName, false);
                    var listHandle = redisField.fieldValue as IList;

                    for (var i = 0; i < listField.Count; ++i)
                    {
                        listHandle.Add(listField[i]);
                    }
                }
                else
                {
                    redisField.fieldValue = new List<RedisSerializableData>();
                    var listHandle = redisField.fieldValue as IList;

                    for (var i = 0; i < listField.Count; ++i)
                    {
                        var tableFieldData = listField[i];
                        listHandle.Add(RedisTableDataToRedisSerializableData(tableFieldData as RedisTableData));
                    };
                }
            }
            else
            {
                redisField.fieldValue = RedisTableDataToRedisSerializableData(redisTableField.fieldValue as RedisTableData);
            }

            return redisField;
        }

        /// <summary>
        /// get memory redis data type name
        /// </summary>
        /// <param name="memoryRedisdata"></param>
        /// <returns></returns>
        public static string GetMemoryRedisDataTypeName(RedisBase memoryRedisdata)
        {
            return memoryRedisdata.GetType().Name;
        }

        /// <summary>
        /// get type name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetTypeName<T>()
        {
            return typeof(T).Name;
        }

        /// <summary>
        /// extern database translate to redis serializ data struct
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="externTableData"></param>
        /// <returns></returns>
        public static RedisSerializableData ExternTableDataToRedisSerializData(string databaseName, object externTableData)
        {
            var redisSerializData = LantisPoolSystem.GetPool<RedisSerializableData>().NewObject();
            var type = externTableData.GetType();
            var fields = type.GetFields();
            redisSerializData.databaseName = databaseName;

            for (var i = 0; i < fields.Length; ++i)
            {
                var fieldItem = fields[i];
                var serializField = ExternFieldDataToRedisSerializField(fieldItem, externTableData);
                redisSerializData.AddFieldData(serializField);
            }

            return redisSerializData;
        }

        /// <summary>
        /// extern database field translate to redis field data struct
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="externTableData"></param>
        /// <returns></returns>
        public static RedisSerializableField ExternFieldDataToRedisSerializField(FieldInfo fieldInfo, object externTableData)
        {
            var fieldType = fieldInfo.FieldType;
            var fieldData = fieldInfo.GetValue(externTableData);
            var redisSerializField = LantisPoolSystem.GetPool<RedisSerializableField>().NewObject();
            redisSerializField.fieldName = fieldInfo.Name;
            redisSerializField.fieldType = fieldType.Name;

            if (IsGeneralType(fieldType.Name))
            {
                redisSerializField.fieldValue = fieldData;
            }
            else if (IsStringType(fieldType.Name))
            {
                redisSerializField.fieldValue = fieldData;
            }
            else if (IsList(fieldType.Name))
            {
                var listValue = fieldData as IList;
                var genericTypeArry = fieldType.GetGenericArguments();
                var genericType = genericTypeArry[0];

                if (IsGeneralType(genericType.Name))
                {
                    redisSerializField.fieldValue = fieldType.Assembly.CreateInstance(fieldType.FullName, false);
                    var listField = redisSerializField.fieldValue as IList;

                    for (var i = 0; i < listValue.Count; ++i)
                    {
                        var listItem = listValue[i];
                        listField.Add(listItem);
                    }
                }
                else if (IsStringType(genericType.Name))
                {
                    redisSerializField.fieldValue = fieldType.Assembly.CreateInstance(fieldType.FullName, false);
                    var listField = redisSerializField.fieldValue as IList;

                    for (var i = 0; i < listValue.Count; ++i)
                    {
                        var listItem = listValue[i];
                        listField.Add(listItem);
                    }
                }
                else
                {
                    redisSerializField.fieldValue = new List<RedisSerializableData>();
                    var listField = redisSerializField.fieldValue as IList;

                    for (var i = 0; i < listValue.Count; ++i)
                    {
                        var listItem = listValue[i];
                        var serializData = ExternTableDataToRedisSerializData(string.Empty, listItem);
                        listField.Add(serializData);
                    }
                }
            }
            else
            {
                redisSerializField.fieldValue = ExternTableDataToRedisSerializData(string.Empty, fieldData);
            }

            return redisSerializField;
        }

        public static string GetSelectTableString(string tableName)
        {
            string sqlSelect = $"SELECT * FROM {tableName}";

            return sqlSelect;
        }

        public static string GetCreateTableString(string tableName, List<RedisTableFieldDefine> redisTableFieldDefines)
        {
            string sqlCreate = $"CREATE TABLE [dbo].[{tableName}]( ";

            for (var j = 0; j < redisTableFieldDefines.Count; ++j)
            {
                var field = redisTableFieldDefines[j];

                sqlCreate += $"[{field.fieldName}] {SqlValueTypeByType.GetSqlType(field.fieldType)} NOT NULL,";
            }

            sqlCreate += ")";

            return sqlCreate;
        }

        public static string GetCheckTableString(string tableName)
        {
            return $"if object_id('{tableName}','u') is not null select 1 else select 0";
        }

        public static string GetUpdataCommand(string tableName, RedisTableData redisTableData, LantisRedisConditionGroup conditionGroup)
        {
            var fieldList = redisTableData.GetFieldCollects().ValueToList();
            string sqlString = "";
            string fields = "";

            for (var i = 0; i < fieldList.Count; ++i)
            {
                var field = fieldList[i];
                var value = GetSingleValue(field.fieldType, field.fieldValue);

                if (i != 0)
                {
                    fields += ",";
                }

                fields += $"{field.fieldName} = {value}";
            }

            string whereString = "";

            for (var i = 0; i < conditionGroup.conditionList.Count; ++i)
            {
                var condition = conditionGroup.conditionList[i];

                if (i != 0)
                {
                    whereString += "AND";
                }

                whereString += $"{condition.fieldName} {condition.operation} {condition.fieldValue}";
            }
            

            if (string.IsNullOrEmpty(whereString))
            {
                sqlString = $"UPDATE {tableName} SET {fields}";
            }
            else
            {
                sqlString = $"UPDATE {tableName} SET {fields} WHERE {whereString}";
            }

            return sqlString;
        }

        public static string GetInsertCommand(string tableName,RedisTableData redisTableData)
        {
            var fieldList = redisTableData.GetFieldCollects().ValueToList();
            string sqlString = "";
            string keys = "(";
            string values = "(";

            for (var i = 0; i < fieldList.Count; ++i)
            {
                var field = fieldList[i];
                var value = GetSingleValue(field.fieldType, field.fieldValue);

                if (i != 0)
                {
                    keys += ",";
                    values += ",";
                }

                keys += field.fieldName;
                values += value;
            }

            keys += ")";
            values += ")";
            sqlString = $"INSERT INTO {tableName} {keys} VALUES{values}";

            return sqlString;


            //			if ((int)SqlHelp.ExecuteNonQuery(sqlString) == 1)
            //			{
            //				Logger.Log("sql执行成功");
            //				StartExecuteAsync();
            //			}
            //			else
            //			{
            //				failedTimes++;
            //				Logger.Error($"sql执行失败次数{failedTimes}");

            //				if (failedTimes > 3)
            //				{
            //					Execute(executeInfo, failedTimes);
            //				}
            //				else
            //				{
            //					Logger.Error("sql执行失败次数过多 忽略当前数据");
            //					StartExecuteAsync();
            //				}
            //			}
        }

        public static void DataTableToMemory(RedisTable redisTable,DataTable dataTable)
        {
            if (dataTable != null && dataTable.Rows != null)
            {
                for (var i = 0; i < dataTable.Rows.Count; ++i)
                {
                    var oneDataRow = dataTable.Rows[i];
                    var tableFields = redisTable.GetRedisTableFieldList();
                    var oneData = LantisPoolSystem.GetPool<RedisTableData>().NewObject();

                    for (var j = 0; j < tableFields.Count; ++j)
                    {
                        var fieldInfo = tableFields[j];
                        SetDataRowToRedisTableData(fieldInfo, oneData, oneDataRow);
                    }

                    var fieldObject = oneData.GetFieldObject(RedisConst.id);
                    redisTable.AddDataById(fieldObject.fieldValue as string, oneData);
                }
            }
        }

        public static RedisTableData SetDataRowToRedisTableData(RedisTableFieldDefine fieldDefine,RedisTableData redisTableData, DataRow dataRow)
        {
            var oneField = LantisPoolSystem.GetPool<RedisTableField>().NewObject();
            oneField.fieldName = fieldDefine.fieldName;
            oneField.fieldType = fieldDefine.fieldType;
            redisTableData.AddField(oneField);
            var type = fieldDefine.fieldType;

            if (type == GetTypeName(typeof(Boolean)))
            {
                oneField.fieldValue = Boolean.Parse(dataRow[fieldDefine.fieldName].ToString());
            }
            else if (type == GetTypeName(typeof(Byte)))
            {
                oneField.fieldValue = Byte.Parse(dataRow[fieldDefine.fieldName].ToString());
            }
            else if (type == GetTypeName(typeof(SByte)))
            {
                oneField.fieldValue = SByte.Parse(dataRow[fieldDefine.fieldName].ToString());
            }
            else if (type == GetTypeName(typeof(Int16)))
            {
                oneField.fieldValue = Int16.Parse(dataRow[fieldDefine.fieldName].ToString());
            }
            else if (type == GetTypeName(typeof(UInt16)))
            {
                oneField.fieldValue = UInt16.Parse(dataRow[fieldDefine.fieldName].ToString());
            }
            else if (type == GetTypeName(typeof(Int32)))
            {
                oneField.fieldValue = Int32.Parse(dataRow[fieldDefine.fieldName].ToString());
            }
            else if (type == GetTypeName(typeof(UInt32)))
            {
                oneField.fieldValue = UInt32.Parse(dataRow[fieldDefine.fieldName].ToString());
            }
            else if (type == GetTypeName(typeof(Int64)))
            {
                oneField.fieldValue = Int64.Parse(dataRow[fieldDefine.fieldName].ToString());
            }
            else if (type == GetTypeName(typeof(UInt64)))
            {
                oneField.fieldValue = UInt64.Parse(dataRow[fieldDefine.fieldName].ToString());
            }
            else if (type == GetTypeName(typeof(Single)))
            {
                oneField.fieldValue = Single.Parse(dataRow[fieldDefine.fieldName].ToString());
            }
            else if (type == GetTypeName(typeof(Double)))
            {
                oneField.fieldValue = Double.Parse(dataRow[fieldDefine.fieldName].ToString());
            }
            else if (type == GetTypeName(typeof(Decimal)))
            {
                oneField.fieldValue = Decimal.Parse(dataRow[fieldDefine.fieldName].ToString());
            }
            else if (type == GetTypeName(typeof(String)))
            {
                oneField.fieldValue = dataRow[fieldDefine.fieldName].ToString();
            }
            else if (IsList(type))
            {
                var base64String = dataRow[fieldDefine.fieldName].ToString();
                var bytes = Convert.FromBase64String(base64String);
                var typeLenght = BitConverter.ToInt32(bytes,0);
                var valueType = Encoding.UTF8.GetString(bytes, 4, typeLenght);
                byte[] datas = new byte[bytes.Length - 4 - typeLenght];
                Array.Copy(bytes, 4 + typeLenght, datas, 0, datas.Length);

                if (valueType == GetTypeName(typeof(Boolean)))
                {
                    oneField.fieldValue = RedisSerializable.DeSerialize<List<Boolean>>(datas);
                }
                else if (valueType == GetTypeName(typeof(Byte)))
                {
                    oneField.fieldValue = RedisSerializable.DeSerialize<List<Byte>>(datas);
                }
                else if (valueType == GetTypeName(typeof(SByte)))
                {
                    oneField.fieldValue = RedisSerializable.DeSerialize<List<SByte>>(datas);
                }
                else if (valueType == GetTypeName(typeof(Int16)))
                {
                    oneField.fieldValue = RedisSerializable.DeSerialize<List<Int16>>(datas);
                }
                else if (valueType == GetTypeName(typeof(UInt16)))
                {
                    oneField.fieldValue = RedisSerializable.DeSerialize<List<UInt16>>(datas);
                }
                else if (valueType == GetTypeName(typeof(Int32)))
                {
                    oneField.fieldValue = RedisSerializable.DeSerialize<List<Int32>>(datas);
                }
                else if (valueType == GetTypeName(typeof(UInt32)))
                {
                    oneField.fieldValue = RedisSerializable.DeSerialize<List<UInt32>>(datas);
                }
                else if (valueType == GetTypeName(typeof(Int64)))
                {
                    oneField.fieldValue = RedisSerializable.DeSerialize<List<Int64>>(datas);
                }
                else if (valueType == GetTypeName(typeof(UInt64)))
                {
                    oneField.fieldValue = RedisSerializable.DeSerialize<List<UInt64>>(datas);
                }
                else if (valueType == GetTypeName(typeof(Single)))
                {
                    oneField.fieldValue = RedisSerializable.DeSerialize<List<Single>>(datas);
                }
                else if (valueType == GetTypeName(typeof(Double)))
                {
                    oneField.fieldValue = RedisSerializable.DeSerialize<List<Double>>(datas);
                }
                else if (valueType == GetTypeName(typeof(Decimal)))
                {
                    oneField.fieldValue = RedisSerializable.DeSerialize<List<Decimal>>(datas);
                }
                else if (valueType == GetTypeName(typeof(String)))
                {
                    oneField.fieldValue = RedisSerializable.DeSerialize<List<String>>(datas);
                }
                else
                {
                    oneField.fieldValue = RedisSerializable.DeSerialize<List<RedisTableData>>(datas);
                }
            }
            else
            {
                var newTableData = LantisPoolSystem.GetPool<RedisTableData>().NewObject();
                var base64String = dataRow[fieldDefine.fieldName].ToString();
                var bytes = Convert.FromBase64String(base64String);
                DataToRedisTableData(bytes, redisTableData);
                oneField.fieldValue = newTableData;
            }

            return redisTableData;
        }

        public static string GetSingleValue(string type, object value)
        {
            if (IsGeneralType(type))
            {
                return value.ToString();
            }
            if (IsStringType(type))
            {
                return $"'{value as string}'";
            }
            else if (IsList(type))
            {
                var dataType = value.GetType();
                var genericTypeArry = dataType.GetGenericArguments();
                var genericType = genericTypeArry[0];
                byte[] typeData = Encoding.UTF8.GetBytes(genericType.Name);
                byte[] typeLenght = BitConverter.GetBytes(typeData.Length);
                byte[] bytes = null;

                if (IsGeneralType(genericType.Name))
                {
                    bytes = RedisSerializable.Serialize(value);
                }
                else if (IsStringType(genericType.Name))
                {
                    bytes = RedisSerializable.Serialize(value);
                }
                else 
                {
                    bytes = RedisSerializable.Serialize(value);
                }

                var dataBuff = new byte[4 + typeData.Length + bytes.Length];
                Array.Copy(typeLenght, 0, dataBuff, 0, typeLenght.Length);
                Array.Copy(typeData, 0, dataBuff, 4, typeData.Length);
                Array.Copy(bytes, 0, dataBuff, 4 + typeData.Length, bytes.Length);

                return $"'{Convert.ToBase64String(dataBuff)}'";
            }
            else
            {
                var bytes = RedisTableDataToData((RedisTableData)value);

                return $"'{Convert.ToBase64String(bytes)}'";
            }
        }

        public static void DataTableToRedisTableData(DataTable dataTable)
        {
            for (var i = 0; i < dataTable.Rows.Count; ++i)
            {
                var dr = dataTable.Rows[i];
                DataRowCollectToData(dr);
            }
        }

        public static void DataRowCollectToData(DataRow dr)
        {
            
        }
    }
}
