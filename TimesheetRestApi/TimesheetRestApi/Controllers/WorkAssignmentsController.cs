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
    public class WorkAssignmentsController : ControllerBase
    {

        // -------------- HTTP-GET: HAETAAN KAIKKI AKTIIVISET TYÖTEHTÄVÄT LISTAAN ------------------ //
        [HttpGet]
        [Route("")]

        public string[] GetAllWorkAssignments()
        {
            //alustetaan tyhjä merkkijonotaulukko sekä luodaan yhteys malliin (tietokantaan)
            string[] workAssignmentsNames = null;

            ProjektitDBCareContext context = new ProjektitDBCareContext();

            try
            {
                workAssignmentsNames = (from wa in context.WorkAssignments
                                        where wa.Active == true &&
                                        wa.Completed == false
                                        select wa.Title).ToArray();

                return workAssignmentsNames;
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


        // ---- HTTP-POST: TYÖN ALOITUS JA LOPETUS PYYNTÖJEN HALLINTA --- Boolean frontiin, tietokantaan muutoksia --------//
        [HttpPost]
        [Route("")]

        public bool PostStatus(WorkAssignmentOperationModel input)
        {
            ProjektitDBCareContext context = new ProjektitDBCareContext();

            try
            {
                // Haetaan aktiiviset työtehtävät nimen perusteella ja sijoitetaan se oneAssignment-muuttujaan
                WorkAssignments oneAssignment = (from wa in context.WorkAssignments
                                                 where wa.Active == true &&
                                                 wa.Title == input.AssignmentTitle //Otsikko vastaa sitä, mitä mobiilisovellus on lähettänyt
                                                 select wa).FirstOrDefault();



                //Hajautetaan valitun työntekijän nimi kahteen muuttujaan etu- ja sukunimeksi
                //(Lisättiin WorkAssingmentOperationModel.cs:ään prop Name)
                string[] nameParts = input.Name.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                string fname = nameParts[0];
                string lname = nameParts[1];


                //Luodaan Employees tyyppinen olio (employee) edellisen nimen jaon perusteella
                Employees employee = (from e in context.Employees
                                      where e.Active == true &&
                                      e.FirstName == fname &&
                                      e.LastName == lname
                                      select e).FirstOrDefault();


                if (oneAssignment == null)
                {
                    return false;
                }


                // ---------- Jos painettu "ALOITA TYÖ" -------------//
                else if (input.Operation == "Start")
                {


                    if (oneAssignment.InProgress == true || oneAssignment.Completed == true)
                    {
                        return false;
                    }
                    else
                    {
                        int assignmentId = oneAssignment.IdWorkAssignment;
                        int employeeId = employee.IdEmployee;
                        string oneComment = input.Comment;
                        string latitude = input.Latitude;
                        string longitude = input.Longitude;

                        //Luodaan työlle uusi TimeSheet
                        Timesheet newEntry = new Timesheet()
                        {
                            IdWorkAssignment = assignmentId,
                            IdEmployee = employeeId,
                            StartTime = DateTime.Now,
                            Active = true,
                            Comments = oneComment + " - " + fname,
                            Longitude = longitude,
                            Latitude = latitude,
                            CreatedAt = DateTime.Now,
                            LastModifiedAt = DateTime.Now
                        };

                        context.Timesheet.Add(newEntry);

                        //Päivitetään samalla myös WorkAssignments-taulua
                        oneAssignment.InProgress = true;
                        oneAssignment.WorkStartedAt = DateTime.Now;
                        oneAssignment.LastModifiedAt = DateTime.Now;
                    }


                    //Tallennetaan kaikki em. muutokset tietokantaan
                    context.SaveChanges();

                }

                // ---------- Jos painettu "LOPETA TYÖ" -------------//
                else if (input.Operation == "Stop")
                {

                    string oneFinalComment = input.Comment;

                    int assignmentId = oneAssignment.IdWorkAssignment;

                    //Haetaan tiedot Timesheet ja WorkAssignments-tauluista ja..... 
                    Timesheet timesheet = (from ts in context.Timesheet
                                           where ts.IdWorkAssignment == assignmentId &&
                                           ts.Active == true
                                           select ts).FirstOrDefault();


                    WorkAssignments assignments = (from wa in context.WorkAssignments
                                                   where wa.IdWorkAssignment == assignmentId &&
                                                   wa.Active == true
                                                   select wa).FirstOrDefault();


                    //....muutetaan niiden tietoja
                    if (timesheet != null && assignments != null)
                    {

                        if (assignments.InProgress == false || assignments.Completed == true)
                        {
                            return false;
                        }

                        else
                        {
                            //Timesheet-taulun uudet tiedot
                            timesheet.StopTime = DateTime.Now;
                            timesheet.LastModifiedAt = DateTime.Now;
                            timesheet.Comments = oneFinalComment + " - " + fname;


                            //WorkAssignments-taulun uudet tiedot
                            assignments.Completed = true;
                            assignments.CompletedAt = DateTime.Now;
                            assignments.InProgress = false;
                            assignments.LastModifiedAt = DateTime.Now;

                        }
                    }
                    else
                    {
                        return false;  //Jos id-tieto on null jommassa kummassa
                    }

                }

                context.SaveChanges();
            }

            catch (Exception)
            {
                return false;  //jos jotain meni pieleen
            }

            finally
            {
                context.Dispose();
            }

            return true;  //Mobiilisovellukselle palautetaan true, kun kaikki onnistui

        }

    }
}