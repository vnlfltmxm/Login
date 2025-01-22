using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using MySql.Data.MySqlClient;
using Unity.VisualScripting;
using UnityEditor.Search;
using System.Security.Cryptography;
using System;
using System.Text;

public class DBController : Singleton<DBController>
{
    public MySqlConnection sqlConn;

    string ipAddress = "127.0.0.1";
    string db_ID = "root";
    string db_Passward = "1234";
    string db_TableName = "study_database";

    string strConn; 

    private void Awake()
    {
        strConn = string.Format($"server={ipAddress};port=3306;uid={db_ID};pwd={db_Passward};database={db_TableName};charset=utf8mb4;");

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
    public DataSet OnSelectRequest(string query, string tableName)
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

            MySqlCommand cmd= new MySqlCommand(query,sqlConn);

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

    public void Login(string id, string passward)
    {
        //string cleanedID = id.Trim().Replace("\u200B", "");
        //string cleanedPassward = passward.Trim().Replace("\u200B", "");
        if (!CheckID(id))
        {
            Debug.Log("아이디 없음");
            return;
        }
        else
        {
            Debug.Log("아이디 있음");
            if(CheckPassward(id, passward))
            {
                Debug.Log("로그인 성공");
            }
        }


    }
    private bool CheckID(string id)
    {
        //string cleanedID = id.Trim().Replace("\u200B", ""); // Zero-Width Space 제거
        try
        {
            if (sqlConn.State == ConnectionState.Closed)
            {
                sqlConn.Open();
                Debug.Log("Connected Database: " + sqlConn.Database);
            }

            string query = "SELECT COUNT(*) FROM study WHERE ID = @id";

            MySqlCommand cmd = new MySqlCommand(query,sqlConn);

            cmd.Parameters.AddWithValue("@id", id);

            int matchCount=Convert.ToInt32(cmd.ExecuteScalar());

            if (matchCount > 0)
            {
                return true;
            }
            else
            {
                Debug.Log("fail");
                return false;
            }
        
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
        finally
        {
            if (sqlConn != null && sqlConn.State == System.Data.ConnectionState.Open)
            {
                sqlConn.Close();
            }
        }
        
    }

    private bool CheckPassward(string id, string passward)
    {
        try
        {
            if (sqlConn.State == ConnectionState.Closed)
            {
                sqlConn.Open();
            }

            string qurey = "SELECT Passward FROM study where ID = @id";
            MySqlCommand cmd = new MySqlCommand(qurey, sqlConn);
            cmd.Parameters.AddWithValue("@id", id);

            string hashedPassword = HashSHA256(passward);
            string dbPassward = cmd.ExecuteScalar().ToString();

            if (dbPassward != hashedPassword)
            {
                Debug.Log("비번 틀림");
                return false;
            }
            else
            {
                Debug.Log("비번 맞음");
                return true;
            }

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
        finally
        {
            if(sqlConn !=null&&sqlConn.State == ConnectionState.Open)
            {
                sqlConn.Close();
            }
        }
    }

    public void SignIn(string id, string password)
    {
        if (!CheckID(id))
        {
            try
            {
                if(sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                string hashedPassword = HashSHA256(password); // 비밀번호 해시화
                string qurey = "INSERT INTO study (ID , Passward) VALUES(@id , @passward)";
                MySqlCommand cmd = new MySqlCommand(qurey, sqlConn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@passward", hashedPassword);

                int rowsAffected = cmd.ExecuteNonQuery();//쿼리를 실행하는 메서드 없으면 실행이 안됨

                if (rowsAffected > 0)
                {
                    Debug.Log("회원가입 완료");
                }
                else
                {
                    Debug.Log("값 추가가 안됨");
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return;
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close();
                }
            }
        }
        else
        {
            Debug.Log("아이디 있음"); 
        }
        
    }

    public static string HashSHA256(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input)); // 입력 문자열을 UTF-8 바이트 배열로 변환 후 해시
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2")); // 바이트 값을 16진수 문자열로 변환
            }
            return builder.ToString(); // 최종 해시 문자열 반환
        }
    }
}
