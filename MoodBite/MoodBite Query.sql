CREATE DATABASE MoodBite

USE MoodBite

USE MASTER
DROP DATABASE MoodBite
SELECT * FROM [UserRole]
CREATE TABLE [User] (
	UserID INT IDENTITY(1,1) PRIMARY KEY,
	Username VARCHAR(100) UNIQUE,
	[Password] VARCHAR(100) UNIQUE,
	FirstName VARCHAR(100),
	LastName VARCHAR(100),
	[Address] VARCHAR(255),
	BirthDate DATE
)
USE MOODBITE

ALTER TABLE [User]
ADD Gender VARCHAR(100)

ALTER TABLE [User]
ADD Email VARCHAR(100)

ALTER TABLE [USER]
ADD CONSTRAINT UQ_UserEmail UNIQUE (Email);


CREATE TABLE [Role] (
	RoleID INT IDENTITY(1,1) PRIMARY KEY,
	Rolename VARCHAR(100)
)

CREATE TABLE UserRole (
	UserRoleID INT IDENTITY(1,1) PRIMARY KEY,
	UserID INT,
	RoleID INT,
	FOREIGN KEY(UserID) REFERENCES [User](UserID) ON DELETE CASCADE,
	FOREIGN KEY(RoleID) REFERENCES [Role](RoleID) ON DELETE CASCADE
)

CREATE TABLE Premium (
	PremiumID INT IDENTITY(1,1) PRIMARY KEY,
	PremiumType VARCHAR(100),
	Price DECIMAL,
	ExpiryDate Date
)

CREATE TABLE UserPremium (
	UserPremiumID INT IDENTITY(1,1) PRIMARY KEY,
	UserID INT,
	PremiumID INT,
	DateSubscribed Date
	FOREIGN KEY(UserID) REFERENCES [User](UserID) ON DELETE CASCADE,
	FOREIGN KEY(PremiumID) REFERENCES [Premium](PremiumID) ON DELETE CASCADE
)

CREATE TABLE Mood (
	MoodID INT IDENTITY(1,1) PRIMARY KEY,
	MoodName VARCHAR(100)
)

CREATE TABLE IngredientType (
	IngredientTypeID INT IDENTITY(1,1) PRIMARY KEY,
	IngredientTypeName VARCHAR(255)
)
SELECT * FROM IngredientType

DELETE FROM IngredientType

DBCC CHECKIDENT ('IngredientType', RESEED, 0);


INSERT INTO IngredientType (IngredientTypeName)
VALUES
    ('Dairy'),
    ('Meat'),
    ('Poultry'),
    ('Seafood'),
    ('Fruits'),
    ('Vegetables'),
    ('Grains'),
    ('Herbs and Spices'),
    ('Sweeteners'),
    ('Condiments and Sauces'),
    ('Oils and Fats'),
    ('Nuts and Seeds'),
    ('Legumes'),
    ('Beverages'),
    ('Dairy Alternatives'),
    ('Gluten-Free'),
    ('Organic'),
    ('Frozen'),
    ('Canned'),
    ('Miscellaneous');

CREATE TABLE Ingredient (
	IngredientID INT IDENTITY(1,1) PRIMARY KEY,
	IngredientTypeID INT,
	IngredientName VARCHAR(255)
	FOREIGN KEY(IngredientTypeID) REFERENCES IngredientType(IngredientTypeID) ON DELETE CASCADE
)

SELECT * FROM Ingredient
DELETE FROM Ingredient
DBCC CHECKIDENT ('Ingredient', RESEED, 0);

