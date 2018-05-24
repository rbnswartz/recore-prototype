# recore-prototype
A prototype for recore

# What is this

recore is a XRM system that allows you to rapidly build data management systems. It is meant
to be expanded and built upon. Everything from the forms, and views, to db tables is created based upon user data.

THIS IS STILL A PROTOTYPE. You are free to use it (and let me know what you like and dislike) but it has still not gotten a lot of the features and functionality that I think it needs to be production ready

# Prerequisists 

- Postgressql
- dotnet core

Also for the time being a postgres database needs to be created as well as a user with full access to the db

# Building
Can be built using the standard .net core build process. In a terminal type `dotnet build`

# Running

Move to the recore.web directory
Modify the appsettings.json to add a connection string to your postgres database
In a terminal run `dotnet run` inside of the recore.web directory
Issue a GET request to http://localhost:5000/system/init In order to set up the system (if you are running this on a different port then substitute 5000 for that port)

# Getting involved
You want to get involved in the project!? The main thing that you could do is try to build a 
sample system using recore and let me know what worked well and what didn't work so well.
This is primarily being built to fulfill my needs (and the people around me) so getting
another opinion as to what they would like would be nice.

# WARNING
Again this is still a work in progress. Expect anything to change.