using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//This Class needs to have access to ADO.NET EntityFrameWork
//The Nuget package has alreadty be added to this project
//This project also needs the assembly System.Data.Entity
//This project will need using clauses that point to a) the System.Data.Entity namespace
//
#region
using System.Data.Entity;
using NorthwindSystem.Data;
#endregion
namespace northwindSystem.DAL
{
    //The class acces internal restrict calls to this class to methods within this project
    //This context class needs to inherit DbContext from EntityFramework
    internal class NorthwindContext:DbContext
    {
        //Setup your class default constructor to supply your connection string name to the DbContext inherit class
        public NorthwindContext() : base("NWDB")
        {

        }

        //Create EntityFramework DbSet<T> for each mapped SQL table
        //<T> is your class in the .Data project
        public DbSet<Product> Products { get; set; }//if you have one class - one DbSet, 10 classes - 10 DbSet

        public DbSet<Category> Categories { get; set; }
    }
}
