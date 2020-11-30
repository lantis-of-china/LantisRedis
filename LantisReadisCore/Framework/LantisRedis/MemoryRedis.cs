//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace LantisRedisCore
//{
//	public class MemoryRedis
//	{
//        /// <summary>
//        /// check lock
//        /// </summary>
//		public static object lockTarget = new object();
//        /// <summary>
//        /// record memory state
//        /// </summary>
//		private static Dictionary<string,Dictionary<object, RedisState>> memoryTable = new Dictionary<string, Dictionary<object, RedisState>>();

//		private static List<DbExecuteInfo> waitExecuteList = new List<DbExecuteInfo>();

//		/// <summary>
//		/// 数据库列表
//		/// </summary>
//		private static List<Type> dbList = new List<Type>()
//		{
//		};

//		public static List<T> List2Type<T>(List<RedisState> datas) where T : RedisBase
//		{
//			List<T> dataList = new List<T>();

//			foreach (var item in datas)
//			{
//				dataList.Add((T)item.data);
//			}

//			return dataList;
//		}

//		/// <summary>
//		/// 获取以及创建模板
//		/// </summary>
//		/// <typeparam name="T"></typeparam>
//		/// <returns></returns>
//		public static Dictionary<object, RedisState> GetTempmentDictionary<T>() where T : RedisBase
//		{
//			lock (lockTarget)
//			{
//				Type dataType = typeof(T);

//				if (memoryTable.ContainsKey(dataType.Name))
//				{
//					return memoryTable[dataType.Name];
//				}
//				else
//				{
//					var dataDictionary = new Dictionary<object, RedisState>();

//					memoryTable.Add(dataType.Name, dataDictionary);

//					return dataDictionary;
//				}
//			}
//		}

//		public static Dictionary<object, RedisState> GetTempmentDictionary(Type dataType)
//		{
//			lock (lockTarget)
//			{
//				if (memoryTable.ContainsKey(dataType.Name))
//				{
//					return memoryTable[dataType.Name];
//				}
//				else
//				{
//					var dataDictionary = new Dictionary<object, RedisState>();

//					memoryTable.Add(dataType.Name, dataDictionary);

//					return dataDictionary;
//				}
//			}
//		}

//		/// <summary>
//		/// 从列表移除匹配数据 任一个数据就移除
//		/// </summary>
//		/// <typeparam name="T"></typeparam>
//		/// <param name="dataList"></param>
//		/// <param name="fields"></param>
//		/// <returns></returns>
//		public static List<T> RemoveDataWithFieldsOrList<T>(List<T> dataList, List<KeyValuePair<string, object>> fields) where T : RedisBase
//		{
//			Type dataType = typeof(T);

//			var recordFieldList = new List<KeyValuePair<FieldInfo, KeyValuePair<string, object>>>();

//			for (var i = 0; i < fields.Count; ++i)
//			{
//				var fieldInfo = dataType.GetField(fields[i].Key);

//				recordFieldList.Add(new KeyValuePair<FieldInfo, KeyValuePair<string, object>>(fieldInfo, fields[i]));
//			}

//			for (var r = dataList.Count - 1; r >= 0; --r)
//			{
//				var dataItem = dataList[r];

//				for (var i = 0; i < recordFieldList.Count; ++i)
//				{
//					var recordKv = recordFieldList[i];

//					var value = recordKv.Key.GetValue(dataItem);

//					if (recordKv.Value.Value.Equals(value))
//					{
//						dataList.RemoveAt(r);

//						break;
//					}
//				}
//			}

//			return dataList;
//		}

//		private static void LoadAddData(object id, RedisBase data,Type dataType)
//		{
//			lock (lockTarget)
//			{
//				var dataDictionary = GetTempmentDictionary(dataType);

//				if (dataDictionary.ContainsKey(id))
//				{
//					Logger.Error("加载重复ID数据Type:" + data.GetType() + " Id:" + id);
//				}
//				else
//				{
//					var dataWarp = new RedisState
//					{
//						isNew = false,
//						isSave = true,
//						data = data,
//					};

//					dataDictionary.Add(id, dataWarp);
//				}
//			}
//		}

//		/// <summary>
//		/// 设置数据
//		/// </summary>
//		/// <param name="menberId"></param>
//		/// <param name="data"></param>
//		public static void SetData<T>(object id, T data) where T : RedisBase
//		{
//			lock (lockTarget)
//			{
//				var dataDictionary = GetTempmentDictionary<T>();

