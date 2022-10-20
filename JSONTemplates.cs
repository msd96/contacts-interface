namespace ContactsInterface
{
    public class create_contact
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? BirthDate { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public static readonly create_contact Empty = new create_contact();
        public Boolean IsEmpty()
        {
            return Empty.Equals(this);
        }
    }
    public class output_with_message_and_errorcode
    {
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }

        public static readonly create_contact Empty = new create_contact();
        public Boolean IsEmpty()
        {
            return Empty.Equals(this);
        }
    }
    public class read_contacts_output_with_message_and_errorcode
    {
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }
        public List<Dictionary<string, string>>? Data { get; set; }

        public static readonly create_contact Empty = new create_contact();
        public Boolean IsEmpty()
        {
            return Empty.Equals(this);
        }
    }
}
