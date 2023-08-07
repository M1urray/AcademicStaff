using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.ViewModel
{
    public class TransportReqList
    {
        public string No { get; set; }
        public string Commencement { get; set; }
        public string Destination { get; set; }
        public string VehicleHired { get; set; }
        public string Vehicle { get; set; }
        public string DriverHired { get; set; }
        public string Driver { get; set; }
        public string Cost { get; set; }
        public string DateRequested { get; set; }
        public string DateOfTrip { get; set; }
        public string respC { get; set; }
        public string NoOfPassngers { get; set; }
        public string NoOfDays { get; set; }
        public string Dep_Time { get; set; }
        public string Purpose_Of_Trip { get; set; }
        public string Status { get; set; }
        public bool TRMgr { get; set; }
    }
    public class NewTransportRequisition
    {
        public string RespC { get; set; }
        public List<SelectListItem> ListOfResponsibility { get; set; }
    }
    public class NewTransportDocument
    {
        public string Commencement { get; set; }
        public string Destination { get; set; }
        public string RespC { get; set; }
        public string DateTrip { get; set; }
        public string TimeTrip { get; set; }
        public string NoOfDays { get; set; }
        public string NoOfPassengers { get; set; }        
        public string Purpose { get; set; }
    }
    public class Passengers
    {
        public string Type { get; set; }
        public string No { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public List<SelectListItem> ListOfEmployee { get; set; }
    }
    public class PassengerList
    {
        public string Status { get; set; }
        public List<Passengers> ListOfPassengers { get; set; }
    }
    public class TransDocument
    {
        public TransportReqList DocHeader { get; set; }
        public List<Passengers> ListOfPassengers { get; set; }
        public List<SelectListItem> ListOfDrivers { get; set; }
        public List<SelectListItem> ListOfVehicles { get; set; }
    }
}