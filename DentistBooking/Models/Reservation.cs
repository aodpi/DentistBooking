using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DentistBooking.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [StringLength(150), Display(Name = "Your name, surname:")]
        public string FullName { get; set; }

        [Phone, Required, Display(Name = "Your Phone Number:")]
        public string Phone { get; set; }

        [EmailAddress, Required, Display(Name = "Your Email:")]
        public string Email { get; set; }

        [DataType(DataType.DateTime), Display(Name = "Check doctor availability:")]
        public DateTime ReservationTime { get; set; }

        [StringLength(250)]
        public string Comments { get; set; }

        [Required, Display(Name = "Choose a doctor:")]
        public int MedicId { get; set; }
        public virtual Medic Medic { get; set; }

        [Required, Display(Name = "Choose a procedure:")]
        public int ProcedureId { get; set; }
        public virtual Procedure Procedure { get; set; }
        
    }
}
