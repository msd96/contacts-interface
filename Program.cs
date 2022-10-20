using System.Text;
using System.Text.Json;
using System.Data;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
string connection = builder.Configuration.GetConnectionString("myconnstr");

app.MapPost("/contacts", async (HttpContext context) =>
{
    string jsonstring = String.Empty;
    string? FirstName = String.Empty;
    string? LastName = String.Empty;
    string? BirthDate = String.Empty;
    string? Email = String.Empty;
    string? Phone = String.Empty;
    string ErrorCode = String.Empty;
    string Message = String.Empty;
    try
    {
        using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
        {
            jsonstring = await reader.ReadToEndAsync();
            if (jsonstring.Length > 0)
            {
                ContactsInterface.create_contact? myDeserializedClass = new ContactsInterface.create_contact();
                myDeserializedClass = JsonSerializer.Deserialize<ContactsInterface.create_contact>(jsonstring);

                if (!myDeserializedClass.IsEmpty())
                {
                    FirstName = myDeserializedClass.FirstName;
                    LastName = myDeserializedClass.LastName;
                    BirthDate = myDeserializedClass.BirthDate;
                    Email = myDeserializedClass.Email;
                    Phone = myDeserializedClass.Phone;
                    if (!String.IsNullOrEmpty(FirstName) && !String.IsNullOrEmpty(LastName) && !String.IsNullOrEmpty(BirthDate) && !String.IsNullOrEmpty(Email) && !String.IsNullOrEmpty(Phone))
                    {
                        try
                        {
                            int i = 0;
                            using (SqlConnection sqlConnection = new SqlConnection(connection))
                            {
                                sqlConnection.Open();
                                using (SqlCommand command = new SqlCommand("insert into contacts_interface select @firstname, @lastname, @birthdate, @email, @phone"))
                                {
                                    command.CommandType = CommandType.Text;
                                    command.Parameters.Add("@firstname", SqlDbType.NVarChar, 400).Value = FirstName;
                                    command.Parameters.Add("@lastname", SqlDbType.NVarChar, 400).Value = LastName;
                                    command.Parameters.Add("@birthdate", SqlDbType.VarChar, 10).Value = BirthDate;
                                    command.Parameters.Add("@email", SqlDbType.VarChar, 40).Value = Email;
                                    command.Parameters.Add("@phone", SqlDbType.VarChar, 40).Value = Phone;
                                    command.Connection = sqlConnection;
                                    i = command.ExecuteNonQuery();
                                    if (i > 0)
                                    {
                                        Message = @"ok";
                                        ErrorCode = @"0";
                                    }
                                    else
                                    {
                                        Message = @"error while insertion";
                                        ErrorCode = @"1";
                                    }
                                }
                                sqlConnection.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            Message = @"error: " + ((!String.IsNullOrEmpty(ex.Message)) ? ex.Message : "unknown");
                            ErrorCode = @"1";
                        }
                    }
                    else
                    {
                        Message = @"some values are empty";
                        ErrorCode = @"1";
                    }
                }
                else
                {
                    Message = @"empty";
                    ErrorCode = @"1";
                }
            }
            else
            {
                Message = @"empty";
                ErrorCode = @"1";
            }
        }
    }
    catch (Exception ex)
    {
        Message = @"error: " + ((!String.IsNullOrEmpty(ex.Message)) ? ex.Message : "unknown");
        ErrorCode = @"1";
    }
    ContactsInterface.output_with_message_and_errorcode outputclass = new ContactsInterface.output_with_message_and_errorcode();
    outputclass.Message = Message;
    outputclass.ErrorCode = ErrorCode;
    //var outputjson = JsonSerializer.Serialize(outputclass);
    //context.Response.ContentType = "application/json";
    return outputclass;
});

var read_handler = async (HttpContext context, int? contact_id) =>
{
    string jsonstring = String.Empty;
    string? FirstName = String.Empty;
    string? LastName = String.Empty;
    string? BirthDate = String.Empty;
    string? Email = String.Empty;
    string? Phone = String.Empty;
    string ErrorCode = String.Empty;
    string Message = String.Empty;
    string SqlTxt = String.Empty;
    List<Dictionary<string, string>> contacts = new List<Dictionary<string, string>>();
    try
    {
        int i = 0;
        using (SqlConnection sqlConnection = new SqlConnection(connection))
        {
            sqlConnection.Open();
            if (String.IsNullOrEmpty(contact_id.ToString()))
            {
                SqlTxt = @"select * from contacts_interface";
            }
            else
            {
                SqlTxt = @"select * from contacts_interface where id = @contact_id";
            }
            using (SqlCommand command = new SqlCommand(SqlTxt))
            {
                command.CommandType = CommandType.Text;
                if (!String.IsNullOrEmpty(contact_id.ToString())) { command.Parameters.Add("@contact_id", SqlDbType.Int, -1).Value = contact_id; };
                command.Connection = sqlConnection;
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                Dictionary<string, string> theitems = new Dictionary<string, string>();
                foreach (DataRow Row in dt.Rows)
                {
                    theitems = new Dictionary<string, string>();
                    foreach (DataColumn column in Row.Table.Columns)
                    {
                        theitems.Add(column.ColumnName, Row[column.ColumnName].ToString());
                    }
                    contacts.Add(theitems);
                }
                if (contacts.Count > 0)
                {
                    Message = @"ok";
                    ErrorCode = @"0";
                }
                else
                {
                    Message = @"error: The result is empty";
                    ErrorCode = @"1";
                }
            }
            sqlConnection.Close();
        }
    }
    catch (Exception ex)
    {
        Message = @"error: " + ((!String.IsNullOrEmpty(ex.Message)) ? ex.Message : "unknown");
        ErrorCode = @"1";
    }

    ContactsInterface.read_contacts_output_with_message_and_errorcode outputclass = new ContactsInterface.read_contacts_output_with_message_and_errorcode();
    outputclass.Message = Message;
    outputclass.ErrorCode = ErrorCode;
    outputclass.Data = contacts;
    //var outputjson = JsonSerializer.Serialize(outputclass);
    //context.Response.ContentType = "application/json";
    return outputclass;
};
app.MapGet(@"/contacts/{contact_id:regex(^[1-9]\d*$)}", read_handler);
app.MapGet(@"/contacts", read_handler);

var edit_handler = async (HttpContext context, int? contact_id) =>
{
    string jsonstring = String.Empty;
    string? FirstName = String.Empty;
    string? LastName = String.Empty;
    string? BirthDate = String.Empty;
    string? Email = String.Empty;
    string? Phone = String.Empty;
    string ErrorCode = String.Empty;
    string Message = String.Empty;
    try
    {
        using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
        {
            jsonstring = await reader.ReadToEndAsync();
            if (jsonstring.Length > 0)
            {
                ContactsInterface.create_contact? myDeserializedClass = new ContactsInterface.create_contact();
                myDeserializedClass = JsonSerializer.Deserialize<ContactsInterface.create_contact>(jsonstring);

                if (!myDeserializedClass.IsEmpty())
                {
                    FirstName = myDeserializedClass.FirstName;
                    LastName = myDeserializedClass.LastName;
                    BirthDate = myDeserializedClass.BirthDate;
                    Email = myDeserializedClass.Email;
                    Phone = myDeserializedClass.Phone;
                    if (!String.IsNullOrEmpty(FirstName) && !String.IsNullOrEmpty(LastName) && !String.IsNullOrEmpty(BirthDate) && !String.IsNullOrEmpty(Email) && !String.IsNullOrEmpty(Phone))
                    {
                        try
                        {
                            int i = 0;
                            using (SqlConnection sqlConnection = new SqlConnection(connection))
                            {
                                sqlConnection.Open();
                                using (SqlCommand command = new SqlCommand("update contacts_interface set FirstName = @firstname, LastName = @lastname, BirthDate = @birthdate, Email = @email, Phone = @phone where id = @contact_id"))
                                {
                                    command.CommandType = CommandType.Text;
                                    command.Parameters.Add("@firstname", SqlDbType.NVarChar, 400).Value = FirstName;
                                    command.Parameters.Add("@lastname", SqlDbType.NVarChar, 400).Value = LastName;
                                    command.Parameters.Add("@birthdate", SqlDbType.VarChar, 10).Value = BirthDate;
                                    command.Parameters.Add("@email", SqlDbType.VarChar, 40).Value = Email;
                                    command.Parameters.Add("@phone", SqlDbType.VarChar, 40).Value = Phone;
                                    command.Parameters.Add("@contact_id", SqlDbType.Int, -1).Value = contact_id;
                                    command.Connection = sqlConnection;
                                    i = command.ExecuteNonQuery();
                                    if (i > 0)
                                    {
                                        Message = @"ok";
                                        ErrorCode = @"0";
                                    }
                                    else
                                    {
                                        Message = @"error while updating";
                                        ErrorCode = @"1";
                                    }
                                }
                                sqlConnection.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            Message = @"error: " + ((!String.IsNullOrEmpty(ex.Message)) ? ex.Message : "unknown");
                            ErrorCode = @"1";
                        }
                    }
                    else
                    {
                        Message = @"some values are empty";
                        ErrorCode = @"1";
                    }
                }
                else
                {
                    Message = @"empty";
                    ErrorCode = @"1";
                }
            }
            else
            {
                Message = @"empty";
                ErrorCode = @"1";
            }
        }
    }
    catch (Exception ex)
    {
        Message = @"error: " + ((!String.IsNullOrEmpty(ex.Message)) ? ex.Message : "unknown");
        ErrorCode = @"1";
    }
    ContactsInterface.output_with_message_and_errorcode outputclass = new ContactsInterface.output_with_message_and_errorcode();
    outputclass.Message = Message;
    outputclass.ErrorCode = ErrorCode;
    //var outputjson = JsonSerializer.Serialize(outputclass);
    //context.Response.ContentType = "application/json";
    return outputclass;
};
app.MapPut(@"/contacts/{contact_id:regex(^[1-9]\d*$)}", edit_handler);


var delete_handler = async (HttpContext context, int contact_id) =>
{
    string jsonstring = String.Empty;
    string? FirstName = String.Empty;
    string? LastName = String.Empty;
    string? BirthDate = String.Empty;
    string? Email = String.Empty;
    string? Phone = String.Empty;
    string ErrorCode = String.Empty;
    string Message = String.Empty;
    string SqlTxt = String.Empty;
    List<Dictionary<string, string>> contacts = new List<Dictionary<string, string>>();
    try
    {
        int i = 0;
        using (SqlConnection sqlConnection = new SqlConnection(connection))
        {
            sqlConnection.Open();
            SqlTxt = @"delete from contacts_interface where id = @contact_id";
            using (SqlCommand command = new SqlCommand(SqlTxt))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@contact_id", SqlDbType.Int, -1).Value = contact_id;
                command.Connection = sqlConnection;
                int j = command.ExecuteNonQuery();
                Message = @"ok";
                ErrorCode = @"0";
            }
            sqlConnection.Close();
        }
    }
    catch (Exception ex)
    {
        Message = @"error: " + ((!String.IsNullOrEmpty(ex.Message)) ? ex.Message : "unknown");
        ErrorCode = @"1";
    }

    ContactsInterface.output_with_message_and_errorcode outputclass = new ContactsInterface.output_with_message_and_errorcode();
    outputclass.Message = Message;
    outputclass.ErrorCode = ErrorCode;
    //var outputjson = JsonSerializer.Serialize(outputclass);
    //context.Response.ContentType = "application/json";
    return outputclass;
};
app.MapDelete(@"/contacts/{contact_id:regex(^[1-9]\d*$)}", delete_handler);

app.Run();

// GET    /contacts
// POST   /contacts
// GET    /contacts/{id}
// PUT    /cintacts/{id}
// DELETE /contacts/{id}