//				if (dataDictionary.ContainsKey(id))
//				{
//					var dataWarp = dataDictionary[id];

//					dataWarp.data = data;

//					if (!dataWarp.isNew)
//					{
//						//进存储栈
//						dataWarp.isSave = false;
//					}

//					var executeInfo = new DbExecuteInfo();
//					executeInfo.type = typeof(T);
//					executeInfo.executeType = DbExecuteType.Update;
//					executeInfo.data = dataWarp;

//					if (IsGeneralType(id.GetType()))
//					{
//						executeInfo.whereField.Add("id");
//					}
//					else
//					{
//						executeInfo.whereField.Add("convert(nvarchar(max), id)");
//					}

//					executeInfo.whereOperator.Add("=");
//					executeInfo.whereValue.Add(GetStringValueObject(id));
//					AddExecute(executeInfo);
//				}
//				else
//				{
//					var dataWarp = new RedisState
//					{
//						isNew = true,
//						isSave = false,
//						data = data,
//					};

//					dataDictionary.Add(id, dataWarp);
//					var executeInfo = new DbExecuteInfo();
//					executeInfo.type = typeof(T);
//					executeInfo.executeType = DbExecuteType.Insert;
//					executeInfo.data = dataWarp;
//					AddExecute(executeInfo);
//				}
//			}
//		}
		
//		/// <summary>
//		/// 获取数据
//		/// </summary>
//		/// <param name="menberId"></param>
//		/// <returns></returns>
//		public static T GetData<T>(object id) where T : RedisBase
//		{
//			lock (lockTarget)
//			{
//				Type dataType = typeof(T);

//				if (memoryTable.ContainsKey(dataType.Name) && memoryTable[dataType.Name].ContainsKey(id))
//				{
//					return (T)memoryTable[dataType.Name][id].data;
//				}
//			}

//			return null;
//		}

//		/// <summary>
//		/// 获取数据指定页面数据
//		/// </summary>
//		/// <typeparam name="T"></typeparam>
//		/// <param name="everyCount"></param>
//		/// <param name="page"></param>
//		/// <returns></returns>
//		private static ParamarResult GetPageDatasFromData<T>(List<T> data, int everyCount, int page) where T : RedisBase
//		{
//			var datas = data;

//			ParamarResult result = new ParamarResult();

//			List<T> dataList = new List<T>();

//			var toldPage = datas.Count / everyCount;

//			if (datas.Count % everyCount > 0)
//			{
//				toldPage++;
//			}

//			result.paramar = toldPage;

//			if (page > toldPage)
//			{
//				page = toldPage;
//			}

//			if (page > 0)
//			{
//				int startIndex = (page - 1) * everyCount;

//				int endIndex = startIndex + everyCount;

//				if (endIndex >= datas.Count)
//				{
//					endIndex = datas.Count - 1;
//				}

//				for (var i = startIndex; i <= endIndex; ++i)
//				{
//					dataList.Add((T)datas[i]);
//				}
//			}

//			result.result = dataList;

//			return result;
//		}

//		/// <summary>
//		/// 获取数据指定页面数据
//		/// </summary>
//		/// <typeparam name="T"></typeparam>
//		/// <param name="everyCount"></param>
//		/// <param name="page"></param>
//		/// <returns></returns>
//		public static ParamarResult GetPageDatas<T>(int everyCount,int page) where T : RedisBase
//		{
//			ParamarResult result = new ParamarResult();

//			result.result = new List<T>();

//			lock (lockTarget)
//			{
//				Type dataType = typeof(T);

//				if (memoryTable.ContainsKey(dataType.Name))
//				{
//					var datas = memoryTable[dataType.Name].Values.ToList();
					
//					result = GetPageDatasFromData<T>(List2Type<T>(datas), everyCount, page);
//				}
//			}

//			return result;
//		}

//		/// <summary>
//		/// 获取指定匹配参数的数据页数据
//		/// </summary>
//		/// <typeparam name="T"></typeparam>
//		/// <param name="fields"></param>
//		/// <param name="everyCount"></param>
//		/// <param name="page"></param>
//		/// <returns></returns>
//		public static ParamarResult GetPageDatasWithField<T>(List<KeyValuePair<string, object>> fields, int everyCount, int page) where T : RedisBase
//		{
//			ParamarResult result = null;

//			var dataList = GetDatasWithFieldsList<T>(fields);

//			result = GetPageDatasFromData<T>(dataList, everyCount, page);

//			return result;
//		}

