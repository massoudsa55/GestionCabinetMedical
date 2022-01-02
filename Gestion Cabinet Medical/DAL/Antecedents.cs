namespace Gestion_Cabinet_Medical.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Antecedents
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID_Consultation { get; set; }

        public string Anti_Medicaux { get; set; }

        public string Anti_Chirurgicaux { get; set; }

        public string Anti_Familiales { get; set; }

        public string Autres_Anti { get; set; }

        public virtual Consultations Consultations { get; set; }
    }
}
