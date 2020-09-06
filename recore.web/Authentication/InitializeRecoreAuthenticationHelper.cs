using recore.db;
using recore.db.Commands;
using recore.db.FieldTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace recore.web.Authentication
{
    public class InitializeRecoreAuthenticationHelper
    {
        public static void Initialize(IDataService service)
        {
            RecordType userRecordType = new RecordType("recore_user", "recore_user")
            {
                Fields = new List<IFieldType>
                {
                    new NumberField("accessfailedcount",false),
                    new TextField("concurrencystamp",100,false),
                    new TextField("email", 300, false),
                    new BooleanField("emailconfirmed", false),
                    new BooleanField("lockoutenabled", false),
                    new TextField("passwordhash",100,false),
                    new TextField("phonenumber",100,true),
                    new BooleanField("phonenumberconfirmed",false),
                    new TextField("securitystamp", 100, false),
                    new BooleanField("twofactorenabled", false),
                    new TextField("username",200,false),
                }
            };
            service.Execute(new CreateRecordTypeCommand() { Target = userRecordType });
        }
    }
}
