using BRoles.Data;
using BRoles.Models;
using BRoles.tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.RegularExpressions;

namespace BRoles.UtilityService
{
    public class check : Icheck
    {
        private readonly OperationsDBContext _operationsDBContext;

        public check(OperationsDBContext operationsDBContext)
        {
            _operationsDBContext = operationsDBContext;
        }

        public String checkData(checkUser user, int apiNum)
        {
            Log.WriteLine("inside the check data  : ");
           

            var regexAlphanumeric = new Regex("^[a-zA-Z0-9]{1,9}$");// to make sure that it's alphanumerique                                                  
            var regexLong = new Regex(@"^[0-9,.]{1,9}$");// to make sure that it's numerique and it's length is less that 10 digits
            var regexEmail = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");//make sure that it's email

            try
            {


                switch (apiNum)
                {
            
                    case 1://if the user is coming from the login 
                        if (!regexAlphanumeric.IsMatch(user.Username))
                        {   
                            Log.WriteLine("Wrong format username !!!!!!!!!!!!!: " + user.Username);
                         
                            saveAttempt(user.Username, "login", "wrong format username");
                            return ("Wrong format username: contains unusual characters or too long");
                        }



                         if (user.Password.Length > 15)//the password's length should be less than 15 characters 
                        {
                            Log.WriteLine("Wrong format password(too long) : " + user.Password);
                            saveAttempt(user.Username, "login", "wrong format password");
                            return ("Wrong format password: too long");

                        }



                        break;

                        case 2://if the user is coming from the register 

                        if (!regexAlphanumeric.IsMatch(user.Username))
                            {  
                                Log.WriteLine("Wrong format username !!!!!!!!!!!!!: " + user.Username);
                            saveAttempt(user.Username, "register", "wrong format username");

                            return ("Wrong format username: contains unusual characters");
                            }




                            if (user.Password.Length > 15)//the password's length should be less than 15 characters 
                            {
                                Log.WriteLine("Wrong format password(too long) : " + user.Password);
                            saveAttempt(user.Username, "register", "wrong format password");
                            return ("Wrong format password: too long");
                            }




                            if (! regexLong.IsMatch(user.Salary.ToString()))//we used to string so it can be passed as a parameter since the method only accepts strings
                            {   
                                Log.WriteLine("Wrong format salary !: " + user.Username);
                            saveAttempt(user.Username, "register", "wrong format salary");
                            return ("Wrong format salary: contains unusual characters or too long");
                            }

                          

                            if (!regexAlphanumeric.IsMatch(user.Role))
                            {  
                                Log.WriteLine("Wrong format role !: " + user.Role);
                            saveAttempt(user.Username, "register", "wrong format role");
                            return ("Wrong format Role: contains unusual characters or loo long");
                            }



                            if(user.Token!="")
                            {
                                Log.WriteLine("Wrong format token !: " + user.Token);
                            saveAttempt(user.Username, "register", "wrong format token");
                            return ("Wrong format token: should be empty string");
                            }


                            if (!regexEmail.IsMatch(user.Email))
                            {  
                               Log.WriteLine("Wrong format Email !: " + user.Email);
                            saveAttempt(user.Username, "register", "wrong format email");
                            return ("Wrong format Email");
                            }



                        break;



















                }
            }
            catch(Exception ex) {
                Log.WriteLine("EXECPTION RAISED : " + ex);
            }

           // this.cause = "";
           // saveAttempt(user.Username);

            return "ok";
        }





        public void saveAttempt(String username, string concernedApi, string cause)
        {
            try
            {
                var userOp = new Operations(username, cause, concernedApi);
                _operationsDBContext.Add(userOp);
                _operationsDBContext.SaveChanges();
            }


            catch (Exception ex)
            {
                Log.WriteLine("EXECPTION RAISED WHILE ADDING THE OPERATIONS  : " + ex);

            }
        }













    }

   
}