//		/// <summary>
//		/// 获取指定匹配参数的数据页数据
//		/// </summary>
//		/// <typeparam name="T"></typeparam>
//		/// <param name="fields"></param>
//		/// <param name="everyCount"></param>
//		/// <param name="page"></param>
//		/// <returns></returns>
//		public static ParamarResult GetPageDatasWithFieldsStringContainsList<T>(List<KeyValuePair<string, string>> fields, int everyCount, int page) where T : RedisBase
//		{
//			ParamarResult result = null;

//			var dataList = GetDataWithFieldsStringContainsList<T>(fields);

//			result = GetPageDatasFromData<T>(dataList, everyCount, page);

//			return result;
//		}

//		/// <summary>
//		/// 获取包含字符串并且排斥任意匹配条件后的数据
//		/// </summary>
//		/// <typeparam name="T"></typeparam>
//		/// <param name="fields"></param>
//		/// <param name="extends"></param>
//		/// <param name="everyCount"></param>
//		/// <param name="page"></param>
//		/// <returns></returns>
//		public static ParamarResult GetPageDatasWithFieldsStringContainsAndExtendOrList<T>(List<KeyValuePair<string, string>> fields, List<KeyValuePair<string, object>> extends, int everyCount, int page) where T : RedisBase
//		{
//			ParamarResult result = null;

//			var dataList = GetDataWithFieldsStringContainsList<T>(fields);

//			dataList = RemoveDataWithFieldsOrList(dataList, extends);

//			result = GetPageDatasFromData<T>(dataList, everyCount, page);

//			return result;
//		}

//		/// <summary>
//		/// 获取字段数据匹配的第一个数据
//		/// </summary>
//		/// <param name="menberId"></param>
//		/// <returns></returns>
//		public static T GetDataWithFieldsFirst<T>(List<KeyValuePair<string,object>> fields) where T : RedisBase
//		{
//			lock (lockTarget)
//			{
//				Type dataType = typeof(T);

//				var recordFieldList = new List<KeyValuePair<FieldInfo, KeyValuePair<string, object>>>();

//				for (var i = 0; i < fields.Count; ++i)
//				{
//					var fieldInfo = dataType.GetField(fields[i].Key);

//					recordFieldList.Add(new KeyValuePair<FieldInfo, KeyValuePair<string, object>>(fieldInfo, fields[i]));
//				}				

//				if (memoryTable.ContainsKey(dataType.Name))
//				{
//					var typeDirctionary = memoryTable[dataType.Name];

//					bool isSame = true;

//					foreach (var kv in typeDirctionary)
//					{
//						isSame = true;

//						for (var i = 0; i < recordFieldList.Count; ++i)
//						{
//							var recordKv = recordFieldList[i];

//							var value = recordKv.Key.GetValue(kv.Value.data);

//							if (!recordKv.Value.Value.Equals(value))
//							{
//								isSame = false;

//								break;
//							}
//						}

//						if (isSame)
//						{
//							return (T)kv.Value.data;
//						}
//					}
//				}
//			}

//			return null;
//		}

//		/// <summary>
//		/// 获取字段数据匹配的数据
//		/// </summary>
//		/// <param name="menberId"></param>
//		/// <returns></returns>
//		public static List<T> GetDatasWithFieldsList<T>(List<KeyValuePair<string, object>> fields) where T : RedisBase
//		{
//			List<T> resultDataList = new List<T>();

//			lock (lockTarget)
//			{
//				Type dataType = typeof(T);

//				var recordFieldList = new List<KeyValuePair<FieldInfo, KeyValuePair<string, object>>>();

//				for (var i = 0; i < fields.Count; ++i)
//				{
//					var fieldInfo = dataType.GetField(fields[i].Key);

//					recordFieldList.Add(new KeyValuePair<FieldInfo, KeyValuePair<string, object>>(fieldInfo, fields[i]));
//				}


//				if (memoryTable.ContainsKey(dataType.Name))
//				{
//					var typeDirctionary = memoryTable[dataType.Name];

//					bool isSame = true;

//					foreach (var kv in typeDirctionary)
//					{
//						isSame = true;

//						for (var i = 0; i < recordFieldList.Count; ++i)
//						{
//							var recordKv = recordFieldList[i];

//							var value = recordKv.Key.GetValue(kv.Value.data);

//							if (!recordKv.Value.Value.Equals(value))
//							{
//								isSame = false;

//								break;
//							}
//						}

//						if (isSame)
//						{
//							resultDataList.Add((T)kv.Value.data);
//						}
//					}
//				}
//			}