INSERT INTO Ingredient (IngredientTypeID, IngredientName)
VALUES
    (1, 'Milk'),
    (1, 'Cheese'),
    (1, 'Yogurt'),
    (1, 'Butter'),
    (1, 'Condensed Milk'),
    (1, 'Evaporated Milk'),
    (1, 'Cream'),
    (1, 'Cheese Spread'),
    (1, 'Sour Cream'),
    (1, 'Whipping Cream'),
    (2, 'Pork'),
    (2, 'Chicken'),
    (2, 'Beef'),
    (2, 'Sausage'),
    (2, 'Ham'),
    (2, 'Bacon'),
    (2, 'Ground Beef'),
    (2, 'Liver'),
    (3, 'Chicken'),
    (3, 'Turkey'),
    (3, 'Duck'),
    (3, 'Quail'),
    (3, 'Chicken Liver'),
    (3, 'Chicken Breast'),
    (3, 'Chicken Thighs'),
    (3, 'Chicken Wings'),
    (3, 'Chicken Drumsticks'),
    (3, 'Chicken Gizzard'),
    (4, 'Fish'),
    (4, 'Shrimp'),
    (4, 'Crab'),
    (4, 'Squid'),
    (4, 'Mussels'),
    (4, 'Clams'),
    (4, 'Tilapia'),
    (4, 'Bangus (Milkfish)'),
    (4, 'Tuna'),
    (4, 'Salmon'),
    (5, 'Banana'),
    (5, 'Mango'),
    (5, 'Pineapple'),
    (5, 'Watermelon'),
    (5, 'Coconut'),
    (5, 'Papaya'),
    (5, 'Guava'),
    (5, 'Avocado'),
    (5, 'Jackfruit'),
    (5, 'Lemon'),
    (6, 'Eggplant'),
    (6, 'Tomato'),
    (6, 'Onion'),
    (6, 'Garlic'),
    (6, 'Ginger'),
    (6, 'Green Beans'),
    (6, 'Okra'),
    (6, 'Bitter Melon'),
    (6, 'Cabbage'),
    (6, 'Spinach'),
    (7, 'Rice'),
    (7, 'Corn'),
    (7, 'Wheat Flour'),
    (7, 'Rice Flour'),
    (7, 'Bread'),
    (7, 'Noodles'),
    (7, 'Pasta'),
    (7, 'Oats'),
    (7, 'Barley'),
    (7, 'Couscous'),
    (8, 'Garlic'),
    (8, 'Onion'),
    (8, 'Ginger'),
    (8, 'Bay Leaves'),
    (8, 'Black Pepper'),
    (8, 'Salt'),
    (8, 'Paprika'),
    (8, 'Cumin'),
    (8, 'Turmeric'),
    (8, 'Chili Powder'),
    (9, 'Sugar'),
    (9, 'Honey'),
    (9, 'Molasses'),
    (9, 'Condensed Milk'),
    (9, 'Coconut Sugar'),
    (9, 'Palm Sugar'),
    (9, 'Stevia'),
    (9, 'Artificial Sweetener'),
    (9, 'Maple Syrup'),
    (9, 'Agave Syrup'),
    (10, 'Soy Sauce'),
    (10, 'Fish Sauce'),
    (10, 'Vinegar'),
    (10, 'Ketchup'),
    (10, 'Mayonnaise'),
    (10, 'Oyster Sauce'),
    (10, 'Barbecue Sauce'),
    (10, 'Hot Sauce'),
    (10, 'Worcestershire Sauce'),
   (10, 'Hoisin Sauce'),
    (10, 'Sesame Oil'),
    (11, 'Cooking Oil'),
    (11, 'Olive Oil'),
    (11, 'Coconut Oil'),
    (11, 'Palm Oil'),
    (11, 'Butter'),
    (11, 'Lard'),
    (11, 'Shortening'),
    (11, 'Ghee'),
    (11, 'Margarine'),
    (11, 'Vegetable Oil'),
    (12, 'Peanuts'),
    (12, 'Cashews'),
    (12, 'Almonds'),
    (12, 'Walnuts'),
    (12, 'Sesame Seeds'),
    (12, 'Pumpkin Seeds'),
    (12, 'Chia Seeds'),
    (12, 'Flaxseeds'),
    (12, 'Sunflower Seeds'),
    (12, 'Hazelnuts'),
    (13, 'Lentils'),
    (13, 'Chickpeas'),
    (13, 'Kidney Beans'),
    (13, 'Black Beans'),
    (13, 'Mung Beans'),
    (13, 'Soybeans'),
    (13, 'Pinto Beans'),
    (13, 'Adzuki Beans'),
    (13, 'Green Peas'),
    (13, 'Red Beans'),
    (14, 'Water'),
    (14, 'Coffee'),
    (14, 'Tea'),
    (14, 'Milk'),
    (14, 'Orange Juice'),
    (14, 'Apple Juice'),
    (14, 'Soda'),
    (14, 'Coconut Water'),
    (14, 'Energy Drink'),
    (14, 'Iced Tea'),
    (15, 'Soy Milk'),
    (15, 'Almond Milk'),
    (15, 'Coconut Milk'),
    (15, 'Oat Milk'),
    (15, 'Rice Milk'),
    (15, 'Cashew Milk'),
    (15, 'Hemp Milk'),
    (15, 'Pea Milk'),
    (15, 'Quinoa Milk'),
    (15, 'Flaxseed Milk'),
    (16, 'Rice Flour'),
    (16, 'Almond Flour'),
    (16, 'Coconut Flour'),
    (16, 'Corn Flour'),
    (16, 'Potato Flour'),
    (16, 'Tapioca Flour'),
    (16, 'Quinoa Flour'),
    (16, 'Buckwheat Flour'),
    (16, 'Sorghum Flour'),
    (16, 'Millet Flour'),
    (17, 'Organic Chicken'),
    (17, 'Organic Beef'),
    (17, 'Organic Eggs'),
    (17, 'Organic Milk'),
    (17, 'Organic Fruits'),
    (17, 'Organic Vegetables'),
    (17, 'Organic Grains'),
    (17, 'Organic Legumes'),
    (17, 'Organic Nuts'),
    (17, 'Organic Herbs'),
    (18, 'Frozen Vegetables'),
    (18, 'Frozen Fruits'),
    (18, 'Frozen Fish'),
    (18, 'Frozen Chicken'),
    (18, 'Frozen Shrimp'),
    (18, 'Frozen Pizza'),
    (18, 'Frozen French Fries'),
    (18, 'Frozen Dumplings'),
    (18, 'Frozen Desserts'),
    (18, 'Frozen Meatballs'),
    (19, 'Canned Tomatoes'),
    (19, 'Canned Tuna'),
    (19, 'Canned Corn'),
    (19, 'Canned Beans'),
    (19, 'Canned Pineapple'),
    (19, 'Canned Coconut Milk'),
    (19, 'Canned Soup'),
    (19, 'Canned Vegetables'),
    (19, 'Canned Fruit Cocktail'),
    (19, 'Canned Sardines'),
	(20, 'Ice Cubes');

SELECT * FROM Recipe
DELETE FROM Recipe WHERE RecipeID = 2

UPDATE Recipe SET RecipeName = 'RecipeNumber1' WHERE RecipeName IS NULL


ALTER TABLE Recipe
DROP CONSTRAINT FK__Recipe__RecipeIm__534D60F1

ALTER TABLE Recipe
DROP COLUMN RecipeImageID

DELETE FROM Recipe
DBCC CHECKIDENT ('Recipe', RESEED, 0);

CREATE TABLE Recipe (
	RecipeID INT IDENTITY(1,1) PRIMARY KEY,
	MoodID INT,
	RecipeName TEXT,
	IngredientsCount INT,
	CookingInstruction TEXT,
	PreparationTime TIME,
	CookingDuration TIME,
	DateUploaded DATETIME,
	IsApproved BIT DEFAULT 0,
	DateApproved DATETIME,
	ApprovedBy INT,
	FOREIGN KEY(MoodID) REFERENCES Mood(MoodID) ON DELETE CASCADE,
	FOREIGN KEY(ApprovedBy) REFERENCES [User](UserID) ON DELETE CASCADE
)

ALTER TABLE Recipe
ADD RecipeName TEXT

ALTER TABLE Recipe
ADD IngredientsCount INT

ALTER TABLE Recipe
ALTER COLUMN CookingInstruction TEXT

ALTER TABLE Recipe
ADD CONSTRAINT DF_DateUploaded DEFAULT GETDATE() FOR DateUploaded;

CREATE TABLE RecipeImage (
	RecipeImageID INT IDENTITY(1,1) PRIMARY KEY,
	RecipeID INT,
	ImageName VARCHAR(255),
	ImageURL VARBINARY(MAX),
	FOREIGN KEY(RecipeID) REFERENCES Recipe(RecipeID) ON DELETE CASCADE
)

