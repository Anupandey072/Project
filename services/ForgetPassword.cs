
using MySql.Data.MySqlClient;

namespace Project.services
{
    public class ForgetPassword
    {
        dbServices ds = new dbServices();
        public async Task<responseData> forgetPassword(requestData req)
        {
            responseData resData= new responseData();
            resData.rData["rCode"]=0;
            try
            {
              var result = new List<Dictionary<string, object>>();
                MySqlParameter[] myParams = new MySqlParameter[] {
                new MySqlParameter("@ID", req.addInfo["ID"]),
                };
                var sq = $"SELECT * FROM User_Register where ID=@ID";
                var data = ds.ExecuteSQLName(sq, myParams);
                
                if (data==null || data[0].Count()==0)
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Invalid Credentials";
                }
                else
                {
                  if(data !=null)
                  {
                    var list = new List<Dictionary<string, object>>();
                    for (var i = 0; i < data.Count(); i++)
                    {
                      foreach (var row in data[i])
                        {
                            Dictionary<string, object> myDict = new Dictionary<string, object>();

                            foreach (var field in row.Keys)
                            {
                                myDict[field] = row[field].ToString();
                            }

                            result.Add(myDict);
                        } 
                        
                    }
                    resData.rData["rMessage"] = "Dashboard ";
                  }
                  else 
                  {
                    resData.rData["rMessage"] = "Invalid User Details";
                  }
                  resData.rData["rCode"]=1;
                  resData.rData["rData"]=result;
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"]=1;
                resData.rData["rMessage"]=ex.Message;
            }
            return resData;
        }


       
          
    }
}