//			return resultDataList;
//		}

//		/// <summary>
//		/// 获取字段字符串包含的数据列表
//		/// </summary>
//		/// <param name="menberId"></param>
//		/// <returns></returns>
//		public static List<T> GetDataWithFieldsStringContainsList<T>(List<KeyValuePair<string, string>> fields) where T : RedisBase
//		{
//			List<T> resultDataList = new List<T>();

//			lock (lockTarget)
//			{
//				Type dataType = typeof(T);

//				var recordFieldList = new List<KeyValuePair<FieldInfo, KeyValuePair<string, string>>>();

//				for (var i = 0; i < fields.Count; ++i)
//				{
//					var fieldInfo = dataType.GetField(fields[i].Key);

//					recordFieldList.Add(new KeyValuePair<FieldInfo, KeyValuePair<string, string>>(fieldInfo, fields[i]));
//				}


//				if (memoryTable.ContainsKey(dataType.Name))
//				{
//					var typeDirctionary = memoryTable[dataType.Name];

//					bool isSame = true;

//					foreach (var kv in typeDirctionary)
//					{
//						isSame = true;

//						for (var i = 0; i < recordFieldList.Count; ++i)
//						{
//							var recordKv = recordFieldList[i];

//							var value = recordKv.Key.GetValue(kv.Value.data) as string;

//							if (!value.Contains(recordKv.Value.Value))
//							{
//								isSame = false;

//								break;
//							}
//						}

//						if (isSame)
//						{
//							resultDataList.Add((T)kv.Value.data);
//						}
//					}
//				}
//			}

//			return resultDataList;
//		}

//		/// <summary>
//		/// 获取字段数据匹配的数据
//		/// </summary>
//		/// <param name="menberId"></param>
//		/// <returns></returns>
//		public static int GetDataWithFieldsCount<T>(List<KeyValuePair<string, object>> fields) where T : RedisBase
//		{
//			List<T> resultDataList = new List<T>();

//			lock (lockTarget)
//			{
//				Type dataType = typeof(T);

//				var recordFieldList = new List<KeyValuePair<FieldInfo, KeyValuePair<string, object>>>();

//				for (var i = 0; i < fields.Count; ++i)
//				{
//					var fieldInfo = dataType.GetField(fields[i].Key);

//					recordFieldList.Add(new KeyValuePair<FieldInfo, KeyValuePair<string, object>>(fieldInfo, fields[i]));
//				}
				
//				if (memoryTable.ContainsKey(dataType.Name))
//				{
//					var typeDirctionary = memoryTable[dataType.Name];

//					bool isSame = true;

//					foreach (var kv in typeDirctionary)
//					{
//						isSame = true;

//						for (var i = 0; i < recordFieldList.Count; ++i)
//						{
//							var recordKv = recordFieldList[i];

//							var value = recordKv.Key.GetValue(kv.Value.data);

//							if (!recordKv.Value.Value.Equals(value))
//							{
//								isSame = false;

//								break;
//							}
//						}

//						if (isSame)
//						{
//							resultDataList.Add((T)kv.Value.data);
//						}
//					}
//				}
//			}

//			return resultDataList.Count;
//		}

//		/// <summary>
//		/// 获取数据类型数
//		/// </summary>
//		/// <returns></returns>
//		public static int GetDataTypeCount()
//		{
//			return memoryTable.Count;
//		}

//		/// <summary>
//		/// 获取指定类型数据条数
//		/// </summary>
//		/// <typeparam name="T"></typeparam>
//		/// <returns></returns>
//		public static int GetTypeDataCount<T>()
//		{
//			lock (lockTarget)
//			{
//				Type dataType = typeof(T);

//				if (memoryTable.ContainsKey(dataType.Name))
//				{
//					return memoryTable[dataType.Name].Count;
//				}
//				else
//				{
//					return 0;
//				}
//			}
//		}


//		public static object lockExecute = new object();

//		public static void DeleteAllDb()
//		{
//			for (var i = 0; i < dbList.Count; ++i)
//			{
//				var dbType = dbList[i];

//				Logger.Log($"删除数据库:{dbType.Name}");

//				string sqlCreate = $"DROP TABLE {dbType.Name}";

//				SqlHelp.ExecuteNonQuery(sqlCreate);
//			}
//		}

//		public static void LoadDb()
//		{
//			for (var i = 0; i < dbList.Count; ++i)
//			{
//				var dbType = dbList[i];

//				Logger.Log($"加载数据库:{dbType.Name}");

