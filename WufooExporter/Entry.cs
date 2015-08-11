using System;
using Newtonsoft.Json;

namespace WufooExporter
{
    public class Entry
    {
        public int EntryId { get; set; }

        [JsonProperty("Field3")]
        public String Name { get; set; }

        [JsonProperty("Field6")]
        public String FathersName { get; set; }

        [JsonProperty("Field7")]
        public String MothersName { get; set; }

        [JsonProperty("Field5")]
        public String DateOfBirth { get; set; }

        [JsonProperty("Field9")]
        public String Email { get; set; }

        [JsonProperty("Field10")]
        public String Phone { get; set; }

        [JsonProperty("Field12")]
        public String AddressLine1 { get; set; }

        [JsonProperty("Field13")]
        public String AddressLine2 { get; set; }

        [JsonProperty("Field14")]
        public String City{ get; set; }

        [JsonProperty("Field15")]
        public String State { get; set; }

        [JsonProperty("Field16")]
        public String ZipCode { get; set; }

        [JsonProperty("Field17")]
        public String Country { get; set; }

        [JsonProperty("Field22")]
        public String NagaratharVillage { get; set; }

        [JsonProperty("Field27")]
        public String Degree { get; set; }

        [JsonProperty("Field28")]
        public String Specialization { get; set; }

        [JsonProperty("Field29")]
        public String YearOfStudy { get; set; }

        [JsonProperty("Field32")]
        public String College { get; set; }

        [JsonProperty("Field34")]
        public String CollegeAddressLine1 { get; set; }

        [JsonProperty("Field35")]
        public String CollegeAddressLine2 { get; set; }

        [JsonProperty("Field36")]
        public String CollegeCity { get; set; }

        [JsonProperty("Field37")]
        public String CollegeState { get; set; }

        [JsonProperty("Field38")]
        public String CollegeZipCode { get; set; }

        [JsonProperty("Field39")]
        public String CollegeCountry { get; set; }

        [JsonProperty("Field43")]
        public String AnnualFamilyIncome { get; set; }

        [JsonProperty("Field46")]
        public String ExtraCurricularActivities { get; set; }

        [JsonProperty("Field48")]
        public String AdditionalComments { get; set; }

        [JsonProperty("Field50")]
        public String LetterOfAttestationLink { get; set; }

        [JsonProperty("Field52")]
        public String AdditionalDocument1 { get; set; }

        [JsonProperty("Field53")]
        public String AdditionalDocument2 { get; set; }

        [JsonProperty("Field54")]
        public String AdditionalDocument3 { get; set; }

        [JsonProperty("Field55")]
        public String AdditionalDocument4 { get; set; }

        public DateTime DateCreated { get; set; }
        public string DateUpdated { get; set; }
        public String CreatedBy { get; set; }
        public String UpdatedBy { get; set; }
        public int CompleteSubmission { get; set; }
        [JsonProperty("LastPage")]
        public int CompletedPages { get; set; }


    }
}