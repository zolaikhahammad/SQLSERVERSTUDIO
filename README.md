# SQL SERVER STUDIO (DESKTOP APPLICATION)

Created a desktop application using WPF in .NET Core 3.1 framework. Users who need to update a keyword in a specifc column or create a clone of a table with exact fields and contraints then this application is exactly what you are looking for. You may need to alter some code according to your needs.

Application Overview:

1) Connect to yout sql server studio (either by windows or sql server authentication)
2) List of all databases will be shown to you without actually opening the sql server studio itself
3) Select a database and all tables related to the selected database will be displayed
4) Select a table and click on the column that you want to update
5) Add two keyword from and to. **From** the existing word that exists in the column of the selected table and **to** to the keyword that you want to updated
6) Click on update then the values will be updated in the rows that contains that **from** keyword to the new values.
7) A new clone of a table with primary/foreign keys will be created with the updated values (Previous table will be kept as it was).
8) You can generate an excel report containing the information about how many rows were updated, what were the values before and after.