//				string sqlCreate = $"SELECT * FROM {dbType.Name}";

//				SelectDataToMemory(dbType, sqlCreate);
//			}
//		}

//		private static void SelectDataToMemory(Type type, string sqlCommand)
//		{
//			var fields = type.GetFields();

//			var idField = type.GetField("id");

//			var tableData = SqlHelp.ExecuteDataTable(sqlCommand);

//			if (tableData != null && tableData.Rows != null)
//			{
//				for (var i = 0; i < tableData.Rows.Count; ++i)
//				{
//					var oneDataRow = tableData.Rows[i];

//					var oneData = Activator.CreateInstance(type);

//					for (var j = 0; j < fields.Length; ++j)
//					{
//						var fieldInfo = fields[j];

//						SetObjectValue(fieldInfo, oneData, oneDataRow);
//					}

//					LoadAddData(idField.GetValue(oneData), (RedisBase)oneData, type);
//				}
//			}
//		}

//		public static void CheckTable()
//		{
//			for (var i = 0; i < dbList.Count; ++i)
//			{
//				var dbType = dbList[i];

//				string sqlCreate = $"CREATE TABLE [dbo].[{dbType.Name}]( ";

//				var fields = dbType.GetFields();

//				for (var j = 0; j < fields.Length; ++j)
//				{
//					var field = fields[j];

//					sqlCreate += $"[{field.Name}] {SqlValueTypeByType.GetSqlType(field.FieldType)} NOT NULL,";
//				}

//				sqlCreate += ")";

//				//这里是创建表
//				var sqlCheck = $"if object_id( '{dbType.Name}') is not null select 1 else select 0";

//				if ((int)SqlHelp.ExecuteScalar(sqlCheck) == 0)
//				{
//					Logger.Wring($"表{dbType.Name}不存在 准备创建");

//					if ((int)SqlHelp.ExecuteNonQuery(sqlCreate) == 0)
//					{
//						Logger.Error($"表{dbType.Name}创建失败");
//					}
//					else
//					{
//						Logger.Log($"表{dbType.Name}创建成功");
//					}
//				}
//				else
//				{
//					Logger.Log($"表{dbType.Name}已存在不做处理");
//				}
//			}
//		}

//		private static void AddExecute(DbExecuteInfo executeInfo)
//		{
//			lock (lockExecute)
//			{
//				waitExecuteList.Add(executeInfo);
//			}
//		}

//		public static void StartExecuteAsync()
//		{
//			Thread.Sleep(10);

//			ThreadPool.QueueUserWorkItem(new WaitCallback(UpExecute));
//		}
		
//		private static void UpExecute(object paramar)
//		{
//			try
//			{
//				DbExecuteInfo executeInfo = null;

//				lock (lockExecute)
//				{
//					if (waitExecuteList.Count > 0)
//					{
//						executeInfo = waitExecuteList[0];

//						waitExecuteList.RemoveAt(0);
//					}
//				}

//				if (executeInfo != null)
//				{
//					Execute(executeInfo, 0);
//				}
//				else
//				{
//					//等待进异步
//					StartExecuteAsync();
//				}
//			}
//			catch (Exception e)
//			{
//				Logger.Error("数据库出现异常情况:" + e.ToString());
//			}
//		}

//		private static void Execute(DbExecuteInfo executeInfo,int failedTimes)
//		{
//			var fieldList = executeInfo.type.GetFields();
//			var tableName = executeInfo.type.Name;
//			string sqlString = "";

//			if (executeInfo.executeType == DbExecuteType.Insert)
//			{
//				string keys = "(";
//				string values = "(";

//				for (var i = 0; i < fieldList.Length; ++i)
//				{
//					var field = fieldList[i];
//					var value = GetStringValue(executeInfo.type,field, executeInfo.data.data);

//					if (i != 0)
//					{
//						keys += ",";
//						values += ",";
//					}

//					keys += field.Name;
//					values += value;
//				}

//				keys += ")";
//				values += ")";
//				sqlString = $"INSERT INTO {tableName} {keys} VALUES{values}";

//				Logger.Log("sqlstring:" + sqlString);
//			}
//			else if (executeInfo.executeType == DbExecuteType.Update)
//			{
//				string fields = "";

//				for (var i = 0; i < fieldList.Length; ++i)
//				{
//					var field = fieldList[i];
//					var value = GetStringValue(executeInfo.type, field, executeInfo.data.data);

//					if (i != 0)
//					{
//						fields += ",";
//					}

