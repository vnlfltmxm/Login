using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using MySql.Data.MySqlClient;
using Unity.VisualScripting;

public class DBController : MonoBehaviour
{
    public static MySqlConnection sqlConn;

    static string ipAddress = "127.0.0.1";
    static string db_ID = "newuser";
    static string db_Passward = "1234";
    static string db_TableName = "study_datatable";

    string strConn = string.Format($"server={ipAddress};port=3306;uid={db_ID};pwd={db_Passward};database={db_TableName};charset=utf8 ;");

    private void Awake()
    {
        try
        {
            sqlConn = new MySqlConnection(strConn);
            Debug.Log("awake success");
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            throw;
        }
    }

    private void Start()
    {
        string query = "select * from study";
        DataSet ds = OnSelectRequest(query, "study");
        Debug.Log(ds.GetXml());
    }

    //select 조회 쿼리시 사용
    public static DataSet OnSelectRequest(string query, string tableName)
    {
        try
        {
            if (sqlConn == null)
            {
                Debug.LogError("sqlConn is null.");
                return null;
            }

            if (sqlConn.State == ConnectionState.Closed)
            {
                sqlConn.Open();
            }

            MySqlCommand cmd= new MySqlCommand();
            cmd.Connection = sqlConn;
            cmd.CommandText = query;

            MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sd.Fill(ds, tableName);

            sqlConn.Close();

            return ds;

        }
        catch (MySqlException ex)
        {
            Debug.LogError("MySQL Exception: " + ex.Message);
            Debug.LogError("Error Number: " + ex.Number);
            Debug.LogError("StackTrace: " + ex.StackTrace);
            throw;  // 예외를 다시 던져 호출한 곳에서 처리하도록 함
        }
        catch (System.Exception e)
        {
            Debug.LogError("General Exception: " + e.Message);
            Debug.LogError("StackTrace: " + e.StackTrace);
            return null;
        }
    }

}
