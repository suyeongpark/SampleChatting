using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Suyeong.Lib.DB.MySql;
using Suyeong.Lib.Util;
using Sample.Chatting.Lib;

namespace Sample.Chatting.Server.Login
{
    public static class DataBase
    {
        static string _conStr;

        public static void Init()
        {
            _conStr = MySqlDB.GetDbConStr(server: "", database: "", uid: "", password: "");
            //_conStr = MySqlDB.GetDbConStr(dataSource: Values.DB_DATASOURCE, password: Values.DB_PASSWORD);
        }

        async public static Task<bool> IsDuplicated(string id)
        {
            string result = string.Empty;

            try
            {
                MySqlParameter[] parameters = {
                    new MySqlParameter(Parameters.ID, id),
                };

                result = await Task.Run(() => MySqlDB.GetDataSingle(conStr: _conStr, query: Queries.SELECT_ID, parameters: parameters));
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
                // 여기서 password 암호화해서 DB에 저장
                // http://suyeongpark.me/archives/2903
                // https://d2.naver.com/helloworld/318732
                MySqlParameter[] parameters = {
                    new MySqlParameter(Parameters.ID, id),
                    new MySqlParameter(Parameters.PASSWORD, password),
                };

                result = await Task.Run(() => MySqlDB.SetQuery(conStr: _conStr, query: Queries.INSERT_USER, parameters: parameters));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }

        async public static Task<bool> IsApproved(string id, string password)
        {
            string result = string.Empty;

            try
            {
                // 여기서 password 암호화해서 DB에 비교
                MySqlParameter[] parameters = {
                    new MySqlParameter(Parameters.ID, id),
                    new MySqlParameter(Parameters.PASSWORD, password),
                };

                result = await Task.Run(() => MySqlDB.GetDataSingle(conStr: _conStr, query: Queries.SELECT_ACCOUNT, parameters: parameters));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Utils.GetIntFromString(result) > 0;
        }
    }
}
