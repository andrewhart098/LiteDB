using System;
using System.Linq;
using Xunit;
using LiteDB;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnitTest
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }

        public UserDomain Domain { get; set; }
    }

    public class UserDomain
    {
        public string DomainName { get; set; }
    }

    public class LinqTest
    {
        [Fact]
        public void Linq_Test()
        {
            LiteDB.BsonMapper.Global.UseLowerCaseDelimiter('_');

            using (var db = new LiteDatabase(DB.Path()))
            {
                var c1 = new User { Id = 1, Name = "Mauricio", Active = true, Domain = new UserDomain { DomainName = "Numeria" } };
                var c2 = new User { Id = 2, Name = "Malafaia", Active = false, Domain = new UserDomain { DomainName = "Numeria" } };
                var c3 = new User { Id = 3, Name = "Chris", Domain = new UserDomain { DomainName = "Numeria" } };
                var c4 = new User { Id = 4, Name = "Juliane" };

                var col = db.GetCollection<User>("Customer");

                col.EnsureIndex(x => x.Name, true);

                col.Insert(new User[] { c1, c2, c3, c4 });

                // sub-class
                Assert.Equal(3, col.Count(x => x.Domain.DomainName == "Numeria"));

                // == !=
                Assert.Equal(1, col.Count(x => x.Id == 1));
                Assert.Equal(3, col.Count(x => x.Id != 1));

                // member booleans
                Assert.Equal(3, col.Count(x => !x.Active));
                Assert.Equal(1, col.Count(x => x.Active));

                // methods
                Assert.Equal(1, col.Count(x => x.Name.StartsWith("mal")));
                Assert.Equal(1, col.Count(x => x.Name.Equals("Mauricio")));
                Assert.Equal(1, col.Count(x => x.Name.Contains("cio")));

                // > >= < <=
                Assert.Equal(1, col.Count(x => x.Id > 3));
                Assert.Equal(1, col.Count(x => x.Id >= 4));
                Assert.Equal(1, col.Count(x => x.Id < 2));
                Assert.Equal(1, col.Count(x => x.Id <= 1));

                // and/or
                Assert.Equal(1, col.Count(x => x.Id > 0 && x.Name == "MAURICIO"));
                Assert.Equal(2, col.Count(x => x.Name == "malafaia" || x.Name == "MAURICIO"));
            }
        }
    }
}