CREATE TABLE RecipeIngredient (
	RecipeIngredientID INT IDENTITY(1,1) PRIMARY KEY,
	RecipeID INT,
	IngredientID INT,
	Unit VARCHAR(100),
	Quantity INT,
	FOREIGN KEY(RecipeID) REFERENCES Recipe(RecipeID) ON DELETE CASCADE,
	FOREIGN KEY(IngredientID) REFERENCES Ingredient(IngredientID)
)

CREATE TABLE Allergy (
	AllergyID INT IDENTITY(1,1) PRIMARY KEY,
	AllergyName VARCHAR(100)
)

CREATE TABLE RecipeAllergy (
	RecipeAllergyID INT IDENTITY(1,1) PRIMARY KEY,
	RecipeID INT,
	AllergyID INT,
	FOREIGN KEY(RecipeID) REFERENCES Recipe(RecipeID) ON DELETE CASCADE,
	FOREIGN KEY(AllergyID) REFERENCES Allergy(AllergyID) ON DELETE CASCADE
)

CREATE TABLE Intolerance (
	IntoleranceID INT IDENTITY(1,1) PRIMARY KEY,
	IntoleranceName VARCHAR(100)
)

CREATE TABLE RecipeIntolerance (
	RecipeIntoleranceID INT IDENTITY(1,1) PRIMARY KEY,
	RecipeID INT,
	IntoleranceID INT,
	FOREIGN KEY(RecipeID) REFERENCES Recipe(RecipeID) ON DELETE CASCADE,
	FOREIGN KEY(IntoleranceID) REFERENCES Intolerance(IntoleranceID) ON DELETE CASCADE
)

CREATE TABLE UserRecipe (
	UserRecipeID INT IDENTITY(1,1) PRIMARY KEY,
	UserID INT,
	RecipeID INT,
	FOREIGN KEY(UserID) REFERENCES [User](UserID) ON DELETE CASCADE,
	FOREIGN KEY(RecipeID) REFERENCES Recipe(RecipeID)
)

CREATE TABLE FoodSale (
	FoodSaleID INT IDENTITY(1,1) PRIMARY KEY,
	UserRecipeID INT,
	UserPremiumID INT,
	Price MONEY,
	[Address] TEXT,
	FOREIGN KEY(UserRecipeID) REFERENCES UserRecipe(UserRecipeID) ON DELETE CASCADE,
	FOREIGN KEY(UserPremiumID) REFERENCES UserPremium(UserPremiumID)
)

CREATE TABLE OrderMaster (
	PO_ID INT IDENTITY(1,1) PRIMARY KEY,
	UserID INT,
	DateOrdered DATETIME,
	IsPaid BIT DEFAULT 0,
	FoodSaleID INT,
	FOREIGN KEY(UserID) REFERENCES [User](UserID) ON DELETE CASCADE,
	FOREIGN KEY(FoodSaleID) REFERENCES FoodSale(FoodSaleID) ON DELETE CASCADE
)

ALTER TABLE OrderMaster
ADD FoodSaleID INT;

ALTER TABLE OrderMaster
ADD CONSTRAINT FK_OrderMaster_FoodSaleID FOREIGN KEY (FoodSaleID)
REFERENCES FoodSale(FoodSaleID);

INSERT INTO OrderMaster
VALUES('5', GETDATE(), 'Babag1', 0)

INSERT INTO OrderDetail
VALUES(1, 3, 5, 380, 5*380)

SELECT * FROM OrderMaster om
INNER JOIN OrderDetail od
ON om.PO_ID = od.PO_ID

CREATE TABLE OrderDetail (
	OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
	PO_ID INT,
	FoodForSaleID INT,
	Quantity INT,
	TotalPrice MONEY,
	FOREIGN KEY(PO_ID) REFERENCES [OrderMaster](PO_ID) ON DELETE CASCADE
)

CREATE TABLE Cart (
	CartID INT IDENTITY(1,1) PRIMARY KEY,
	UserID INT,
	RecipeID INT,
	FoodSaleID INT,
	FOREIGN KEY(UserID) REFERENCES [User](UserID) ON DELETE CASCADE,
	FOREIGN KEY(RecipeID) REFERENCES Recipe(RecipeID),
	FOREIGN KEY(FoodSaleID) REFERENCES FoodSale(FoodSaleID)
)

CREATE TABLE OrderPayment (
	OrderPaymentID INT IDENTITY(1,1) PRIMARY KEY,
	OrderDetailID INT,
	AmountPaid MONEY,
	DatePaid DATETIME,
	FOREIGN KEY(OrderDetailID) REFERENCES [OrderDetail](OrderDetailID) ON DELETE CASCADE
)
GO

CREATE VIEW vw_UserDetailsWithRole
AS
SELECT u.userID, u.Username, u.Password, u.FirstName, u.LastName, u.Address, u.BirthDate, u.Gender, ur.UserRoleID, r.Rolename
FROM [User] u
INNER JOIN [UserRole] ur
ON u.UserID = ur.UserID
INNER JOIN [Role] r
ON ur.RoleID = r.RoleID

ALTER VIEW vw_RecipeWithMoodName
AS
SELECT r.*, m.MoodName
FROM Recipe r
INNER JOIN Mood m
ON r.MoodID = m.MoodID

DROP VIEW vw_RecipeDetailsWithMoodTag --dropped already

SELECT * FROM vw_IngredientsOfRecipe

ALTER VIEW vw_RecipeDetailsWithMoodTagSimplified
AS
SElECT ri.RecipeIngredientID, r.RecipeID, m.MoodName, r.RecipeName, i.IngredientName, ri.Quantity, ri.Unit, it.IngredientTypeName, r.PreparationTime, r.CookingDuration, r.CookingInstruction, r.DateUploaded, r.DateApproved, r.ApprovedBy
FROM RecipeIngredient ri
INNER JOIN Recipe r
ON ri.RecipeID = r.RecipeID
INNER JOIN Ingredient i
ON ri.IngredientID = i.IngredientID
INNER JOIN IngredientType it
ON it.IngredientTypeID = i.IngredientTypeID
INNER JOIN Mood m
ON m.MoodID = r.MoodID

DROP VIEW 

CREATE VIEW vw_IngredientWithType
AS
SELECT i.IngredientID, i.IngredientName, it.IngredientTypeName
FROM Ingredient i
INNER JOIN IngredientType it
ON i.IngredientTypeID = it.IngredientTypeID

SELECT * FROM Recipe

DELETE FROM Recipe
DBCC CHECKIDENT ('Recipe', RESEED, 0);