//					fields += $"{field.Name} = {value}";					
//				}

//				string whereString = "";

//				if (executeInfo.whereField.Count > 0)
//				{
//					for (var i = 0; i < executeInfo.whereField.Count; ++i)
//					{
//						if (i != 0)
//						{
//							whereString += "AND";
//						}

//						whereString += $"{executeInfo.whereField[i]} {executeInfo.whereOperator[i]} {executeInfo.whereValue[i]}";
//					}
//				}

//				if (string.IsNullOrEmpty(whereString))
//				{
//					sqlString = $"UPDATE {tableName} SET {fields}";
//				}
//				else
//				{
//					sqlString = $"UPDATE {tableName} SET {fields} WHERE {whereString}";
//				}

//				Logger.Log("sqlstring:" + sqlString);
//			}

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
//		}

//		private static string GetStringValue(Type classType,FieldInfo field,RedisBase memoryRedisData)
//		{
//			var value = field.GetValue(memoryRedisData);

//			var resultString = GetStringValueObject(value);

//			return resultString;
//		}

//		private static bool IsGeneralType(Type type)
//		{
//			if (type == typeof(Boolean) ||
//				type == typeof(Byte) ||
//				type == typeof(SByte) ||
//				type == typeof(UInt16) ||
//				type == typeof(Int16) ||
//				type == typeof(UInt16) ||
//				type == typeof(Int32) ||
//				type == typeof(UInt32) ||
//				type == typeof(Int64) ||
//				type == typeof(UInt64) ||
//				type == typeof(Single) ||
//				type == typeof(Double) ||
//				type == typeof(Decimal))
//			{
//				return true;
//			}

//			return false;
//		}

//		private static string GetStringValueObject(object value)
//		{
//			string resultString = "";

//			var type = value.GetType();

//			if (IsGeneralType(type))
//			{
//				resultString = value.ToString();
//			}
//			else if (type == typeof(String))
//			{
//				resultString = $"'{value}'";
//			}
//			else
//			{
//				resultString = $"'{JsonConvert.SerializeObject(value)}'";
//			}

//			return resultString;
//		}

//		private static void SetObjectValue(FieldInfo field,object dataInstance,DataRow dbRow)
//		{
//			var type = field.FieldType;

//			if (type == typeof(Boolean))
//			{
//				field.SetValue(dataInstance, Boolean.Parse(dbRow[field.Name].ToString()));
//			}
//			else if (type == typeof(Byte))
//			{
//				field.SetValue(dataInstance, Byte.Parse(dbRow[field.Name].ToString()));
//			}
//			else if (type == typeof(SByte))
//			{
//				field.SetValue(dataInstance, SByte.Parse(dbRow[field.Name].ToString()));
//			}
//			else if (type == typeof(Int16))
//			{
//				field.SetValue(dataInstance, Int16.Parse(dbRow[field.Name].ToString()));
//			}
//			else if (type == typeof(UInt16))
//			{
//				field.SetValue(dataInstance, UInt16.Parse(dbRow[field.Name].ToString()));
//			}
//			else if (type == typeof(Int32))
//			{
//				field.SetValue(dataInstance, Int32.Parse(dbRow[field.Name].ToString()));
//			}
//			else if (type == typeof(UInt32))
//			{
//				field.SetValue(dataInstance, UInt32.Parse(dbRow[field.Name].ToString()));
//			}
//			else if (type == typeof(Int64))
//			{
//				field.SetValue(dataInstance, Int64.Parse(dbRow[field.Name].ToString()));
//			}
//			else if (type == typeof(UInt64))
//			{
//				field.SetValue(dataInstance, UInt64.Parse(dbRow[field.Name].ToString()));
//			}
//			else if (type == typeof(Single))
//			{
//				field.SetValue(dataInstance, Single.Parse(dbRow[field.Name].ToString()));
//			}
//			else if (type == typeof(Double))
//			{
//				field.SetValue(dataInstance, Double.Parse(dbRow[field.Name].ToString()));
//			}
//			else if (type == typeof(Decimal))
//			{
//				field.SetValue(dataInstance,Decimal.Parse(dbRow[field.Name].ToString()));
//			}
//			else if (type == typeof(String))
//			{
//				var readString = dbRow[field.Name].ToString();
//				field.SetValue(dataInstance, readString);
//			}
//			else
//			{
//				var readString = dbRow[field.Name].ToString();
//				field.SetValue(dataInstance, JsonConvert.DeserializeObject(readString, type));
//			}
//		}
//	}
//}
