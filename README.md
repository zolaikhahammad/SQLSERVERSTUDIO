# SQLConnection

Hi All,

I have developed a WPF application in .netcore 3.1 that connects to sql server management studion. You can connect through windows authentication or can provide the sql credentials. 
you have connected, it will show all the list of database in your sql server studio.

clicking on database it will show all the related tables

Basically the purpose of the application is to add two keywords to/from and update in the selected column of the selected table, and create a new copy of the selected table with the updated values of the selected column. Basically a new table will be created with a structure of the previous one, means exact cop of it. With all of primary and foreign keys relations.

You can also export the excel file report of the new table, how many rows are/aren't updated , what was the previous value in the column and what is after the updation.