INSERT INTO Recipe (MoodID, RecipeName, IngredientsCount, CookingInstruction, PreparationTime, CookingDuration, DateUploaded, IsApproved, DateApproved, ApprovedBy)
VALUES
    (1, 'Creamy Chicken Pasta', 6, '1. Cook pasta according to package instructions. 2. In a separate pan, cook chicken until brown. 3. Add cream, cheese, and spices to the chicken. 4. Mix cooked pasta with the creamy chicken sauce. 5. Serve hot.', '00:15:00', '00:30:00', GETDATE(), 1, GETDATE(), NULL),
    (2, 'Grilled Salmon with Lemon', 4, '1. Preheat grill to medium heat. 2. Season salmon with salt, pepper, and lemon juice. 3. Grill salmon for 6-8 minutes on each side. 4. Serve with a side of roasted vegetables.', '00:10:00', '00:20:00', GETDATE(), 1, GETDATE(), NULL),
    (3, 'Banana Smoothie', 3, '1. In a blender, combine bananas, milk, and honey. 2. Blend until smooth. 3. Add ice cubes and blend again. 4. Serve chilled.', '00:05:00', '00:05:00', GETDATE(), 0, NULL, NULL);


DELETE FROM RecipeIngredient
DBCC CHECKIDENT ('RecipeIngredient', RESEED, 0);

INSERT INTO RecipeIngredient (RecipeID, IngredientID, Unit, Quantity)
VALUES
    (1, 65, 'g', 250), 
	(1, 12, 'g', 300), 
	(1, 7, 'ml', 250), 
	(1, 2, 'g', 100), 
	(1, 74, 'g', 10), 
	(1, 73, 'g', 5),
    (2, 38, 'g', 200), 
	(2, 48, 'g', 20), 
	(2, 74, 'g', 5), 
	(2, 73, 'g', 5),
    (3, 39, 'g', 150), 
	(3, 133, 'ml', 250), 
	(3, 80, 'g', 15), 
	(3, 194, 'unit', 5);



SELECT * FROM vw_RecipeDetailsWithMoodTagSimplified

SELECT * FROM Ingredient

ALTER VIEW vw_IngredientsOfRecipe
AS
SELECT r.RecipeID, r.RecipeName, i.IngredientName
FROM RecipeIngredient ri
INNER JOIN Recipe r
ON ri.RecipeID = r.RecipeID
INNER JOIN Ingredient i
ON ri.IngredientID = i.IngredientID
INNER JOIN IngredientType it
ON it.IngredientTypeID = i.IngredientTypeID
INNER JOIN Mood m
ON m.MoodID = r.MoodID

SELECT * FROM vw_RecipeDetailsWithMoodTagSimplified


CREATE VIEW vw_RecommendedRecipeForMood
AS
SELECT r.RecipeID, r.RecipeName, m.MoodName
FROM Recipe r
INNER JOIN Mood m
ON r.MoodID = m.MoodID

SELECT COUNT(r.RecipeID), r.RecipeName, i.IngredientName
FROM RecipeIngredient ri
INNER JOIN Recipe r
ON ri.RecipeID = r.RecipeID
INNER JOIN Ingredient i
ON ri.RecipeIngredientID = i.IngredientID
GROUP BY r.RecipeID

SELECT * FROM vw_RecommendedRecipeForMood

SELECT * FROM vw_IngredientsOfRecipe

SELECT *
FROM vw_IngredientsOfRecipe
WHERE RecipeID = 1

SELECT * FROM vw_RecommendedRecipeForMood

SELECT * FROM vw_RecipeDetailsWithoutIngredients

ALTER VIEW vw_RecipeDetailsWithoutIngredients
AS
SELECT m.MoodName, r.RecipeID, r.RecipeName, r.RecipeDescription, r.CookingInstruction, r.PreparationTime, r.CookingDuration, r.DateUploaded, r.IsApproved, r.DateApproved, r.ApprovedBy, r.Rating, ri.RecipeImageID, ri.ImageName, ri.ImageURL
FROM dbo.Recipe r 
INNER JOIN dbo.Mood m 
ON r.MoodID = m.MoodID
INNER JOIN RecipeImage ri
ON ri.RecipeID = r.RecipeID
WHERE r.IsApproved = 1

SELECT * FROM Recipe

	
SELECT ISNULL(lastname, 'NA'), ISNULL(firstname,'NA'), ISNULL(phone, 'NA')
FROM CUSTOMER
ORDER BY lastname;

SELECT MIN(zipcode), MAX(zipcode)
FROM Customer
WHERE Location LIKE 'NY'

SELECT *
FROM vw_RecipeDetailsWithMoodTagSimplified
WHERE NOT MoodName = 'Sad'

SELECT MIN(Quantity), MAX(Quantity)
FROM vw_RecipeDetailsWithMoodTagSimplified
WHERE MoodName LIKE 'Happy'

CREATE DATABASE EXAM

USE EXAM

CREATE TABLE Customer (
	customer_id INT IDENTITY(1,1) PRIMARY KEY,
	first_name VARCHAR(255) NOT NULL,
	last_name VARCHAR(255) NOT NULL,
	phone VARCHAR(25),
	email VARCHAR(255) not null,
	street varchar(255),
	city varchar(50),
	[state] varchar(50),
	zip_code varchar(5)
)

alter PROCEDURE sp_InsertDataIntoCustomer @fname varchar(255), @lname varchar(255), @phone varchar(25), @email varchar(255), @street varchar(255), @city varchar(50), @state varchar(50), @zipcode varchar(5)
AS
INSERT INTO Customer
VALUES(@fname, @lname, @phone, @email, @street, @city, @state, @zipcode);

exec sp_InsertDataIntoCustomer 'lord', 'lagare', '09658916049', 'l@mail.com', 'babag 1', 'lapu-lapu', 'cebu', '6015'

select * from customer

select
	product_name,
	case
		when model_year = 2016 then 'year of the monkey'
		when model_year = 2017 then 'year of the roster'
		when model_year = 2018 then 'year of the dog'
		when model_year = 2019 then 'year of the pig'
	end as 'year'
from product
USE MoodBite
select * from [User]
SELECT * FROM RecipeIngredient
DELETE FROM [User]
DBCC CHECKIDENT ('UserRole', RESEED, 0);

