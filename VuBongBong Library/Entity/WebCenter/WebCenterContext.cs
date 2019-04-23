using System.Data.Entity;

namespace MainLibrary.Entity.WebCenter
{
    public class WebCenterContext : DbContext
    {
        public WebCenterContext()
            : base("WebCenterDatabase")
        {

        }


        public virtual DbSet<MenuItem> MenuItems { get; set; }
        public virtual DbSet<MenuManagement> MenuManagements { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User_Session> User_Sessions { get; set; }
        public virtual DbSet<FileManagement> FileManagements { get; set; }
        public virtual DbSet<Banner> Banners { get; set; }
        public virtual DbSet<WebInfomation> WebInfomations { get; set; }
        public virtual DbSet<TaskWeb> Tasks { get; set; }
        public virtual DbSet<Task_Type> Task_Types { get; set; }
        public virtual DbSet<Task_Details> Task_Detailss { get; set; }
        public virtual DbSet<Customer_Message> Customer_Messages { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<News> Newss { get; set; }
        public virtual DbSet<AlbumDetail> AlbumDetails { get; set; }
        public virtual DbSet<Customer_Feedback> Customer_Feedbacks { get; set; }
    }
}
