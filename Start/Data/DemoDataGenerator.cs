using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace UserManagement.Data
{
    /// <summary>
    /// Provides methods for filling the database with demo data
    /// </summary>
    public class DemoDataGenerator
    {
        private readonly UserManagementDataContext dc;

        public DemoDataGenerator(UserManagementDataContext dc)
        {
            this.dc = dc;
        }

        /// <summary>
        /// Delete all data in the database
        /// </summary>
        /// <returns></returns>
        public async Task ClearAll()
        {
            dc.Users.RemoveRange(await dc.Users.ToArrayAsync());
            dc.Groups.RemoveRange(await dc.Groups.ToArrayAsync());
            await dc.SaveChangesAsync();
        }

        /// <summary>
        /// Fill database with demo data
        /// </summary>
        public async Task Fill()
        {
            #region Add some users
            User foo, john, jane;
            dc.Users.Add(foo = new User
            {
                NameIdentifier = "foo.bar",
                FirstName = "Foo",
                LastName = "Bar",
                Email = "foo.bar@acme.corp"
            });

            dc.Users.Add(john = new User
            {
                NameIdentifier = "john.doe",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@acme.corp"
            });

            dc.Users.Add(jane = new User
            {
                NameIdentifier = "jane.doe",
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@acme.corp"
            });
            #endregion

            #region Add some groups

            dc.Groups.Add(new Group()
            {
                Name = "IT",
                Users = new List<User>()
                {
                    foo,
                }
            });
            dc.Groups.Add(new Group()
            {
                Name = "Management",
                Users = new List<User>()
                {
                    john,
                    jane,
                }
            });

            var g2 = new Group() {Name = "Abc", Users = new List<User>() {jane}};
            var g1 = new Group() {Name = "Test", Users = new List<User>() {john}, ChildGroups = new List<Group>() {g2}};
            
            dc.Groups.Add(new Group() {
                Name = "Hallo", 
                Users = new List<User>() {foo},
                ChildGroups = new List<Group>() {g1}
            });
            #endregion

            await dc.SaveChangesAsync();
        }
    }
}
