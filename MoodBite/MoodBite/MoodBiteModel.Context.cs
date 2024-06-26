﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MoodBite
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class MoodBiteEntities : DbContext
    {
        public MoodBiteEntities()
            : base("name=MoodBiteEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AccountCreationLog> AccountCreationLog { get; set; }
        public virtual DbSet<Allergy> Allergy { get; set; }
        public virtual DbSet<BuyPremiumTransaction> BuyPremiumTransaction { get; set; }
        public virtual DbSet<Cart> Cart { get; set; }
        public virtual DbSet<FoodCategory> FoodCategory { get; set; }
        public virtual DbSet<FoodSale> FoodSale { get; set; }
        public virtual DbSet<Intolerance> Intolerance { get; set; }
        public virtual DbSet<Mood> Mood { get; set; }
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }
        public virtual DbSet<OrderMaster> OrderMaster { get; set; }
        public virtual DbSet<OrderPayment> OrderPayment { get; set; }
        public virtual DbSet<Premium> Premium { get; set; }
        public virtual DbSet<Recipe> Recipe { get; set; }
        public virtual DbSet<RecipeAllergy> RecipeAllergy { get; set; }
        public virtual DbSet<RecipeImage> RecipeImage { get; set; }
        public virtual DbSet<RecipeIngredient> RecipeIngredient { get; set; }
        public virtual DbSet<RecipeIntolerance> RecipeIntolerance { get; set; }
        public virtual DbSet<RecipeRating> RecipeRating { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Testimonials> Testimonials { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserPremium> UserPremium { get; set; }
        public virtual DbSet<UserRecipe> UserRecipe { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<UsersFavoriteRecipes> UsersFavoriteRecipes { get; set; }
        public virtual DbSet<vw_AllRecipeDetailsWithFoodCategoryName> vw_AllRecipeDetailsWithFoodCategoryName { get; set; }
        public virtual DbSet<vw_AllUserColWithRecipeID> vw_AllUserColWithRecipeID { get; set; }
        public virtual DbSet<vw_CartView> vw_CartView { get; set; }
        public virtual DbSet<vw_CheckOutView> vw_CheckOutView { get; set; }
        public virtual DbSet<vw_CoverImageOfRecipes> vw_CoverImageOfRecipes { get; set; }
        public virtual DbSet<vw_FilterAllergy> vw_FilterAllergy { get; set; }
        public virtual DbSet<vw_FilterFC> vw_FilterFC { get; set; }
        public virtual DbSet<vw_FoodSaleView> vw_FoodSaleView { get; set; }
        public virtual DbSet<vw_ForOrderPaymentInsertionView> vw_ForOrderPaymentInsertionView { get; set; }
        public virtual DbSet<vw_IncomingOrderView> vw_IncomingOrderView { get; set; }
        public virtual DbSet<vw_ManageSubscriptions> vw_ManageSubscriptions { get; set; }
        public virtual DbSet<vw_ManageUploads> vw_ManageUploads { get; set; }
        public virtual DbSet<vw_ManageUsers> vw_ManageUsers { get; set; }
        public virtual DbSet<vw_OrderDetailView> vw_OrderDetailView { get; set; }
        public virtual DbSet<vw_RecipeDetailsWithoutIngredients> vw_RecipeDetailsWithoutIngredients { get; set; }
        public virtual DbSet<vw_RecipeDetailsWithoutIngredientsWithRating> vw_RecipeDetailsWithoutIngredientsWithRating { get; set; }
        public virtual DbSet<vw_RecipeReadMoreStocksView> vw_RecipeReadMoreStocksView { get; set; }
        public virtual DbSet<vw_recipeUploadersNameWithRating> vw_recipeUploadersNameWithRating { get; set; }
        public virtual DbSet<vw_RecipeWithMoodName> vw_RecipeWithMoodName { get; set; }
        public virtual DbSet<vw_RecommendedRecipeForMood> vw_RecommendedRecipeForMood { get; set; }
        public virtual DbSet<vw_RoleView> vw_RoleView { get; set; }
        public virtual DbSet<vw_UploaderDetailsForReadMorePage> vw_UploaderDetailsForReadMorePage { get; set; }
        public virtual DbSet<vw_UserDetailsWithRole> vw_UserDetailsWithRole { get; set; }
        public virtual DbSet<vw_UserRecipeView> vw_UserRecipeView { get; set; }
        public virtual DbSet<vw_UsersUploadCounts> vw_UsersUploadCounts { get; set; }
        public virtual DbSet<vw_MyFavoritesView> vw_MyFavoritesView { get; set; }
        public virtual DbSet<vw_StarterUser> vw_StarterUser { get; set; }
    
        public virtual int sp_alterdiagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_alterdiagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_creatediagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_creatediagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_dropdiagram(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_dropdiagram", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_FilterSearchByAllergy_Result> sp_FilterSearchByAllergy(string chosenMood, string recipeNameKeyword, string allergyname)
        {
            var chosenMoodParameter = chosenMood != null ?
                new ObjectParameter("chosenMood", chosenMood) :
                new ObjectParameter("chosenMood", typeof(string));
    
            var recipeNameKeywordParameter = recipeNameKeyword != null ?
                new ObjectParameter("recipeNameKeyword", recipeNameKeyword) :
                new ObjectParameter("recipeNameKeyword", typeof(string));
    
            var allergynameParameter = allergyname != null ?
                new ObjectParameter("allergyname", allergyname) :
                new ObjectParameter("allergyname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_FilterSearchByAllergy_Result>("sp_FilterSearchByAllergy", chosenMoodParameter, recipeNameKeywordParameter, allergynameParameter);
        }
    
        public virtual ObjectResult<sp_FilterSearchByAllergyAndFoodCat_Result> sp_FilterSearchByAllergyAndFoodCat(string chosenMood, string recipeNameKeyword, string allergyname, string fcname)
        {
            var chosenMoodParameter = chosenMood != null ?
                new ObjectParameter("chosenMood", chosenMood) :
                new ObjectParameter("chosenMood", typeof(string));
    
            var recipeNameKeywordParameter = recipeNameKeyword != null ?
                new ObjectParameter("recipeNameKeyword", recipeNameKeyword) :
                new ObjectParameter("recipeNameKeyword", typeof(string));
    
            var allergynameParameter = allergyname != null ?
                new ObjectParameter("allergyname", allergyname) :
                new ObjectParameter("allergyname", typeof(string));
    
            var fcnameParameter = fcname != null ?
                new ObjectParameter("fcname", fcname) :
                new ObjectParameter("fcname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_FilterSearchByAllergyAndFoodCat_Result>("sp_FilterSearchByAllergyAndFoodCat", chosenMoodParameter, recipeNameKeywordParameter, allergynameParameter, fcnameParameter);
        }
    
        public virtual ObjectResult<sp_FilterSearchByFoodCat_Result> sp_FilterSearchByFoodCat(string chosenMood, string recipeNameKeyword, string fcname)
        {
            var chosenMoodParameter = chosenMood != null ?
                new ObjectParameter("chosenMood", chosenMood) :
                new ObjectParameter("chosenMood", typeof(string));
    
            var recipeNameKeywordParameter = recipeNameKeyword != null ?
                new ObjectParameter("recipeNameKeyword", recipeNameKeyword) :
                new ObjectParameter("recipeNameKeyword", typeof(string));
    
            var fcnameParameter = fcname != null ?
                new ObjectParameter("fcname", fcname) :
                new ObjectParameter("fcname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_FilterSearchByFoodCat_Result>("sp_FilterSearchByFoodCat", chosenMoodParameter, recipeNameKeywordParameter, fcnameParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagramdefinition_Result> sp_helpdiagramdefinition(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagramdefinition_Result>("sp_helpdiagramdefinition", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagrams_Result> sp_helpdiagrams(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagrams_Result>("sp_helpdiagrams", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_renamediagram(string diagramname, Nullable<int> owner_id, string new_diagramname)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var new_diagramnameParameter = new_diagramname != null ?
                new ObjectParameter("new_diagramname", new_diagramname) :
                new ObjectParameter("new_diagramname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_renamediagram", diagramnameParameter, owner_idParameter, new_diagramnameParameter);
        }
    
        public virtual int sp_upgraddiagrams()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_upgraddiagrams");
        }
    }
}