SELECT * FROM vw_IngredientsOfRecipe

SELECT * FROM vw_RecipeWithMoodName

SELECT * FROM Recipe

SELECT * FROM UserRecipe

SELECT * FROM vw_RecipeDetailsWithMoodTagSimplified

ALTER VIEW vw_RecipeWithMoodName
AS
SELECT r.*, m.MoodName, u.userID, CONCAT(u.FirstName, ' ' ,u.LastName) as 'Uploaded by'
FROM Recipe r
INNER JOIN Mood m
ON r.MoodID = m.MoodID
INNER JOIN UserRecipe ur
ON r.RecipeID = ur.RecipeID
INNER JOIN [User] u
ON ur.UserID = u.userID

SELECT * FROM vw_RecommendedRecipeForMood

CREATE PROCEDURE sp_InsertRecipeImage
    @RecipeID INT,
    @ImageName VARCHAR(255),
    @ImagePath NVARCHAR(MAX)
AS
BEGIN
    DECLARE @ImageData VARBINARY(MAX)
    DECLARE @Sql NVARCHAR(MAX)

    -- Dynamic SQL to read image data
    SET @Sql = 'SELECT @ImageData = BulkColumn FROM OPENROWSET(BULK ''' + @ImagePath + ''', SINGLE_BLOB) AS ImageData'
    EXEC sp_executesql @Sql, N'@ImageData VARBINARY(MAX) OUTPUT', @ImageData OUTPUT

    -- Insert into RecipeImage table
    INSERT INTO RecipeImage (RecipeID, ImageName, ImageURL)
    VALUES (@RecipeID, @ImageName, @ImageData)
END

SELECT * FROM vw_RecommendedRecipeForMood
SELECT * FROM RecipeImage

SELECT * FROM Recipe

sp_InsertRecipeImage 1, 'Creamy Chicken Pasta Cover', 'C:\Users\Windows 10\Documents\GitHub\MoodBiteWebApp\MoodBite\MoodBite\Content\RecipeImages\creamy-chicken-pasta.jpg'

sp_InsertRecipeImage 2, 'Grilled Salmon with Lemon Cover', 'C:\Users\Windows 10\Documents\GitHub\MoodBiteWebApp\MoodBite\MoodBite\Content\RecipeImages\grilled-salmon-with-lemon.jpg'

sp_InsertRecipeImage 3, 'Banana Smoothie Cover', 'C:\Users\Windows 10\Documents\GitHub\MoodBiteWebApp\MoodBite\MoodBite\Content\RecipeImages\banana-smoothie.jpg'

SELECT * FROM RecipeIngredient
SELECT * FROM vw_RecommendedRecipeForMood
SELECT * FROM vw_IngredientsOfRecipe
DROP VIEW vw_IngredientsOfRecipe --dropped already


CREATE VIEW vw_CoverImageOfRecipes
AS
SELECT ri.RecipeImageID, r.RecipeID, ri.ImageName, ri.ImageURL, r.RecipeName, m.MoodName,
	(SELECT CONCAT(u.FirstName, ' ', u.LastName)
	FROM [User] u
	INNER JOIN UserRecipe ur
	ON u.userID = ur.UserID
	INNER JOIN Recipe rForUB
	ON rForUB.RecipeID = ur.RecipeID
	WHERE rForUB.RecipeID = r.RecipeID
	) as 'Uploaded by'
FROM RecipeImage ri
INNER JOIN Recipe r
ON ri.RecipeID = r.RecipeID
INNER JOIN Mood m
ON r.MoodID = m.MoodID

SELECT * FROM Recipe
SELECT * FROM UserRecipe

USE MoodBite

SELECT * 
FROM vw_IngredientsOfRecipe ir
WHERE RecipeID = 1

SELECT * FROM Recipe

ALTER TABLE Recipe
ADD RecipeDescription TEXT
USE MoodBite

SELECT * FROM vw_RecipeDetailsWithoutIngredients

SELECT * FROM RecipeIngredient

ALTER VIEW vw_IngredientsOfRecipe
AS
SELECT r.RecipeID, r.RecipeName, CONCAT(ri.Quantity, ' ', ri.Unit, ' ', i.IngredientName) 'Ingredient', ri.Quantity, ri.Unit, i.IngredientName
FROM RecipeIngredient ri
INNER JOIN Recipe r
ON ri.RecipeID = r.RecipeID
INNER JOIN Ingredient i
ON ri.IngredientID = i.IngredientID
INNER JOIN IngredientType it
ON it.IngredientTypeID = i.IngredientTypeID
INNER JOIN Mood m
ON m.MoodID = r.MoodID

USE MoodBite
ALTER TABLE Recipe
ALTER COLUMN Rating FLOAT

SELECT * FROM vw_RecipeDetailsWithoutIngredients

CREATE TABLE RecipeRating (
	RecipeRatingID INT IDENTITY(1,1) PRIMARY KEY,
	UserID INT,
	RecipeID INT,
	Rate FLOAT,
	FOREIGN KEY(UserID) REFERENCES [User](userID) ON DELETE CASCADE,
	FOREIGN KEY(RecipeID) REFERENCES Recipe(RecipeID)
)

CREATE VIEW vw_RecipeRating
AS
SELECT rr.RecipeRatingID, rr.RecipeID, AVG(rr.Rate) 'Rating', COUNT(rr.Rate) 'TotalReview'
FROM RecipeRating rr
INNER JOIN Recipe r
ON rr.RecipeID = r.RecipeID
INNER JOIN [User] u
ON rr.UserID = u.userID
GROUP BY rr.RecipeRatingID, rr.RecipeID


SELECT * FROM RecipeRating

CREATE VIEW vw_RecipeDetailsWithoutIngredientsWithRating
AS
SELECT m.MoodName, r.RecipeID, CAST(r.RecipeName as VARCHAR) 'RecipeName', CAST(r.RecipeDescription AS VARCHAR(MAX)) 'RecipeDescription', CAST(r.CookingInstruction AS varchar(MAX)) 'CookingInstruction', r.PreparationTime, r.CookingDuration, r.DateUploaded, r.IsApproved, r.DateApproved, r.ApprovedBy, AVG(rr.Rate) 'Rating', COUNT(rr.Rate) 'TotalReview', ri.RecipeImageID, ri.ImageName, ri.ImageURL
FROM dbo.Recipe r 
INNER JOIN dbo.Mood m 
ON r.MoodID = m.MoodID
INNER JOIN RecipeImage ri
ON ri.RecipeID = r.RecipeID
INNER JOIN RecipeRating rr
ON rr.RecipeID = r.RecipeID
GROUP BY r.RecipeID, m.MoodName, CAST(r.RecipeName as VARCHAR), CAST(r.RecipeDescription as varchar(MAX)), CAST(r.CookingInstruction AS varchar(MAX)), r.PreparationTime, r.CookingDuration, r.DateUploaded, r.IsApproved, r.DateApproved, r.ApprovedBy, ri.RecipeImageID, ri.ImageName, ri.ImageURL

CREATE TABLE AccountCreationLog (
	AccountCreationLogID INT IDENTITY(1,1) PRIMARY KEY,
	UserID INT,
	DateCreated DATETIME,
	FOREIGN KEY(UserID) REFERENCES [User](UserID) ON DELETE CASCADE
)

CREATE TRIGGER tr_InsertToAccountCreationLog
ON [dbo].[User]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserID INT;
    DECLARE @DateCreated DATETIME;

    SELECT @UserID = UserID, @DateCreated = GETDATE()
    FROM inserted;

    INSERT INTO AccountCreationLog (UserID, DateCreated)
    VALUES (@UserID, @DateCreated);
END;

SELECT * FROM [User]
SELECT * FROM AccountCreationLog

INSERT INTO [User] 
VALUES('triggertest', 'trigger', 'test', 'trigger', 'trg', '2024-04-03', 'Male', 'trg@tr.tr');

CREATE VIEW vw_UploaderDetailsForReadMorePage
AS
SELECT CONCAT(u.FirstName, ' ', u.LastName) 'UploadedBy', ISNULL(rating.Rating, 0) 'Rating', ISNULL(rating.TotalReview, 0) 'TotalReview'
FROM UserRecipe ur
INNER JOIN [User] u
ON ur.UserID = u.userID
LEFT JOIN vw_RecipeDetailsWithoutIngredientsWithRating rating
ON rating.RecipeID = ur.RecipeID

GROUP BY u.FirstName, u.LastName


SELECT * FROM vw_RecipeDetailsWithoutIngredientsWithRating
SELECT * FROM RecipeRating

SELECT ur.UserID, COUNT(RecipeID) 'Recipe Uploaded'
FROM UserRecipe ur
GROUP BY ur.UserID

USE MoodBite

CREATE VIEW vw_UploadersInfo
AS
SELECT	CONCAT(u.FirstName, ' ', u.LastName) 'UploadedBy', COUNT(ur.RecipeID) 'TotalUploads'
FROM UserRecipe ur
INNER JOIN [User] u
ON ur.UserID = u.userID
INNER JOIN Recipe r
ON r.RecipeID = ur.RecipeID
LEFT JOIN vw_RecipeDetailsWithoutIngredientsWith

Rating rd
ON rd.RecipeID = r.RecipeID
GROUP BY u.FirstName, u.LastName

SELECT * FROM RecipeRating
SELECT * FROM Recipe

ALTER VIEW vw_recipeUploadersNameWithRating
AS
SELECT ur.UserRecipeID, u.userID, CONCAT(u.FirstName, ' ', u.LastName) 'UploadedBy', CAST(r.RecipeName AS VARCHAR) 'RecipeName', ISNULL(AVG(rr.Rate), 0) 'Rating', COUNT(rr.RecipeID) 'TotalReview', acl.DateCreated
FROM UserRecipe ur
INNER JOIN [User] u
ON ur.UserID = u.userID
INNER JOIN Recipe r
ON r.RecipeID = ur.RecipeID
LEFT JOIN RecipeRating rr
ON rr.RecipeID = r.RecipeID
INNER JOIN AccountCreationLog acl
ON acl.UserID = u.userID
GROUP BY u.userID, u.FirstName, u.LastName, CAST(r.RecipeName AS VARCHAR), ur.UserRecipeID, acl.DateCreated

CREATE VIEW vw_UsersUploadCounts
AS
SELECT u.userID, COUNT(ur.RecipeID) 'TotalUpload'
FROM UserRecipe ur
INNER JOIN [User] u
ON ur.UserID = u.userID
GROUP BY u.userID

SELECT * FROM Mood
SELECT * FROM Recipe


DBCC CHECKIDENT ('Recipe', RESEED, 3);

CREATE VIEW vw_ManageSubscriptions
AS
SELECT up.UserPremiumID 'ID', u.FirstName 'First name', u.LastName 'Last name', p.PremiumType 'Premium type', up.DateSubscribed 'Date of Subscription', DATEADD(DAY, p.Duration, up.DateSubscribed) 'Expiry date' 
FROM UserPremium up
INNER JOIN [User] u
ON up.UserID = u.userID
INNER JOIN Premium p
ON p.PremiumID = up.PremiumID

--UserPremium, User, Premium
 

 --, r.DateApproved 'Date approved', (SELECT CONCAT(u.FirstName, ' ', u.LastName) FROM [User] u WHERE r.ApprovedBy = u.userID) 'Approved by'
ALTER VIEW vw_ManageUploads
AS
SELECT r.RecipeID 'ID', r.RecipeName, CONCAT(u.FirstName, ' ', u.LastName) 'Uploaded by', r.DateUploaded 'Date uploaded', 
	CASE 
		WHEN r.DateApproved IS NOT NULL AND r.IsApproved = 0 THEN 'Rejected'
		WHEN r.IsApproved = 0 THEN 'Pending'
		WHEN r.IsApproved = 1 THEN 'Approved'
	END 'Status',
	r.DateApproved
FROM Recipe r
INNER JOIN UserRecipe ur
ON r.RecipeID = ur.RecipeID
INNER JOIN [User] u
ON ur.UserID = u.userID

ALTER VIEW vw_ManageUsers
AS
SELECT u.userID 'ID', u.Username 'Username', u.FirstName 'First name', u.LastName 'Last name', r.Rolename 'Role'
FROM [User] u
INNER JOIN UserRole ur
ON u.userID = ur.UserID
INNER JOIN Role r
ON r.RoleID = ur.RoleID

ALTER TABLE Premium
ADD Duration INT

USE MoodBITE

SELECT * FROM Recipe

SELECT * FROM UserRecipe

SELECT * FROM FoodSale

SELECT * FROM UserPremium

SELECT * FROM [User]

SELECT * FROM RecipeImage

SELECT * FROM RecipeIngredient

SELECT * FROM UserRole

SELECT * FROM [Role]

SELECT * FROM vw_RecommendedRecipeForMood


ALTER VIEW vw_RecommendedRecipeForMood
AS
SELECT r.RecipeID, r.RecipeName, fc.FoodCategoryName, m.MoodName, u.userID, CONCAT(u.FirstName, ' ' ,u.LastName) as 'Uploaded by', r.DateUploaded, 
	(SELECT CONCAT(u.FirstName, ' ' ,u.LastName)
	FROM [User] u 
	INNER JOIN  Recipe re
	ON u.userID = re.ApprovedBy
	WHERE re.RecipeID = r.RecipeID
	) as 'Approved by', 
	r.DateApproved, ri.RecipeImageID, ri.ImageName, ri.ImageURL
FROM Recipe r
INNER JOIN Mood m
ON r.MoodID = m.MoodID
INNER JOIN UserRecipe ur
ON r.RecipeID = ur.RecipeID
INNER JOIN [User] u
ON u.userID = ur.UserID
INNER JOIN RecipeImage ri
ON ri.RecipeID = r.RecipeID
INNER JOIN FoodCategory fc
ON r.FoodCategoryID = fc.FoodCategoryID
WHERE r.IsApproved = 1

SELECT * FROM vw_ManageUploads
SELECT * FROM UserRecipe
SELECT * FROM RecipeIngredient
USE MOODBITE

SELECT * FROM vw_RecipeDetailsWithoutIngredients

SELECT * FROM RecipeIngredient

SELECT * FROM Recipe

CREATE VIEW vw_RecipeFullDetails
AS 
SELECT r.RecipeID, r.RecipeName, r.
FROM Recipe r
INNER JOIN RecipeIngredient ri
ON r.RecipeID = ri.RecipeID
INNER JOIN Mood m
ON r.MoodID = m.MoodID

CREATE TABLE FoodCategory (
	FoodCategoryID INT IDENTITY(1,1) PRIMARY KEY,
	FoodCategoryName VARCHAR(100),
	FoodCategoryDescription VARCHAR(255)
)

ALTER TABLE Recipe
ADD CONSTRAINT FK_Recipe_FoodCategory
FOREIGN KEY (FoodCategoryID) REFERENCES FoodCategory(FoodCategoryID)

ALTER VIEW vw_RecipeDetailsWithoutIngredients
AS
SELECT m.MoodName, r.RecipeID, r.RecipeName, fc.FoodCategoryName, r.RecipeDescription, r.CookingInstruction, r.PreparationTime, r.CookingDuration, r.DateUploaded, r.IsApproved, r.DateApproved, r.ApprovedBy, ri.RecipeImageID, ri.ImageName, ri.ImageURL
FROM Recipe r 
INNER JOIN Mood m 
ON r.MoodID = m.MoodID 
INNER JOIN RecipeImage ri 
ON ri.RecipeID = r.RecipeID
INNER JOIN FoodCategory fc
ON r.FoodCategoryID = fc.FoodCategoryID
WHERE r.IsApproved = 1

USE MOODBITE

INSERT INTO Allergy (AllergyName)
VALUES ('Milk'),
       ('Eggs'),
       ('Fish'),
       ('Shellfish'),
       ('Tree nuts'),
       ('Peanuts'),
       ('Wheat'),
       ('Soy'),
       ('Sesame seeds'),
       ('Sulfites'),
       ('Mustard'),
       ('Lupin'),
       ('Celery'),
       ('Mollusks');

INSERT INTO Intolerance (IntoleranceName)
VALUES ('Lactose'),
       ('Gluten'),
       ('Fructose'),
       ('Histamine'),
       ('Caffeine'),
       ('Sulfite'),
       ('Soy'),
       ('Egg'),
       ('Shellfish'),
       ('Nut'),
       ('Dairy'),
       ('Wheat'),
       ('Corn');

UPDATE [User] 
SET ProfilePicture = (SELECT BulkColumn FROM OPENROWSET(BULK 'C:\Users\Windows 10\Pictures\me.jpg', SINGLE_BLOB) AS Image)
WHERE userId = 1


ALTER VIEW vw_AllUserColWithRecipeID
AS
SELECT u.*, ur.RecipeID
FROM [User] u
INNER JOIN UserRecipe ur
ON u.userID = ur.UserID

DELETE FROM Allergy

DBCC CHECKIDENT ('Allergy', RESEED, 0);


MERGE INTO Allergy AS target
USING (VALUES
    ('Peanuts'),
    ('Tree nuts'),
    ('Shellfish'),
    ('Fish'),
    ('Eggs'),
    ('Milk'),
    ('Soy'),
    ('Wheat'),
    ('Gluten'),
    ('Sesame'),
    ('Mustard'),
    ('Sulfites'),
    ('Crab'),
    ('Salmon'),
    ('Shrimp'),
    ('Lobster'),
    ('Tuna'),
    ('Cod'),
    ('Sardine'),
    ('Mussel'),
    ('Oyster'),
    ('Squid'),
    ('Scallops'),
    ('Haddock'),
    ('Trout'),
    ('Anchovy'),
    ('Catfish'),
    ('Halibut'),
    ('Herring'),
    ('Octopus'),
    ('Perch'),
    ('Pollock'),
    ('Snapper'),
    ('Sole'),
    ('Surimi'),
    ('Tilefish'),
    ('Whitefish'),
    ('Abalone'),
    ('Crayfish'),
    ('Eel'),
    ('Mahi Mahi'),
    ('Marlin'),
    ('Swordfish'),
    ('Turbot'),
    ('Apples'),
    ('Bananas'),
    ('Berries'),
    ('Mangoes'),
    ('Peaches'),
    ('Pineapples'),
    ('Kiwi'),
    ('Tomatoes'),
    ('Carrots'),
    ('Celery'),
    ('Potatoes'),
    ('Avocado'),
    ('Cucumber'),
    ('Bell peppers'),
    ('Soybeans'),
    ('Lentils'),
    ('Peas'),
    ('Pumpkin seeds'),
    ('Sunflower seeds'),
    ('Flaxseed'),
    ('Chia seeds'),
    ('Quinoa'),
    ('Buckwheat'),
    ('Barley'),
    ('Rye'),
    ('Oats'),
    ('Corn'),
    ('Rice'),
    ('Coconut')
) AS source (AllergyName)
ON target.AllergyName = source.AllergyName
WHEN NOT MATCHED THEN
    INSERT (AllergyName)
    VALUES (source.AllergyName);

UPDATE [User] 
SET ProfilePicture = (SELECT BulkColumn FROM OPENROWSET(BULK 'C:\Users\Windows 10\Pictures\noprofile-girl.jpg', SINGLE_BLOB) AS Image)
WHERE userId = 2

UPDATE [User] 
SET ProfilePicture = (SELECT BulkColumn FROM OPENROWSET(BULK 'C:\Users\Windows 10\Pictures\noprofile-boy.png', SINGLE_BLOB) AS Image)
WHERE userId = 5

CREATE VIEW vw_AllRecipeDetailsWithFoodCategoryName
AS
SELECT r.*, fc.FoodCategoryName
FROM Recipe r
INNER JOIN FoodCategory fc
ON r.FoodCategoryID = fc.FoodCategoryID

USE MoodBite

CREATE VIEW vw_FilterAllergy
AS
SELECT r.*, ri.IngredientName, ri.Quantity, ri.Unit, a.AllergyName
FROM Recipe r
INNER JOIN RecipeIngredient ri
    ON r.RecipeID = ri.RecipeID
INNER JOIN Allergy a
    ON ri.IngredientName LIKE CONCAT('%', a.AllergyName, '%');


SELECT *
FROM vw_RecommendedRecipeForMood vw
INNER JOIN RecipeIngredient ri
ON vw.RecipeID = ri.RecipeID
WHERE RecipeName LIKE '%Pasta%' AND MoodName = 'Happy' AND ri.IngredientName NOT IN('Pasta')


SELECT u.Username, u.Password, u.userID, up.PremiumID
FROM UserPremium up
INNER JOIN [User] u
ON up.UserID = u.userID

SELECT u.Username, u.Password, u.userID, up.PremiumID
FROM UserPremium up
RIGHT JOIN [User] u
ON up.UserID = u.userID

SELECT u.userID, u.Username, u.Password, upr.UserPremiumID, up.UserRecipeID, r.RecipeName
FROM [User] u
INNER JOIN UserRecipe up
ON u.userID = up.UserID
INNER JOIN UserPremium upr
ON u.userID = upr.UserID
INNER JOIN Recipe r
ON r.RecipeID = up.RecipeID
ORDER BY u.userID

SELECT * FROM Cart

ALTER VIEW vw_CartView
AS
SELECT  u.userID, r.RecipeID, fs.FoodSaleID, c.CartID, r.RecipeName, fs.Price, c.Qty, (fs.Price * c.Qty) 'Total Price'
FROM Cart c
INNER JOIN [User] u
ON c.UserID = u.userID
INNER JOIN Recipe r
ON c.RecipeID = r.RecipeID
INNER JOIN FoodSale fs
ON fs.FoodSaleID = c.FoodSaleID

SELECT * FROM FoodSale

SELECT * FROM Cart

CREATE VIEW vw_CheckOutView
AS
SELECT cv.*, ri.ImageURL
FROM vw_CartView cv
INNER JOIN Recipe r
ON cv.RecipeID = r.RecipeID
INNER JOIN RecipeImage ri
ON cv.RecipeID = ri.RecipeID

CREATE VIEW vw_OrderDetailView
AS
SELECT om.PO_ID, om.CustomerID, od.TotalPrice
FROM OrderDetail od
INNER JOIN OrderMaster om
ON od.PO_ID = om.PO_ID

SELECT * FROM Cart

SELECT * FROM OrderMaster
SELECT * FROM OrderDetail
SELECT * FROM OrderPayment
SELECT * FROM FoodSale

SELECT r.RecipeName, ur.UserRecipeID FROM UserRecipe ur
INNER JOIN Recipe r
ON ur.RecipeID = r.RecipeID

SELECT * FROM FoodSale

SELECT cv.*, ri.ImageURL
FROM vw_CartView cv
INNER JOIN Recipe r
ON cv.RecipeID = r.RecipeID
INNER JOIN RecipeImage ri
ON cv.RecipeID = ri.RecipeID
SELECT * FROM Cart

ALTER TABLE OrderPayment
DROP COLUMN CustomerID INT
USE MOODBITE


ALTER VIEW vw_ForOrderPaymentInsertionView
AS
SELECT od.*, om.CustomerID, om.CustomerAddress 'DeliveredTo', fs.Address 'DeliveredFrom', om.DateOrdered, om.IsPaid
FROM OrderDetail od 
INNER JOIN OrderMaster om
ON om.PO_ID = od.PO_ID
INNER JOIN FoodSale fs
ON fs.FoodSaleID = od.FoodForSaleID
WHERE om.IsPaid = 0

SELECT *
FROM OrderMaster
WHERE CustomerID = 5

UPDATE OrderMaster SET IsPaid = 1 WHERE CustomerID = 5(userinput)

SELECT *
FROM vw_FoodSaleView

ALTER VIEW vw_FoodSaleView
AS
SELECT fs.FoodSaleID, u.userID, u.Username, u.Password, ur.UserRecipeID, r.RecipeID, r.RecipeName, fs.Price, fs.Address, fs.Available
FROM FoodSale fs
INNER JOIN UserRecipe ur
ON fs.UserRecipeID = ur.UserRecipeID
INNER JOIN Recipe r
ON r.RecipeID = ur.RecipeID
INNER JOIN [User] u
ON u.userID = ur.UserID

SELECT * FROM [User]
SELECT * FROM UserRecipe

DELETE FROM [User] WHERE EmailConfirmed = 0


CREATE TABLE UsersFavoriteRecipes (
	UsersFaveRecipeID INT IDENTITY(1,1) PRIMARY KEY,
	UserID INT,
	RecipeID INT,
	FOREIGN KEY(UserID) REFERENCES [User](userID) ON DELETE CASCADE,
	FOREIGN KEY(RecipeID) REFERENCES Recipe(RecipeID)
)

SELECT * FROM vw_AllUserColWithRecipeID

SELECT * FROM UsersFavoriteRecipes

SELECT * FROM Recipe

USE MOODBITE