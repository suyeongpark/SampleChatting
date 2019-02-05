using System;
using System.Data;
using System.Threading.Tasks;
using System.Data.SQLite;
using Suyeong.Lib.DB.Sqlite;
using Suyeong.Lib.Util;
using SampleChatting.Lib;

namespace SampleChatting.Server.Login
{
    public static class DataBase
    {
        static string _conStr;

        public static void Init()
        {
            _conStr = SqliteDB.GetDbConStr(dataSource: Values.DB_DATASOURCE, password: Values.DB_PASSWORD);
        }

        async public static Task<bool> IsDuplicateID(string id)
        {
            string result = string.Empty;

            try
            {
                SQLiteParameter[] parameters = {
                    new SQLiteParameter("@ID", id),
                };

                result = await SqliteDB.GetDataSingleAsync(conStr: _conStr, query: Query.SELECT_ID, parameters: parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Utils.GetIntFromString(result) > 0;
        }

        async public static Task<bool> CreateUser(string id, string password)
        {
            bool result = false;

            try
            {
                SQLiteParameter[] parameters = {
                    new SQLiteParameter("@ID", id),
                    new SQLiteParameter("@Password", password),
                };

                result = await SqliteDB.SetQueryAsync(conStr: _conStr, query: Query.SELECT_ACCESS, parameters: parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }

        async public static Task<DataTable> GetAccess(string id, string password)
        {
            DataTable table = new DataTable();

            try
            {
                SQLiteParameter[] parameters = {
                    new SQLiteParameter("@ID", id),
                    new SQLiteParameter("@Password", password),
                };

                table = await SqliteDB.GetDataTableAsync(conStr: _conStr, query: Query.SELECT_ACCESS, parameters: parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return table;
        }
    }
}
