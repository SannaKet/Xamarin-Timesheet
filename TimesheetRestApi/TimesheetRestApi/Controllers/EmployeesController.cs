using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimesheetRestApi.Models;

namespace TimesheetRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        //Haetaan kaikki aktiiviset työntekijät mobiilisovelluksen Employees-sivulle

        [HttpGet]
        [Route("")]

        public string[] GetAllEmployees()


        {                
            //alustetaan tyhjä merkkijonotaulukko sekä luodaan yhteys malliin (tietokantaan)
            string[] employeeNames = null;
            ProjektitDBCareContext context = new ProjektitDBCareContext();

            try
            {
                employeeNames = (from e in context.Employees
                                 where e.Active == true
                                 select e.FirstName + " " + e.LastName).ToArray();

                return employeeNames;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                context.Dispose();
            }

        }
    }
}