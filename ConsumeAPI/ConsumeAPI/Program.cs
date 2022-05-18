using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;

namespace ConsumeAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
        hapa:
            Console.WriteLine("Please Choose Service");
            Console.WriteLine("1. FingerPrint");
            Console.WriteLine("2. Token"); 
            Console.WriteLine("3. Get All Employees"); 
            Console.WriteLine("4. Get Logs"); 
            int selection;
            int.TryParse(Console.ReadLine(),out selection);
            if (selection==1)
            {
                Console.WriteLine("Please enter ID number");
                string ID = Console.ReadLine();
                Console.WriteLine("------Please wait--------");
                PostData(int.Parse(ID));
            }else if (selection == 2)
            {
                RequestToken();
            }
            else if (selection == 3)
            {
                GetEmployees();
            }
            else if (selection == 4)
            {
                GetLogs();
            }
            else
            {
                Console.WriteLine("------Invalid selection--------");
            }
            Console.WriteLine();
            Console.WriteLine("------Any other request(y/n)--------");
            string rqID = Console.ReadLine();
            if (rqID == "y" || rqID == "Y")
            {
                goto hapa;
            }
            else
            {
                return;
            }
            
        }
        static string API = "http://154.118.230.227:8095/bas-api/";
        static string deviceID = "868646030145487";
        static string employeeId = "37";
        static string constring = @"Data Source=192.168.2.245;Initial Catalog=DemoPoc;User id = frank;password=12345";
        static string rqToken ;

        public static void PostData(int UId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(API+"finger-print");
                    
                    DataModel dataModel = new DataModel();
                    PostDataModel postdataModel = new PostDataModel();                
                    
                    using (SqlConnection con = new SqlConnection(constring))
                    {
                        string oString = "Select * from [dbo].[FingerPrint] where UserID=@UId";
                        using (SqlCommand cmd = new SqlCommand(oString, con))
                        {
                            //cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@UId", UId);
                            con.Open();
                            using (SqlDataReader oReader = cmd.ExecuteReader())
                            {
                                if (oReader.HasRows)
                                {
                                    while (oReader.Read())
                                    {
                                        dataModel.UserID = int.Parse(oReader["UserID"].ToString());
                                        dataModel.deviceID = oReader["deviceID"].ToString();
                                        dataModel.base64WSQ = oReader["base64WSQ"].ToString();
                                        dataModel.devicefinger = oReader["devicefinger"].ToString();
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("------No Match found for this UserID--------");
                                    return;
                                }

                                con.Close();
                            }
                        }
                    }

                    #region console
                    Console.WriteLine("Data From Database");
                    Console.WriteLine("UserID --> " + dataModel.UserID);
                    Console.WriteLine("deviceID --> " + dataModel.deviceID);
                    Console.WriteLine("base64WSQ --> " + dataModel.base64WSQ);
                    Console.WriteLine("devicefinger --> " + dataModel.devicefinger);

                    Console.WriteLine("------Please wait--------");
                    Console.WriteLine("-----------------------------------");

                    
                    postdataModel.serialNumber = deviceID;
                    postdataModel.checkNumber = "";
                    postdataModel.employeeId = employeeId;
                    postdataModel.fingerPrint = dataModel.base64WSQ;
                    postdataModel.deviceFinger = dataModel.devicefinger;
                    postdataModel.facePrint = "";
                    postdataModel.deviceId = deviceID;

                    Thread.Sleep(3000);

                    Console.WriteLine("Data To BUS");
                    Console.WriteLine("serialNumber --> " + postdataModel.serialNumber);
                    Console.WriteLine("checkNumber --> " + postdataModel.checkNumber);
                    Console.WriteLine("employeeId --> " + postdataModel.employeeId);
                    Console.WriteLine("fingerPrint --> " + postdataModel.fingerPrint);
                    Console.WriteLine("deviceFinger --> " + postdataModel.deviceFinger);
                    Console.WriteLine("facePrint --> " + postdataModel.facePrint);
                    Console.WriteLine("deviceId --> " + postdataModel.deviceId);

                    Console.WriteLine("------Please wait--------");
                    var postTask = client.PostAsJsonAsync<PostDataModel>("finger-print", postdataModel);
                    postTask.Wait();
                    Console.WriteLine("-----------------------------------");
                    #endregion
                    
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        Console.WriteLine(result);
                    }
                    else
                    {
                        Console.WriteLine(result);
                    }
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void RequestToken()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(API+"get-token");

                    PostDataTokenModel Model = new PostDataTokenModel();
                    Model.serialNumber = deviceID;
                    Model.voteCode = "BAS-ATT-DATA";
                    Console.WriteLine("------Please wait--------");
                    Console.WriteLine("-----------------------------------");
                    var postTask = client.PostAsJsonAsync<PostDataTokenModel>("get-token", Model);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        //Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                        ResponseTokenModel obj = JsonSerializer.Deserialize<ResponseTokenModel>(result.Content.ReadAsStringAsync().Result);
                        Console.WriteLine("------Token Response From Api--------");
                        Console.WriteLine("Token --> " + obj.data.token);
                        Console.WriteLine("ExpireDate --> " + obj.data.expireDate);
                        Console.WriteLine("Code --> " + obj.code);
                        Console.WriteLine("Status --> " + obj.status);
                        rqToken = obj.data.token;
                    }
                    else
                    {
                        Console.WriteLine(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static List<ResponseEmployeedataModel> GetEmployees()
        {
            RequestToken();
            using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("BAS-TOKEN", rqToken);
                    client.BaseAddress = new Uri(API + "employee");

                    PostDataEmployeeModel Model = new PostDataEmployeeModel();
                    Model.serialNumber = deviceID;
                    Console.WriteLine("------Please wait--------");
                    Console.WriteLine("-----------------------------------");
                    var postTask = client.PostAsJsonAsync<PostDataEmployeeModel>("employee", Model);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                       // Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                        Console.WriteLine("------Employees Response From Api--------");
                        ResponseEmployeeModel obj = JsonSerializer.Deserialize<ResponseEmployeeModel>(result.Content.ReadAsStringAsync().Result);
                        Console.WriteLine("Code --> " + obj.code);
                        Console.WriteLine("Status --> " + obj.status);
                        Console.WriteLine("------Data--------");
                        Console.WriteLine();
                        foreach (ResponseEmployeedataModel item in obj.data) {
                            Console.WriteLine("UserID --> " + item.id);                        
                            Console.WriteLine("UserName --> " + String.Concat(item.firstName,item.lastName));
                            Console.WriteLine("isActive --> " + item.isActive);
                            Console.WriteLine("isAdmin --> " + item.isAdmin);
                            Console.WriteLine("userLevel --> " + item.userLevel);
                            Console.WriteLine("verificationStatus --> " + item.verificationStatus);
                            Console.WriteLine("fingerPrint --> " + item.fingerPrint);
                            Console.WriteLine("deviceFinger --> " + item.deviceFinger);
                            Console.WriteLine();
                            Thread.Sleep(10);
                            DBUserID(item.id.ToString());
                            DBUser(item.id.ToString(), item.firstName+" "+item.lastName);
                            DBUserFinger(item.id.ToString(), Getbytefrombase64(item.deviceFinger));
                        }
                    return obj.data;
                    }
                    else
                    {
                          List<ResponseEmployeedataModel> data=new List<ResponseEmployeedataModel>();
                        Console.WriteLine(result);
                    return data;
                    }
                }
        }

        public static void GetLogs()
        {
            try
            {
                RequestToken();
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("BAS-TOKEN", rqToken);
                    client.BaseAddress = new Uri(API + "attendance");

                    PostLogsModel Modeltoken = new PostLogsModel();
                    List<AttendanceData> data = new List<AttendanceData>();
                    AttendanceData dmodel=new AttendanceData();

                    using (SqlConnection con = new SqlConnection(constring))
                    {
                        string oString = "SELECT [id],[dateTime],[deviceID],[code],[msg],[msgCode],[UserID] FROM [DemoPoc].[dbo].[log] where msgCode = 'IDENTIFY_SUCCESS_FINGER' and cast([dateTime] as date)= cast(getdate() as date)";
                        using (SqlCommand cmd = new SqlCommand(oString, con))
                        {
                            //cmd.CommandType = CommandType.StoredProcedure;
                            con.Open();
                            using (SqlDataReader oReader = cmd.ExecuteReader())
                            {
                                if (oReader.HasRows)
                                {
                                    while (oReader.Read())
                                    {
                                        DateTime db = DateTime.Parse(oReader["dateTime"].ToString());
                                        dmodel.attId = oReader["id"].ToString();
                                        dmodel.checkNumber = oReader["id"].ToString();
                                       // dmodel.status = oReader["msgCode"].ToString();
                                        dmodel.status = "checkin";
                                        dmodel.attendanceTime = db.ToShortTimeString();
                                        dmodel.attendanceDate = db.ToString("yyyy-MM-dd");
                                        dmodel.longitude = "24.897878";
                                        dmodel.latitude = "456.897878";
                                        data.Add(dmodel);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("------No Match found--------");
                                    return;
                                }

                                con.Close();
                            }
                        }
                    }
                    Modeltoken.serialNumber = deviceID;
                    Modeltoken.attendanceData = data;

                    var postTask = client.PostAsJsonAsync<PostLogsModel>("attendance", Modeltoken);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void DBUserID(string ID)
        {
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                SqlCommand sql_cmnd = new SqlCommand("adddbuser", con);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@ID", SqlDbType.NVarChar).Value = ID;
                sql_cmnd.ExecuteNonQuery();
                con.Close();
            }
        }
        public static byte[] Getbytefrombase64(String byteImage)
        {
            byte[] byteString = Convert.FromBase64String(byteImage);
            return byteString;
        }

        public static void DBUser(string ID,string Username)
        {
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                SqlCommand sql_cmnd = new SqlCommand("spBS2UserName", con);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@userID", SqlDbType.NVarChar).Value = ID;
                sql_cmnd.Parameters.AddWithValue("@userName", SqlDbType.NVarChar).Value = Username;
                sql_cmnd.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void DBUserFinger(string ID, byte[] data)
        {
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                SqlCommand sql_cmnd = new SqlCommand("spBS2Fingerprint", con);
                sql_cmnd.CommandType = CommandType.StoredProcedure;
                sql_cmnd.Parameters.AddWithValue("@userID", SqlDbType.NVarChar).Value = ID;
                sql_cmnd.Parameters.AddWithValue("@data", SqlDbType.VarBinary).Value = data;
                sql_cmnd.ExecuteNonQuery();
                con.Close();
            }
        }


        public class DataModel
        {
            public int UserID { get; set; }
            public byte[] fingerIndex { get; set; }
            public byte[] flag { get; set; }
            public int templateFormat { get; set; }
            public string base64WSQ { get; set; }
            public string base64Image { get; set; }
            public string devicefinger { get; set; }
            public string deviceID { get; set; }
        }

        public class PostDataModel
        {
            public string serialNumber { get; set; }
            public string checkNumber { get; set; }
            public string employeeId { get; set; }
            public string fingerPrint { get; set; }
            public string deviceFinger { get; set; }
            public string facePrint { get; set; }
            public string deviceId { get; set; }
        }

        public class PostDataTokenModel
        {
            public string serialNumber { get; set; }
            public string voteCode { get; set; }
        }

        public class PostDataEmployeeModel
        {
            public string serialNumber { get; set; }
        }

        public class ResponseTokenModel
        {
            public string status { get; set; }
            public string code { get; set; }
            public ResponseTokendataModel data { get; set; }
        }

        public class ResponseTokendataModel
        {
            public string token { get; set; }
            public string expireDate { get; set; }
        }

        public class ResponseEmployeeModel
        {
            public string status { get; set; }
            public string code { get; set; }
            public List<ResponseEmployeedataModel> data { get; set; }
        }

        public class PostLogsModel
        {
            public string serialNumber { get; set; }
            public List<AttendanceData> attendanceData { get; set; }
        }

        public class AttendanceData
        {
            public string attId { get; set; }
            public string checkNumber { get; set; }
            public string status { get; set; }
            public string attendanceTime { get; set; }
            public string attendanceDate { get; set; }
            public string longitude { get; set; }
            public string latitude { get; set; }
        }

        public class ResponseEmployeedataModel
        {
            public int id { get; set; }
            public string firstName { get; set; }
            public string middleName { get; set; }
            public string lastName { get; set; }
            public string fingerPrint { get; set; }
            public string facePrint { get; set; }
            public string deviceFinger { get; set; }
            public string userLevel { get; set; }
            public string verificationStatus { get; set; }
            public bool isAdmin { get; set; }
            public bool isActive { get; set; }
        }
    }
}
