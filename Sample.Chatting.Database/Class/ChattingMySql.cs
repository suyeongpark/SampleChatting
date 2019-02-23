using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Suyeong.Lib.DB.MySql;
using Suyeong.Lib.Util;

namespace Sample.Chatting.Database
{
    public class ChattingMySql : IChattingDB
    {
        string conStr;

        public void Init()
        {
            this.conStr = MySqlDB.GetDbConStr(server: Variables.MYSQL_SERVER, database: Variables.MYSQL_DATA_BASE, uid: Variables.MYSQL_UID, password: Variables.MYSQL_PASSWORD);
        }

        async public Task<bool> IsDuplicatedAsync(string id)
        {
            bool result = false;

            try
            {
                MySqlParameter[] parameters = {
                    new MySqlParameter(Parameters.ID, id),
                };

                object value = await MySqlDB.GetDataSingleAsync(conStr: this.conStr, query: Queries.SELECT_ID, parameters: parameters);
                result = Utils.GetIntFromString(value.ToString()) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }

        async public Task<bool> CreateUserAsync(string id, string password)
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

                result = await MySqlDB.SetQueryAsync(conStr: this.conStr, query: Queries.INSERT_USER, parameters: parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }

        async public Task<bool> IsApprovedAsync(string id, string password)
        {
            bool result = false;

            try
            {
                // 여기서 password 암호화해서 DB에 비교
                MySqlParameter[] parameters = {
                    new MySqlParameter(Parameters.ID, id),
                    new MySqlParameter(Parameters.PASSWORD, password),
                };

                object value = await MySqlDB.GetDataSingleAsync(conStr: this.conStr, query: Queries.SELECT_ACCOUNT, parameters: parameters);
                result = Utils.GetIntFromString(value.ToString()) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }
    }
}
