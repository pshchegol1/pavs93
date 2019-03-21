using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

#region Additional namespaces
using northwindSystem.BLL;
using NorthwindSystem.BLL;
using NorthwindSystem.Data;
#endregion

namespace WebApp.SamplePages
{
    public partial class SqlProcQueries : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Clear old message
            MessageLabel.Text = "";

            //The dropdownlist (ddl) control will be loaded with data from the database
            //Consideration needs to be given to the data as to it change frequence
            //if your data does not change frequently then you can consider loading on page load

            if (!Page.IsPostBack)
            {
                //Use user friendly error handling

                try
                {
                    //Create and connect to the appropriate BLL class
                    CategoryController sysmgr = new CategoryController();
                    //Issue the request to the appropriate BLL class method and capture results
                    List<Category> datainfo = sysmgr.Category_List();
                    //Optionally: sort the results
                    datainfo.Sort((x, y) => x.CategoryName.CompareTo (y.CategoryName));

                    //attach data source collection to ddl
                    CategoryList.DataSource = datainfo;
                    //set ddl DataTextField and DataValueField properties
                    CategoryList.DataTextField = nameof(Category.CategoryName);
                    CategoryList.DataValueField = "CategoryID";
                    //Physically bind the data to the ddl control
                    CategoryList.DataBind();
                    //Optionally: add a prompt to the ddl control
                    CategoryList.Items.Insert(0, "select...");

                }
                catch (Exception ex)
                {
                    MessageLabel.Text = ex.Message;
                }
            }
        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            //Ensure selection was made
            if (CategoryList.SelectedIndex == 0)
            {
                //No selection:message to user
                MessageLabel.Text = "Select a product category to view products";
            }
            else
            {
                //Yes selection:process lookup
                //    useer friendly error handling
                try
                {

                    //create and connect to BLL class
                    ProductController sysmgr = new ProductController();
                    //Issue request for lookup to appropriate BLL class method
                    //And capture results
                    List<Product> datainfo = sysmgr.Product_GetByCategory(int.Parse(CategoryList.SelectedValue));
                  
                    //Check Results ( .Count() == 0)
                    if(datainfo.Count() == 0)
                    {
                        //No records: message to user
                        MessageLabel.Text = "No data found for selected category";
                        //Optionally: you may wish to remove from display any old data
                        CategoryProductList.DataSource = null;
                       
                    }
                    else
                    {
                        //Yes Records: Display data
                        CategoryProductList.DataSource = datainfo;
                        CategoryProductList.DataBind();
                    }


                }
                catch (Exception ex)
                {
                    MessageLabel.Text = ex.Message;
                }

            }



        }

        protected void Clear_Click(object sender, EventArgs e)
        {
            CategoryList.ClearSelection();
        }
    }
}